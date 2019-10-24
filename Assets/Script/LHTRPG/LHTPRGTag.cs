using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using EnumExtension;
using KM.Utility;

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
        public static bool IsAttackType(this Unit unit, AttackType type)
        {
            if (!unit.Tags.IsExist<TagAttack>()) return false;
            // 武器攻撃は白兵攻撃と射撃攻撃を合わせた概念
            return unit.Tags.GetTags<TagAttack>().Any(t => t.Type == type)
                || (type == AttackType.Weapon && (IsAttackType(unit, AttackType.Proximity) || IsAttackType(unit, AttackType.Shooting)));
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
    }

    [DebuggerDisplay("{ToString()}")]
    /// <summary> 数量を持ったタグ基本クラス </summary>
    public class TagValue : Tag
    {
        /// <summary> 同じタグが追加されたときの動作 </summary>
        public TagValueType Type { get; }

        /// <summary> 数値 </summary>
        public virtual int Value { get; protected set; }

        /// <summary> 重複存在可能かどうか </summary>
        public override bool IsCanOverlap => Type == TagValueType.Overlap;

        public override string ToString() { return "[" + Name + "：" + Value + "]"; }

        public TagValue(string name, TagValueType type, int value) : base(name) { Type = type; Value = value; }
    }

    /// <summary> 属性タグ </summary>
    public class TagElement : Tag
    {
        /// <summary> 属性種別 </summary>
        public ElementType Type { get; }
        public TagElement(ElementType type) : base(type.GetText()) { Type = type; }
    }

    /// <summary> 武器種別タグ </summary>
    public class TagWeapon : Tag
    {
        /// <summary> 武器種別 </summary>
        public WeaponType Type { get; }
        public TagWeapon(WeaponType type) : base(type.GetText()) { Type = type; }
    }

    /// <summary> 攻撃種別タグ </summary>
    public class TagAttack : Tag
    {
        /// <summary> 攻撃種別 </summary>
        public AttackType Type { get; }
        public TagAttack(AttackType type) : base(type.GetText()) { Type = type; }
    }

    public class TagMovement : Tag { public TagMovement() : base("移動") { } }

    public class TagStyle : Tag { public TagStyle() : base("スタイル") { } }

    public class TagDoubleSword : Tag { public TagDoubleSword() : base("二刀流") { } }

    public class TagStance : Tag { public TagStance() : base("構え") { } }

    public class TagTraining : Tag { public TagTraining() : base("訓練") { } }

    public class TagSupportingSong : Tag { public TagSupportingSong() : base("援護歌") { } }

    public class TagSummonServant : Tag { public TagSummonServant() : base("従者召喚") { } }

    public class TagSupport : Tag { public TagSupport() : base("支援") { } }

    public class TagHaste : Tag { public TagHaste() : base("速攻") { } }

    public class TagPreparation : Tag { public TagPreparation() : base("準備") { } }

    public class TagScout : Tag { public TagScout() : base("偵察") { } }

    public class TagExpendables : Tag { public TagExpendables() : base("消耗品") { } }

    public class TagCorruption : Tag { public TagCorruption() : base("破損") { } }

    public class TagNotForSale : Tag { public TagNotForSale() : base("非売品") { } }

    public class TagNoTrading : Tag { public TagNoTrading() : base("取引不可") { } }

    public class TagExchange : Tag { public TagExchange() : base("換金") { } }

    public class TagFood : Tag { public TagFood() : base("食料") { } }

    public class TagMedicine : Tag { public TagMedicine() : base("水薬") { } }

    public class TagPoison : Tag { public TagPoison() : base("呪薬") { } }

    public class TagTalisman : Tag { public TagTalisman() : base("霊符") { } }

    public class TagScroll : Tag { public TagScroll() : base("巻物") { } }

    public class TagGem : Tag { public TagGem() : base("宝珠") { } }

    public class TagService : Tag { public TagService() : base("サービス") { } }

    public class TagHand : Tag
    {
        public bool IsOneHand { get; protected set; }
        public TagHand(bool isOneHand) : base(isOneHand ? "片手" : "両手") { IsOneHand = isOneHand; }
    }

    public class TagInstrument : Tag { public TagInstrument() : base("楽器") { } }

    public class TagShield : Tag { public TagShield() : base("盾") { } }

    public class TagArmor : Tag
    {
        public ArmorType Type { get; protected set; }
        public TagArmor(ArmorType type) : base(type.GetText()) { Type = type; }
    }

    public class TagAssistantEquipment : Tag { public TagAssistantEquipment() : base("補助装備") { } }

    public class TagEquipment : Tag
    {
        public EquipmentType Type { get; protected set; }
        public TagEquipment(EquipmentType type) : base(type.GetText()) { Type = type; }
    }

    public class TagBag : Tag { public TagBag() : base("鞄") { } }

    public class TagMagicGrade : TagValue
    {
        public TagMagicGrade(int value) : base("M", TagValueType.None, value) { }
        public override string ToString() { return "[" + Name + Value + "]"; }
    }

    public class TagMagicCatalyst : TagValue
    {
        public TagMagicCatalyst(int value) : base("魔触媒", TagValueType.None, value) { }
        public override string ToString() { return "[" + Name + " " + Value + "]"; }
    }

    public class TagCoreMaterial : Tag { public TagCoreMaterial() : base("コア素材") { } }

    public class TagProp : Tag
    {
        public PropType Type { get; protected set; }
        public TagProp(PropType type) : base(type.GetText()) { Type = type; }
    }

    public class TagTrap : Tag { public TagTrap() : base("トラップ") { } }

    public class TagOrigin : Tag
    {
        public OriginType Type { get; protected set; }
        public TagOrigin(OriginType type) : base(type.GetText()) { Type = type; }
    }

    public class TagGoods : Tag { public TagGoods() : base("物品") { } }

    public class TagBoss : Tag { public TagBoss() : base("ボス") { } }

    public class TagMob : Tag { public TagMob() : base("モブ") { } }

    public class TagNightVision : Tag { public TagNightVision() : base("暗視") { } }

    public class TagAquatic : Tag { public TagAquatic() : base("水棲") { } }

    public class TagLargeRace : Tag
    {
        public LargeRaceType Type { get; protected set; }
        public TagLargeRace(LargeRaceType type) : base(type.GetText()) { Type = type; }
    }

    public class TagEnemyRace : Tag { public TagEnemyRace(string name) : base(name) { } }

    public class TagPerson : Tag
    {
        public PersonType Type { get; protected set; }
        public TagPerson(PersonType type) : base(type.GetText()) { Type = type; }
    }

    public class TagPhase : Tag
    {
        public PhaseType Type { get; protected set; }
        public TagPhase(PhaseType type) : base(type.GetText()) { Type = type; }
    }

    public class TagParty : Tag { public TagParty() : base("パーティ") { } }
}
