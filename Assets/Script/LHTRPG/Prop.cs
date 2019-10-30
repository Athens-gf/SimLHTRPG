using System;
using EnumExtension;
using KM.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace LHTRPG
{
    public abstract class Prop : Unit
    {
        public virtual string Name { get; protected set; }
        public TagProp PropType { get; protected set; }
        public TagOrigin Origin { get; protected set; }
        public TagTrap Trap { get; protected set; }
        public bool IsTrap { get { return Trap != null; } }
        public virtual bool CanBreak { get { return false; } protected set { } }
        public virtual bool IsBroken { get; protected set; }

        public int DifficultySearch { get; protected set; }
        public bool AutoSearch { get { return DifficultySearch == 0; } }
        public int DifficultyAnalysis { get; protected set; }
        public bool AutoAnalysis { get { return DifficultyAnalysis == 0; } }
        public int DifficultyRelease { get; protected set; }
        public bool CanRelease { get { return DifficultyRelease != 0; } }
        protected override List<Tag> LTags { get { return PropType.MakeCollection<Tag>().Append(Origin).Append(Trap).Where(t => t != null).ToList(); } set { } }

        public Prop(string _name, TagProp.Type _propType, TagOrigin.Type _origin, bool _isTrap) : base(Type.Prop)
        {
            Name = _name;
            PropType = new TagProp(this, _propType);
            Origin = new TagOrigin(this, _origin);
            Trap = _isTrap ? new TagTrap(this) : null;
            IsBroken = false;
        }

        public override void Damage(int _damage, DamageType _type, Unit _unit)
        {
            if (_damage <= 0) return;
            HP -= _damage;
            if (HP <= 0)
            {
                HP = 0;
                IsBroken = true;
            }
        }

        public override void Heal(int _heal, Unit _unit) { }
    }

    public class Terrain : Prop
    {
        public Terrain(string _name, TagOrigin.Type _origin, bool _isTrap) : base(_name, TagProp.Type.Terrain, _origin, _isTrap)
        {

        }
    }

    public class Wall : Prop
    {
        public Wall(string _name, TagOrigin.Type _origin, bool _isTrap) : base(_name, TagProp.Type.Wall, _origin, _isTrap)
        {

        }
    }

    public class Space : Prop
    {
        public Space(string _name, TagOrigin.Type _origin, bool _isTrap) : base(_name, TagProp.Type.Space, _origin, _isTrap)
        {

        }
    }

    public class Object : Prop
    {
        public Object(string _name, TagOrigin.Type _origin, bool _isTrap) : base(_name, TagProp.Type.Object, _origin, _isTrap)
        {

        }
    }

    public class SceneEffect : Prop
    {
        public SceneEffect(string _name, TagOrigin.Type _origin, bool _isTrap) : base(_name, TagProp.Type.SceneEffect, _origin, _isTrap)
        {

        }
    }
}
