// ----------------------------------------------------------------------------
// <copyright file="IMethodActorGetBounds.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// Implements a method of obtaining an actor's bounds.
/// </summary>
public interface IMethodActorGetBounds
{
    /// <summary>
    /// Get the actor's bounds in chunk-coordinates.
    /// </summary>
    /// <param name="actor">The actor.</param>
    /// <returns>The bounds.</returns>
    RectI GetBounds(ActorComponent actor);
}