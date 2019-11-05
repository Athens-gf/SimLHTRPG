using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LHTRPG.Skills
{
    // 基本動作
    public abstract class SkillBasic : Skill
    {
        public SkillBasic(string name, int maxRank, Timing timing, Judgement judgement, Target target, Range range, Cost cost, Limitation limitation)
            : base(name, maxRank, timing, judgement, target, range, cost, limitation) { }
    }


}
