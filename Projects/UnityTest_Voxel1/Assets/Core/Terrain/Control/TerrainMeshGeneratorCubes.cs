// ----------------------------------------------------------------------------
// <copyright file="TerrainMeshGeneratorCubes.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Generates meshes for terrain blocks in which blocks are represented by simple cubes.
/// </summary>
public class TerrainMeshGeneratorCubes : TerrainMeshGenerator
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

        // Look up the base material for this block
        material = this.MaterialLookup.GetMaterial(block.BlockType);

        // Determine how many quads will exist for this block (ie. how many block faces)
        int quadCount = 1; // Front face
        if (blockUp.BlockType == BlockType.None)
        {
            quadCount += this.BlockDepth;
        }

        if (blockRight.BlockType == BlockType.None)
        {
            quadCount += this.BlockDepth;
        }

        if (blockDown.BlockType == BlockType.None)
        {
            quadCount += this.BlockDepth;
        }

        if (blockLeft.BlockType == BlockType.None)
        {
            quadCount += this.BlockDepth;
        }

        // Initialise the mesh arrays
        vertices = new Vector3[quadCount * 4];
        indices = new int[quadCount * 6];

        // Populate the quads
        this.AddFaceQuad(position, 0, vertices, indices);

        if (quadCount > 1)
        {
            int quadIndex = 1;
            if (blockUp.BlockType == BlockType.None)
            {
                this.AddUpQuad(position, quadIndex++, vertices, indices);
            }

            if (blockRight.BlockType == BlockType.None)
            {
                this.AddRightQuad(position, quadIndex++, vertices, indices);
            }

            if (blockDown.BlockType == BlockType.None)
            {
                this.AddDownQuad(position, quadIndex++, vertices, indices);
            }

            if (blockLeft.BlockType == BlockType.None)
            {
                this.AddLeftQuad(position, quadIndex++, vertices, indices);
            }
        }

        return new BlockMesh(material, vertices, indices);
    }

    /// <summary>
    /// Add a quad to the mesh arrays.
    /// </summary>
    /// <param name="basePos">The base position of the block.</param>
    /// <param name="quadIndex">The index of the current quad.</param>
    /// <param name="vertices">The vertice array.</param>
    /// <param name="indices">The indices array.</param>
    private void AddFaceQuad(Vector2I basePos, int quadIndex, Vector3[] vertices, int[] indices)
    {
        // Add the vertices
        int vertIndex = quadIndex * 4;
        vertices[vertIndex] = new Vector3(basePos.X, basePos.Y, 0);
        vertices[vertIndex + 1] = new Vector3(basePos.X + 1, basePos.Y, 0);
        vertices[vertIndex + 2] = new Vector3(basePos.X + 1, basePos.Y - 1, 0);
        vertices[vertIndex + 3] = new Vector3(basePos.X, basePos.Y - 1, 0);

        // Add the indices
        int indiceIndex = quadIndex * 6;
        indices[indiceIndex++] = vertIndex;
        indices[indiceIndex++] = vertIndex + 1;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 3;
        indices[indiceIndex++] = vertIndex;
    }

    /// <summary>
    /// Add a quad to the mesh arrays.
    /// </summary>
    /// <param name="basePos">The base position of the block.</param>
    /// <param name="quadIndex">The index of the current quad.</param>
    /// <param name="vertices">The vertice array.</param>
    /// <param name="indices">The indices array.</param>
    private void AddUpQuad(Vector2I basePos, int quadIndex, Vector3[] vertices, int[] indices)
    {
        // Add the vetices
        int vertIndex = quadIndex * 4;
        vertices[vertIndex] = new Vector3(basePos.X, basePos.Y, 0);
        vertices[vertIndex + 1] = new Vector3(basePos.X, basePos.Y, this.BlockDepth);
        vertices[vertIndex + 2] = new Vector3(basePos.X + 1, basePos.Y, this.BlockDepth);
        vertices[vertIndex + 3] = new Vector3(basePos.X + 1, basePos.Y, 0);

        // Add the indices
        int indiceIndex = quadIndex * 6;
        indices[indiceIndex++] = vertIndex;
        indices[indiceIndex++] = vertIndex + 1;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 3;
        indices[indiceIndex++] = vertIndex;
    }

    /// <summary>
    /// Add a quad to the mesh arrays.
    /// </summary>
    /// <param name="basePos">The base position of the block.</param>
    /// <param name="quadIndex">The index of the current quad.</param>
    /// <param name="vertices">The vertice array.</param>
    /// <param name="indices">The indices array.</param>
    private void AddRightQuad(Vector2I basePos, int quadIndex, Vector3[] vertices, int[] indices)
    {
        // Add the vertices
        int vertIndex = quadIndex * 4;
        vertices[vertIndex] = new Vector3(basePos.X + 1, basePos.Y, 0);
        vertices[vertIndex + 1] = new Vector3(basePos.X + 1, basePos.Y, this.BlockDepth);
        vertices[vertIndex + 2] = new Vector3(basePos.X + 1, basePos.Y - 1, this.BlockDepth);
        vertices[vertIndex + 3] = new Vector3(basePos.X + 1, basePos.Y - 1, 0);

        // Add the indices
        int indiceIndex = quadIndex * 6;
        indices[indiceIndex++] = vertIndex;
        indices[indiceIndex++] = vertIndex + 1;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 3;
        indices[indiceIndex++] = vertIndex;
    }

    /// <summary>
    /// Add a quad to the mesh arrays.
    /// </summary>
    /// <param name="basePos">The base position of the block.</param>
    /// <param name="quadIndex">The index of the current quad.</param>
    /// <param name="vertices">The vertice array.</param>
    /// <param name="indices">The indices array.</param>
    private void AddDownQuad(Vector2I basePos, int quadIndex, Vector3[] vertices, int[] indices)
    {
        // Add the vertices
        int vertIndex = quadIndex * 4;
        vertices[vertIndex] = new Vector3(basePos.X, basePos.Y - 1, 0);
        vertices[vertIndex + 1] = new Vector3(basePos.X + 1, basePos.Y - 1, 0);
        vertices[vertIndex + 2] = new Vector3(basePos.X + 1, basePos.Y - 1, this.BlockDepth);
        vertices[vertIndex + 3] = new Vector3(basePos.X, basePos.Y - 1, this.BlockDepth);

        // Add the indices
        int indiceIndex = quadIndex * 6;
        indices[indiceIndex++] = vertIndex;
        indices[indiceIndex++] = vertIndex + 1;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 3;
        indices[indiceIndex++] = vertIndex;
    }

    /// <summary>
    /// Add a quad to the mesh arrays.
    /// </summary>
    /// <param name="basePos">The base position of the block.</param>
    /// <param name="quadIndex">The index of the current quad.</param>
    /// <param name="vertices">The vertice array.</param>
    /// <param name="indices">The indices array.</param>
    private void AddLeftQuad(Vector2I basePos, int quadIndex, Vector3[] vertices, int[] indices)
    {
        // Add the vertices
        int vertIndex = quadIndex * 4;
        vertices[vertIndex] = new Vector3(basePos.X, basePos.Y, 0);
        vertices[vertIndex + 1] = new Vector3(basePos.X, basePos.Y - 1, 0);
        vertices[vertIndex + 2] = new Vector3(basePos.X, basePos.Y - 1, this.BlockDepth);
        vertices[vertIndex + 3] = new Vector3(basePos.X, basePos.Y, this.BlockDepth);

        // Add the indices
        int indiceIndex = quadIndex * 6;
        indices[indiceIndex++] = vertIndex;
        indices[indiceIndex++] = vertIndex + 1;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 2;
        indices[indiceIndex++] = vertIndex + 3;
        indices[indiceIndex++] = vertIndex;
    }
}