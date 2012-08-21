// ----------------------------------------------------------------------------
// <copyright file="VoxelChunk.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    /// <summary>
    /// A 2D chunk of voxels.
    /// </summary>
    public class VoxelChunk
    {
        #region Constants

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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the VoxelChunk class.
        /// </summary>
        public VoxelChunk()
        {
            this.Voxels = new Voxel[Width * Height];
        }

        /// <summary>
        /// Gets the voxels in this chunk.
        /// </summary>
        public Voxel[] Voxels { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the array index for the voxel at the given chunk coordinates.
        /// </summary>
        /// <param name="chunkX">The x position.</param>
        /// <param name="chunkY">The y position.</param>
        /// <returns>The index.</returns>
        public static int GetVoxelIndex(int chunkX, int chunkY)
        {
            return chunkX + (chunkY * Width);
        }

        #endregion
    }
}