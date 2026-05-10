using UnityEngine;

namespace DTT.MiniGame.SlidingPuzzle
{
    /// <summary>
    /// Handle the outline of the board.
    /// </summary>
    public class Outline : MonoBehaviour
    {
        /// <summary>
        /// Hold the rect of a side outline of the board.
        /// </summary>
        [SerializeField]
        private RectTransform _leftOutLine;
        
        /// <summary>
        /// Hold the rect of the top outline of the board.
        /// </summary>
        [SerializeField]
        private RectTransform _topOutLine;
        
        /// <summary>
        /// Factor for calculation of the board outline size,
        /// multiply by 2 to take both side into account plus a small offset.
        /// </summary>
        private const float OUTLINE_FACTOR = 2.2f;

        /// <summary>
        /// Get the size of the outline.
        /// </summary>
        /// <returns>The width and height of the outline.</returns>
        public Vector2 GetOutlineSize() => new Vector2(_leftOutLine.rect.size.x, _topOutLine.rect.size.y) * OUTLINE_FACTOR;
    }
}