using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using UnityEngine;
using AthensUtility;
using EnumExtension;

namespace LHTRPG
{
    #region Unit
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
        public abstract void Damage(int damage, DamageType type, Unit unit);

        /// <summary> 回復する処理 </summary>
        public abstract void Heal(int heal, Unit unit);

        /// <summary> ランク </summary>
        public int Rank { get; set; }

        /// <summary> 基礎保持タグ </summary>
        protected abstract IEnumerable<Tag> LTags { get; set; }

        /// <summary> 保持タグ </summary>
        public IEnumerable<Tag> Tags => LTags?.Concat(HaveStatus.OrderBy(s => (int)s.Status).Select(s => s as Tag)) ?? new List<Tag>();

        /// <summary> タグ文字列化 </summary>
        public string GetTagString() => Tags.Select(t => t.ToString()).Aggregate((now, next) => now + " " + next);

        public Unit(UnitType type) { Type = type; }

        /// <summary> 属性タグ一覧 </summary>
        public IEnumerable<TagElement> Elements => Tags.GetTags<TagElement>().Distinct();

        /// <summary> 武器タグ一覧 </summary>
        public IEnumerable<TagWeapon> Weapons => Tags.GetTags<TagWeapon>().Distinct();

        /// <summary> ステータスタグ一覧 </summary>
        public List<IStatusTag> HaveStatus { get; } = new List<IStatusTag>();

        /// <summary> 特定のステータスタグを取得 </summary>
        public IStatusTag GetStatus(Status status, Tag targetTag = null)
        {
            if (targetTag != null && status != Status.WeakPoint && status != Status.Mitigation)
                throw new System.Exception("Incorrect status.");
            var sts = HaveStatus.Where(t => t.Status == status);
            if (targetTag != null)
                sts = sts.Cast<IHaveTargetStatusTag>().Where(t => t.Target == targetTag).Cast<IStatusTag>();
            return sts.Any() ? sts.First() : TagStatus.Create(this, status);
        }

        /// <summary> 特定のステータスタグをキャストして取得 </summary>
        public T GetStatus<T>(Status status, Tag targetTag = null) where T : Tag, IStatusTag => GetStatus(status, targetTag) as T;

        /// <summary> 特定のステータスタグの一覧を取得 </summary>
        public IEnumerable<IStatusTag> GetStatusList(Status status, Tag target = null)
        {
            if (target != null && status != Status.WeakPoint && status != Status.Mitigation)
                throw new Exception("Incorrect status.");
            var sts = HaveStatus.Where(t => t.Status == status);
            if (target != null)
                sts = sts.Cast<IHaveTargetStatusTag>().Where(t => t.Target == target).Cast<IStatusTag>();
            return sts.Any() ? sts.ToList() : new List<IStatusTag>() { TagStatus.Create(this, status) };
        }

        /// <summary> 特定のステータスタグの一覧をキャストして取得 </summary>
        public IEnumerable<T> GetStatusList<T>(Status status, Tag target = null) where T : Tag, IStatusTag
            => GetStatusList(status, target).Cast<T>();
    }
    #endregion

    #region Charactor

    public enum SkillValueType
    {
        /// <summary> 運動 </summary>
        [EnumText("運動")] Exercise,
        /// <summary> 耐久 </summary>
        [EnumText("耐久")] Endurance,
        /// <summary> 解除 </summary>
        [EnumText("解除")] Release,
        /// <summary> 操作 </summary>
        [EnumText("操作")] Operation,
        /// <summary> 知覚 </summary>
        [EnumText("知覚")] Perception,
        /// <summary> 交渉 </summary>
        [EnumText("交渉")] Negotiation,
        /// <summary> 知識 </summary>
        [EnumText("知識")] Knowledge,
        /// <summary> 解析 </summary>
        [EnumText("解析")] Analysis,
        /// <summary> 命中 </summary>
        [EnumText("命中")] Hit,
        /// <summary> 回避 </summary>
        [EnumText("回避")] Avoidance,
        /// <summary> 抵抗 </summary>
        [EnumText("抵抗")] Resistance,
    }

    public enum AbilityType
    {
        STR,
        DEX,
        POW,
        INT,
    }

    public enum BattleStatusType
    {
        /// <summary> STR能力基本値 </summary>
        STRBase,
        /// <summary> STR能力値 </summary>
        STR,
        /// <summary> DEX能力基本値 </summary>
        DEXBase,
        /// <summary> DEX能力値 </summary>
        DEX,
        /// <summary> POW能力基本値 </summary>
        POWBase,
        /// <summary> POW能力値 </summary>
        POW,
        /// <summary> INT能力基本値 </summary>
        INTBase,
        /// <summary> INT能力値 </summary>
        INT,
        /// <summary> 最大HP </summary>
        MaxHP,
        /// <summary> 因果力 </summary>
        StartFate,
        /// <summary> 攻撃力 </summary>
        Attack,
        /// <summary> 魔力 </summary>
        Magic,
        /// <summary> 回復力 </summary>
        Recovary,
        /// <summary> 物理防御力 </summary>
        PhyDefense,
        /// <summary> 魔法防御力 </summary>
        MagDefense,
        /// <summary> 行動力 </summary>
        Behavior,
        /// <summary> 移動力 </summary>
        MovePoint,
    }

    public enum CorType
    {
        /// <summary> 元の数値を置き換え </summary>
        ChangeOriginal,
        /// <summary> 加減算 </summary>
        AddSub,
        /// <summary> 最終数値を置き換え </summary>
        Replace,
    }

    public class CorTuple<TVT, TN>
    {
        /// <summary> 種別 </summary>
        public TVT Type { get; set; }
        /// <summary> 発生源 </summary>
        public Unit Source { get; set; }
        /// <summary> 効果が有効かどうかのチェック
        /// (対象) => 判定結果 </summary>
        public Func<Character, bool> Check { get; set; } = c => true;
        /// <summary> 補正値
        /// (対象) => 補正値 </summary>
        public Func<Character, TN> Correct { get; set; }
    }

    public class CorTupleJudgement
    {
        /// <summary> 種別 </summary>
        public SkillValueType Type { get; set; }
        /// <summary> 発生源 </summary>
        public Unit Source { get; set; }
        /// <summary> 効果が有効かどうかのチェック
        /// (対象, 目標) => 判定結果 </summary>
        public Func<Character, List<Unit>, bool> Check { get; set; } = (c, t) => true;
        /// <summary> 補正値
        /// (対象, 目標) => 補正値 </summary>
        public Func<Character, List<Unit>, DiceNumber> Correct { get; set; }
    }

    public class CorTupleDamage
    {
        /// <summary> 発生源 </summary>
        public Unit Source { get; set; }
        /// <summary> 効果が有効かどうかのチェック
        /// (対象, 目標) => 判定結果 </summary>
        public Func<Character, List<Unit>, bool> Check { get; set; } = (c, t) => true;
        /// <summary> 補正値
        /// (対象, 目標) => 補正値 </summary>
        public Func<Character, List<Unit>, DiceNumber> Correct { get; set; }
    }
    public class CorValues<T>
    {
        public Dictionary<CorType, LinkedList<T>> Values { get; }

        public CorValues()
        {
            Values = new Dictionary<CorType, LinkedList<T>>();
            foreach (CorType e in Enum.GetValues(typeof(CorType)))
                Values[e] = new LinkedList<T>();
        }
    }

    /// <summary> キャラクター(冒険者・ゲスト・エネミー)共通クラス </summary>
    public abstract class Character : Unit
    {
        /// <summary> 数値系の修正値 </summary>
        public CorValues<CorTuple<BattleStatusType, int>> CorBattleStatus { get; } = new CorValues<CorTuple<BattleStatusType, int>>();
        /// <summary> 技能値の修正値 </summary>
        public CorValues<CorTuple<SkillValueType, int>> CorSkillValue { get; } = new CorValues<CorTuple<SkillValueType, int>>();
        /// <summary> 技能判定の修正値 </summary>
        public CorValues<CorTuple<SkillValueType, DiceNumber>> CorJudgeValue { get; } = new CorValues<CorTuple<SkillValueType, DiceNumber>>();
        /// <summary> ダメージロールの修正値 </summary>
        public CorValues<CorTupleDamage> CorDamageRoll { get; } = new CorValues<CorTupleDamage>();

        protected Character(UnitType type) : base(type) { }

        /// <summary> 数値系の基礎数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>基礎数値</returns>
        protected abstract int GetBaseBattleStatus(BattleStatusType type);

        /// <summary> 数値系の修正入り最終数値を取得する </summary>
        /// <param name="type">種類</param>
        /// <returns>最終数値</returns>
        public virtual int GetBattleStatus(BattleStatusType type)
        {
            if (CorBattleStatus.Values[CorType.Replace].Any(t => t.Type == type))
            {
                var last = CorBattleStatus.Values[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            return GetBaseBattleStatus(type)
                + CorBattleStatus.Values[CorType.AddSub].Where(t => t.Type == type && t.Check(this))
                    .Select(t => t.Correct(this)).Sum();
        }

        /// <summary> 能力値を取得する </summary>
        /// <param name="ability">能力種類</param>
        /// <returns>能力値</returns>
        public abstract int GetAbility(AbilityType ability);

        /// <summary> 技能値の基礎数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>基礎数値</returns>
        protected virtual int GetBaseSkillValue(SkillValueType type)
        {
            if (CorSkillValue.Values[CorType.ChangeOriginal].Any(t => t.Type == type))
            {
                var lastCSV = CorSkillValue.Values[CorType.ChangeOriginal].Last(t => t.Type == type);
                if (lastCSV.Check(this))
                    return lastCSV.Correct(this);
            }
            switch (type)
            {
                case SkillValueType.Exercise:
                case SkillValueType.Endurance:
                    return GetAbility(AbilityType.STR);
                case SkillValueType.Release:
                case SkillValueType.Operation:
                case SkillValueType.Avoidance:
                    return GetAbility(AbilityType.DEX);
                case SkillValueType.Perception:
                case SkillValueType.Negotiation:
                case SkillValueType.Resistance:
                    return GetAbility(AbilityType.POW);
                case SkillValueType.Knowledge:
                case SkillValueType.Analysis:
                    return GetAbility(AbilityType.INT);
                case SkillValueType.Hit:
                    return Mathf.Max(GetAbility(AbilityType.STR), GetAbility(AbilityType.DEX), GetAbility(AbilityType.POW), GetAbility(AbilityType.INT));
                default:
                    throw new Exception("The argument is incorrect.");
            }
        }

        /// <summary> 技能値の最終数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>最終数値</returns>
        public virtual int GetSkillValue(SkillValueType type)
        {
            if (CorSkillValue.Values[CorType.Replace].Any(t => t.Type == type))
            {
                var last = CorSkillValue.Values[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            return GetBaseSkillValue(type)
                + CorSkillValue.Values[CorType.AddSub].Where(t => t.Type == type && t.Check(this))
                    .Select(t => t.Correct(this)).Sum();
        }

        /// <summary> 技能判定のロール数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>最終数値</returns>
        public virtual DiceNumber GetJudgeValue(SkillValueType type)
        {
            if (CorJudgeValue.Values[CorType.Replace].Any(t => t.Type == type))
            {
                var last = CorJudgeValue.Values[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            return new DiceNumber(2, GetSkillValue(type))
                + CorJudgeValue.Values[CorType.AddSub].Where(t => t.Type == type && t.Check(this))
                    .Select(t => t.Correct(this))
                    .Aggregate((t0, t1) => t0 + t1);
        }
    }
    #endregion
}
