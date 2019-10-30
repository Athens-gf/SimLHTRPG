using EnumExtension;

namespace LHTRPG
{
    public enum Person
    {
        /// <summary> 一般人 </summary>
        [EnumText("一般人")] Common,
        /// <summary> 職人 </summary>
        [EnumText("職人")] Craftsman,
        /// <summary> 商人 </summary>
        [EnumText("商人")] Merchant,
        /// <summary> 武人 </summary>
        [EnumText("武人")] Warrior,
        /// <summary> 為政者 </summary>
        [EnumText("為政者")] Politician,
        /// <summary> 知識人 </summary>
        [EnumText("知識人")] Intellectuals,
        /// <summary> 自由人 </summary>
        [EnumText("自由人")] Free,
        /// <summary> 芸術家 </summary>
        [EnumText("芸術家")] Artist,
    }

    /// <summary> 人物タグ </summary>
    public class TagPerson : Tag<Person> { public TagPerson(Person type) : base(type) { } }
}
