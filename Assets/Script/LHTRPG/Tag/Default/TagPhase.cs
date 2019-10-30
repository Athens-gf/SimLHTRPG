using EnumExtension;

namespace LHTRPG
{
    public enum Phase
    {
        /// <summary> オープニング </summary>
        [EnumText("オープニング")] Opening,
        /// <summary> ミドル </summary>
        [EnumText("ミドル")] Middle,
        /// <summary> クライマックス </summary>
        [EnumText("クライマックス")] Climax,
        /// <summary> エンディング </summary>
        [EnumText("エンディング")] Ending,
    }

    /// <summary> フェイズタグ </summary>
    public class TagPhase : Tag<Phase> { public TagPhase(Phase type) : base(type) { } }
}
