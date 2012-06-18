// ----------------------------------------------------------------------------
// <copyright file="TerrainData.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain
{
    using System;

    /// <summary>
    /// Data for a terrain block.
    /// </summary>
    public struct TerrainData
    {
        /// <summary>
        /// Initializes a new instance of the TerrainData struct.
        /// </summary>
        /// <param name="material">The terrain material.</param>
        /// <param name="createTime">The creation time of the terrain.</param>
        public TerrainData(TerrainMaterial material, TimeSpan createTime)
            : this()
        {
            this.Material = material;
            this.CreateTime = createTime;

            // Initialize the state as either Terrain or NoTerrain
            this.State =
                material == TerrainMaterial.None ? TerrainState.Empty : TerrainState.Terrain;
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
    }
}