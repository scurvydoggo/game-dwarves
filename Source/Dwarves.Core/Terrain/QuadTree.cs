// ----------------------------------------------------------------------------
// <copyright file="QuadTree.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Terrain
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A 2D quadtree which splits a rectangle.
    /// </summary>
    public class QuadTree
    {
        /// <summary>
        /// Initializes a new instance of the QuadTree class.
        /// </summary>
        /// <param name="min">The top-left position.</param>
        /// <param name="max">The bottom-right position.</param>
        public QuadTree(Vector2 min, Vector2 max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Gets the top-left position.
        /// </summary>
        public Vector2 Min { get; private set; }

        /// <summary>
        /// Gets the bottom-right position.
        /// </summary>
        public Vector2 Max { get; private set; }

        /// <summary>
        /// Gets the top-left terrain quadrant.
        /// </summary>
        public QuadTree TopLeft { get; private set; }

        /// <summary>
        /// Gets the top-right terrain quadrant.
        /// </summary>
        public QuadTree TopRight { get; private set; }

        /// <summary>
        /// Gets the bottom-left terrain quadrant.
        /// </summary>
        public QuadTree BottomLeft { get; private set; }

        /// <summary>
        /// Gets the bottom-right terrain quadrant.
        /// </summary>
        public QuadTree BottomRight { get; private set; }

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
            Vector2 center = this.Min + ((this.Max - this.Min) / 2);

            this.TopLeft = new QuadTree(this.Min, center);
            this.TopRight = new QuadTree(new Vector2(center.X, this.Min.Y), new Vector2(this.Max.X, center.Y));
            this.BottomLeft = new QuadTree(new Vector2(this.Min.X, center.Y), new Vector2(center.X, this.Max.Y));
            this.BottomRight = new QuadTree(center, this.Max);
        }
    }
}