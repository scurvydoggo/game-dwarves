using System.Collections.Generic;

/// <summary>
/// Manages the terrain blocks.
/// </summary>
public class Terrain
{
    /// <summary>
    /// The currently active chunks, keyed by chunk index.
    /// </summary>
    private Dictionary<Vector2I, Chunk> activeChunks;

    /// <summary>
    /// Initializes a new instance of the TerrainChunks class.
    /// </summary>
    public Terrain()
    {
        this.activeChunks = new Dictionary<Vector2I, Chunk>();
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
            return this.activeChunks[Terrain.GetChunkIndex(worldX, worldY)]
                [worldX & Chunk.MaskX, worldY & Chunk.MaskY];
        }

        set
        {
            this.activeChunks[Terrain.GetChunkIndex(worldX, worldY)]
                [worldX & Chunk.MaskX, worldY & Chunk.MaskY] = value;
        }
    }

    /// <summary>
    /// Gets or sets the chunk at the given chunk index.
    /// </summary>
    /// <param name="chunkIndex">The chunk index.</param>
    /// <returns>The chunk.</returns>
    public Chunk this[Vector2I chunkIndex]
    {
        get
        {
            return this.activeChunks[chunkIndex];
        }

        set
        {
            this.activeChunks[chunkIndex] = value;
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

    /// <summary>
    /// Try to get the block at the given world position.
    /// </summary>
    /// <param name="worldX">The x position.</param>
    /// <param name="worldY">The y position.</param>
    /// <param name="block">The block.</param>
    /// <returns>True if the block was retrieved.</returns>
    public bool TryGetBlock(int worldX, int worldY, out Block block)
    {
        Chunk chunk;
        if (this.TryGetChunk(Terrain.GetChunkIndex(worldX, worldY), out chunk))
        {
            return chunk.TryGetBlock(worldX & Chunk.MaskX, worldY & Chunk.MaskY, out block);
        }
        else
        {
            block = null;
            return false;
        }
    }

    /// <summary>
    /// Try to get the chunk at the given chunk index.
    /// </summary>
    /// <param name="chunkIndex">The chunk index.</param>
    /// <param name="blchunkock">The chunk.</param>
    /// <returns>True if the chunk was retrieved.</returns>
    public bool TryGetChunk(Vector2I chunkIndex, out Chunk chunk)
    {
        return this.activeChunks.TryGetValue(chunkIndex, out chunk);
    }
}