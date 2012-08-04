// ----------------------------------------------------------------------------
// <copyright file="TerrainRenderComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for rendering the terrain.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(TerrainComponent))]
public class TerrainRenderComponent : MonoBehaviour
{
    /// <summary>
    /// The core terrain component.
    /// </summary>
    private TerrainComponent cTerrain;

    /// <summary>
    /// The mesh filter component.
    /// </summary>
    private MeshFilter cMeshFilter;

    /// <summary>
    /// The mesh renderer component.
    /// </summary>
    private MeshRenderer cMeshRenderer;

    /// <summary>
    /// Gets the mesh generator.
    /// </summary>
    public TerrainMeshGenerator MeshGenerator { get; private set; }

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.MeshGenerator = new TerrainMeshGeneratorCubes();

        // Get a reference to the related terrain components
        this.cTerrain = this.GetComponent<TerrainComponent>();
        this.cMeshFilter = this.GetComponent<MeshFilter>();
        this.cMeshRenderer = this.GetComponent<MeshRenderer>();

        // Create the empty mesh
        this.cMeshFilter.mesh = new Mesh();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        // Check if the terrain mesh needs to be rebuilt
        if (this.cTerrain.Terrain.Mesh.MeshChanged)
        {
            this.RebuildMesh();
        }
    }

    /// <summary>
    /// Rebuild the geometry on the mesh filter.
    /// </summary>
    private void RebuildMesh()
    {
        // Build the arrays for the vertices and triangle indices for each submesh
        Vector3[] vertices = new Vector3[this.cTerrain.Terrain.Mesh.GetVerticeCount()];
        var materialIndices = new Dictionary<MaterialType, int[]>();
        var materialArrayIndexes = new Dictionary<MaterialType, int>();
        foreach (MaterialType material in this.cTerrain.Terrain.Mesh.GetMaterials())
        {
            materialIndices.Add(material, new int[this.cTerrain.Terrain.Mesh.GetIndiceCount(material)]);
            materialArrayIndexes.Add(material, 0);
        }

        // Populate the vertice and indice arrays
        int verticeArrayIndex = 0;
        foreach (KeyValuePair<Vector2I, BlockMesh> kvp in this.cTerrain.Terrain.Mesh)
        {
            BlockMesh blockMesh = kvp.Value;

            // Copy the vertices
            Array.Copy(blockMesh.Vertices, 0, vertices, verticeArrayIndex, blockMesh.Vertices.Length);

            // Copy the indices
            int[] indices = materialIndices[blockMesh.Material];
            int indiceArrayIndex = materialArrayIndexes[blockMesh.Material];
            for (int i = 0; i < blockMesh.Indices.Length; i++)
            {
                indices[indiceArrayIndex + i] = blockMesh.Indices[i] + verticeArrayIndex;
            }

            // Update array indexers
            verticeArrayIndex += blockMesh.Vertices.Length;
            materialArrayIndexes[blockMesh.Material] = indiceArrayIndex + blockMesh.Indices.Length;
        }

        // Update the mesh filter geometry
        this.cMeshFilter.mesh.Clear();
        this.cMeshFilter.mesh.vertices = vertices;

        // Populate the submesh triangles and materials
        int materialIndex = 0;
        this.cMeshFilter.mesh.subMeshCount = materialIndices.Count;
        this.cMeshRenderer.materials = new Material[materialIndices.Count];
        foreach (KeyValuePair<MaterialType, int[]> kvp in materialIndices)
        {
            // Set the triangles
            this.cMeshFilter.mesh.SetTriangles(kvp.Value, materialIndex);

            // Set the material on the mesh renderer
            // TODO: Do this properly. Right now it just uses the generic diffuse material
            this.cMeshRenderer.materials[materialIndex] = new Material(Shader.Find("Diffuse"));

            // Increment the index
            materialIndex++;
        }

        // Recalculate the mesh normals
        this.cMeshFilter.mesh.RecalculateNormals();

        // Reset the mesh changed flag
        this.cTerrain.Terrain.Mesh.ResetMeshChanged();
    }
}