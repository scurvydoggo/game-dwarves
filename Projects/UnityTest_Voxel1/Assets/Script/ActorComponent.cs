// ----------------------------------------------------------------------------
// <copyright file="ActorComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

/// <summary>
/// Indicates the method of obtaining an actor's bounds.
/// </summary>
public enum ActorBoundsType
{
    /// <summary>
    /// The actor bounds are determined via the actor's camera view port.
    /// </summary>
    Camera,

    /// <summary>
    /// The actor bounds are determined by the actor's mesh.
    /// </summary>
    Mesh
}

/// <summary>
/// Component representing an actor that can move through the world. The significance of this is that the terrain must
/// be loaded wherever the actor is.
/// </summary>
public class ActorComponent : MonoBehaviour
{
    /// <summary>
    /// Indicates whether the actor requires render-related processing to be performed on the terrain chunks in which it
    /// resides.
    /// </summary>
    public bool RequiresTerrainRendering;

    /// <summary>
    /// Indicates whether the actor requires physics-related processing to be performed on the terrain chunks in which
    /// it resides.
    /// </summary>
    public bool RequiresTerrainPhysics;

    /// <summary>
    /// Indicates the method of obtaining the actor's bounds.
    /// </summary>
    public ActorBoundsType BoundsType;

    /// <summary>
    /// Provides the method of obtaining the actor's bounds.
    /// </summary>
    private IMethodGetBounds getBounds;

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        switch (this.BoundsType)
        {
            case ActorBoundsType.Camera:
                this.getBounds = new MethodGetBoundsCamera();
                break;

            case ActorBoundsType.Mesh:
                throw new NotImplementedException();

            default:
                throw new ApplicationException("Unexpected bounds type: " + this.BoundsType);
        }
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
    }

    /// <summary>
    /// Determine the bounds of this actor in chunk-coordinates.
    /// </summary>
    /// <returns>The bounds.</returns>
    public RectI GetChunkBounds()
    {
        RectI worldBounds = this.getBounds.GetBounds(this);

        // Get the chunk coordinates for the world bounds
        Vector2I topCoords = TerrainBlocks.GetChunkIndex(worldBounds.X, worldBounds.Y);
        Vector2I bottomCoords = TerrainBlocks.GetChunkIndex(worldBounds.Right - 1, worldBounds.Bottom - 1);

        // Return the bounds in chunk coordinates
        return new RectI(
            topCoords.X,
            topCoords.Y,
            bottomCoords.X - topCoords.X + 1,
            topCoords.Y - bottomCoords.Y + 1);
    }
}