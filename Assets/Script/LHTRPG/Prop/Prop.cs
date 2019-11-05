using AthensUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LHTRPG
{
    /// <summary> プロップ </summary>
    public abstract class Prop : Unit
    {
        /// <summary> 名前 </summary>
        public virtual string Name { get; set; }

        /// <summary> プロップ種別 </summary>
        public TagProp PropType { get; set; }

        /// <summary> 出自タグ </summary>
        public TagOrigin Origin { get; set; }

        /// <summary> トラップタグ </summary>
        public Tag Trap { get; set; }

        /// <summary> トラップかどうか </summary>
        public bool IsTrap => Trap != null;

        /// <summary> 破壊できるかどうか </summary>
        public virtual bool CanBreak { get => false; set { } }

        /// <summary> 破壊されたかどうか </summary>
        public virtual bool IsBroken { get; set; } = false;

        /// <summary> 探知難易度 </summary>
        public int DifficultySearch { get; set; } = 0;

        /// <summary> 探知難易度が自動かどうか </summary>
        public bool AutoSearch => DifficultySearch == 0;

        /// <summary> 解析難易度 </summary>
        public int DifficultyAnalysis { get; set; } = 0;

        /// <summary> 解析難易度が自動かどうか </summary>
        public bool AutoAnalysis => DifficultyAnalysis == 0;

        /// <summary> 解除難易度 </summary>
        public int DifficultyRelease { get; set; } = 0;

        /// <summary> 解除難易度が自動かどうか </summary>
        public bool CanRelease => DifficultyRelease != 0;

        /// <summary> 最大HP </summary>
        public int MaxHP { get; set; } = 0;

        /// <summary> 防御力 </summary>
        public int Defense { get; set; } = 0;

        protected override IEnumerable<Tag> LTags => PropType.MakeCollection<Tag>()
            .Append(Origin)
            .Append(Trap)
            .Where(t => t != null);

        protected Prop(string name, PropType propType, Origin origin, bool isTrap) : base(UnitType.Prop)
        {
            Name = name;
            PropType = new TagProp(propType);
            Origin = new TagOrigin(origin);
            Trap = isTrap ? Tag.Trap : null;
            IsBroken = false;
        }

        /// <summary> ダメージを受ける処理 </summary>
        /// <param name="evplayer">イベント処理機、何らかのイベント発行する場合使用</param>
        /// <param name="damage">与ダメージ</param>
        /// <param name="type">ダメージ種別</param>
        /// <param name="fromUnit">攻撃Unit</param>
        /// <returns>[実際のダメージ,[障壁]減少分]</returns>
        public override Tuple<int, int> Damage(EventPlayer evplayer, int damage, DamageType type, Unit fromUnit)
        {
            if (!CanBreak || IsBroken)
                return null;
            damage -= Defense;
            HP = Math.Max(0, HP - damage);
            if (HP == 0)
                IsBroken = true;
            return new Tuple<int, int>(damage, 0);
        }

        /// <summary> 回復する処理 </summary>
        public override int Heal(EventPlayer evplayer, int heal, Unit fromUnit)
        {
            if (!CanBreak)
                return 0;
            HP = Math.Min(MaxHP, HP + heal);
            return heal;
        }
    }
}
