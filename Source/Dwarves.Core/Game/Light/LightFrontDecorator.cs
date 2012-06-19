// ----------------------------------------------------------------------------
// <copyright file="LightFrontDecorator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Light
{
    using System;
    using Dwarves.Common;
    using Dwarves.Game.Terrain;

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
            throw new NotImplementedException();
        }

        #endregion
    }
}
