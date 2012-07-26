/// <summary>
/// Responsible for dynamically generating terrain chunks.
/// </summary>
public class TerrainGenerator
{
    /// <summary>
    /// Initializes a new instance of the TerrainGenerator class.
    /// </summary>
    public TerrainGenerator()
    {
    }

    /// <summary>
    /// Initializes a new instance of the TerrainGenerator class.
    /// </summary>
    /// <param name="chunkIndex">The chunk index.</param>
    /// <param name="terrain">The terrain object to provide neighbouring chunk context.</param>
    /// <returns>The chunk.</returns>
    public Chunk GenerateChunk(Vector2I chunkIndex, Terrain terrain)
    {
        Chunk chunk = new Chunk();

        // TODO: Implement Perlin Noise algorithm for generating terrain blocks. For now, just hardcode some junk
        for (int index = Chunk.Navigation.Start; index <= Chunk.Navigation.End; index++)
        {
            if (index % 3 != 0)
            {
                chunk[index] = new Block(BlockType.Dirt);
            }
            else
            {
                chunk[index] = new Block(BlockType.None);
            }
        }

        return chunk;
    }
}