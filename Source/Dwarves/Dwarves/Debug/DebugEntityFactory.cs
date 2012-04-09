// ----------------------------------------------------------------------------
// <copyright file="DebugEntityFactory.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
#if DEBUG
namespace Dwarves.Debug
{
    using Dwarves.Component.Physics;
    using Dwarves.Component.Screen;
    using Dwarves.Component.Spatial;
    using EntitySystem;
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Factory for creating commonly used entities.
    /// </summary>
    public class DebugEntityFactory
    {
        #region Public Methods

        /// <summary>
        /// Create a camera entity.
        /// </summary>
        /// <param name="entityManager">The entity manager.</param>
        /// <param name="centerX">The X point for the center of the camera.</param>
        /// <param name="centerY">The Y point for the center of the camera.</param>
        /// <param name="zoom">The zoom ratio of the camera.</param>
        /// <returns>The entity.</returns>
        public Entity CreateCamera(EntityManager entityManager, float centerX, float centerY, float zoom)
        {
            Entity entity = entityManager.CreateEntity();

            // Create the components
            var camera = new CameraComponent();
            var position = new PositionComponent(new Vector2(centerX, centerY));
            var scale = new ScaleComponent(zoom);

            // Add the components
            entityManager.AddComponent(entity, camera);
            entityManager.AddComponent(entity, position);
            entityManager.AddComponent(entity, scale);

            return entity;
        }

        /// <summary>
        /// Create a dwarf entity.
        /// </summary>
        /// <param name="entityManager">The entity manager.</param>
        /// <param name="world">The physics world.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The entity.</returns>
        public Entity CreateDwarf(EntityManager entityManager, World world, float x, float y)
        {
            Entity entity = entityManager.CreateEntity();

            // Create the physics body
            var body = this.CreateDwarfBody(entity, world);

            // Create the components
            entityManager.AddComponent(entity, new PositionComponent(body, new Vector2(x, y)));
            entityManager.AddComponent(entity, new PhysicsComponent(body));

            return entity;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create a dwarf body.
        /// </summary>
        /// <param name="entity">The entity that this body belongs to.</param>
        /// <param name="world">The physics world.</param>
        /// <returns>The body.</returns>
        private Body CreateDwarfBody(Entity entity, World world)
        {
            var body = new Body(world);
            body.UserData = entity;
            body.IsStatic = false;

            return body;
        }

        #endregion
    }
}
#endif