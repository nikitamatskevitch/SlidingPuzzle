using UnityEngine;

namespace DTT.MiniGame.SlidingPuzzle.Demo
{
    ///<summary>
    /// Holds audio files.
    ///</summary>
    [CreateAssetMenu(fileName = "AudioAsset", menuName = "AudioAsset")]
    public class AudioAsset : ScriptableObject
    {
        /// <summary>
        /// Used for scaling audio down.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("This can be used to turn down non-music audio if it's too loud")]
        private float volume = 1f;

        /// <summary>
        /// Get and Set for volume.
        /// </summary>
        public float Volume
        {
            get => volume;
            set => volume = value;
        }

        /// <summary>
        /// Possible audio clips.
        /// </summary>
        [SerializeField]
        private AudioClip[] _possibleClips;

        /// <summary>
        /// Getter for all possible audio clips.
        /// </summary>
        public AudioClip[] PossibleClips => _possibleClips;

        /// <summary>
        /// Gets a specific audio clip.
        /// </summary>
        /// <param name="index">Index of the clip</param>
        /// <returns>Audio clip.</returns>
        public AudioClip GetClip(int index = 0) => _possibleClips[Mathf.Clamp(index, 0, _possibleClips.Length - 1)];

        /// <summary>
        /// Gets a random audio clip from the possible audio clips.
        /// </summary>
        /// <returns>Audio clip.</returns>
        public AudioClip GetRandomClip() =>  _possibleClips[Random.Range(0, _possibleClips.Length)];
    }
}
