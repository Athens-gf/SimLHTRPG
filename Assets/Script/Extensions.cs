using System;
using System.Collections.Generic;
using System.Linq;

namespace KM.Utility
{
    public static class Extensions
    {
        // 文字列からremStrを取り除いて返す
        public static string Remove(this string _str, string _removeStr) { return _str.Replace(_removeStr, ""); }

        // 文字列中のcの数を数える
        public static int Count(this string _str, char _c) { return _str.Split(_c).Length - 1; }

        // コンテナに1つ要素を追加する
        public static IEnumerable<T> Append<T>(this IEnumerable<T> _iter, T _t)
        {
            var list = _iter.ToList(); list.Add(_t);
            return list;
        }

        // コンテナに複数要素を追加する
        public static IEnumerable<T> Append<T>(this IEnumerable<T> _iter, IEnumerable<T> _t)
        {
            var list = _iter.ToList(); list.AddRange(_t);
            return list;
        }

        // 一つの要素を初期要素としたコンテナを用意する
        public static IEnumerable<T> MakeCollection<T>(this T _t) { return new List<T>() { _t }; }

        // Enumをキーに取るDirectionaryを初期化
        public static Dictionary<K, V> ResetEnumDictionary<K, V>(this Dictionary<K, V> _dic, V _value) where K : struct
        {
            _dic = new Dictionary<K, V>();
            foreach (K v in Enum.GetValues(typeof(K))) _dic[v] = _value;
            return _dic;
        }
    }

    // DistinctのIEqualityComparer<T>の実装用
    public class CompareSelector<T, TKey> : IEqualityComparer<T>
    {
        private Func<T, TKey> selector;

        public CompareSelector(Func<T, TKey> _selector) { selector = _selector; }

        public bool Equals(T x, T y) { return selector(x).Equals(selector(y)); }

        public int GetHashCode(T obj) { return selector(obj).GetHashCode(); }
    }
}
