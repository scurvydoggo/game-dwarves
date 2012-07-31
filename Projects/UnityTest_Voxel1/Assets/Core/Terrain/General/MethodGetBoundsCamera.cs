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
    /// Get the actor's bounds in world-coordinates.
    /// </summary>
    /// <param name="actor">The actor.</param>
    /// <returns>The bounds.</returns>
    public RectI GetBounds(ActorComponent actor)
    {
        Ray bottomLeftRay = Camera.main.ViewportPointToRay(new Vector3(0, 0, 0));
        Vector3 bottomLeft = bottomLeftRay.GetPoint(Math.Abs(actor.transform.position.z));
        Ray topRightRay = Camera.main.ViewportPointToRay(new Vector3(1, 1, 0));
        Vector3 topRight = topRightRay.GetPoint(Math.Abs(actor.transform.position.z));

        return new RectI(
            (int)bottomLeft.x,
            (int)topRight.y,
            (int)(topRight.x - bottomLeft.x + 0.5f),
            (int)(topRight.y - bottomLeft.y + 0.5f));
    }
}