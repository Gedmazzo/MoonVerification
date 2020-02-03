using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Memory
{
    public class MemoryGameController : MonoBehaviour
    {
        public MemoryAnimations MemoryAnimations;
        public MemoryCardDeal memoryCardDeal;

        public AsyncState RunGame(MemoryGameModel gameModel)
        {
            // game login entry point
            return Planner.Chain()
                    .AddFunc(memoryCardDeal.CardDealing, 3)
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
