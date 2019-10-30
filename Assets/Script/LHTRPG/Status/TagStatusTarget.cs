using System.Linq;

namespace LHTRPG
{
    /// <summary> 数量を持ち、対象タグを持つステータスタグ </summary>
    public class TagStatusTarget : TagStatusValue
    {
        /// <summary> 弱点になるタグ </summary>
        public Tag Target { get; protected set; }

        public TagStatusTarget(TagStatusType type, Status status, Tag targetTag) : base(TagStatusType.Overlap, Status.Mitigation) => Target = targetTag;

        public override string ToString() { return Target == null ? base.ToString() : "[" + Name + "（" + Target.Name + "）：" + Value + "]"; }

        /// <returns>適用されるならTrue</returns>
        /// <summary> 軽減・弱点が適用されるかどうか </summary>
        /// <param name="target">このステータスを持つUnit</param>
        /// <param name="attacker">攻撃者Unit</param>
        public bool IsApplication(Unit target, Unit attacker)
        {
            if (Target == null)
                return true;
            if (Target.Name == LHTRPGBase.TagNameNearbyAttack || Target.Name == LHTRPGBase.TagNameNotNearbyAttack)
            {
                if (target.Session.CurrentScene is SceneBattle)
                {
                    var pos = (target.Session.CurrentScene as SceneBattle).Positions;
                    return Target.Name == LHTRPGBase.TagNameNearbyAttack
                        ? pos[target] == pos[attacker]
                        : pos[target] != pos[attacker];
                }
                return false;
            }
            return attacker.Tags.Contains(Target);
        }
    }
}
