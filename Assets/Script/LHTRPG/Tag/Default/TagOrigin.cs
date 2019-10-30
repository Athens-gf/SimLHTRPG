using EnumExtension;

namespace LHTRPG
{
    public enum Origin
    {
        /// <summary> 機械 </summary>
        [EnumText("機械")] Mashine,
        /// <summary> 天然 </summary>
        [EnumText("天然")] Natural,
        /// <summary> 魔法 </summary>
        [EnumText("魔法")] Magic,
        //          [EnumText("出自可変")] Variable,
    }

    /// <summary> 出自タグ </summary>
    public class TagOrigin : Tag<Origin> { public TagOrigin(Origin type) : base(type) { } }
}
