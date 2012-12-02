// ----------------------------------------------------------------------------
// <copyright file="TerrainConst.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using Dwarves.Core.Math;

    /// <summary>
    /// Terrain related constants.
    /// </summary>
    public static class TerrainConst
    {
        /// <summary>
        /// The power-of-2 width of the chunk for quickly determining chunk index.
        /// </summary>
        public const byte ChunkWidthLog = 4;

        /// <summary>
        /// The power-of-2 height of the chunk for quickly determining chunk index.
        /// </summary>
        public const byte ChunkHeightLog = 4;

        /// <summary>
        /// The width of a chunk.
        /// </summary>
        public const int ChunkWidth = 1 << TerrainConst.ChunkWidthLog;

        /// <summary>
        /// The height of a chunk.
        /// </summary>
        public const int ChunkHeight = 1 << TerrainConst.ChunkHeightLog;

        /// <summary>
        /// The minimum density value. This lies inside the surface.
        /// </summary>
        public const byte DensityMin = 0x00;

        /// <summary>
        /// The maximum density value. This lies outside the surface.
        /// </summary>
        public const byte DensityMax = 0x0E;

        /// <summary>
        /// The density at which the surface lies (aka the isolevel).
        /// </summary>
        public const byte DensitySurface = 0x07;

        /// <summary>
        /// The number of cubes dug in the Z direction for voxels.
        /// </summary>
        public const byte DigDepth = 4;

        /// <summary>
        /// The number of cubes drawn in the Z direction for voxels.
        /// </summary>
        public const byte DrawDepth = 8;

        /// <summary>
        /// Get the index of the chunk at the given world coordinates.
        /// </summary>
        /// <param name="worldX">The x position.</param>
        /// <param name="worldY">The y position.</param>
        /// <returns>The chunk index.</returns>
        public static Vector2I ChunkIndex(int worldX, int worldY)
        {
            return new Vector2I(worldX >> TerrainConst.ChunkWidthLog, worldY >> TerrainConst.ChunkHeightLog);
        }

        /// <summary>
        /// Gets the index for the voxel at the given chunk coordinates.
        /// </summary>
        /// <param name="chunkPos">The position.</param>
        /// <returns>The index.</returns>
        public static int VoxelIndex(Vector2I chunkPos)
        {
            return TerrainConst.VoxelIndex(chunkPos.X, chunkPos.Y);
        }

        /// <summary>
        /// Gets the index for the voxel at the given chunk coordinates.
        /// </summary>
        /// <param name="chunkX">The x position.</param>
        /// <param name="chunkY">The y position.</param>
        /// <returns>The index.</returns>
        public static int VoxelIndex(int chunkX, int chunkY)
        {
            return chunkX + (chunkY * TerrainConst.ChunkWidth);
        }

        /// <summary>
        /// Convert the world coordinates into chunk coordinates.
        /// </summary>
        /// <param name="worldX">The x position.</param>
        /// <param name="worldY">The y position.</param>
        /// <returns>The position in chunk coordinates.</returns>
        public static Vector2I WorldToChunk(int worldX, int worldY)
        {
            const int MaskX = TerrainConst.ChunkWidth - 1;
            const int MaskY = TerrainConst.ChunkHeight - 1;
            return new Vector2I(worldX & MaskX, worldY & MaskY);
        }

        /// <summary>
        /// Get the origin of the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>The origin of the chunk in world coordinates.</returns>
        public static Vector2I GetChunkOrigin(Vector2I chunk)
        {
            return new Vector2I(chunk.X * TerrainConst.ChunkWidth, chunk.Y * TerrainConst.ChunkHeight);
        }
    }
}