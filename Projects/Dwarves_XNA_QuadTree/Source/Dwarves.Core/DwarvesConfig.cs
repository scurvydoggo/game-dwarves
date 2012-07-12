// ----------------------------------------------------------------------------
// <copyright file="DwarvesConfig.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves
{
    /// <summary>
    /// Game configuration.
    /// </summary>
    public class DwarvesConfig
    {
        /// <summary>
        /// Gets or sets the database connection string.
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the assembly name containing the DbProviderFactory implementation.
        /// </summary>
        public string DbProviderFactoryAssembly { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified classname of the DbProviderFactory implementation.
        /// </summary>
        public string DbProviderFactoryClass { get; set; }

        /// <summary>
        /// Gets or sets the assembly name containing the entity transformer.
        /// </summary>
        public string EntityTransformerAssembly { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified classname of the entity transformer.
        /// </summary>
        public string EntityTransformerClass { get; set; }
    }
}