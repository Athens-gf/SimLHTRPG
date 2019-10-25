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
    public class CorValues<T>
    {
        public Dictionary<CorType, LinkedList<T>> Values { get; }

        public CorValues()
        {
            Values = new Dictionary<CorType, LinkedList<T>>();
            foreach (CorType e in Enum.GetValues(typeof(CorType)))
                Values[e] = new LinkedList<T>();
        }
    }

    /// <summary> キャラクター(冒険者・ゲスト・エネミー)共通クラス </summary>
    public abstract class Character : Unit
    {
        /// <summary> 数値系の修正値 </summary>
        public CorValues<CorTuple<BattleStatusType, int>> CorBattleStatus { get; } = new CorValues<CorTuple<BattleStatusType, int>>();
        /// <summary> 技能値の修正値 </summary>
        public CorValues<CorTuple<SkillValueType, int>> CorSkillValue { get; } = new CorValues<CorTuple<SkillValueType, int>>();
        /// <summary> 技能判定の修正値 </summary>
        public CorValues<CorTuple<SkillValueType, DiceNumber>> CorJudgeValue { get; } = new CorValues<CorTuple<SkillValueType, DiceNumber>>();
        /// <summary> ダメージロールの修正値 </summary>
        public CorValues<CorTupleDamage> CorDamageRoll { get; } = new CorValues<CorTupleDamage>();

        protected Character(UnitType type) : base(type) { }

        /// <summary> 数値系の基礎数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>基礎数値</returns>
        protected abstract int GetBaseBattleStatus(BattleStatusType type);

        /// <summary> 数値系の修正入り最終数値を取得する </summary>
        /// <param name="type">種類</param>
        /// <returns>最終数値</returns>
        public virtual int GetBattleStatus(BattleStatusType type)
        {
            if (CorBattleStatus.Values[CorType.Replace].Any(t => t.Type == type))
            {
                var last = CorBattleStatus.Values[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            return GetBaseBattleStatus(type)
                + CorBattleStatus.Values[CorType.AddSub].Where(t => t.Type == type && t.Check(this))
                    .Select(t => t.Correct(this)).Sum();
        }

        /// <summary> 能力値を取得する </summary>
        /// <param name="ability">能力種類</param>
        /// <returns>能力値</returns>
        public abstract int GetAbility(AbilityType ability);

        /// <summary> 技能値の基礎数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>基礎数値</returns>
        protected virtual int GetBaseSkillValue(SkillValueType type)
        {
            if (CorSkillValue.Values[CorType.ChangeOriginal].Any(t => t.Type == type))
            {
                var lastCSV = CorSkillValue.Values[CorType.ChangeOriginal].Last(t => t.Type == type);
                if (lastCSV.Check(this))
                    return lastCSV.Correct(this);
            }
            switch (type)
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
                    throw new Exception("The argument is incorrect.");
            }
        }

        /// <summary> 技能値の最終数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>最終数値</returns>
        public virtual int GetSkillValue(SkillValueType type)
        {
            if (CorSkillValue.Values[CorType.Replace].Any(t => t.Type == type))
            {
                var last = CorSkillValue.Values[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            return GetBaseSkillValue(type)
                + CorSkillValue.Values[CorType.AddSub].Where(t => t.Type == type && t.Check(this))
                    .Select(t => t.Correct(this)).Sum();
        }

        /// <summary> 技能判定のロール数値を取得する関数 </summary>
        /// <param name="type">種類</param>
        /// <returns>最終数値</returns>
        public virtual DiceNumber GetJudgeValue(SkillValueType type)
        {
            if (CorJudgeValue.Values[CorType.Replace].Any(t => t.Type == type))
            {
                var last = CorJudgeValue.Values[CorType.Replace].Last(t => t.Type == type);
                if (last.Check(this))
                    return last.Correct(this);
            }
            return new DiceNumber(2, GetSkillValue(type))
                + CorJudgeValue.Values[CorType.AddSub].Where(t => t.Type == type && t.Check(this))
                    .Select(t => t.Correct(this))
                    .Aggregate((t0, t1) => t0 + t1);
        }
    }
}
