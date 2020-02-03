using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Memory
{
    public class MemoryGameController : MonoBehaviour
    {
        public MemoryAnimations MemoryAnimations;

        public AsyncState RunGame(MemoryGameModel gameModel)
        {
            // game login entry point
            return Planner.Chain()
                    .AddAwait(AwaitFunc)
                ;
        }

        private void AwaitFunc(AsyncStateInfo state)
        {
            // todo: game complete condition;
            state.IsComplete = false;
        }
    }
}
