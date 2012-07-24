using System.Collections.Generic;

/// <summary>
/// Generates meshes for terrain blocks in which blocks are represented by simple cubes.
/// </summary>
public class CubedBlockMeshGenerator : BlockMeshGenerator
{
    /// <summary>
    /// Populates the mesh cloud with data for the given chunk.
    /// </summary>
    /// <param name="meshCloud">The mesh cloud to be populated.</param>
    /// <param name="chunkIndex">The index of the chunk from which meshes will be generated.</param>
    public override void GenerateBlockMeshes(BlockMeshCloud meshCloud, Vector2I chunkIndex)
    {
    }
}