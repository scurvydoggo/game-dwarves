// ----------------------------------------------------------------------------
// <copyright file="ChunkVoxelGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Generation
{
    /// <summary>
    /// Generates the voxels for a chunk.
    /// </summary>
    public class ChunkVoxelGenerator
    {
        /// <summary>
        /// Initializes a new instance of the ChunkVoxelGenerator class.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        public ChunkVoxelGenerator(float seed)
        {
            this.Seed = seed;
        }

        /// <summary>
        /// Gets or sets the seed value.
        /// </summary>
        public float Seed { get; set; }
    }
}
