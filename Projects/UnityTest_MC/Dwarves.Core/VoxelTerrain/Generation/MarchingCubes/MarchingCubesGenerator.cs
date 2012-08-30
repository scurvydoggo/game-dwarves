// ----------------------------------------------------------------------------
// <copyright file="MarchingCubesGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Generation.MarchingCubes
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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
            // Create a mesh within the each 'cube' of squares along the Z-axis
            for (int z = 0; z < Voxel.Depth; z++)
            {
                Vector3[] vertices;
                int[] indices;

                // This point represents the front-lower-left corner of the cube
                var cubePos = new Vector3(voxelSquare.LowerLeft.Position.X, voxelSquare.LowerLeft.Position.Y, z);

                // Get the densities of the 8 corners of the cube at this depth
                byte d0 = voxelSquare.LowerLeft.Voxel.GetDensity(z + 1);
                byte d1 = voxelSquare.LowerRight.Voxel.GetDensity(z + 1);
                byte d2 = voxelSquare.LowerRight.Voxel.GetDensity(z);
                byte d3 = voxelSquare.LowerLeft.Voxel.GetDensity(z);
                byte d4 = voxelSquare.UpperLeft.Voxel.GetDensity(z + 1);
                byte d5 = voxelSquare.UpperRight.Voxel.GetDensity(z + 1);
                byte d6 = voxelSquare.UpperRight.Voxel.GetDensity(z);
                byte d7 = voxelSquare.UpperLeft.Voxel.GetDensity(z);

                // Get the cube index for the given corner densities.
                // This is an 8-bit bitmask with bits indicating if a corner is underneath the isolevel surface
                byte cubeIndex = MarchingCubes.GetCubeIndex(this.IsoLevel, d0, d1, d2, d3, d4, d5, d6, d7);

                // The voxel is fully inside or outside the surface if cube index is 0 or 255
                if (cubeIndex == 0 || cubeIndex == 255)
                {
                    // TODO: Clear any voxel mesh at this position and return
                    throw new NotImplementedException("Clear empty mesh not implemented!");
                }

                // Get the edge index, which indicates which edges of the cube are intersected by the isolevel surface
                // This is a 12-bit bitmask with bits indicating if an edge is intersected
                int edgeIndex = MarchingCubes.EdgeTable[cubeIndex];

                // Get the array of vertices for the cube
                vertices =
                    MarchingCubes.GetCubeVertices(cubePos, edgeIndex, this.IsoLevel, d0, d1, d2, d3, d4, d5, d6, d7);

                // Get the triangle indices
                var indiceList = new List<int>();
                for (int i = 0; MarchingCubes.TriTable[cubeIndex][i] != -1; i += 3)
                {
                    indiceList.Add(MarchingCubes.TriTable[cubeIndex][i]);
                    indiceList.Add(MarchingCubes.TriTable[cubeIndex][i + 2]);
                    indiceList.Add(MarchingCubes.TriTable[cubeIndex][i + 1]);
                }

                // Set the value of the indices array
                indices = indiceList.ToArray();

                // Create the mesh object
                throw new NotImplementedException("Create marching cubes mesh not implemented!");
            }
        }
    }
}