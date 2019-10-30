namespace LHTRPG
{
    /// <summary> ゲスト(NPC、能力値やHPなどのデータを持つキャラクター) </summary>
    public class Guest : Adventurer
    {
        /// <summary> 冒険者かどうか </summary>
        public override bool IsAdventurer { get; set; }

        public Guest() : base(UnitType.Guest) { }
    }
}
