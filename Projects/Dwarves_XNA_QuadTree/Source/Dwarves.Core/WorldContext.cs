// ----------------------------------------------------------------------------
// <copyright file="WorldContext.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves
{
    using System;
    using EntitySystem;
    using FarseerPhysics.Dynamics;

    /// <summary>
    /// Maintains a reference to the key objects representing the state of the game world.
    /// </summary>
    public class WorldContext
    {
        /// <summary>
        /// Initializes a new instance of the WorldContext class.
        /// </summary>
        /// <param name="entityManager">The entity manager.</param>
        /// <param name="physics">The physics world.</param>
        /// <param name="resources">The game resources.</param>
        /// <param name="currentTime">The current game time.</param>
        public WorldContext(EntityManager entityManager, World physics, ResourceManager resources, TimeSpan currentTime)
        {
            this.EntityManager = entityManager;
            this.Physics = physics;
            this.Resources = resources;
            this.CurrentTime = currentTime;
        }

        /// <summary>
        /// Gets the entity manager.
        /// </summary>
        public EntityManager EntityManager { get; private set; }

        /// <summary>
        /// Gets the physics world.
        /// </summary>
        public World Physics { get; private set; }

        /// <summary>
        /// Gets the game resources.
        /// </summary>
        public ResourceManager Resources { get; private set; }

        /// <summary>
        /// Gets or sets the current game time.
        /// </summary>
        public TimeSpan CurrentTime { get; set; }
    }
}