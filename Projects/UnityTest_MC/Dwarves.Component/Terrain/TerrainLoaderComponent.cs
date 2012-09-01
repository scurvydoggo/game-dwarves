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
        private Dictionary<Position, ChunkUsage> actorChunks;

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
            this.actorChunks = new Dictionary<Position, ChunkUsage>();

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
                        chunk.Mesh.ClearMesh();
                    }
                }
            }
            else
            {
                // Load the chunk data
                chunk = this.ChunkLoader.LoadChunk(terrain, chunkIndex, usage);

                // Create the chunk game object
                var chunkObject = new GameObject(this.GetChunkLabel(chunkIndex));
                chunkObject.transform.parent = this.transform;
                ChunkComponent cChunk = chunkObject.AddComponent<ChunkComponent>();
                cChunk.Chunk = chunk;
                cChunk.ChunkIndex = chunkIndex;

                // Generate the chunk mesh if necessary
                if ((usage & ChunkUsage.Rendering) != 0)
                {
                    this.cTerrainRender.MeshGenerator.UpdateChunk(this.cTerrain.Terrain, chunkIndex);
                }
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