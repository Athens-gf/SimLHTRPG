using AthensUtility;
using System;

namespace LHTRPG
{
    /// <summary> ランクを含んだ数値 </summary>
    public struct RankNumber
    {
        /// <summary> 数値 </summary>
        public int Value { get; set; }

        /// <summary> ランク参照かどうか </summary>
        public bool IsRank { get; set; }

        /// <summary> 0かどうか </summary>
        public bool IsZero => !IsRank && Value == 0;

        /// <summary> 1かどうか </summary>
        public bool IsOne => !IsRank && Value == 1;

        public RankNumber(int value, bool isRank) { Value = value; IsRank = isRank; }

        public string ToString(bool isSign = false) => IsRank ? $"［ＳＲ{(Value == 0 ? "" : Value.FullWidth(true))}］" : Value.FullWidth(isSign);

        public static implicit operator RankNumber(int value) => new RankNumber(value, false);

        public static implicit operator RankNumber(string str)
        {
            if (str.StartsWith("［", StringComparison.CurrentCulture)
                && str.EndsWith("］", StringComparison.CurrentCulture))
                str = str.Remove("［").Remove("］");
            var isRank = str.Contains("ＳＲ");
            if (isRank)
                str = str.Remove("ＳＲ");
            if (!Extensions.TryParseFullWidth(str, out int value))
                throw new SystemException();
            return new RankNumber(value, isRank);
        }

        /// <summary> 実際の数値を獲得する </summary>
        /// <param name="unit">CR、SR、IR等</param>
        public int GetValue(Unit unit) => (IsRank ? unit.Rank : 0) + Value;

        public static RankNumber Base0 => new RankNumber(0, false);
    }
}
