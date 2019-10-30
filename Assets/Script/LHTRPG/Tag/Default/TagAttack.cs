using EnumExtension;

namespace LHTRPG
{
    public enum Attack
    {
        /// <summary> 白兵攻撃 </summary>
        [EnumText("白兵攻撃")] Proximity,
        /// <summary> 射撃攻撃 </summary>
        [EnumText("射撃攻撃")] Shooting,
        /// <summary> 武器攻撃 </summary>
        [EnumText("武器攻撃")] Weapon,
        /// <summary> 魔法攻撃 </summary>
        [EnumText("魔法攻撃")] Magic,
        /// <summary> 特殊攻撃 </summary>
        [EnumText("特殊攻撃")] Special,
    }

    /// <summary> 攻撃種別タグ </summary>
    public class TagAttack : Tag<Attack> { public TagAttack(Attack type) : base(type) { } }
}
