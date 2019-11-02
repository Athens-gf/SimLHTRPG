namespace LHTRPG
{
    /// <summary> 地形 </summary>
    public class Terrain : Prop
    {
        public Terrain(string name, Origin origin, bool isTrap) : base(name, LHTRPG.PropType.Terrain, origin, isTrap) { }
    }
}
