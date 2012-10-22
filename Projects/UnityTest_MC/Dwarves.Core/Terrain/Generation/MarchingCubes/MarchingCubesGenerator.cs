// ----------------------------------------------------------------------------
// <copyright file="MarchingCubesGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation.MarchingCubes
{
    using System.Collections.Generic;
    using Dwarves.Core.Mesh;
    using UnityEngine;

    /// <summary>
    /// Generates the mesh for a chunk using the Marching Cubes algorithm.
    /// </summary>
    public class MarchingCubesGenerator : ChunkMeshGenerator
    {
        /// <summary>
        /// Initialises a new instance of the MarchingCubesGenerator class.
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
        /// Update the mesh for the point surrounded by the given 2x2 square of voxels.
        /// </summary>
        /// <param name="voxelSquare">The 2x2 square of voxels surrounding the point to update.</param>
        protected override void UpdateVoxelMesh(VoxelSquare voxelSquare)
        {
            // Build the list of vertices for each 'cube' of squares along the Z-axis
            var vertexList = new List<Vector3>();
            var indiceList = new List<int>();
            int vertexOffset = 0;
            for (int z = -1; z < Voxel.Depth; z++)
            {
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

                // Check if the voxel is fully inside or outside the surface
                if (cubeIndex == 0)
                {
                    // The point is outside of the surface, so continue with the next Z-depth
                    continue;
                }
                else if (cubeIndex == 255)
                {
                    // The point is fully inside the surface, which means no need to iterate deeper
                    break;
                }

                // Get the edge index, which indicates which edges of the cube are intersected by the isolevel surface
                // This is a 12-bit bitmask with bits indicating if an edge is intersected
                int edgeIndex = MarchingCubes.EdgeTable[cubeIndex];

                // Get the array of vertices for the cube
                var lowerLeft = new Vector3(voxelSquare.WorldOrigin.X, voxelSquare.WorldOrigin.Y, z);
                Vector3[] vertices =
                    MarchingCubes.GetCubeVertices(lowerLeft, edgeIndex, this.IsoLevel, d0, d1, d2, d3, d4, d5, d6, d7);
                vertexList.AddRange(vertices);

                // Get the indices which indicate which vertices each triangle will use
                for (int i = 0; MarchingCubes.TriTable[cubeIndex][i] != -1; i += 3)
                {
                    indiceList.Add(MarchingCubes.TriTable[cubeIndex][i] + vertexOffset);
                    indiceList.Add(MarchingCubes.TriTable[cubeIndex][i + 2] + vertexOffset);
                    indiceList.Add(MarchingCubes.TriTable[cubeIndex][i + 1] + vertexOffset);
                }

                // Update the vertex offset so that indices are correct for the next Z-depth
                vertexOffset += vertices.Length;
            }

            // Create/Clear the mesh in the chunk
            if (indiceList.Count > 0)
            {
                // Create the mesh object
                var mesh = new MeshData(
                    vertexList.ToArray(),
                    indiceList.ToArray(),
                    null,
                    null,
                    null);

                // Update the chunk
                voxelSquare.LowerLeft.Chunk.Mesh.SetMesh(voxelSquare.LowerLeft.Position, mesh);
            }
            else
            {
                // The mesh is empty, so make sure it is cleared on the chunk
                voxelSquare.LowerLeft.Chunk.Mesh.RemoveMesh(voxelSquare.LowerLeft.Position);
            }
        }
    }
}