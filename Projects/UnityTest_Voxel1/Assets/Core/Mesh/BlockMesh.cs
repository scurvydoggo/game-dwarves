/// <summary>
/// The mesh data for a terrain block.
/// </summary>
public class BlockMesh
{
    /// <summary>
    /// Initializes a new instance of the BlockMesh class.
    /// </summary>
    public BlockMesh()
    {
        this.Vertices = new Vector3[0];
        this.Indices = new int[0];
        this.Material = 0;
    }

    /// <summary>
    /// Gets or sets the vertices of this block.
    /// </summary>
    public Vector3[] Vertices { get; set; }

    /// <summary>
    /// Gets or sets the triangle indices array of this block.
    /// </summary>
    public int[] Indices { get; set; }

    /// <summary>
    /// Gets or sets the material of this block.
    /// </summary>
    public byte Material { get; set; }
}