// ----------------------------------------------------------------------------
// <copyright file="DwarvesDataAdapter.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Data
{
    using System.Data.Common;
    using System.Reflection;
    using EntitySystem;
    using EntitySystem.Data;
    using EntitySystem.Data.Db;

    /// <summary>
    /// Handles loading and saving game data from the database for the Dwarves game.
    /// </summary>
    public class DwarvesDataAdapter : EntityDataAdapter
    {
        /// <summary>
        /// The entity data adapter.
        /// </summary>
        private DbEntityDataAdapter entityDataAdapter;

        /// <summary>
        /// Initializes a new instance of the DwarvesDataAdapter class.
        /// </summary>
        /// <param name="config">The game config.</param>
        public DwarvesDataAdapter(DwarvesConfig config)
        {
            // Create the database connection
            Assembly dbFactoryAssembly = Assembly.LoadFrom(config.DbProviderFactoryAssembly);
            var dbFactory = (DbProviderFactory)dbFactoryAssembly.CreateInstance(config.DbProviderFactoryClass);

            // Create the entity transformer
            Assembly transformerAssembly = Assembly.LoadFrom(config.EntityTransformerAssembly);
            var transformer = (IEntityTransformer)transformerAssembly.CreateInstance(config.EntityTransformerClass);

            // Create the database data adapter
            this.entityDataAdapter = new DbEntityDataAdapter(dbFactory, config.DataConnectionString, transformer);
        }

        /// <summary>
        /// Get the available levels.
        /// </summary>
        /// <returns>An array of level information.</returns>
        public override LevelInfo[] GetLevels()
        {
            return this.entityDataAdapter.GetLevels();
        }

        /// <summary>
        /// Load the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager to be populated.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was loaded.</returns>
        protected override bool DoLoadLevel(EntityManager entityManager, int levelNum)
        {
            return this.entityDataAdapter.LoadLevel(entityManager, levelNum);
        }

        /// <summary>
        /// Save the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager of the level to be saved.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was saved.</returns>
        protected override bool DoSaveLevel(EntityManager entityManager, int levelNum)
        {
            return this.entityDataAdapter.SaveLevel(entityManager, levelNum);
        }
    }
}