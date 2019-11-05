using System.Collections.Generic;

namespace LHTRPG
{
    /// <summary> マッププロップデータ </summary>
    public class Props
    {
        public Terrain Terrain { get; set; }
        public List<Space> Spaces { get; }
        public List<Wall> Walls { get; } = new List<Wall>();
        public List<Object> Objects { get; } = new List<Object>();

        public Props()
        {
            Terrain = new Terrain("草原", Origin.Natural, false);
            Spaces = new List<Space> { new Space("空間", Origin.Natural, false) };
        }
    }

    public class Field
    {
        /// <summary> フィールド位置情報 </summary>
        public struct Position
        {
            public int Row { get; set; }
            public int Columun { get; set; }
            public Position(int row, int columun) { Row = row; Columun = columun; }
        }

        /// <summary> バトルシーンへの参照 </summary>
        public SceneBattle Battle { get; }

        /// <summary> 初期配置可能位置 </summary>
        public List<Position> StartPos { get; } = new List<Position>();

        /// <summary> プロップ情報 </summary>
        public Map<Props> Props { get; }

        /// <summary> シーンエフェクト一覧 </summary>
        public List<SceneEffect> SceneEffects { get; } = new List<SceneEffect>();

        public Field(SceneBattle battle, int mapRow = 8, int mapColumn = 8)
        {
            Battle = battle;
            Props = new Map<Props>(mapRow, mapColumn, () => new Props());
        }
    }
}
