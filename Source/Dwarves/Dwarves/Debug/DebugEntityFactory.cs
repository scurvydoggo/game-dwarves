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
    using FarseerPhysics.Factories;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Factory for creating commonly used entities.
    /// </summary>
    public class DebugEntityFactory
    {
        /// <summary>
        /// Create a camera entity.
        /// </summary>
        /// <param name="world">The world context.</param>
        /// <param name="centerX">The X point for the center of the camera.</param>
        /// <param name="centerY">The Y point for the center of the camera.</param>
        /// <param name="zoom">The zoom ratio of the camera.</param>
        /// <returns>The entity.</returns>
        public Entity CreateCamera(WorldContext world, float centerX, float centerY, float zoom)
        {
            Entity entity = world.EntityManager.CreateEntity();

            // Create the components
            var camera = new CameraComponent();
            var position = new PositionComponent(new Vector2(centerX, centerY));
            var scale = new ScaleComponent(zoom);

            // Add the components
            world.EntityManager.AddComponent(entity, camera);
            world.EntityManager.AddComponent(entity, position);
            world.EntityManager.AddComponent(entity, scale);

            return entity;
        }

        /// <summary>
        /// Create a crate entity.
        /// </summary>
        /// <param name="world">The world context.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The entity.</returns>
        public Entity CreateCrate(WorldContext world, float x, float y)
        {
            Entity entity = world.EntityManager.CreateEntity();

            // Create the physics body
            var body = this.CreateRectangleBody(entity, world.Physics, 2, 2, 1.0f, false);

            // Create the components
            var position = new PositionComponent(body, new Vector2(x, y));
            var physics = new PhysicsComponent(body);

            // Add the components
            world.EntityManager.AddComponent(entity, position);
            world.EntityManager.AddComponent(entity, physics);

            return entity;
        }

        /// <summary>
        /// Create a rectangle body.
        /// </summary>
        /// <param name="entity">The entity that this body belongs to.</param>
        /// <param name="world">The physics world.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="density">The density of the rectangle.</param>
        /// <param name="isStatic">Indicates whether this is a static body.</param>
        /// <returns>The body.</returns>
        private Body CreateRectangleBody(
            Entity entity, World world, float width, float height, float density, bool isStatic)
        {
            var body = new Body(world);
            body.UserData = entity;

            // Create the fixture
            Fixture fixture = FixtureFactory.AttachRectangle(width, height, density, new Vector2(0, 0), body);
            fixture.UserData = entity;

            // Set the body type
            body.IsStatic = isStatic;

            return body;
        }
    }
}
#endif