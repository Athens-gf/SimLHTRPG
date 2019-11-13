using System;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using AthensUtility;

namespace LHTRPG
{
    // 動作／特技
    public abstract class Skill : Unit
    {
        public string Name { get; set; }

        public int MaxRank { get; set; }

        public Timing Timing { get; set; }

        public Judgement Judgement { get; set; }

        public Target Target { get; set; }

        public Range Range { get; set; }

        public Cost Cost { get; set; }

        public Limitation Limitation { get; set; }

        public List<Effect> Effects { get; set; }

        protected Skill(string name, int maxRank, Timing timing, Judgement judgement, Target target, Range range, Cost cost, Limitation limitation) : base(UnitType.Action)
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
