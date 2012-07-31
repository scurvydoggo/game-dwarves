// ----------------------------------------------------------------------------
// <copyright file="TerrainBlockLoader.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// Responsible for loading and unloading terrain blocks.
/// </summary>
public class TerrainBlockLoader
{
    /// <summary>
    /// Serializes and deserializes terrain chunks.
    /// </summary>
    private TerrainBlockSerializer serializer;

    /// <summary>
    /// Dynamically generates terrain chunks.
    /// </summary>
    private TerrainBlockGenerator generator;

    /// <summary>
    /// Initializes a new instance of the TerrainBlockLoader class.
    /// </summary>
    public TerrainBlockLoader()
    {
        this.serializer = new TerrainBlockSerializer();
        this.generator = new TerrainBlockGenerator();
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
        if (!this.serializer.TryDeserializeChunk(chunkIndex, out chunk))
        {
            // The chunk doesn't yet exist, so generate it
            chunk = this.generator.GenerateChunk(terrain, chunkIndex);

            // Serialize the generated chunk
            this.serializer.SerializeChunk(chunk, chunkIndex);
        }

        // Add the chunk from the Terrain object
        terrain.Blocks.ActiveChunks.Add(chunkIndex, chunk);
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
        this.serializer.SerializeChunk(chunk, chunkIndex);

        // Remove the chunk from the Terrain object
        terrain.Blocks.ActiveChunks.Remove(chunkIndex);
    }
}