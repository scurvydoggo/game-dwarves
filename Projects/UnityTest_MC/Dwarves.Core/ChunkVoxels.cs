// ----------------------------------------------------------------------------
// <copyright file="ChunkVoxels.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    /// <summary>
    /// The voxels in a chunk.
    /// </summary>
    public class ChunkVoxels
    {
        /// <summary>
        /// The voxels.
        /// </summary>
        private Voxel[] voxels;

        /// <summary>
        /// Initializes a new instance of the ChunkVoxels class.
        /// </summary>
        public ChunkVoxels()
        {
            this.voxels = new Voxel[Chunk.Width * Chunk.Height];
        }

        /// <summary>
        /// Gets or sets the voxel at the given index.
        /// </summary>
        /// <param name="index">The voxel index.</param>
        /// <returns>The voxel.</returns>
        public Voxel this[int index]
        {
            get
            {
                return this.voxels[index];
            }

            set
            {
                this.voxels[index] = value;
            }
        }

        /// <summary>
        /// Gets the index for the voxel at the given chunk coordinates.
        /// </summary>
        /// <param name="chunkX">The x position.</param>
        /// <param name="chunkY">The y position.</param>
        /// <returns>The index.</returns>
        public static int GetIndex(int chunkX, int chunkY)
        {
            return chunkX + (chunkY * Chunk.Width);
        }
    }
}