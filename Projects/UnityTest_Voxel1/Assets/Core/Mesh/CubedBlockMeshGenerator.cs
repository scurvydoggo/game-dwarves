/// <summary>
/// Generates meshes for terrain blocks in which blocks are represented by simple cubes.
/// </summary>
public class CubedBlockMeshGenerator : BlockMeshGenerator
{
    /// <summary>
    /// Initializes a new instance of the CubedBlockMeshGenerator class.
    /// </summary>
    /// <param name="terrain">The terrain object from which the meshes are generated.</param>
    public CubedBlockMeshGenerator(Terrain terrain)
        : base(terrain)
    {
    }

    /// <summary>
    /// Create a mesh for the given block.
    /// </summary>
    /// <param name="block">The block to create the mesh for.</param>
    /// <param name="blockUp">The block above.</param>
    /// <param name="blockRight">The block to the right.</param>
    /// <param name="blockDown">The block below.</param>
    /// <param name="blockLeft">The block to the left.</param>
    /// <returns></returns>
    protected override BlockMesh CreateBlockMesh(
        Block block,
        Block blockUp,
        Block blockRight,
        Block blockDown,
        Block blockLeft)
    {
        return null;
    }
}