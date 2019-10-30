using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AthensUtility;

namespace LHTRPG
{
    public enum UnitType
    {
        /// <summary> 冒険者 </summary>
        Adventurer,
        /// <summary> ゲスト(NPC、能力値やHPなどのデータを持つキャラクター) </summary>
        Guest,
        /// <summary> エキストラ(NPC、ルール城のデータを一切持たない演出上のキャラクター) </summary>
        Extra,
        /// <summary> エネミー </summary>
        Enemy,
        /// <summary> 行動 </summary>
        Action,
        /// <summary> 攻撃 </summary>
        Attack,
        /// <summary> アイテム </summary>
        Item,
        /// <summary> プロップ </summary>
        Prop,
    }

    [DebuggerDisplay("{GetTagString()}")]
    /// <summary> 全てのタグを持つ基礎クラス </summary>
    public abstract class Unit
    {
        /// <summary> ユニット種別 </summary>
        public UnitType Type { get; }

        /// <summary> キャラクターかどうか </summary>
        public bool IsCharacter => Type == UnitType.Adventurer || Type == UnitType.Guest || Type == UnitType.Enemy;

        /// <summary> セッションへの参照 </summary>
        public Session Session { get; set; } = null;

        /// <summary> 攻撃対象に選べるかどうか(キャラクター、もしくは破壊可能なプロップ) </summary>
        public bool IsTarget => IsCharacter || (Type == UnitType.Prop && ((this as Prop)?.CanBreak ?? false));

        /// <summary> 保持HP </summary>
        public virtual int HP { get; set; } = 0;

        /// <summary> ダメージを受ける処理 </summary>
        public abstract int Damage(EventPlayer evplayer, int damage, DamageType type, Unit fromUnit);

        /// <summary> 回復する処理 </summary>
        public abstract int Heal(EventPlayer evplayer, int heal, Unit fromUnit);

        /// <summary> ランク </summary>
        public int Rank { get; set; }

        /// <summary> 基礎保持タグ </summary>
        protected abstract IEnumerable<Tag> LTags { get; }

        /// <summary> 保持タグ </summary>
        public IEnumerable<Tag> Tags => LTags?.Concat(HaveStatus.OrderBy(s => (int)s.Status).Select(s => s as Tag)) ?? new List<Tag>();

        /// <summary> タグ文字列化 </summary>
        public string GetTagString() => Tags.Select(t => t.ToString()).Aggregate((now, next) => now + " " + next);

        protected Unit(UnitType type) { Type = type; }

        /// <summary> 属性タグ一覧 </summary>
        public IEnumerable<TagElement> Elements => Tags.GetTags<TagElement>().Distinct();

        /// <summary> 武器タグ一覧 </summary>
        public IEnumerable<TagWeapon> Weapons => Tags.GetTags<TagWeapon>().Distinct();

        /// <summary> ステータスタグ一覧 </summary>
        public LinkedList<IStatusTag> HaveStatus { get; } = new LinkedList<IStatusTag>();

        /// <summary> 特定のステータスタグの一覧をNodeで取得 </summary>
        private IEnumerable<LinkedListNode<IStatusTag>> GetStatusNodeList(Status status, Tag target = null)
        {
            if (target != null && status != Status.WeakPoint && status != Status.Mitigation)
                throw new Exception("Incorrect status.");
            var sts = HaveStatus.EnumerateNodes()
                .Where(x => x.Value.Status == status);
            if (status == Status.WeakPoint || status == Status.Mitigation)
                sts = sts.Where(x => (x.Value as TagStatusTarget)?.Target == target);
            return sts;
        }

        /// <summary> 特定のステータスタグの一覧を取得 </summary>
        public IEnumerable<IStatusTag> GetStatusList(Status status, Tag target = null)
            => GetStatusNodeList(status, target).Select(x => x.Value);

        /// <summary> 特定のステータスタグの一覧をキャストして取得 </summary>
        public IEnumerable<T> GetStatusList<T>(Status status, Tag target = null) where T : Tag, IStatusTag
            => GetStatusNodeList(status, target).Select(x => x.Value).Cast<T>();

        /// <summary> 特定のステータスタグが存在するかどうか </summary>
        public bool IsExistStatus(Status status, Tag target = null) => GetStatusNodeList(status, target).Any();

        /// <summary> 特定のステータスタグをNodeで取得 </summary>
        private LinkedListNode<IStatusTag> GetStatusNode(Status status, Tag target = null) => GetStatusNodeList(status, target).FirstOrDefault();

        /// <summary> 特定のステータスタグを取得 </summary>
        public IStatusTag GetStatus(Status status, Tag target = null) => GetStatusNode(status, target)?.Value;

        /// <summary> 特定のステータスタグをキャストして取得 </summary>
        public T GetStatus<T>(Status status, Tag target = null) where T : Tag, IStatusTag => GetStatus(status, target) as T;

        /// <summary> ステータスを与える </summary>
        /// <param name="status">ステータス種別</param>
        /// <param name="value">数値を持つステータスならその数値</param>
        /// <param name="target">軽減・弱点の対象</param>
        public void GiveStatus(EventPlayer evplayer, Status status, int value = 0, Tag target = null)
        {
            if (status.HasValue())
            {
                if (IsExistStatus(status, target))
                // 既に同じステータスを持っている場合
                {
                    var tag = GetStatus<TagStatusValue>(status, target);
                    switch (tag.Type)
                    {
                        // 加算タイプ
                        case TagStatusType.Add:
                            tag.Value += value;
                            break;
                        // 大きい方優先タイプ
                        case TagStatusType.Max:
                            tag.Value = Math.Max(tag.Value, value);
                            break;
                        // 重複可能タイプ
                        case TagStatusType.Overlap:
                            tag = TagStatus.MakeStatus(status, target) as TagStatusValue;
                            tag.Value = value;
                            HaveStatus.AddLast(tag);
                            break;
                        default:
                            throw new Exception("TagStatusValue Type is incorrect.");
                    }
                }
                else
                {
                    var tag = TagStatus.MakeStatus(status, target) as TagStatusValue;
                    tag.Value = value;
                    HaveStatus.AddLast(tag);
                }
            }
            else if (!IsExistStatus(status))
                HaveStatus.AddLast(TagStatus.MakeStatus(status, null, IsCharacter));
        }

        /// <summary> ステータスを取り除く(Node指定) </summary>
        public void RemoveStatus(EventPlayer evplayer, LinkedListNode<IStatusTag> statusNode) => HaveStatus.Remove(statusNode);

        /// <summary> ステータスを取り除く </summary>
        /// <param name="isAll">すべて取り除くかどうか、falseの場合最初に登録されたもの</param>
        public void RemoveStatus(EventPlayer evplayer, Status status, Tag target = null, bool isAll = false)
        {
            if (isAll)
                foreach (var node in GetStatusNodeList(status, target))
                    RemoveStatus(evplayer, node);
            else
                RemoveStatus(evplayer, GetStatusNode(status, target));
        }

        /// <summary> ステータスの数値を変更する </summary>
        /// <param name="change">変更関数(元数値)=>変更数値</param>
        /// <param name="status">ステータス種別</param>
        /// <param name="target">軽減・弱点の場合の対象タグ</param>
        /// <param name="fillter">対象をとるFillter</param>
        public void ChangeStatusValue(EventPlayer evplayer, Func<int, int> change, Status status, Tag target = null,
            Func<IEnumerable<IStatusTag>, IEnumerable<TagStatusValue>> fillter = null)
        {
            if (!status.HasValue())
                throw new ArgumentException("status is incorrect. Not value tag");
            if (fillter == null)
                fillter = l => l.Take(1).Cast<TagStatusValue>();
            foreach (var svt in fillter(GetStatusList(status, target)))
                svt.Value = change(svt.Value);
        }
    }
}
