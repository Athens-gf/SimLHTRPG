using EnumExtension;

namespace LHTRPG
{
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
}
