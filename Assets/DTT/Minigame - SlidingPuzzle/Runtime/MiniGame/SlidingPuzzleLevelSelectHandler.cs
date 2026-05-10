using DTT.MinigameBase.UI;
using UnityEngine;
using DTT.MinigameBase.LevelSelect;

namespace DTT.MiniGame.SlidingPuzzle
{
    public class SlidingPuzzleLevelSelectHandler : LevelSelectHandler<LevelData, SlidingPuzzleResult, SlidingPuzzleManager>
    {
        [SerializeField] private LevelData[] _configs;

        private void Start()
        {
            GameUI gameUI = FindObjectOfType<GameUI>();
            if (gameUI != null)
                gameUI.NextLevelRequested += LoadNextLevel;
        }

        private void LoadNextLevel()
        {
            int nextLevelNumber = CurrentLevel + 1;
            if (nextLevelNumber > _configs.Length)
            {
                ShowLevelSelect();
                return;
            }

            var minigame = FindObjectOfType<SlidingPuzzleManager>();
            if (minigame == null) return;

            SetCurrentLevel(nextLevelNumber);
            minigame.ClearLevel();
            minigame.StartGame(GetConfig(nextLevelNumber));
        }

        protected override float CalculateScore(SlidingPuzzleResult result) => result.score;

        protected override LevelData GetConfig(int levelNumber) => _configs[(levelNumber - 1) % _configs.Length];
    }
}
