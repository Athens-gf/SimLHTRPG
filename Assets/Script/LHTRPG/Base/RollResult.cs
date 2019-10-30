using System.Collections.Generic;
using System.Linq;
using AthensUtility;

namespace LHTRPG
{
    /// <summary> ダイス結果 </summary>
    public class RollResult
    {
        /// <summary> ダイス結果 </summary>
        public IList<int> Dices { get; set; }

        /// <summary> 固定値 </summary>
        public int FixedNumber { get; set; }

        /// <summary> 合計値 </summary>
        public int Sum => Dices.Sum() + FixedNumber;

        /// <summary> クリティカルかどうか </summary>
        public bool IsCritical(Unit unit) => !IsFumble(unit) && Dices.Count(i => i >= 6) >= 2;

        /// <summary> ファンブルかどうか </summary>
        public bool IsFumble(Unit unit) => unit.IsExistStatus(Status.Prosperity) ? Dices.Any(i => i <= 1) : Dices.All(i => i <= 1);

        public RollResult(List<int> dices, int fixedNumber)
        {
            Dices = dices.AsReadOnly();
            FixedNumber = fixedNumber;
        }
    }
}
