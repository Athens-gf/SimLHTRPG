using System;
using System.Diagnostics;
using System.Linq;
using AthensUtility;

namespace LHTRPG
{
    [DebuggerDisplay("{ToString()}")]
    /// <summary> ダイス数値 </summary>
    public class DiceNumber
    {
        /// <summary> ダイス個数 </summary>
        public int Dice { get; set; } = 0;

        /// <summary> 固定値 </summary>
        public int FixedNumber { get; set; } = 0;

        /// <summary> ダイスが使われているかどうか </summary>
        public bool IsUseDice => Dice != 0;

        public override string ToString() => Dice == 0 ? FixedNumber.FullWidth() : $"{Dice.FullWidth()}Ｄ{FixedNumber.FullWidth(true)}";

        public static implicit operator string(DiceNumber dNum) => dNum.ToString();

        /// <summary> ダイス個数の暗黙的変換、文字列 </summary>
        public static implicit operator DiceNumber(string str)
        {
            var ss = str.Split('Ｄ');
            int f = 0;
            if (ss.Length == 2)
            {
                if (int.TryParse(ss[0], out int d) && (ss[1] == "" || int.TryParse(ss[1], out f)))
                    return new DiceNumber { Dice = d, FixedNumber = f };
            }
            else if (ss.Length == 1 && (ss[0] == "" || int.TryParse(ss[0], out f)))
                return new DiceNumber { FixedNumber = f };

            throw new Exception($"this string \"{str}\" can't change dice number");
        }

        /// <summary> ダイス個数の暗黙的変換、整数値ならダイス数0個としてみなす </summary>
        public static implicit operator DiceNumber(int num) => new DiceNumber { FixedNumber = num };

        /// <summary> ダイス個数・固定値同士の加算 </summary>
        public static DiceNumber operator +(DiceNumber dn0, DiceNumber dn1) => new DiceNumber
        {
            Dice = dn0.Dice + dn1.Dice,
            FixedNumber = dn0.FixedNumber + dn1.FixedNumber
        };

        /// <summary> ダイス個数・固定値同士の減算 </summary>
        public static DiceNumber operator -(DiceNumber dn0, DiceNumber dn1) => new DiceNumber
        {
            Dice = dn0.Dice - dn1.Dice,
            FixedNumber = dn0.FixedNumber - dn1.FixedNumber
        };

        /// <summary> ダイス結果を返す </summary>
        /// <returns>1~6の乱数</returns>
        public static int GetDice() => UnityEngine.Random.Range(1, 7);

        /// <summary> ロール結果を取得する </summary>
        public RollResult Roll() => new RollResult(Enumerable.Range(0, Dice).Select(_ => GetDice()).ToList(), FixedNumber);
    }
}
