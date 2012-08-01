// ----------------------------------------------------------------------------
// <copyright file="TerrainLoaderComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for loading terrain chunks.
/// </summary>
[RequireComponent(typeof(TerrainComponent))]
[RequireComponent(typeof(TerrainMeshGeneratorComponent))]
public class TerrainLoaderComponent : MonoBehaviour
{
    /// <summary>
    /// The chunks which contain actors.
    /// </summary>
    private Dictionary<Vector2I, ChunkUsage> actorChunks;
	
    /// <summary>
    /// The core terrain component.
    /// </summary>
	private TerrainComponent terrainComponent;

    /// <summary>
    /// The core terrain component.
    /// </summary>
	private TerrainMeshGeneratorComponent meshGeneratorComponent;
	
    /// <summary>
    /// Gets the terrain block loader.
    /// </summary>
    public TerrainBlockLoader TerrainBlockLoader { get; private set; }

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.TerrainBlockLoader = new TerrainBlockLoader();
        this.actorChunks = new Dictionary<Vector2I, ChunkUsage>();

        // Get a reference to the related terrain components
        this.terrainComponent = this.GetComponent<TerrainComponent>();
        this.meshGeneratorComponent = this.GetComponent<TerrainMeshGeneratorComponent>();
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

        // Iterate through each ActorComponent in the scene
        foreach (ActorComponent actor in GameObject.FindObjectsOfType(typeof(ActorComponent)))
        {
            // Get the chunk-bounds of the actor
            RectI bounds = actor.GetChunkBounds();

            // Determine the usage that the actor requires
            ChunkUsage usage = ChunkUsage.Blocks;
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
                    Vector2I chunkIndex = new Vector2I(x, y);

                    // Update the actor chunks dictionary
                    ChunkUsage existingUsage;
                    if (this.actorChunks.TryGetValue(chunkIndex, out existingUsage))
                    {
                        // 'Increase' the chunk usage if this actor requires more from it
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

        // Unload chunks that are no longer used
        foreach (Vector2I chunkIndex in this.terrainComponent.Terrain.Blocks.ActiveChunks.Keys)
        {
            if (!this.actorChunks.ContainsKey(chunkIndex))
            {
                this.UnloadChunk(this.terrainComponent.Terrain, chunkIndex);
            }
        }

        // Load/update the new/modified chunks
        foreach (KeyValuePair<Vector2I, ChunkUsage> kvp in this.actorChunks)
        {
			this.LoadUpdateChunk(this.terrainComponent.Terrain, kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Load or update the given chunk.
    /// </summary>
    /// <param name="terrain">The terrain object.</param>
    /// <param name="chunkIndex">The chunk index.</param>
    /// <param name="newUsage">The chunk usage.</param>
    private void LoadUpdateChunk(Terrain terrain, Vector2I chunkIndex, ChunkUsage newUsage)
    {
		// Load the chunk or get the existing chunk
		Chunk chunk;
		bool doLoadMesh = false;
		if (this.terrainComponent.Terrain.Blocks.TryGetChunk(kvp.Key, out chunk))
		{
			// The chunk is already loaded. Check if the chunk usage needs to be updated.
			if (chunk.Usage != kvp.Value)
			{
				if (chunk.Usage & ChunkUsage.Rendering == 0 && newUsage & ChunkUsage.Rendering != 0)
				{
					// The chunk mesh data is now required
					// TODO
				}
				else if (chunk.Usage & ChunkUsage.Rendering != 0 && newUsage & ChunkUsage.Rendering == 0)
				{
					// The mesh data is no longer required
					// TODO
				}
			}
		}
		else
		{
			// Load the block data
			chunk = this.TerrainBlockLoader.LoadChunk(terrain, chunkIndex);
			
			// Set the chunk usage flag
			chunk.Usage = newUsage;
		
			if (chunk.Usage & ChunkUsage.Rendering != 0)
			{
				// The chunk mesh data is required
				// TODO
			}
		}
    }

    /// <summary>
    /// Unload the given chunk.
    /// </summary>
    /// <param name="terrain">The terrain object.</param>
    /// <param name="chunkIndex">The chunk index.</param>
    private void UnloadChunk(Terrain terrain, Vector2I chunkIndex)
    {
		// Unload the block data
        Chunk chunk = this.TerrainBlockLoader.UnloadChunk(terrain, chunkIndex);
		
		if (chunk.Usage & ChunkUsage.Rendering != 0)
		{
			// Unload the mesh data
			// TODO
		}
    }
}