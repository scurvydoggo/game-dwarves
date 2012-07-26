using UnityEngine;

/// <summary>
/// Generates meshes for terrain blocks in which blocks are represented by simple cubes.
/// </summary>
public class TerrainMeshGeneratorCubed : TerrainMeshGenerator
{
    /// <summary>
    /// Create a mesh for the given block.
    /// </summary>
    /// <param name="block">The block to create the mesh for.</param>
    /// <param name="position">The position of the block.</param>
    /// <param name="blockUp">The block above.</param>
    /// <param name="blockRight">The block to the right.</param>
    /// <param name="blockDown">The block below.</param>
    /// <param name="blockLeft">The block to the left.</param>
    /// <returns>The mesh data for the block.</returns>
    protected override BlockMesh CreateBlockMesh(
        Block block,
        Vector2I position,
        Block blockUp,
        Block blockRight,
        Block blockDown,
        Block blockLeft)
    {
        MaterialType material;
        Vector3[] vertices;
        int[] indices;

        // TODO
        material = 0;
        vertices = new Vector3[0];
        indices = new int[0];

        return new BlockMesh(material, vertices, indices);
    }
}