using UnityEngine;
using UnityEngine.UI;
namespace DTT.MiniGame.SlidingPuzzle
{
    /// <summary>
    /// The component in the tile that handle the text.
    /// </summary>
    public class TileText : MonoBehaviour
    {
        /// <summary>
        /// Text component of the tile.
        /// </summary>
        [SerializeField]
        private Text _text;

        /// <summary>
        /// Set the text of a tile.
        /// </summary>
        /// <param name="tile">Tile game object that represents this tile in the scene.</param>
        /// <param name="text">Text component of the tile.</param>
        /// <param name="index">The index of this tile.</param>
        public void SetText( int index) => _text.text = index.ToString();
        
        
    }
}