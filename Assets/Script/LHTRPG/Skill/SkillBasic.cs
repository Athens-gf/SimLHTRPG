using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LHTRPG.Skills
{
	// 基本動作
	public abstract class SkillBasic : Skill
	{
		public SkillBasic(string _name, int _maxRank, Timing _timing, Judgement _judgement, Target _target, Range _range, Cost _cost, Limitation _limitation)
			: base(_name, _maxRank, _timing, _judgement, _target, _range, _cost, _limitation) { }
	}


}
