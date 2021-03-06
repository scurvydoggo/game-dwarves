﻿// ----------------------------------------------------------------------------
// <copyright file="DebugEntityFactory.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
#if DEBUG
namespace Dwarves.Debug
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Assembler.Body;
    using Dwarves.Common;
    using Dwarves.Component.Game;
    using Dwarves.Component.Physics;
    using Dwarves.Component.Screen;
    using Dwarves.Component.Spatial;
    using Dwarves.Game.Path;
    using Dwarves.Game.Terrain;
    using EntitySystem;
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Factory for creating commonly used entities.
    /// </summary>
    public class DebugEntityFactory
    {
        #region Public Methods

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
            world.EntityManager.AddComponent(entity, new CameraComponent());
            world.EntityManager.AddComponent(entity, new PositionComponent(new Vector2(centerX, centerY)));
            world.EntityManager.AddComponent(entity, new ScaleComponent(zoom));

            return entity;
        }

        /// <summary>
        /// Create a dwarf entity.
        /// </summary>
        /// <param name="world">The world context.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The entity.</returns>
        public Entity CreateDwarf(WorldContext world, float x, float y)
        {
            Entity entity = world.EntityManager.CreateEntity();

            // Prepare the args for the humanoid assembler
            var neckJoint = new HumanoidAssemblerArgs.RevoluteJoint(
                new Vector2(0, 11) * Const.PixelsToMeters,
                true,
                -MathHelper.Pi / 4,
                MathHelper.Pi / 4,
                false,
                0);
            var shoulderJoint = new HumanoidAssemblerArgs.RevoluteJoint(
                new Vector2(-2.5f, 8) * Const.PixelsToMeters,
                true,
                -MathHelper.Pi / 4,
                MathHelper.Pi / 4,
                false,
                0);
            var hipJoint = new HumanoidAssemblerArgs.RevoluteJoint(
                new Vector2(-2, 3) * Const.PixelsToMeters,
                true,
                -MathHelper.Pi / 4,
                MathHelper.Pi / 4,
                false,
                0);
            var args = new HumanoidAssemblerArgs(
                "dwarf",
                Const.CollisionGroupDwarf,
                new Vector2(x, y),                                  // Body position
                new Vector2(4, 0) * Const.PixelsToMeters,    // Right-left offset
                new Vector2(-5, 11) * Const.PixelsToMeters,  // Torso
                new Vector2(-2, 19) * Const.PixelsToMeters,  // Head
                new Vector2(-3, 9) * Const.PixelsToMeters,   // Arm
                new Vector2(-2, 3) * Const.PixelsToMeters,   // Leg
                new Vector2(-3, 20) * Const.PixelsToMeters,  // Beard
                neckJoint,                                          // Neck joint
                shoulderJoint,                                      // Shoulder joint
                hipJoint);                                          // Hip joint

            // Assemble the body
            var bodyAssembler = new HumanoidAssembler(world);
            bodyAssembler.AssembleBody(entity, args);

            return entity;
        }

        /// <summary>
        /// Create a terrain entity from a bitmap image.
        /// </summary>
        /// <param name="world">The world context.</param>
        /// <param name="x">The top-left X position in world-coordinates.</param>
        /// <param name="y">The top-left Y position in world-coordinates.</param>
        /// <param name="scale">The scale ratio for the terrain.</param>
        /// <param name="isCollidable">Indicates whether the terrain can be collided with.</param>
        /// <param name="terrainBitmapName">The name of the terrain bitmap.</param>
        /// <param name="currentTime">The current time used as the terrain creation time.</param>
        /// <returns>The entity.</returns>
        public Entity CreateTerrain(
            WorldContext world,
            float x,
            float y,
            float scale,
            bool isCollidable,
            string terrainBitmapName,
            TimeSpan currentTime)
        {
            Entity entity = world.EntityManager.CreateEntity();

            // Load the terrain texture
            Texture2D texture = world.Resources.Load<Texture2D>(terrainBitmapName);

            // Create the terrain quad tree
            var terrainFactory = new TerrainFactory();
            ClipQuadTree<TerrainData> terrainQuadTree = terrainFactory.CreateTerrainQuadTree(texture, currentTime);

            // Build the path nodes from the terrain quad tree
            var pathBuilder = new PathBuilder(terrainQuadTree, 30, 5);
            Dictionary<Point, LinkedPathNode> pathNodes = pathBuilder.BuildPathNodes();

            // Create the terrain component
            var cTerrain = new TerrainComponent(terrainQuadTree, isCollidable, pathNodes);

            // Create the physics body. Initially this has no fixtures, as those are populated dynamically
            var body = new Body(world.Physics);
            var cPhysics = new PhysicsComponent(body);

            // Add components to entity
            world.EntityManager.AddComponent(entity, cTerrain);
            world.EntityManager.AddComponent(entity, cPhysics);
            world.EntityManager.AddComponent(entity, new PositionComponent(body, new Vector2(x, y)));
            world.EntityManager.AddComponent(entity, new ScaleComponent(scale));

            return entity;
        }

        private void CreateTestPath(WorldContext world, TerrainComponent terrain, Point start, Point goal)
        {
            var testEntity = world.EntityManager.CreateEntity();

            // Create the path finder
            var pathFinder = new PathFinder(terrain);

            // Find the nodes along the path
            PathNode[] path;
            if (pathFinder.FindPath(start, goal, 1, 1, out path))
            {
                // Create the path component
                world.EntityManager.AddComponent(testEntity, new PathComponent(path));
            }
        }

        #endregion
    }
}
#endif