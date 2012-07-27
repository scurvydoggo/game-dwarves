// ----------------------------------------------------------------------------
// <copyright file="TerrainLoaderComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

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
    /// Gets the terrain loader.
    /// </summary>
    public TerrainLoader TerrainLoader { get; private set; }

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.TerrainLoader = new TerrainLoader();
		this.actorChunks = new Dictionary<Vector2I, ChunkUsage>();
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
		// TODO
		
		// Load the new chunks
		// TODO
	}
}