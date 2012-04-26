// ----------------------------------------------------------------------------
// <copyright file="SpriteSystem.cs" company="Dematic">
//     Copyright © 2009-2012 Dematic. All rights reserved.
// </copyright>
// <Summary>
// </Summary>
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// <copyright file="SpriteSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Subsystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        #region Public Methods

        /// <summary>
        /// Perform the system's processing.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public override void Process(int delta)
        {
            // Get the camera components
            Entity cameraEntity = this.GetCameraEntity();
            var cCamera =
                (CameraComponent)this.EntityManager.GetComponent(cameraEntity, typeof(CameraComponent));
            var cCameraScale =
                (ScaleSpatialComponent)this.EntityManager.GetComponent(cameraEntity, typeof(ScaleSpatialComponent));
            var cCameraPosition =
                (PositionComponent)this.EntityManager.GetComponent(cameraEntity, typeof(PositionComponent));

            // Calculate the camera translation and scale values
            float scaleX = ((float)this.graphics.Viewport.Width / cCamera.ProjectionWidth) * cCameraScale.Scale;
            float scaleY = ((float)this.graphics.Viewport.Height / cCamera.ProjectionHeight) * cCameraScale.Scale;
            float translateX =
                (float)(((float)this.graphics.Viewport.Width * 0.5) / scaleX) - cCameraPosition.Position.X;
            float translateY =
                (float)(((float)this.graphics.Viewport.Height * 0.5) / scaleY) + cCameraPosition.Position.Y;

            using (SpriteBatch spriteBatch = new SpriteBatch(this.graphics))
            {
                // Draw the terrain components
                this.DrawTerrainComponents(spriteBatch, translateX, translateY, scaleX, scaleY);

                // Draw the sprite components
                this.DrawSpriteComponents(spriteBatch, translateX, translateY, scaleX, scaleY);
            }
        }

        #endregion

        #region Draw Methods

        /// <summary>
        /// Draw the SpriteComponent sprites.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="cameraTranslateX">The camera x translation value.</param>
        /// <param name="cameraTranslateY">The camera y translation value.</param>
        /// <param name="cameraScaleX">The camera x scale value.</param>
        /// <param name="cameraScaleY">The camera y scale value.</param>
        private void DrawSpriteComponents(
            SpriteBatch spriteBatch,
            float cameraTranslateX,
            float cameraTranslateY,
            float cameraScaleX,
            float cameraScaleY)
        {
            // Create the transform matrix
            Matrix transform =
                Matrix.Identity *
                Matrix.CreateTranslation(cameraTranslateX, cameraTranslateY, 0) *
                Matrix.CreateScale(cameraScaleX, cameraScaleY, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);

            foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(SpriteComponent)))
            {
                // Get the sprite components
                var cSprite =
                    (SpriteComponent)this.EntityManager.GetComponent(entity, typeof(SpriteComponent));
                var cPosition =
                    (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));
                var cScale =
                    (ScaleRenderComponent)this.EntityManager.GetComponent(entity, typeof(ScaleRenderComponent));

                // Draw the sprite
                spriteBatch.Draw(
                    this.resources.SpriteSheet,
                    cPosition.Position * new Vector2(1, -1),
                    this.resources.GetSpriteRectangle(cSprite.SpriteName),
                    Color.White,
                    -cPosition.Rotation,
                    Vector2.Zero,
                    cScale != null ? cScale.Scale : 1,
                    SpriteEffects.None,
                    0);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Draw the TerrainComponent sprites.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="cameraTranslateX">The camera x translation value.</param>
        /// <param name="cameraTranslateY">The camera y translation value.</param>
        /// <param name="cameraScaleX">The camera x scale value.</param>
        /// <param name="cameraScaleY">The camera y scale value.</param>
        private void DrawTerrainComponents(
            SpriteBatch spriteBatch,
            float cameraTranslateX,
            float cameraTranslateY,
            float cameraScaleX,
            float cameraScaleY)
        {
            foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(TerrainComponent)))
            {
                var cTerrain =
                    (TerrainComponent)this.EntityManager.GetComponent(entity, typeof(TerrainComponent));
                var cPosition =
                    (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));
                var cScaleSpace =
                    (ScaleSpatialComponent)this.EntityManager.GetComponent(entity, typeof(ScaleSpatialComponent));
                var cScaleRender =
                    (ScaleRenderComponent)this.EntityManager.GetComponent(entity, typeof(ScaleRenderComponent));

                // Create the transform matrix
                Matrix transform =
                    Matrix.Identity *
                    Matrix.CreateTranslation(cameraTranslateX / cScaleRender.Scale, cameraTranslateY / cScaleRender.Scale, 0) *
                    Matrix.CreateTranslation(cPosition.Position.X, cPosition.Position.Y, 0) *
                    Matrix.CreateScale(cameraScaleX, cameraScaleY, 0) *
                    Matrix.CreateScale(cScaleRender.Scale, cScaleRender.Scale, 0);

                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);

                // Step through each terrain block
                foreach (QuadTreeData<TerrainType> data in cTerrain.QuadTree)
                {
                    // Don't draw anything if no terrain exists here
                    TerrainType terrainType = data.Data;
                    if (terrainType == TerrainType.None)
                    {
                        continue;
                    }

                    // Calculate the bounds of this terrain block in on-screen coordinates
                    var bounds = new Rectangle(
                        (int)Math.Round(data.Bounds.X * cScaleSpace.Scale),
                        (int)Math.Round(data.Bounds.Y * cScaleSpace.Scale),
                        (int)Math.Round(data.Bounds.Length * cScaleSpace.Scale),
                        (int)Math.Round(data.Bounds.Length * cScaleSpace.Scale));

                    // Tile the terrain within the bounds
                    this.DrawTiledTerrain(spriteBatch, terrainType, bounds);
                }

                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw a tiled terrain within the given bounds.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that is being drawn.</param>
        /// <param name="terrain">The terrain type.</param>
        /// <param name="bounds">The bounds within which terrain is tiled.</param>
        private void DrawTiledTerrain(SpriteBatch spriteBatch, TerrainType terrain, Rectangle bounds)
        {
            // Get the variations of this terrain type
            // TODO: Use the TerrainType value, rather than just using mud here
            var spriteRects = new Dictionary<int, Rectangle>();
            foreach (int variation in this.resources.GetSpriteVariations("terrain", "earth", "mud"))
            {
                string name = this.resources.GetSpriteName("terrain", "earth", "mud", variation);
                spriteRects.Add(variation, this.resources.GetSpriteRectangle(name));
            }

            // Calculate the scaled tile size and determine the offset of the top-right corner of the tile
            int offsetX = bounds.X % Const.TileSize;
            int offsetY = bounds.Y % Const.TileSize;
            for (int x = bounds.Left; x < bounds.Right; x += Const.TileSize)
            {
                for (int y = bounds.Top; y < bounds.Bottom; y += Const.TileSize)
                {
                    // Calculate the top-left position of the tile
                    int tileX = x - offsetX;
                    int tileY = y - offsetY;

                    // Get a random sprite seeded by the x/y tile coordinate
                    int rectIndex = new Random(tileX ^ tileY).Next(0, spriteRects.Count);
                    Rectangle srcRect = spriteRects.ElementAt(rectIndex).Value;

                    // Clip the left bounds
                    if (tileX < bounds.Left)
                    {
                        int diff = bounds.Left - tileX;
                        srcRect.X += diff;
                        srcRect.Width -= diff;
                        tileX += diff;
                    }

                    // Clip the top bounds
                    if (tileY < bounds.Top)
                    {
                        int diff = bounds.Top - tileY;
                        srcRect.Y += diff;
                        srcRect.Height -= diff;
                        tileY += diff;
                    }

                    // Clip the right bounds
                    if (x + srcRect.Width > bounds.Right)
                    {
                        srcRect.Width = bounds.Right - x;
                    }

                    // Clip the bottom bounds
                    if (srcRect.Height > bounds.Bottom)
                    {
                        srcRect.Height = bounds.Bottom - y;
                    }

                    // Create the dest rectangle
                    var destRect = new Rectangle(tileX, tileY, srcRect.Width, srcRect.Height);

                    // Draw the sprite
                    spriteBatch.Draw(this.resources.SpriteSheet, destRect, srcRect, Color.White);
                }
            }
        }

        #endregion

        #region Helper Methods

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

        #endregion
    }
}
