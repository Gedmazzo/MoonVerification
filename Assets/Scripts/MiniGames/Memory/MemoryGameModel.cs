using UnityEngine;

namespace MiniGames.Memory
{
    [CreateAssetMenu(menuName = "MiniGames/Memory/MemoryGameModel")]
    public class MemoryGameModel: ScriptableObject
    {
        public int numberOfCardPairs;
        public float timeCardShow;
        public Texture2D[] images;
        public int numberOfRounds;

        private DifficultyController difficultyController;

        public void SetDifficultyController(DifficultyController difficultyController)
        {
            this.difficultyController = difficultyController;   
        }

        public DifficultyController GetController()
        {
            return difficultyController;
        }
    }
}