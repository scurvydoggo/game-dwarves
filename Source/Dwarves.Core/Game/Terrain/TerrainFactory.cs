// ----------------------------------------------------------------------------
// <copyright file="TerrainFactory.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain
{
    using Dwarves.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Creates terrain objects.
    /// </summary>
    public class TerrainFactory
    {
        #region Factory Methods

        /// <summary>
        /// Create a terrain object from the given terrain bitmap.
        /// </summary>
        /// <param name="x">The top left X position in world coordinates.</param>
        /// <param name="y">The top-left Y position in world coordinates.</param>
        /// <param name="scale">The ratio for scaling quad tree coordinates to world coordinates.</param>
        /// <param name="bitmap">The bitmap defining the terrain.</param>
        /// <returns>The terrain object.</returns>
        public Terrain CreateTerrain(float x, float y, float scale, Texture2D bitmap)
        {
            // Create the quad tree
            var bounds =
                new Square(0, 0, this.GetUpperPowerOf2(bitmap.Height > bitmap.Width ? bitmap.Height : bitmap.Width));
            var quadTree = new ClipQuadTree<TerrainType>(bounds);

            // Read the bitmap data
            var bitmapData = new Color[bitmap.Width * bitmap.Height];
            bitmap.GetData<Color>(bitmapData);

            // Populate the quad tree from the bitmap data
            this.PopulateQuadTree(quadTree, bitmapData, bitmap.Width);

            return null;
        }

        #endregion

        #region Bitmap

        /// <summary>
        /// Populate the quad tree from the bit map data.
        /// </summary>
        /// <param name="quadTree">The quad tree to populate.</param>
        /// <param name="bitmapData">The bit map color data.</param>
        /// <param name="bitmapWidth">The width in pixels of the bitmap image.</param>
        private void PopulateQuadTree(ClipQuadTree<TerrainType> quadTree, Color[] bitmapData, int bitmapWidth)
        {
            // Create the quadrants
            TerrainType? terrainType = this.PopulateQuadrants(quadTree, quadTree.Bounds, bitmapData, bitmapWidth);
            if (terrainType.HasValue)
            {
                // The entire terrain quad tree is of a single terrain type, so set the root value with no leaves
                quadTree.SetData(terrainType.Value, quadTree.Bounds, null);
            }
        }

        /// <summary>
        /// Populate the quad tree quadrants in the given bounds from the bit map data.
        /// <para />
        /// This function will be recursively called the on the four sub-quadrants of this quadrant. If all of the sub-
        /// quandrants share the same terrain type, then no data is added to the quad tree and the type is returned. The
        /// data will be set at a higher level in the quad tree hierarchy. The reason for this is because if the parent
        /// quadrant is also fully uniform, then any quadrants below it (this one) would have be removed, wasting
        /// processor time.
        /// <para />
        /// If any of the sub-quadrants are of a fully uniform type (yet not *all* sub-quadrants like in the above case)
        /// then the data for those sub-quadrants is set in the quad tree here, making those sub-quadrants leaf nodes
        /// (an example of this could be Q1 and Q2 returning TerrainType.None, Q3 returning TerrainType.Mud, and Q4
        /// returning null since it is a 'mixed' quadrant and the data is set at a lower level. In this situation Q1, Q2
        /// and Q3 would have their data set at this level).
        /// </summary>
        /// <param name="quadTree">The quad tree to populate.</param>
        /// <param name="bounds">The bounds of the quadrant whose sub-quandrants are being populated.</param>
        /// <param name="bitmapData">The bit map color data.</param>
        /// <param name="bitmapWidth">The width in pixels of the bitmap image.</param>
        /// <returns>The terrain type value if all sub-quadrants in this bound are filled with the same terrain type;
        /// Null if the sub-quadrants have various terrain types.</returns>
        private TerrainType? PopulateQuadrants(
            ClipQuadTree<TerrainType> quadTree,
            Square bounds,
            Color[] bitmapData,
            int bitmapWidth)
        {
            if (bounds.Length > 1)
            {
                // Get the sub-quadrant bounds of this quadrant
                Square topLeftBounds = bounds.GetTopLeftQuadrant();
                Square topRightBounds = bounds.GetTopRightQuadrant();
                Square bottomLeftBounds = bounds.GetBottomLeftQuadrant();
                Square bottomRightBounds = bounds.GetBottomRightQuadrant();

                // Populate the sub quadrants or if they are uniform get the single terrain type they contain
                TerrainType? topLeftTerrain =
                    this.PopulateQuadrants(quadTree, topLeftBounds, bitmapData, bitmapWidth);
                TerrainType? topRightTerrain =
                    this.PopulateQuadrants(quadTree, topRightBounds, bitmapData, bitmapWidth);
                TerrainType? bottomLeftTerrain =
                    this.PopulateQuadrants(quadTree, bottomLeftBounds, bitmapData, bitmapWidth);
                TerrainType? bottomRightTerrain =
                    this.PopulateQuadrants(quadTree, bottomRightBounds, bitmapData, bitmapWidth);

                // Check if all of the quadrants are uniform and all have the same terrain type
                if (topLeftTerrain.HasValue &&
                    topLeftTerrain == topRightTerrain &&
                    topRightTerrain == bottomLeftTerrain &&
                    bottomLeftTerrain == bottomRightTerrain)
                {
                    // All quadrants have the same terrain type. Return this to create the quadrant at a higher level
                    return topLeftTerrain;
                }
                else
                {
                    // The quadrants do not all share the same terrain type. However, they may themselves be uniform
                    // in which case their quadrant values need to be set in the quad tree
                    if (topLeftTerrain.HasValue)
                    {
                        // Quadrant has a uniform color
                        quadTree.SetData(topLeftTerrain.Value, topLeftBounds, null);
                    }

                    if (topRightTerrain.HasValue)
                    {
                        // Quadrant has a uniform color
                        quadTree.SetData(topRightTerrain.Value, topRightBounds, null);
                    }

                    if (bottomLeftTerrain.HasValue)
                    {
                        // Quadrant has a uniform color
                        quadTree.SetData(bottomLeftTerrain.Value, bottomLeftBounds, null);
                    }

                    if (bottomRightTerrain.HasValue)
                    {
                        // Quadrant has a uniform color
                        quadTree.SetData(bottomRightTerrain.Value, bottomRightBounds, null);
                    }

                    // Return null since there isnt a single color shared between all the quadrants
                    return null;
                }
            }
            else
            {
                // Get the terrain type at this pixel
                int dataIndex = bounds.X + (bounds.Y * bitmapWidth);
                if (dataIndex < bitmapData.Length)
                {
                    return TerrainTypeConverter.GetValue(bitmapData[dataIndex]);
                }
                else
                {
                    // The index is out of bounds for this quadrant. This will happen if the bitmap image is not a
                    // square with a power-of-2 length. Just return TerrainType.bitmapData[]None
                    return TerrainType.None;
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get the closest power of 2 at or above the given number.
        /// </summary>
        /// <param name="x">The number.</param>
        /// <returns>The closest power of 2 at or above the given number.</returns>
        private int GetUpperPowerOf2(int x)
        {
            if (x < 0)
            {
                return 0;
            }

            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x++;

            return x;
        }

        #endregion
    }
}