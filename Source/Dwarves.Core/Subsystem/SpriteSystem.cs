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
            // Get the camera components, since this needs to be taken into account with determining location
            Entity cameraEntity = this.GetCameraEntity();
            var camera =
                (CameraComponent)this.EntityManager.GetComponent(cameraEntity, typeof(CameraComponent));
            var cameraZoom =
                (ScaleComponent)this.EntityManager.GetComponent(cameraEntity, typeof(ScaleComponent));
            var cameraPos =
                (PositionComponent)this.EntityManager.GetComponent(cameraEntity, typeof(PositionComponent));

            // Calculate the origin and scale vector for the current camera position
            var scale = new Vector2(
                (float)this.graphics.Viewport.Width / camera.ProjectionWidth * cameraZoom.Scale,
                (float)this.graphics.Viewport.Height / camera.ProjectionHeight * cameraZoom.Scale);
            var origin = new Vector2(
                (cameraPos.Position.X * scale.X) - ((float)this.graphics.Viewport.Width) / 2,
                (cameraPos.Position.Y * scale.Y) + ((float)this.graphics.Viewport.Height) / 2);

            // Draw the sprites in a single batch
            using (var spriteBatch = new SpriteBatch(this.graphics))
            {
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise);

                // Draw the sprite components
                this.DrawSpriteComponents(spriteBatch, origin, scale);

                // Draw the terrain components
                this.DrawTerrainComponents(spriteBatch, origin, scale);

                spriteBatch.End();
            }
        }

        #endregion

        #region Draw Methods

        /// <summary>
        /// Draw the SpriteComponent sprites.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that is being drawn.</param>
        /// <param name="cameraOrigin">The origin of the camera.</param>
        /// <param name="cameraScale">The scale factor of the camera.</param>
        private void DrawSpriteComponents(SpriteBatch spriteBatch, Vector2 cameraOrigin, Vector2 cameraScale)
        {
            foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(SpriteComponent)))
            {
                // Get the sprite components
                var cSprite = (SpriteComponent)this.EntityManager.GetComponent(entity, typeof(SpriteComponent));
                var cPosition =
                    (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));

                // Calculate the sprite position and scale factor
                Vector2 origin;
                Vector2 spriteScale;
                if (cPosition.IsScreenCoordinates)
                {
                    origin = cPosition.Position;
                    spriteScale = new Vector2(Const.PixelsToMeters);
                }
                else
                {
                    origin = new Vector2(
                        cPosition.Position.X * cameraScale.X - cameraOrigin.X,
                        cameraOrigin.Y - cPosition.Position.Y * cameraScale.Y);
                    spriteScale = cameraScale * Const.PixelsToMeters;
                }

                // Draw the sprite
                spriteBatch.Draw(
                    this.resources.SpriteSheet,
                    origin,
                    this.resources.GetSpriteRectangle(cSprite.SpriteName),
                    Color.White,
                    -cPosition.Rotation,
                    Vector2.Zero,
                    spriteScale,
                    SpriteEffects.None,
                    0);
            }
        }

        /// <summary>
        /// Draw the TerrainComponent sprites.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that is being drawn.</param>
        /// <param name="cameraOrigin">The origin of the camera.</param>
        /// <param name="cameraScale">The scale factor of the camera.</param>
        private void DrawTerrainComponents(SpriteBatch spriteBatch, Vector2 cameraOrigin, Vector2 cameraScale)
        {
            foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(TerrainComponent)))
            {
                var cTerrain = (TerrainComponent)this.EntityManager.GetComponent(entity, typeof(TerrainComponent));
                var cPosition = (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));
                var cScale = (ScaleComponent)this.EntityManager.GetComponent(entity, typeof(ScaleComponent));

                // Calculate the terrain origin and sprite scale factor
                Vector2 origin;
                Vector2 spriteScale;
                if (cPosition.IsScreenCoordinates)
                {
                    origin = cPosition.Position;
                    spriteScale = new Vector2(Const.PixelsToMeters);
                }
                else
                {
                    origin = new Vector2(
                        cPosition.Position.X * cameraScale.X - cameraOrigin.X,
                        cameraOrigin.Y - cPosition.Position.Y * cameraScale.Y);
                    spriteScale = cameraScale * Const.PixelsToMeters;
                }

                // Calculate the scale factor for the terrain blocks
                Vector2 blockScale = spriteScale * cScale.Scale;

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
                        (int)(origin.X + data.Bounds.X * blockScale.X + 0.5),
                        (int)(origin.Y + data.Bounds.Y * blockScale.Y + 0.5),
                        (int)(data.Bounds.Length * blockScale.X + 0.5),
                        (int)(data.Bounds.Length * blockScale.Y + 0.5));

                    // Check if the terrain block intersects with the viewport
                    if (!(bounds.Right < 0 ||
                        bounds.Left > this.graphics.Viewport.Width ||
                        bounds.Bottom < 0 ||
                        bounds.Top > this.graphics.Viewport.Height))
                    {
                        // Tile the terrain within the bounds
                        this.DrawTiledTerrain(spriteBatch, terrainType, bounds, spriteScale);
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
        /// <param name="scale">The sprite scale vector.</param>
        private void DrawTiledTerrain(SpriteBatch spriteBatch, TerrainType terrain, Rectangle bounds, Vector2 scale)
        {
            // Get the variations of this terrain type
            // TODO: Use the TerrainType value, rather than just using mud here
            var spriteNames = new Dictionary<int, string>();
            foreach (int variation in this.resources.GetSpriteVariations("terrain", "earth", "mud"))
            {
                spriteNames.Add(variation, this.resources.GetSpriteName("terrain", "earth", "mud", variation));
            }

            // Calculate the scaled tile width/height
            int tileWidth = (int)(ResourceManager.TileSize * scale.X + 0.5);
            int tileHeight = (int)(ResourceManager.TileSize * scale.Y + 0.5);

            // Calculate the tile offset
            int offsetX = bounds.X % tileWidth;
            int offsetY = bounds.Y % tileWidth;

            for (int x = bounds.X; x < bounds.Right + tileWidth; x += tileWidth)
            {
                for (int y = bounds.Y; y < bounds.Bottom + tileHeight; y += tileHeight)
                {
                    // Clip the tile rectangle, as it may go outside the bounds of the bounds
                    int clipWidth = x + tileWidth < bounds.Right ? tileWidth : bounds.Right - x + 1;
                    int clipHeight = y + tileHeight < bounds.Bottom ? tileHeight : bounds.Bottom - y + 1;
                    var tileRectangle = new Rectangle(x, y, clipWidth, clipHeight);

                    // Draw the sprite
                    spriteBatch.Draw(
                        this.resources.SpriteSheet,
                        tileRectangle,
                        this.resources.GetSpriteRectangle(spriteNames[1]),
                        Color.White);
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
