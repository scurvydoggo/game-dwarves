// ----------------------------------------------------------------------------
// <copyright file="QuadTreeDataSplitter.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Common
{
    /// <summary>
    /// Splits a quad tree node's data value into four values corresponding to four equally sized sub-quadrants.
    /// </summary>
    /// <typeparam name="T">The type of data to split.</typeparam>
    public abstract class QuadTreeDataSplitter<T>
    {
        /// <summary>
        /// Split the <typeparamref name="T"/> data into four values corresponding to four equally sized sub-quadrants
        /// of the given square.
        /// </summary>
        /// <param name="data">The data to split.</param>
        /// <param name="bounds">The bounds of the data.</param>
        /// <returns>The split values.</returns>
        public abstract Result Split(T data, Square bounds);

        /// <summary>
        /// The result of the split.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// Gets the data for the top left quadrant.
            /// </summary>
            public T TopLeft { get; private set; }

            /// <summary>
            /// Gets the data for the top right quadrant.
            /// </summary>
            public T TopRight { get; private set; }

            /// <summary>
            /// Gets the data for the bottom left quadrant.
            /// </summary>
            public T BottomLeft { get; private set; }

            /// <summary>
            /// Gets the data for the bottom right quadrant.
            /// </summary>
            public T BottomRight { get; private set; }
        }
    }
}