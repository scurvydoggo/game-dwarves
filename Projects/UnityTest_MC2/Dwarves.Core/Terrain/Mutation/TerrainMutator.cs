// ----------------------------------------------------------------------------
// <copyright file="TerrainMutator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Mutation
{
    using Dwarves.Core.Math;
    using UnityEngine;

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
        public void Dig(VoxelTerrain terrain, Vector2I position, Vector2 offset)
        {
            // Get the voxel array
            Voxel[] voxels;
            if (terrain.Voxels.TryGetValue(TerrainConst.GetChunkIndex(position.X, position.Y), out voxels))
            {
                // Get the voxel
                Vector2I positionChunk = TerrainConst.GetChunkCoordinates(position.X, position.Y);
                int voxelIndex = TerrainConst.GetVoxelIndex(positionChunk.X, positionChunk.Y);
                Voxel voxel = voxels[voxelIndex];

                // Update the voxel density
                voxel.Density = TerrainConst.DensityMax;
                voxels[voxelIndex] = voxel;
            }
        }
    }
}