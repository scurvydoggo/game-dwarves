// ----------------------------------------------------------------------------
// <copyright file="TerrainChunkComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using System.Collections.Generic;
    using Dwarves.Core.Math;
    using Dwarves.Core.Terrain;
    using UnityEngine;

    /// <summary>
    /// Represents a terrain mesh.
    /// </summary>
    public class TerrainChunkComponent : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the terrain instance.
        /// </summary>
        public VoxelTerrain Terrain { get; set; }

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
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            // Rebuild the mesh for this chunk if required
            if (!this.Terrain.Meshes.ContainsKey(this.Chunk))
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
            this.Terrain.Meshes.Remove(this.Chunk);

            // TODO: Run the marching cubes algorithm on this chunk
        }
    }
}