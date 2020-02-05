using MiniGames.Common;
using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Memory
{
    public class MemoryScenario : GameScenarioBase
    {
        public MemoryGameController controller;

        public DifficultyController difficultyController;
        public MemoryGameModel defaultGameModel;

        protected override AsyncState OnExecute()
        {
            return Planner.Chain()
                    .AddFunc(Intro)
                    .AddFunc(GameCircle)
                    .AddFunc(Outro)
                ;
        }

        private AsyncState Intro()
        {
            return Planner.Chain()
                    .AddAction(Debug.Log, "start intro")
                    .AddFunc(controller.MemoryAnimations.StartCutScene)
                    .AddAction(Debug.Log, "intro finished")
                ;
        }

        private AsyncState GameCircle()
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "game started");
            asyncChain.AddFunc(progress.ShowProgressBar);
            asyncChain.AddAction(() => progress.NumberOfRounds = defaultGameModel.numberOfRounds);

            for (var i = 0; i < defaultGameModel.numberOfRounds; i++)
            {
                asyncChain
                        .AddAction(defaultGameModel.SetDifficultyController, difficultyController)
                        .AddFunc(controller.RunGame, defaultGameModel)
                        .AddFunc(progress.IncrementProgress)
                    ;
            }

            asyncChain.AddFunc(progress.CloseProgressBar);
            asyncChain.AddAction(Debug.Log, "game finished");
            return asyncChain;
        }

        private AsyncState Outro()
        {
            return Planner.Chain()
                    .AddAction(Debug.Log, "start outro")
                    .AddFunc(controller.MemoryAnimations.EndCutScene)
                    .AddAction(Debug.Log, "outro finished")
                ;
        }

    }
}