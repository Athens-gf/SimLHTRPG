using System;
using System.Collections.Generic;

namespace LHTRPG
{
    public enum CorType
    {
        /// <summary> 元の数値を置き換え </summary>
        ChangeOriginal,
        /// <summary> 加減算 </summary>
        AddSub,
        /// <summary> 最終数値を置き換え </summary>
        Replace,
    }

    /// <summary> 修正値の組み合わせ </summary>
    public class CorTuple<TVT, TN>
    {
        /// <summary> 種別 </summary>
        public TVT Type { get; set; }

        /// <summary> 発生源 </summary>
        public Unit Source { get; set; }

        /// <summary> 効果が有効かどうかのチェック
        /// (対象) => 判定結果 </summary>
        public Func<Character, bool> Check { get; set; } = c => true;

        /// <summary> 補正値
        /// (対象) => 補正値 </summary>
        public Func<Character, TN> Correct { get; set; }
    }

    /// <summary> 修正値の組み合わせ(判定) </summary>
    public class CorTupleJudgement
    {
        /// <summary> 種別 </summary>
        public SkillValueType Type { get; set; }

        /// <summary> 発生源 </summary>
        public Unit Source { get; set; }

        /// <summary> 効果が有効かどうかのチェック
        /// (対象, 目標) => 判定結果 </summary>
        public Func<Character, List<Unit>, bool> Check { get; set; } = (c, t) => true;

        /// <summary> 補正値
        /// (対象, 目標) => 補正値 </summary>
        public Func<Character, List<Unit>, DiceNumber> Correct { get; set; }
    }

    /// <summary> 修正値の組み合わせ(ダメージ) </summary>
    public class CorTupleDamage
    {
        /// <summary> 発生源 </summary>
        public Unit Source { get; set; }

        /// <summary> 効果が有効かどうかのチェック
        /// (対象, 目標) => 判定結果 </summary>
        public Func<Character, List<Unit>, bool> Check { get; set; } = (c, t) => true;

        /// <summary> 補正値
        /// (対象, 目標) => 補正値 </summary>
        public Func<Character, List<Unit>, DiceNumber> Correct { get; set; }
    }

    /// <summary> 修正値のリスト </summary>
    public class CorValues<T>
    {
        public Dictionary<CorType, LinkedList<T>> Values { get; }

        public CorValues()
        {
            Values = new Dictionary<CorType, LinkedList<T>>();
            foreach (CorType e in Enum.GetValues(typeof(CorType)))
                Values[e] = new LinkedList<T>();
        }

        public LinkedList<T> this[CorType type] => Values[type];
    }
}
