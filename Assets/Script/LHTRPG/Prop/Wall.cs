namespace LHTRPG
{
    /// <summary> 壁 </summary>
    public class Wall : Prop
    {
        /// <summary> 上部に壁があるかどうか </summary>
        public bool IsTop { get; set; } = false;

        /// <summary> 左部に壁があるかどうか </summary>
        public bool IsLeft { get; set; } = false;

        public Wall(string name, Origin origin, bool isTrap) : base(name, LHTRPG.PropType.Wall, origin, isTrap) { }
    }
}
