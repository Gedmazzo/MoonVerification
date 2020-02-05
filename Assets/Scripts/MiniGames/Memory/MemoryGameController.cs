using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Memory
{
    public class MemoryGameController : MonoBehaviour
    {
        public MemoryAnimations MemoryAnimations;
        public MemoryCardDeal memoryCardDeal;
        public TutorialHand tutorialHand;

        public AsyncState RunGame(MemoryGameModel gameModel)
        {
            // game login entry point
            return Planner.Chain()
                    .AddAction(memoryCardDeal.SetImages, gameModel.images)
                    .AddAction(memoryCardDeal.SetDifficultyController, gameModel.GetController())
                    .AddFunc(memoryCardDeal.CardDealing, gameModel.numberOfCardPairs)
                    .AddFunc(tutorialHand.StartTutorial)
                    .AddAwait(AwaitFunc)
                ;
        }

        private void AwaitFunc(AsyncStateInfo state)
        {
            state.IsComplete = memoryCardDeal.IsFinishGameRound;
        }
    }
}
