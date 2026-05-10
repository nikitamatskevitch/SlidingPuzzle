using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MiniGame.SlidingPuzzle
{
	///<summary>
	/// Class that holds info about a slide tile's state (non-monobehaviour).
	///</summary>
	public class SlideTile
	{
        public Vector2 CorrectAnchoredPosition;
        /// <summary>
        /// The co-ordinates in the internal grid.
        /// </summary>
        public Vector2Int GridCoords;

        /// <summary>
        /// The dragging component.
        /// </summary>
        public TileDrag DragInput { get; private set; }

        /// <summary>
        /// Whether this is an empty tile.
        /// </summary>
        public bool Empty { get; private set; }

        /// <summary>
        /// Tile's rect transform.
        /// </summary>
        public RectTransform Transform { get; private set; }

        /// <summary>
        /// The desired position of this tile to be correct in the board.
        /// </summary>
        public int CorrectPosition { get;  }

        /// <summary>
        /// Tile's image component.
        /// </summary>
        private Image _tileImage;

        /// <summary>
        /// Text the tile to represent the index.
        /// </summary>
        private TileText _textController;

        /// <summary>
        /// A slideable tile in a board.
        /// </summary>
        /// <param name="empty">Is this tile empty (no other parameters provided if true)</param>
        /// <param name="gridCoords">What are the starting co-ordinates?</param>
        /// <param name="position">Position in the board from left to right, top to bottom e.g. first is 0, second is 1</param>
        public SlideTile(bool empty, Vector2Int gridCoords, int position)
        {
            Empty = empty;
            GridCoords = gridCoords;
            CorrectPosition = position;
        }

        /// <summary>
        /// A slideable tile in a board.
        /// </summary>
        /// <param name="empty">Is this tile empty (no other parameters provided if true)</param>
        /// <param name="gridCoords">The coordinates of the tile currently</param>
        /// <param name="position">Position in the board from left to right, top to bottom e.g. first is 0, second is 1</param>
        /// <param name="startCoords">The start coordinates of the tile</param>
        public SlideTile(bool empty, Vector2Int gridCoords, int position, Vector2Int startCoords) : this(empty, gridCoords, position) { }

        /// <summary>
        /// A slideable tile in a board.
        /// </summary>
        /// <param name="empty">Is this tile empty (no other parameters provided if true)</param>
        /// <param name="gridCoords">What are the starting co-ordinates?</param>
        /// <param name="position">Position in the board from left to right, top to bottom e.g. first is 0, second is 1</param>
        /// <param name="tile">Tile game object that represents this tile in the scene</param>
        /// <param name="tileSprite">The sprite section of the full image</param>
        /// <param name="index">The index of this tile.</param>
        public SlideTile(bool empty, Vector2Int gridCoords, int position, TileDrag tile, Sprite tileSprite,int index) : this(empty, gridCoords, position)
        {
            Transform = (RectTransform)tile.transform;
            Transform.GetChild(0).Rotate(new Vector3(0f, 0f, 90f * Random.Range(0, 3)));
            DragInput = tile;
            DragInput.CanDrag = false;
            _tileImage = tile.GetComponent<Image>();
            _textController = tile.GetComponent<TileText>();
            _textController.SetText(index);
            if (_tileImage != null)
                _tileImage.sprite = tileSprite;
        }

        /// <summary>
        /// A numbered slideable tile in a board
        /// </summary>
        /// <param name="empty">Is this tile empty (no other parameters provided if true)</param>
        /// <param name="gridCoords">What are the starting co-ordinates?</param>
        /// <param name="position">Position in the board from left to right, top to bottom e.g. first is 0, second is 1</param>
        /// <param name="tile">Tile game object that represents this tile in the scene</param>
        /// <param name="index">The index of this tile.</param>
        public SlideTile(bool empty, Vector2Int gridCoords, int position, TileDrag tile, int index) : this(empty, gridCoords, position)
        {
            Transform = (RectTransform)tile.transform;
            DragInput = tile;
            DragInput.CanDrag = false;
            _textController = tile.GetComponent<TileText>();
            _textController.SetText(index);
        }
    }
}