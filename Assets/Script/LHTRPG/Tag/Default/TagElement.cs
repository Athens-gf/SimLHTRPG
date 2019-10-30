using EnumExtension;

namespace LHTRPG
{
    public enum Element
    {
        /// <summary> 火炎 </summary>
        [EnumText("火炎")] Flame,
        /// <summary> 冷気 </summary>
        [EnumText("冷気")] Cold,
        /// <summary> 電撃 </summary>
        [EnumText("電撃")] Lightning,
        /// <summary> 光輝 </summary>
        [EnumText("光輝")] Shine,
        /// <summary> 邪毒 </summary>
        [EnumText("邪毒")] EvilPoison,
        /// <summary> 精神 </summary>
        [EnumText("精神")] Mind,
    }

    /// <summary> 属性タグ </summary>
    public class TagElement : Tag<Element> { public TagElement(Element type) : base(type) { } }
}
