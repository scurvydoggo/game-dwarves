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
	
    /// <summary>
    /// Determine the bounds of this actor.
    /// </summary>
	/// <returns>The bounds.</returns>
	public RectI GetBounds()
	{
		// TODO
		return RectI.Empty;
	}
}