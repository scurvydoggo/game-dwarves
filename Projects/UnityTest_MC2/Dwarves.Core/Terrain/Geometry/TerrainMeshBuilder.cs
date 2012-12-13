// ----------------------------------------------------------------------------
// <copyright file="TerrainMeshBuilder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;
    using UnityEngine;

    /// <summary>
    /// Builds meshes for the terrain.
    /// </summary>
    public class TerrainMeshBuilder
    {
        /// <summary>
        /// The shared indices to keep track of while creating meshes. This instance is reused for all mesh creation.
        /// </summary>
        private SharedIndices sharedIndices;

        /// <summary>
        /// Initialises a new instance of the TerrainMeshBuilder class.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        public TerrainMeshBuilder(VoxelTerrain terrain)
        {
            this.Terrain = terrain;
            this.sharedIndices = new SharedIndices(this.Terrain.ChunkWidth, this.Terrain.ChunkWidth);
        }

        /// <summary>
        /// Gets the terrain.
        /// </summary>
        public VoxelTerrain Terrain { get; private set; }

        /// <summary>
        /// Creates a mesh for the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>The mesh.</returns>
        public MeshData CreateMesh(Vector2I chunk)
        {
            var mesh = new MeshData();

            var chunkOrigin = this.Terrain.GetChunkOrigin(chunk);
            for (int z = 0; z < this.Terrain.ChunkDepth; z++)
            {
                for (int x = chunkOrigin.X; x < chunkOrigin.X + this.Terrain.ChunkWidth; x++)
                {
                    for (int y = chunkOrigin.Y; y < chunkOrigin.Y + this.Terrain.ChunkHeight; y++)
                    {
                        this.CreateMeshCell(new Vector3I(x, y, z), mesh);
                    }
                }
            }

            return mesh;
        }

        /// <summary>
        /// Create the cell at the given position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="mesh">The mesh data.</param>
        private void CreateMeshCell(Vector3I pos, MeshData mesh)
        {
            // Get the voxels at each corner of the cell
            var corners = new Voxel[8];
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = this.Terrain.GetVoxel(pos + MarchingCubes.CornerVector[i]);
            }

            // Get the case code
            byte caseCode = MarchingCubes.GetCaseCode(corners);
            if ((caseCode ^ ((corners[7].Density >> 7) & 0xFF)) == 0)
            {
                // For these cases there is no triangulation
                return;
            }

            // Look up the geometry and vertex data for this case code
            CellGeometry geometry = MarchingCubes.CellGeometry[MarchingCubes.CellClass[caseCode]];
            ushort[] vertexData = MarchingCubes.VertexData[caseCode];

            // Calculate the mask which indicates whether vertices can be shared for a given direction
            byte directionMask = (byte)((pos.X > 0 ? 1 : 0) | ((pos.Y > 0 ? 1 : 0) << 1) | ((pos.Z > 0 ? 1 : 0) << 2));

            // Get the indices for each vertex in this cell, creating the vertices if necessary (otherwise use shared)
            var actualIndices = new ushort[geometry.Indices.Length];
            for (int i = 0; i < geometry.VertexCount; i++)
            {
                // Determine which edge this vertex lies on
                byte edge = (byte)(vertexData[i] >> 8);

                // Get the corner indices for the edge's end points
                byte cornerA = (byte)((vertexData[i] >> 4) & 0x0F);
                byte cornerB = (byte)(vertexData[i] & 0x0F);

                // Get the direction and index for reusing indices of a shared cell
                byte sharedDirection = (byte)(edge >> 4);
                byte sharedIndex = (byte)(edge & 0xF);

                // Check if a vertex should be re-used rather than creating a new one
                int actualIndex = -1;
                if (cornerB != 7 && (sharedDirection & directionMask) == sharedDirection)
                {
                    actualIndex = this.sharedIndices.GetIndexInDirection(pos, sharedDirection, sharedIndex);
                }

                // Check if a new vertex should be created
                if (actualIndex == -1)
                {
                    this.CreateVertex(pos, mesh, corners, cornerA, cornerB);
                    actualIndex = mesh.LatestVertexIndex();
                }

                // Cache this vertex index so it can be used by other cells
                if ((sharedDirection & 8) != 0)
                {
                    this.sharedIndices[pos, sharedIndex] = mesh.LatestVertexIndex();
                }

                actualIndices[i] = (ushort)actualIndex;
            }

            // Add the triangle indices to the mesh
            for (int t = 0; t < geometry.TriangleCount; t++)
            {
                // Step through the 3 vertices of this triangle
                int indexBase = t * 3;
                for (int i = 0; i < 3; i++)
                {
                    mesh.Indices.Add(actualIndices[geometry.Indices[indexBase + i]]);
                }
            }
        }

        /// <summary>
        /// Create the vertex for the given point.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="mesh">The mesh.</param>
        /// <param name="corners">The voxel data for the cell.</param>
        /// <param name="cornerA">The first corner index of the of the edge on which the vertex lies.</param>
        /// <param name="cornerB">The second corner index of the of the edge on which the vertex lies.</param>
        private void CreateVertex(Vector3I pos, MeshData mesh, Voxel[] corners, byte cornerA, byte cornerB)
        {
            // Calculate the position of the two end points between which the vertex lies
            Vector3I pA = pos + MarchingCubes.CornerVector[cornerA];
            Vector3I pB = pos + MarchingCubes.CornerVector[cornerB];

            // Interpolate the vertex position between the two end points
            byte densityA = corners[cornerA].Density;
            byte densityB = corners[cornerB].Density;
            Vector3 point = this.InterpolatePoint(pA, pB, densityA, densityB);

            // Add the vertex to the mesh
            mesh.Vertices.Add(point);
        }

        /// <summary>
        /// Interpolate between the two points.
        /// </summary>
        /// <param name="pointA">The first point.</param>
        /// <param name="pointB">The second point.</param>
        /// <param name="densityA">The first density.</param>
        /// <param name="densityB">The second density.</param>
        /// <returns>The interpolated point.</returns>
        private Vector3 InterpolatePoint(Vector3I pointA, Vector3I pointB, byte densityA, byte densityB)
        {
            // Calculate the density ratio
            int t = (densityB << 8) / (densityB - densityA);
            int u = 256 - t;

            Vector3I resultI = (pointA * t) + (pointB * u);
            return new Vector3(resultI.X, resultI.Y, resultI.Z) / 256; // Divide by 256 to normalise the result
        }
    }
}