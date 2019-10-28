using System;
using System.Linq;
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

        /// <summary> そのステータスが数値を持つものかどうかを判定する </summary>
        /// <param name="status">ステータス種類</param>
        public static bool HasValue(this Status status)
        {
            switch (status)
            {
                case Status.Fatigue:
                case Status.WeakPoint:
                case Status.Weakness:
                case Status.Pursuit:
                case Status.Regeneration:
                case Status.Mitigation:
                case Status.Barrier:
                    return true;
            }
            return false;
        }

        public static readonly string TagNameNearbyAttack = "至近距離からの攻撃";
        public static Tag TagNearbyAttack => new Tag(TagNameNearbyAttack);

        public static readonly string TagNameNotNearbyAttack = "至近以外からの攻撃";
        public static Tag TagNotNearbyAttack => new Tag(TagNameNotNearbyAttack);

    }

    public enum StatusCategory
    {
        Life,
        Bad,
        Combat,
        Other,
    }

    public enum Status
    {
        // ライフステータス
        /// <summary> 疲労 </summary>
        [EnumText("疲労")] Fatigue,
        /// <summary> 弱点 </summary>
        [EnumText("弱点")] WeakPoint,
        /// <summary> 戦闘不能 </summary>
        [EnumText("戦闘不能")] UnableFight,
        /// <summary> 死亡 </summary>
        [EnumText("死亡")] Death,
        // バッドステータス
        /// <summary> 萎縮 </summary>
        [EnumText("萎縮")] Atrophy,
        /// <summary> 放心 </summary>
        [EnumText("放心")] Emptiness,
        /// <summary> 硬直 </summary>
        [EnumText("硬直")] Stiffness,
        /// <summary> 惑乱 </summary>
        [EnumText("惑乱")] Scare,
        /// <summary> 衰弱 </summary>
        [EnumText("衰弱")] Weakness,
        /// <summary> 追撃 </summary>
        [EnumText("追撃")] Pursuit,
        /// <summary> 重篤 </summary>
        [EnumText("重篤")] Serious,
        /// <summary> 慢心 </summary>
        [EnumText("慢心")] Prosperity,
        // コンバットステータス
        /// <summary> 再生 </summary>
        [EnumText("再生")] Regeneration,
        /// <summary> 軽減 </summary>
        [EnumText("軽減")] Mitigation,
        /// <summary> 障壁 </summary>
        [EnumText("障壁")] Barrier,
        // アザーステータス
        /// <summary> 水泳 </summary>
        [EnumText("水泳")] Swimming,
        /// <summary> 飛行 </summary>
        [EnumText("飛行")] Flying,
        /// <summary> 二刀流 </summary>
        [EnumText("二刀流")] DoubleSword,
        /// <summary> 隠密 </summary>
        [EnumText("隠密")] Hiding,
        /// <summary> 識別済 </summary>
        [EnumText("識別済")] Identified,
        /// <summary> シーンに存在しない </summary>
        [EnumText("シーンに存在しない")] NotInScene,
        /// <summary> 行動 </summary>
        [EnumText("行動")] Behavior,
        /// <summary> ヘイト </summary>
        [EnumText("ヘイト")] Hate,
        // その他
        /// <summary> その他 </summary>
        [EnumText("その他")] Other,
    }

    public enum TagStatusType
    {
        /// <summary> 無し：同時に取得するということがない </summary>
        None,
        /// <summary> 加算：数値を加算する </summary>
        Add,
        /// <summary> 最大：数値の大きい方を残す </summary>
        Max,
        /// <summary> 重複：数値毎に別々のタグとして存在する </summary>
        Overlap,
    }

    /// <summary> ステータスタグ共通インターフェース </summary>
    public interface IStatusTag
    {
        /// <summary> ステータス種別 </summary>
        Status Status { get; }

        /// <summary> ステータスカテゴリー </summary>
        StatusCategory Category { get; }
    }

    /// <summary> (数量を持たない)ステータスタグ </summary>
    public class TagStatus : Tag, IStatusTag
    {
        /// <summary> ステータス種別 </summary>
        public Status Status { get; }

        /// <summary> ステータスカテゴリー </summary>
        public virtual StatusCategory Category => Status.GetCategory();

        public TagStatus(Status status) : base(status.GetText()) => Status = status;

        protected TagStatus(string name) : base(name) => Status = Status.Other;

        public enum Statusaa
        {
            // その他
            /// <summary> その他 </summary>
            [EnumText("その他")] Other,
        }

        // ライフステータス
        /// <summary> 疲労 </summary>
        public static TagStatusValue Fatigue => new TagStatusValue(TagStatusType.Add, Status.Fatigue);
        /// <summary> 弱点 </summary>
        public static TagStatusTarget WeakPoint(Tag target) => new TagStatusTarget(TagStatusType.Overlap, Status.WeakPoint, target);
        /// <summary> 戦闘不能 </summary>
        public static TagStatus UnableFight => new TagStatus(Status.UnableFight);
        /// <summary> 死亡 </summary>
        public static TagStatus Death => new TagStatus(Status.Death);

        // バッドステータス
        /// <summary> 萎縮 </summary>
        public static TagStatus Atrophy => new TagStatus(Status.Atrophy);
        /// <summary> 放心 </summary>
        public static TagStatus Emptiness => new TagStatus(Status.Emptiness);
        /// <summary> 硬直 </summary>
        public static TagStatus Stiffness => new TagStatus(Status.Stiffness);
        /// <summary> 惑乱 </summary>
        public static TagStatus Scare => new TagStatus(Status.Scare);
        /// <summary> 衰弱 </summary>
        public static TagStatusValue Weakness => new TagStatusValue(TagStatusType.Max, Status.Weakness);
        /// <summary> 追撃 </summary>
        public static TagStatusValue Pursuit => new TagStatusValue(TagStatusType.Overlap, Status.Pursuit);
        /// <summary> 重篤 </summary>
        public static TagStatus Serious => new TagStatus(Status.Serious);
        /// <summary> 慢心 </summary>
        public static TagStatus Prosperity => new TagStatus(Status.Prosperity);

        // コンバットステータス
        /// <summary> 再生 </summary>
        public static TagStatusValue Regeneration => new TagStatusValue(TagStatusType.Max, Status.Regeneration);
        /// <summary> 軽減 </summary>
        public static TagStatusTarget Mitigation(Tag target) => new TagStatusTarget(TagStatusType.Overlap, Status.Mitigation, target);
        /// <summary> 障壁 </summary>
        public static TagStatusValue Barrier => new TagStatusValue(TagStatusType.Max, Status.Barrier);

        // アザーステータス
        /// <summary> 水泳 </summary>
        public static TagStatus Swimming => new TagStatus(Status.Swimming);
        /// <summary> 飛行 </summary>
        public static TagStatus Flying => new TagStatus(Status.Flying);
        /// <summary> 二刀流 </summary>
        public static new TagStatus DoubleSword => new TagStatus(Status.DoubleSword);
        /// <summary> 隠密 </summary>
        public static TagStatus Hiding(bool isCharacter) => new TagStatusHiding(isCharacter);
        /// <summary> 識別済 </summary>
        public static TagStatus Identified(bool isCharacter) => new TagStatusIdentified(isCharacter);
        /// <summary> シーンに存在しない </summary>
        public static TagStatus NotInScene => new TagStatus(Status.NotInScene);
        /// <summary> 行動 </summary>
        public static TagStatusBehavior Behavior => new TagStatusBehavior();
        /// <summary> ヘイト </summary>
        public static TagStatusHate Hate => new TagStatusHate();

        public static IStatusTag MakeStatus(Status status, Tag target = null, bool isCharacter = true)
        {
            switch (status)
            {
                // ライフステータス
                /// <summary> 疲労 </summary>
                case Status.Fatigue: return Fatigue;
                /// <summary> 弱点 </summary>
                case Status.WeakPoint: return WeakPoint(target);
                /// <summary> 戦闘不能 </summary>
                case Status.UnableFight: return UnableFight;
                /// <summary> 死亡 </summary>
                case Status.Death: return Death;
                // バッドステータス
                /// <summary> 萎縮 </summary>
                case Status.Atrophy: return Atrophy;
                /// <summary> 放心 </summary>
                case Status.Emptiness: return Emptiness;
                /// <summary> 硬直 </summary>
                case Status.Stiffness: return Stiffness;
                /// <summary> 惑乱 </summary>
                case Status.Scare: return Scare;
                /// <summary> 衰弱 </summary>
                case Status.Weakness: return Weakness;
                /// <summary> 追撃 </summary>
                case Status.Pursuit: return Pursuit;
                /// <summary> 重篤 </summary>
                case Status.Serious: return Serious;
                /// <summary> 慢心 </summary>
                case Status.Prosperity: return Prosperity;
                // コンバットステータス
                /// <summary> 再生 </summary>
                case Status.Regeneration: return Regeneration;
                /// <summary> 軽減 </summary>
                case Status.Mitigation: return Mitigation(target);
                /// <summary> 障壁 </summary>
                case Status.Barrier: return Barrier;
                // アザーステータス
                /// <summary> 水泳 </summary>
                case Status.Swimming: return Swimming;
                /// <summary> 飛行 </summary>
                case Status.Flying: return Flying;
                /// <summary> 二刀流 </summary>
                case Status.DoubleSword: return DoubleSword;
                /// <summary> 隠密 </summary>
                case Status.Hiding: return Hiding(isCharacter);
                /// <summary> 識別済 </summary>
                case Status.Identified: return Identified(isCharacter);
                /// <summary> シーンに存在しない </summary>
                case Status.NotInScene: return NotInScene;
                /// <summary> 行動 </summary>
                case Status.Behavior: return Behavior;
                /// <summary> ヘイト </summary>
                case Status.Hate: return Hate;
            }
            throw new ArgumentOutOfRangeException();
        }

    }

    /// <summary> (数量を持たない)その他のステータスタグ </summary>
    public class TagStatusOther : TagStatus
    {
        private StatusCategory category;

        /// <summary> ステータスカテゴリー </summary>
        public override StatusCategory Category => category;

        public TagStatusOther(StatusCategory cat, string name) : base(name) => category = cat;

        public override string ToString() => Name;
    }

    /// <summary> 数量を持つステータスタグ </summary>
    public class TagStatusValue : TagValue, IStatusTag
    {
        /// <summary> ステータス種別 </summary>
        public Status Status { get; }

        /// <summary> ステータスカテゴリー </summary>
        public virtual StatusCategory Category => Status.GetCategory();

        public TagStatusValue(TagStatusType type, Status status) : base(status.GetText(), type, 0) => Status = status;

        protected TagStatusValue(TagStatusType type, string name) : base(name, type, 0) => Status = Status.Other;
    }

    /// <summary> 数量を持つその他のステータスタグ </summary>
    public class TagStatusValueOther : TagStatusValue
    {
        private StatusCategory category;

        /// <summary> ステータスカテゴリー </summary>
        public override StatusCategory Category => category;

        public TagStatusValueOther(StatusCategory cat, TagStatusType type, string name) : base(type, name) => category = cat;

        public override string ToString() => Name;
    }

    /// <summary> 数量を持ち、対象タグを持つステータスタグ </summary>
    public class TagStatusTarget : TagStatusValue
    {
        /// <summary> 弱点になるタグ </summary>
        public Tag Target { get; protected set; }

        public TagStatusTarget(TagStatusType type, Status status, Tag targetTag) : base(TagStatusType.Overlap, Status.Mitigation) => Target = targetTag;

        public override string ToString() { return Target == null ? base.ToString() : "[" + Name + "（" + Target.Name + "）：" + Value + "]"; }

        /// <returns>適用されるならTrue</returns>
        /// <summary> 軽減・弱点が適用されるかどうか </summary>
        /// <param name="target">このステータスを持つUnit</param>
        /// <param name="attacker">攻撃者Unit</param>
        public bool IsApplication(Unit target, Unit attacker)
        {
            if (Target == null)
                return true;
            if (Target.Name == LHTRPGBase.TagNameNearbyAttack || Target.Name == LHTRPGBase.TagNameNotNearbyAttack)
            {
                if (target.Session.CurrentScene is SceneBattle)
                {
                    var pos = (target.Session.CurrentScene as SceneBattle).Positions;
                    return Target.Name == LHTRPGBase.TagNameNearbyAttack
                        ? pos[target] == pos[attacker]
                        : pos[target] != pos[attacker];
                }
                return false;
            }
            return attacker.Tags.Contains(Target);
        }
    }

    /// <summary> 隠密／隠蔽 </summary>
    public class TagStatusHiding : TagStatus
    {
        /// <summary> 対象がキャラクターかどうか </summary>
        public bool IsCharacter { get; }

        public TagStatusHiding(bool isCharacter) : base(Status.Hiding) => IsCharacter = isCharacter;

        public override string Name => IsCharacter ? "隠密" : "隠蔽";
    }

    /// <summary> 識別済み／解析済み </summary>
    public class TagStatusIdentified : TagStatus
    {
        /// <summary> 対象がキャラクターかどうか </summary>
        public bool IsCharacter { get; }

        public TagStatusIdentified(bool isCharacter) : base(Status.Identified) => IsCharacter = isCharacter;

        public override string Name => IsCharacter ? "識別済み" : "解析済み";
    }

    public enum BehaviorType
    {
        /// <summary> 未行動 </summary>
        [EnumText("未行動")] NotYet,
        /// <summary> 行動済み </summary>
        [EnumText("行動済み")] Already,
        /// <summary> 待機 </summary>
        [EnumText("待機")] Waiting,
    }

    /// <summary> 未行動／行動済み／待機 </summary>
    public class TagStatusBehavior : TagStatus
    {
        public BehaviorType Type { get; set; }

        public TagStatusBehavior(BehaviorType type = BehaviorType.NotYet) : base(Status.Behavior) => Type = type;

        public override string Name => Type.GetText();
    }

    /// <summary> ヘイトトップ／ヘイトアンダー </summary>
    public class TagStatusHate : TagStatus
    {
        public bool IsHateTop { get; set; } = false;

        public TagStatusHate() : base(Status.Hate) { }

        public override string Name => IsHateTop ? "ヘイトトップ" : "ヘイトアンダー";
    }
}
