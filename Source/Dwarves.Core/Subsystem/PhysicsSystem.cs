// ----------------------------------------------------------------------------
// <copyright file="PhysicsSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Subsystem
{
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
            float scalePlaceholder = 1;

            foreach (Entity terrainEntity in this.EntityManager.GetEntitiesWithComponent(typeof(TerrainComponent)))
            {
                // Get the terrain components
                var cTerrain =
                    (TerrainComponent)this.EntityManager.GetComponent(terrainEntity, typeof(TerrainComponent));
                var cTerrainPhysics =
                    (PhysicsComponent)this.EntityManager.GetComponent(terrainEntity, typeof(PhysicsComponent));
                var cTerrainScale =
                    (ScaleComponent)this.EntityManager.GetComponent(terrainEntity, typeof(ScaleComponent));
                var terrainPosition = new Vector2(cTerrain.QuadTree.Bounds.X, cTerrain.QuadTree.Bounds.Y);

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

                    // Invert Y-coordinate as physics uses a different scale
                    bodyAABB.LowerBound *= new Vector2(1, -1);
                    bodyAABB.UpperBound *= new Vector2(1, -1);

                    // Transform the bounds of the body into game-world coordinates
                    Vector2 bodyPosition =
                        terrainPosition + ((bodyAABB.LowerBound - terrainPosition) / cTerrainScale.Scale);
                    Vector2 bodySize = (bodyAABB.UpperBound - bodyAABB.LowerBound) / cTerrainScale.Scale;
                    var bodyBounds = new Rectangle(
                        (int)bodyPosition.X, (int)bodyPosition.Y, (int)(bodySize.X + 0.5), (int)(bodySize.Y + 0.5));

                    // Get the terrain blocks which intersect the body
                    QuadTreeData<TerrainData>[] blocks;
                    if (cTerrain.QuadTree.GetDataIntersecting(bodyBounds, out blocks))
                    {
                        // Add each terrain block to the set
                        foreach (QuadTreeData<TerrainData> block in blocks)
                        {
                            //if (block.Data.Type != TerrainType.None)
                            //{
                            blocksInRange.Add(block.Bounds);
                            //}
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
                        Vector2 blockPosition =
                            terrainPosition + ((new Vector2(block.X, block.Y) - terrainPosition) * cTerrainScale.Scale);
                        float blockSize = block.Length * cTerrainScale.Scale;

                        // Create the fixture for this block
                        var fixture = FixtureFactory.AttachRectangle(
                            blockSize,
                            blockSize,
                            1,
                            new Vector2(blockPosition.X, blockPosition.Y - blockSize),
                            cTerrainPhysics.Body);

                        // Add the fixture to the terrain's collection
                        cTerrain.Fixtures.Add(block, fixture);
                    }
                }
            }
        }
    }
}