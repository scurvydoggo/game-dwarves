// ----------------------------------------------------------------------------
// <copyright file="TerrainChunkComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using System.Collections.Generic;
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Geometry;
    using UnityEngine;

    /// <summary>
    /// Represents a terrain mesh.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TerrainChunkComponent : MonoBehaviour
    {
        /// <summary>
        /// The mesh filter component.
        /// </summary>
        private MeshFilter cMeshFilter;

        /// <summary>
        /// Gets or sets the chunk index.
        /// </summary>
        public Vector2I Chunk { get; set; }

        /// <summary>
        /// Gets the label for the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>The chunk label.</returns>
        public static string GetLabel(Vector2I chunk)
        {
            return "Chunk[" + chunk.X + "," + chunk.Y + "]";
        }

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.cMeshFilter = this.GetComponent<MeshFilter>();
            this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Terrain_GrassMud");
        }

        /// <summary>
        /// Called once per frame after the Update method has been called for all components.
        /// </summary>
        public void LateUpdate()
        {
            // Rebuild the mesh for this chunk if required
            if (TerrainManager.Instance.Terrain.RebuildRequired(this.Chunk))
            {
                this.RebuildMesh();
            }
        }

        /// <summary>
        /// Build the mesh for this chunk.
        /// </summary>
        public void RebuildMesh()
        {
            // Create the mesh for this chunk
            MeshData meshData = TerrainManager.Instance.TerrainMeshBuilder.CreateMesh(this.Chunk);

            // Update the mesh filter geometry
            this.cMeshFilter.mesh.Clear();
            this.cMeshFilter.mesh.vertices = meshData.Vertices.ToArray();
            this.cMeshFilter.mesh.normals = meshData.Normals.ToArray();
            this.cMeshFilter.mesh.triangles = meshData.Indices.ToArray();
            this.cMeshFilter.mesh.uv = meshData.UVs.ToArray();

            // Flag this chunk as no longer requiring a rebuild
            TerrainManager.Instance.Terrain.GetChunk(this.Chunk).RebuildRequired = false;
        }
    }
}