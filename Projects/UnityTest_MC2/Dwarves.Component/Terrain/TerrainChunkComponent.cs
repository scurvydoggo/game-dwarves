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
            return string.Format("Chunk[{0},{1}]", chunk.X, chunk.Y);
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
            // TODO: Check if the mesh for this chunk needs to be rebuilt
        }
    }
}