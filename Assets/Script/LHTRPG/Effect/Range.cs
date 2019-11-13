using System;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using AthensUtility;

namespace LHTRPG
{
    public enum RangeType
    {
        [EnumText("距離")] Distance,
        [EnumText("武器")] Weapon,
        [EnumText("本文")] Text,
    }

    /// <summary> 射程 </summary>
    public class Range
    {
        public RangeType Type { get; set; }
        public RankNumber Value { get; set; }

        public Range(RangeType type) : this(type, RankNumber.Base0) { }

        public Range(RangeType type, RankNumber value) { Type = type; Value = value; }

        public override string ToString()
        {
            if (Type == RangeType.Distance)
                return Value.IsZero ? "至近" : Value + "Ｓｑ";
            return Type.GetText();
        }
    }
}
