using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnumExtension;
using KM.Unity;


namespace LHTRPG
{
    /// <summary> クリード(掟) </summary>
    public class Creed
    {
        /// <summary> クリード名 </summary>
        public string Name { get; set; }

        /// <summary> 説明 </summary>
        public string Explanation { get; set; }

        /// <summary> 要約 </summary>
        public string Summary { get; set; }

        /// <summary> 人物タグ種別 </summary>
        public Person Tag { get; set; }
    }

    /// <summary> ガイディングクリード </summary>
    public class GuidingCreed
    {
        /// <summary> 登録ガイディングクリード </summary>
        public static ReadOnlyCollection<Creed> Creeds { get; }

        /// <summary> ガイディングクリード名一覧 </summary>
        public static IEnumerable<string> CreedNames => Creeds.Select(c => c.Name);

        static GuidingCreed()
        {
            var csv = new CSVReader(@"Data/Creed");
            var lb = new List<Creed>();
            foreach (var line in csv.Line())
                lb.Add(new Creed
                {
                    Name = line[0],
                    Explanation = line[1],
                    Summary = line[2],
                    Tag = line[3].GetEnumByText<Person>()
                });
            Creeds = new ReadOnlyCollection<Creed>(lb);
        }

        /// <summary> ランダマイザ </summary>
        public static GuidingCreed GetRand() { return new GuidingCreed(Creeds.GetRand()); }

        /// <summary> ユニット </summary>
        public Unit Unit { get; set; }

        /// <summary> 選択ガイディングクリード </summary>
        public Creed Creed { get; set; }

        /// <summary> 人物タグ </summary>
        public TagPerson Tag { get; set; }

        public GuidingCreed(string creedName) : this(Creeds.First(c => c.Name == creedName)) { }
        public GuidingCreed(Creed creed) { Creed = creed; Tag = new TagPerson(Creed.Tag); }
    }
}
