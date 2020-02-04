using MiniGames.Common;
using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Memory
{
    public class MemoryScenario : GameScenarioBase
    {
        public MemoryGameController controller;

        //TODO: select gameModel with difficulty controller class
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
            // TODO: implement game circle using game controller
            // TODO: move hardcoded "5" count to game config
            for (var i = 0; i < 5; i++)
            {
                asyncChain
                        .AddFunc(controller.RunGame, defaultGameModel)
                        .AddFunc(progress.IncrementProgress)
                    ;
            }

            asyncChain.AddAction(Debug.Log, "game finished");
            return asyncChain;
        }

        private AsyncState Outro()
        {
            return Planner.Chain()
                    .AddAction(Debug.Log, "start outro")
                    .AddFunc(controller.MemoryAnimations.StartCutScene)
                    .AddAction(Debug.Log, "outro finished")
                ;
        }

    }
}