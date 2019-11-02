namespace LHTRPG
{
    /// <summary> 壁 </summary>
    public class Wall : Prop
    {
        public Wall(string name, Origin origin, bool isTrap) : base(name, LHTRPG.PropType.Wall, origin, isTrap) { }
    }
}
