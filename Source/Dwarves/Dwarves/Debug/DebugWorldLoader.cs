// ----------------------------------------------------------------------------
// <copyright file="DebugWorldLoader.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
#if DEBUG
namespace Dwarves.Debug
{
    using Dwarves.Component.Physics;
    using EntitySystem;
    using FarseerPhysics.Collision;
    using FarseerPhysics.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Contains code for loading hard-coded world setups to use for development and testing. In the final product, all
    /// levels will be loaded via the SQLite database, however to get the level into the db in the first place it is
    /// easier to build a world via code in this class and then save it to the database.
    /// </summary>
    public class DebugWorldLoader
    {
        /// <summary>
        /// The entity factory.
        /// </summary>
        private DebugEntityFactory entityFactory;

        /// <summary>
        /// Initializes a new instance of the DebugWorldLoader class.
        /// </summary>
        public DebugWorldLoader()
        {
            this.entityFactory = new DebugEntityFactory();
        }

        /// <summary>
        /// Load test level 1.
        /// </summary>
        /// <param name="world">The world context.</param>
        public void LoadTest1(WorldContext world)
        {
            // Create the camera entity
            this.entityFactory.CreateCamera(world, 1.0f, 1.0f, 1.0f);

            // Add a dwarf
            this.entityFactory.CreateDwarf(world, -5.0f, 1.0f);

            // Create terrain
            Entity terrainEntity = world.EntityManager.CreateEntity();

            // Create terrain object
            var terrain = new MSTerrain(world.Physics, new AABB(new Vector2(-25, -45), new Vector2(60, 30)))
                {
                    PointsPerUnit = 8,
                    CellSize = 15,
                    SubCellSize = 2,
                    Decomposer = Decomposer.Earclip,
                    Iterations = 2,
                };

            terrain.Initialize();
            Texture2D texture = world.Resources.Load<Texture2D>("Terrain\\Test1_Terrain");
            terrain.ApplyTexture(texture, new Vector2(0, 0), (c) => { return c == Color.Black; });

            // Add terrain component
            world.EntityManager.AddComponent(terrainEntity, new TerrainComponent(terrain));
        }
    }
}
#endif