// ----------------------------------------------------------------------------
// <copyright file="MethodGetBoundsCamera.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Implements a method of obtaining a camera's bounds.
/// </summary>
public class MethodGetBoundsCamera : IMethodGetBounds
{
    /// <summary>
    /// Get the actor's bounds in chunk-coordinates.
    /// </summary>
    /// <param name="actor">The actor.</param>
    /// <returns>The bounds.</returns>
    public RectI GetBounds(ActorComponent actor)
    {
        return new RectI(
            (int)Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x,
            (int)Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y,
            (int)Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x,
            (int)Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y);
    }
}