namespace LHTRPG
{
    /// <summary> 空間 </summary>
    public class Space : Prop
    {
        public Space(string name, Origin origin, bool isTrap) : base(name, LHTRPG.PropType.Space, origin, isTrap) { }
    }
}
