// ----------------------------------------------------------------------------
// <copyright file="Chunk.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// Represents a 2D region of terrain blocks.
/// </summary>
public class Chunk
{
    #region Constants

    /// <summary>
    /// The power-of-2 size of the chunk for quickly determining chunk index.
    /// </summary>
    public const byte LogSizeX = 5;

    /// <summary>
    /// The power-of-2 size of the chunk for quickly determining chunk index.
    /// </summary>
    public const byte LogSizeY = 5;

    /// <summary>
    /// The block width of a chunk.
    /// </summary>
    public const int SizeX = 1 << LogSizeX;

    /// <summary>
    /// The block height of a chunk.
    /// </summary>
    public const int SizeY = 1 << LogSizeY;

    /// <summary>
    /// Mask for performing bitwise modulus operations.
    /// </summary>
    public const int MaskX = SizeX - 1;

    /// <summary>
    /// Mask for performing bitwise modulus operations.
    /// </summary>
    public const int MaskY = SizeY - 1;

    /// <summary>
    /// Mask for performing bitwise modulus operations.
    /// </summary>
    public const int MaskXNot = ~MaskX;

    /// <summary>
    /// Mask for performing bitwise modulus operations.
    /// </summary>
    public const int MaskYNot = ~MaskY;

    #endregion

    /// <summary>
    /// Initializes a new instance of the Chunk class.
    /// </summary>
    public Chunk()
    {
        this.Blocks = new Block[SizeX * SizeY];
		this.Usage = ChunkUsage.Blocks;
    }

    /// <summary>
    /// Gets the blocks in this chunk. This is a flattened 2D array.
    /// </summary>
    public Block[] Blocks { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating how the chunk is to be used.
    /// </summary>
	public ChunkUsage Usage { get; set; }
	
    /// <summary>
    /// Gets or sets the block at the given block index.
    /// </summary>
    /// <param name="blockIndex">The block index.</param>
    /// <returns>The block.</returns>
    public Block this[int blockIndex]
    {
        get
        {
            return this.Blocks[blockIndex];
        }

        set
        {
            this.Blocks[blockIndex] = value;
        }
    }

    /// <summary>
    /// Gets or sets the block at the given position in chunk-space.
    /// </summary>
    /// <param name="chunkX">The x position.</param>
    /// <param name="chunkY">The y position.</param>
    /// <returns>The block.</returns>
    public Block this[int chunkX, int chunkY]
    {
        get
        {
            return this.Blocks[Chunk.GetBlockIndex(chunkX, chunkY)];
        }

        set
        {
            this.Blocks[Chunk.GetBlockIndex(chunkX, chunkY)] = value;
        }
    }

    /// <summary>
    /// Gets the array index for the block at the given x and y position.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <returns>The index.</returns>
    public static int GetBlockIndex(int x, int y)
    {
        return x + (y * SizeX);
    }

    /// <summary>
    /// Try to get the block at the given position in chunk-space.
    /// </summary>
    /// <param name="chunkX">The x position.</param>
    /// <param name="chunkY">The y position.</param>
    /// <param name="block">The block.</param>
    /// <returns>True if the block was retrieved.</returns>
    public bool TryGetBlock(int chunkX, int chunkY, out Block block)
    {
        int blockIndex = Chunk.GetBlockIndex(chunkX, chunkY);
        if (blockIndex >= Navigation.Start && blockIndex <= Navigation.End)
        {
            block = this.Blocks[blockIndex];
            return true;
        }
        else
        {
            block = Block.None;
            return false;
        }
    }

    /// <summary>
    /// Provides constants for navigating the chunk's block array.
    /// </summary>
    public static class Navigation
    {
        /// <summary>
        /// Move the index to the block above.
        /// </summary>
        public const short Up = -Chunk.SizeX;

        /// <summary>
        /// Move the index to the block below.
        /// </summary>
        public const short Down = Chunk.SizeX;

        /// <summary>
        /// Move the index to the previous block (to the left or end of prev row).
        /// </summary>
        public const short Prev = -1;

        /// <summary>
        /// Move the index to the next block (to the right or start of next row).
        /// </summary>
        public const short Next = 1;

        /// <summary>
        /// The start index.
        /// </summary>
        public const short Start = 0;

        /// <summary>
        /// The end index (inclusive).
        /// </summary>
        public const short End = (Chunk.SizeX * Chunk.SizeY) - 1;

        /// <summary>
        /// The start of the last row.
        /// </summary>
        public const short LastRow = (Chunk.SizeX * Chunk.SizeY) - Chunk.SizeX;
    }
}