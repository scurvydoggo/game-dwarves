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
        /// <param name="type">The terrain type.</param>
        /// <param name="createTime">The creation time of the terrain.</param>
        public TerrainData(TerrainType type, TimeSpan createTime)
            : this()
        {
            this.Type = type;
            this.CreateTime = createTime;
        }

        /// <summary>
        /// Gets or sets the terrain type.
        /// </summary>
        public TerrainType Type { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the terrain.
        /// </summary>
        public TimeSpan CreateTime { get; set; }
    }
}
