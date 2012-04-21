// ----------------------------------------------------------------------------
// <copyright file="DebugEntityFactory.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
#if DEBUG
namespace Dwarves.Debug
{
    using Dwarves.Assembler.Body;
    using Dwarves.Component.Screen;
    using Dwarves.Component.Spatial;
    using Dwarves.Game;
    using EntitySystem;
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
        /// Create a dwarf entity.
        /// </summary>
        /// <param name="world">The world context.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The entity.</returns>
        public Entity CreateDwarf(WorldContext world, float x, float y)
        {
            Entity entity = world.EntityManager.CreateEntity();

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
                new Vector2(x, y),                          // Body position
                new Vector2(4, 0) * Const.PixelsToMeters,   // Right-left offset
                new Vector2(-5, 11) * Const.PixelsToMeters, // Torso
                new Vector2(-2, 19) * Const.PixelsToMeters, // Head
                new Vector2(-3, 9) * Const.PixelsToMeters,  // Arm
                new Vector2(-2, 3) * Const.PixelsToMeters,  // Leg
                new Vector2(-3, 20) * Const.PixelsToMeters, // Beard
                neckJoint,                                  // Neck joint
                shoulderJoint,                              // Shoulder joint
                hipJoint);                                  // Hip joint

            var bodyAssembler = new HumanoidAssembler(world);
            bodyAssembler.AssembleBody(entity, args);

            return entity;
        }

        /// <summary>
        /// Create a terrain entity.
        /// </summary>
        /// <param name="world">The world context.</param>
        /// <param name="x">The top-left X position in world-coordinates.</param>
        /// <param name="y">The top-left Y position in world-coordinates.</param>
        /// <param name="scale">The scale ratio for the terrain.</param>
        /// <param name="terrainImageName">The name of the terrain image.</param>
        /// <returns>The entity.</returns>
        public Entity CreateTerrain(WorldContext world, float x, float y, float scale, string terrainImageName)
        {
            // Create terrain
            Entity terrainEntity = world.EntityManager.CreateEntity();

            // Load the terrain texture
            Texture2D texture = world.Resources.Load<Texture2D>(terrainImageName);

            var terrainFactory = new TerrainFactory();
            var terrain = terrainFactory.CreateTerrain(x, y, scale, texture);

            ////// Create terrain object
            ////const int PPU = 1;
            ////var terrainLowerRight = new Vector2(
            ////    (x + texture.Width) * (PPU / (Const.PixelsToMeters * scale)),
            ////    (y + texture.Height) * (PPU / (Const.PixelsToMeters * scale)));
            ////var terrain = new MSTerrain(world.Physics, new AABB(new Vector2(x, y), terrainLowerRight))
            ////{
            ////    PointsPerUnit = (int)(PPU / (Const.PixelsToMeters * scale)),
            ////    CellSize = 50,
            ////    SubCellSize = 1,
            ////    Decomposer = Decomposer.Earclip,
            ////    Iterations = 2,
            ////};

            ////terrain.Initialize();
            ////terrain.ApplyTexture(texture, new Vector2(0, 0), (c) => { return c == Color.Black; });

            ////// Add terrain component
            ////world.EntityManager.AddComponent(terrainEntity, new TerrainComponent(terrain));

            return terrainEntity;
        }

        #endregion
    }
}
#endif