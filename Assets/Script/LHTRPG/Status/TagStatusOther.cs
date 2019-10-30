namespace LHTRPG
{
    /// <summary> (数量を持たない)その他のステータスタグ </summary>
    public class TagStatusOther : TagStatus
    {
        protected StatusCategory category;

        /// <summary> ステータスカテゴリー </summary>
        public override StatusCategory Category => category;

        public TagStatusOther(StatusCategory cat, string name) : base(name) => category = cat;

        public override string ToString() => Name;
    }
}
