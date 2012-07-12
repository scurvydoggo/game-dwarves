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
        /// The rotation of the component for IsPhysicsBased = false.
        /// </summary>
        private float rotation;

        /// <summary>
        /// The physics body. This is only set when IsPhysicsBased = true.
        /// </summary>
        private Body body;

        /// <summary>
        /// Initializes a new instance of the PositionComponent class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation in radians.</param>
        /// <param name="isScreenCoordinates">Indicates whether this position represents screen coordinates.</param>
        public PositionComponent(Vector2 position, float rotation = 0.0f, bool isScreenCoordinates = false)
        {
            this.body = null;
            this.Position = position;
            this.Rotation = rotation;
            this.IsScreenCoordinates = isScreenCoordinates;
        }

        /// <summary>
        /// Initializes a new instance of the PositionComponent class.
        /// </summary>
        /// <param name="body">The physics body whose position will be acted upon.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation in radians.</param>
        public PositionComponent(Body body, Vector2 position, float rotation = 0.0f)
        {
            this.body = body;
            this.Position = position;
            this.Rotation = rotation;
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
        /// Gets or sets the rotation in radians.
        /// </summary>
        public float Rotation
        {
            get
            {
                if (this.IsPhysicsBased)
                {
                    return this.body.Rotation;
                }
                else
                {
                    return this.rotation;
                }
            }

            set
            {
                if (this.IsPhysicsBased)
                {
                    this.body.Rotation = value;
                }
                else
                {
                    this.rotation = value;
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