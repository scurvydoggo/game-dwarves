// ----------------------------------------------------------------------------
// <copyright file="PhysicsSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Subsystem
{
    using EntitySystem;
    using EntitySystem.Subsystem;
    using FarseerPhysics.Dynamics;

    /// <summary>
    /// System responsible for processing Physics related components.
    /// </summary>
    public class PhysicsSystem : BaseSystem
    {
        /// <summary>
        /// The physics world.
        /// </summary>
        private World world;

        /// <summary>
        /// Initializes a new instance of the PhysicsSystem class.
        /// </summary>
        /// <param name="entityManager">The EntityManager for the world that this system belongs to.</param>
        /// <param name="world">The physics world.</param>
        public PhysicsSystem(EntityManager entityManager, World world)
            : base(entityManager)
        {
            this.world = world;
        }

        /// <summary>
        /// Perform the system's processing.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public override void Process(int delta)
        {
            // Physics world steps by seconds
            float seconds = (float)delta / 1000;

            // Step the physics world
            this.world.Step(seconds);
        }
    }
}