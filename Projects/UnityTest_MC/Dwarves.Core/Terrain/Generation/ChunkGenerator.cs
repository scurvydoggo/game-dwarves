// ----------------------------------------------------------------------------
// <copyright file="ChunkGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Generates new chunks.
    /// </summary>
    public class ChunkGenerator
    {
        /// <summary>
        /// Generates chunk voxels.
        /// </summary>
        private VoxelGenerator voxelGenerator;

        /// <summary>
        /// Initializes a new instance of the ChunkGenerator class.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        public ChunkGenerator(float seed)
        {
            this.Seed = seed;
            this.voxelGenerator = new VoxelGenerator(seed);
        }
        
        /// <summary>
        /// Gets the seed value.
        /// </summary>
        public float Seed { get; private set; }
        
        /// <summary>
        /// Generate a new chunk at the given chunk index and add it to the terrain.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The generated chunk.</returns>
        public Chunk Generate(VoxelTerrain terrain, Position chunkIndex)
        {
            var chunk = new Chunk();

            // Generate the voxels
            this.voxelGenerator.Generate(chunk.Voxels, chunkIndex);

            return chunk;
        }
    }
}
