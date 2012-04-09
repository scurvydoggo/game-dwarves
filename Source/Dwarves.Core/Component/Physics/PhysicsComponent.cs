// ----------------------------------------------------------------------------
// <copyright file="PhysicsComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Physics
{
    using EntitySystem.Component;
    using FarseerPhysics.Dynamics;

    /// <summary>
    /// Component for entity physics.
    /// </summary>
    public class PhysicsComponent : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the PhysicsComponent class.
        /// </summary>
        /// <param name="body">The physics body.</param>
        public PhysicsComponent(Body body)
        {
            this.Body = body;
        }

        /// <summary>
        /// Gets the physics body.
        /// </summary>
        public Body Body { get; private set; }
    }
}