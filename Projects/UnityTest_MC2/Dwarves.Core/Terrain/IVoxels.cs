// ----------------------------------------------------------------------------
// <copyright file="IVoxels.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using Dwarves.Core.Math;

    /// <summary>
    /// The terrain voxels.
    /// </summary>
    public interface IVoxels
    {
        /// <summary>
        /// Gets or sets a value indicating whether the mesh requires a rebuild.
        /// </summary>
        bool RebuildRequired { get; set; }

        /// <summary>
        /// Gets or sets the voxel at the given position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <returns>The voxel.</returns>
        Voxel this[int x, int y, int z] { get; set; }

        /// <summary>
        /// Gets or sets the voxel at the given position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>The voxel.</returns>
        Voxel this[Vector3I pos] { get; set; }
    }
}