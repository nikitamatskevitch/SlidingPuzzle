using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = System.Random;

namespace DTT.MiniGame.SlidingPuzzle
{
    ///<summary>
    /// Controller for sliding tile puzzle game.
    ///</summary>
    public class SlideTileController : MonoBehaviour
    {
        public Vector2 CorrectAnchoredPosition;
        /// <summary>
        /// Main layout group of the tiles.
        /// </summary>
        [SerializeField]
        [Tooltip("Layout group of the tiles")]
        private GridLayoutGroup _tileLayoutGroup;

        /// <summary>
        /// The outline of the board.
        /// </summary>
        [SerializeField]
        [Tooltip("Outline of the board")]
        private RectTransform _boardOutline;

        /// <summary>
        /// The beakground of the board.
        /// </summary>
        [SerializeField]
        [Tooltip("Background of the board")]
        private RectTransform _boardBackground;

        /// <summary>
        /// Image for the board completed Sprite shown when the game is completed.
        /// </summary>
        [SerializeField]
        [Tooltip("The Image holding the board completed image")]
        private RectTransform _boardCompleted;

        /// <summary>
        /// The prefab of an image tile.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab for the tile slide")]
        private TileDrag _slideTileImagePrefab;
        
        /// <summary>
        /// The prefab of a numbered tile.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab for the tile slide")]
        private TileDrag _slideTileNumberedPrefab;

        /// <summary>
        /// The prefab of an outline.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab for the outline of the board")]
        private Outline _outlinePrefab;

        /// <summary>
        /// The animation speed of a tile.
        /// </summary>
        [SerializeField]
        [Tooltip("Animation speed of the tiles")]
        private float _tileAnimSpeed = 0.1f;

        /// <summary>
        /// Camera for the scene.
        /// </summary>
        [SerializeField]
        [Tooltip("Camera for the scene")]
        private Camera _sceneCamera;
        
        /// <summary>
        /// Whether the index start horinzontaly or verticaly.
        /// </summary>
        [Header("Index")]
        [SerializeField]
        [Tooltip("Horizontal index")]
        private bool horizontal;

        /// <summary>
        /// Whether to start the indexing at 0 or 1.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether the first index is 1 or 0")]
        private bool startingAtZero;

        /// <summary>
        /// Nullable value for which way is the current moving tile going to get into an empty space.
        /// </summary>
        public Vector2Int? TileMoveDir { get; private set; }

        /// <summary>
        /// Getter for the tileMoveAmount parameter.
        /// </summary>
        public float TileMoveAmount => _tileMoveAmount;

        /// <summary>
        /// Getter for the scrambeling parameter.
        /// </summary>
        public bool IsScrambling => _isScrambling;

        /// <summary>
        /// Size of the board.
        /// </summary>
        public LevelData LevelData { get; private set; }

        /// <summary>
        /// Action performed at the end of a tile move.
        /// </summary>
        public event Action onTileMoved;

        /// <summary>
        /// Action performed when the user moves a tile for the first time.
        /// </summary>
        public event Action onFirstMove;

        /// <summary>
        /// Action performed when the scrambling of the tiles has been finished.
        /// </summary>
        public event Action onScrambleEnd;

        /// <summary>
        /// How far in UI units does a tile need to move to go to a different space?
        /// </summary>
        private float _tileMoveAmount;

        /// <summary>
        /// Is set to true whenever the game is scrambeling, and false whenever it stopped.
        /// </summary>
        private bool _isScrambling = false;

        /// <summary>
        /// Permanent list of our current slide tiles.
        /// </summary>
        private List<SlideTile> _slideTiles;

        /// <summary>
        /// Grid representation of our tiles.
        /// </summary>
        private Grid<SlideTile> _tileGrid;

        /// <summary>
        /// The reference to our instance of random.
        /// </summary>
        private Random _random;

        /// <summary>
        /// Tile max position according to the maximum position of the board, 
        /// to be used to clamp the max position to prevent tile to be outside the box.
        /// It's value is assigned on the creation of the tile board.
        /// </summary>
        private Vector2 _tileMaxPos = new Vector2(float.MinValue, float.MinValue);

        /// <summary>
        /// Tile min position according to the minimum position of the board, 
        /// to be used to clamp the min position to prevent tile to be outside the box.
        /// It's value is assigned on the creation of the tile board.
        /// </summary>
        private Vector2 _tileMinPos = new Vector2(float.MaxValue, float.MaxValue);

        /// <summary>
        /// The initialization of this object.
        /// </summary>
        protected void Awake()
        {
            // Add the board outline.
            Outline outline = Instantiate(_outlinePrefab, _boardOutline.transform);
            _boardOutline = outline.GetComponent<RectTransform>();

            if (_sceneCamera == null)
                Debug.LogError("sceneCamera inspector value was not set");
        }
        
        /// <summary>
        /// Create and layout all our new tiles for a level.
        /// </summary>
        /// <param name="size">the size of the tiles</param>
        /// <param name="levelData">Configuration of the level.</param>
        /// <param name="random">Random generator</param>
        /// <param name="scrambleDelay">The delay before the scrambling starts</param>
        internal void CreateTiles(LevelData levelData, Random random, float scrambleDelay = 0f)
        {
            LevelData = levelData;
            int size = LevelData.Size;

            //_readyToScramble = false;.
            _random = random;

            // Set size and scale of tile board.
            ResizeGrid(size);

            
            _boardCompleted.gameObject.SetActive(true);

            if (_boardCompleted.GetComponent<Image>() == null)
                _boardCompleted.gameObject.AddComponent<Image>();

            Sprite[] tileSprites = new Sprite[size*size];
            // Get the individual tile sprites.
            if (levelData.GameType == GameType.IMAGE)
            {
                tileSprites = LevelData.GetTileSprites();
                _boardCompleted.GetComponent<Image>().sprite = LevelData.GetCompletedImage();
            }

            Color boardColor = _boardCompleted.GetComponent<Image>().color;

            _slideTiles = new List<SlideTile>();
            int i;
            int index = 0; 
            int column = 0;
            int row = 0;
            // Creation of regular tiles.
            for (i = 0; i < (size * size) - 1; ++i)
            {
                index = (horizontal) ? (row+size*column) : index;
                index = (startingAtZero) ? index : index + 1;
                if (levelData.GameType == GameType.NUMBERED) 
                    _slideTiles.Add(new SlideTile(false, new Vector2Int(i % _tileLayoutGroup.constraintCount, i / _tileLayoutGroup.constraintCount), i, 
                        Instantiate(_slideTileNumberedPrefab, _tileLayoutGroup.transform), index));
                else
                    _slideTiles.Add(new SlideTile(false, new Vector2Int(i % _tileLayoutGroup.constraintCount, i / _tileLayoutGroup.constraintCount), i,
                        Instantiate(_slideTileImagePrefab, _tileLayoutGroup.transform), tileSprites[i], index));

                int tileIndex = i;
                TileDrag drag = _slideTiles[i].DragInput;
                drag.SceneCamera = _sceneCamera;
                drag.TileController = this;
                drag.CanDrag = false;
                drag.OnStartDragAction += () =>
                {
                    if (!drag.CanDrag)
                        return;

                    drag.SetInitialPosition();

                    TileInteract(_slideTiles[tileIndex]);
                };

                drag.OnPointerUpAction += () =>
                {
                    if (!drag.CanDrag)
                        return;

                    // Action communicates to the levelManager to start the timer from the UI.
                    onFirstMove?.Invoke();

                    drag.SetRayCast(false);

                    if (!drag.BeenDragged)
                        MoveTile(_slideTiles[tileIndex]);
                    else if (Vector3.Distance(drag.InitialAnchoredPosition, _slideTiles[tileIndex].Transform.anchoredPosition) < _tileMoveAmount / 2)
                        MoveTileToOrigin(_slideTiles[tileIndex]);
                    else
                        MoveTile(_slideTiles[tileIndex]);
                };
                column = (i + 1) % size;
                row = (int) (i + 1) / size;
            }

            // Add empty tile.
            _slideTiles.Add(new SlideTile(true, new Vector2Int(i % _tileLayoutGroup.constraintCount, i / _tileLayoutGroup.constraintCount), i));

            // Call the layout group to arrange the tile objects.
            _tileLayoutGroup.CalculateLayoutInputHorizontal();
            _tileLayoutGroup.CalculateLayoutInputVertical();
            _tileLayoutGroup.SetLayoutHorizontal();
            _tileLayoutGroup.SetLayoutVertical();

            for (int t = 0; t < _slideTiles.Count - 1; t++)
            {
                _slideTiles[t].CorrectAnchoredPosition =
                    _slideTiles[t].Transform.anchoredPosition;
            }

            // Create our new internal grid.
            _tileGrid = new Grid<SlideTile>(size, size, _slideTiles.ToArray());

            // Get how far our tiles need to move in canvas space.
            _tileMoveAmount = _tileLayoutGroup.cellSize.x;

            // Set the size and render order of the board outline sprite.
            _boardOutline.sizeDelta = _tileLayoutGroup.cellSize * _tileLayoutGroup.constraintCount;
            _boardOutline.sizeDelta += _outlinePrefab.GetOutlineSize();
            
            _boardOutline.gameObject.SetActive(true);

            _boardBackground.sizeDelta = _outlinePrefab.GetOutlineSize() + _tileLayoutGroup.cellSize * _tileLayoutGroup.constraintCount;
            _boardBackground.gameObject.SetActive(true);
            _boardCompleted.transform.position = _boardBackground.transform.position;
            _boardCompleted.GetComponent<Image>().preserveAspect = true;

            // Set the highest/lowest value anchored position, used to prevent tiles to be outside the board.
            for (i = 0; i < (size * size) - 1; ++i)
            {
                // Find & Replace the max possible position (X + Y axis).
                if (_slideTiles[i].Transform.anchoredPosition.x > _tileMaxPos.x)
                    _tileMaxPos.x = _slideTiles[i].Transform.anchoredPosition.x;

                if (_slideTiles[i].Transform.anchoredPosition.y > _tileMaxPos.y)
                    _tileMaxPos.y = _slideTiles[i].Transform.anchoredPosition.y;

                // Find & Replace the min possible position (X + Y axis).
                if (_slideTiles[i].Transform.anchoredPosition.x < _tileMinPos.x)
                    _tileMinPos.x = _slideTiles[i].Transform.anchoredPosition.x;

                if (_slideTiles[i].Transform.anchoredPosition.y < _tileMinPos.y)
                    _tileMinPos.y = _slideTiles[i].Transform.anchoredPosition.y;
            }

            StartCoroutine(OnBoardLoaded());
        }

        /// <summary>
        /// Animation to fade out the completed board and show the outlines of the board.
        /// </summary>
        /// <returns></returns>
        internal IEnumerator OnBoardLoaded()
        {
            if(LevelData.GameType == GameType.IMAGE) 
                yield return (StartCoroutine(Animations.Value(1f, 0f, 1.2f, (value) => _boardCompleted.GetComponent<Image>().color = new Color(_boardCompleted.GetComponent<Image>().color.r, 
                    _boardCompleted.GetComponent<Image>().color.g, _boardCompleted.GetComponent<Image>().color.b, value)))); 
            _boardCompleted.gameObject.SetActive(false);
            StartCoroutine(ScrambleTiles(_slideTiles[_slideTiles.Count - 1]));
            yield return null;
        }

        /// <summary>
        /// Function for when a tile is touched.
        /// </summary>
        /// <param name="tile">the tile that is touched</param>
        internal void TileInteract(SlideTile tile) => TileMoveDir = GetEmptyDirection(tile.GridCoords);

        /// <summary>
        /// Has the puzzle been completed?
        /// </summary>
        /// <returns>Bool for whether the puzzle has been done</returns>
        internal bool PuzzleCompleted()
        {
            if (TilesCorrect() == _slideTiles.Count)
            {
                SetTilesDraggable(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Clear all our tiles and their game objects.
        /// </summary>
        internal void ClearTiles()
        {
            StopAllCoroutines();

            // Don't try to clear if the board outline isn't active
            if (!_boardOutline.gameObject.activeSelf || _slideTiles == null)
                return;

            for (int i = _slideTiles.Count - 1; i >= 0; --i)
            {
                _slideTiles[i].Transform?.gameObject.SetActive(false);
                Destroy(_slideTiles[i].Transform?.gameObject);
            }
            _slideTiles.Clear();

            _boardOutline.gameObject.SetActive(false);
            _boardBackground.gameObject.SetActive(false);
        }

        /// <summary>
        /// Get the transform of a tile next to the empty tile if one exists.
        /// </summary>
        /// <param name="offset">the offset from the empty tile</param>
        /// <returns>returns the tile at the position and the offset</returns>
        internal SlideTile GetEmptyTileAndOffset(Vector2Int offset)
        {
            SlideTile emptyTile = _slideTiles[_slideTiles.Count - 1];
            return _tileGrid.GetValue(emptyTile.GridCoords + offset);
        }

        /// <summary>
        /// Completes the puzzle by showing the image without outlines.
        /// </summary>
        /// <param name="duration">Duration for it to complete</param>
        internal IEnumerator CompleteEmptyTile(float duration)
        {
            _boardCompleted.gameObject.SetActive(true);
            yield return (StartCoroutine(Animations.Value(0f, 1f, 1.2f, (value) => _boardCompleted.GetComponent<Image>().color = new Color(_boardCompleted.GetComponent<Image>().color.r,
                _boardCompleted.GetComponent<Image>().color.g, _boardCompleted.GetComponent<Image>().color.b, value))));
            yield return null;
        }

        /// <summary>
        /// Coroutine to animate the scrambling of tiles.
        /// </summary>
        /// <param name="emptyTile">the empty tile</param>
        private IEnumerator ScrambleTiles(SlideTile emptyTile)
        {
            _isScrambling = true;
            Vector2Int moveDir = Vector2Int.zero;
            Vector2Int lastDir = moveDir;
            SlideTile currentTile;

            for (int i = 0; i < LevelData.ScrambleCount; ++i)
            {
                moveDir = FindTileNeighbour(emptyTile.GridCoords, lastDir);
                currentTile = _tileGrid.GetValue(emptyTile.GridCoords + moveDir);
                currentTile.DragInput?.SetInitialPosition();
                MoveTile(currentTile, emptyTile);
                lastDir = moveDir;

                yield return new WaitForSeconds(_tileAnimSpeed + 0.02f);
            }

            onScrambleEnd?.Invoke();
            yield return new WaitForSeconds(1.2f);

            SetTilesDraggable(true);

            for (int j = 0; j < _slideTiles.Count; j++)
                _slideTiles[j].DragInput?.SetInitialPosition();

            yield return new WaitForSeconds(0.4f);
            _isScrambling = false;
        }

        /// <summary>
        /// Move a tile in the scene and inside the internal grid.
        /// </summary>
        /// <param name="tile">the tile that needs to slide</param>
        /// <param name="emptyTile">the empty tile</param>
        private void MoveTile(SlideTile tile, SlideTile emptyTile = null)
        {
            Vector2Int? dir;
            if (emptyTile == null)
            {
                dir = GetEmptyDirection(tile.GridCoords);
                if (dir.HasValue)
                    emptyTile = _tileGrid.GetValue(tile.GridCoords + dir.Value);
            }
            else
            {
                dir = emptyTile.GridCoords - tile.GridCoords;
            }

            if (!dir.HasValue || emptyTile == null)
            {
                tile.DragInput.SetRayCast(true);
                return;
            }

            _tileGrid.SwapValues(tile.GridCoords, tile.GridCoords + dir.Value);
            emptyTile.GridCoords -= dir.Value;
            tile.GridCoords += dir.Value;

            Vector2 clampedAnchoredPosition = tile.DragInput.InitialAnchoredPosition;
            clampedAnchoredPosition += new Vector2(dir.Value.x, dir.Value.y * -1) * _tileMoveAmount;

            // Makes sure that the tiles stay inside the board, by clamping.
            clampedAnchoredPosition.x = Mathf.Clamp(clampedAnchoredPosition.x, _tileMinPos.x, _tileMaxPos.x);
            clampedAnchoredPosition.y = Mathf.Clamp(clampedAnchoredPosition.y, _tileMinPos.y, _tileMaxPos.y);

            // Moves and animates the tile to the new position.
            StartCoroutine(TileMoveAnimation(tile, clampedAnchoredPosition));
        }

        /// <summary>
        /// Movement and animation for the tiles.
        /// </summary>
        /// <param name="tile">Tile to move</param>
        /// <param name="clampedAnchoredPosition">The new position for the tile</param>
        /// <returns></returns>
        private IEnumerator TileMoveAnimation(SlideTile tile, Vector2 clampedAnchoredPosition)
        {

            if (tile.Transform.GetComponent<RectTransform>().anchoredPosition.x != clampedAnchoredPosition.x)
            {
                yield return (StartCoroutine(Animations.Value(tile.Transform.GetComponent<RectTransform>().anchoredPosition.x, clampedAnchoredPosition.x, 0.08f,
                    (value) => SetNewTilePosition(tile, new Vector2(value, clampedAnchoredPosition.y)))));
            }
            else if (tile.Transform.GetComponent<RectTransform>().anchoredPosition.y != clampedAnchoredPosition.y)
            {
                yield return (StartCoroutine(Animations.Value(tile.Transform.GetComponent<RectTransform>().anchoredPosition.y, clampedAnchoredPosition.y, 0.08f,
                    (value) => SetNewTilePosition(tile, new Vector2(clampedAnchoredPosition.x, value)))));
            }

            tile.Transform.anchoredPosition = clampedAnchoredPosition;
            TileMoveEnd(tile);
            yield return null;
        }

        /// <summary>
        /// Sets the position for the chosen tile, used for the animation.
        /// </summary>
        /// <param name="tile">Tile to set new position</param>
        /// <param name="newAnchoredPosition">New Position</param>
        private void SetNewTilePosition(SlideTile tile, Vector2 newAnchoredPosition)
        {
            RectTransform tileRect = (RectTransform)tile.Transform;
            tileRect.anchoredPosition = newAnchoredPosition;
            tile.DragInput.InitialAnchoredPosition = newAnchoredPosition;
        }

        /// <summary>
        /// Move a tile in the scene back to where it started moving from.
        /// </summary>
        /// <param name="tile">The tile to move to origin</param>
        private void MoveTileToOrigin(SlideTile tile) => StartCoroutine(TileMoveAnimation(tile, tile.DragInput.InitialAnchoredPosition));

        /// <summary>
        /// Function called at the end of a tile moving.
        /// </summary>
        /// <param name="tile">The tile thats ending its move</param>
        private void TileMoveEnd(SlideTile tile)
        {
            tile.DragInput.SetRayCast(true);
            onTileMoved?.Invoke();
        }

        /// <summary>
        /// Get the direction of an empty tile if one exists next to the supplied co-ordinates.
        /// </summary>
        /// <param name="coords">Where to check for an empty tile neighbour in the grid</param>
        /// <returns>the empty tile's position if it is there </returns>
        private Vector2Int? GetEmptyDirection(Vector2Int coords)
        {
            Vector2Int[] checkCoords = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            SlideTile checkTile;

            for (int i = 0; i < 4; ++i)
            {
                checkTile = _tileGrid.GetValue(coords + checkCoords[i]);
                if (checkTile != null)
                    if (checkTile.Empty)
                        return checkCoords[i];
            }
            return null;
        }

        /// <summary>
        /// Get a random valid tile neighbour.
        /// </summary>
        /// <param name="coords">Where to check for neighbours</param>
        /// <param name="lastDir">The direction we last moved (for scrambling so we don't go back on ourselves)</param>
        /// <returns>the neighour tile</returns>
        private Vector2Int FindTileNeighbour(Vector2Int coords, Vector2Int lastDir)
        {

            List<Vector2Int> dirs = new List<Vector2Int> { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down, lastDir };
            dirs.Remove(lastDir * -1);
            Vector2Int checkCoord = dirs[_random.Next(0, dirs.Count)];
            while (_tileGrid.GetValue(coords + checkCoord) == null)
            {
                dirs.Remove(checkCoord);
                checkCoord = dirs[_random.Next(0, dirs.Count)];
            }
            return checkCoord;
        }

        /// <summary>
        /// How many tiles are in the correct position?
        /// </summary>
        /// <returns>Number of correctly placed tiles</returns>
        private int TilesCorrect()
        {
            int correct = 0;
            SlideTile[] tileArray = _tileGrid.GridArray;
            for (int i = 0; i < tileArray.Length; ++i)
            {
                if (tileArray[i].CorrectPosition == i)
                    ++correct;
            }
            return correct;
        }

        /// <summary>
        /// Resize the layout group grid to fit the screen and the supplied constraints.
        /// </summary>
        /// <param name="size">How many rows and columns on each side?</param>
        private void ResizeGrid(int size)
        {
            float gridSizeWidth = ((RectTransform)_tileLayoutGroup.transform).rect.width - _tileLayoutGroup.padding.left - _tileLayoutGroup.padding.right;
            float gridSizeHeight = ((RectTransform)_tileLayoutGroup.transform).rect.height - _tileLayoutGroup.padding.top - _tileLayoutGroup.padding.bottom;
            float cardSizeX = Mathf.FloorToInt(gridSizeWidth / size);
            float cardSizeY = Mathf.FloorToInt(gridSizeHeight / size);
            float smallestSize = Mathf.Min(cardSizeX, cardSizeY);

            _tileLayoutGroup.cellSize = new Vector2(smallestSize, smallestSize);
            _tileLayoutGroup.constraintCount = size;
        }

        /// <summary>
        /// Set whether tiles can be dragged/interacted with?
        /// </summary>
        /// <param name="canDrag">if it can drag yes or no</param>
        public void SetTilesDraggable(bool canDrag)
        {
            foreach (SlideTile tile in _slideTiles)
            {
                if (!tile.Empty)
                {
                    tile.DragInput.CanDrag = canDrag;
                    tile.DragInput.SetRayCast(canDrag);
                }
            }
        }

        public IEnumerator AutoArrangeTiles()
        {
            SetTilesDraggable(false);

            foreach (var tile in _slideTiles)
            {
                if (tile.Empty)
                    continue;

                Vector2 targetPos = tile.CorrectAnchoredPosition;

                yield return StartCoroutine(
                    Animations.Value(
                        0f,
                        1f,
                        _tileAnimSpeed,
                        value =>
                        {
                            tile.Transform.anchoredPosition =
                                Vector2.Lerp(
                                    tile.Transform.anchoredPosition,
                                    targetPos,
                                    value);
                        }));
                
                tile.Transform.anchoredPosition = targetPos;

                tile.DragInput.InitialAnchoredPosition = targetPos;

                tile.GridCoords = new Vector2Int(
                    tile.CorrectPosition % LevelData.Size,
                    tile.CorrectPosition / LevelData.Size
                );
            }

            RebuildGrid();
        }

        // Вспомогательный метод для обновления _tileGrid после ручной перестановки
        private void RebuildGrid()
        {
            int size = LevelData.Size;
            SlideTile[] gridArray = new SlideTile[size * size];
            foreach (var tile in _slideTiles)
            {
                int index = tile.GridCoords.y * size + tile.GridCoords.x;
                if (index >= 0 && index < gridArray.Length)
                    gridArray[index] = tile;
            }
            _tileGrid = new Grid<SlideTile>(size, size, gridArray);
        }
    }
}