using System.Collections.Generic;
using System.Linq;
using AthensUtility;
using EnumExtension;

namespace LHTRPG
{
    public static partial class LHTRPGBase
    {
        /// <summary> コンテナの中から1つランダムに返す </summary>
        /// <returns>ランダムに選ばれた要素</returns>
        public static T GetRand<T>(this IEnumerable<T> list) => list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
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
