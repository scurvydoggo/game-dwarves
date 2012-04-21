// ----------------------------------------------------------------------------
// <copyright file="QuadTreeItem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Common
{
    /// <summary>
    /// An item that can be inserted into a quadtree.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the quad tree.</typeparam>
    public class QuadTreeItem<T>
    {
        /// <summary>
        /// Initializes a new instance of the QuadTreeItem class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="bounds">The bounds of the item.</param>
        public QuadTreeItem(T value, RectangleF bounds)
        {
            this.Value = value;
            this.Bounds = bounds;
        }

        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets the rectangle of the item.
        /// </summary>
        public RectangleF Bounds { get; private set; }
    }
}