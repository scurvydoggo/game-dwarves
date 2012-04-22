// ----------------------------------------------------------------------------
// <copyright file="QuadTreeData.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Common
{
    /// <summary>
    /// Represents location-bound data in the quad tree.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the quad tree.</typeparam>
    public class QuadTreeData<T>
    {
        /// <summary>
        /// Initializes a new instance of the QuadTreeData class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="bounds">The bounds of the data.</param>
        public QuadTreeData(T data, Square bounds)
        {
            this.Data = data;
            this.Bounds = bounds;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Gets the bounds of the data.
        /// </summary>
        public Square Bounds { get; private set; }
    }
}
