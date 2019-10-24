using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumExtension;

namespace LHTRPG
{
    public interface IAction
    {
        ActionType Type { get; }
    }

    public enum CorrectionType
    {
        BaseReplace,
    }

    public enum TagValueType
    {
        /// <summary> 無し：同時に取得するということがない </summary>
        None,
        /// <summary> 加算：数値を加算する </summary>
        Add,
        /// <summary> 最大：数値の大きい方を残す </summary>
        Max,
        /// <summary> 重複：数値毎に別々のタグとして存在する </summary>
        Overlap,
    }

    public enum ActionType
    {
        /// <summary> 動作 </summary>
        Action,
        /// <summary> 特技 </summary>
        Skill,
        /// <summary> アイテム </summary>
        Item,
        /// <summary> EXパワー </summary>
        EXPower,
        /// <summary> その他の行動 </summary>
        Other,
    }

    public enum DamageType
    {
        /// <summary> 物理ダメージ </summary>
        [EnumText("物理")] Physics,
        /// <summary> 魔法ダメージ </summary>
        [EnumText("魔法")] Magic,
        /// <summary> 貫通ダメージ </summary>
        [EnumText("貫通")] Through,
        /// <summary> 直接ダメージ </summary>
        [EnumText("直接")] Directly,
    }

    public enum UnitType
    {
        /// <summary> 冒険者 </summary>
        Adventurer,
        /// <summary> ゲスト(NPC、能力値やHPなどのデータを持つキャラクター) </summary>
        Guest,
        /// <summary> エキストラ(NPC、ルール城のデータを一切持たない演出上のキャラクター) </summary>
        Extra,
        /// <summary> エネミー </summary>
        Enemy,
        /// <summary> 行動 </summary>
        Action,
        /// <summary> 攻撃 </summary>
        Attack,
        /// <summary> アイテム </summary>
        Item,
        /// <summary> プロップ </summary>
        Prop,
    }

    public enum SkillValueType
    {
        /// <summary> 運動 </summary>
        [EnumText("運動")] Exercise,
        /// <summary> 耐久 </summary>
        [EnumText("耐久")] Endurance,
        /// <summary> 解除 </summary>
        [EnumText("解除")] Release,
        /// <summary> 操作 </summary>
        [EnumText("操作")] Operation,
        /// <summary> 知覚 </summary>
        [EnumText("知覚")] Perception,
        /// <summary> 交渉 </summary>
        [EnumText("交渉")] Negotiation,
        /// <summary> 知識 </summary>
        [EnumText("知識")] Knowledge,
        /// <summary> 解析 </summary>
        [EnumText("解析")] Analysis,
        /// <summary> 命中 </summary>
        [EnumText("命中")] Hit,
        /// <summary> 回避 </summary>
        [EnumText("回避")] Avoidance,
        /// <summary> 抵抗 </summary>
        [EnumText("抵抗")] Resistance,
    }

    public enum AbilityType
    {
        STR,
        DEX,
        POW,
        INT,
    }

    public enum ValueType
    {
        /// <summary> STR能力基本値 </summary>
        STRBase,
        /// <summary> STR能力値 </summary>
        STR,
        /// <summary> DEX能力基本値 </summary>
        DEXBase,
        /// <summary> DEX能力値 </summary>
        DEX,
        /// <summary> POW能力基本値 </summary>
        POWBase,
        /// <summary> POW能力値 </summary>
        POW,
        /// <summary> INT能力基本値 </summary>
        INTBase,
        /// <summary> INT能力値 </summary>
        INT,
        /// <summary> 最大HP </summary>
        MaxHP,
        /// <summary> 因果力 </summary>
        StartFate,
        /// <summary> 攻撃力 </summary>
        Attack,
        /// <summary> 魔力 </summary>
        Magic,
        /// <summary> 回復力 </summary>
        Recovary,
        /// <summary> 物理防御力 </summary>
        PhyDefense,
        /// <summary> 魔法防御力 </summary>
        MagDefense,
        /// <summary> 行動力 </summary>
        Behavior,
        /// <summary> 移動力 </summary>
        MovePoint,
    }

    public enum StatusCategory
    {
        Life,
        Bad,
        Combat,
        Other,
    }

    public enum Status
    {
        // ライフステータス
        /// <summary> 疲労 </summary>
        [EnumText("疲労")] Fatigue,
        /// <summary> 弱点 </summary>
        [EnumText("弱点")] WeakPoint,
        /// <summary> 戦闘不能 </summary>
        [EnumText("戦闘不能")] UnableFight,
        /// <summary> 死亡 </summary>
        [EnumText("死亡")] Death,
        // バッドステータス
        /// <summary> 萎縮 </summary>
        [EnumText("萎縮")] Atrophy,
        /// <summary> 放心 </summary>
        [EnumText("放心")] Emptiness,
        /// <summary> 硬直 </summary>
        [EnumText("硬直")] Stiffness,
        /// <summary> 惑乱 </summary>
        [EnumText("惑乱")] Scare,
        /// <summary> 衰弱 </summary>
        [EnumText("衰弱")] Weakness,
        /// <summary> 追撃 </summary>
        [EnumText("追撃")] Pursuit,
        /// <summary> 重篤 </summary>
        [EnumText("重篤")] Serious,
        /// <summary> 慢心 </summary>
        [EnumText("慢心")] Prosperity,
        // コンバットステータス
        /// <summary> 再生 </summary>
        [EnumText("再生")] Regeneration,
        /// <summary> 軽減 </summary>
        [EnumText("軽減")] Mitigation,
        /// <summary> 障壁 </summary>
        [EnumText("障壁")] Barrier,
        // アザーステータス
        /// <summary> 水泳 </summary>
        [EnumText("水泳")] Swimming,
        /// <summary> 飛行 </summary>
        [EnumText("飛行")] Flying,
        /// <summary> 二刀流 </summary>
        [EnumText("二刀流")] DoubleSword,
        /// <summary> 隠密 </summary>
        [EnumText("隠密")] Hiding,
        /// <summary> 識別済 </summary>
        [EnumText("識別済")] Identified,
        /// <summary> シーンに存在しない </summary>
        [EnumText("シーンに存在しない")] NotInScene,
        /// <summary> 行動 </summary>
        [EnumText("行動")] Behavior,
        /// <summary> ヘイト </summary>
        [EnumText("ヘイト")] Hate,
    }

    public enum ElementType
    {
        /// <summary> 火炎 </summary>
        [EnumText("火炎")] Flame,
        /// <summary> 冷気 </summary>
        [EnumText("冷気")] Cold,
        /// <summary> 電撃 </summary>
        [EnumText("電撃")] Lightning,
        /// <summary> 光輝 </summary>
        [EnumText("光輝")] Shine,
        /// <summary> 邪毒 </summary>
        [EnumText("邪毒")] EvilPoison,
        /// <summary> 精神 </summary>
        [EnumText("精神")] Mind,
    }

    public enum WeaponType
    {
        /// <summary> 剣 </summary>
        [EnumText("剣")] Sword,
        /// <summary> 刀 </summary>
        [EnumText("刀")] Katana,
        /// <summary> 槍 </summary>
        [EnumText("槍")] Spear,
        /// <summary> 槌斧 </summary>
        [EnumText("槌斧")] HammerAxis,
        /// <summary> 鞭 </summary>
        [EnumText("鞭")] Whip,
        /// <summary> 格闘 </summary>
        [EnumText("格闘")] Grappling,
        /// <summary> 杖 </summary>
        [EnumText("杖")] Cane,
        /// <summary> 弓 </summary>
        [EnumText("弓")] Bow,
        /// <summary> 投擲 </summary>
        [EnumText("投擲")] Throwing,
        /// <summary> 固定砲 </summary>
        [EnumText("固定砲")] Cannon,
        /// <summary> 魔石 </summary>
        [EnumText("魔石")] MagicStone,
    }

    public enum AttackType
    {
        /// <summary> 白兵攻撃 </summary>
        [EnumText("白兵攻撃")] Proximity,
        /// <summary> 射撃攻撃 </summary>
        [EnumText("射撃攻撃")] Shooting,
        /// <summary> 武器攻撃 </summary>
        [EnumText("武器攻撃")] Weapon,
        /// <summary> 魔法攻撃 </summary>
        [EnumText("魔法攻撃")] Magic,
        /// <summary> 特殊攻撃 </summary>
        [EnumText("特殊攻撃")] Special,
    }

    public enum ArmorType
    {
        /// <summary> 軽鎧 </summary>
        [EnumText("軽鎧")] Light,
        /// <summary> 中鎧 </summary>
        [EnumText("中鎧")] Medium,
        /// <summary> 重鎧 </summary>
        [EnumText("重鎧")] Heavy,
    }

    public enum EquipmentType
    {
        /// <summary> 頭部 </summary>
        [EnumText("頭部")] Head,
        /// <summary> 腕部 </summary>
        [EnumText("腕部")] Arm,
        /// <summary> 脚部 </summary>
        [EnumText("脚部")] Leg,
        /// <summary> 外套 </summary>
        [EnumText("外套")] Mantle,
    }

    public enum PropType
    {
        /// <summary> 地形 </summary>
        [EnumText("地形")] Terrain,
        /// <summary> 壁 </summary>
        [EnumText("壁")] Wall,
        /// <summary> 空間 </summary>
        [EnumText("空間")] Space,
        /// <summary> オブジェクト </summary>
        [EnumText("オブジェクト")] Object,
        /// <summary> シーンエフェクト </summary>
        [EnumText("シーンエフェクト")] SceneEffect,
    }

    public enum OriginType
    {
        /// <summary> 機械 </summary>
        [EnumText("機械")] Mashine,
        /// <summary> 天然 </summary>
        [EnumText("天然")] Natural,
        /// <summary> 魔法 </summary>
        [EnumText("魔法")] Magic,
        //			[EnumText("出自可変")] Variable,
    }

    public enum LargeRaceType
    {
        /// <summary> 人間 </summary>
        [EnumText("人間")] Human,
        /// <summary> 人型 </summary>
        [EnumText("人型")] Humanoid,
        /// <summary> 自然 </summary>
        [EnumText("自然")] Naure,
        /// <summary> 精霊 </summary>
        [EnumText("精霊")] Spirit,
        /// <summary> 幻獣 </summary>
        [EnumText("幻獣")] IllusionalBeast,
        /// <summary> 不死 </summary>
        [EnumText("不死")] Undead,
        /// <summary> 人造 </summary>
        [EnumText("人造")] Artificial,
        /// <summary> ギミック </summary>
        [EnumText("ギミック")] Gimmick,
    }

    public enum PersonType
    {
        /// <summary> 一般人 </summary>
        [EnumText("一般人")] Common,
        /// <summary> 職人 </summary>
        [EnumText("職人")] Craftsman,
        /// <summary> 商人 </summary>
        [EnumText("商人")] Merchant,
        /// <summary> 武人 </summary>
        [EnumText("武人")] Warrior,
        /// <summary> 為政者 </summary>
        [EnumText("為政者")] Politician,
        /// <summary> 知識人 </summary>
        [EnumText("知識人")] Intellectuals,
        /// <summary> 自由人 </summary>
        [EnumText("自由人")] Free,
        /// <summary> 芸術家 </summary>
        [EnumText("芸術家")] Artist,
    }

    public enum PhaseType
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

    public enum BehaviorType
    {
        /// <summary> 未行動 </summary>
        [EnumText("未行動")] NotYet,
        /// <summary> 行動済み </summary>
        [EnumText("行動済み")] Already,
        /// <summary> 待機 </summary>
        [EnumText("待機")] Waiting,
    }
}
