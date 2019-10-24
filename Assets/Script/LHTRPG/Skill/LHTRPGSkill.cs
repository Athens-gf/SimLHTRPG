using System;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using KM.Utility;

namespace LHTRPG
{
    public struct RankNumber
    {
        public static RankNumber Base0 { get { return new RankNumber(0, false); } }
        public int Value { get; private set; }
        public bool IsRank { get; private set; }
        public bool IsZero { get { return !IsRank && Value == 0; } }
        public bool IsOne { get { return !IsRank && Value == 1; } }
        public RankNumber(int _value, bool _isRank) { Value = _value; IsRank = _isRank; }

        public override string ToString() { return (IsRank ? "［ＳＲ" + Value + "］" : Value.ToString()); }
        public static implicit operator RankNumber(int _value) { return new RankNumber(_value, false); }
        public static implicit operator RankNumber(string _str)
        {
            if (_str.StartsWith("［") && _str.EndsWith("］")) _str = _str.Remove("［").Remove("］");
            var isRank = _str.Contains("ＳＲ");
            if (isRank) _str = _str.Remove("ＳＲ");
            int value;
            if (!int.TryParse(_str, out value)) throw new SystemException();
            return new RankNumber(value, isRank);
        }
        public int GetValue(Unit _unit) { return (IsRank ? _unit.Rank : 0) + Value; }
    }

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

        public Timing(Type _type) { Type = _type; }

        public Timing(Action _action) : this(Type.Action) { Action = _action; }

        public Timing(Process _process) : this(Type.Process) { Process = _process; }

        public Timing(Type _type, bool _isBefore) : this(_type) { IsBefore = _isBefore; }

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

        public Judgement(Type _type) { Type = _type; }

        public Judgement(SkillValue _useSkill, BasicType _basicType) : this(_useSkill, _basicType, RankNumber.Base0) { }
        public Judgement(SkillValue _useSkill, BasicType _basicType, RankNumber _buff) : this(Type.Basic)
        { UseSkillActive = _useSkill; BasicType = _basicType; BuffActive = _buff; }

        public Judgement(SkillValue _active, SkillValue _passive) : this(_active, _passive, RankNumber.Base0) { }
        public Judgement(SkillValue _active, SkillValue _passive, RankNumber _buffActive) : this(_active, _passive, _buffActive, RankNumber.Base0) { }
        public Judgement(SkillValue _active, SkillValue _passive, RankNumber _buffActive, RankNumber _buffPassive) : this(Type.Versus)
        { UseSkillActive = _active; UseSkillPassive = _passive; BuffActive = _buffActive; BuffPassive = _buffPassive; }

        protected string GetBuffStr(RankNumber _buff) { return (_buff.ToString() == "0" ? "" : "＋" + _buff); }

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

        public Target(Type _type) : this(_type, RankNumber.Base0) { }
        public Target(Type _type, RankNumber _value) : this(_type, _value, true) { }
        public Target(Type _type, RankNumber _value, bool _isSelect) { Type = _type; Value = _value; IsSelect = _isSelect; }

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

        public Range(Type _type) : this(_type, RankNumber.Base0) { }
        public Range(Type _type, RankNumber _value) { Type = _type; Value = _value; }

        public override string ToString()
        {
            if (Type == Type.Distance) return (Value.IsZero ? "至近" : Value + "Ｓｑ");
            return Type.GetText();
        }
    }

    // コスト
    public class Cost
    {
        public enum Type
        {
            [EnumText("－")] None,
            [EnumText("ヘイト")] Hate,
            [EnumText("因果力")] Fate,
            [EnumText("パーティ")] Party,
            [EnumText("仲間")] Buddy,
            [EnumText("本文")] Text,
        }
        public Type Type { get; protected set; }
        public int Value { get; protected set; }
        public Cost SubCost { get; protected set; }

        public Cost(Type _type, int _value = 0, Cost _subCost = null) { Type = _type; Value = _value; SubCost = _subCost; }

        public override string ToString()
        {
            switch (Type)
            {
                case Type.Hate:
                case Type.Fate:
                    return Type.GetText() + Value;
                case Type.Party:
                case Type.Buddy:
                    return SubCost + "（" + Type.GetText() + "）";
                default:
                    return Type.GetText();
            }
        }
    }

    // 制限
    public class Limitation
    {
        public enum Type
        {
            [EnumText("－")] None,
            [EnumText("シナリオ")] Scenario,
            [EnumText("シーン")] Scene,
            [EnumText("ラウンド")] Round,
            [EnumText("［パーティ］")] Party,
            [EnumText("本文")] Text,
        }
        public Type Type { get; private set; }
        public RankNumber Value { get; protected set; }

        public Limitation(Type _type) : this(_type, RankNumber.Base0) { }
        public Limitation(Type _type, RankNumber _value) { Type = _type; Value = _value; }

        public override string ToString()
        {
            switch (Type)
            {
                case Type.Scenario:
                case Type.Scene:
                case Type.Round:
                    return Type.GetText() + Value + "回";
                default:
                    return Type.GetText();
            }
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

        public Skill(string _name, int _maxRank, Timing _timing, Judgement _judgement, Target _target, Range _range, Cost _cost, Limitation _limitation) : base(Type.Action)
        {
            Name = _name;
            MaxRank = _maxRank;
            Timing = _timing;
            Judgement = _judgement;
            Target = _target;
            Range = _range;
            Cost = _cost;
            Limitation = _limitation;
            Effects = new List<Effect>();
        }
    }
}
