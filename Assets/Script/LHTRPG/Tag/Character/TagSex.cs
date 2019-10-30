using EnumExtension;

namespace LHTRPG
{
    public enum Sex
    {
        [EnumText("男性")] Male,
        [EnumText("女性")] Female,
    }

    /// <summary> 性別タグ </summary>
    public class TagSex : Tag<Sex> { public TagSex(Sex type) : base(type) { } }
}
