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
        /// <param name="terrain">The terrain.</param>
        /// <param name="isoLevel">The isolevel, which is the density at which the mesh surface lies.</param>
        public MarchingCubesGenerator(VoxelTerrain terrain, byte isoLevel)
            : base(terrain)
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
            for (int z = -1; z < Voxel.DrawDepth; z++)
            {
                // Get the densities of the 8 corners of the cube at this depth
                byte d0, d1, d2, d3, d4, d5, d6, d7;
                if (z == -1)
                {
                    // This cube lies is on the surface, so for the corners facing outwards use max-density to indicate
                    // air. This creates the wall of terrain that the user sees
                    d0 = (byte)(voxelSquare.LowerLeft.Voxel.Density & Voxel.DensityMax);
                    d1 = (byte)(voxelSquare.LowerRight.Voxel.Density & Voxel.DensityMax);
                    d2 = Voxel.DensityMax;
                    d3 = Voxel.DensityMax;
                    d4 = (byte)(voxelSquare.UpperLeft.Voxel.Density & Voxel.DensityMax);
                    d5 = (byte)(voxelSquare.UpperRight.Voxel.Density & Voxel.DensityMax);
                    d6 = Voxel.DensityMax;
                    d7 = Voxel.DensityMax;
                }
                else if (z >= Voxel.DigDepth - 1)
                {
                    // This cube lies at the deepest depth, so for the corners facing inwards use min-density to
                    // indicate a back wall. This creates the 'inner' wall which represents 'dug out' terrain
                    // Note: An exception is if a voxel is of the type 'Air', in which case there is no inner wall
                    d0 = (byte)(voxelSquare.LowerLeft.Voxel.Density >> 4);
                    d1 = (byte)(voxelSquare.LowerRight.Voxel.Density >> 4);
                    d4 = (byte)(voxelSquare.UpperLeft.Voxel.Density >> 4);
                    d5 = (byte)(voxelSquare.UpperRight.Voxel.Density >> 4);

                    if (z >= Voxel.DigDepth)
                    {
                        d2 = (byte)(voxelSquare.LowerRight.Voxel.Density >> 4);
                        d3 = (byte)(voxelSquare.LowerLeft.Voxel.Density >> 4);
                        d6 = (byte)(voxelSquare.UpperRight.Voxel.Density >> 4);
                        d7 = (byte)(voxelSquare.UpperLeft.Voxel.Density >> 4);
                    }
                    else
                    {
                        d2 = (byte)(voxelSquare.LowerRight.Voxel.Density & Voxel.DensityMax);
                        d3 = (byte)(voxelSquare.LowerLeft.Voxel.Density & Voxel.DensityMax);
                        d6 = (byte)(voxelSquare.UpperRight.Voxel.Density & Voxel.DensityMax);
                        d7 = (byte)(voxelSquare.UpperLeft.Voxel.Density & Voxel.DensityMax);
                    }
                }
                else
                {
                    // This cube lies between the surface and the deepest point, so each pair of inward and outward
                    // facing corners have the same density. This creates a straight wall going inwards.
                    d0 = (byte)(voxelSquare.LowerLeft.Voxel.Density & Voxel.DensityMax);
                    d1 = (byte)(voxelSquare.LowerRight.Voxel.Density & Voxel.DensityMax);
                    d2 = (byte)(voxelSquare.LowerRight.Voxel.Density & Voxel.DensityMax);
                    d3 = (byte)(voxelSquare.LowerLeft.Voxel.Density & Voxel.DensityMax);
                    d4 = (byte)(voxelSquare.UpperLeft.Voxel.Density & Voxel.DensityMax);
                    d5 = (byte)(voxelSquare.UpperRight.Voxel.Density & Voxel.DensityMax);
                    d6 = (byte)(voxelSquare.UpperRight.Voxel.Density & Voxel.DensityMax);
                    d7 = (byte)(voxelSquare.UpperLeft.Voxel.Density & Voxel.DensityMax);
                }

                // Get the cube index for the given corner densities.
                // This is an 8-bit bitmask with bits indicating if a corner is underneath the isolevel surface
                byte cubeIndex = MarchingCubes.GetCubeIndex(this.IsoLevel, (byte)d0, d1, d2, d3, d4, d5, d6, d7);

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
                // Calculate the UV coordinates
                var uvs = new Vector2[vertexList.Count];
                for (int i = 0; i < vertexList.Count; i++)
                {
                    uvs[i] = new Vector2(vertexList[i].x, vertexList[i].y);
                }

                // Create the mesh object
                var mesh = new MeshData(
                    vertexList.ToArray(),
                    indiceList.ToArray(),
                    uvs,
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