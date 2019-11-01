using System;
using System.Collections.Generic;
using System.Linq;
using AthensUtility;
using EnumExtension;
using KM.Unity;


namespace LHTRPG
{
    /// <summary> 冒険者 </summary>
    public class Adventurer : Character, IHumanCharacter
    {
        /// <summary> 基礎能力値データ </summary>
        public struct Statust
        {
            /// <summary> 能力値 </summary>
            public Dictionary<AbilityType, int> Ability { get; set; }

            /// <summary> 初期HP </summary>
            public int HP { get; set; }

            /// <summary> HP成長、または、初期因果力 </summary>
            public int Other { get; set; }
        }

        /// <summary> メインジョブの基礎データ、OtherはHP成長 </summary>
        public static Dictionary<MainJob, Statust> StaMainJobs { get; }

        /// <summary> 種族の基礎データ、Otherは初期因果力 </summary>
        public static Dictionary<Race, Statust> StaRaces { get; }

        static Adventurer()
        {
            void func<T>(Dictionary<T, Statust> stas, string filepath) where T : System.Enum
            {
                var csv = new CSVReader(filepath);
                stas = new Dictionary<T, Statust>();
                foreach (var line in csv.Line())
                {

                    stas[line[0].GetEnumByText<T>()] = new Statust
                    {
                        Ability = new Dictionary<AbilityType, int>
                        {
                            { AbilityType.STR, int.Parse(line[1]) },
                            { AbilityType.DEX, int.Parse(line[2]) },
                            { AbilityType.POW, int.Parse(line[3]) },
                            { AbilityType.INT, int.Parse(line[4]) }
                        },
                        HP = int.Parse(line[5]),
                        Other = int.Parse(line[6])
                    };
                }
            }
            func(StaMainJobs, @"Data/StatusMainJob");
            func(StaRaces, @"Data/StatusRace");
        }

        /// <summary> 冒険者かどうか </summary>
        public virtual bool IsAdventurer { get => true; set { } }

        /// <summary> 名前 </summary>
        public string Name { get; set; }

        /// <summary> 性別 </summary>
        public TagSex Sex { get; set; }

        /// <summary> レベル </summary>
        public int Level { get; set; }

        /// <summary> ガイディングクリード </summary>
        public GuidingCreed GuidingCreed { get; set; }

        /// <summary> 種族 </summary>
        public TagRace Race { get; set; }

        /// <summary> サブ職業 </summary>
        public TagSubJob SubJob { get; set; }

        /// <summary> 今までなったことのあるサブ職業の履歴 </summary>
        public List<TagSubJob> HistorySubJob { get; } = new List<TagSubJob>();

        protected override IEnumerable<Tag> LTags
        {
            get
            {
                return new Tag(IsAdventurer ? "冒険者" : "大地人").MakeCollection()
                    .Append(Sex)
                    .Append(Race)
                    .Append(MainJob)
                    .Append(SubJob)
                    .Append(Union)
                    .Append(GuidingCreed.Tag)
                    .Concat(OtherTags)
                    .Where(t => t != null);
            }
        }

        /// <summary> コネクション </summary>
        public List<Character> Connection { get; } = new List<Character>();

        /// <summary> ユニオン </summary>
        public Tag Union { get; set; }

        /// <summary> その他のタグ </summary>
        public List<Tag> OtherTags { get; } = new List<Tag>();

        /// <summary> アーキ職業 </summary>
        public ArchetypeJob ArchetypeJob => MainJob.Type.GetArchetype();

        /// <summary> メイン職業 </summary>
        public TagMainJob MainJob { get; set; }

        /// <summary> メイン職業の基礎値 </summary>
        public Statust StaMainJob => StaMainJobs[MainJob.Type];

        /// <summary> 種族の基礎値 </summary>
        public Statust StaRace => StaRaces[Race.Type];

        /// <summary> ボーナスポイント振り割 </summary>
        public Dictionary<AbilityType, int> AbiBonus { get; }
            = EnumExtension.EnumExtension.GetEnumerable<AbilityType>().ToDictionary(t => t, _ => 0);

        /// <summary> ボーナスポイント合計値 </summary>
        public int SumAbiBonus => AbiBonus.Sum(a => a.Value);

        protected Adventurer(UnitType type) : base(type) { }

        public Adventurer() : base(UnitType.Adventurer) { }

        /// <summary> 能力値を取得する関数 </summary>
        /// <param name="ability">能力値種類</param>
        public int GetPreBaseAbility(AbilityType ability)
            => StaMainJob.Ability[ability] + StaRace.Ability[ability] + AbiBonus[ability];

        /// <summary> 基礎能力値を取得する関数 </summary>
        /// <param name="ability">能力値種類</param>
        private int GetBaseAbility(AbilityType ability) => GetPreBaseAbility(ability) + Rank - 1;

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
                    return GetBaseAbility(AbilityType.STR);
                case BattleStatusType.STR:
                    return GetBattleStatus(BattleStatusType.STRBase) / 3;
                case BattleStatusType.DEXBase:
                    return GetBaseAbility(AbilityType.DEX);
                case BattleStatusType.DEX:
                    return GetBattleStatus(BattleStatusType.DEXBase) / 3;
                case BattleStatusType.POWBase:
                    return GetBaseAbility(AbilityType.POW);
                case BattleStatusType.POW:
                    return GetBattleStatus(BattleStatusType.POWBase) / 3;
                case BattleStatusType.INTBase:
                    return GetBaseAbility(AbilityType.INT);
                case BattleStatusType.INT:
                    return GetBattleStatus(BattleStatusType.INTBase) / 3;
                case BattleStatusType.MaxHP:
                    return StaMainJob.HP + StaRace.HP + (Rank - 1) * StaMainJob.Other;
                case BattleStatusType.StartFate:
                    return StaRace.Other;
                case BattleStatusType.Attack:
                    return 0;
                case BattleStatusType.Magic:
                    return 0;
                case BattleStatusType.Recovary:
                    return 0;
                case BattleStatusType.PhyDefense:
                    return GetBattleStatus(BattleStatusType.STR) * 2;
                case BattleStatusType.MagDefense:
                    return GetBattleStatus(BattleStatusType.INT) * 2;
                case BattleStatusType.Behavior:
                    return GetBattleStatus(BattleStatusType.STR) + GetBattleStatus(BattleStatusType.INT);
                case BattleStatusType.MovePoint:
                    return 2;
            }
            throw new ArgumentOutOfRangeException();
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
