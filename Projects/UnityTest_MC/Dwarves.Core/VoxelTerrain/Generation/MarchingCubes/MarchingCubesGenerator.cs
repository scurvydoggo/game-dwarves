// ----------------------------------------------------------------------------
// <copyright file="MarchingCubesGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Generation.MarchingCubes
{
    /// <summary>
    /// Generates the mesh for a chunk using the Marching Cubes algorithm.
    /// </summary>
    public class MarchingCubesGenerator : MeshGenerator
    {
        /// <summary>
        /// Update the mesh for the given voxel.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="voxel">The voxel.</param>
        /// <param name="neighbours">The neighbouring voxels.</param>
        protected override void UpdateVoxelMesh(Chunk chunk, Voxel voxel, MeshGenerator.VoxelNeighbours neighbours)
        {
            // TODO
        }
    }
}