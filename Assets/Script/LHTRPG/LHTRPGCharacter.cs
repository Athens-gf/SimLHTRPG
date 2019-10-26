using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnumExtension;
using KM.Unity;
using AthensUtility;


namespace LHTRPG
{
    using Ability = Dictionary<AbilityType, int>;

    public static partial class LHTRPGBase
    {
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
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        public static TagRace GetTag(this Race race, Unit unit = null) { return new TagRace(race) { Unit = unit }; }
        public static TagMainJob GetTag(this MainJob mj, Unit unit = null) { return new TagMainJob(mj) { Unit = unit }; }
    }

    public enum Sex
    {
        [EnumText("男性")] Male,
        [EnumText("女性")] Female,
    }

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

    public class TagMainJob : Tag<MainJob> { public TagMainJob(MainJob type) : base(type) { } }

    public class TagSubJob : Tag
    {
        public static ReadOnlyCollection<string> SubJobs { get; }
        static TagSubJob()
        {
            var csv = new CSVReader(@"Data/SubJobs");
            var l = new List<string>();
            for (int i = 0; i < csv.Row; i++) l.Add(csv[i][0]);
            SubJobs = new ReadOnlyCollection<string>(l);
        }
        public static TagSubJob GetRand() { return new TagSubJob(SubJobs.GetRand()); }

        public string BaseSubJob { get; private set; }
        public TagSubJob(string name) : base(name) { BaseSubJob = name; }
        public TagSubJob(string name, string @base) : base(name) { BaseSubJob = @base; }
    }

    public struct Creeds
    {
        public string Name { get; set; }
        public string Creed { get; set; }
        public Person Tag { get; set; }
    }

    public class GuidingCreed
    {
        public static ReadOnlyCollection<Creeds> Creeds { get; }
        public static List<string> CreedNames { get { return Creeds.Select(c => c.Name).ToList(); } }
        static GuidingCreed()
        {
            var csv = new CSVReader(@"Data/Creed");
            var lb = new List<Creeds>();
            foreach (var line in csv.Line())
                lb.Add(new Creeds { Name = line[0], Creed = line[1], Tag = line[2].GetEnumByText<Person>() });
            Creeds = new ReadOnlyCollection<Creeds>(lb);
        }
        public static GuidingCreed GetRand(Unit unit) { return new GuidingCreed(unit, Creeds.GetRand()); }

        public Unit Unit { get; set; }
        public Creeds Creed { get; set; }
        public TagPerson Tag { get; set; }

        public GuidingCreed(Unit unit, string creedName) : this(unit, Creeds.First(c => c.Name == creedName)) { }
        public GuidingCreed(Unit unit, Creeds creed) { Creed = creed; Tag = new TagPerson(Creed.Tag) { Unit = unit }; }
    }

    public interface IHumanCharacter
    {
        bool IsAdventurer { get; }
        string Name { get; set; }
        TagSex Sex { get; set; }
        int Level { get; set; }
        GuidingCreed GuidingCreed { get; set; }
        TagRace Race { get; set; }
        TagSubJob SubJob { get; set; }
        List<Tag> OtherTags { get; }
    }

    public class Extra : Unit, IHumanCharacter
    {
        public Extra() : base(UnitType.Extra) { }

        public bool IsAdventurer { get; set; }
        public string Name { get; set; }
        private TagSex sex;
        public TagSex Sex { get => sex; set => sex = new TagSex(value.Type) { Unit = this }; }
        public int Level { get; set; }
        private GuidingCreed guidingCreed;
        public GuidingCreed GuidingCreed { get { return guidingCreed; } set { guidingCreed = new GuidingCreed(this, value.Creed); } }
        private TagRace race;
        public TagRace Race { get => race; set => race = new TagRace(value.Type) { Unit = this }; }
        private TagSubJob subJob;
        public TagSubJob SubJob { get { return subJob; } set { subJob = new TagSubJob(value.Name, value.BaseSubJob) { Unit = this }; } }
        public List<Tag> OtherTags { get; } = new List<Tag>();

        protected override IEnumerable<Tag> LTags
        {
            get => new Tag(IsAdventurer ? "冒険者" : "大地人") { Unit = this }.MakeCollection()
                    .Append(Sex)
                    .Append(Race)
                    .Append(SubJob)
                    .Append(GuidingCreed.Tag)
                    .Concat(OtherTags)
                    .Where(t => t != null);
            set { }
        }

        public override void Damage(int damage, DamageType type, Unit unit) { }

        public override void Heal(int heal, Unit unit) { }
    }

    public class Guest : Adventurer
    {
        public override bool IsAdventurer { get; set; }
        public Guest() : base(UnitType.Guest) { }
    }

    public class Adventurer : Character, IHumanCharacter
    {
        public struct Statust
        {
            public Ability Ability { get; set; }
            public int HP { get; set; }
            public int Other { get; set; }
        }
        public static Dictionary<MainJob, Statust> StaMainJobs { get; }
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
                        Ability = new Ability
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

        public virtual bool IsAdventurer { get { return true; } set { } }
        public string Name { get; set; }
        private Sex sex;
        public Sex Sex { get { return sex; } set { sex = value.Type != Sex.Sex.Other ? new Sex(this, value.Type == Sex.Sex.Male) : new Sex(this, value.OtherName); } }
        public int Level { get; set; }
        private GuidingCreed guidingCreed;
        public GuidingCreed GuidingCreed { get { return guidingCreed; } set { guidingCreed = new GuidingCreed(this, value.Creed); } }

        public Race Race { get; set; }
        private SubJob subJob;
        public SubJob SubJob { get { return subJob; } set { subJob = new SubJob(this, value.Name, value.BaseSubJob); } }
        public List<SubJob> HistorySubJob { get; private set; }

        protected override List<Tag> LTags
        {
            get
            {
                return (new Tag(this, IsAdventurer ? "冒険者" : "大地人")).MakeCollection().Append(Sex).Append(Race.GetTag(this)).Append(MainJob.GetTag(this))
                    .Append(SubJob).Append(Union).Append(GuidingCreed.Tag).Append(OtherTags).Where(t => t != null).ToList();
            }
            set { }
        }

        public List<Character> Connection { get; private set; }
        public Tag Union { get; set; }
        public List<Tag> OtherTags { get; private set; }

        public ArchetypeJob ArchetypeJob { get { return MainJob.GetArchetype(); } }
        public MainJob MainJob { get; set; }

        public Statust StaMainJob { get { return StaMainJobs[MainJob]; } }
        public Statust StaRace { get { return StaRaces[Race]; } }
        public Ability AbiHuman { get; set; }
        public Ability AbiBonus { get; set; }
        public int SumAbiBonus { get { return AbiBonus.Sum(a => a.Value); } }

        protected Adventurer(UnitType type) : base(type)
        {
            Connection = new List<Character>();
            HistorySubJob = new List<SubJob>();
            Union = null;
            OtherTags = new List<Tag>();
            AbiBonus = new Ability { { Ability.STR, 0 }, { Ability.DEX, 0 }, { Ability.POW, 0 }, { Ability.INT, 0 } };
            AbiHuman = new Ability { { Ability.STR, 0 }, { Ability.DEX, 0 }, { Ability.POW, 0 }, { Ability.INT, 0 } };
        }

        public Adventurer() : this(Type.Adventurer) { }

        private int GetBaseAbility(Ability ba) { return StaMainJob.Ability[ba] + StaRace.Ability[ba] + AbiHuman[ba] + AbiBonus[ba] + Rank - 1; }

        protected override int GetBaseValue(Value value)
        {
            if (FuncReplaceBaseValues[value] != null) return FuncReplaceBaseValues[value]();
            switch (value)
            {
                case Value.BaseSTR:
                    return GetBaseAbility(Ability.STR);
                case Value.STR:
                    return GetValue(Value.BaseSTR) / 3;
                case Value.BaseDEX:
                    return GetBaseAbility(Ability.DEX);
                case Value.DEX:
                    return GetValue(Value.BaseDEX) / 3;
                case Value.BasePOW:
                    return GetBaseAbility(Ability.POW);
                case Value.POW:
                    return GetValue(Value.BasePOW) / 3;
                case Value.BaseINT:
                    return GetBaseAbility(Ability.INT);
                case Value.INT:
                    return GetValue(Value.BaseINT) / 3;
                case Value.MaxHP:
                    return StaMainJob.HP + StaRace.HP + (Rank - 1) * StaMainJob.Other;
                case Value.StartFate:
                    return StaRace.Other;
                case Value.Attack:
                    return 0;
                case Value.Magic:
                    return 0;
                case Value.Recovary:
                    return 0;
                case Value.PhyDefense:
                    return GetValue(Value.STR) * 2;
                case Value.MagDefense:
                    return GetValue(Value.INT) * 2;
                case Value.Behavior:
                    return GetValue(Value.STR) + GetValue(Value.INT);
                case Value.MovePoint:
                    return 2;
                default:
                    return 0;
            }
        }

        public override int GetValue(Value value)
        {
            if (FuncReplaceValues[value] != null) return FuncReplaceValues[value]();
            return GetBaseValue(value) + FuncBuffValues[value].Sum();
        }

        public override int GetAbility(Ability abi)
        {
            switch (abi)
            {
                case Ability.STR:
                    return GetValue(Value.STR);
                case Ability.DEX:
                    return GetValue(Value.DEX);
                case Ability.POW:
                    return GetValue(Value.POW);
                case Ability.INT:
                    return GetValue(Value.INT);
                default:
                    return 0;
            }
        }
    }
}
