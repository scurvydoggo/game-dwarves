﻿// ----------------------------------------------------------------------------
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
    using Dwarves.Game.Light;
    using Dwarves.Game.Path;
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
        #region Private Variables

        /// <summary>
        /// The game resources.
        /// </summary>
        private ResourceManager resources;

        /// <summary>
        /// The graphics device.
        /// </summary>
        private GraphicsDevice graphics;

        /// <summary>
        /// The lighting shader.
        /// </summary>
        private Effect lightShader;

        private BlendState blendStateAdditiveMax;

        /// <summary>
        /// A 1x1 white texture.
        /// </summary>
        private Texture2D whiteTexture;

        #endregion

        #region Constructor

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

            // Load the lighting shader
            this.lightShader = (Effect)this.resources.Load<Effect>("Shader\\LightShader");

            // Create a 1x1 white texture
            this.whiteTexture = new Texture2D(this.graphics, 1, 1);
            this.whiteTexture.SetData<Color>(new Color[] { Color.White });

            // Create the blend state for additive blending with the max blend function
            this.blendStateAdditiveMax = new BlendState()
            {
                ColorSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.Max,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
                AlphaBlendFunction = BlendFunction.Max
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Perform the system's processing.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public override void Process(int delta)
        {
            // Get the camera components
            Entity cameraEntity = this.EntityManager.GetFirstEntityWithComponent(typeof(CameraComponent));
            var cCamera =
                (CameraComponent)this.EntityManager.GetComponent(cameraEntity, typeof(CameraComponent));
            var cCameraPosition =
                (PositionComponent)this.EntityManager.GetComponent(cameraEntity, typeof(PositionComponent));
            var cCameraScale =
                (ScaleComponent)this.EntityManager.GetComponent(cameraEntity, typeof(ScaleComponent));

            // Get the terrain components
            Entity terrainEntity = this.EntityManager.GetFirstEntityWithComponent(typeof(TerrainComponent));
            var cTerrain =
                (TerrainComponent)this.EntityManager.GetComponent(terrainEntity, typeof(TerrainComponent));
            var cTerrainPosition =
                (PositionComponent)this.EntityManager.GetComponent(terrainEntity, typeof(PositionComponent));
            var cTerrainScale =
                (ScaleComponent)this.EntityManager.GetComponent(terrainEntity, typeof(ScaleComponent));

            // Calculate the camera translation and scale values
            float camScaleX =
                ((float)this.graphics.Viewport.Width / cCamera.ProjectionWidth) * cCameraScale.Scale;
            float camScaleY =
                ((float)this.graphics.Viewport.Height / cCamera.ProjectionHeight) * cCameraScale.Scale;
            float camTranslateX =
                (float)(((float)this.graphics.Viewport.Width * 0.5) / camScaleX) - cCameraPosition.Position.X;
            float camTranslateY =
                (float)(((float)this.graphics.Viewport.Height * 0.5) / camScaleY) + cCameraPosition.Position.Y;

            // Calculate the transformation matrices
            var terrainTranslation = new Vector3(
                (camTranslateX + cTerrainPosition.Position.X) / cTerrainScale.Scale,
                (camTranslateY - cTerrainPosition.Position.Y) / cTerrainScale.Scale,
                0);
            var terrainScale = new Vector3(
                camScaleX * cTerrainScale.Scale,
                camScaleY * cTerrainScale.Scale,
                0);
            Matrix terrainTransform =
                Matrix.CreateTranslation(terrainTranslation) *
                Matrix.CreateScale(terrainScale);
            Matrix spriteTransform =
                Matrix.CreateTranslation(camTranslateX, camTranslateY, 0) *
                Matrix.CreateScale(camScaleX, camScaleY, 0);

            // Get the terrain blocks that are in view
            ClipQuadTree<TerrainData>[] terrainBlocks =
                this.GetTerrainInView(cTerrain, terrainTranslation, terrainScale, 10, 10);

            using (SpriteBatch spriteBatch = new SpriteBatch(this.graphics))
            {
                using (RenderTarget2D mainTarget = this.CreateRenderTarget())
                {
                    using (RenderTarget2D lightTarget = this.CreateRenderTarget())
                    {
                        // Render the game
                        this.graphics.SetRenderTarget(mainTarget);
                        this.graphics.Clear(Color.FromNonPremultiplied(150, 200, 255, 255));
                        this.DrawGame(spriteBatch, terrainTransform, spriteTransform, terrainBlocks, cTerrain);

                        // Render the lighting
                        this.graphics.SetRenderTarget(lightTarget);
                        this.graphics.Clear(Color.Black);
                        this.DrawLighting(spriteBatch, terrainTransform, terrainBlocks);

                        // Blend the game and lighting textures
                        this.graphics.SetRenderTarget(null);
                        this.DrawBlendedLighting(spriteBatch, mainTarget, lightTarget);
                    }
                }
            }
        }

        #endregion

        private ClipQuadTree<TerrainData>[] GetTerrainInView(
            TerrainComponent cTerrain,
            Vector3 translation,
            Vector3 scale,
            int widthPadding,
            int heightPadding)
        {
            var rect = new Rectangle(
                cTerrain.Terrain.Bounds.X - (int)translation.X - widthPadding,
                cTerrain.Terrain.Bounds.Y - (int)translation.Y - heightPadding,
                (int)Math.Ceiling(this.graphics.Viewport.Width / scale.X) + widthPadding,
                (int)Math.Ceiling(this.graphics.Viewport.Height / scale.Y) + heightPadding);

            return cTerrain.Terrain.GetNodesIntersecting(rect).ToArray();
        }

        #region Render Game

        /// <summary>
        /// Draw the game.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="cameraTranslateX">The camera x translation value.</param>
        /// <param name="cameraTranslateY">The camera y translation value.</param>
        /// <param name="cameraScaleX">The camera x scale value.</param>
        /// <param name="cameraScaleY">The camera y scale value.</param>
        private void DrawGame(
            SpriteBatch spriteBatch,
            Matrix terrainTransform,
            Matrix spriteTransform,
            ClipQuadTree<TerrainData>[] terrainBlocks,
            TerrainComponent cTerrain)
        {
            // Draw the terrain
            this.DrawTerrainComponents(spriteBatch, terrainTransform, terrainBlocks, cTerrain);

            // Draw the sprites
            this.DrawSpriteComponents(spriteBatch, spriteTransform);
        }

        #region Draw Sprites

        /// <summary>
        /// Draw the SpriteComponent sprites.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="cameraTranslateX">The camera x translation value.</param>
        /// <param name="cameraTranslateY">The camera y translation value.</param>
        /// <param name="cameraScaleX">The camera x scale value.</param>
        /// <param name="cameraScaleY">The camera y scale value.</param>
        private void DrawSpriteComponents(SpriteBatch spriteBatch, Matrix transform)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);

            foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(SpriteComponent)))
            {
                // Get the sprite components
                var cSprite = (SpriteComponent)this.EntityManager.GetComponent(entity, typeof(SpriteComponent));
                var cPosition = (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));
                var cScale = (ScaleComponent)this.EntityManager.GetComponent(entity, typeof(ScaleComponent));

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

        #endregion

        #region Draw Terrain

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
            Matrix transform,
            ClipQuadTree<TerrainData>[] terrainBlocks,
            TerrainComponent cTerrain)
        {
            // Begin the sprite batch with the camera transform
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);

            foreach (ClipQuadTree<TerrainData> terrainBlock in terrainBlocks)
            {
                // Don't draw anything if no terrain exists here
                TerrainMaterial material = terrainBlock.Data.Material;
                if (material == TerrainMaterial.None)
                {
                    continue;
                }

                // Calculate the bounds of this terrain block in on-screen coordinates
                var screenBounds = new Rectangle(
                    terrainBlock.Bounds.X,
                    terrainBlock.Bounds.Y,
                    terrainBlock.Bounds.Length,
                    terrainBlock.Bounds.Length);

                // Tile the terrain within the bounds
                this.DrawTiledTerrain(spriteBatch, material, screenBounds);
            }

            // Draw fringe sprites for each terrain block
            foreach (ClipQuadTree<TerrainData> terrainBlock in terrainBlocks)
            {
                // Don't draw anything if no terrain exists here
                TerrainMaterial material = terrainBlock.Data.Material;
                if (material == TerrainMaterial.None)
                {
                    continue;
                }

                // Draw the fringe tiles
                this.DrawTerrainFringe(spriteBatch, material, terrainBlock.Bounds, cTerrain.PathNodes);
            }

            /*
            // DEBUG: Draw path components
            Texture2D debugTexture = new Texture2D(this.graphics, 1, 1);
            debugTexture.SetData<Color>(new Color[] { Color.White });
            foreach (PathComponent path in this.EntityManager.GetComponents(typeof(PathComponent)))
            {
                for (int i = 0; i < path.Nodes.Length - 1; i++)
                {
                    PathNode p1 = path.Nodes[i];
                    PathNode p2 = path.Nodes[i + 1];

                    int width = Math.Abs(p2.X - p1.X);
                    if (width == 0) width = 1;

                    int height = Math.Abs(p2.Y - p1.Y);
                    if (height == 0) height = 1;

                    spriteBatch.Draw(
                        debugTexture,
                        new Rectangle(p1.X, p1.Y, width, height),
                        (p1.Type == PathNodeType.Normal) ? Color.Red : Color.Yellow);
                }
            }
            */

            spriteBatch.End();
        }

        /// <summary>
        /// Draw a tiled terrain within the given bounds.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that is being drawn.</param>
        /// <param name="material">The terrain material.</param>
        /// <param name="scaledBounds">The bounds of the terrain block scaled by the terrain scale factor.</param>
        private void DrawTiledTerrain(SpriteBatch spriteBatch, TerrainMaterial material, Rectangle scaledBounds)
        {
            // Get the variations of this terrain material
            // TODO: Use the TerrainMaterial value, rather than just using mud here
            var spriteRects = new Dictionary<int, Rectangle>();
            foreach (int variation in this.resources.GetSpriteVariations("terrain", "earth", "mud"))
            {
                spriteRects.Add(variation, this.resources.GetSpriteRectangle("terrain", "earth", "mud", variation));
            }

            // Calculate the scaled tile size and determine the offset of the top-left corner of the tile
            int offsetX = scaledBounds.X % Const.TileSize;
            int offsetY = scaledBounds.Y % Const.TileSize;
            for (int x = scaledBounds.Left; x < scaledBounds.Right; x += Const.TileSize)
            {
                for (int y = scaledBounds.Top; y < scaledBounds.Bottom; y += Const.TileSize)
                {
                    // Calculate the top-left position of the tile
                    int tileX = x - offsetX;
                    int tileY = y - offsetY;

                    // Get a random sprite seeded by the x/y tile coordinate
                    int rectIndex = new Random(tileX ^ tileY).Next(0, spriteRects.Count);
                    Rectangle srcRect = spriteRects.ElementAt(rectIndex).Value;

                    // Clip the left bounds
                    if (tileX < scaledBounds.Left)
                    {
                        int diff = scaledBounds.Left - tileX;
                        tileX += diff;
                        srcRect.X += diff;
                        srcRect.Width -= diff;
                    }

                    // Clip the top bounds
                    if (tileY < scaledBounds.Top)
                    {
                        int diff = scaledBounds.Top - tileY;
                        tileY += diff;
                        srcRect.Y += diff;
                        srcRect.Height -= diff;
                    }

                    // Clip the right bounds
                    if (x + srcRect.Width > scaledBounds.Right)
                    {
                        srcRect.Width = scaledBounds.Right - x;
                    }

                    // Clip the bottom bounds
                    if (y + srcRect.Height > scaledBounds.Bottom)
                    {
                        srcRect.Height = scaledBounds.Bottom - y;
                    }

                    // Create the dest rectangle
                    var destRect = new Rectangle(tileX, tileY, srcRect.Width, srcRect.Height);

                    // Draw the sprite
                    spriteBatch.Draw(this.resources.SpriteSheet, destRect, srcRect, Color.White);
                }
            }
        }

        /// <summary>
        /// Draw the fringe tiles such as grass and rocks.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that is being drawn.</param>
        /// <param name="material">The terrain material.</param>
        /// <param name="terrainBounds">The bounds of the terrain block in terrain units.</param>
        /// <param name="groundNodes">The ground nodes which represents the walkable ground in the terrain on which the
        /// fringe sprites will be rendered.</param>
        private void DrawTerrainFringe(
            SpriteBatch spriteBatch,
            TerrainMaterial material,
            Square terrainBounds,
            Dictionary<Point, LinkedPathNode> groundNodes)
        {
            // Determine which segments (if any) of this terrain block are ground nodes
            var groundPoints = new List<Tuple<int, int>>();
            int groundY = terrainBounds.Y - 1;
            int currentStart = -1;
            int currentLength = 0;
            for (int x = terrainBounds.X; x < terrainBounds.Right; x++)
            {
                // Check if this point is a ground node
                if (groundNodes.ContainsKey(new Point(x, groundY)))
                {
                    // This is a ground node, so add it to the current point set
                    if (currentStart != -1)
                    {
                        currentLength++;
                    }
                    else
                    {
                        currentStart = x;
                        currentLength = 1;
                    }
                }
                else
                {
                    // This is not a ground node. If a range was being build, add it to the collection
                    if (currentStart != -1)
                    {
                        groundPoints.Add(Tuple.Create(currentStart, currentLength));
                        currentStart = -1;
                    }
                }
            }

            // Add the last x range
            if (currentStart != -1)
            {
                groundPoints.Add(Tuple.Create(currentStart, currentLength));
                currentStart = -1;
            }

            // If there are no ground points for this terrain block, there is nothing to render
            if (groundPoints.Count == 0)
            {
                return;
            }

            // Get the variations of the lower fringe sprites
            // TODO: Use the TerrainMaterial value, rather than just using mud here
            var lowerFringeRects = new Dictionary<int, Rectangle>();
            foreach (int variation in this.resources.GetSpriteVariations("terrain", "lowerfringe", "mud"))
            {
                lowerFringeRects.Add(variation, this.resources.GetSpriteRectangle("terrain", "lowerfringe", "mud", variation));
            }

            // Get the variations of the upper fringe sprites
            // TODO: Use the TerrainMaterial value, rather than just using mud here
            var upperFringeRects = new Dictionary<int, Rectangle>();
            foreach (int variation in this.resources.GetSpriteVariations("terrain", "upperfringe", "mud"))
            {
                upperFringeRects.Add(variation, this.resources.GetSpriteRectangle("terrain", "upperfringe", "mud", variation));
            }

            // Do nothing if no sprites exist for this terrain material
            if (lowerFringeRects.Count == 0 && upperFringeRects.Count == 0)
            {
                return;
            }

            // Determine the offset of the left side of the tile
            int offsetX = terrainBounds.X % Const.TileSize;

            // Draw the sprites
            foreach (Tuple<int, int> range in groundPoints)
            {
                int startX = range.Item1;
                int width = range.Item2;

                for (int x = startX; x < startX + width; x += Const.TileSize)
                {
                    // Calculate the x position of the tile
                    int tileX = x - offsetX;

                    // Get a random sprite seeded by the x/y tile coordinate
                    int lowerFringeIndex = new Random(tileX ^ terrainBounds.Y).Next(0, lowerFringeRects.Count);
                    Rectangle lowerFringeRect = lowerFringeRects.ElementAt(lowerFringeIndex).Value;
                    int upperFringeIndex = new Random(tileX ^ terrainBounds.Y).Next(0, upperFringeRects.Count);
                    Rectangle upperFringeRect = upperFringeRects.ElementAt(upperFringeIndex).Value;

                    // Clip the left bounds
                    if (tileX < startX)
                    {
                        int diff = startX - tileX;
                        tileX += diff;
                        lowerFringeRect.X += diff;
                        lowerFringeRect.Width -= diff;
                        upperFringeRect.X += diff;
                        upperFringeRect.Width -= diff;
                    }

                    // Clip the right bounds
                    if (x + lowerFringeRect.Width > startX + width)
                    {
                        int newWidth = startX + width - x;
                        lowerFringeRect.Width = newWidth;
                        upperFringeRect.Width = newWidth;
                    }

                    // Create the dest rectangles
                    var lowerFringeDestRect =
                        new Rectangle(tileX, terrainBounds.Y, lowerFringeRect.Width, lowerFringeRect.Height);
                    var upperFringeDestRect = new Rectangle(
                        tileX,
                        terrainBounds.Y - upperFringeRect.Height, // Upper fringe is above ground, so offset by its height
                        upperFringeRect.Width,
                        upperFringeRect.Height);

                    // Draw the sprites
                    spriteBatch.Draw(this.resources.SpriteSheet, lowerFringeDestRect, lowerFringeRect, Color.White);
                    spriteBatch.Draw(this.resources.SpriteSheet, upperFringeDestRect, upperFringeRect, Color.White);
                }
            }
        }

        #endregion

        #endregion

        #region Post-processing

        /// <summary>
        /// Draw the lighting.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="cameraTranslateX">The camera x translation value.</param>
        /// <param name="cameraTranslateY">The camera y translation value.</param>
        /// <param name="cameraScaleX">The camera x scale value.</param>
        /// <param name="cameraScaleY">The camera y scale value.</param>
        private void DrawLighting(SpriteBatch spriteBatch, Matrix transform, ClipQuadTree<TerrainData>[] terrainBlocks)
        {
            // TODO: Make this a configurable variable
            const int lightLength = 125;
            const int lightEdgeStep = 1;
            int halfLightLength = lightLength / 2;

            // Begin the sprite batch with the camera transform
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                this.blendStateAdditiveMax,
                null,
                null,
                null,
                null,
                transform);

            // Draw light-front sprites
            Rectangle destRect;
            Rectangle srcRect = this.resources.GetSpriteRectangle("light", "lightfront", "radial");
            foreach (ClipQuadTree<TerrainData> terrainBlock in terrainBlocks)
            {
                foreach (Edge light in terrainBlock.Data.LightFronts)
                {
                    // Draw light for the first point
                    destRect = new Rectangle(
                        light.Point1.X - halfLightLength,
                        light.Point1.Y - halfLightLength,
                        lightLength,
                        lightLength);
                    spriteBatch.Draw(this.resources.SpriteSheet, destRect, srcRect, Color.White);

                    if (light.Orientation != EdgeOrientation.Point)
                    {
                        // Draw light for the last point
                        destRect = new Rectangle(
                            light.Point2.X - halfLightLength,
                            light.Point2.Y - halfLightLength,
                            lightLength,
                            lightLength);
                        spriteBatch.Draw(this.resources.SpriteSheet, destRect, srcRect, Color.White);

                        // Draw light for points along the edge
                        if (light.Orientation == EdgeOrientation.Horizontal)
                        {
                            if (light.Point1.X < light.Point2.X)
                            {
                                for (int x = light.Point1.X; x < light.Point2.X; x += lightEdgeStep)
                                {
                                    destRect = new Rectangle(
                                        x - halfLightLength,
                                        light.Point1.Y - halfLightLength,
                                        lightLength,
                                        lightLength);
                                    spriteBatch.Draw(this.resources.SpriteSheet, destRect, srcRect, Color.White);
                                }
                            }
                            else
                            {
                                for (int x = light.Point2.X; x < light.Point1.X; x += lightEdgeStep)
                                {
                                    destRect = new Rectangle(
                                        x - halfLightLength,
                                        light.Point1.Y - halfLightLength,
                                        lightLength,
                                        lightLength);
                                    spriteBatch.Draw(this.resources.SpriteSheet, destRect, srcRect, Color.White);
                                }
                            }
                        }
                        else if (light.Orientation == EdgeOrientation.Vertical)
                        {
                            if (light.Point1.Y < light.Point2.Y)
                            {
                                for (int y = light.Point1.Y; y < light.Point2.Y; y += lightEdgeStep)
                                {
                                    destRect = new Rectangle(
                                        light.Point1.X - halfLightLength,
                                        y - halfLightLength,
                                        lightLength,
                                        lightLength);
                                    spriteBatch.Draw(this.resources.SpriteSheet, destRect, srcRect, Color.White);
                                }
                            }
                            else
                            {
                                for (int y = light.Point2.Y; y < light.Point1.Y; y += lightEdgeStep)
                                {
                                    destRect = new Rectangle(
                                        light.Point1.X - halfLightLength,
                                        y - halfLightLength,
                                        lightLength,
                                        lightLength);
                                    spriteBatch.Draw(this.resources.SpriteSheet, destRect, srcRect, Color.White);
                                }
                            }
                        }
                    }
                }
            }

            // Draw ambient light
            foreach (ClipQuadTree<TerrainData> terrainBlock in terrainBlocks)
            {
                // If this block is empty (ie. is just air), then it is fully lit 
                if (terrainBlock.Data.State == TerrainState.Empty)
                {
                    var blockBounds = new Rectangle(
                        terrainBlock.Bounds.X,
                        terrainBlock.Bounds.Y,
                        terrainBlock.Bounds.Length,
                        terrainBlock.Bounds.Length);
                    spriteBatch.Draw(this.whiteTexture, blockBounds, Color.White);
                }
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Blend the main texture with the light texture.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="mainTexture">The main texture.</param>
        /// <param name="lightTexture">The light texture.</param>
        private void DrawBlendedLighting(SpriteBatch spriteBatch, Texture2D mainTexture, Texture2D lightTexture)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // Apply the lighting shader
            this.lightShader.Parameters["LightsTexture"].SetValue(lightTexture);
            this.lightShader.CurrentTechnique.Passes[0].Apply();

            // Get the rectangle for the screen buffer
            var rectangle = new Rectangle(
                0,
                0,
                this.graphics.PresentationParameters.BackBufferWidth,
                this.graphics.PresentationParameters.BackBufferHeight);

            spriteBatch.Draw(mainTexture, rectangle, Color.White);
            spriteBatch.End();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Create a 2D render target.
        /// </summary>
        /// <returns>The render target.</returns>
        private RenderTarget2D CreateRenderTarget()
        {
            return new RenderTarget2D(
                this.graphics,
                this.graphics.PresentationParameters.BackBufferWidth,
                this.graphics.PresentationParameters.BackBufferHeight);
        }

        #endregion
    }
}