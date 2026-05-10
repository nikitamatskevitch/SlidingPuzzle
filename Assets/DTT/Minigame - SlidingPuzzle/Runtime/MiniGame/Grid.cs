using System.Runtime.CompilerServices;
using UnityEngine;

namespace DTT.MiniGame.SlidingPuzzle
{
	///<summary>
	/// A grid representation of a data array
	///</summary>
	public class Grid<T>
    {
        /// <summary>
        /// The width of the grid.
        /// </summary>
        public int Width  => _maxColumns;

        /// <summary>
        /// The height of the grid.
        /// </summary>
        public int Height => _maxRows;

        /// <summary>
        /// Get for the Grid Array.
        /// </summary>
        public T[] GridArray => _gridArray;

        /// <summary>
        /// Grid array.
        /// </summary>
        private T[] _gridArray;

        /// <summary>
        /// The maximum amount of columns.
        /// </summary>
        private int _maxColumns;

        /// <summary>
        /// The max amound of rows.
        /// </summary>
        private int _maxRows;

        /// <summary>
        /// Creates a gridarray.
        /// </summary>
        /// <param name="width"> the width</param>
        /// <param name="height">the height</param>
        public Grid(int width, int height)
        {
            if(width < 0)
                throw new System.Exception("the grid cant have a negative width");
            
            if (height < 0)
                throw new System.Exception("the grid cant have a negative height");

            _gridArray = new T[width * height];
            _maxRows = height;
            _maxColumns = width;
        }

        /// <summary>
        /// Creates a gridarray.
        /// </summary>
        /// <param name="width">the width</param>
        /// <param name="height">the height</param>
        /// <param name="dataArray">the elements for in the array</param>
        public Grid(int width, int height, T[] dataArray) : this(width, height) => PopulateGrid(dataArray);
        
        /// <summary>
        /// Adds the elements in the array.
        /// </summary>
        /// <param name="dataArray">the elements wanted to be added</param>
        public void PopulateGrid(T[] dataArray) => dataArray.CopyTo(_gridArray, 0);

        /// <summary>
        /// Gets the value at a specific value.
        /// </summary>
        /// <param name="coords">the coords of the value you want to get</param>
        /// <returns></returns>
        public T GetValue(Vector2Int coords) => GetValue(coords.x, coords.y);

        /// <summary>
        /// Gets the value of specific point.
        /// </summary>
        /// <param name="x">the x pos</param>
        /// <param name="y">the y pos</param>
        /// <returns></returns>
        public T GetValue(int x, int y)
        {
            if (Invalid(x, y))
                return default(T);

            return _gridArray[Index(x, y)];
        }

        /// <summary>
        /// Swaps the two values.
        /// </summary>
        /// <param name="coords1">the first coordinate</param>
        /// <param name="coords2">the second coordinate</param>
        public void SwapValues(Vector2Int coords1, Vector2Int coords2) => SwapValues(coords1.x, coords1.y, coords2.x, coords2.y);

        /// <summary>
        /// Swaps the values.
        /// </summary>
        /// <param name="x1">x first point</param>
        /// <param name="y1">y first point</param>
        /// <param name="x2">x second point</param>
        /// <param name="y2">y second point</param>
        public void SwapValues(int x1, int y1, int x2, int y2)
        {
            T tempValue;
            tempValue = GetValue(x1, y1);

            if (tempValue == null)
                Debug.LogError("Wrong tile coordinate values");

            SetValue(x1, y1, GetValue(x2, y2));

            if (GetValue(x2, y2) == null)
                Debug.LogError("Wrong tile coordinate values");

            SetValue(x2, y2, tempValue);
        }

        /// <summary>
        /// Sets the values.
        /// </summary>
        /// <param name="x">the x pos</param>
        /// <param name="y">the y pos</param>
        /// <param name="newValue">the value</param>
        private void SetValue(int x, int y, T newValue)
        {
            if (Invalid(x, y))
                return;

            _gridArray[Index(x, y)] = newValue;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="x">the x pos</param>
        /// <param name="y">the y pos</param>
        /// <returns>the index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Index(int x, int y) => (Width * y) + x;

        /// <summary>
        /// Checks if the pos is invalid.
        /// </summary>
        /// <param name="x">the x pos</param>
        /// <param name="y">the y pos</param>
        /// <returns>if its a valid location</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Invalid(int x, int y) => (x < 0 || x >= Width) || (y < 0 || y >= Height);
    }
}