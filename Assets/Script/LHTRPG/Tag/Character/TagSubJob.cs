using System.Collections.Generic;
using System.Collections.ObjectModel;
using KM.Unity;

namespace LHTRPG
{
    /// <summary> サブ職業タグ </summary>
    public class TagSubJob : Tag
    {
        /// <summary> 登録サブ職業一覧 </summary>
        public static ReadOnlyCollection<string> SubJobs { get; }

        static TagSubJob()
        {
            var csv = new CSVReader(@"Data/SubJobs");
            var l = new List<string>();
            for (int i = 0; i < csv.Row; i++)
                l.Add(csv[i][0]);
            SubJobs = new ReadOnlyCollection<string>(l);
        }

        /// <summary> サブ職業乱数取得 </summary>
        public static TagSubJob GetRand() => new TagSubJob(SubJobs.GetRand());

        /// <summary> 一覧にないサブ職業を登録するとき、処理のベースになる職業 </summary>
        public string BaseSubJob { get; }

        public TagSubJob(string name) : base(name) { BaseSubJob = name; }
        public TagSubJob(string name, string @base) : base(name) { BaseSubJob = @base; }
    }
}
