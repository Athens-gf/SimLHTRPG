using EnumExtension;

namespace LHTRPG
{
    public enum JudgementType
    {
        [EnumText("判定なし")] None,
        [EnumText("自動成功")] AlwaysSuccess,
        [EnumText("基本")] Basic,
        [EnumText("対決")] Versus,
        [EnumText("本文")] Text,
    }

    public enum BasicType
    {
        [EnumText("")] Number,
        [EnumText("識別難易度")] Identification,
        [EnumText("探知難易度")] Search,
        [EnumText("解析難易度")] Analysis,
        [EnumText("解除難易度")] Release,
        [EnumText("本文")] Text,
    }

    /// <summary> 判定 </summary>
    public class Judgement
    {
        /// <summary> 判定種別 </summary>
        public JudgementType Type { get; set; }

        /// <summary> 判定が「基本」の時の目標値種別 </summary>
        public BasicType BasicType { get; set; }

        /// <summary> 判定に使う技能値 </summary>
        public SkillValueType UseSkillActive { get; set; }

        /// <summary> 能動側の補正値 </summary>
        public DiceNumber BuffActive { get; set; } = 0;

        /// <summary> 判定が「対決」の時の受動側の技能値 </summary>
        public SkillValueType UseSkillPassive { get; set; } = 0;

        /// <summary> 受動側の補正値、あるいは判定が「基本」かつBasisTypeが「Number」の時の目標値 </summary>
        public DiceNumber BuffPassive { get; set; }

        public Judgement(JudgementType type) { Type = type; }

        public override string ToString()
        {
            string passiveText;
            switch (Type)
            {
                case JudgementType.Basic:
                    if (BasicType == BasicType.Number)
                        if (BuffPassive.IsZero)
                            passiveText = "";
                        else
                            passiveText = BuffPassive.ToString(false);
                    else
                        passiveText = $"／{BasicType.GetText()}{BuffPassive.ToString(true)}";
                    break;
                case JudgementType.Versus:
                    passiveText = $"／{UseSkillPassive.GetText()}{BuffPassive.ToString(true)}";
                    break;
                default:
                    return Type.GetText();
            }
            return $"{Type.GetText()}（{UseSkillActive.GetText()}{BuffActive.ToString(true)}{passiveText}）";
        }
    }
}
