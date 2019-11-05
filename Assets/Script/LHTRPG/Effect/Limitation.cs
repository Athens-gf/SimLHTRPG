using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumExtension;

namespace LHTRPG
{
    public enum LimitedType
    {
        [EnumText("－")] None,
        [EnumText("シナリオ")] Scenario,
        [EnumText("シーン")] Scene,
        [EnumText("ラウンド")] Round,
        [EnumText("［パーティ］")] Party,
        [EnumText("本文")] Text,
    }

    /// <summary> 制限 </summary>
    public class Limitation
    {
        /// <summary> 制限種別 </summary>
        public LimitedType Type { get; set; }

        /// <summary> 数値 </summary>
        public RankNumber Value { get; set; }

        public Limitation(LimitedType type) : this(type, RankNumber.Base0) { }

        public Limitation(LimitedType type, RankNumber value) { Type = type; Value = value; }

        public override string ToString()
        {
            switch (Type)
            {
                case LimitedType.Scenario:
                case LimitedType.Scene:
                case LimitedType.Round:
                    return Type.GetText() + Value + "回";
                default:
                    return Type.GetText();
            }
        }
    }
}
