// ----------------------------------------------------------------------------
// <copyright file="ChunkRenderComponent.cs" company="Acidwashed Games">
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
    using Dwarves.Core.Terrain.Generation;
    using Dwarves.Core.Terrain.Generation.MarchingCubes;
    using UnityEngine;

    /// <summary>
    /// Component for rendering a terrain chunk.
    /// </summary>
    [RequireComponent(typeof(ChunkComponent))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ChunkRenderComponent : MonoBehaviour
    {
        /// <summary>
        /// The isolevel for the rendered terrain. This is the density at which the surface is rendered.
        /// </summary>
        public byte IsoLevel;

        /// <summary>
        /// The chunk component.
        /// </summary>
        private ChunkComponent cChunk;

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
        public MeshGenerator MeshGenerator { get; private set; }

        /// <summary>
        /// Gets the chunk index.
        /// </summary>
        public Position ChunkIndex
        {
            get
            {
                return this.cChunk.ChunkIndex;
            }
        }

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.MeshGenerator = new MarchingCubesGenerator(this.IsoLevel);

            // Get a reference to the related components
            this.cChunk = this.GetComponent<ChunkComponent>();
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
            // Check if the chunk mesh needs to be rebuilt
            if ((this.cChunk.Chunk.Usage & ChunkUsage.Rendering) != 0 && this.cChunk.Chunk.Mesh.MeshChanged)
            {
                this.RebuildMesh();
            }
        }

        /// <summary>
        /// Clear the mesh data.
        /// </summary>
        public void ClearMesh()
        {
            // Clear the mesh data
            this.cChunk.Chunk.Mesh.ClearMesh();

            // Clear the unity mesh object
            this.cMeshFilter.mesh.Clear();
        }

        /// <summary>
        /// Rebuild the geometry on the mesh filter.
        /// </summary>
        private void RebuildMesh()
        {
            // Build the arrays for the vertices and triangle indices for each submesh
            Vector3[] vertices = new Vector3[this.cChunk.Chunk.Mesh.VertexCount];
            int[] indices = new int[this.cChunk.Chunk.Mesh.TriangleIndicesCount];

            // Populate the vertice and indice arrays
            int vertexArrayIndex = 0;
            int indiceArrayIndex = 0;
            foreach (KeyValuePair<Position, MeshData> kvp in this.cChunk.Chunk.Mesh)
            {
                MeshData meshData = kvp.Value;

                // Copy the vertices
                Array.Copy(meshData.Vertices, 0, vertices, vertexArrayIndex, meshData.Vertices.Length);

                // Copy the indices
                for (int i = 0; i < meshData.TriangleIndices.Length; i++)
                {
                    indices[indiceArrayIndex + i] = meshData.TriangleIndices[i] + vertexArrayIndex;
                }

                // Update array indexers
                vertexArrayIndex += meshData.Vertices.Length;
                indiceArrayIndex += meshData.TriangleIndices.Length;
            }

            // Update the mesh filter geometry
            this.cMeshFilter.mesh.Clear();
            this.cMeshFilter.mesh.vertices = vertices;
            this.cMeshFilter.mesh.triangles = indices;

            // Set the UV coordinates
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
            }

            this.cMeshFilter.mesh.uv = uvs;

            // Recalculate the mesh normals
            this.cMeshFilter.mesh.RecalculateNormals();

            // Reset the mesh changed flag
            this.cChunk.Chunk.Mesh.ResetMeshChanged();
        }
    }
}