using System.Collections.Generic;

/// <summary>
/// Generates meshes for terrain blocks.
/// </summary>
public abstract class BlockMeshGenerator
{
    /// <summary>
    /// Initializes a new instance of the BlockMeshGenerator class.
    /// </summary>
    /// <param name="terrain">The terrain object from which the meshes are generated.</param>
    public BlockMeshGenerator(Terrain terrain)
    {
        this.Terrain = terrain;
    }

    /// <summary>
    /// Gets the terrain object from which the meshes are generated.
    /// </summary>
    public Terrain Terrain { get; private set; }

    /// <summary>
    /// Generates mesh data for the given chunk.
    /// </summary>
    /// <param name="meshCloud">The mesh cloud to be populated.</param>
    /// <param name="chunkIndex">The index of the chunk from which meshes will be generated.</param>
    public abstract void GenerateBlockMeshes(BlockMeshCloud meshCloud, Vector2I chunkIndex);
}