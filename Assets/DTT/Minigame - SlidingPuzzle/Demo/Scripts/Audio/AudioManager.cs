using System.Collections.Generic;
using UnityEngine;

namespace DTT.MiniGame.SlidingPuzzle.Demo
{
    ///<summary>
    /// Holds and plays audio assets.
    ///</summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Holds the different audio objects that can be played.
        /// </summary>
        public enum GameSfx
        {
            [InspectorName("Tile Slide")]
            TILE_SLIDE = 0,
            [InspectorName("Card Match")]
            CARD_MATCH = 1,
            [InspectorName("Button Click")]
            UI_BUTTON_CLICK = 2
        }

        /// <summary>
        /// The audio source used to play the clips.
        /// </summary>
        [SerializeField]
        [Tooltip("The audio source used to play")]
        private AudioSource _audioSource;

        /// <summary>
        /// This list contains all the clips we will play.
        /// </summary>
        [SerializeField]
        [Tooltip("The audio clips we wish to play in app")]
        private List<AudioAsset> _clips;

        /// <summary>
        /// A set of dictionaries allowing quick look up of the correct clip by its enum.
        /// </summary>
        private readonly Dictionary<GameSfx, AudioAsset> _getGameClip = new Dictionary<GameSfx, AudioAsset>();

        /// <summary>
        /// Creates the dictionary to play audio through.
        /// </summary>
        protected void Awake() => CreateDictionaries();

        /// <summary>
        /// Plays an audio clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        public void PlayAudioClip(AudioAsset clip) => PlayAudioClip(clip, 1, 0);

        /// <summary>
        /// Plays an audio clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        public void PlayAudioClip(GameSfx clip) => PlayAudioClip(clip, 1, 0);

        /// <summary>
        /// Plays an audio clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <param name="volumeScale">The volume for the clip (0 to 1)</param>
        /// <param name="index">Index of the specific audio clip to play from the audio asset</param>
        public void PlayAudioClip(GameSfx clip, float volumeScale, int index) => PlayAudioClip(_getGameClip[clip], volumeScale, index);

        /// <summary>
        /// Plays an audio clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <param name="volumeScale">The volume for the clip (0 to 1)</param>
        /// <param name="index">Index of the specific audio clip to play from the audio asset</param>
        public void PlayAudioClip(AudioAsset clip, float volumeScale, int index) => _audioSource.PlayOneShot(clip.GetClip(index), volumeScale);

        /// <summary>
        /// Plays a random audio clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        public void PlayRandomAudioClip(GameSfx clip) => PlayRandomAudioClip(clip, 1);

        /// <summary>
        /// Plays a random audio clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        public void PlayRandomAudioClip(AudioAsset clip) => PlayRandomAudioClip(clip, 1);

        /// <summary>
        /// Plays a random audio clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <param name="volumeScale">The volume for the clip (0 to 1)</param>
        public void PlayRandomAudioClip(GameSfx clip, float volumeScale) => PlayRandomAudioClip(_getGameClip[clip], volumeScale);

        /// <summary>
        /// Plays a random audio clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <param name="volumeScale">The volume for the clip (0 to 1)</param>
        public void PlayRandomAudioClip(AudioAsset clip, float volumeScale) => _audioSource.PlayOneShot(clip.GetRandomClip(), volumeScale);

        /// <summary>
        /// Creates dictionaries for quick lookup of sound effects by enum.
        /// </summary>
        private void CreateDictionaries()
        {
            for (int i = 0; i < _clips.Count; i++)
                _getGameClip.Add((GameSfx)i, _clips[i]);
        }
    }
}
