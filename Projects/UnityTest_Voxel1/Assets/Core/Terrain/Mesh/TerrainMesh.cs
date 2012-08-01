// ----------------------------------------------------------------------------
// <copyright file="TerrainMesh.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using System.Collections.Generic;

/// <summary>
/// Manages the mesh data of terrain blocks.
/// </summary>
public class TerrainMesh : IEnumerable<KeyValuePair<Vector2I, BlockMesh>>
{
    /// <summary>
    /// The mesh data per block.
    /// </summary>
    private Dictionary<Vector2I, BlockMesh> meshes;

    /// <summary>
    /// The total number of vertices in the cloud.
    /// </summary>
    private int verticeCount;

    /// <summary>
    /// The total number of indices (vertex indices for triangles) per material.
    /// </summary>
    private Dictionary<MaterialType, int> indiceCounts;

    /// <summary>
    /// Initializes a new instance of the TerrainMesh class.
    /// </summary>
    public TerrainMesh()
    {
        this.meshes = new Dictionary<Vector2I, BlockMesh>();
        this.verticeCount = 0;
        this.indiceCounts = new Dictionary<MaterialType, int>();
    }
	
    /// <summary>
    /// Gets a value indicating whether the mesh has been changed.
    /// </summary>
	public bool MeshChanged { get; private set; }

    /// <summary>
    /// Gets the block mesh at the given world position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>The block mesh.</returns>
    public BlockMesh this[Vector2I position]
    {
        get
        {
            return this.meshes[position];
        }

        set
        {
            this.SetMesh(position, value);
        }
    }
	
    /// <summary>
    /// Gets an enumerator that iterates through the block meshes.
    /// </summary>
    /// <returns>The enumerator.</returns>
	public IEnumerator<KeyValuePair<Vector2I, BlockMesh>> GetEnumerator()
	{
		return this.meshes.GetEnumerator();
	}

    /// <summary>
    /// Try to get the block mesh at the given world position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="mesh">The block mesh.</param>
    /// <returns>True if the block mesh was retrieved.</returns>
    public bool TryGetMesh(Vector2I position, out BlockMesh mesh)
    {
        return this.meshes.TryGetValue(position, out mesh);
    }

    /// <summary>
    /// Update the mesh at the given world position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="mesh">The block mesh.</param>
    public void SetMesh(Vector2I position, BlockMesh mesh)
    {
        // Add/Replace the mesh at this position
        BlockMesh existingMesh;
        if (this.meshes.TryGetValue(position, out existingMesh))
        {
            this.meshes[position] = mesh;

            // Decrement the vertex/triangle count for the mesh that was replaced
            this.verticeCount -= existingMesh.Vertices.Length;
            this.indiceCounts[existingMesh.Material] -= existingMesh.Indices.Length;
        }
        else
        {
            this.meshes.Add(position, mesh);
        }

        // Update the counts for the new mesh
        this.verticeCount += mesh.Vertices.Length;
        if (this.indiceCounts.ContainsKey(mesh.Material))
        {
            this.indiceCounts[mesh.Material] += mesh.Indices.Length;
        }
        else
        {
            this.indiceCounts.Add(mesh.Material, mesh.Indices.Length);
        }
		
		this.MeshChanged = true;
    }

    /// <summary>
    /// Remove the mesh at the given world position.
    /// </summary>
    /// <param name="position">The position.</param>
    public void RemoveMesh(Vector2I position)
    {
        BlockMesh mesh;
        if (this.meshes.TryGetValue(position, out mesh))
        {
            // Remove the mesh
            this.meshes.Remove(position);

            // Decrement the vertex/triangle count for the mesh that was removed
            this.verticeCount -= mesh.Vertices.Length;
            int newIndiceCount = this.indiceCounts[mesh.Material] - mesh.Indices.Length;
            if (newIndiceCount > 0)
            {
                this.indiceCounts[mesh.Material] = newIndiceCount;
            }
            else
            {
                this.indiceCounts.Remove(mesh.Material);
            }
        }
		
		this.MeshChanged = true;
    }

    /// <summary>
    /// Get the total number of vertices in the cloud.
    /// </summary>
    /// <returns>The vertice count.</returns>
    public int GetVerticeCount()
    {
        return this.verticeCount;
    }

    /// <summary>
    /// Get the total number of indices for the given material.
    /// </summary>
    /// <param name="material">The material for which to get the indice count.</param>
    /// <returns>The indice count.</returns>
    public int GetIndiceCount(MaterialType material)
    {
        int count;
        if (this.indiceCounts.TryGetValue(material, out count))
        {
            return count;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Gets the materials the exist in the meshes.
    /// </summary>
    /// <returns>The list of materials.</returns>
    public MaterialType[] GetMaterials()
    {
        var materials = new MaterialType[this.indiceCounts.Count];
        this.indiceCounts.Keys.CopyTo(materials, 0);
        return materials;
    }
	
    /// <summary>
    /// Resets the MeshChanged flag.
    /// </summary>
	public void ResetMeshChanged()
	{
		this.MeshChanged = false;
	}
}