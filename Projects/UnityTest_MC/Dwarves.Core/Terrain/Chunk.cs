// ----------------------------------------------------------------------------
// <copyright file="Chunk.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    /// <summary>
    /// A 2D chunk of voxels.
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// The power-of-2 size of the chunk for quickly determining chunk index.
        /// </summary>
        public const byte LogSizeX = 4;

        /// <summary>
        /// The power-of-2 size of the chunk for quickly determining chunk index.
        /// </summary>
        public const byte LogSizeY = 4;

        /// <summary>
        /// The width of a chunk.
        /// </summary>
        public const int Width = 1 << LogSizeX;

        /// <summary>
        /// The height of a chunk.
        /// </summary>
        public const int Height = 1 << LogSizeY;

        /// <summary>
        /// Mask for converting from world coordinates to chunk coordinates.
        /// </summary>
        public const int MaskX = Width - 1;

        /// <summary>
        /// Mask for converting from world coordinates to chunk coordinates.
        /// </summary>
        public const int MaskY = Height - 1;

        /// <summary>
        /// Initializes a new instance of the Chunk class.
        /// </summary>
        public Chunk()
        {
            this.Voxels = new ChunkVoxels();
            this.Mesh = new ChunkMesh();
        }

        /// <summary>
        /// Gets the voxels in this chunk.
        /// </summary>
        public ChunkVoxels Voxels { get; private set; }

        /// <summary>
        /// Gets the meshes in this chunk.
        /// </summary>
        public ChunkMesh Mesh { get; private set; }

        /// <summary>
        /// Gets or sets the chunk usage.
        /// </summary>
        public ChunkUsage Usage { get; set; }

        /// <summary>
        /// Get the voxel at the given chunk coordinates.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The voxel.</returns>
        public Voxel GetVoxel(int x, int y)
        {
            return this.Voxels[ChunkVoxels.GetIndex(x, y)];
        }

        /// <summary>
        /// Get the voxel at the given chunk coordinates.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The voxel.</returns>
        public Voxel GetVoxel(Position position)
        {
            return this.GetVoxel(position.X, position.Y);
        }
    }
}