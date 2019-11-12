using System;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using AthensUtility;

namespace LHTRPG
{
    public enum TimingType
    {
        [EnumText("常時")] Always,
        [EnumText("プリプレイ")] PrePlay,
        [EnumText("インタールード")] Interlude,
        [EnumText("ブリーフィング")] Briefing,
        [EnumText("レストタイム")] RestTime,
        [EnumText("アクション")] Action,
        [EnumText("プロセス")] Process,
        [EnumText("判定")] Judgment,
        [EnumText("ダメージロール")] DamageRoll,
        [EnumText("ダメージ適用")] ApplyDamage,
        [EnumText("行動")] Behavior,
        [EnumText("本文")] Text,
        [EnumText("その他")] Other,
    }

    public enum TimingAction
    {
        [EnumText("ムーブ")] Move,
        [EnumText("マイナー")] Minor,
        [EnumText("メジャー")] Major,
        [EnumText("インスタント")] Instant,
    }

    public enum TimingProcess
    {
        [EnumText("セットアップ")] Setup,
        [EnumText("イニシアチブ")] Initiative,
        [EnumText("メインプロセス")] Main,
        [EnumText("クリンナップ")] Cleanup,
    }


    /// <summary> 発動タイミング </summary>
    public class Timing
    {
        public TimingType Type { get; set; }

        public TimingAction? Action { get; set; } = null;

        public TimingProcess? Process { get; set; } = null;

        public bool IsBefore { get; set; } = false;

        public Timing(TimingType type) { Type = type; }

        public Timing(TimingAction action) : this(TimingType.Action) { Action = action; }

        public Timing(TimingProcess process) : this(TimingType.Process) { Process = process; }

        public Timing(TimingType type, bool isBefore) : this(type) { IsBefore = isBefore; }

        public override string ToString()
        {
            switch (Type)
            {
                case TimingType.Action:
                    return Action.Value.GetText();
                case TimingType.Process:
                    return Process.Value.GetText();
                case TimingType.Judgment:
                case TimingType.ApplyDamage:
                    return Type.GetText() + (IsBefore ? "直前" : "直後");
                default:
                    return Type.GetText();
            }
        }
    }
}
