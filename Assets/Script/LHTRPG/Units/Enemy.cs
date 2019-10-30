using System;
using EnumExtension;
using KM.DeaUtility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace LHTRPG
{
    public class Enemy : Character
    {
        protected override List<Tag> LTags
        {
            get
            {
                throw new NotImplementedException();
            }
            set { }
        }

        public Enemy() : base(Type.Enemy)
        {

        }

        protected override int GetBaseValue(Value _value)
        {
            throw new NotImplementedException();
        }

        public override int GetValue(Value _value)
        {
            throw new NotImplementedException();
        }

        public override int GetAbility(Ability _abi)
        {
            throw new NotImplementedException();
        }
    }


}
