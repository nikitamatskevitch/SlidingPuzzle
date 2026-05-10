using UnityEngine;

namespace DTT.MiniGame.SlidingPuzzle.Demo
{
    /// <summary>
    /// Handles playing audio for the sliding puzzle game events.
    /// </summary>
    public class SlidePuzzleAudioHandler : MonoBehaviour
    {
        /// <summary>
        /// Reference to the game manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the game manager.")]
        private SlidingPuzzleManager _gameManager;

        /// <summary>
        /// Reference to the tile controller.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the tile controller.")]
        private SlideTileController _tileController;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the audio manager.")]
        private AudioManager _audioManager;

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable()
        {
            _gameManager.Finished += OnGameEnd;
            _tileController.onTileMoved += OnSlide;
        }

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable()
        {
            _gameManager.Finished -= OnGameEnd;
            _tileController.onTileMoved -= OnSlide;
        }

        /// <summary>
        /// Plays game end audio.
        /// </summary>
        private void OnGameEnd() => _audioManager.PlayAudioClip(AudioManager.GameSfx.CARD_MATCH);

        /// <summary>
        /// Plays tile sliding audio.
        /// </summary>
        private void OnSlide() => _audioManager.PlayRandomAudioClip(AudioManager.GameSfx.TILE_SLIDE);
    }
}