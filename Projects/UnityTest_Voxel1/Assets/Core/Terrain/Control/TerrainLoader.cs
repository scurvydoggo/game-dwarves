/// <summary>
/// Responsible for loading and unloading chunks in the terrain.
/// </summary>
public class TerrainLoader
{
    /// <summary>
    /// Serializes and deserializes terrain chunks.
    /// </summary>
    private TerrainSerializer terrainSerializer;

    /// <summary>
    /// Dynamically generates terrain chunks.
    /// </summary>
    private TerrainGenerator terrainGenerator;

    /// <summary>
    /// Initializes a new instance of the TerrainLoader class.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    public TerrainLoader(Terrain terrain)
    {
        this.Terrain = terrain;
        this.terrainSerializer = new TerrainSerializer();
        this.terrainGenerator = new TerrainGenerator();
    }

    /// <summary>
    /// Gets the terrain.
    /// </summary>
    public Terrain Terrain { get; private set; }

    /// <summary>
    /// Load a chunk.
    /// </summary>
    /// <param name="chunkIndex">The chunk index.</param>
    public void LoadChunk(Vector2I chunkIndex)
    {
        // Deserialize or generate the chunk
        Chunk chunk;
        if (!this.terrainSerializer.TryDeserializeChunk(chunkIndex, out chunk))
        {
            // The chunk doesn't yet exist, so generate it
            chunk = this.terrainGenerator.GenerateChunk(chunkIndex, this.Terrain);

            // Serialize the generated chunk
            this.terrainSerializer.SerializeChunk(chunkIndex, chunk);
        }

        // TODO: Add the chunk to the Terrain object
    }

    /// <summary>
    /// Unload a chunk.
    /// </summary>
    /// <param name="chunkIndex">The chunk index.</param>
    public void UnloadChunk(Vector2I chunkIndex)
    {
        // Get the chunk
        Chunk chunk;
        if (!this.Terrain.Blocks.TryGetChunk(chunkIndex, out chunk))
        {
            // The chunk isn't loaded so do nothing
            return;
        }

        // Serialize the chunk
        this.terrainSerializer.SerializeChunk(chunkIndex, chunk);

        // TODO: Remove the chunk from the Terrain object
    }
}