/// <summary>
/// Represents the terrain.
/// </summary>
public class Terrain
{
    /// <summary>
    /// Initializes a new instance of the Terrain class.
    /// </summary>
    public Terrain()
    {
        this.BlockData = new TerrainBlocks();
        this.MeshData = new TerrainMesh();
    }

    /// <summary>
    /// Gets the terrain block data.
    /// </summary>
    public TerrainBlocks BlockData { get; private set; }

    /// <summary>
    /// Gets the terrain mesh data.
    /// </summary>
    public TerrainMesh MeshData { get; private set; }
}