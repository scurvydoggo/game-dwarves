﻿// ----------------------------------------------------------------------------
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
        /// The terrain manager.
        /// </summary>
        private TerrainManager terrainManager;

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
            // Get a reference to the related components
            this.terrainManager = this.transform.parent.GetComponent<TerrainComponent>().TerrainManager;
            this.cMeshFilter = this.GetComponent<MeshFilter>();
        }

        /// <summary>
        /// Called once per frame after the Update method has been called for all components.
        /// </summary>
        public void LateUpdate()
        {
            // Rebuild the mesh for this chunk if required
            if (!this.terrainManager.Terrain.Meshes.ContainsKey(this.Chunk))
            {
                this.RebuildMesh();
            }
        }

        /// <summary>
        /// Build the mesh for this chunk.
        /// </summary>
        public void RebuildMesh()
        {
            // Remove any existing mesh data for this chunk
            this.terrainManager.Terrain.Meshes.Remove(this.Chunk);

            // Build the mesh for this chunk
            MeshData meshData = this.terrainManager.TerrainMeshBuilder.CreateMesh(this.Chunk);

            // TODO: Load the mesh data
        }
    }
}