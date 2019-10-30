using EnumExtension;

namespace LHTRPG
{
    public enum Armor
    {
        /// <summary> 軽鎧 </summary>
        [EnumText("軽鎧")] Light,
        /// <summary> 中鎧 </summary>
        [EnumText("中鎧")] Medium,
        /// <summary> 重鎧 </summary>
        [EnumText("重鎧")] Heavy,
    }

    /// <summary> 鎧種別タグ </summary>
    public class TagArmor : Tag<Armor> { public TagArmor(Armor type) : base(type) { } }
}
