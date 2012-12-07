// ----------------------------------------------------------------------------
// <copyright file="TerrainEngineFactory.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Engine
{
    using System;

    /// <summary>
    /// Responsible for creating terrain engine components.
    /// </summary>
    public class TerrainEngineFactory
    {
        /// <summary>
        /// The type of terrain engine.
        /// </summary>
        private TerrainEngineType engineType;

        /// <summary>
        /// Initialises a new instance of the TerrainEngineFactory class.
        /// </summary>
        /// <param name="engineType">The type of terrain engine.</param>
        public TerrainEngineFactory(TerrainEngineType engineType)
        {
            this.engineType = engineType;
        }

        /// <summary>
        /// Creates the voxel data of the given size.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>The IVoxels instance.</returns>
        public IVoxels CreateVoxels(int width, int height, int depth)
        {
            switch (this.engineType)
            {
                case TerrainEngineType.Standard:
                    return new StandardVoxels(width, height, depth);

                default:
                    throw new InvalidOperationException("Unexpected terrain engine type: " + this.engineType);
            }
        }
    }
}