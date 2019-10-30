using EnumExtension;

namespace LHTRPG
{
    public enum LargeRace
    {
        /// <summary> 人間 </summary>
        [EnumText("人間")] Human,
        /// <summary> 人型 </summary>
        [EnumText("人型")] Humanoid,
        /// <summary> 自然 </summary>
        [EnumText("自然")] Naure,
        /// <summary> 精霊 </summary>
        [EnumText("精霊")] Spirit,
        /// <summary> 幻獣 </summary>
        [EnumText("幻獣")] IllusionalBeast,
        /// <summary> 不死 </summary>
        [EnumText("不死")] Undead,
        /// <summary> 人造 </summary>
        [EnumText("人造")] Artificial,
        /// <summary> ギミック </summary>
        [EnumText("ギミック")] Gimmick,
    }

    /// <summary> 大種族タグ </summary>
    public class TagLargeRace : Tag<LargeRace> { public TagLargeRace(LargeRace type) : base(type) { } }
}
