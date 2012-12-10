// ----------------------------------------------------------------------------
// <copyright file="CachedCell.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;

    /// <summary>
    /// Reference to a cell whose vertices have been generated.
    /// </summary>
    public class CachedCell
    {
        /// <summary>
        /// Initialises a new instance of the CachedCell class.
        /// </summary>
        /// <param name="depth">The depth.</param>
        public CachedCell(int depth)
        {
            this.Indices = new int[depth, 4];
        }

        /// <summary>
        /// Gets the cached indices. 1st dimension indicates the z-depth; 2nd dimension indicates the vertex index of
        /// each of the sharable vertices.
        /// </summary>
        public int[,] Indices { get; private set; }
    }
}