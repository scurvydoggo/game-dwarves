// ----------------------------------------------------------------------------
// <copyright file="TerrainLoaderComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using System.Collections.Generic;
    using Dwarves.Component.Bounds;
    using Dwarves.Core;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Load;
    using UnityEngine;

    /// <summary>
    /// Component for loading the terrain.
    /// </summary>
    [RequireComponent(typeof(TerrainComponent))]
    [RequireComponent(typeof(TerrainMeshComponent))]
    public class TerrainLoaderComponent : MonoBehaviour
    {
        /// <summary>
        /// The seed for generating terrain.
        /// </summary>
        public float Seed;

        /// <summary>
        /// The core terrain component.
        /// </summary>
        private TerrainComponent cTerrain;

        /// <summary>
        /// The terrain render component.
        /// </summary>
        private TerrainMeshComponent cTerrainRender;

        /// <summary>
        /// Gets the terrain chunk loader.
        /// </summary>
        public ChunkLoader ChunkLoader { get; private set; }

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            // Get a reference to the related components
            this.cTerrain = this.GetComponent<TerrainComponent>();
            this.cTerrainRender = this.GetComponent<TerrainMeshComponent>();

            // Create the chunk loader
            this.ChunkLoader = new ChunkLoader(this.Seed);
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            this.LoadUnloadActorChunks();
        }

        /// <summary>
        /// Gets the label for the given chunk index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk label.</returns>
        private static string GetChunkLabel(Position chunkIndex)
        {
            return string.Format("Chunk[{0},{1}]", chunkIndex.X, chunkIndex.Y);
        }

        /// <summary>
        /// Check the bounds of each actor in the game world and load/unload the chunks that are new/no longer required.
        /// </summary>
        private void LoadUnloadActorChunks()
        {
            var activeChunks = new HashSet<Position>();
            var toRemove = new HashSet<Position>();
            var toGenerateMesh = new List<Position>();

            // Check which chunks are currently active
            foreach (ActorComponent actor in GameObject.FindObjectsOfType(typeof(ActorComponent)))
            {
                // Get the chunk-bounds of the actor
                Rectangle bounds = actor.GetChunkBounds();

                // Step through each chunk index in the actor bounds
                for (int x = bounds.X; x < bounds.Right; x++)
                {
                    for (int y = bounds.Y; y > bounds.Bottom; y--)
                    {
                        activeChunks.Add(new Position(x, y));
                    }
                }
            }

            // Check if any chunks are now off screen and will need to be removed
            foreach (Position chunkIndex in this.cTerrain.Terrain.Chunks.Keys)
            {
                if (!activeChunks.Contains(chunkIndex))
                {
                    toRemove.Add(chunkIndex);
                }
            }

            // Load the new chunk data
            foreach (Position chunkIndex in activeChunks)
            {
                if (!this.cTerrain.Terrain.Chunks.ContainsKey(chunkIndex))
                {
                    // Load the chunk data
                    Chunk chunk = this.ChunkLoader.LoadChunk(this.cTerrain.Terrain, chunkIndex);

                    // Create the chunk game object
                    var chunkObject = new GameObject(TerrainLoaderComponent.GetChunkLabel(chunkIndex));
                    chunkObject.transform.parent = this.transform;
                    ChunkComponent chunkComponent = chunkObject.AddComponent<ChunkComponent>();
                    chunkComponent.Chunk = chunk;
                    chunkComponent.ChunkIndex = chunkIndex;

                    // Add the chunk to a list indicating that it's mesh needs to be generated
                    toGenerateMesh.Add(chunkIndex);
                }
            }

            // Generate the meshes for the newly loaded chunks
            foreach (Position chunkIndex in toGenerateMesh)
            {
                this.cTerrainRender.MeshGenerator.UpdateChunk(this.cTerrain.Terrain, chunkIndex);
            }

            // Unload chunks that are no longer used
            foreach (Position chunkIndex in toRemove)
            {
                // Destroy the chunk's game object
                ChunkComponent cChunk = this.GetChunkComponent(chunkIndex);
                if (cChunk != null)
                {
                    GameObject.Destroy(cChunk.gameObject);
                }

                // Unload the chunk data
                this.ChunkLoader.UnloadChunk(this.cTerrain.Terrain, chunkIndex);
            }
        }

        /// <summary>
        /// Gets the chunk component with the given index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk component; Null if it does not exist.</returns>
        private ChunkComponent GetChunkComponent(Position chunkIndex)
        {
            Transform chunkTransform = this.transform.FindChild(TerrainLoaderComponent.GetChunkLabel(chunkIndex));
            return chunkTransform != null ? chunkTransform.GetComponent<ChunkComponent>() : null;
        }
    }
}