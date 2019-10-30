using EnumExtension;

namespace LHTRPG
{
    public enum Weapon
    {
        /// <summary> 剣 </summary>
        [EnumText("剣")] Sword,
        /// <summary> 刀 </summary>
        [EnumText("刀")] Katana,
        /// <summary> 槍 </summary>
        [EnumText("槍")] Spear,
        /// <summary> 槌斧 </summary>
        [EnumText("槌斧")] HammerAxis,
        /// <summary> 鞭 </summary>
        [EnumText("鞭")] Whip,
        /// <summary> 格闘 </summary>
        [EnumText("格闘")] Grappling,
        /// <summary> 杖 </summary>
        [EnumText("杖")] Cane,
        /// <summary> 弓 </summary>
        [EnumText("弓")] Bow,
        /// <summary> 投擲 </summary>
        [EnumText("投擲")] Throwing,
        /// <summary> 固定砲 </summary>
        [EnumText("固定砲")] Cannon,
        /// <summary> 魔石 </summary>
        [EnumText("魔石")] MagicStone,
    }

    /// <summary> 武器種別タグ </summary>
    public class TagWeapon : Tag<Weapon> { public TagWeapon(Weapon type) : base(type) { } }
}
