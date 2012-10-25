// ----------------------------------------------------------------------------
// <copyright file="ChunkComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core;
    using Dwarves.Core.Mesh;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Load;
    using UnityEngine;

    /// <summary>
    /// Core component for chunk entity.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ChunkComponent : MonoBehaviour
    {
        /// <summary>
        /// The chunk.
        /// </summary>
        public Chunk Chunk;

        /// <summary>
        /// The index of this chunk.
        /// </summary>
        public Position ChunkIndex;

        /// <summary>
        /// The mesh filter component.
        /// </summary>
        private MeshFilter cMeshFilter;

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            // Get a reference to the related components
            this.cMeshFilter = this.GetComponent<MeshFilter>();
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            // Check if the chunk mesh needs to be rebuilt
            if (this.Chunk.Mesh.MeshChanged)
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
            Vector3[] vertices = new Vector3[this.Chunk.Mesh.VertexCount];
            int[] indices = new int[this.Chunk.Mesh.TriangleIndicesCount];
            Vector2[] uvs = new Vector2[this.Chunk.Mesh.VertexCount];
            
            // Populate the vertice and indice arrays
            int vertexArrayIndex = 0;
            int indiceArrayIndex = 0;
            foreach (KeyValuePair<Position, MeshData> kvp in this.Chunk.Mesh)
            {
                MeshData meshData = kvp.Value;

                // Copy the vertices
                Array.Copy(meshData.Vertices, 0, vertices, vertexArrayIndex, meshData.Vertices.Length);

                // Copy the indices
                for (int i = 0; i < meshData.TriangleIndices.Length; i++)
                {
                    indices[indiceArrayIndex + i] = meshData.TriangleIndices[i] + vertexArrayIndex;
                }

                // Copy the UV coordinates
                Array.Copy(meshData.UVs, 0, uvs, vertexArrayIndex, meshData.UVs.Length);

                // Update array indexers
                vertexArrayIndex += meshData.Vertices.Length;
                indiceArrayIndex += meshData.TriangleIndices.Length;
            }

            // Update the mesh filter geometry
            this.cMeshFilter.mesh.Clear();
            this.cMeshFilter.mesh.vertices = vertices;
            this.cMeshFilter.mesh.triangles = indices;
            this.cMeshFilter.mesh.uv = uvs;

            // Recalculate the mesh normals
            this.cMeshFilter.mesh.RecalculateNormals();

            // TODO: FIXME: Test code. Use Dirt material by default for now
            this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Dirt");

            // Reset the mesh changed flag
            this.Chunk.Mesh.ResetMeshChanged();
        }
    }
}