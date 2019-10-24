using System;
using EnumExtension;
using KM.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace LHTRPG
{
    public class Session
    {
        public enum Play
        {
            [EnumText("プリプレイ")] Pre,
            [EnumText("メインプレイ")] Main,
            [EnumText("アフタープレイ")] After,
        }
        public Play Play { get; protected set; }
        public List<Adventurer> Players { get; protected set; }
        // メインプレイ
        public Dictionary<TagPhase.Type, List<Scene>> Scenes { get; protected set; }
        public TagPhase.Type Phase { get; set; }
        protected IEnumerator<Scene> IterNextScene { get; set; }
        public Scene CurrentScene { get { return IterNextScene.Current; } }
        protected IEnumerable<Scene> IterGetNextScene()
        {
            foreach (TagPhase.Type phase in Enum.GetValues(typeof(TagPhase.Type)))
            {
                Phase = phase;
                foreach (var scene in Scenes[phase])
                    yield return scene;
                yield return null;
            }
        }

        public Scene NextScene() { return IterNextScene.MoveNext() ? CurrentScene : null; }

        public Session()
        {
            Play = Play.Pre;
            Players = new List<Adventurer>();
            Scenes = new Dictionary<TagPhase.Type, List<Scene>>();
            foreach (TagPhase.Type phase in Enum.GetValues(typeof(TagPhase.Type)))
                Scenes[phase] = new List<Scene>();
            IterNextScene = IterGetNextScene().GetEnumerator();
        }
    }

    public class Scene
    {
        public enum Type
        {
            [EnumText("シネマティック")] Cinematic,
            [EnumText("アブストラクト")] Abstract,
            [EnumText("マスター")] Master,
            [EnumText("戦闘")] Battle,
            [EnumText("ブリーフィング")] Briefing,
        }
        public Type Type { get; private set; }
        public Session Session { get; protected set; }
        public List<Adventurer> Players { get; protected set; }
        public List<Guest> Guests { get; protected set; }
        public List<Extra> Extras { get; protected set; }

        protected Scene(Session _session, Type _type)
        {
            Session = _session;
            Type = _type;
        }
    }

    public class SceneBriefing : Scene
    {
        public SceneBattle Battle { get; protected set; }

        public SceneBriefing(Session _session, SceneBattle _sceneBattle) : base(_session, Type.Briefing)
        {
            Battle = _sceneBattle;
        }

        public void SetFumbleScout()
        {
            foreach (var player in Players)
                Battle.Hates[player] = 3;
            Session.NextScene();
        }
    }

    public class SceneBattle : Scene
    {
        public Dictionary<Adventurer, int> Hates { get; private set; }
        public Field Field { get; set; }
        public List<Enemy> Enemys { get; protected set; }
        public Dictionary<Unit, Terrain> Positions { get; protected set; }

        public SceneBattle(Session _session) : base(_session, Type.Battle)
        {
            Hates = new Dictionary<Adventurer, int>();
            Positions = new Dictionary<Unit, Terrain>();
        }
    }

    public struct Position
    {
        public int Row { get; set; }
        public int Columun { get; set; }
        public Position(int _row, int _columun) { Row = _row; Columun = _columun; }
    }

    public class Map<T> where T : class
    {
        private List<List<T>> Data { get; set; }
        public int Row { get { return Data.Count; } }
        public int Column { get { return Data.Count == 0 ? 0 : Data[0].Count; } }
        public T this[int _row, int _column] { get { return Data[_row][_column]; } set { Data[_row][_column] = value; } }

        public Map(int _row, int _column, Func<T> _new = null)
        {
            Data = new List<List<T>>(_row);
            for (int i = 0; i < _row; i++)
            {
                Data[i] = new List<T>(_column);
                for (int j = 0; j < _column; j++)
                    Data[i][j] = _new == null ? null : _new();
            }
        }

        public IEnumerable<T> GetIterRow(int _row) { for (int i = 0; i < Column; i++) yield return Data[_row][i]; }

        public IEnumerable<T> GetIterColumn(int _column) { for (int i = 0; i < Row; i++) yield return Data[i][_column]; }

        public IEnumerable<T> GetIter()
        {
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Column; j++)
                    yield return Data[i][j];
        }
    }

    public class Field
    {
        public SceneBattle Battle { get; protected set; }
        public List<Position> StartPos { get; protected set; }
        public Map<Terrain> Terrain { get; protected set; }
        public Map<List<Space>> Space { get; protected set; }

        public Field(SceneBattle _battle, int _mapRow = 8, int _mapColumn = 8)
        {
            Battle = _battle;
            Terrain = new Map<Terrain>(_mapRow, _mapColumn, () => new Terrain("草原", TagOrigin.Type.Natural, false));
            Space = new Map<List<Space>>(_mapRow, _mapColumn, () => new List<Space>() { new Space("空間", TagOrigin.Type.Natural, false) });
            StartPos = new List<Position>();
        }
    }
}
