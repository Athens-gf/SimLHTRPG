using System.Diagnostics;

namespace LHTRPG
{
    [DebuggerDisplay("{ToString()}")]
    /// <summary> 数量を持ったタグ基本クラス </summary>
    public class TagValue : Tag
    {
        /// <summary> 同じタグが追加されたときの動作 </summary>
        public TagStatusType Type { get; }

        /// <summary> 数値 </summary>
        public virtual int Value { get; set; }

        /// <summary> 重複存在可能かどうか </summary>
        public override bool IsCanOverlap => Type == TagStatusType.Overlap;

        public override string ToString() { return "[" + Name + "：" + Value + "]"; }

        public TagValue(string name, TagStatusType type, int value) : base(name) { Type = type; Value = value; }
    }
}
