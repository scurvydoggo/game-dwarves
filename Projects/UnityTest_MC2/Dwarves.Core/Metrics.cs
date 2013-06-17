// ----------------------------------------------------------------------------
// <copyright file="Metrics.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    using System;
    using Dwarves.Core.Math;

    /// <summary>
    /// The terrain metrics.
    /// </summary>
    public static class Metrics
    {
        /// <summary>
        /// Indicates whether the class has been initialised.
        /// </summary>
        private static bool isInitialised;

        /// <summary>
        /// The power-of-2 height of the chunk for quickly determining chunk index.
        /// </summary>
        private static int chunkWidthLog;

        /// <summary>
        /// The power-of-2 width of the chunk for quickly determining chunk index.
        /// </summary>
        private static int chunkHeightLog;

        /// <summary>
        /// Gets the chunk height.
        /// </summary>
        public static int ChunkWidth { get; private set; }

        /// <summary>
        /// Gets the chunk width.
        /// </summary>
        public static int ChunkHeight { get; private set; }

        /// <summary>
        /// Gets the chunk depth.
        /// </summary>
        public static int ChunkDepth { get; private set; }

        /// <summary>
        /// Gets the depth to which digging occurs.
        /// </summary>
        public static int DigDepth { get; private set; }

        /// <summary>
        /// Gets the distance from the mean surface height that the terrain oscillates.
        /// </summary>
        public static int SurfaceAmplitude { get; private set; }

        /// <summary>
        /// Initialise the Metrics class.
        /// </summary>
        /// <param name="chunkWidthLog">The power-of-2 width of the chunk.</param>
        /// <param name="chunkHeightLog">The power-of-2 height of the chunk.</param>
        /// <param name="chunkDepth">The depth of the chunk.</param>
        /// <param name="digDepth">The depth to which digging occurs.</param>
        /// <param name="surfaceAmplitude">The distance from the mean surface height that the terrain oscillates.
        /// </param>
        public static void Initialise(
            int chunkWidthLog,
            int chunkHeightLog,
            int chunkDepth,
            int digDepth,
            int surfaceAmplitude)
        {
            if (Metrics.isInitialised)
            {
                throw new InvalidOperationException("Terrain metrics have already been initialised.");
            }

            Metrics.chunkWidthLog = chunkWidthLog;
            Metrics.chunkHeightLog = chunkHeightLog;
            Metrics.ChunkWidth = 1 << chunkWidthLog;
            Metrics.ChunkHeight = 1 << chunkHeightLog;
            Metrics.ChunkDepth = chunkDepth;
            Metrics.DigDepth = digDepth;
            Metrics.SurfaceAmplitude = surfaceAmplitude;

            Metrics.isInitialised = true;
        }

        /// <summary>
        /// Get the index of the chunk at the given world coordinates.
        /// </summary>
        /// <param name="worldX">The x position.</param>
        /// <param name="worldY">The y position.</param>
        /// <returns>The chunk index.</returns>
        public static Vector2I ChunkIndex(int worldX, int worldY)
        {
            return new Vector2I(
                worldX >> Metrics.chunkWidthLog,
                worldY >> Metrics.chunkHeightLog);
        }

        /// <summary>
        /// Convert the world bounds into chunk bounds.
        /// </summary>
        /// <param name="worldBounds">The bounds.</param>
        /// <returns>The bounds in chunk coordinates</returns>
        public static RectangleI ChunkIndices(RectangleI worldBounds)
        {
            Vector2I top = Metrics.ChunkIndex(worldBounds.X, worldBounds.Y);
            Vector2I bottom = Metrics.ChunkIndex(worldBounds.Right - 1, worldBounds.Bottom - 1);
            return new RectangleI(
                top.X,
                top.Y,
                bottom.X - top.X + 1,
                top.Y - bottom.Y + 1);
        }

        /// <summary>
        /// Get the origin of the given chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The origin of the chunk in world coordinates.</returns>
        public static Vector2I GetChunkOrigin(Vector2I chunkIndex)
        {
            return new Vector2I(
                chunkIndex.X * Metrics.ChunkWidth,
                chunkIndex.Y * Metrics.ChunkHeight);
        }

        /// <summary>
        /// Convert the world coordinates into chunk coordinates.
        /// </summary>
        /// <param name="worldPos">The position.</param>
        /// <returns>The position in chunk coordinates.</returns>
        public static Vector2I WorldToChunk(Vector2I worldPos)
        {
            return new Vector2I(
                worldPos.X & (Metrics.ChunkWidth - 1),
                worldPos.Y & (Metrics.ChunkHeight - 1));
        }

        /// <summary>
        /// Convert the world coordinates into chunk coordinates.
        /// </summary>
        /// <param name="worldPos">The position.</param>
        /// <returns>The position in chunk coordinates.</returns>
        public static Vector3I WorldToChunk(Vector3I worldPos)
        {
            return new Vector3I(
                worldPos.X & (Metrics.ChunkWidth - 1),
                worldPos.Y & (Metrics.ChunkHeight - 1),
                worldPos.Z);
        }
    }
}