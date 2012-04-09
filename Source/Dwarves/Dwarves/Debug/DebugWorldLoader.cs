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
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
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
        /// The content manager.
        /// </summary>
        private ContentManager content;

        /// <summary>
        /// Initializes a new instance of the DebugWorldLoader class.
        /// </summary>
        /// <param name="content">The game content manager.</param>
        public DebugWorldLoader(ContentManager content)
        {
            this.entityFactory = new DebugEntityFactory();
            this.content = content;
        }

        /// <summary>
        /// Load test level 1.
        /// </summary>
        /// <param name="entityManager">The entity manager.</param>
        /// <param name="world">The physics world.</param>
        public void LoadTest1(EntityManager entityManager, World world)
        {
            // Create the camera entity
            this.entityFactory.CreateCamera(entityManager, 1.0f, 1.0f, 1.0f);

            // Add a dwarf
            this.entityFactory.CreateDwarf(entityManager, world, 0.0f, 10.0f);

            // Create terrain
            Entity terrainEntity = entityManager.CreateEntity();

            // Create terrain object
            var terrain = new MSTerrain(world, new AABB(new Vector2(-25, -45), new Vector2(60, 30)))
                {
                    PointsPerUnit = 8,
                    CellSize = 15,
                    SubCellSize = 2,
                    Decomposer = Decomposer.Earclip,
                    Iterations = 2,
                };

            terrain.Initialize();
            Texture2D texture = this.content.Load<Texture2D>("Terrain\\Test1_Terrain");
            terrain.ApplyTexture(texture, new Vector2(0, 0), (c) => { return c == Color.Black; });

            // Add terrain component
            entityManager.AddComponent(terrainEntity, new TerrainComponent(terrain));
        }
    }
}
#endif