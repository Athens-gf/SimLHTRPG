using EnumExtension;

namespace LHTRPG
{
    public enum BehaviorType
    {
        /// <summary> 未行動 </summary>
        [EnumText("未行動")] NotYet,
        /// <summary> 行動済み </summary>
        [EnumText("行動済み")] Already,
        /// <summary> 待機 </summary>
        [EnumText("待機")] Waiting,
    }

    /// <summary> 未行動／行動済み／待機 </summary>
    public class TagStatusBehavior : TagStatus
    {
        public BehaviorType Type { get; set; }

        public TagStatusBehavior(BehaviorType type = BehaviorType.NotYet) : base(Status.Behavior) => Type = type;

        public override string Name => Type.GetText();
    }
}
