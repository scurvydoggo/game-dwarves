using UnityEngine;

/// <summary>
/// The mesh data for a terrain block.
/// </summary>
public class BlockMesh
{
    /// <summary>
    /// Initializes a new instance of the BlockMesh class.
    /// </summary>
    /// <param name="material">The material of this block.</param>
    /// <param name="vertices">The vertices of this block.</param>
    /// <param name="indices">The triangle indices of this block.</param>
    public BlockMesh(MaterialType material, Vector3[] vertices, int[] indices)
    {
        this.Material = material;
        this.Vertices = vertices;
        this.Indices = indices;
    }

    /// <summary>
    /// Gets or sets the material of this block.
    /// </summary>
    public MaterialType Material { get; set; }

    /// <summary>
    /// Gets or sets the vertices of this block.
    /// </summary>
    public Vector3[] Vertices { get; set; }

    /// <summary>
    /// Gets or sets the triangle indices of this block.
    /// </summary>
    public int[] Indices { get; set; }
}