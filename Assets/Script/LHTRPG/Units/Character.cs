using System;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using UnityEngine;

namespace LHTRPG
{
    /// <summary> パーソナルファクターインターフェース </summary>
    public interface IHumanCharacter
    {
        /// <summary> 冒険者かどうか </summary>
        bool IsAdventurer { get; }

        /// <summary> 名前 </summary>
        string Name { get; set; }

        /// <summary> 性別 </summary>
        TagSex Sex { get; set; }

        /// <summary> レベル </summary>
        int Level { get; set; }

        /// <summary> ガイディングクリード </summary>
        GuidingCreed GuidingCreed { get; set; }

        /// <summary> 種族 </summary>
        TagRace Race { get; set; }

        /// <summary> サブ職業 </summary>
        TagSubJob SubJob { get; set; }

        /// <summary> その他のタグ </summary>
        List<Tag> OtherTags { get; }
    }

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
        /// <summary> 識別難易度 </summary>
        IdentificationDifficulty,
        /// <summary> ヘイト倍率 </summary>
        HateRate,
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
            if (CorBattleStatus[CorType.Replace].Any(t => t.Type == type))
            {
                var last = CorBattleStatus[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            return GetBaseBattleStatus(type)
                + CorBattleStatus[CorType.AddSub].Where(t => t.Type == type && t.Check(this))
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
            if (CorSkillValue[CorType.ChangeOriginal].Any(t => t.Type == type))
            {
                var lastCSV = CorSkillValue[CorType.ChangeOriginal].Last(t => t.Type == type);
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
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> 技能値の最終数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>最終数値</returns>
        public virtual int GetSkillValue(SkillValueType type)
        {
            if (CorSkillValue[CorType.Replace].Any(t => t.Type == type))
            {
                var last = CorSkillValue[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            return GetBaseSkillValue(type)
                + CorSkillValue[CorType.AddSub].Where(t => t.Type == type && t.Check(this))
                    .Select(t => t.Correct(this)).Sum();
        }

        /// <summary> 技能判定のロール数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>最終数値</returns>
        public virtual DiceNumber GetJudgeValue(SkillValueType type)
        {
            if (CorJudgeValue[CorType.Replace].Any(t => t.Type == type))
            {
                var last = CorJudgeValue[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            return new DiceNumber { Dice = 2, FixedNumber = GetSkillValue(type) }
                + CorJudgeValue[CorType.AddSub].Where(t => t.Type == type && t.Check(this))
                    .Select(t => t.Correct(this))
                    .Aggregate((t0, t1) => t0 + t1);
        }

        /// <summary> ダメージを受ける処理 </summary>
        /// <param name="evplayer">イベント処理機、何らかのイベント発行する場合使用</param>
        /// <param name="damage">与ダメージ</param>
        /// <param name="type">ダメージ種別</param>
        /// <param name="fromUnit">攻撃Unit</param>
        /// <returns>[実際のダメージ,[障壁]減少分]</returns>
        public override Tuple<int, int> Damage(EventPlayer evplayer, int damage, DamageType type, Unit fromUnit)
        {
            switch (type)
            {
                case DamageType.Physics:
                    // 物理ダメージなら物理防御力分減少
                    damage -= GetBattleStatus(BattleStatusType.PhyDefense);
                    break;
                case DamageType.Magic:
                    // 魔法ダメージなら魔法防御力分減少
                    damage -= GetBattleStatus(BattleStatusType.MagDefense);
                    break;
                case DamageType.Through:
                case DamageType.Directly:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // 直接ダメージ以外なら[軽減]を適用
            if (type != DamageType.Directly)
            {
                var mitigations = GetStatusList<TagStatusTarget>(Status.Mitigation)
                    .Where(t => t.IsApplication(this, fromUnit))
                    .Select(t => t.Value);
                if (mitigations.Any())
                    damage -= mitigations.Max();
            }
            // 軽減系の効果適用

            // ダメージが0以下なら0に
            damage = Math.Max(0, damage);
            var barrierNum = 0;
            // [障壁]がある
            if (IsExistStatus(Status.Barrier))
            {
                var barrierNode = GetStatusNode(Status.Barrier);
                var barrier = barrierNode.Value as TagStatusValue;
                barrierNum = Math.Min(damage, barrier.Value);
                barrier.Value -= barrierNum;
                // [障壁]消滅
                if (barrier.Value <= 0)
                    RemoveStatus(evplayer, barrierNode);
                damage -= barrierNum;
            }
            // HP減少
            HP = Math.Max(0, HP - damage);
            // [戦闘不能]処理
            if (HP <= 0)
                evplayer.AddNext(EventType.GiveStatus, new object[] { Status.UnableFight, fromUnit, this });
            return new Tuple<int, int>(damage, barrierNum);
        }

        /// <summary> 回復する処理 </summary>
        public override int Heal(EventPlayer evplayer, int heal, Unit fromUnit)
        {
            // HP回復
            HP = Math.Min(GetBattleStatus(BattleStatusType.MaxHP), HP + heal);
            return heal;
        }
    }
}
