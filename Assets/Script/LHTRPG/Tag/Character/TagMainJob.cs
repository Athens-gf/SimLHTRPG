using System;
using EnumExtension;

namespace LHTRPG
{
    public static partial class LHTRPGBase
    {
        /// <summary> メイン職業からそれに対応したアーキ職業を取得する </summary>
        /// <param name="mainJob">メイン職業</param>
        public static ArchetypeJob GetArchetype(this MainJob mainJob)
        {
            switch (mainJob)
            {
                case MainJob.Guardian:
                case MainJob.Samurai:
                case MainJob.Monk:
                    return ArchetypeJob.Warrior;
                case MainJob.Cleric:
                case MainJob.Druid:
                case MainJob.Kannagi:
                    return ArchetypeJob.Warrior;
                case MainJob.Assassin:
                case MainJob.Swashbuckler:
                case MainJob.Bard:
                    return ArchetypeJob.Warrior;
                case MainJob.Sorcerer:
                case MainJob.Summoner:
                case MainJob.Enchanter:
                    return ArchetypeJob.Warrior;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    public enum ArchetypeJob
    {
        [EnumText("戦士職")] Warrior,
        [EnumText("回復職")] Recovery,
        [EnumText("武器攻撃職")] Weapon,
        [EnumText("魔法攻撃職")] Magic,
    }

    public enum MainJob
    {
        // 戦士職
        [EnumText("守護戦士")] Guardian,
        [EnumText("武士")] Samurai,
        [EnumText("武闘家")] Monk,
        // 回復職
        [EnumText("施療神官")] Cleric,
        [EnumText("森呪遣い")] Druid,
        [EnumText("神祇官")] Kannagi,
        // 武器攻撃職
        [EnumText("暗殺者")] Assassin,
        [EnumText("盗剣士")] Swashbuckler,
        [EnumText("吟遊詩人")] Bard,
        // 魔法攻撃職
        [EnumText("妖術師")] Sorcerer,
        [EnumText("召喚術師")] Summoner,
        [EnumText("付与術師")] Enchanter,
    }

    /// <summary> メイン職業タグ </summary>
    public class TagMainJob : Tag<MainJob> { public TagMainJob(MainJob type) : base(type) { } }
}
