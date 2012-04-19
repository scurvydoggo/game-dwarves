// ----------------------------------------------------------------------------
// <copyright file="QuadTree.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Common
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A 2D position-based quadtree.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the quad tree.</typeparam>
    public class QuadTree<T>
    {
        /// <summary>
        /// Initializes a new instance of the QuadTree class.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public QuadTree(RectangleF rectangle)
        {
            this.Rectangle = rectangle;
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        public RectangleF Rectangle { get; private set; }

        /// <summary>
        /// Gets the top-left quadrant.
        /// </summary>
        public QuadTree<T> TopLeft { get; private set; }

        /// <summary>
        /// Gets the top-right quadrant.
        /// </summary>
        public QuadTree<T> TopRight { get; private set; }

        /// <summary>
        /// Gets the bottom-left quadrant.
        /// </summary>
        public QuadTree<T> BottomLeft { get; private set; }

        /// <summary>
        /// Gets the bottom-right quadrant.
        /// </summary>
        public QuadTree<T> BottomRight { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this node is a leaf (has no child quadrants).
        /// </summary>
        public bool IsLeaf
        {
            get { return this.TopLeft == null; }
        }

        /// <summary>
        /// Gets the number of leaves contained by the node.
        /// </summary>
        public int LeafCount
        {
            get
            {
                if (this.IsLeaf)
                {
                    return 1;
                }
                else
                {
                    return
                        this.TopLeft.LeafCount +
                        this.TopRight.LeafCount +
                        this.BottomLeft.LeafCount +
                        this.BottomRight.LeafCount;
                }
            }
        }

        /// <summary>
        /// Split the QuadTree rectangle into four even sub-quadrants.
        /// </summary>
        public void Split()
        {
            Vector2 topLeft = this.Rectangle.TopLeft;
            Vector2 center = this.Rectangle.Center;
            Vector2 bottomRight = this.Rectangle.BottomRight;

            this.TopLeft = new QuadTree<T>(
                new RectangleF(topLeft, center));
            this.TopRight = new QuadTree<T>(
                new RectangleF(new Vector2(center.X, bottomRight.Y), new Vector2(topLeft.X, center.Y)));
            this.BottomLeft = new QuadTree<T>(
                new RectangleF(new Vector2(bottomRight.X, center.Y), new Vector2(center.X, topLeft.Y)));
            this.BottomRight = new QuadTree<T>(
                new RectangleF(center, topLeft));
        }
    }
}