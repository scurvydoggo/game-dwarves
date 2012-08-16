// ----------------------------------------------------------------------------
// <copyright file="BlockType.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// Represents a terrain block type.
/// </summary>
public enum BlockType
{
    /// <summary>
    /// Empty block.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// A block type.
    /// </summary>
    DirtDug = 0x02,

    /// <summary>
    /// A block type.
    /// </summary>
    Dirt = 0x03,

    /// <summary>
    /// A block type.
    /// </summary>
    StoneDug = 0x04,

    /// <summary>
    /// A block type.
    /// </summary>
    Stone = 0x05,

    /// <summary>
    /// A block type.
    /// </summary>
    ClayDug = 0x06,

    /// <summary>
    /// A block type.
    /// </summary>
    Clay = 0x07
}