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
        /// Initializes a new instance of the MarchingCubesGenerator class.
        /// </summary>
        /// <param name="isoLevel">The isolevel, which is the density at which the mesh surface lies.</param>
        public MarchingCubesGenerator(byte isoLevel)
        {
            this.IsoLevel = isoLevel;
        }

        /// <summary>
        /// Gets or sets the isolevel, which is the density at which the mesh surface lies.
        /// </summary>
        public byte IsoLevel { get; set; }
        
        /// <summary>
        /// Update the mesh for the given 2x2 square of voxels.
        /// </summary>
        /// <param name="voxelSquare">The 2x2 square of voxels surrounding the mesh to update.</param>
        protected override void UpdateVoxelMesh(VoxelSquare voxelSquare)
        {
            for (int z = 0; z < Voxel.Depth; z++)
            {
                // Get the density of each point for corners around the cube with bounds (x, y, z) to (x+1, y+1, x+1)
                // ie. this voxel represents the cube's left-bottom-front corner
            }
        }
    }
}