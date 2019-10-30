using System.Collections.Generic;
using System.Linq;
using AthensUtility;

namespace LHTRPG
{
    /// <summary> エキストラ(NPC、ルール城のデータを一切持たない演出上のキャラクター) </summary>
    public class Extra : Unit, IHumanCharacter
    {
        public Extra() : base(UnitType.Extra) { }

        /// <summary> 冒険者かどうか </summary>
        public bool IsAdventurer { get; set; }

        /// <summary> 名前 </summary>
        public string Name { get; set; }

        /// <summary> 性別 </summary>
        public TagSex Sex { get; set; }

        public int Level { get; set; }

        /// <summary> ガイディングクリード </summary>
        public GuidingCreed GuidingCreed { get; set; }

        /// <summary> 種族 </summary>
        public TagRace Race { get; set; }

        /// <summary> サブ職業 </summary>
        public TagSubJob SubJob { get; set; }

        /// <summary> その他のタグ </summary>
        public List<Tag> OtherTags { get; } = new List<Tag>();

        protected override IEnumerable<Tag> LTags
        {
            get => new Tag(IsAdventurer ? "冒険者" : "大地人").MakeCollection()
                    .Append(Sex)
                    .Append(Race)
                    .Append(SubJob)
                    .Append(GuidingCreed.Tag)
                    .Concat(OtherTags)
                    .Where(t => t != null);
        }

        /// <summary> ダメージを受ける処理 </summary>
        public override int Damage(EventPlayer evplayer, int damage, DamageType type, Unit fromUnit) => 0;

        /// <summary> 回復する処理 </summary>
        public override int Heal(EventPlayer evplayer, int heal, Unit fromUnit) => 0;
    }
}
