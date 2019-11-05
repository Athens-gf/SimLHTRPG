using System;
using System.Collections.Generic;
using EnumExtension;

namespace LHTRPG
{
    public enum PlayTiming
    {
        [EnumText("プリプレイ")] Pre,
        [EnumText("メインプレイ")] Main,
        [EnumText("アフタープレイ")] After,
    }

    /// <summary> セッション全体の進行 </summary>
    public class Session
    {
        /// <summary> セッション内のどのタイミングか </summary>
        public PlayTiming PlayTiming { get; set; }

        /// <summary> 冒険者のリスト </summary>
        public List<Adventurer> Players { get; } = new List<Adventurer>();

        // メインプレイ
        /// <summary> フェイズ毎のシーン一覧 </summary>
        public Dictionary<Phase, List<Scene>> Scenes { get; } = new Dictionary<Phase, List<Scene>>();

        /// <summary> 現在どのフェイズか </summary>
        public Phase Phase { get; set; }

        /// <summary> イテレータ管理のシーン順 </summary>
        protected IEnumerator<Scene> IterNextScene { get; set; }

        /// <summary> 現在のシーン </summary>
        public Scene CurrentScene => IterNextScene.Current;

        /// <summary> シーンイテレータ生成 </summary>
        protected IEnumerable<Scene> IterGetNextScene()
        {
            foreach (Phase phase in Enum.GetValues(typeof(Phase)))
            {
                Phase = phase;
                foreach (var scene in Scenes[phase])
                    yield return scene;
                yield return null;
            }
        }

        /// <summary> 次のシーンへ移動 </summary>
        /// <returns>次のシーン、最終の場合null</returns>
        public Scene NextScene() => IterNextScene.MoveNext() ? CurrentScene : null;

        public Session()
        {
            foreach (Phase phase in Enum.GetValues(typeof(Phase)))
                Scenes[phase] = new List<Scene>();
            Restart();
        }

        /// <summary> プリプレイからに設定 </summary>
        public void Restart()
        {
            PlayTiming = PlayTiming.Pre;
            IterNextScene = IterGetNextScene().GetEnumerator();
        }
    }
}
