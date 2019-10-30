using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AthensUtility;
using EnumExtension;

namespace LHTRPG
{
    public static partial class LHTRPGBase
    {
        /// <summary> 特定の種類のタグの個数を取得 </summary>
        public static int Count<T>(this IEnumerable<Tag> tags) where T : Tag => tags.Count(t => t.GetType() == typeof(T));

        /// <summary> 特定の種類のタグが存在しているか </summary>
        public static bool IsExist<T>(this IEnumerable<Tag> tags) where T : Tag => tags.Any(t => t.GetType() == typeof(T));

        /// <summary> 特定の種類のタグだけ抽出する </summary>
        public static IEnumerable<T> GetTags<T>(this IEnumerable<Tag> tags) where T : Tag => tags.OfType<T>().NotNull();

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

        public Tag(string name) => Name = name;

        public override bool Equals(object obj)
        {
            if (obj is Tag) return Name == (obj as Tag).Name;
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 77306245;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + IsCanOverlap.GetHashCode();
            return hashCode;
        }

        #region 事前定義
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
        #endregion
    }

    /// <summary> 種類タグ </summary>
    public class Tag<T> : Tag where T : System.Enum
    {
        public T Type { get; }
        public Tag(T type) : base(type.GetText()) { Type = type; }
    }
}
