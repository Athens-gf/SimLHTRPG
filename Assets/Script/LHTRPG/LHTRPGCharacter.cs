using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using EnumExtension;
using KM.Unity;
using AthensUtility;


namespace LHTRPG
{
    public static partial class LHTRPGBase
    {
        /// <summary> メイン職業からそれに対応したアーキ職業を取得する </summary>
        /// <param name="mainJob">メイン職業</param>
        public static ArchetypeJob GetArchetype(this MainJob mainJob)
        {
            switch (mainJob)
            {
                case MainJob.Guardian:
                case MainJob.Samurai:
                case MainJob.Monk:
                    return ArchetypeJob.Warrior;
                case MainJob.Cleric:
                case MainJob.Druid:
                case MainJob.Kannagi:
                    return ArchetypeJob.Warrior;
                case MainJob.Assassin:
                case MainJob.Swashbuckler:
                case MainJob.Bard:
                    return ArchetypeJob.Warrior;
                case MainJob.Sorcerer:
                case MainJob.Summoner:
                case MainJob.Enchanter:
                    return ArchetypeJob.Warrior;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum Sex
    {
        [EnumText("男性")] Male,
        [EnumText("女性")] Female,
    }

    /// <summary> 性別タグ </summary>
    public class TagSex : Tag<Sex> { public TagSex(Sex type) : base(type) { } }

    public enum Race
    {
        [EnumText("ヒューマン")] Human,
        [EnumText("エルフ")] Elf,
        [EnumText("ドワーフ")] Dwarf,
        [EnumText("ハーフアルヴ")] HalfArve,
        [EnumText("猫人族")] Cat,
        [EnumText("狼牙族")] Wolf,
        [EnumText("狐尾族")] Fox,
        [EnumText("法儀族")] Pattern,
    }

    /// <summary> 種族タグ </summary>
    public class TagRace : Tag<Race> { public TagRace(Race type) : base(type) { } }

    public enum ArchetypeJob
    {
        [EnumText("戦士職")] Warrior,
        [EnumText("回復職")] Recovery,
        [EnumText("武器攻撃職")] Weapon,
        [EnumText("魔法攻撃職")] Magic,
    }

    public enum MainJob
    {
        // 戦士職
        [EnumText("守護戦士")] Guardian,
        [EnumText("武士")] Samurai,
        [EnumText("武闘家")] Monk,
        // 回復職
        [EnumText("施療神官")] Cleric,
        [EnumText("森呪遣い")] Druid,
        [EnumText("神祇官")] Kannagi,
        // 武器攻撃職
        [EnumText("暗殺者")] Assassin,
        [EnumText("盗剣士")] Swashbuckler,
        [EnumText("吟遊詩人")] Bard,
        // 魔法攻撃職
        [EnumText("妖術師")] Sorcerer,
        [EnumText("召喚術師")] Summoner,
        [EnumText("付与術師")] Enchanter,
    }

    /// <summary> メイン職業タグ </summary>
    public class TagMainJob : Tag<MainJob> { public TagMainJob(MainJob type) : base(type) { } }

    /// <summary> サブ職業タグ </summary>
    public class TagSubJob : Tag
    {
        /// <summary> 登録サブ職業一覧 </summary>
        public static ReadOnlyCollection<string> SubJobs { get; }

        static TagSubJob()
        {
            var csv = new CSVReader(@"Data/SubJobs");
            var l = new List<string>();
            for (int i = 0; i < csv.Row; i++)
                l.Add(csv[i][0]);
            SubJobs = new ReadOnlyCollection<string>(l);
        }

        /// <summary> サブ職業乱数取得 </summary>
        public static TagSubJob GetRand() => new TagSubJob(SubJobs.GetRand());

        /// <summary> 一覧にないサブ職業を登録するとき、処理のベースになる職業 </summary>
        public string BaseSubJob { get; }

        public TagSubJob(string name) : base(name) { BaseSubJob = name; }
        public TagSubJob(string name, string @base) : base(name) { BaseSubJob = @base; }
    }


    /// <summary> クリード(掟) </summary>
    public struct Creed
    {
        /// <summary> クリード名 </summary>
        public string Name { get; set; }

        /// <summary> 説明 </summary>
        public string Explanation { get; set; }

        /// <summary> 要約 </summary>
        public string Summary { get; set; }

        /// <summary> 人物タグ種別 </summary>
        public Person Tag { get; set; }
    }

    /// <summary> ガイディングクリード </summary>
    public class GuidingCreed
    {
        /// <summary> 登録ガイディングクリード </summary>
        public static ReadOnlyCollection<Creed> Creeds { get; }

        /// <summary> ガイディングクリード名一覧 </summary>
        public static List<string> CreedNames => Creeds.Select(c => c.Name).ToList();

        static GuidingCreed()
        {
            var csv = new CSVReader(@"Data/Creed");
            var lb = new List<Creed>();
            foreach (var line in csv.Line())
                lb.Add(new Creed
                {
                    Name = line[0],
                    Explanation = line[1],
                    Summary = line[2],
                    Tag = line[3].GetEnumByText<Person>()
                });
            Creeds = new ReadOnlyCollection<Creed>(lb);
        }

        /// <summary> ランダマイザ </summary>
        public static GuidingCreed GetRand() { return new GuidingCreed(Creeds.GetRand()); }

        /// <summary> ユニット </summary>
        public Unit Unit { get; set; }

        /// <summary> 選択ガイディングクリード </summary>
        public Creed Creed { get; set; }

        /// <summary> 人物タグ </summary>
        public TagPerson Tag { get; set; }

        public GuidingCreed(string creedName) : this(Creeds.First(c => c.Name == creedName)) { }
        public GuidingCreed(Creed creed) { Creed = creed; Tag = new TagPerson(Creed.Tag); }
    }

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

    /// <summary> エキストラ(NPC、ルール城のデータを一切持たない演出上のキャラクター) </summary>
    public class Extra : Unit, IHumanCharacter
    {
        public Extra() : base(UnitType.Extra) { }

        /// <summary> 冒険者かどうか </summary>
        public bool IsAdventurer { get; set; }

        /// <summary> 名前 </summary>
        public string Name { get; set; }

        /// <summary> 性別 </summary>
        public TagSex Sex { get; set; }

        public int Level { get; set; }

        /// <summary> ガイディングクリード </summary>
        public GuidingCreed GuidingCreed { get; set; }

        /// <summary> 種族 </summary>
        public TagRace Race { get; set; }

        /// <summary> サブ職業 </summary>
        public TagSubJob SubJob { get; set; }

        /// <summary> その他のタグ </summary>
        public List<Tag> OtherTags { get; } = new List<Tag>();

        protected override IEnumerable<Tag> LTags
        {
            get => new Tag(IsAdventurer ? "冒険者" : "大地人").MakeCollection()
                    .Append(Sex)
                    .Append(Race)
                    .Append(SubJob)
                    .Append(GuidingCreed.Tag)
                    .Concat(OtherTags)
                    .Where(t => t != null);
        }

        /// <summary> ダメージを受ける処理 </summary>
        public override int Damage(int damage, DamageType type, Unit fromUnit) => 0;

        /// <summary> 回復する処理 </summary>
        public override int Heal(int heal, Unit fromUnit) => 0;
    }

    /// <summary> ゲスト(NPC、能力値やHPなどのデータを持つキャラクター) </summary>
    public class Guest : Adventurer
    {
        /// <summary> 冒険者かどうか </summary>
        public override bool IsAdventurer { get; set; }

        public Guest() : base(UnitType.Guest) { }
    }

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
        public override int Damage(int damage, DamageType type, Unit fromUnit)
        {
            switch (type)
            {
                case DamageType.Physics:
                    break;
                case DamageType.Magic:
                    break;
                case DamageType.Through:
                    break;
                case DamageType.Directly:
                    break;
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary> 回復する処理 </summary>
        public override int Heal(int heal, Unit fromUnit)
        {
            return heal;
        }
    }
}
