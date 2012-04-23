// ----------------------------------------------------------------------------
// <copyright file="SpriteSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Subsystem
{
    using System;
    using Dwarves.Common;
    using Dwarves.Component.Game;
    using Dwarves.Component.Render;
    using Dwarves.Component.Screen;
    using Dwarves.Component.Spatial;
    using Dwarves.Game.Terrain;
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
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise);

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

                    // Transform from game coordinates into screen coordinates
                    Vector2 screenPos;
                    Vector2 screenScale;
                    if (spritePos.IsScreenCoordinates)
                    {
                        screenPos = spritePos.Position;
                        screenScale = new Vector2(1.0f);
                    }
                    else
                    {
                        float x = spritePos.Position.X;
                        float y = spritePos.Position.Y;
                        float screenScaleX = (float)this.graphics.Viewport.Width / cameraComponent.ProjectionWidth;
                        float screenScaleY = (float)this.graphics.Viewport.Height / cameraComponent.ProjectionHeight;

                        // Calculate the screen scale vector
                        screenScale = new Vector2(screenScaleX, screenScaleY) * cameraZoom.Scale;

                        // Transform from game coordinates to screen coordinates
                        x = (x - cameraPos.Position.X) * screenScale.X;
                        y = (cameraPos.Position.Y - y) * screenScale.Y;

                        // Offset from the camera which points to the center of the viewport
                        x += ((float)this.graphics.Viewport.Width) / 2;
                        y += ((float)this.graphics.Viewport.Height) / 2;

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
                        screenScale * Const.PixelsToMeters,
                        SpriteEffects.None,
                        0);
                }

                // TODO: Remove this!
                // Test code to ensure terrain is built correctly
                foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(TerrainComponent)))
                {
                    var terrain =
                        (TerrainComponent)this.EntityManager.GetComponent(entity, typeof(TerrainComponent));
                    var terrainPos =
                        (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));
                    var terrainScale =
                        (ScaleComponent)this.EntityManager.GetComponent(entity, typeof(ScaleComponent));

                    // Calculate the position and scaling ratio for converting world-coordinates to screen-coordinates
                    Vector2 basePosition;
                    Vector2 baseScale;
                    if (terrainPos.IsScreenCoordinates)
                    {
                        basePosition = terrainPos.Position;
                        baseScale = new Vector2(1.0f);
                    }
                    else
                    {
                        float x = terrainPos.Position.X;
                        float y = terrainPos.Position.Y;
                        float screenScaleX = (float)this.graphics.Viewport.Width / cameraComponent.ProjectionWidth;
                        float screenScaleY = (float)this.graphics.Viewport.Height / cameraComponent.ProjectionHeight;

                        // Calculate the base scale vector
                        baseScale = new Vector2(screenScaleX, screenScaleY) * cameraZoom.Scale;

                        // Transform from game coordinates to camera coordinates
                        x = (x - cameraPos.Position.X) * baseScale.X;
                        y = (cameraPos.Position.Y - y) * baseScale.Y;

                        // Offset from the camera which points to the center of the viewport
                        x += ((float)this.graphics.Viewport.Width) / 2;
                        y += ((float)this.graphics.Viewport.Height) / 2;

                        basePosition = new Vector2(x, y);
                    }

                    // Apply the terrain scaling ratio
                    baseScale *= terrainScale.Scale * Const.PixelsToMeters;

                    // Step through each terrain block
                    foreach (QuadTreeData<TerrainType> data in terrain.QuadTree)
                    {
                        TerrainType terrainType = data.Data;
                        Square bounds = data.Bounds;

                        // Don't draw anything if no terrain exists here
                        if (terrainType == TerrainType.None)
                        {
                            continue;
                        }

                        // Calculate the terrain block size
                        int width = (int)Math.Round(bounds.Length * baseScale.X);
                        int height = (int)Math.Round(bounds.Length * baseScale.Y);
                        if (width > 0 && height > 0)
                        {
                            // Translate and scale the block position
                            Vector2 position =
                                basePosition + new Vector2(bounds.X * baseScale.X, bounds.Y * baseScale.Y);
                            if (position.X + width + 1 <= this.graphics.Viewport.Width &&
                                position.Y + height + 1 <= this.graphics.Viewport.Height)
                            {
                                var texture = new Texture2D(this.graphics, width, height);

                                // Fill with green for test
                                var textureData = new Color[width * height];
                                for (int i = 0; i < textureData.Length; i++)
                                {
                                    textureData[i] = Color.PaleGreen;
                                }

                                // Set the texture data
                                texture.SetData(textureData);

                                // Draw the square
                                spriteBatch.Draw(texture, position, Color.White);
                            }
                        }
                    }
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
