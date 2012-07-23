using System.Collections.Generic;

/// <summary>
/// Manages the terrain blocks.
/// </summary>
public class Terrain
{
    /// <summary>
    /// Gets the currently active chunks, keyed by chunk index.
    /// </summary>
    public Dictionary<Vector2I, Chunk> ActiveChunks { get; private set; }

    /// <summary>
    /// Initializes a new instance of the TerrainChunks class.
    /// </summary>
    public Terrain()
    {
        this.ActiveChunks = new Dictionary<Vector2I, Chunk>();
    }

    /// <summary>
    /// Gets the block at the given world position.
    /// </summary>
    /// <param name="worldX">The x position.</param>
    /// <param name="worldY">The y position.</param>
    /// <returns>The block.</returns>
    public Block this[int worldX, int worldY]
    {
        get
        {
            return this.ActiveChunks[Terrain.GetChunkIndex(worldX, worldY)]
                [worldX & Chunk.MaskX, worldY & Chunk.MaskY];
        }

        set
        {
            this.ActiveChunks[Terrain.GetChunkIndex(worldX, worldY)]
                [worldX & Chunk.MaskX, worldY & Chunk.MaskY] = value;
        }
    }

    /// <summary>
    /// Get the index of the chunk for the given world coordinates.
    /// </summary>
    /// <param name="worldX">The x position.</param>
    /// <param name="worldY">The y position.</param>
    /// <returns>The chunk index.</returns>
    public static Vector2I GetChunkIndex(int worldX, int worldY)
    {
        return new Vector2I(worldX >> Chunk.LogSizeX, worldY >> Chunk.LogSizeY);
    }
}