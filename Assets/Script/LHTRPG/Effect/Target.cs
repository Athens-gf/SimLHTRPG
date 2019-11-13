using System;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using AthensUtility;

namespace LHTRPG
{
    public enum TargetType
    {
        [EnumText("自身")] Myself,
        [EnumText("個体")] Yourself,
        [EnumText("範囲")] Range,
        [EnumText("広範囲")] WideRange,
        [EnumText("直線")] Line,
        [EnumText("本文")] Text,
    }

    /// <summary> 対象 </summary>
    public class Target
    {
        public TargetType Type { get; set; }

        public RankNumber Value { get; set; }

        public bool IsSelect { get; set; }

        public Target(TargetType type) : this(type, RankNumber.Base0) { }

        public Target(TargetType type, RankNumber value) : this(type, value, true) { }

        public Target(TargetType type, RankNumber value, bool isSelect) { Type = type; Value = value; IsSelect = isSelect; }

        public override string ToString()
        {
            switch (Type)
            {
                case TargetType.Yourself:
                    return Value.IsZero ? "単体" : Value + "体";
                case TargetType.WideRange:
                case TargetType.Line:
                    return Type.GetText() + Value;
                default:
                    return Type.GetText();
            }
        }
    }
}
