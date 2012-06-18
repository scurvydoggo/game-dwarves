// ----------------------------------------------------------------------------
// <copyright file="PhysicsSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Subsystem
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Common;
    using Dwarves.Component.Game;
    using Dwarves.Component.Physics;
    using Dwarves.Component.Spatial;
    using Dwarves.Game.Terrain;
    using EntitySystem;
    using EntitySystem.Subsystem;
    using FarseerPhysics.Collision;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Factories;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// System responsible for processing Physics related components.
    /// </summary>
    public class PhysicsSystem : BaseSystem
    {
        /// <summary>
        /// The physics world.
        /// </summary>
        private World world;

        /// <summary>
        /// Initializes a new instance of the PhysicsSystem class.
        /// </summary>
        /// <param name="entityManager">The EntityManager for the world that this system belongs to.</param>
        /// <param name="world">The physics world.</param>
        public PhysicsSystem(EntityManager entityManager, World world)
            : base(entityManager)
        {
            this.world = world;
        }

        /// <summary>
        /// Perform the system's processing.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public override void Process(int delta)
        {
            // Update the terrain fixtures
            this.CreateAndDestroyTerrainFixtures();

            // Step the physics world
            this.world.Step((float)delta / 1000);
        }

        /// <summary>
        /// Creates terrain fixtures that are nearby physics entities and destroys those that are no longer in range of
        /// any entities.
        /// </summary>
        private void CreateAndDestroyTerrainFixtures()
        {
            foreach (Entity terrainEntity in this.EntityManager.GetEntitiesWithComponent(typeof(TerrainComponent)))
            {
                // Get the terrain components
                var cTerrain =
                    (TerrainComponent)this.EntityManager.GetComponent(terrainEntity, typeof(TerrainComponent));
                var cTerrainPhysics =
                    (PhysicsComponent)this.EntityManager.GetComponent(terrainEntity, typeof(PhysicsComponent));
                var cTerrainPosition =
                    (PositionComponent)this.EntityManager.GetComponent(terrainEntity, typeof(PositionComponent));
                var cTerrainScale =
                    (ScaleComponent)this.EntityManager.GetComponent(terrainEntity, typeof(ScaleComponent));

                // Build the list of terrain blocks that are in range of a physics entity
                var blocksInRange = new HashSet<Square>();
                foreach (Entity physicsEntity in this.EntityManager.GetEntitiesWithComponent(typeof(PhysicsComponent)))
                {
                    var cPhysics =
                        (PhysicsComponent)this.EntityManager.GetComponent(physicsEntity, typeof(PhysicsComponent));

                    // Skip the body if it is static or has no fixtures
                    if (cPhysics.Body.IsStatic || cPhysics.Body.FixtureList.Count == 0)
                    {
                        continue;
                    }

                    // Get the bounds of body in physics-world coordinates
                    AABB bodyAABB = new AABB();
                    bool firstStep = true;
                    foreach (Fixture fixture in cPhysics.Body.FixtureList)
                    {
                        AABB fixtureBounds;
                        fixture.GetAABB(out fixtureBounds, 0);
                        if (firstStep)
                        {
                            bodyAABB = fixtureBounds;
                            firstStep = false;
                        }
                        else
                        {
                            bodyAABB.Combine(ref fixtureBounds);
                        }
                    }

                    // Translate the body AABB with the body position
                    bodyAABB.LowerBound += cPhysics.Body.Position;
                    bodyAABB.UpperBound += cPhysics.Body.Position;

                    // Add the body padding
                    bodyAABB.LowerBound -= new Vector2(Const.BodyCollisionPadding);
                    bodyAABB.UpperBound += new Vector2(Const.BodyCollisionPadding);

                    // Get the distance of the top-left point of the body relative to the terrain's top-left position
                    // Remember that the terrain quad tree goes top-left to bottom-right, so Y increases as the body is
                    // further down
                    var terrainRelativeDistance = new Vector2(
                        bodyAABB.LowerBound.X - cTerrainPosition.Position.X,
                        cTerrainPosition.Position.Y - bodyAABB.UpperBound.Y);

                    // Scale the distance by the terrain scale factor (ie. if the terrain is x2 sized, the body's
                    // relative distance should be halved)
                    terrainRelativeDistance /= cTerrainScale.Scale;

                    // Scale the length/width of the body by the terrain scale factor to get the size
                    Vector2 bodySize = (bodyAABB.UpperBound - bodyAABB.LowerBound) / cTerrainScale.Scale;

                    var bodyBounds = new Rectangle(
                        (int)terrainRelativeDistance.X,
                        (int)terrainRelativeDistance.Y,
                        (int)(Math.Abs(bodySize.X) + 0.5),
                        (int)(Math.Abs(bodySize.Y) + 0.5));

                    // Get the terrain blocks which intersect the body
                    foreach (ClipQuadTree<TerrainData> block in cTerrain.Terrain.GetNodesIntersecting(bodyBounds))
                    {
                        if (block.Data.State == TerrainState.Terrain)
                        {
                            blocksInRange.Add(block.Bounds);
                        }
                    }
                }

                // Determine which terrain fixtures which are no longer in range of any bodies
                var toRemove = new List<KeyValuePair<Square, Fixture>>();
                foreach (KeyValuePair<Square, Fixture> kvp in cTerrain.Fixtures)
                {
                    if (!blocksInRange.Contains(kvp.Key))
                    {
                        toRemove.Add(kvp);
                    }
                }

                // Remove terrain fixtures which are no longer in range of any bodies
                foreach (KeyValuePair<Square, Fixture> kvp in toRemove)
                {
                    cTerrainPhysics.Body.DestroyFixture(kvp.Value);
                    cTerrain.Fixtures.Remove(kvp.Key);
                }

                // Add new terrain fixtures
                foreach (Square block in blocksInRange)
                {
                    if (!cTerrain.Fixtures.ContainsKey(block))
                    {
                        Vector2 blockPosition = new Vector2(block.X, -block.Y) * cTerrainScale.Scale;
                        float blockSize = block.Length * cTerrainScale.Scale;
                        float halfSize = blockSize / 2;

                        // Create the fixture for this block
                        var fixture = FixtureFactory.AttachRectangle(
                            blockSize,
                            blockSize,
                            1,
                            new Vector2(blockPosition.X + halfSize, blockPosition.Y - halfSize),
                            cTerrainPhysics.Body);

                        // Add the fixture to the terrain's collection
                        cTerrain.Fixtures.Add(block, fixture);
                    }
                }
            }
        }
    }
}