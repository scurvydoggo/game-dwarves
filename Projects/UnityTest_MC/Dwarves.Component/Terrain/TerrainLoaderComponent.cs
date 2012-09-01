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
    public class TerrainLoaderComponent : MonoBehaviour
    {
        /// <summary>
        /// The chunks which contain actors.
        /// </summary>
        private Dictionary<Position, ChunkUsage> actorChunks;

        /// <summary>
        /// The core terrain component.
        /// </summary>
        private TerrainComponent cTerrain;

        /// <summary>
        /// The mesh generator component.
        /// </summary>
        private ChunkRenderComponent cTerrainRender;

        /// <summary>
        /// Gets the terrain chunk loader.
        /// </summary>
        public ChunkLoader ChunkLoader { get; private set; }

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.actorChunks = new Dictionary<Position, ChunkUsage>();

            // Get a reference to the related terrain components
            this.cTerrain = this.GetComponent<TerrainComponent>();
            this.cTerrainRender = this.GetComponent<ChunkRenderComponent>();

            // Create the chunk loader
            this.ChunkLoader = new ChunkLoader(this.cTerrain.Seed);
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
            this.actorChunks.Clear();

            // Iterate through each actor in the scene
            foreach (ActorComponent actor in GameObject.FindObjectsOfType(typeof(ActorComponent)))
            {
                // Get the chunk-bounds of the actor
                Rectangle bounds = actor.GetChunkBounds();

                // Determine the usage that the actor requires
                ChunkUsage usage = ChunkUsage.Data;
                if (actor.RequiresTerrainRendering)
                {
                    usage |= ChunkUsage.Rendering;
                }

                if (actor.RequiresTerrainPhysics)
                {
                    usage |= ChunkUsage.Physics;
                }

                // Step through each chunk index in the actor bounds
                for (int x = bounds.X; x < bounds.Right; x++)
                {
                    for (int y = bounds.Y; y > bounds.Bottom; y--)
                    {
                        Position chunkIndex = new Position(x, y);

                        // Update the actor chunks dictionary
                        ChunkUsage existingUsage;
                        if (this.actorChunks.TryGetValue(chunkIndex, out existingUsage))
                        {
                            // Escalate the chunk usage if this actor requires more from it
                            ChunkUsage newUsage = existingUsage | usage;
                            if (newUsage != existingUsage)
                            {
                                this.actorChunks[chunkIndex] = newUsage;
                            }
                        }
                        else
                        {
                            this.actorChunks.Add(chunkIndex, usage);
                        }
                    }
                }
            }

            // Check if any chunks are now off screen
            var toRemove = new List<Position>();
            foreach (KeyValuePair<Position, Chunk> kvp in this.cTerrain.Terrain)
            {
                if (!this.actorChunks.ContainsKey(kvp.Key))
                {
                    toRemove.Add(kvp.Key);
                }
            }

            // Unload chunks that are no longer used
            foreach (Position chunkIndex in toRemove)
            {
                this.UnloadChunk(this.cTerrain.Terrain, chunkIndex);
            }

            // Load/update the new/modified chunks
            foreach (KeyValuePair<Position, ChunkUsage> kvp in this.actorChunks)
            {
                this.LoadUpdateChunk(this.cTerrain.Terrain, kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Load or update the given chunk.
        /// </summary>
        /// <param name="terrain">The terrain object.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="usage">The chunk usage.</param>
        private void LoadUpdateChunk(VoxelTerrain terrain, Position chunkIndex, ChunkUsage usage)
        {
            // Load the chunk or get the existing chunk
            Chunk chunk;
            if (this.cTerrain.Terrain.TryGetChunk(chunkIndex, out chunk))
            {
                // The chunk is already loaded. Check if the chunk usage needs to be updated.
                if (chunk.Usage != usage)
                {
                    if ((chunk.Usage & ChunkUsage.Rendering) != 0 && (usage & ChunkUsage.Rendering) == 0)
                    {
                        // The mesh data is no longer required
                        ChunkRenderComponent cChunk = this.GetChunkRenderComponent(chunkIndex);
                        cChunk.ClearMesh();
                    }
                }
            }
            else
            {
                // Load the chunk data
                chunk = this.ChunkLoader.LoadChunk(terrain, chunkIndex);

                // Set the chunk usage flag
                chunk.Usage = usage;

                // Create the chunk game object
                var chunkObject = new GameObject();
                chunkObject.transform.parent = this.transform;
                ChunkComponent cChunk = chunkObject.AddComponent<ChunkComponent>();
                ChunkRenderComponent cChunkRender = chunkObject.AddComponent<ChunkRenderComponent>();
                cChunk.Chunk = chunk;
                cChunk.ChunkIndex = chunkIndex;
                cChunkRender.IsoLevel = this.cTerrain.IsoLevel;
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
            ChunkRenderComponent cChunk = this.GetChunkRenderComponent(chunkIndex);
            if (cChunk != null)
            {
                GameObject.Destroy(cChunk.gameObject);
            }

            // Unload the chunk data
            this.ChunkLoader.UnloadChunk(terrain, chunkIndex);
        }

        /// <summary>
        /// Gets the chunk render component with the given index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk render component; Null if it does not exist.</returns>
        private ChunkRenderComponent GetChunkRenderComponent(Position chunkIndex)
        {
            foreach (ChunkRenderComponent cRender in this.GetComponentsInChildren<ChunkRenderComponent>(true))
            {
                if (cRender.ChunkIndex.Equals(chunkIndex))
                {
                    return cRender;
                }
            }

            return null;
        }
    }
}