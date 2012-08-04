// ----------------------------------------------------------------------------
// <copyright file="MethodGetBoundsCamera.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

/// <summary>
/// Implements a method of obtaining a camera's bounds.
/// </summary>
public class MethodGetBoundsCamera : IMethodGetBounds
{
	/// <summary>
	/// The plane at Z=0.
	/// </summary>
	private Plane planeZ;
	
	/// <summary>
	/// Initializes a new instance of the MethodGetBoundsCamera class.
	/// </summary>
	public MethodGetBoundsCamera()
	{
		this.planeZ = new Plane(Vector3.back, Vector3.zero);
	}
	
    /// <summary>
    /// Get the actor's bounds in world-coordinates.
    /// </summary>
    /// <param name="actor">The actor.</param>
    /// <returns>The bounds.</returns>
    public RectI GetBounds(ActorComponent actor)
    {
		// Cast a ray at the bottom left and top right corners of the viewport
		Ray bottomRay = Camera.main.ViewportPointToRay(new Vector3(0, 0, 0));
        Ray topRay = Camera.main.ViewportPointToRay(new Vector3(1, 1, 0));
        
		// Determine the distance to Z=0 along the ray vector (since the ray will be at an angle)
		float bottomDistance;
		float topDistance;
		if (this.planeZ.Raycast(bottomRay, out bottomDistance) && this.planeZ.Raycast(topRay, out topDistance))
		{
			// Get the position where the ray intersects the Z=0 plane
	        Vector3 bottom = bottomRay.GetPoint(Math.Abs(bottomDistance));
	        Vector3 top = topRay.GetPoint(Math.Abs(topDistance));

			return new RectI((int)bottom.x, (int)top.y, (int)(top.x - bottom.x + 0.5f), (int)(top.y - bottom.y + 0.5f));
		}
		else
		{
			return RectI.Empty;
		}		
    }
}