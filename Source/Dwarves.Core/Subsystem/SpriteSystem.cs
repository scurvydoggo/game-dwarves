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
            Matrix view = this.CreateViewMatrix();

            // Draw the sprites in a single batch
            using (var spriteBatch = new SpriteBatch(this.graphics))
            {
                // Begin the sprite batch
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, view);

                // Draw the terrain components
                this.DrawTerrainComponents(spriteBatch);

                // Draw the sprite components
                this.DrawSpriteComponents(spriteBatch);

                // End the sprite batch
                spriteBatch.End();
            }
        }

        #endregion

        #region Draw Methods

        /// <summary>
        /// Draw the SpriteComponent sprites.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that is being drawn.</param>
        private void DrawSpriteComponents(SpriteBatch spriteBatch)
        {
            foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(SpriteComponent)))
            {
                // Get the sprite components
                var cSprite = (SpriteComponent)this.EntityManager.GetComponent(entity, typeof(SpriteComponent));
                var cPosition =
                    (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));

                // Draw the sprite
                spriteBatch.Draw(
                    this.resources.SpriteSheet,
                    cPosition.Position * new Vector2(1, -1),
                    this.resources.GetSpriteRectangle(cSprite.SpriteName),
                    Color.White,
                    -cPosition.Rotation,
                    Vector2.Zero,
                    new Vector2(Const.PixelsToMeters),
                    SpriteEffects.None,
                    0);
            }
        }

        /// <summary>
        /// Draw the TerrainComponent sprites.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that is being drawn.</param>
        private void DrawTerrainComponents(SpriteBatch spriteBatch)
        {
            foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(TerrainComponent)))
            {
                var cTerrain = (TerrainComponent)this.EntityManager.GetComponent(entity, typeof(TerrainComponent));
                var cPosition = (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));
                var cScale = (ScaleComponent)this.EntityManager.GetComponent(entity, typeof(ScaleComponent));

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
                        (int)Math.Round(data.Bounds.X * cScale.Scale),
                        (int)Math.Round(data.Bounds.Y * cScale.Scale),
                        (int)Math.Round(data.Bounds.Length * cScale.Scale),
                        (int)Math.Round(data.Bounds.Length * cScale.Scale));

                    // Check if the terrain block intersects with the viewport
                    if (!(bounds.Right < 0 ||
                        bounds.Left > this.graphics.Viewport.Width ||
                        bounds.Bottom < 0 ||
                        bounds.Top > this.graphics.Viewport.Height))
                    {
                        // Tile the terrain within the bounds
                        this.DrawTiledTerrain(spriteBatch, terrainType, bounds);
                    }
                }
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

            // Draw test block
            spriteBatch.Draw(
                this.resources.SpriteSheet,
                bounds,
                spriteRects[1],
                Color.White);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Create the view matrix for the current camera settings.
        /// </summary>
        /// <returns>A view matrix.</returns>
        private Matrix CreateViewMatrix()
        {
            // Get the camera components
            Entity cameraEntity = this.GetCameraEntity();
            var camera =
                (CameraComponent)this.EntityManager.GetComponent(cameraEntity, typeof(CameraComponent));
            var cameraZoom =
                (ScaleComponent)this.EntityManager.GetComponent(cameraEntity, typeof(ScaleComponent));
            var cameraPos =
                (PositionComponent)this.EntityManager.GetComponent(cameraEntity, typeof(PositionComponent));

            // Calculate the camera translation and scale values
            float scaleX = ((float)this.graphics.Viewport.Width / camera.ProjectionWidth) * cameraZoom.Scale;
            float scaleY = ((float)this.graphics.Viewport.Height / camera.ProjectionHeight) * cameraZoom.Scale;
            float translateX = (float)(((float)this.graphics.Viewport.Width * 0.5) / scaleX) - cameraPos.Position.X;
            float translateY = (float)(((float)this.graphics.Viewport.Height * 0.5) / scaleY) + cameraPos.Position.Y;

            // Create the matrix
            return
                Matrix.Identity *
                Matrix.CreateTranslation(translateX, translateY, 0) *
                Matrix.CreateScale(scaleX, scaleY, 0);
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

        #endregion
    }
}
