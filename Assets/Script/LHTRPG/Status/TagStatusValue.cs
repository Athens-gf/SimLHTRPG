using EnumExtension;

namespace LHTRPG
{
    /// <summary> 数量を持つステータスタグ </summary>
    public class TagStatusValue : TagValue, IStatusTag
    {
        /// <summary> ステータス種別 </summary>
        public Status Status { get; }

        /// <summary> ステータスカテゴリー </summary>
        public virtual StatusCategory Category => Status.GetCategory();

        public TagStatusValue(TagStatusType type, Status status) : base(status.GetText(), type, 0) => Status = status;

        protected TagStatusValue(TagStatusType type, string name) : base(name, type, 0) => Status = Status.Other;
    }
}
