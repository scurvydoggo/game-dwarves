// ----------------------------------------------------------------------------
// <copyright file="TerrainMutator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Mutation
{
    using Dwarves.Core.Math;
    using UnityEngine;

    using Terrain = Dwarves.Core.VoxelTerrain.Terrain;

    /// <summary>
    /// Mutates voxel terrain.
    /// </summary>
    public class TerrainMutator
    {
        /// <summary>
        /// Dig at the given position.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="position">The position.</param>
        /// <param name="offset">The offset indicating the position inside the voxel with values between 0.0 and 1.0.
        /// </param>
        public void Dig(Terrain terrain, Vector2I position, Vector2 offset)
        {
            // Get the voxel array
            Voxel[] voxels;
            if (terrain.Voxels.TryGetValue(TerrainConst.ChunkIndex(position.X, position.Y), out voxels))
            {
                // Get the voxel
                Vector2I positionChunk = TerrainConst.WorldToChunk(position.X, position.Y);
                int voxelIndex = TerrainConst.VoxelIndex(positionChunk.X, positionChunk.Y);
                Voxel voxel = voxels[voxelIndex];

                // Update the voxel density
                voxel.Density = TerrainConst.DensityMax;
                voxels[voxelIndex] = voxel;
            }
        }
    }
}