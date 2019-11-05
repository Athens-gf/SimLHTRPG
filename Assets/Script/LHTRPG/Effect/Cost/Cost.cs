using EnumExtension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LHTRPG
{
    /// <summary> コスト </summary>
    public class Cost
    {
        public enum Type
        {
            [EnumText("ヘイト")] Hate,
            [EnumText("因果力")] Fate,
            [EnumText("本文")] Text,
            [EnumText("ヘイト本文")] HateText,
        }

        /// <summary> パーティ全体が請け負うコストかどうか </summary>
        public bool IsParty { get; set; } = false;

        /// <summary> 任意の仲間1人も請け負うコストかどうか </summary>
        public bool IsBuddy { get; set; } = false;

        public List<CostElemet> Values { get; } = new List<CostElemet>();

        public override string ToString()
        {
            if (!Values.Any()) return "－";
            return string.Join("＆", Values.Select(v => v.ToString()));
        }

        /// <summary> 内部コストクラス </summary>
        public abstract class CostElemet
        {
            /// <summary> 種別 </summary>
            public Type Type { get; }

            public CostElemet(Type type) { Type = type; }

            public override abstract string ToString();

            /// <summary> 実際に支払うヘイトコスト </summary>
            public virtual int CostHate => 0;

            /// <summary> 実際に支払う因果力コスト </summary>
            public virtual int CostFate => 0;

            /// <summary> 実際に支払うタグコスト </summary>
            public virtual List<Tuple<Tag, int>> CostTag => null;

            //// <summary> 実際に支払うアイテムコスト </summary>
        }

        /// <summary> ヘイト </summary>
        public class Hate : CostElemet
        {
            public Hate() : base(Type.Hate) { }

            /// <summary> 数量 </summary>
            public RankNumber Value { get; set; } = RankNumber.Base0;

            public override string ToString() => $"{Type.GetText()}{Value}";

            /// <summary> 実際に支払うヘイトコスト </summary>
            public override int CostHate => Value.Value;
        }

        /// <summary> 因果力 </summary>
        public class Fate : CostElemet
        {
            public Fate() : base(Type.Fate) { }

            /// <summary> 数量 </summary>
            public RankNumber Value { get; set; } = RankNumber.Base0;

            public override string ToString() => $"{Type.GetText()}{Value}";

            /// <summary> 実際に支払う因果力コスト </summary>
            public override int CostFate => Value.Value;
        }

        /// <summary> 本文 </summary>
        public abstract class Text : CostElemet
        {
            public Text() : base(Type.Text) { }

            public override string ToString() => Type.GetText();
        }

        /// <summary> ヘイト本文 </summary>
        public abstract class HateText : CostElemet
        {
            public HateText() : base(Type.HateText) { }

            public override string ToString() => Type.GetText();

            /// <summary> 実際に支払うヘイトコスト </summary>
            public override abstract int CostHate { get; }
        }
    }
}
