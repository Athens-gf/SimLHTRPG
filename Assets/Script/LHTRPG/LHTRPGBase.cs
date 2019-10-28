using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using AthensUtility;
using EnumExtension;

namespace LHTRPG
{
    public static partial class LHTRPGBase
    {
        /// <summary> ダイス結果を返す </summary>
        /// <returns>1~6の乱数</returns>
        public static int GetDice() => UnityEngine.Random.Range(1, 7);

        /// <summary> 複数個のダイス結果を返す </summary>
        /// <param name="num">ダイス個数</param>
        /// <returns>ダイス結果</returns>
        public static DiceResult GetDices(int num) => new DiceResult(Enumerable.Range(0, num).Select(_ => GetDice()).ToList());

        /// <summary> コンテナの中から1つランダムに返す </summary>
        /// <returns>ランダムに選ばれた要素</returns>
        public static T GetRand<T>(this IEnumerable<T> list) => list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
    }

    [DebuggerDisplay("{ToString()}")]
    /// <summary> ダイス数値 </summary>
    public class DiceNumber
    {
        /// <summary> ダイス個数 </summary>
        public int Dice { get; }

        /// <summary> 固定値 </summary>
        public int FixedNumber { get; }

        /// <summary> ダイスが使われているかどうか </summary>
        public bool IsUseDice => Dice != 0;

        /// <summary> コンストラクタ </summary>
        /// <param name="dice">ダイス個数</param>
        /// <param name="fixedNumber">固定値</param>
        public DiceNumber(int dice, int fixedNumber) { Dice = dice; FixedNumber = fixedNumber; }

        public override string ToString() => Dice == 0 ? $"{FixedNumber}" : $"{Dice}D{FixedNumber.ToString("+#;-#;")}";

        public static implicit operator string(DiceNumber dNum) => dNum.ToString();

        /// <summary> ダイス個数の暗黙的変換、文字列 </summary>
        public static implicit operator DiceNumber(string str)
        {
            var ss = str.Split('D').ToList();
            int c = 0;
            if (ss.Count == 2)
            {
                if (int.TryParse(ss[0], out int d) && (ss[1] == "" || int.TryParse(ss[1], out c)))
                    return new DiceNumber(d, c);
            }
            else if (ss.Count == 1 && (ss[0] == "" || int.TryParse(ss[0], out c)))
                return new DiceNumber(0, c);

            throw new Exception($"this string \"{str}\" can't change dice number");
        }

        /// <summary> ダイス個数の暗黙的変換、整数値ならダイス数0個としてみなす </summary>
        public static implicit operator DiceNumber(int num) => new DiceNumber(0, num);

        /// <summary> ダイス個数・固定値同士の加算 </summary>
        public static DiceNumber operator +(DiceNumber dn0, DiceNumber dn1) => new DiceNumber(dn0.Dice + dn1.Dice, dn0.FixedNumber + dn1.FixedNumber);

        /// <summary> ダイス個数・固定値同士の減算 </summary>
        public static DiceNumber operator -(DiceNumber dn0, DiceNumber dn1) => new DiceNumber(dn0.Dice - dn1.Dice, dn0.FixedNumber - dn1.FixedNumber);
    }

    /// <summary> ダイス結果 </summary>
    public class DiceResult
    {
        /// <summary> ダイス結果 </summary>
        public IList<int> Dices { get; }

        /// <summary> 合計値 </summary>
        public int Sum => Dices.Sum();

        /// <summary> クリティカルかどうか </summary>
        public bool IsCritical(Unit unit) => !IsFumble(unit) && Dices.Count(i => i >= 6) >= 2;

        /// <summary> ファンブルかどうか </summary>
        public bool IsFumble(Unit unit) => unit.IsExistStatus(Status.Prosperity) ? Dices.Any(i => i <= 1) : Dices.All(i => i <= 1);

        public DiceResult(List<int> dices) { Dices = dices.AsReadOnly(); }
    }


    public enum ActionType
    {
        /// <summary> 動作 </summary>
        Action,
        /// <summary> 特技 </summary>
        Skill,
        /// <summary> アイテム </summary>
        Item,
        /// <summary> EXパワー </summary>
        EXPower,
        /// <summary> その他の行動 </summary>
        Other,
    }

    public interface IAction
    {
        ActionType Type { get; }
    }

    public enum DamageType
    {
        /// <summary> 物理ダメージ </summary>
        [EnumText("物理")] Physics,
        /// <summary> 魔法ダメージ </summary>
        [EnumText("魔法")] Magic,
        /// <summary> 貫通ダメージ </summary>
        [EnumText("貫通")] Through,
        /// <summary> 直接ダメージ </summary>
        [EnumText("直接")] Directly,
    }
}
