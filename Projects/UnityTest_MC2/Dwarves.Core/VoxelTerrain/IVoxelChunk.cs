// ----------------------------------------------------------------------------
// <copyright file="IVoxelChunk.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain
{
    /// <summary>
    /// A chunk of voxels.
    /// </summary>
    public interface IVoxelChunk
    {
        /// <summary>
        /// Gets or sets the voxel at the given position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <returns>The voxel.</returns>
        Voxel this[int x, int y, int z] { get; set; }
    }
}