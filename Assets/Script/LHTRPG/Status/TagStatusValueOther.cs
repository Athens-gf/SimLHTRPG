namespace LHTRPG
{
    /// <summary> 数量を持つその他のステータスタグ </summary>
    public class TagStatusValueOther : TagStatusValue
    {
        protected StatusCategory category;

        /// <summary> ステータスカテゴリー </summary>
        public override StatusCategory Category => category;

        public TagStatusValueOther(StatusCategory cat, TagStatusType type, string name) : base(type, name) => category = cat;

        public override string ToString() => Name;
    }

}
