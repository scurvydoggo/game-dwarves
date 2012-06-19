// ----------------------------------------------------------------------------
// <copyright file="TerrainData.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain
{
    using System;
    using Dwarves.Game.Light;

    /// <summary>
    /// Data for a terrain block.
    /// </summary>
    public class TerrainData
    {
        /// <summary>
        /// Initializes a new instance of the TerrainData class.
        /// </summary>
        /// <param name="material">The terrain material.</param>
        /// <param name="createTime">The creation time of the terrain.</param>
        public TerrainData(TerrainMaterial material, TimeSpan createTime)
        {
            this.Material = material;
            this.State = material == TerrainMaterial.None ? TerrainState.Empty : TerrainState.Terrain;
            this.CreateTime = createTime;
            this.StaticLightFronts = new LightFront[0];
        }

        /// <summary>
        /// Gets or sets the terrain material.
        /// </summary>
        public TerrainMaterial Material { get; set; }

        /// <summary>
        /// Gets or sets the terrain state.
        /// </summary>
        public TerrainState State { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the terrain.
        /// </summary>
        public TimeSpan CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the static (terrain-based) light fronts at this terrain block.
        /// </summary>
        public LightFront[] StaticLightFronts { get; set; }
    }
}