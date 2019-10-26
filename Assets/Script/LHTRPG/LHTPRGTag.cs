using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using AthensUtility;

namespace LHTRPG
{
    public static partial class LHTRPGBase
    {
        /// <summary> 特定の種類のタグの個数を取得 </summary>
        public static int Count<T>(this IEnumerable<Tag> tags) where T : Tag => tags.Count(t => t.GetType() == typeof(T));

        /// <summary> 特定の種類のタグが存在しているか </summary>
        public static bool IsExist<T>(this IEnumerable<Tag> tags) where T : Tag => tags.Any(t => t.GetType() == typeof(T));

        /// <summary> 特定の種類のタグだけ抽出する </summary>
        public static List<T> GetTags<T>(this IEnumerable<Tag> tags) where T : Tag => tags.Select(t => t as T).Where(t => t != null).ToList();

        /// <summary> 攻撃のタグかどうか </summary>
        public static bool IsAttack(this Unit unit, Attack type)
        {
            if (!unit.Tags.IsExist<TagAttack>()) return false;
            // 武器攻撃は白兵攻撃と射撃攻撃を合わせた概念
            return unit.Tags.GetTags<TagAttack>().Any(t => t.Type == type)
                || (type == Attack.Weapon && (IsAttack(unit, Attack.Proximity) || IsAttack(unit, Attack.Shooting)));
        }
    }

    [DebuggerDisplay("{ToString()}")]
    /// <summary> タグ基本クラス </summary>
    public class Tag
    {
        /// <summary> 名称 </summary>
        public virtual string Name { get; }

        /// <summary> 重複存在可能かどうか </summary>
        public virtual bool IsCanOverlap => false;

        public override string ToString() { return "[" + Name + "]"; }

        /// <summary> Unitへの参照 </summary>
        public Unit Unit { get; set; } = null;

        public Tag(string name) { Name = name; }

        public override bool Equals(object obj)
        {
            if (obj is Tag) return Name == (obj as Tag).Name;
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 77306245;
            hashCode = hashCode * -1521134295 + EqualityComparer<Unit>.Default.GetHashCode(Unit);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + IsCanOverlap.GetHashCode();
            return hashCode;
        }

        /// <summary> 移動タグ </summary>
        public static Tag Movement => new Tag("移動");
        /// <summary> スタイルタグ </summary>
        public static Tag Style => new Tag("スタイル");
        /// <summary> 二刀流タグ </summary>
        public static Tag DoubleSword => new Tag("二刀流");
        /// <summary> 構えタグ </summary>
        public static Tag Stance => new Tag("構え");
        /// <summary> 訓練タグ </summary>
        public static Tag Training => new Tag("訓練");
        /// <summary> 援護歌タグ </summary>
        public static Tag SupportingSong => new Tag("援護歌");
        /// <summary> 従者召喚タグ </summary>
        public static Tag SummonServant => new Tag("従者召喚");
        /// <summary> 支援タグ </summary>
        public static Tag Support => new Tag("支援");
        /// <summary> 速攻タグ </summary>
        public static Tag Haste => new Tag("速攻");
        /// <summary> 準備タグ </summary>
        public static Tag Preparation => new Tag("準備");
        /// <summary> 偵察タグ </summary>
        public static Tag Scout => new Tag("偵察");
        /// <summary> 消耗品タグ </summary>
        public static Tag Expendables => new Tag("消耗品");
        /// <summary> 破損タグ </summary>
        public static Tag Corruption => new Tag("破損");
        /// <summary> 非売品タグ </summary>
        public static Tag NotForSale => new Tag("非売品");
        /// <summary> 取引不可タグ </summary>
        public static Tag NoTrading => new Tag("取引不可");
        /// <summary> 換金タグ </summary>
        public static Tag Exchange => new Tag("換金");
        /// <summary> 食料タグ </summary>
        public static Tag Food => new Tag("食料");
        /// <summary> 水薬タグ </summary>
        public static Tag Medicine => new Tag("水薬");
        /// <summary> 呪薬タグ </summary>
        public static Tag Poison => new Tag("呪薬");
        /// <summary> 霊符タグ </summary>
        public static Tag Talisman => new Tag("霊符");
        /// <summary> 巻物タグ </summary>
        public static Tag Scroll => new Tag("巻物");
        /// <summary> 宝珠タグ </summary>
        public static Tag Gem => new Tag("宝珠");
        /// <summary> サービスタグ </summary>
        public static Tag Service => new Tag("サービス");
        /// <summary> 片手タグ </summary>
        public static Tag OneHand => new Tag("片手");
        /// <summary> 両手タグ </summary>
        public static Tag TwoHand => new Tag("両手");
        /// <summary> 楽器タグ </summary>
        public static Tag TagInstrument => new Tag("楽器");
        /// <summary> 盾タグ </summary>
        public static Tag TagShield => new Tag("盾");
        /// <summary> 補助装備タグ </summary>
        public static Tag TagAssistantEquipment => new Tag("補助装備");
        /// <summary> 鞄タグ </summary>
        public static Tag TagBag => new Tag("鞄");
        /// <summary> コア素材タグ </summary>
        public static Tag TagCoreMaterial => new Tag("コア素材");
        /// <summary> トラップタグ </summary>
        public static Tag TagTrap => new Tag("トラップ");
        /// <summary> 物品タグ </summary>
        public static Tag TagGoods => new Tag("物品");
        /// <summary> ボスタグ </summary>
        public static Tag TagBoss => new Tag("ボス");
        /// <summary> モブタグ </summary>
        public static Tag TagMob => new Tag("モブ");
        /// <summary> 暗視タグ </summary>
        public static Tag TagNightVision => new Tag("暗視");
        /// <summary> 水棲タグ </summary>
        public static Tag TagAquatic => new Tag("水棲");
        /// <summary> パーティタグ </summary>
        public static Tag TagParty => new Tag("パーティ");
    }

    [DebuggerDisplay("{ToString()}")]
    /// <summary> 数量を持ったタグ基本クラス </summary>
    public class TagValue : Tag
    {
        /// <summary> 同じタグが追加されたときの動作 </summary>
        public TagStatusType Type { get; }

        /// <summary> 数値 </summary>
        public virtual int Value { get; protected set; }

        /// <summary> 重複存在可能かどうか </summary>
        public override bool IsCanOverlap => Type == TagStatusType.Overlap;

        public override string ToString() { return "[" + Name + "：" + Value + "]"; }

        public TagValue(string name, TagStatusType type, int value) : base(name) { Type = type; Value = value; }
    }

    /// <summary> 種類タグ </summary>
    public class Tag<T> : Tag where T : System.Enum
    {
        public T Type { get; }
        public Tag(T type) : base(type.GetText()) { Type = type; }
    }

    public enum Element
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

    /// <summary> 属性タグ </summary>
    public class TagElement : Tag<Element> { public TagElement(Element type) : base(type) { } }

    public enum Weapon
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

    /// <summary> 武器種別タグ </summary>
    public class TagWeapon : Tag<Weapon> { public TagWeapon(Weapon type) : base(type) { } }

    public enum Attack
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

    /// <summary> 攻撃種別タグ </summary>
    public class TagAttack : Tag<Attack> { public TagAttack(Attack type) : base(type) { } }

    public enum Armor
    {
        /// <summary> 軽鎧 </summary>
        [EnumText("軽鎧")] Light,
        /// <summary> 中鎧 </summary>
        [EnumText("中鎧")] Medium,
        /// <summary> 重鎧 </summary>
        [EnumText("重鎧")] Heavy,
    }

    /// <summary> 鎧種別タグ </summary>
    public class TagArmor : Tag<Armor> { public TagArmor(Armor type) : base(type) { } }

    public enum Equipment
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

    /// <summary> 装備種別タグ </summary>
    public class TagEquipment : Tag<Equipment> { public TagEquipment(Equipment type) : base(type) { } }

    /// <summary> マジックアイテムグレードタグ </summary>
    public class TagMagicGrade : TagValue
    {
        public TagMagicGrade(int value) : base("M", TagStatusType.None, value) { }
        public override string ToString() { return "[" + Name + Value + "]"; }
    }

    /// <summary> 魔触媒タグ </summary>
    public class TagMagicCatalyst : TagValue
    {
        public TagMagicCatalyst(int value) : base("魔触媒", TagStatusType.None, value) { }
        public override string ToString() { return "[" + Name + " " + Value + "]"; }
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

    /// <summary> プロップタグ </summary>
    public class TagProp : Tag<PropType> { public TagProp(PropType type) : base(type) { } }

    public enum Origin
    {
        /// <summary> 機械 </summary>
        [EnumText("機械")] Mashine,
        /// <summary> 天然 </summary>
        [EnumText("天然")] Natural,
        /// <summary> 魔法 </summary>
        [EnumText("魔法")] Magic,
        //			[EnumText("出自可変")] Variable,
    }

    /// <summary> 出自タグ </summary>
    public class TagOrigin : Tag<Origin> { public TagOrigin(Origin type) : base(type) { } }

    public enum LargeRace
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

    /// <summary> 大種族タグ </summary>
    public class TagLargeRace : Tag<LargeRace> { public TagLargeRace(LargeRace type) : base(type) { } }

    /// <summary> エネミー種別タグ </summary>
    public class TagEnemyRace : Tag { public TagEnemyRace(string name) : base(name) { } }

    public enum Person
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

    /// <summary> 人物タグ </summary>
    public class TagPerson : Tag<Person> { public TagPerson(Person type) : base(type) { } }

    public enum Phase
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

    /// <summary> フェイズタグ </summary>
    public class TagPhase : Tag<Phase> { public TagPhase(Phase type) : base(type) { } }
}
