using System.Collections.Generic;

namespace LHTRPG
{
    public class SceneBattle : Scene
    {
        public Dictionary<Adventurer, int> Hates { get; private set; }
        public Field Field { get; set; }
        public List<Enemy> Enemys { get; protected set; }
        public Dictionary<Unit, Terrain> Positions { get; protected set; }

        public SceneBattle(Session session) : base(session, SceneType.Battle)
        {
            Hates = new Dictionary<Adventurer, int>();
            Positions = new Dictionary<Unit, Terrain>();
        }
    }
}
