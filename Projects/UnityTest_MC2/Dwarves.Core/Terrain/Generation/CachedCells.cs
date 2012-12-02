// ----------------------------------------------------------------------------
// <copyright file="CachedCells.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;

    /// <summary>
    /// Reference to the cells whose vertices have been generated.
    /// </summary>
    public class CachedCells
    {
        /// <summary>
        /// Initialises a new instance of the CachedCells class.
        /// </summary>
        public CachedCells()
        {
            this.Cells = new CachedCell[TerrainConst.ChunkWidth * TerrainConst.ChunkHeight];
        }

        /// <summary>
        /// Gets the cached cells.
        /// </summary>
        public CachedCell[] Cells { get; private set; }
    }
}