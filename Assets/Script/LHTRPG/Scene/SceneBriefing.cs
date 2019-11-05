namespace LHTRPG
{
    public class SceneBriefing : Scene
    {
        public SceneBattle Battle { get; protected set; }

        public SceneBriefing(Session session, SceneBattle sceneBattle) : base(session, SceneType.Briefing)
        {
            Battle = sceneBattle;
        }

        public void SetFumbleScout()
        {
            foreach (var player in Players)
                Battle.Hates[player] = 3;
            Session.NextScene();
        }
    }
}
