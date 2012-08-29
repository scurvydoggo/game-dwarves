// ----------------------------------------------------------------------------
// <copyright file="Terrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the terrain.
    /// </summary>
    public class Terrain
    {
        /// <summary>
        /// The currently active chunks.
        /// </summary>
        private Dictionary<Position, Chunk> chunks;

        /// <summary>
        /// Initializes a new instance of the Terrain class.
        /// </summary>
        public Terrain()
        {
            this.chunks = new Dictionary<Position, Chunk>();
        }

        /// <summary>
        /// Get the index of the chunk at the given world coordinates.
        /// </summary>
        /// <param name="worldX">The x position.</param>
        /// <param name="worldY">The y position.</param>
        /// <returns>The chunk index.</returns>
        public static Position GetChunkIndex(int worldX, int worldY)
        {
            return new Position(worldX >> Chunk.LogSizeX, worldY >> Chunk.LogSizeY);
        }

        /// <summary>
        /// Get the chunk at the given chunk index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk.</returns>
        public Chunk GetChunk(Position chunkIndex)
        {
            return this.chunks[chunkIndex];
        }

        /// <summary>
        /// Try to get the chunk at the given chunk index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>True if the chunk was retrieved.</returns>
        public bool TryGetChunk(Position chunkIndex, out Chunk chunk)
        {
            return this.chunks.TryGetValue(chunkIndex, out chunk);
        }
    }
}