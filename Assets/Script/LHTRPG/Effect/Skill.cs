using System;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using AthensUtility;

namespace LHTRPG
{
    // 対象
    public class Target
    {
        public enum Type
        {
            [EnumText("自身")] Myself,
            [EnumText("個体")] Yourself,
            [EnumText("範囲")] Range,
            [EnumText("広範囲")] WideRange,
            [EnumText("直線")] Line,
            [EnumText("本文")] Text,
        }
        public Type Type { get; protected set; }
        public RankNumber Value { get; protected set; }
        public bool IsSelect { get; protected set; }

        public Target(Type type) : this(type, RankNumber.Base0) { }
        public Target(Type type, RankNumber value) : this(type, value, true) { }
        public Target(Type type, RankNumber value, bool isSelect) { Type = type; Value = value; IsSelect = isSelect; }

        public override string ToString()
        {
            switch (Type)
            {
                case Type.Yourself:
                    return (Value.IsZero ? "単体" : Value + "体");
                case Type.WideRange:
                case Type.Line:
                    return Type.GetText() + Value;
                default:
                    return Type.GetText();
            }
        }
    }

    // 射程
    public class Range
    {
        public enum Type
        {
            [EnumText("距離")] Distance,
            [EnumText("武器")] Weapon,
            [EnumText("本文")] Text,
        }
        public Type Type { get; protected set; }
        public RankNumber Value { get; protected set; }

        public Range(Type type) : this(type, RankNumber.Base0) { }
        public Range(Type type, RankNumber value) { Type = type; Value = value; }

        public override string ToString()
        {
            if (Type == Type.Distance) return (Value.IsZero ? "至近" : Value + "Ｓｑ");
            return Type.GetText();
        }
    }





    // 動作／特技
    public abstract class Skill : Unit
    {
        public string Name { get; protected set; }
        public int MaxRank { get; protected set; }
        public Timing Timing { get; protected set; }
        public Judgement Judgement { get; protected set; }
        public Target Target { get; protected set; }
        public Range Range { get; protected set; }
        public Cost Cost { get; protected set; }
        public Limitation Limitation { get; protected set; }
        public List<Effect> Effects { get; protected set; }

        public Skill(string name, int maxRank, Timing timing, Judgement judgement, Target target, Range range, Cost cost, Limitation limitation) : base(Type.Action)
        {
            Name = name;
            MaxRank = maxRank;
            Timing = timing;
            Judgement = judgement;
            Target = target;
            Range = range;
            Cost = cost;
            Limitation = limitation;
            Effects = new List<Effect>();
        }
    }
}
