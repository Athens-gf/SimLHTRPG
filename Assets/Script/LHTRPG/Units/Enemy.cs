using AthensUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LHTRPG
{
    /// <summary> エネミークラス </summary>
    public class Enemy : Character
    {
        /// <summary> モブまたはボスタグ </summary>
        public Tag MobBoss { get; set; } = null;

        /// <summary> エネミー種族タグ </summary>
        public TagEnemyRace Race { get; set; }

        /// <summary> その他のタグ </summary>
        public List<Tag> OtherTags { get; } = new List<Tag>();

        /// <summary> 能力値 </summary>
        public Dictionary<BattleStatusType, int> BattleStatus { get; set; }

        /// <summary> 回避 </summary>
        public DiceNumber Avoidance { get; set; }

        /// <summary> 抵抗 </summary>
        public DiceNumber Resistance { get; set; }

        protected override IEnumerable<Tag> LTags => MobBoss.MakeCollection()
            .Append(Race)
            .Concat(OtherTags)
            .Where(t => t != null);

        public Enemy() : base(UnitType.Enemy) { }

        /// <summary> 数値系の基礎数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>基礎数値</returns>
        protected override int GetBaseBattleStatus(BattleStatusType type)
        {
            if (CorBattleStatus[CorType.ChangeOriginal].Any(t => t.Type == type))
            {
                var last = CorBattleStatus[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            switch (type)
            {
                case BattleStatusType.STRBase:
                    return BattleStatus[BattleStatusType.STR] * 3;
                case BattleStatusType.DEXBase:
                    return BattleStatus[BattleStatusType.DEX] * 3;
                case BattleStatusType.POWBase:
                    return BattleStatus[BattleStatusType.POW] * 3;
                case BattleStatusType.INTBase:
                    return BattleStatus[BattleStatusType.INT] * 3;
                case BattleStatusType.STR:
                case BattleStatusType.DEX:
                case BattleStatusType.POW:
                case BattleStatusType.INT:
                case BattleStatusType.MaxHP:
                case BattleStatusType.StartFate:
                case BattleStatusType.PhyDefense:
                case BattleStatusType.MagDefense:
                case BattleStatusType.Behavior:
                case BattleStatusType.MovePoint:
                case BattleStatusType.IdentificationDifficulty:
                case BattleStatusType.HateRate:
                    return BattleStatus[type];
                case BattleStatusType.Attack:
                case BattleStatusType.Magic:
                case BattleStatusType.Recovary:
                    return 0;
                default:
                    break;
            }

            throw new NotImplementedException();
        }

        /// <summary> 能力値を取得する </summary>
        /// <param name="ability">能力種類</param>
        /// <returns>能力値</returns>
        public override int GetAbility(AbilityType ability)
        {
            switch (ability)
            {
                case AbilityType.STR:
                    return GetBattleStatus(BattleStatusType.STR);
                case AbilityType.DEX:
                    return GetBattleStatus(BattleStatusType.DEX);
                case AbilityType.POW:
                    return GetBattleStatus(BattleStatusType.POW);
                case AbilityType.INT:
                    return GetBattleStatus(BattleStatusType.INT);
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary> 技能判定のロール数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>最終数値</returns>
        public override DiceNumber GetJudgeValue(SkillValueType type)
        {
            if (CorJudgeValue[CorType.Replace].Any(t => t.Type == type))
                return base.GetJudgeValue(type);
            switch (type)
            {
                case SkillValueType.Avoidance:
                    return Avoidance + CorJudgeValue[CorType.AddSub]
                        .Where(t => t.Type == type && t.Check(this))
                        .Select(t => t.Correct(this))
                        .Aggregate((t0, t1) => t0 + t1);
                case SkillValueType.Resistance:
                    return Resistance + CorJudgeValue[CorType.AddSub]
                        .Where(t => t.Type == type && t.Check(this))
                        .Select(t => t.Correct(this))
                        .Aggregate((t0, t1) => t0 + t1);
                default:
                    break;
            }
            return base.GetJudgeValue(type);
        }
    }
}
