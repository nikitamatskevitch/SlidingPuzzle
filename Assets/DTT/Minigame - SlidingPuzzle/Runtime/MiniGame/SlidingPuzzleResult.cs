using System.Text;
using UnityEngine;

namespace DTT.MiniGame.SlidingPuzzle
{
    /// <summary>
    /// Holds the result data of a sliding puzzle level.
    /// </summary>
    public struct SlidingPuzzleResult
    {
        /// <summary>
        /// The final score of a game.
        /// </summary>
        public float score;

        /// <summary>
        /// The amount of times the user slid a tile.
        /// </summary>
        public int SlideAmount { get; private set; }

        /// <summary>
        /// The amount of time it took to complete this level.
        /// </summary>
        public float TimeTaken { get; private set; }

        /// <summary>
        /// Sets the result data.
        /// </summary>
        /// <param name="slideAmount">Amount of tiles slid.</param>
        /// <param name="timeTaken">Amount of time the level took.</param>
        public SlidingPuzzleResult(int slideAmount, float timeTaken)
        {
            SlideAmount = slideAmount;
            TimeTaken = timeTaken;

            float score = 1f - (0.1f * timeTaken / 1000f);
            this.score = Mathf.Clamp(score, 0, 1);
        }

        /// <summary>
        /// Returns result info in string format for debugging.
        /// </summary>
        /// <returns>Result in string format</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Time taken (s): ");
            sb.Append(TimeTaken);
            sb.Append('\t');
            sb.Append("Amount of slid tiles: ");
            sb.Append(SlideAmount);

            return sb.ToString();
        }
    }
}