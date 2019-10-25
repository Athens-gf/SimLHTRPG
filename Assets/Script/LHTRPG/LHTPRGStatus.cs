using System.Linq;
using UnityEngine;
using EnumExtension;

namespace LHTRPG
{
    public static partial class LHTRPGBase
    {
        /// <summary> ステータスのカテゴリーを取得する </summary>
        public static StatusCategory GetCategory(this Status status)
        {
            switch (status)
            {
                case Status.Fatigue:
                case Status.WeakPoint:
                case Status.UnableFight:
                case Status.Death:
                    return StatusCategory.Life;
                case Status.Atrophy:
                case Status.Emptiness:
                case Status.Stiffness:
                case Status.Scare:
                case Status.Weakness:
                case Status.Pursuit:
                case Status.Serious:
                case Status.Prosperity:
                    return StatusCategory.Bad;
                case Status.Regeneration:
                case Status.Mitigation:
                case Status.Barrier:
                    return StatusCategory.Combat;
                default:
                    return StatusCategory.Other;
            }
        }

        public static readonly string TagNameNearbyAttack = "至近距離からの攻撃";
        public static Tag TagNearbyAttack => new Tag(TagNameNearbyAttack);
        public static readonly string TagNameNotNearbyAttack = "至近以外からの攻撃";
        public static Tag TagNotNearbyAttack => new Tag(TagNameNotNearbyAttack);

        /// <summary> 軽減・弱点が適用されるかどうか </summary>
        /// <param name="tag">軽減・弱点タグ</param>
        /// <param name="unit">軽減=>攻撃者、弱点=>攻撃対象</param>
        /// <returns>適用されるならTrue</returns>
        public static bool IsApplication<T>(this T tag, Unit unit) where T : TagStatusValue, IHaveTargetStatusTag
        {
            var targetName = tag.Target.Name;
            if (targetName == TagNameNearbyAttack || targetName == TagNameNotNearbyAttack)
            {
                if (tag.Unit?.Session.CurrentScene is SceneBattle)
                {
                    var pos = (tag.Unit.Session.CurrentScene as SceneBattle).Positions;
                    return targetName == TagNameNearbyAttack ? pos[tag.Unit] == pos[unit] : pos[tag.Unit] != pos[unit];
                }
                return false;
            }
            return unit.Tags.Contains(tag.Target);
        }
    }

    /// <summary> ステータスタグ共通インターフェース </summary>
    public interface IStatusTag
    {
        /// <summary> 重複可能かどうか </summary>
        Status Status { get; }
        /// <summary> 存在しているかどうか </summary>
        bool IsExist { get; }
        /// <summary>  </summary>
        void Recieve(int value = 0);
        /// <summary>  </summary>
        void Remove();
    }

    /// <summary> ステータスタグ共通インターフェース </summary>
    public interface IHaveTargetStatusTag
    {
        /// <summary> 対象 </summary>
        Tag Target { get; }
    }

    /// <summary> (数量を持たない)ステータスタグ </summary>
    public class TagStatus : Tag, IStatusTag
    {
        public TagStatus(Unit unit, Status status) : base(status.GetText()) { Status = status; Unit = unit; }

        public Status Status { get; }

        public bool IsExist => Unit.HaveStatus.Any(s => s == this);

        public void Remove() { Unit.HaveStatus.Remove(this); }

        public void Recieve(int value = 0) { Unit.HaveStatus.Add(this); }

        /// <summary> タグ生成関数 </summary>
        public static IStatusTag Create(Unit unit, Status status, Tag targetTag = null)
        {
            switch (status)
            {
                case Status.Fatigue:
                    return new StFatigue(unit);
                case Status.WeakPoint:
                    return new StWeakPoint(unit, targetTag);
                case Status.Weakness:
                    return new StWeakness(unit);
                case Status.Pursuit:
                    return new StPursuit(unit);
                case Status.Regeneration:
                    return new StRegeneration(unit);
                case Status.Mitigation:
                    return new StMitigation(unit, targetTag);
                case Status.Barrier:
                    return new StBarrier(unit);
                case Status.DoubleSword:
                    return new StDoubleSword(unit);
                case Status.Hiding:
                    return new StHiding(unit);
                case Status.Identified:
                    return new StIdentified(unit);
                case Status.Behavior:
                    return new StBehavior(unit);
                case Status.Hate:
                    return new StHate(unit);
                default:
                    break;
            }
            return new TagStatus(unit, status);
        }
    }

    /// <summary> 数量を持つステータスタグ </summary>
    public class TagStatusValue : TagValue, IStatusTag
    {
        public TagStatusValue(Unit unit, TagBattleStatusType type, Status status) : base(status.GetText(), type, 0)
        { Status = status; Unit = unit; }

        public Status Status { get; }

        public bool IsExist => Unit.HaveStatus.Any(s => s == this);

        public void Remove() { Unit.HaveStatus.Remove(this); }

        public void Recieve(int value)
        {
            if (Type == TagBattleStatusType.Overlap || Value == 0) Unit.HaveStatus.Add(this);
            switch (Type)
            {
                case TagBattleStatusType.Add:
                    Value += value;
                    break;
                case TagBattleStatusType.Max:
                    Value = Mathf.Max(Value, value);
                    break;
                case TagBattleStatusType.Overlap:
                    Value = value;
                    break;
            }
            if (Value <= 0) Remove();
        }
    }

    // ライフステータス
    /// <summary> 疲労 </summary>
    public class StFatigue : TagStatusValue { public StFatigue(Unit unit) : base(unit, TagBattleStatusType.Add, Status.Fatigue) { } }

    /// <summary> 弱点 </summary>
    public class StWeakPoint : TagStatusValue, IHaveTargetStatusTag
    {
        public Tag Target { get; protected set; }

        public StWeakPoint(Unit unit, Tag targetTag) : base(unit, TagBattleStatusType.Overlap, Status.Mitigation) { Target = targetTag; }

        public override string ToString() { return Target == null ? base.ToString() : "[" + Name + "（" + Target.Name + "）：" + Value + "]"; }
    }

    // バッドステータス
    /// <summary> 衰弱 </summary>
    public class StWeakness : TagStatusValue { public StWeakness(Unit unit) : base(unit, TagBattleStatusType.Max, Status.Weakness) { } }

    /// <summary> 追撃 </summary>
    public class StPursuit : TagStatusValue { public StPursuit(Unit unit) : base(unit, TagBattleStatusType.Overlap, Status.Pursuit) { } }

    // コンバットステータス
    /// <summary> 再生 </summary>
    public class StRegeneration : TagStatusValue { public StRegeneration(Unit unit) : base(unit, TagBattleStatusType.Max, Status.Regeneration) { } }

    /// <summary> 軽減 </summary>
    public class StMitigation : TagStatusValue, IHaveTargetStatusTag
    {
        public Tag Target { get; protected set; }

        public StMitigation(Unit unit, Tag targetTag = null) : base(unit, TagBattleStatusType.Overlap, Status.Mitigation) { Target = targetTag; }

        public override string ToString() { return Target == null ? base.ToString() : "[" + Name + "（" + Target.Name + "）：" + Value + "]"; }
    }

    /// <summary> 障壁 </summary>
    public class StBarrier : TagStatusValue { public StBarrier(Unit unit) : base(unit, TagBattleStatusType.Max, Status.Barrier) { } }

    // アザーステータス
    /// <summary> 二刀流状態 </summary>
    public class StDoubleSword : TagStatus { public StDoubleSword(Unit unit) : base(unit, Status.DoubleSword) { } }

    /// <summary> 隠密／隠蔽 </summary>
    public class StHiding : TagStatus
    {
        public StHiding(Unit unit) : base(unit, Status.Hiding) { }
        public override string Name => Unit.IsCharacter ? "隠密" : "隠蔽";
    }

    /// <summary> 識別済み／解析済み </summary>
    public class StIdentified : TagStatus
    {
        public StIdentified(Unit unit) : base(unit, Status.Identified) { }
        public override string Name => Unit.IsCharacter ? "識別済み" : "解析済み";
    }

    /// <summary> 未行動／行動済み／待機 </summary>
    public class StBehavior : TagStatus
    {
        public BehaviorType Type { get; set; }
        public StBehavior(Unit unit, BehaviorType type = BehaviorType.NotYet) : base(unit, Status.Behavior) { Type = type; }
        public override string Name => Type.GetText();
    }

    /// <summary> ヘイトトップ／ヘイトアンダー </summary>
    public class StHate : TagStatus
    {
        public bool IsHateTop { get; set; } = false;
        public StHate(Unit unit) : base(unit, Status.Hate) { }
        public override string Name => IsHateTop ? "ヘイトトップ" : "ヘイトアンダー";
    }
}
