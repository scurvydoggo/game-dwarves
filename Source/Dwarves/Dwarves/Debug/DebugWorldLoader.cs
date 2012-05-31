// ----------------------------------------------------------------------------
// <copyright file="DebugWorldLoader.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
#if DEBUG
namespace Dwarves.Debug
{
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
            this.entityFactory.CreateCamera(world, 1, 1, 1);

            // Add terrain
            this.entityFactory.CreateTerrain(
                world,
                -700,
                -360,
                Const.PixelsToMeters,
                true,
                "Terrain\\Test1_Terrain",
                world.CurrentTime);

            // Add a dwarf
            this.entityFactory.CreateDwarf(world, 0, 50);
        }
    }
}
#endif