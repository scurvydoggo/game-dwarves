// ----------------------------------------------------------------------------
// <copyright file="LightFrontDecorator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Light
{
    using System.Collections.Generic;
    using Dwarves.Common;
    using Dwarves.Game.Terrain;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Responsible for decorating terrain quad trees with light fronts. This essentially means finding the edges of the
    /// terrain where the daylight 'bleeds' in to partially illuminate the terrain. A 'light front' is a line from which
    /// light fades from fully-lit to complete darkness.
    /// </summary>
    public class LightFrontDecorator
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the LightFrontDecorator class.
        /// </summary>
        /// <param name="quadTree">The terrain quad tree.</param>
        public LightFrontDecorator(ClipQuadTree<TerrainData> quadTree)
        {
            this.QuadTree = quadTree;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the terrain quad tree.
        /// </summary>
        public ClipQuadTree<TerrainData> QuadTree { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Scan through the quad tree and update the StaticLightFronts property of each terrain block.
        /// </summary>
        public void Decorate()
        {
            // Add all ground nodes
            foreach (ClipQuadTree<TerrainData> terrainNode in this.QuadTree)
            {
                // Ignore non-terrain
                if (terrainNode.Data.State == TerrainState.Empty)
                {
                    continue;
                }

                // Get the list of light fronts
                LightFront[] lightFronts = this.GetNodeLightFronts(terrainNode);

                // Update the node
                terrainNode.Data.StaticLightFronts = lightFronts;
            }
        }

        #endregion

        #region Decorate Terrain Block

        /// <summary>
        /// Get the light fronts for the given terrain node.
        /// </summary>
        /// <param name="terrainNode">The terrain node.</param>
        /// <returns>Array of light fronts.</returns>
        private LightFront[] GetNodeLightFronts(ClipQuadTree<TerrainData> terrainNode)
        {
            var lightFronts = new List<LightFront>();

            // For convenience, get the corrdinates of the outside points for this node
            int above = terrainNode.Bounds.Y - 1;
            int below = terrainNode.Bounds.Bottom;
            int left = terrainNode.Bounds.X - 1;
            int right = terrainNode.Bounds.Right;

            // Test each point 1 pixel outside of this quad on the top/bottom sides
            int startAboveX = -1;
            int startBelowX = -1;
            for (int x = terrainNode.Bounds.X; x < terrainNode.Bounds.Right; x++)
            {
                if (this.IsEmptyTerrain(x, above))
                {
                    if (startAboveX == -1)
                    {
                        startAboveX = x;
                    }
                }
                else
                {
                    if (startAboveX != -1)
                    {
                        var lightFront = new LightFront(
                            startAboveX,
                            above,
                            LightDirection.Down,
                            x - startAboveX - 1);
                        lightFronts.Add(lightFront);
                        startAboveX = -1;
                    }
                }

                if (this.IsEmptyTerrain(x, below))
                {
                    if (startBelowX == -1)
                    {
                        startBelowX = x;
                    }
                }
                else
                {
                    if (startBelowX != -1)
                    {
                        var lightFront = new LightFront(
                            startBelowX,
                            below,
                            LightDirection.Up,
                            x - startBelowX - 1);
                        lightFronts.Add(lightFront);
                        startBelowX = -1;
                    }
                }
            }

            // Add uncompleted fronts
            if (startAboveX != -1)
            {
                var lightFront = new LightFront(
                    startAboveX,
                    above,
                    LightDirection.Down,
                    terrainNode.Bounds.Right - startAboveX - 1);
                lightFronts.Add(lightFront);
            }

            // Add uncompleted fronts
            if (startBelowX != -1)
            {
                var lightFront = new LightFront(
                    startBelowX,
                    below,
                    LightDirection.Up,
                    terrainNode.Bounds.Right - startBelowX - 1);
                lightFronts.Add(lightFront);
            }

            // Test each point 1 pixel outside of this quad on the left/right sides
            int startLeftY = -1;
            int startRightY = -1;
            for (int y = terrainNode.Bounds.Y; y < terrainNode.Bounds.Bottom; y++)
            {
                if (this.IsEmptyTerrain(left, y))
                {
                    if (startLeftY == -1)
                    {
                        startLeftY = y;
                    }
                }
                else
                {
                    if (startLeftY != -1)
                    {
                        var lightFront = new LightFront(
                            left,
                            startLeftY,
                            LightDirection.Right,
                            y - startLeftY - 1);
                        lightFronts.Add(lightFront);
                        startLeftY = -1;
                    }
                }

                if (this.IsEmptyTerrain(right, y))
                {
                    if (startRightY == -1)
                    {
                        startRightY = y;
                    }
                }
                else
                {
                    if (startRightY != -1)
                    {
                        var lightFront = new LightFront(
                            right,
                            startRightY,
                            LightDirection.Left,
                            y - startRightY - 1);
                        lightFronts.Add(lightFront);
                        startRightY = -1;
                    }
                }
            }

            // Add uncompleted fronts
            if (startLeftY != -1)
            {
                var lightFront = new LightFront(
                    left,
                    startLeftY,
                    LightDirection.Right,
                    terrainNode.Bounds.Bottom - startLeftY - 1);
                lightFronts.Add(lightFront);
            }

            // Add uncompleted fronts
            if (startRightY != -1)
            {
                var lightFront = new LightFront(
                    right,
                    startRightY,
                    LightDirection.Left,
                    terrainNode.Bounds.Bottom - startRightY - 1);
                lightFronts.Add(lightFront);
            }

            return lightFronts.ToArray();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns a value indicating whether the given point is empty terrain.
        /// </summary>
        /// <param name="x">The x coordinate of the point to test.</param>
        /// <param name="y">The y coordinate of the point to test.</param>
        /// <returns>True if the point is empty.</returns>
        private bool IsEmptyTerrain(int x, int y)
        {
            TerrainData terrainData;
            if (this.QuadTree.GetDataAt(new Point(x, y), out terrainData))
            {
                return terrainData.State == TerrainState.Empty;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}