// ----------------------------------------------------------------------------
// <copyright file="ChunkComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using Dwarves.Core;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Load;
    using UnityEngine;

    /// <summary>
    /// Core component for chunk entity.
    /// </summary>
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
        }
    }
}