using System.Collections.Generic;
using EnumExtension;

namespace LHTRPG
{
    public enum SceneType
    {
        [EnumText("シネマティック")] Cinematic,
        [EnumText("アブストラクト")] Abstract,
        [EnumText("マスター")] Master,
        [EnumText("戦闘")] Battle,
        [EnumText("ブリーフィング")] Briefing,
    }

    public class Scene
    {
        public SceneType Type { get; private set; }
        public Session Session { get; protected set; }
        public List<Adventurer> Players { get; protected set; }
        public List<Guest> Guests { get; protected set; }
        public List<Extra> Extras { get; protected set; }

        protected Scene(Session session, SceneType type)
        {
            Session = session;
            Type = type;
        }
    }
}
