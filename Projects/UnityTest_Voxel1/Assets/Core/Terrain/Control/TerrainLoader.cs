// ----------------------------------------------------------------------------
// <copyright file="TerrainLoader.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

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
    public TerrainLoader()
    {
        this.terrainSerializer = new TerrainSerializer();
        this.terrainGenerator = new TerrainGenerator();
    }

    /// <summary>
    /// Load a chunk into the terrain object.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <param name="chunkIndex">The chunk index.</param>
    public void LoadChunk(Terrain terrain, Vector2I chunkIndex)
    {
        // Deserialize or generate the chunk
        Chunk chunk;
        if (!this.terrainSerializer.TryDeserializeChunk(chunkIndex, out chunk))
        {
            // The chunk doesn't yet exist, so generate it
            chunk = this.terrainGenerator.GenerateChunk(terrain, chunkIndex);

            // Serialize the generated chunk
            this.terrainSerializer.SerializeChunk(chunk, chunkIndex);
        }

        // TODO: Add the chunk to the Terrain object
    }

    /// <summary>
    /// Unload a chunk from the terrain object.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <param name="chunkIndex">The chunk index.</param>
    public void UnloadChunk(Terrain terrain, Vector2I chunkIndex)
    {
        // Get the chunk
        Chunk chunk;
        if (!terrain.Blocks.TryGetChunk(chunkIndex, out chunk))
        {
            // The chunk isn't loaded so do nothing
            return;
        }

        // Serialize the chunk
        this.terrainSerializer.SerializeChunk(chunk, chunkIndex);

        // TODO: Remove the chunk from the Terrain object
    }
}