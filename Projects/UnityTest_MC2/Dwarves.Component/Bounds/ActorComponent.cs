// ----------------------------------------------------------------------------
// <copyright file="ActorComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Bounds
{
    using System;
    using Dwarves.Core;
    using Dwarves.Core.Bounds;
    using Dwarves.Core.Math;
    using Dwarves.Core.Terrain;
    using UnityEngine;

    /// <summary>
    /// Indicates the method of obtaining an actor's bounds.
    /// </summary>
    public enum ActorBoundsType
    {
        /// <summary>
        /// The bounds are determined via the camera view port.
        /// </summary>
        Camera,

        /// <summary>
        /// The bounds are determined by the actor's mesh.
        /// </summary>
        Mesh
    }

    /// <summary>
    /// Component representing an actor. An actor is an entity which interacts with the terrain and other actors in
    /// the game world.
    /// </summary>
    public class ActorComponent : MonoBehaviour
    {
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
        public RectangleI GetChunkBounds()
        {
            RectangleI worldBounds = this.getBounds.GetChunkBounds(this.gameObject);

            // Get the chunk coordinates for the world bounds
            Vector2I topCoords = TerrainConst.GetChunkIndex(worldBounds.X, worldBounds.Y);
            Vector2I bottomCoords = TerrainConst.GetChunkIndex(worldBounds.Right - 1, worldBounds.Bottom - 1);

            // Return the bounds in chunk coordinates
            return new RectangleI(
                topCoords.X,
                topCoords.Y,
                bottomCoords.X - topCoords.X + 1,
                topCoords.Y - bottomCoords.Y + 1);
        }
    }
}