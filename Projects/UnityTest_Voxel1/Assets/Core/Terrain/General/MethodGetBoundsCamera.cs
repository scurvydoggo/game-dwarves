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
        Ray topRay = Camera.main.ScreenPointToRay(new Vector3(0, 0, 0));
        Vector3 top = topRay.GetPoint(actor.camera.transform.position.z);
        Ray bottomRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0));
        Vector3 bottom = bottomRay.GetPoint(actor.camera.transform.position.z);

        return new RectI((int)top.x, (int)top.y, (int)(bottom.x + 0.5f), (int)(bottom.y + 0.5f));
    }
}