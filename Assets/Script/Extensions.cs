using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AthensUtility
{
    public static class Extensions
    {
        private static readonly string[] FullWidthNumbers = { "０", "１", "２", "３", "４", "５", "６", "７", "８", "９" };

        /// <summary> 文字列からremoveStrを取り除いて返す </summary>
        /// <returns>取り除いた後の文字列</returns>
        /// <param name="str">元の文字列</param>
        /// <param name="removeStr">取り除く文字列</param>
        public static string Remove(this string str, string removeStr) => str.Replace(removeStr, "");

        /// <summary> 文字列str中のcの数をカウントする </summary>
        /// <returns>カウント結果</returns>
        /// <param name="str">元の文字列</param>
        /// <param name="c">カウントする文字</param>
        public static int Count(this string str, char c) => str.Split(c).Length - 1;

        /// <summary> すべて全角にした数値を返す </summary>
        /// <param name="value">数値</param>
        public static string FullWidth(this int value, bool isPlus = false)
        {
            var str = value.ToString().Replace("-", "－");
            FullWidthNumbers.Select((s, i) => new { Pre = i.ToString(), To = s })
                .ToList().ForEach(x => str = str.Replace(x.Pre, x.To));
            if (value >= 0)
                str = "＋" + str;
            return str;
        }

        /// <summary> すべて全角の数字文字列から数値取得 </summary>
        /// <param name="s">数値文字列</param>
        public static int FullWidth(this string s)
        {
            s = s.Replace("－", "-");
            FullWidthNumbers.Select((str, i) => new { Pre = str, To = i.ToString() })
                .ToList().ForEach(x => s = s.Replace(x.Pre, x.To));
            return int.Parse(s);
        }

        /// <summary> すべて全角の数字文字列から数値取得 </summary>
        /// <param name="s">数値文字列</param>
        /// <param name="result">変換数値</param>
        /// <returns>変換できたか</returns>
        public static bool TryParseFullWidth(string s, out int result)
        {
            s = s.Replace("－", "-");
            FullWidthNumbers.Select((str, i) => new { Pre = str, To = i.ToString() })
                .ToList().ForEach(x => s = s.Replace(x.Pre, x.To));
            return int.TryParse(s, out result);
        }

        /// <summary> 2つのActionを連続実行するActionを作成する </summary>
        public static Action Add(this Action action0, Action action1) => () => { action0(); action1(); };
        /// <summary> 2つのActionを連続実行するActionを作成する </summary>
        public static Action<T> Add<T>(this Action<T> action0, Action<T> action1) => x => { action0(x); action1(x); };
        /// <summary> 2つのActionを連続実行するActionを作成する </summary>
        public static Action<T1, T2> Add<T1, T2>(this Action<T1, T2> action0, Action<T1, T2> action1)
            => (x1, x2) => { action0(x1, x2); action1(x1, x2); };
        /// <summary> 2つのActionを連続実行するActionを作成する </summary>
        public static Action<T1, T2, T3> Add<T1, T2, T3>(this Action<T1, T2, T3> action0, Action<T1, T2, T3> action1)
            => (x1, x2, x3) => { action0(x1, x2, x3); action1(x1, x2, x3); };
        /// <summary> 2つのActionを連続実行するActionを作成する </summary>
        public static Action<T1, T2, T3, T4> Add<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action0, Action<T1, T2, T3, T4> action1)
            => (x1, x2, x3, x4) => { action0(x1, x2, x3, x4); action1(x1, x2, x3, x4); };

        /// <summary> DeapCopyを取得する </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T src)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, src);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }

    public static class ExIEnumerable
    {
        /// <summary> LinkedListのNodeを列挙する </summary>
        /// <param name="list">LinkedList.</param>
        public static IEnumerable<LinkedListNode<T>> EnumerateNodes<T>(this LinkedList<T> list)
        {
            var node = list.First;
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }

        /// <summary> nullじゃないものだけ抽出する </summary>
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> iter) where T : class => iter.Where(t => t != null);

        /// <summary> startから始まりstepずつ増えるcount個のint型配列を返す </summary>
        /// <param name="count">要素数</param>
        /// <param name="start">初期値</param>
        /// <param name="step">増えていく数</param>
        public static IEnumerable<int> Range(int count, int start = 0, int step = 1) => Enumerable.Range(0, count).Select(i => start + (i * step));

        /// <summary> コンテナに1つ要素を挿入したものを返す </summary>
        /// <returns>挿入後のコンテナ</returns>
        /// <param name="iter">挿入されるコンテナ</param>
        /// <param name="index">挿入する位置</param>
        /// <param name="t">挿入する要素</param>
        public static IEnumerable<T> InsertRetern<T>(this IEnumerable<T> iter, int index, T t)
        {
            var list = iter.ToList(); list.Insert(index, t);
            return list;
        }

        /// <summary> コンテナに別のコンテナを挿入したものを返す </summary>
        /// <returns>挿入後のコンテナ</returns>
        /// <param name="iter">挿入されるコンテナ</param>
        /// <param name="index">挿入する位置</param>
        /// <param name="insertIter">挿入するコンテナ</param>
        public static IEnumerable<T> InsertRetern<T>(this IEnumerable<T> iter, int index, IEnumerable<T> insertIter)
        {
            var list = iter.ToList(); list.InsertRange(index, insertIter);
            return list;
        }

        /// <summary> コンテナから要素を削除したものを返す </summary>
        /// <returns>追加後のコンテナ</returns>
        /// <param name="iter">追加されるコンテナ</param>
        /// <param name="t">削除する要素</param>
        public static IEnumerable<T> RemoveRetern<T>(this IEnumerable<T> iter, T t) => iter.Where(x => !x.Equals(t));

        /// <summary> コンテナから別のコンテナ要素をすべて削除したものを返す </summary>
        /// <returns>追加後のコンテナ</returns>
        /// <param name="iter">追加されるコンテナ</param>
        /// <param name="removeIter">削除する要素</param>
        public static IEnumerable<T> RemoveRetern<T>(this IEnumerable<T> iter, IEnumerable<T> removeIter)
            => iter.Where(x => !removeIter.Any(rx => x.Equals(rx)));

        /// <summary> コンテナから特定の位置の要素を削除したものを返す </summary>
        /// <returns>追加後のコンテナ</returns>
        /// <param name="iter">追加されるコンテナ</param>
        public static IEnumerable<T> RemoveAtRetern<T>(this IEnumerable<T> iter, int index) => iter.Where((_, i) => i != index);

        /// <summary> コンテナ内で条件に一致したすべての要素のインデックスを返す </summary>
        /// <param name="iter">コンテナ</param>
        /// <param name="check">確認用関数</param>
        /// <returns>一致したインデックスのリスト</returns>
        public static IEnumerable<int> AllIndex<T>(this IEnumerable<T> iter, Func<T, bool> check)
            => iter.Select((t, i) => new { Value = t, Index = i }).Where(x => check(x.Value)).Select(x => x.Index);

        /// <summary> コンテナ内で条件に最初に一致した要素のインデックスを返す </summary>
        /// <param name="iter">コンテナ</param>
        /// <param name="check">確認用関数</param>
        /// <returns>一致したインデックス、存在しなかった場合「-1」</returns>
        public static int FirstIndex<T>(this IEnumerable<T> iter, Func<T, bool> check)
            => iter.Any(check) ? iter.AllIndex(check).First() : -1;

        /// <summary> コンテナ内で条件に最初に一致した要素のインデックスを返す </summary>
        /// <param name="iter">コンテナ</param>
        /// <param name="check">確認用関数</param>
        /// <param name="default">デフォルトインデックス</param>
        /// <returns>一致したインデックス、存在しなかった場合「@default」</returns>
        public static int FirstIndex<T>(this IEnumerable<T> iter, Func<T, bool> check, int @default)
            => iter.Any(check) ? iter.AllIndex(check).First() : @default;

        /// <summary> コンテナ内で条件に最後に一致した要素のインデックスを返す </summary>
        /// <param name="iter">コンテナ</param>
        /// <param name="check">確認用関数</param>
        /// <returns>一致したインデックス、存在しなかった場合「-1」</returns>
        public static int LastIndex<T>(this IEnumerable<T> iter, Func<T, bool> check)
            => iter.Any(check) ? iter.AllIndex(check).Last() : -1;

        /// <summary> コンテナ内で条件に最初に一致した要素のインデックスを返す </summary>
        /// <param name="iter">コンテナ</param>
        /// <param name="check">確認用関数</param>
        /// <param name="default">デフォルトインデックス</param>
        /// <returns>一致したインデックス、存在しなかった場合「@default」</returns>
        public static int LastIndex<T>(this IEnumerable<T> iter, Func<T, bool> check, int @default)
            => iter.Any(check) ? iter.AllIndex(check).Last() : @default;

        /// <summary> コンテナ内の最初の要素を返す、コンテナが空の場合指定したデフォルト要素を返す </summary>
        /// <param name="iter">コンテナ</param>
        /// <param name="default">デフォルト要素</param>
        /// <returns>一致した要素、存在しなかった場合「-1」</returns>
        public static T FirstOrDefault<T>(this IEnumerable<T> iter, T @default) => iter.Any() ? iter.First() : @default;

        /// <summary> コンテナ内で条件に最初に一致した要素を返す、一致する要素がない場合指定したデフォルト要素を返す </summary>
        /// <param name="iter">コンテナ</param>
        /// <param name="check">確認用関数</param>
        /// <param name="default">デフォルト要素</param>
        /// <returns>一致した要素、存在しなかった場合「-1」</returns>
        public static T FirstOrDefault<T>(this IEnumerable<T> iter, Func<T, bool> check, T @default) => iter.Any(check) ? iter.First(check) : @default;

        /// <summary> コンテナ内の最後の要素を返す、コンテナが空の場合指定したデフォルト要素を返す </summary>
        /// <param name="iter">コンテナ</param>
        /// <param name="default">デフォルト要素</param>
        /// <returns>一致した要素、存在しなかった場合「-1」</returns>
        public static T LastOrDefault<T>(this IEnumerable<T> iter, T @default) => iter.Any() ? iter.Last() : @default;

        /// <summary> コンテナ内で条件に最後に一致した要素を返す、一致する要素がない場合指定したデフォルト要素を返す </summary>
        /// <param name="iter">コンテナ</param>
        /// <param name="check">確認用関数</param>
        /// <param name="default">デフォルト要素</param>
        /// <returns>一致した要素、存在しなかった場合「-1」</returns>
        public static T LastOrDefault<T>(this IEnumerable<T> iter, Func<T, bool> check, T @default) => iter.Any(check) ? iter.Last(check) : @default;

        /// <summary> 2つのコンテナが同じかどうかを返す </summary>
        public static bool Same<T>(this IEnumerable<T> iter0, IEnumerable<T> iter1) => !iter0.Except(iter1).Any() && !iter1.Except(iter0).Any();

        /// <summary> ある要素を初期要素としたコンテナを用意し，返す </summary>
        /// <returns>用意したコンテナ</returns>
        /// <param name="t">初期要素</param>
        public static IEnumerable<T> MakeCollection<T>(this T t) { return new List<T> { t }; }

        /// <summary> 例外要素の場合例外を返す </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">チェックする値</param>
        /// <param name="predicate">例外チェック関数</param>
        /// <param name="exception">例外</param>
        public static T ThrowIf<T>(this T value, Func<T, bool> predicate, Exception exception)
        {
            if (predicate(value)) throw exception;
            return value;
        }

        /// <summary> コンテナ内に例外要素が存在する場合例外を返す </summary>
        /// <param name="predicate">例外チェック関数</param>
        /// <param name="exception">例外</param>
        public static IEnumerable<T> ThrowIf<T>(this IEnumerable<T> iter, Func<T, bool> predicate, Exception exception)
        {
            try
            {
                return iter.Select(x => x.ThrowIf(predicate, exception));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary> 三項演算子のチェーンを崩さないため </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        static T Throw<T>(Exception exception)
        {
            throw exception;
        }
    }
}
