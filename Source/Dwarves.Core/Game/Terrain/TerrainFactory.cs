// ----------------------------------------------------------------------------
// <copyright file="TerrainFactory.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain
{
    using System;
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
        /// <param name="bitmap">The bitmap defining the terrain.</param>
        /// <param name="currentTime">The current time used as the terrain creation time.</param>
        /// <returns>The terrain object.</returns>
        public ClipQuadTree<TerrainData> CreateTerrainQuadTree(Texture2D bitmap, TimeSpan currentTime)
        {
            // Create the quad tree
            var quadTree = new ClipQuadTree<TerrainData>(
                new Square(0, 0, this.GetUpperPowerOf2(bitmap.Height > bitmap.Width ? bitmap.Height : bitmap.Width)));

            // Read the bitmap data
            var bitmapData = new Color[bitmap.Width * bitmap.Height];
            bitmap.GetData<Color>(bitmapData);

            // Populate the terrain's quad tree from the bitmap data
            this.PopulateQuadTree(quadTree, bitmapData, bitmap.Width, currentTime);

            return quadTree;
        }

        #endregion

        #region Bitmap

        /// <summary>
        /// Populate the quad tree from the bit map data.
        /// </summary>
        /// <param name="quadTree">The quad tree to populate.</param>
        /// <param name="bitmapData">The bit map color data.</param>
        /// <param name="bitmapWidth">The width in pixels of the bitmap image.</param>
        /// <param name="currentTime">The current time used as the terrain creation time.</param>
        private void PopulateQuadTree(
            ClipQuadTree<TerrainData> quadTree,
            Color[] bitmapData,
            int bitmapWidth,
            TimeSpan currentTime)
        {
            // Create the quadrants
            TerrainType? terrainType =
                this.PopulateQuadrants(quadTree, quadTree.Bounds, bitmapData, bitmapWidth, currentTime);
            if (terrainType.HasValue)
            {
                TerrainData data = new TerrainData(terrainType.Value, currentTime);

                // The entire terrain quad tree is of a single terrain type, so set the root value with no leaves
                quadTree.SetData(data, quadTree.Bounds, null);
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
        /// <param name="currentTime">The current time used as the terrain creation time.</param>
        /// <returns>The terrain type value if all sub-quadrants in this bound are filled with the same terrain type;
        /// Null if the sub-quadrants have various terrain types.</returns>
        private TerrainType? PopulateQuadrants(
            ClipQuadTree<TerrainData> quadTree,
            Square bounds,
            Color[] bitmapData,
            int bitmapWidth,
            TimeSpan currentTime)
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
                    this.PopulateQuadrants(quadTree, topLeftBounds, bitmapData, bitmapWidth, currentTime);
                TerrainType? topRightTerrain =
                    this.PopulateQuadrants(quadTree, topRightBounds, bitmapData, bitmapWidth, currentTime);
                TerrainType? bottomLeftTerrain =
                    this.PopulateQuadrants(quadTree, bottomLeftBounds, bitmapData, bitmapWidth, currentTime);
                TerrainType? bottomRightTerrain =
                    this.PopulateQuadrants(quadTree, bottomRightBounds, bitmapData, bitmapWidth, currentTime);

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
                        quadTree.SetData(
                            new TerrainData(topLeftTerrain.Value, currentTime), topLeftBounds, null);
                    }

                    if (topRightTerrain.HasValue)
                    {
                        // Quadrant has a uniform color
                        quadTree.SetData(
                            new TerrainData(topRightTerrain.Value, currentTime), topRightBounds, null);
                    }

                    if (bottomLeftTerrain.HasValue)
                    {
                        // Quadrant has a uniform color
                        quadTree.SetData(
                            new TerrainData(bottomLeftTerrain.Value, currentTime), bottomLeftBounds, null);
                    }

                    if (bottomRightTerrain.HasValue)
                    {
                        // Quadrant has a uniform color
                        quadTree.SetData(
                            new TerrainData(bottomRightTerrain.Value, currentTime), bottomRightBounds, null);
                    }

                    // Return null since there isnt a single color shared between all the quadrants
                    return null;
                }
            }
            else
            {
                // Get the terrain type at this pixel
                if (bounds.X < bitmapWidth)
                {
                    int dataIndex = bounds.X + (bounds.Y * bitmapWidth);
                    if (dataIndex < bitmapData.Length)
                    {
                        return TerrainTypeConverter.GetValue(bitmapData[dataIndex]);
                    }
                    else
                    {
                        // The index is out of bounds for this quadrant. This will happen if the bitmap image is not a
                        // square with a power-of-2 length. Just return TerrainType.None
                        return TerrainType.None;
                    }
                }
                else
                {
                    // The index is out of bounds for this quadrant. This will happen if the bitmap image is not a
                    // square with a power-of-2 length. Just return TerrainType.None
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