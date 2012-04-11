// ----------------------------------------------------------------------------
// <copyright file="WorldContext.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves
{
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
        public WorldContext(EntityManager entityManager, World physics, ResourceManager resources)
        {
            this.EntityManager = entityManager;
            this.Physics = physics;
            this.Resources = resources;
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
    }
}