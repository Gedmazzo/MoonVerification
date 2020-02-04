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
                    .AddAction(memoryCardDeal.SetImages, gameModel.images)
                    .AddFunc(memoryCardDeal.CardDealing, gameModel.numberOfCardPairs)
                    .AddAwait(AwaitFunc)
                ;
        }

        private void AwaitFunc(AsyncStateInfo state)
        {
            state.IsComplete = memoryCardDeal.IsFinishGameRound;
        }
    }
}
