// ----------------------------------------------------------------------------
// <copyright file="ActorComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Component representing an actor that can move through the world. The significance of this is that the terrain must
/// be loaded wherever the actor is.
/// </summary>
public class ActorComponent : MonoBehaviour
{
    /// <summary>
    /// Gets or sets a value indicating the actor requires render-related processing to be performed on the terrain
	/// chunks in which it resides.
    /// </summary>
	public bool RequiresTerrainRendering;
	
    /// <summary>
    /// Gets or sets a value indicating the actor requires physics-related processing to be performed on the terrain
	/// chunks in which it resides.
    /// </summary>
	public bool RequiresTerrainPhysics;

    /// <summary>
    /// Gets the lower coordinate of the actor's bounding box (in terrain chunk coordinates).
    /// </summary>
	public Vector2I LowerBounds { get; private set; }
	
    /// <summary>
    /// Gets the upper coordinate of the actor's bounding box (in terrain chunk coordinates).
    /// </summary>
	public Vector2I UpperBounds { get; private set; }

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
		// Get the initial bounds of the actor
		// TODO
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
		// See if the actor has moved into new chunks and/or left chunks
		// TODO
		
		// Load/unload the chunks
		// TODO
    }
}