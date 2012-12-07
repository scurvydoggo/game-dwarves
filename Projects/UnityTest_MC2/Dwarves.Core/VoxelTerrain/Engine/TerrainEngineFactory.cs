// ----------------------------------------------------------------------------
// <copyright file="TerrainEngineFactory.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Engine
{
    using System;

    /// <summary>
    /// Responsible for creating terrain engine components.
    /// </summary>
    public class TerrainEngineFactory
    {
        /// <summary>
        /// Initialises a new instance of the Terrain class.
        /// </summary>
        /// <param name="engine">The type of terrain engine to use.</param>
        public TerrainEngineFactory(TerrainEngineType engine)
        {
            this.Engine = engine;
        }

        /// <summary>
        /// Gets the terrain engine type.
        /// </summary>
        public TerrainEngineType Engine { get; private set; }

        /// <summary>
        /// Creates the voxel data of the given size.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>The IVoxels instance.</returns>
        public IVoxels CreateVoxels(int width, int height, int depth)
        {
            switch (this.Engine)
            {
                case TerrainEngineType.Standard:
                    return new StandardVoxels(width, height, depth);

                default:
                    throw new InvalidOperationException("Unexpected terrain engine type: " + this.Engine);
            }
        }
    }
}