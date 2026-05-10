using DTT.MinigameBase;
using DTT.MinigameBase.Timer;
using DTT.MinigameBase.UI;
using System;
using UnityEngine;
using System.Collections;

namespace DTT.MiniGame.SlidingPuzzle
{
    ///<summary>
    /// Manager class for the sliding tile puzzle game.
    ///</summary>
    [RequireComponent(typeof(SlideTileController))]
    public class SlidingPuzzleManager : MonoBehaviour, IMinigame<LevelData, SlidingPuzzleResult>, IFinishedable, IRestartable
    {
        /// <summary>
        /// Scriptable object for the image used.
        /// </summary>
        [SerializeField]
        [Tooltip("The level data for the mini game")]
        private LevelData _levelData;

        /// <summary>
        /// Reference to the timer of the game.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the timer of the game.")]
        private Timer _timer;

        /// <summary>
        /// Reference to tile controller.
        /// </summary>
        public SlideTileController TileController { get; private set; }

        /// <summary>
        /// Amount of times a tile was slid during a level.
        /// </summary>
        private int _slideAmount = 0;

        /// <summary>
        /// Whether the user has slid a tile yet.
        /// </summary>
        private bool _hasSlid = false;

        /// <summary>
        /// Whether the game is currently paused or not.
        /// </summary>
        private bool _isPaused = false;

        /// <summary>
        /// Whether the game is currently active or not.
        /// </summary>
        private bool _isGameActive = false;

        /// <summary>
        /// Invoked when a puzzle is solved.
        /// Passes the <see cref="SlidingPuzzleResult"/> of the game as the parameter.
        /// </summary>
        public event Action<SlidingPuzzleResult> Finish;

        /// <summary>
        /// Invoked when the game has started.
        /// </summary>
        public event Action Started;

        /// <summary>
        /// Invoked when the game ended.
        /// </summary>
        public event Action Finished;

        /// <summary>
        /// Whether the game is currently paused or not.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Whether the game is currently active or not.
        /// </summary>
        public bool IsGameActive => _isGameActive;

        /// <summary>
        /// The initialization of this object.
        /// </summary>
        protected void Awake() => TileController = GetComponent<SlideTileController>();

        /// <summary>
        /// Adds listeners to the tile controller.
        /// </summary>
        private void OnEnable()
        {
            TileController.onFirstMove += OnFirstMove;
            TileController.onTileMoved += CheckForCompletion;
        }

        /// <summary>
        /// Removes listeners from the tile controller.
        /// </summary>
        private void OnDisable()
        {
            TileController.onFirstMove -= OnFirstMove;
            TileController.onTileMoved -= CheckForCompletion;
        }

        /// <summary>
        /// Clears the level.
        /// </summary>
        public void ClearLevel() => TileController.ClearTiles();

        /// <summary>
        /// Starts the game with the given list of levels.
        /// </summary>
        /// <param name="levels">List of levels.</param>
        public void StartGame(LevelData level)
        {
            _levelData = level;
            SetupLevel();
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void Pause() => PauseLevel(true);

        /// <summary>
        /// Continues the game.
        /// </summary>
        public void Continue() => PauseLevel(false);

        /// <summary>
        /// Skips this level and goes to the next.
        /// </summary>
        public void ForceFinish()
        {
            _timer.Pause();
            Finish?.Invoke(new SlidingPuzzleResult(_slideAmount, _timer.TimePassed.Seconds));
            Finished?.Invoke();
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void Restart() => StartGame(_levelData);

        /// <summary>
        /// Sets up the level.
        /// </summary>
        private void SetupLevel()
        {
            ClearLevel();

            _slideAmount = 0;
            _hasSlid = false;
            _isGameActive = true;
            _isPaused = false;

            _timer.Begin();
            _timer.Pause();

            // Spawn tiles.
            System.Random r = new System.Random();
            TileController.CreateTiles(_levelData, r, 0);

            Started?.Invoke();
        }

        /// <summary>
        /// Checks if the puzzle has been completed.
        /// </summary>
        private void CheckForCompletion()
        {
            if (_hasSlid)
                _slideAmount++;

            if (TileController.PuzzleCompleted())
                ForceFinish();
        }

        /// <summary>
        /// Allows the game to be paused.
        /// </summary>
        private void PauseLevel(bool pause)
        {
            _isPaused = pause;

            if (pause)
            {
                _timer.Pause();
            }
            else
            {
                if (_hasSlid)
                    _timer.Resume();
            }

            TileController.SetTilesDraggable(!_isPaused);
        }

        /// <summary>
        /// Starts the timer and starts counting the amount of times the user slides a tile.
        /// </summary>
        private void OnFirstMove()
        {
            _timer.Resume();
            _hasSlid = true;
        }

        public void AutoComplete()
        {
            if (!_isGameActive || _isPaused)
                return;

            StartCoroutine(AutoCompleteCoroutine());
        }

        private IEnumerator AutoCompleteCoroutine()
        {
            // Блокируем управление и таймер
            TileController.SetTilesDraggable(false);
            _timer.Pause();
            _isGameActive = false;

            // Запускаем анимацию расстановки плиток и ждём её окончания
            yield return TileController.AutoArrangeTiles();

            // После расстановки вызываем штатное завершение уровня
            ForceFinish();
        }
    }
}