namespace LHTRPG
{
    /// <summary> 魔触媒タグ </summary>
    public class TagMagicCatalyst : TagValue
    {
        public TagMagicCatalyst(int value) : base("魔触媒", TagStatusType.None, value) { }
        public override string ToString() { return "[" + Name + " " + Value + "]"; }
    }
}
