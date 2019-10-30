namespace LHTRPG
{
    /// <summary> 識別済み／解析済み </summary>
    public class TagStatusIdentified : TagStatus
    {
        /// <summary> 対象がキャラクターかどうか </summary>
        public bool IsCharacter { get; }

        public TagStatusIdentified(bool isCharacter) : base(Status.Identified) => IsCharacter = isCharacter;

        public override string Name => IsCharacter ? "識別済み" : "解析済み";
    }
}
