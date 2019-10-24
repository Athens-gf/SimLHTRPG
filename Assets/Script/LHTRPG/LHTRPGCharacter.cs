using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnumExtension;
using KM.Unity;
using KM.Utility;

using Ability = System.Collections.Generic.Dictionary<LHTRPG.Character.Ability, int>;

namespace LHTRPG
{
    public static partial class LHTRPGBase
    {
        public static ArchetypeJob GetArchetype(this MainJob _mj)
        {
            switch (_mj)
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

        public static TagRace GetTag(this Race _race, Unit _unit) { return new TagRace(_unit, _race); }
        public static TagMainJob GetTag(this MainJob _mj, Unit _unit) { return new TagMainJob(_unit, _mj); }
    }

    public class Sex : Tag
    {
        public enum Sex
        {
            [EnumText("男性")] Male,
            [EnumText("女性")] Female,
            [EnumText("その他")] Other,
        }
        public Sex Type { get; private set; }
        public string OtherName { get; private set; }
        public Sex(bool _isMale) : this(null, _isMale) { }
        public Sex(Unit _unit, bool _isMale) : base(_unit, (_isMale ? Sex.Male : Sex.Female).GetText()) { Type = _isMale ? Sex.Male : Sex.Female; }
        public Sex(string _other) : this(null, _other) { }
        public Sex(Unit _unit, string _other) : base(_unit, _other) { Type = Sex.Other; OtherName = _other; }
    }

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

    public class TagRace : Tag
    {
        public Race Race { get; private set; }
        public TagRace(Unit _unit, Race _race) : base(_unit, _race.GetText()) { Race = _race; }
    }

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

    public class TagMainJob : Tag
    {
        public MainJob MainJob { get; private set; }
        public TagMainJob(Unit _unit, MainJob _mj) : base(_unit, _mj.GetText()) { MainJob = _mj; }
    }

    public class SubJob : Tag
    {
        public static ReadOnlyCollection<string> SubJobs { get; private set; }
        static SubJob()
        {
            var csv = new CSVReader(@"Data/SubJobs");
            var l = new List<string>();
            for (int i = 0; i < csv.Row; i++) l.Add(csv[i][0]);
            SubJobs = new ReadOnlyCollection<string>(l);
        }
        public static SubJob GetRand(Unit _unit) { return new SubJob(_unit, SubJobs.GetRand()); }

        public string BaseSubJob { get; private set; }
        public SubJob(string _name) : this((Unit)null, _name) { }
        public SubJob(Unit _unit, string _name) : base(_unit, _name) { BaseSubJob = _name; }
        public SubJob(string _name, string _base) : this(null, _name, _base) { }
        public SubJob(Unit _unit, string _name, string _base) : base(_unit, _name) { BaseSubJob = _base; }
    }

    public class GuidingCreed
    {
        public struct Creed_s
        {
            public string Name { get; set; }
            public string Creed { get; set; }
            public TagPerson.Type Tag { get; set; }
        }
        public static ReadOnlyCollection<Creed_s> Creeds { get; private set; }
        public static List<string> CreedNames { get { return Creeds.Select(c => c.Name).ToList(); } }
        static GuidingCreed()
        {
            var csv = new CSVReader(@"Data/Creed");
            var lb = new List<Creed_s>();
            foreach (var line in csv.Line())
                lb.Add(new Creed_s { Name = line[0], Creed = line[1], Tag = line[2].GetEnumByText<TagPerson.Type>() });
            Creeds = new ReadOnlyCollection<Creed_s>(lb);
        }
        public static GuidingCreed GetRand(Unit _unit) { return new GuidingCreed(_unit, Creeds.GetRand()); }

        public Unit Unit { get; set; }
        public Creed_s Creed { get; private set; }
        public TagPerson Tag { get; private set; }

        public GuidingCreed(string _creedName) : this(null, _creedName) { }
        public GuidingCreed(Unit _unit, string _creedName) : this(_unit, Creeds.First(c => c.Name == _creedName)) { }
        public GuidingCreed(Creed_s _creed) : this(null, _creed) { }
        public GuidingCreed(Unit _unit, Creed_s _creed) { Unit = _unit; Creed = _creed; Tag = new TagPerson(Unit, Creed.Tag); }
    }

    public interface IHumanCharacter
    {
        bool IsAdventurer { get; }
        string Name { get; set; }
        Sex Sex { get; set; }
        int Level { get; set; }
        GuidingCreed GuidingCreed { get; set; }
        Race Race { get; set; }
        SubJob SubJob { get; set; }
        List<Tag> OtherTags { get; }
    }

    public class Extra : Unit, IHumanCharacter
    {
        public Extra() : base(Type.Extra) { OtherTags = new List<Tag>(); }

        public bool IsAdventurer { get; set; }
        public string Name { get; set; }
        private Sex sex;
        public Sex Sex { get { return sex; } set { sex = value.Type != Sex.Sex.Other ? new Sex(this, value.Type == Sex.Sex.Male) : new Sex(this, value.OtherName); } }
        public int Level { get; set; }
        private GuidingCreed guidingCreed;
        public GuidingCreed GuidingCreed { get { return guidingCreed; } set { guidingCreed = new GuidingCreed(this, value.Creed); } }
        public Race Race { get; set; }
        private SubJob subJob;
        public SubJob SubJob { get { return subJob; } set { subJob = new SubJob(this, value.Name, value.BaseSubJob); } }
        public List<Tag> OtherTags { get; private set; }

        protected override List<Tag> LTags
        {
            get
            {
                return (new Tag(this, IsAdventurer ? "冒険者" : "大地人")).MakeCollection().Append(Sex).Append(Race.GetTag(this))
                    .Append(SubJob).Append(GuidingCreed.Tag).Append(OtherTags).Where(t => t != null).ToList();
            }
            set { }
        }

        public override void Damage(int _damage, DamageType _type, Unit _unit) { }

        public override void Heal(int _heal, Unit _unit) { }
    }

    public class Guest : Adventurer
    {
        public override bool IsAdventurer { get; set; }
        public Guest() : base(Type.Guest) { }
    }

    public class Adventurer : Character, IHumanCharacter
    {
        public struct Status_t
        {
            public Ability Ability { get; set; }
            public int HP { get; set; }
            public int Other { get; set; }
        }
        public static Dictionary<MainJob, Status_t> StaMainJobs { get; private set; }
        public static Dictionary<Race, Status_t> StaRaces { get; private set; }

        static Adventurer()
        {
            var csv = new CSVReader(@"Data/StatusMainJob");
            StaMainJobs = new Dictionary<MainJob, Status_t>();
            foreach (var line in csv.Line())
            {

                StaMainJobs[line[0].GetEnumByText<MainJob>()] = new Status_t
                {
                    Ability = new Ability
                    {
                        { Ability.STR, int.Parse(line[1]) },
                        { Ability.DEX, int.Parse(line[2]) },
                        { Ability.POW, int.Parse(line[3]) },
                        { Ability.INT, int.Parse(line[4]) }
                    },
                    HP = int.Parse(line[5]),
                    Other = int.Parse(line[6])
                };
            }
            csv = new CSVReader(@"Data/StatusRace");
            StaRaces = new Dictionary<Race, Status_t>();
            foreach (var line in csv.Line())
            {
                StaRaces[line[0].GetEnumByText<Race>()] = new Status_t
                {
                    Ability = new Ability
                    {
                        { Ability.STR, int.Parse(line[1]) },
                        { Ability.DEX, int.Parse(line[2]) },
                        { Ability.POW, int.Parse(line[3]) },
                        { Ability.INT, int.Parse(line[4]) }
                    },
                    HP = int.Parse(line[5]),
                    Other = int.Parse(line[6])
                };
            }
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

        public Status_t StaMainJob { get { return StaMainJobs[MainJob]; } }
        public Status_t StaRace { get { return StaRaces[Race]; } }
        public Ability AbiHuman { get; set; }
        public Ability AbiBonus { get; set; }
        public int SumAbiBonus { get { return AbiBonus.Sum(a => a.Value); } }

        protected Adventurer(Type _type) : base(_type)
        {
            Connection = new List<Character>();
            HistorySubJob = new List<SubJob>();
            Union = null;
            OtherTags = new List<Tag>();
            AbiBonus = new Ability { { Ability.STR, 0 }, { Ability.DEX, 0 }, { Ability.POW, 0 }, { Ability.INT, 0 } };
            AbiHuman = new Ability { { Ability.STR, 0 }, { Ability.DEX, 0 }, { Ability.POW, 0 }, { Ability.INT, 0 } };
        }

        public Adventurer() : this(Type.Adventurer) { }

        private int GetBaseAbility(Ability _ba) { return StaMainJob.Ability[_ba] + StaRace.Ability[_ba] + AbiHuman[_ba] + AbiBonus[_ba] + Rank - 1; }

        protected override int GetBaseValue(Value _value)
        {
            if (FuncReplaceBaseValues[_value] != null) return FuncReplaceBaseValues[_value]();
            switch (_value)
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

        public override int GetValue(Value _value)
        {
            if (FuncReplaceValues[_value] != null) return FuncReplaceValues[_value]();
            return GetBaseValue(_value) + FuncBuffValues[_value].Sum();
        }

        public override int GetAbility(Ability _abi)
        {
            switch (_abi)
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
