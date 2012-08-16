// ----------------------------------------------------------------------------
// <copyright file="Block.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// Represents a terrain block.
/// </summary>
public struct Block
{
    /// <summary>
    /// Bitmask for determining if a BlockType can be dug.
    /// </summary>
    public const int MaskDiggable = 0x01;

    /// <summary>
    /// Bitmask to apply to a BlockType to make it dug.
    /// </summary>
    public const int MaskDig = ~MaskDiggable;

    /// <summary>
    /// A block containing no terrain.
    /// </summary>
    public static readonly Block None = new Block(BlockType.None);

    /// <summary>
    /// Initializes a new instance of the Block struct.
    /// </summary>
    /// <param name="blockType">The block type.</param>
    public Block(BlockType blockType)
        : this()
    {
        this.BlockType = blockType;
    }

    /// <summary>
    /// Gets or sets the block type.
    /// </summary>
    public BlockType BlockType { get; set; }
}