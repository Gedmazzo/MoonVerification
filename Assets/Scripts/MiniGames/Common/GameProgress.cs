using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Common
{
    public class GameProgress : MonoBehaviour
    {
        [SerializeField] private ProgressBar progressBar;

        public float NumberOfRounds { private get; set; }

        public AsyncState IncrementProgress()
        {
            return Planner.Chain()
                    .AddTween(progressBar.SetCurrentValue, 1f / NumberOfRounds)
                ;
        }

        public AsyncState ShowProgressBar()
        {
            return Planner.Chain()
                    .AddFunc(progressBar.Show)
                ;
        }

        public AsyncState CloseProgressBar()
        {
            return Planner.Chain()
                        .AddFunc(progressBar.Close)
                    ;
        }
    }
}