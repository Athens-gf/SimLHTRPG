namespace LHTRPG
{
    /// <summary> ヘイトトップ／ヘイトアンダー </summary>
    public class TagStatusHate : TagStatus
    {
        public bool IsHateTop { get; set; } = false;

        public TagStatusHate() : base(Status.Hate) { }

        public override string Name => IsHateTop ? "ヘイトトップ" : "ヘイトアンダー";
    }
}
