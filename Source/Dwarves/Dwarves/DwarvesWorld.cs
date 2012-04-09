// ----------------------------------------------------------------------------
// <copyright file="DwarvesWorld.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves
{
    using Dwarves.Data;
    using Dwarves.Subsystem;
    using EntitySystem;
    using EntitySystem.Data;
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The Dwarves game world.
    /// </summary>
    public class DwarvesWorld
    {
        #region Private Variables

        /// <summary>
        /// The data adapter responsible for loading and saving the game content.
        /// </summary>
        private EntityDataAdapter entityDataAdapter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DwarvesWorld class.
        /// </summary>
        /// <param name="device">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public DwarvesWorld(GraphicsDevice device, ContentManager content)
        {
            // Create the entity system world
            this.EntitySystemWorld = new EntitySystemWorld();

            var updateSystems = this.EntitySystemWorld.UpdateSystemManager;
            var drawSystems = this.EntitySystemWorld.DrawSystemManager;
            var entityManager = this.EntitySystemWorld.EntityManager;

            // Create the physics world
            this.PhysicsWorld = new World(new Vector2(0.0f, -9.8f));

            // Create update systems
            updateSystems.AddSystem(new InputSystem(entityManager, device));
            updateSystems.AddSystem(new PhysicsSystem(entityManager, this.PhysicsWorld));

            // Create draw systems
            drawSystems.AddSystem(new DebugDrawSystem(entityManager, this.PhysicsWorld, device, content));

            // Create the data adapter
            DwarvesConfig config = content.Load<DwarvesConfig>("DwarvesConfig");
            this.entityDataAdapter = new DwarvesDataAdapter(config);

#if DEBUG
            ////////////////////////////
            // Development/Debug code //
            ///////////////////////////

            // Load the test level
            var debugWorldLoader = new Dwarves.Debug.DebugWorldLoader(content);
            debugWorldLoader.LoadTest1(entityManager, this.PhysicsWorld);

            // Save the test level to the database
            this.entityDataAdapter.SaveLevel(entityManager, 1);
#endif
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Entity System world.
        /// </summary>
        public EntitySystemWorld EntitySystemWorld { get; private set; }

        /// <summary>
        /// Gets the physics world.
        /// </summary>
        public World PhysicsWorld { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialise the update systems.
        /// </summary>
        private void InitSystems()
        {
        }

        #endregion
    }
}