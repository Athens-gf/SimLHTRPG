using System;
using System.Collections;
using System.Collections.Generic;

namespace LHTRPG
{
    /// <summary> マップ上の管理クラス </summary>
    public class Map<T> : IEnumerable<T> where T : class
    {
        /// <summary> マップデータ二次元配列 </summary>
        private T[,] Data { get; set; }

        /// <summary> 行数 </summary>
        public int Row => Data.GetLength(0);

        /// <summary> 列数 </summary>
        public int Column => Data.GetLength(1);

        /// <summary> データ取得インデクサ </summary>
        public T this[int row, int column]
        {
            get
            {
                try
                {
                    return Data[row, column];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            set
            {
                try
                {
                    Data[row, column] = value;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Map(int row, int column, Func<T> generator = null)
        {
            Data = new T[row, column];
            for (int r = 0; r < row; ++r)
                for (int c = 0; c < column; ++c)
                    Data[r, c] = generator.Invoke();
        }

        /// <summary> 列指定したイテレータ </summary>
        /// <param name="row">列指定</param>
        public IEnumerable<T> GetEnumeratorRow(int row)
        {
            for (int i = 0; i < Column; i++)
                yield return Data[row, i];
        }

        /// <summary> 行指定したイテレータ </summary>
        /// <param name="column">行指定</param>
        public IEnumerable<T> GetEnumeratorColumn(int column)
        {
            for (int i = 0; i < Row; i++)
                yield return Data[i, column];
        }

        /// <summary> 全データのイテレータ </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int r = 0; r < Row; ++r)
                for (int c = 0; c < Column; ++c)
                    yield return Data[r, c];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
