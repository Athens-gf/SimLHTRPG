namespace LHTRPG
{
    /// <summary> オブジェクト </summary>
    public class Object : Prop
    {
        public Object(string name, Origin origin, bool isTrap) : base(name, LHTRPG.PropType.Object, origin, isTrap) { }
    }
}
