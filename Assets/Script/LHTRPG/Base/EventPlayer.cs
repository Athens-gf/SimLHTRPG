using System;
using System.Collections.Generic;

namespace LHTRPG
{
    public enum EventType
    {
        /// <summary> エフェクト発生(種類、発生源(Unit)、対象(List[Unit])) </summary>
        Effect,
        /// <summary> ダイスを振る(種類：ダメージロール・〜〜判定、ロール結果(RollResult)) </summary>
        DiceRoll,
        /// <summary> ダメージを与える(ダメージアクション(Action)、発生源(Unit)、(List[対象(Unit),ロール結果(RollResult)])) </summary>
        Damage,
        /// <summary> 回復する(回復アクション(Action)、発生源(Unit)、(List[対象(Unit),ロール結果(RollResult)])) </summary>
        Heal,
        /// <summary> ステータスを付与する(ステータス(IStatusTag)、発生源(Unit)、(List[対象(Unit)])) </summary>
        GiveStatus,
        /// <summary> ステータスが発動する(ステータス(IStatusTag)、対象(Unit)) </summary>
        TriggerStatus,
        /// <summary> ステータスを取り除く(ステータス(IStatusTag)、対象(Unit)) </summary>
        RemoveStatus,
    }

    public class EventPlayer : LinkedList<EventPlayer.Tuple>
    {
        public class Tuple
        {
            public EventType Type { get; set; }
            public IEnumerable<object> Items { get; set; }
        }

        public LinkedListNode<Tuple> InsertPosNode { get; private set; }

        public LinkedListNode<Tuple> AddNext(EventType type, IEnumerable<object> items)
            => InsertPosNode = AddAfter(InsertPosNode, new Tuple { Type = type, Items = items });

        public LinkedListNode<Tuple> AddLast(EventType type, IEnumerable<object> items)
            => AddLast(new Tuple { Type = type, Items = items });
    }
}
