namespace LHTRPG
{
    /// <summary> シーンエフェクト </summary>
    public class SceneEffect : Prop
    {
        public SceneEffect(string name, Origin origin, bool isTrap) : base(name, LHTRPG.PropType.SceneEffect, origin, isTrap) { }
    }
}
