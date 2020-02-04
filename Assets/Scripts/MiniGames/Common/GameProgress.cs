using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Common
{
    public class GameProgress : MonoBehaviour
    {
        [SerializeField] private ProgressBar progressBar;

        private int progressMax;

        public void ResetProgress(int count)
        {
            progressBar.SetFill(0f);
            progressMax = count;
        }

        public AsyncState IncrementProgress()
        {
            return Planner.Chain()
                    // TODO: run progress animation, await finish
                    .AddTimeout(1f)
                ;
        }

        public AsyncState ShowProgressBar()
        {
            return Planner.Chain()
                    .AddFunc(progressBar.Show)
                    .AddAction(ResetProgress, 0)
                ;
        }
    }
}