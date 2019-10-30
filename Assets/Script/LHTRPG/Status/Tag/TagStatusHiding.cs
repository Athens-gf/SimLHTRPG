namespace LHTRPG
{
    /// <summary> 隠密／隠蔽 </summary>
    public class TagStatusHiding : TagStatus
    {
        /// <summary> 対象がキャラクターかどうか </summary>
        public bool IsCharacter { get; }

        public TagStatusHiding(bool isCharacter) : base(Status.Hiding) => IsCharacter = isCharacter;

        public override string Name => IsCharacter ? "隠密" : "隠蔽";
    }
}
