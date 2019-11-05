using System;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using AthensUtility;

namespace LHTRPG
{
    // タイミング
    public class Timing
    {
        public enum Type
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
        public Type Type { get; protected set; }
        public enum Action
        {
            [EnumText("ムーブ")] Move,
            [EnumText("マイナー")] Minor,
            [EnumText("メジャー")] Major,
            [EnumText("インスタント")] Instant,
        }
        public Action Action { get; protected set; }
        public enum Process
        {
            [EnumText("セットアップ")] Setup,
            [EnumText("イニシアチブ")] Initiative,
            [EnumText("メインプロセス")] Main,
            [EnumText("クリンナップ")] Cleanup,
        }
        public Process Process { get; protected set; }
        public bool IsBefore { get; protected set; }

        public Timing(Type type) { Type = type; }

        public Timing(Action action) : this(Type.Action) { Action = action; }

        public Timing(Process process) : this(Type.Process) { Process = process; }

        public Timing(Type type, bool isBefore) : this(type) { IsBefore = isBefore; }

        public override string ToString()
        {
            switch (Type)
            {
                case Type.Action:
                    return Action.GetText();
                case Type.Process:
                    return Process.GetText();
                case Type.Judgment:
                case Type.ApplyDamage:
                    return Type.GetText() + (IsBefore ? "直前" : "直後");
                default:
                    return Type.GetText();
            }
        }
    }

    // 判定
    public class Judgement
    {
        public enum Type
        {
            [EnumText("判定なし")] None,
            [EnumText("自動成功")] AlwaysSuccess,
            [EnumText("基本")] Basic,
            [EnumText("対決")] Versus,
            [EnumText("本文")] Text,
        }
        public Type Type { get; protected set; }
        public enum BasicType
        {
            [EnumText("無し")] Nothing,
            [EnumText("識別難易度")] Identification,
            [EnumText("探知難易度")] Search,
            [EnumText("解析難易度")] Analysis,
            [EnumText("解除難易度")] Release,
            [EnumText("本文")] Text,
        }
        public BasicType BasicType { get; protected set; }
        public SkillValue UseSkillActive { get; protected set; }
        public SkillValue UseSkillPassive { get; protected set; }
        public RankNumber BuffActive { get; protected set; }
        public RankNumber BuffPassive { get; protected set; }

        public Judgement(Type type) { Type = type; }

        public Judgement(SkillValue useSkill, BasicType basicType) : this(useSkill, basicType, RankNumber.Base0) { }
        public Judgement(SkillValue useSkill, BasicType basicType, RankNumber buff) : this(Type.Basic)
        { UseSkillActive = useSkill; BasicType = basicType; BuffActive = buff; }

        public Judgement(SkillValue active, SkillValue passive) : this(active, passive, RankNumber.Base0) { }
        public Judgement(SkillValue active, SkillValue passive, RankNumber buffActive) : this(active, passive, buffActive, RankNumber.Base0) { }
        public Judgement(SkillValue active, SkillValue passive, RankNumber buffActive, RankNumber buffPassive) : this(Type.Versus)
        { UseSkillActive = active; UseSkillPassive = passive; BuffActive = buffActive; BuffPassive = buffPassive; }

        protected string GetBuffStr(RankNumber buff) { return (buff.ToString() == "0" ? "" : "＋" + buff); }

        public override string ToString()
        {
            switch (Type)
            {
                case Type.Basic:
                    return Type.GetText() + "（" + UseSkillActive.GetText() + GetBuffStr(BuffActive)
                        + (BasicType == BasicType.Nothing ? "" : "／" + BasicType.GetText()) + "）";
                case Type.Versus:
                    return Type.GetText() + "（" + UseSkillActive.GetText() + GetBuffStr(BuffActive) + "／" + UseSkillPassive.GetText() + GetBuffStr(BuffPassive) + "）";
                default:
                    return Type.GetText();
            }
        }
    }

    // 対象
    public class Target
    {
        public enum Type
        {
            [EnumText("自身")] Myself,
            [EnumText("個体")] Yourself,
            [EnumText("範囲")] Range,
            [EnumText("広範囲")] WideRange,
            [EnumText("直線")] Line,
            [EnumText("本文")] Text,
        }
        public Type Type { get; protected set; }
        public RankNumber Value { get; protected set; }
        public bool IsSelect { get; protected set; }

        public Target(Type type) : this(type, RankNumber.Base0) { }
        public Target(Type type, RankNumber value) : this(type, value, true) { }
        public Target(Type type, RankNumber value, bool isSelect) { Type = type; Value = value; IsSelect = isSelect; }

        public override string ToString()
        {
            switch (Type)
            {
                case Type.Yourself:
                    return (Value.IsZero ? "単体" : Value + "体");
                case Type.WideRange:
                case Type.Line:
                    return Type.GetText() + Value;
                default:
                    return Type.GetText();
            }
        }
    }

    // 射程
    public class Range
    {
        public enum Type
        {
            [EnumText("距離")] Distance,
            [EnumText("武器")] Weapon,
            [EnumText("本文")] Text,
        }
        public Type Type { get; protected set; }
        public RankNumber Value { get; protected set; }

        public Range(Type type) : this(type, RankNumber.Base0) { }
        public Range(Type type, RankNumber value) { Type = type; Value = value; }

        public override string ToString()
        {
            if (Type == Type.Distance) return (Value.IsZero ? "至近" : Value + "Ｓｑ");
            return Type.GetText();
        }
    }





    // 動作／特技
    public abstract class Skill : Unit
    {
        public string Name { get; protected set; }
        public int MaxRank { get; protected set; }
        public Timing Timing { get; protected set; }
        public Judgement Judgement { get; protected set; }
        public Target Target { get; protected set; }
        public Range Range { get; protected set; }
        public Cost Cost { get; protected set; }
        public Limitation Limitation { get; protected set; }
        public List<Effect> Effects { get; protected set; }

        public Skill(string name, int maxRank, Timing timing, Judgement judgement, Target target, Range range, Cost cost, Limitation limitation) : base(Type.Action)
        {
            Name = name;
            MaxRank = maxRank;
            Timing = timing;
            Judgement = judgement;
            Target = target;
            Range = range;
            Cost = cost;
            Limitation = limitation;
            Effects = new List<Effect>();
        }
    }
}
