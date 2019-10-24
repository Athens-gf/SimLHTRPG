using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using UnityEngine;
using KM.Utility;


namespace LHTRPG
{
    public static partial class LHTRPGBase
    {
        /// <summary> ダイス結果を返す </summary>
        /// <returns>1~6の乱数</returns>
        public static int GetDice() { return UnityEngine.Random.Range(1, 7); }

        /// <summary> 複数個のダイス結果を返す </summary>
        /// <param name="num">ダイス個数</param>
        /// <returns>ダイス結果</returns>
        public static DiceResult GetDices(int num) => new DiceResult(Enumerable.Range(0, num).Select(_ => GetDice()).ToList());

        /// <summary> コンテナの中から1つランダムに返す </summary>
        /// <returns>ランダムに選ばれた要素</returns>
        public static T GetRand<T>(this IEnumerable<T> list) { return list.ElementAt(UnityEngine.Random.Range(0, list.Count())); }
    }

    [DebuggerDisplay("{ToString()}")]
    /// <summary> ダイス数値 </summary>
    public class DiceNumber
    {
        /// <summary> ダイス個数 </summary>
        public int Dice { get; private set; }

        /// <summary> 固定値 </summary>
        public int FixedNumber { get; private set; }

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
        public bool IsFumble(Unit unit) => unit.GetStatus(Status.Prosperity).IsExist ? Dices.Any(i => i <= 1) : Dices.All(i => i <= 1);

        public DiceResult(List<int> dices) { Dices = dices.AsReadOnly(); }
    }

    [DebuggerDisplay("{GetTagString()}")]
    /// <summary> 全てのタグを持つ基礎クラス </summary>
    public abstract class Unit
    {
        /// <summary> ユニット種別 </summary>
        public UnitType Type { get; }

        /// <summary> キャラクターかどうか </summary>
        public bool IsCharacter => Type == UnitType.Adventurer || Type == UnitType.Guest || Type == UnitType.Enemy;

        /// <summary> セッションへの参照 </summary>
        public Session Session { get; set; } = null;

        /// <summary> 攻撃対象に選べるかどうか(キャラクター、もしくは破壊可能なプロップ) </summary>
        public bool IsTarget => IsCharacter || (Type == UnitType.Prop && ((this as Prop)?.CanBreak ?? false));

        /// <summary> 保持HP </summary>
        public virtual int HP { get; set; } = 0;

        /// <summary> ダメージを受ける処理 </summary>
        public abstract void Damage(int damage, DamageType type, Unit unit);

        /// <summary> 回復する処理 </summary>
        public abstract void Heal(int heal, Unit unit);

        /// <summary> ランク </summary>
        public int Rank { get; protected set; }

        /// <summary> 基礎保持タグ </summary>
        protected abstract List<Tag> LTags { get; set; }

        /// <summary> 保持タグ </summary>
        public IEnumerable<Tag> Tags => LTags?.Concat(HaveStatus.OrderBy(s => (int)s.Status).Select(s => s as Tag)) ?? new List<Tag>();

        /// <summary> タグ文字列化 </summary>
        public string GetTagString() => Tags.Select(t => t.ToString()).Aggregate((now, next) => now + " " + next);

        public Unit(UnitType type) { Type = type; }

        /// <summary> 属性タグ一覧 </summary>
        public IEnumerable<TagElement> Elements { get { return Tags.GetTags<TagElement>().Distinct(); } }

        /// <summary> 武器タグ一覧 </summary>
        public IEnumerable<TagWeapon> Weapons { get { return Tags.GetTags<TagWeapon>().Distinct(); } }

        /// <summary> ステータスタグ一覧 </summary>
        public List<IStatusTag> HaveStatus { get; } = new List<IStatusTag>();

        /// <summary> 特定のステータスタグを取得 </summary>
        public IStatusTag GetStatus(Status status, Tag targetTag = null)
        {
            if (targetTag != null && status != Status.WeakPoint && status != Status.Mitigation)
                throw new System.Exception("Incorrect status.");
            var sts = HaveStatus.Where(t => t.Status == status);
            if (targetTag != null)
                sts = sts.Cast<IHaveTargetStatusTag>().Where(t => t.Target == targetTag).Cast<IStatusTag>();
            return sts.Any() ? sts.First() : TagStatus.Create(this, status);
        }

        /// <summary> 特定のステータスタグをキャストして取得 </summary>
        public T GetStatus<T>(Status status, Tag targetTag = null) where T : Tag, IStatusTag => GetStatus(status, targetTag) as T;

        /// <summary> 特定のステータスタグの一覧を取得 </summary>
        public IEnumerable<IStatusTag> GetStatusList(Status status, Tag target = null)
        {
            if (target != null && status != Status.WeakPoint && status != Status.Mitigation)
                throw new Exception("Incorrect status.");
            var sts = HaveStatus.Where(t => t.Status == status);
            if (target != null)
                sts = sts.Cast<IHaveTargetStatusTag>().Where(t => t.Target == target).Cast<IStatusTag>();
            return sts.Any() ? sts.ToList() : new List<IStatusTag>() { TagStatus.Create(this, status) };
        }

        /// <summary> 特定のステータスタグの一覧をキャストして取得 </summary>
        public IEnumerable<T> GetStatusList<T>(Status status, Tag target = null) where T : Tag, IStatusTag
            => GetStatusList(status, target).Cast<T>();
    }

    public class CorrectValueTuple<TVT, TN>
    {
        public TVT ValueType { get; set; }
        public Unit Unit { get; set; }
        public Func<Character, List<Unit>, bool> Check { get; set; }
        public Func<Character, List<Unit>, TN> Correct { get; set; }
    }

    /// <summary> キャラクター(冒険者・ゲスト・エネミー)共通クラス </summary>
    public abstract class Character : Unit
    {
        public Dictionary<CorrectionType, LinkedList<CorrectValueTuple<ValueType, int>>> BaseValueCorrection;
        public Dictionary<CorrectionType, LinkedList<CorrectValueTuple<ValueType, DiceNumber>>> ValueCorrection;
        public Dictionary<CorrectionType, LinkedList<CorrectValueTuple<SkillValueType, int>>> BaseSkillValueCorrection;
        public Dictionary<CorrectionType, LinkedList<CorrectValueTuple<SkillValueType, DiceNumber>>> SkillValueCorrection;

        private void SettingCorrection<TVT, TN>(Dictionary<CorrectionType, LinkedList<CorrectValueTuple<TVT, TN>>> correction)
        {
            correction = new Dictionary<CorrectionType, LinkedList<CorrectValueTuple<TVT, TN>>>();
            foreach (CorrectionType e in Enum.GetValues(typeof(CorrectionType)))
                correction[e] = new LinkedList<CorrectValueTuple<TVT, TN>>();
        }

        protected Character(UnitType type) : base(type)
        {
            SettingCorrection(BaseValueCorrection);
            SettingCorrection(ValueCorrection);
            SettingCorrection(BaseSkillValueCorrection);
            SettingCorrection(SkillValueCorrection);
        }

        protected abstract int GetBaseValue(ValueType value);

        public abstract int GetAbility(AbilityType ability);

        public abstract DiceNumber GetValue(ValueType value);

        protected virtual int GetBaseSkillValue(SkillValueType skillValue)
        {
            if (ChangeOriginalSkillValue.Any(t => t.ValueType == skillValue))
            {
                var lastCOSK = ChangeOriginalSkillValue.Last(t => t.ValueType == skillValue);
                if (lastCOSK.Check(this, target))
                    return lastCOSK.Correct(this, target);
            }
            switch (skillValue)
            {
                case SkillValueType.Exercise:
                case SkillValueType.Endurance:
                    return GetAbility(AbilityType.STR);
                case SkillValueType.Release:
                case SkillValueType.Operation:
                case SkillValueType.Avoidance:
                    return GetAbility(AbilityType.DEX);
                case SkillValueType.Perception:
                case SkillValueType.Negotiation:
                case SkillValueType.Resistance:
                    return GetAbility(AbilityType.POW);
                case SkillValueType.Knowledge:
                case SkillValueType.Analysis:
                    return GetAbility(AbilityType.INT);
                case SkillValueType.Hit:
                    return Mathf.Max(GetAbility(AbilityType.STR), GetAbility(AbilityType.DEX), GetAbility(AbilityType.POW), GetAbility(AbilityType.INT));
                default:
                    return 0;
            }
        }

        public virtual int GetSkillValue(SkillValueType skillValue)
        {
            if (FuncReplaceSkillValues[skillValue] != null) return FuncReplaceSkillValues[skillValue]();
            return GetBaseSkillValue(skillValue) + FuncBuffSkillDiceNumbers[skillValue].Sum(dn => dn.FixedNumber);
        }

        public virtual DiceNumber GetSkillDiceNumber(SkillValueType skillValue)
        {
            if (FuncReplaceSkillDiceNumbers[skillValue] != null) return FuncReplaceSkillDiceNumbers[skillValue]();
            return new DiceNumber(2 + FuncBuffSkillDiceNumbers[skillValue].Sum(dn => dn.Dice), GetSkillValue(skillValue));
        }

        public override void Damage(int damage, DamageType type, Unit unit)
        {
            throw new NotImplementedException();
        }

        public override void Heal(int heal, Unit unit)
        {
            throw new NotImplementedException();
        }
    }
}
