// ----------------------------------------------------------------------------
// <copyright file="StandardVoxels.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Engine
{
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// The standard structure for the terrain voxels.
    /// </summary>
    public class StandardVoxels : IVoxels
    {
        /// <summary>
        /// Initialises a new instance of the StandardVoxels class.
        /// </summary>
        /// <param name="chunkWidth">The chunk width.</param>
        /// <param name="chunkHeight">The chunk height.</param>
        /// <param name="chunkDepth">The chunk depth.</param>
        /// <param name="scale">The scaling ratio.</param>
        public StandardVoxels(int chunkWidth, int chunkHeight, int chunkDepth, int scale)
        {
            this.Chunks = new Dictionary<Vector2I, IVoxelChunk>();
            this.ChunkWidth = chunkWidth;
            this.ChunkHeight = chunkHeight;
            this.ChunkDepth = chunkDepth;
            this.Scale = scale;
        }

        /// <summary>
        /// Gets the voxel chunks.
        /// </summary>
        public Dictionary<Vector2I, IVoxelChunk> Chunks { get; private set;  }

        /// <summary>
        /// Gets the chunk width.
        /// </summary>
        public int ChunkWidth { get; private set; }

        /// <summary>
        /// Gets the chunk height.
        /// </summary>
        public int ChunkHeight { get; private set; }

        /// <summary>
        /// Gets the chunk depth.
        /// </summary>
        public int ChunkDepth { get; private set; }

        /// <summary>
        /// Gets the scaling ratio for voxel coordinates to world coordinates (essentially the Level of Detail).
        /// </summary>
        public int Scale { get; private set; }
    }
}