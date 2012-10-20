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
        /// The chunks which contain actors.
        /// </summary>
        private HashSet<Position> loadedChunks;

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
            this.loadedChunks = new HashSet<Position>();

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
        /// Check the bounds of each actor in the game world and load/unload the chunks that are new/no longer required.
        /// </summary>
        private void LoadUnloadActorChunks()
        {
            this.loadedChunks.Clear();

            // Iterate through each actor in the scene
            foreach (ActorComponent actor in GameObject.FindObjectsOfType(typeof(ActorComponent)))
            {
                // Get the chunk-bounds of the actor
                Rectangle bounds = actor.GetChunkBounds();

                // Step through each chunk index in the actor bounds
                for (int x = bounds.X; x < bounds.Right; x++)
                {
                    for (int y = bounds.Y; y > bounds.Bottom; y--)
                    {
                        var chunkIndex = new Position(x, y);

                        // Update the loaded chunks list
                        this.loadedChunks.Add(chunkIndex);
                    }
                }
            }

            // Check if any chunks are now off screen
            var toRemove = new List<Position>();
            foreach (Position chunkIndex in this.cTerrain.Terrain.Chunks.Keys)
            {
                if (!this.loadedChunks.Contains(chunkIndex))
                {
                    toRemove.Add(chunkIndex);
                }
            }

            // Load/update the new/modified chunks
            foreach (Position chunkIndex in this.loadedChunks)
            {
                this.LoadUpdateChunk(this.cTerrain.Terrain, chunkIndex);
            }

            // Unload chunks that are no longer used
            foreach (Position chunkIndex in toRemove)
            {
                this.UnloadChunk(this.cTerrain.Terrain, chunkIndex);
            }
        }

        /// <summary>
        /// Load or update the given chunk.
        /// </summary>
        /// <param name="terrain">The terrain object.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        private void LoadUpdateChunk(VoxelTerrain terrain, Position chunkIndex)
        {
            if (!this.cTerrain.Terrain.Chunks.ContainsKey(chunkIndex))
            {
                // Load the chunk data
                Chunk chunk = this.ChunkLoader.LoadChunk(terrain, chunkIndex);

                // Create the chunk game object
                var chunkObject = new GameObject(this.GetChunkLabel(chunkIndex));
                chunkObject.transform.parent = this.transform;
                ChunkComponent chunkComponent = chunkObject.AddComponent<ChunkComponent>();
                chunkComponent.Chunk = chunk;
                chunkComponent.ChunkIndex = chunkIndex;

                // Generate the chunk mesh
                this.cTerrainRender.MeshGenerator.UpdateChunk(this.cTerrain.Terrain, chunkIndex);
            }
        }

        /// <summary>
        /// Unload the given chunk.
        /// </summary>
        /// <param name="terrain">The terrain object.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        private void UnloadChunk(VoxelTerrain terrain, Position chunkIndex)
        {
            // Destroy the chunk's game object
            ChunkComponent cChunk = this.GetChunkComponent(chunkIndex);
            if (cChunk != null)
            {
                GameObject.Destroy(cChunk.gameObject);
            }

            // Unload the chunk data
            this.ChunkLoader.UnloadChunk(terrain, chunkIndex);
        }

        /// <summary>
        /// Gets the chunk component with the given index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk component; Null if it does not exist.</returns>
        private ChunkComponent GetChunkComponent(Position chunkIndex)
        {
            Transform chunkTransform = this.transform.FindChild(this.GetChunkLabel(chunkIndex));
            return chunkTransform != null ? chunkTransform.GetComponent<ChunkComponent>() : null;
        }

        /// <summary>
        /// Gets the label for the given chunk index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk label.</returns>
        private string GetChunkLabel(Position chunkIndex)
        {
            return string.Format("Chunk[{0},{1}]", chunkIndex.X, chunkIndex.Y);
        }
    }
}