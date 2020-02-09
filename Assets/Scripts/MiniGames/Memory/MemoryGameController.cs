using Moon.Asyncs;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Memory
{
    public class MemoryGameController : MonoBehaviour
    {
        public AudioManager audioManager;
        public MemoryAnimations MemoryAnimations;
        public MemoryCardDeal memoryCardDeal;
        public TutorialHand tutorialHand;
        public Button helpButton;

        public AsyncState RunGame(MemoryGameModel gameModel)
        {
            // game login entry point
            return Planner.Chain()
                    .AddAction(memoryCardDeal.SetImages, gameModel.images)
                    .AddAction(memoryCardDeal.SetDifficultyController, gameModel.GetController())
                    .AddAction(memoryCardDeal.SetAudioManager, audioManager)
                    .AddFunc(memoryCardDeal.CardDealing, gameModel.numberOfCardPairs)
                    .AddFunc(tutorialHand.StartTutorial)
                    .AddAction(() =>
                    {
                        if (gameModel.helpCount != 0)
                        {
                            helpButton.gameObject.SetActive(true);
                            helpButton.onClick.AddListener(() => audioManager.Play("HelpEffect"));
                            memoryCardDeal.MaxHelpCount = gameModel.helpCount;
                            memoryCardDeal.HelpButton = helpButton;
                        }
                    })
                    .AddAwait(AwaitFunc)
                ;
        }

        private void AwaitFunc(AsyncStateInfo state)
        {
            state.IsComplete = memoryCardDeal.IsFinishGameRound;
        }
    }
}
