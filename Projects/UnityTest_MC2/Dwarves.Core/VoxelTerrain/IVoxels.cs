// ----------------------------------------------------------------------------
// <copyright file="IVoxels.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain
{
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// The terrain voxels.
    /// </summary>
    public interface IVoxels
    {
        /// <summary>
        /// Gets the voxel chunks.
        /// </summary>
        Dictionary<Vector2I, IVoxelChunk> Chunks { get; }

        /// <summary>
        /// Gets the chunk width.
        /// </summary>
        int ChunkWidth { get; }

        /// <summary>
        /// Gets the chunk height.
        /// </summary>
        int ChunkHeight { get; }

        /// <summary>
        /// Gets the chunk depth.
        /// </summary>
        int ChunkDepth { get; }

        /// <summary>
        /// Gets the scaling ratio for voxel coordinates to world coordinates (essentially the Level of Detail).
        /// </summary>
        int Scale { get; }
    }
}