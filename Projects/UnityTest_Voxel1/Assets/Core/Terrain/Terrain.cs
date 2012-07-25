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
        this.Blocks = new TerrainBlocks();
        this.Mesh = new TerrainMesh();
    }

    /// <summary>
    /// Gets the terrain block data.
    /// </summary>
    public TerrainBlocks Blocks { get; private set; }

    /// <summary>
    /// Gets the terrain mesh data.
    /// </summary>
    public TerrainMesh Mesh { get; private set; }
}