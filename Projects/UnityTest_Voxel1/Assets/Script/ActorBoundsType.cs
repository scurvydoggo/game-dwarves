// ----------------------------------------------------------------------------
// <copyright file="ActorBoundsType.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

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