using EnumExtension;

namespace LHTRPG
{
    public enum Equipment
    {
        /// <summary> 頭部 </summary>
        [EnumText("頭部")] Head,
        /// <summary> 腕部 </summary>
        [EnumText("腕部")] Arm,
        /// <summary> 脚部 </summary>
        [EnumText("脚部")] Leg,
        /// <summary> 外套 </summary>
        [EnumText("外套")] Mantle,
    }

    /// <summary> 装備種別タグ </summary>
    public class TagEquipment : Tag<Equipment> { public TagEquipment(Equipment type) : base(type) { } }
}
