using UnityEngine;

namespace DTT.MiniGame.SlidingPuzzle
{
    /// <summary>
    /// Holds data for a level.
    /// </summary>
    [CreateAssetMenu(fileName = "Level_data_template", menuName = "DTT/Mini Game/Sliding Puzzle/LeveData")]
    public class LevelData : ScriptableObject
    {
        /// <summary>
        /// Type of the game.
        /// </summary>
        [SerializeField]
        [Tooltip(" type of your game")]
        private GameType _gameType;
        
        /// <summary>
        /// Image used to generate the puzzle tiles.
        /// </summary>
        [SerializeField]
        [Tooltip("Image used to generate the puzzle tiles")]
        private Texture2D _tilesTexture;

        /// <summary>
        /// The size of the board for the current level, 3 should be the minimum value.
        /// </summary>
        [SerializeField]
        [Tooltip("The size of the board for the current level, 3 should be the minimum value")]
        [Min(3)]
        private int _size = 3;

        /// <summary>
        /// Number of times the tiles get scrambled.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of times the tiles get scrambled")]
        private int _scrambleCount = 1;

        /// <summary>
        /// Size of the board.
        /// </summary>
        public int Size => _size;

        /// <summary>
        /// Multiplier for the number of times the board is scrambled (number of tiles * ScrambleMultiplier).
        /// </summary>
        public int ScrambleCount => _scrambleCount;

        /// <summary>
        /// <see cref="GameType"/>
        /// </summary>
        public GameType GameType => _gameType;

        /// <summary>
        /// Turns the Texture2D of current level into a Sprite.
        /// </summary>
        /// <returns>The created Sprite of the level</returns>
        public Sprite GetCompletedImage() => Sprite.Create(_tilesTexture, new Rect(0, 0, _tilesTexture.width, _tilesTexture.height), Vector2.one * 0.5f);

        /// <summary>
        /// Get an array of sprites that split the internal texture into the required number of sprites.
        /// </summary>
        /// <returns>array of sprites</returns>
        public Sprite[] GetTileSprites()
        {
            int requiredNumber = _size * _size;
            Sprite[] sprites = new Sprite[requiredNumber];
            float tileSize, tileY, tileX = 0f;

            // Get the number of rows/columns.
            float gridSize = Mathf.Sqrt(requiredNumber);

            // If the texture is not perfectly square.
            if (_tilesTexture.width != _tilesTexture.height)
                Debug.LogWarning("Tile texture is not square, using shortest side");


            // Use the shortest side.
            tileSize = Mathf.Floor(Mathf.Min(_tilesTexture.width, _tilesTexture.height) / gridSize);

            // Y coordinates are inversed so larger number starts at the bottom.
            tileY = tileSize * (gridSize - 1);

            // Split texture into sprites
            for (int i = 0; i < requiredNumber; ++i)
            {
                // If reached end of the texture, go back to the left hand side.
                if (Mathf.Approximately(tileX, tileSize * gridSize))
                {
                    tileX = 0f;
                    tileY -= tileSize;
                }
                sprites[i] = Sprite.Create(_tilesTexture, new Rect(tileX, tileY, tileSize, tileSize), new Vector2(0.5f, 0.5f));
                tileX += tileSize;
            }
            return sprites;
        }
    }
}
