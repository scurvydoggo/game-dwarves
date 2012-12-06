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
        /// Creates a IVoxels instance for the given engine type.
        /// </summary>
        /// <param name="type">The terrain engine type.</param>
        /// <param name="chunkWidth">The chunk width.</param>
        /// <param name="chunkHeight">The chunk height.</param>
        /// <param name="chunkDepth">The chunk depth.</param>
        /// <param name="scale">The scaling ratio.</param>
        /// <returns>The IVoxels instance.</returns>
        public IVoxels CreateVoxels(TerrainEngineType type, int chunkWidth, int chunkHeight, int chunkDepth, int scale)
        {
            switch (type)
            {
                case TerrainEngineType.Standard:
                    return new StandardVoxels(chunkWidth, chunkHeight, chunkDepth, scale);

                default:
                    throw new InvalidOperationException("Unexpected terrain engine type: " + type);
            }
        }
    }
}