// ----------------------------------------------------------------------------
// <copyright file="PositionComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Spatial
{
    using EntitySystem.Component;
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The entity's position.
    /// </summary>
    public class PositionComponent : IComponent
    {
        /// <summary>
        /// The position of the component for IsPhysicsBased = false.
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The physics body. This is only set when IsPhysicsBased = true.
        /// </summary>
        private Body body;

        /// <summary>
        /// Initializes a new instance of the PositionComponent class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="isScreenCoordinates">Indicates whether this position represents screen coordinates.</param>
        public PositionComponent(Vector2 position, bool isScreenCoordinates = false)
        {
            this.body = null;
            this.Position = position;
            this.IsScreenCoordinates = isScreenCoordinates;
        }

        /// <summary>
        /// Initializes a new instance of the PositionComponent class.
        /// </summary>
        /// <param name="body">The physics body whose position will be acted upon.</param>
        /// <param name="position">The position.</param>
        public PositionComponent(Body body, Vector2 position)
        {
            this.body = body;
            this.Position = position;
            this.IsScreenCoordinates = false;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                if (this.IsPhysicsBased)
                {
                    return this.body.Position;
                }
                else
                {
                    return this.position;
                }
            }

            set
            {
                if (this.IsPhysicsBased)
                {
                    this.body.Position = value;
                }
                else
                {
                    this.position = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this position represents screen coordinates.
        /// </summary>
        public bool IsScreenCoordinates { get; set; }

        /// <summary>
        /// Gets a value indicating whether the position is represented by a physics body.
        /// </summary>
        public bool IsPhysicsBased
        {
            get
            {
                return this.body != null;
            }
        }
    }
}