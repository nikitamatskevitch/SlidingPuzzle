using UnityEngine;
using UnityEngine.UI;

namespace DTT.MiniGame.SlidingPuzzle
{
    /// <summary>
    /// Class that allows the image snapping for the grid.
    /// </summary>
    [RequireComponent(typeof(Image))]

    public class SnappingPoint : MonoBehaviour
    {
        /// <summary>
        /// Can you actually snap to this snapping point.
        /// </summary>
        public bool SnappAble => SnapArea.raycastTarget;

        /// <summary>
        /// Reference to the image so raycast can be disabled when you have to get something back out.
        /// </summary>
        public Image SnapArea { get; protected set; }

        /// <summary>
        /// The canvas group of the component, can be null.
        /// </summary>
        public CanvasGroup CanvasGroup => _canvasGroup;

        /// <summary>
        /// The canvas group of the component, not required.
        /// </summary>
        [SerializeField]
        [Tooltip("Optional canvasgroup component")]
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// Gets the image to snap.
        /// </summary>
        protected virtual void Awake() => SnapArea = GetComponent<Image>();
    }
}
