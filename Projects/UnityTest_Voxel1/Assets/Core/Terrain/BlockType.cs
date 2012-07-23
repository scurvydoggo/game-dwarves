/// <summary>
/// Represents a terrain block type.
/// </summary>
public enum BlockType : byte
{
    /// <summary>
    /// The block has not been loaded.
    /// </summary>
    Unknown,

    /// <summary>
    /// The block contains nothing.
    /// </summary>
    Air,

    /// <summary>
    /// The block contains dirt.
    /// </summary>
    Dirt
}