namespace LHTRPG
{
    /// <summary> マジックアイテムグレードタグ </summary>
    public class TagMagicGrade : TagValue
    {
        public TagMagicGrade(int value) : base("M", TagStatusType.None, value) { }
        public override string ToString() { return "[" + Name + Value + "]"; }
    }
}
