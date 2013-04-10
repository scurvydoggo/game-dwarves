// ----------------------------------------------------------------------------
// <copyright file="TerrainMeshBuilder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    using System.Collections.Generic;
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
        public TerrainMeshBuilder(DwarfTerrain terrain)
        {
            this.Terrain = terrain;
            this.sharedIndices = new SharedIndices(this.Terrain.ChunkWidth, this.Terrain.ChunkWidth);
        }

        /// <summary>
        /// Gets the terrain.
        /// </summary>
        public DwarfTerrain Terrain { get; private set; }

        /// <summary>
        /// Creates a mesh for the given chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The mesh.</returns>
        public MeshData CreateMesh(Vector2I chunkIndex)
        {
            // Create a new mesh
            var mesh = new MeshData();

            // Clear the shared indices cache
            this.sharedIndices.Reset();

            // Create the mesh for each cell in the chunk
            var chunkOrigin = this.Terrain.GetChunkOrigin(chunkIndex);
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
            
            // For now, just set the UV coordinates as the x/y position of each vertex. This will look stretched and
            // awful for things on an angle, but for now it will do
            var uvs = new Vector2[mesh.Vertices.Count];
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                uvs[i] = new Vector2(mesh.Vertices[i].x, mesh.Vertices[i].y);
            }

            mesh.UVs = new List<Vector2>(uvs);

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
            var corners = new TerrainVoxel[8];
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
            Vector3I chunkPos = this.Terrain.WorldToChunk(pos);
            byte directionMask = (byte)((chunkPos.X > 0 ? 1 : 0) | ((chunkPos.Z > 0 ? 1 : 0) << 1) | ((chunkPos.Y > 0 ? 1 : 0) << 2));

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
                    actualIndex = this.sharedIndices.GetIndexInDirection(chunkPos, sharedDirection, sharedIndex);
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
                    this.sharedIndices[chunkPos, sharedIndex] = mesh.LatestVertexIndex();
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
        /// <param name="corners">The voxel data for cell corners.</param>
        /// <param name="cornerA">The first corner index of the of the edge on which the vertex lies.</param>
        /// <param name="cornerB">The second corner index of the of the edge on which the vertex lies.</param>
        private void CreateVertex(Vector3I pos, MeshData mesh, TerrainVoxel[] corners, byte cornerA, byte cornerB)
        {
            // Calculate the position of the two end points between which the vertex lies
            Vector3I pAI = pos + MarchingCubes.CornerVector[cornerA];
            Vector3I pBI = pos + MarchingCubes.CornerVector[cornerB];
            Vector3 pA = new Vector3(pAI.X, pAI.Y, pAI.Z);
            Vector3 pB = new Vector3(pBI.X, pBI.Y, pBI.Z);

            // Interpolate the vertex position between the two end points
            byte densityA = corners[cornerA].Density;
            byte densityB = corners[cornerB].Density;
            Vector3 point = this.InterpolatePoint(pA, pB, densityA, densityB);

            // Calculate the normals at each end point and then interpolate the two normals
            Vector3 nA = this.CalculateNormal(pAI);
            Vector3 nB = this.CalculateNormal(pBI);
            Vector3 normal = this.InterpolatePoint(nA, nB, densityA, densityB);

            // Add the vertex to the mesh
            mesh.Vertices.Add(point);
            mesh.Normals.Add(normal);
        }

        /// <summary>
        /// Calculate the normal at the given position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>The normal.</returns>
        private Vector3 CalculateNormal(Vector3I pos)
        {
            byte x0 = this.Terrain.GetVoxel(pos - Vector3I.UnitX).Density;
            byte x1 = this.Terrain.GetVoxel(pos + Vector3I.UnitX).Density;
            byte y0 = this.Terrain.GetVoxel(pos - Vector3I.UnitY).Density;
            byte y1 = this.Terrain.GetVoxel(pos + Vector3I.UnitY).Density;
            byte z0 = this.Terrain.GetVoxel(pos - Vector3I.UnitZ).Density;
            byte z1 = this.Terrain.GetVoxel(pos + Vector3I.UnitZ).Density;
            
            Vector3 normal = new Vector3((x1 - x0) * 0.5f, (y1 - y0) * 0.5f, (z1 - z0) * 0.5f);
            normal.Normalize();
            return normal;
        }

        /// <summary>
        /// Interpolate between the two points.
        /// </summary>
        /// <param name="pointA">The first point.</param>
        /// <param name="pointB">The second point.</param>
        /// <param name="densityA">The first density.</param>
        /// <param name="densityB">The second density.</param>
        /// <returns>The interpolated point.</returns>
        private Vector3 InterpolatePoint(Vector3 pointA, Vector3 pointB, byte densityA, byte densityB)
        {
            if (TerrainVoxel.DensitySurface - densityA == 0)
            {
                return pointA;
            }
            else if (TerrainVoxel.DensitySurface - densityB == 0)
            {
                return pointB;
            }
            else if (densityA - densityB == 0)
            {
                return pointA;
            }
            else
            {
                float mu = (float)(TerrainVoxel.DensitySurface - densityA) / (densityB - densityA);
                return new Vector3(
                    pointA.x + (mu * (pointB.x - pointA.x)),
                    pointA.y + (mu * (pointB.y - pointA.y)),
                    pointA.z + (mu * (pointB.z - pointA.z)));
            }
        }
    }
}