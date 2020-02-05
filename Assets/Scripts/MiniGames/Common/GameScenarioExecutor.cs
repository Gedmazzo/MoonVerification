using Moon.Asyncs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGames.Common
{
    public class GameScenarioExecutor: MonoBehaviour
    {
        public GameProgress progress;
        public GameScenarioBase scenario;
        public string nextSceneName;
        public bool isClearPrefs;

        private void OnEnable()
        {
            if (isClearPrefs)
                PlayerPrefs.DeleteAll();
        }

        private void Awake()
        {
            if (scenario == null)
            {
                Debug.LogError("GameScenarioExecutor has no scenario", this);
                return;
            }

            scenario.Inject(progress);

            Planner.Chain()
                    .AddFunc(scenario.ExecuteScenario)
                    .AddAction(LoadNextScene)
                ;
        }

        private void LoadNextScene()
        {
            // TODO: fade in
            SceneManager.LoadScene(nextSceneName);
            // TODO: fade out
        }
    }
}
