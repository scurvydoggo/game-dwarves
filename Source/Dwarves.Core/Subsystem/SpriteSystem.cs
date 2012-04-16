// ----------------------------------------------------------------------------
// <copyright file="SpriteSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Subsystem
{
    using System;
    using Dwarves.Component.Render;
    using Dwarves.Component.Screen;
    using Dwarves.Component.Spatial;
    using EntitySystem;
    using EntitySystem.Subsystem;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// System responsible for rending sprites.
    /// </summary>
    public class SpriteSystem : BaseSystem
    {
        /// <summary>
        /// The game resources.
        /// </summary>
        private ResourceManager resources;

        /// <summary>
        /// The graphics device.
        /// </summary>
        private GraphicsDevice graphics;

        /// <summary>
        /// Initializes a new instance of the SpriteSystem class.
        /// </summary>
        /// <param name="entityManager">The EntityManager for the world that this system belongs to.</param>
        /// <param name="resources">The game resources.</param>
        /// <param name="graphics">The graphics device.</param>
        public SpriteSystem(EntityManager entityManager, ResourceManager resources, GraphicsDevice graphics)
            : base(entityManager)
        {
            this.resources = resources;
            this.graphics = graphics;
        }

        /// <summary>
        /// Perform the system's processing.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public override void Process(int delta)
        {
            // Get the camera components, since this needs to be taken into account with determining location
            Entity cameraEntity = this.GetCameraEntity();
            var cameraComponent =
                (CameraComponent)this.EntityManager.GetComponent(cameraEntity, typeof(CameraComponent));
            var cameraZoom =
                (ScaleComponent)this.EntityManager.GetComponent(cameraEntity, typeof(ScaleComponent));
            var cameraPos =
                (PositionComponent)this.EntityManager.GetComponent(cameraEntity, typeof(PositionComponent));

            // Draw the sprites in a single batch
            using (var spriteBatch = new SpriteBatch(this.graphics))
            {
                spriteBatch.Begin();

                foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(SpriteComponent)))
                {
                    // Get the sprite components
                    var sprite = (SpriteComponent)this.EntityManager.GetComponent(entity, typeof(SpriteComponent));
                    var spritePos =
                        (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));
                    if (sprite == null || spritePos == null)
                    {
                        continue;
                    }

                    var spriteRectangle = this.resources.GetSpriteRectangle(sprite.SpriteName);

                    // Transform from game-coordinates into screen coordinates
                    Vector2 screenPos;
                    if (spritePos.IsScreenCoordinates)
                    {
                        screenPos = spritePos.Position;
                    }
                    else
                    {
                        float x = spritePos.Position.X;
                        float y = spritePos.Position.Y;

                        // Transform from game-coordinates to camera-coordinates
                        x = (x - cameraPos.Position.X) * cameraZoom.Scale;
                        y = (cameraPos.Position.Y - y) * cameraZoom.Scale;

                        // Transform from camera-coordinates to screen-coordinates
                        float screenScaleX = cameraComponent.ProjectionWidth / (float)this.graphics.Viewport.Width;
                        float screenScaleY = cameraComponent.ProjectionHeight / (float)this.graphics.Viewport.Height;
                        x = x / screenScaleX + ((float)this.graphics.Viewport.Width) / 2;
                        y = y / screenScaleY + ((float)this.graphics.Viewport.Height) / 2;

                        screenPos = new Vector2(x, y);
                    }

                    // Draw the sprite
                    spriteBatch.Draw(
                        this.resources.SpriteSheet,
                        screenPos,
                        this.resources.GetSpriteRectangle(sprite.SpriteName),
                        Color.White,
                        -spritePos.Rotation,
                        Vector2.Zero,
                        cameraZoom.Scale,
                        SpriteEffects.None,
                        0);
                }

                spriteBatch.End();
            }
        }

        /// <summary>
        /// Get the camera entity.
        /// </summary>
        /// <returns>The camera entity.</returns>
        private Entity GetCameraEntity()
        {
            // Get the entity and take the first item
            var enumerator = this.EntityManager.GetEntitiesWithComponent(typeof(CameraComponent)).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new ApplicationException("Camera entity does not exist.");
            }

            return enumerator.Current;
        }
    }
}
