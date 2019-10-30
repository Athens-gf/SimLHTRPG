using EnumExtension;

namespace LHTRPG
{
    public enum PropType
    {
        /// <summary> 地形 </summary>
        [EnumText("地形")] Terrain,
        /// <summary> 壁 </summary>
        [EnumText("壁")] Wall,
        /// <summary> 空間 </summary>
        [EnumText("空間")] Space,
        /// <summary> オブジェクト </summary>
        [EnumText("オブジェクト")] Object,
        /// <summary> シーンエフェクト </summary>
        [EnumText("シーンエフェクト")] SceneEffect,
    }

    /// <summary> プロップタグ </summary>
    public class TagProp : Tag<PropType> { public TagProp(PropType type) : base(type) { } }
}
