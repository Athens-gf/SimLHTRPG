using EnumExtension;
using System.Collections.Generic;
using System.Linq;

namespace LHTRPG
{
    /// <summary> コストとしてＮ以下の【因果力】を消費する（Ｍでもよい）。 </summary>
    public class CostTextAnyFate : Cost.Text
    {
        /// <summary> 上限 </summary>
        public RankNumber UpperLimit { get; set; } = RankNumber.Base0;

        /// <summary> 下限 </summary>
        public int UnderLimit { get; set; } = 0;

        /// <summary> コスト数 </summary>
        public int Value { get; set; } = 0;

        /// <summary> 実際に支払う因果力コスト </summary>
        public override int CostFate => Value;
    }

    /// <summary> 対象は［〇〇］［✕✕］タグを持つ「タイミング：～～」の行動を即座に１回実行する。このとき、対象の行動のヘイト上昇はあなたが代わりに請け負う。 </summary>
    public class CostTextHateOfOtherAction : Cost.Text
    {

    }

}
