// ----------------------------------------------------------------------------
// <copyright file="VoxelTerrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the terrain.
    /// </summary>
    public class VoxelTerrain
    {
        /// <summary>
        /// Initialises a new instance of the VoxelTerrain class.
        /// </summary>
        public VoxelTerrain()
        {
            this.Chunks = new Dictionary<Position, Chunk>();
        }

        /// <summary>
        /// Gets the currently active chunks.
        /// </summary>
        public Dictionary<Position, Chunk> Chunks { get; private set; }

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
        /// Convert the world coordinates into chunk coordinates.
        /// </summary>
        /// <param name="worldPos">The position in world coordinates.</param>
        /// <returns>The position in chunk coordinates.</returns>
        public static Position GetChunkCoordinates(Position worldPos)
        {
            return new Position(worldPos.X & Chunk.Width, worldPos.Y & Chunk.Height);
        }

        /// <summary>
        /// Convert the chunk coordinates into world coordinates.
        /// </summary>
        /// <param name="chunkPos">The position in chunk coordinates.</param>
        /// <param name="chunkIndex">The index of the chunk in which the position lies.</param>
        /// <returns>The position in world coordinates.</returns>
        public static Position GetWorldCoordinates(Position chunkPos, Position chunkIndex)
        {
            return new Position((chunkIndex.X * Chunk.Width) + chunkPos.X, (chunkIndex.Y * Chunk.Height) + chunkPos.Y);
        }
    }
}