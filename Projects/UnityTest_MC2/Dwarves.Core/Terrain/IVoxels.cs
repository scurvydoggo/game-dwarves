// ----------------------------------------------------------------------------
// <copyright file="IVoxels.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    /// <summary>
    /// Represents a chunk of voxels.
    /// </summary>
    public interface IVoxels
    {
        /// <summary>
        /// Gets or sets the voxel at the given coordinate. Voxel.Air is returned for coordinates outside of the bounds
        /// of the chunk.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <returns>The voxel</returns>
        Voxel this[int x, int y, int z] { get; set; }
    }
}