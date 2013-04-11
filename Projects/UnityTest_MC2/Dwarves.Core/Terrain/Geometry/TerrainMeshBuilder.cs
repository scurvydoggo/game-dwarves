// ----------------------------------------------------------------------------
// <copyright file="TerrainMeshBuilder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    using System.Collections.Generic;
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Lighting;
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

            return mesh;
        }

        /// <summary>
        /// Create the cell at the given position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="mesh">The mesh data.</param>
        private void CreateMeshCell(Vector3I pos, MeshData mesh)
        {
            // Get the voxels and light at each corner of the cell
            var corners = new TerrainVoxel[8];
            var light = new Colour?[8];
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3I cornerPos = pos + MarchingCubes.CornerVector[i];
                TerrainPoint point = this.Terrain.GetPoint(cornerPos);
                if (point != null)
                {
                    corners[i] = point.GetVoxel(cornerPos.Z);
                    light[i] = point.Light.Value;
                }
                else
                {
                    corners[i] = TerrainVoxel.CreateEmpty();
                }
            }

            // Get the case code
            byte caseCode = MarchingCubes.GetCaseCode(corners);
            if ((caseCode ^ ((corners[7].Density >> 7) & 0xFF)) == 0)
            {
                // For these cases there is no triangulation
                return;
            }
            else if ((caseCode & 0xCC) == 0xCC)
            {
                // For this case the surface is fully facing away from the camera (behind the terrain)
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
                    this.CreateVertex(pos, mesh, corners, light, cornerA, cornerB);
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
        /// <param name="light">The light data for cell corners.</param>
        /// <param name="cornerA">The first corner index of the of the edge on which the vertex lies.</param>
        /// <param name="cornerB">The second corner index of the of the edge on which the vertex lies.</param>
        private void CreateVertex(
            Vector3I pos,
            MeshData mesh,
            TerrainVoxel[] corners,
            Colour?[] light,
            byte cornerA,
            byte cornerB)
        {
            // Calculate the position of the two end points between which the vertex lies
            Vector3I pAI = pos + MarchingCubes.CornerVector[cornerA];
            Vector3I pBI = pos + MarchingCubes.CornerVector[cornerB];
            Vector3 pA = new Vector3(pAI.X, pAI.Y, pAI.Z);
            Vector3 pB = new Vector3(pBI.X, pBI.Y, pBI.Z);

            // Get the end point densities
            byte densityA = corners[cornerA].Density;
            byte densityB = corners[cornerB].Density;

            // Calculate the interpolation ratio
            float ratio = this.CalculateRatio(densityA, densityB);

            // Interpolate the vertex position between the two end points
            Vector3 point = this.InterpolatePoint(pA, pB, ratio);

            // Calculate the normals at each end point and then interpolate the two normals
            Vector3 nA = this.CalculateNormal(pAI);
            Vector3 nB = this.CalculateNormal(pBI);
            Vector3 normal = this.InterpolatePoint(nA, nB, ratio);

            // Interpolate the color value between the two end points
            Colour? colorA = light[cornerA];
            Colour? colorB = light[cornerB];
            Color color = this.InterpolateColor(colorA, colorB, ratio);

            // Add the vertex to the mesh
            mesh.Vertices.Add(point);
            mesh.Normals.Add(normal);
            mesh.Light.Add(color);
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
        /// Calculate the interpolation ratio of where the surface lies between points A and B.
        /// </summary>
        /// <param name="densityA">The density at point A.</param>
        /// <param name="densityB">The density at point B.</param>
        /// <returns>The interpolation ratio between 0 and 1.</returns>
        private float CalculateRatio(byte densityA, byte densityB)
        {
            if (TerrainVoxel.DensitySurface - densityA == 0)
            {
                return 0;
            }
            else if (TerrainVoxel.DensitySurface - densityB == 0)
            {
                return 1;
            }
            else if (densityA - densityB == 0)
            {
                return 0;
            }
            else
            {
                return (float)(TerrainVoxel.DensitySurface - densityA) / (densityB - densityA);
            }
        }

        /// <summary>
        /// Interpolate between the two points.
        /// </summary>
        /// <param name="pointA">The first point.</param>
        /// <param name="pointB">The second point.</param>
        /// <param name="ratio">The interpolation value.</param>
        /// <returns>The interpolated point.</returns>
        private Vector3 InterpolatePoint(Vector3 pointA, Vector3 pointB, float ratio)
        {
            return new Vector3(
                pointA.x + (ratio * (pointB.x - pointA.x)),
                pointA.y + (ratio * (pointB.y - pointA.y)),
                pointA.z + (ratio * (pointB.z - pointA.z)));
        }

        /// <summary>
        /// Interpolate the colour between the two points.
        /// </summary>
        /// <param name="colorA">The first colour.</param>
        /// <param name="colorB">The second colour.</param>
        /// <param name="ratio">The interpolation value.</param>
        /// <returns>The interpolated colour.</returns>
        private Color InterpolateColor(Colour? colorA, Colour? colorB, float ratio)
        {
            if (colorA.HasValue)
            {
                if (colorB.HasValue)
                {
                    return this.InterpolateColor(colorA.Value, colorB.Value, ratio);
                }
                else
                {
                    return new Color(colorA.Value.R, colorA.Value.G, colorA.Value.B);
                }
            }
            else
            {
                if (colorB.HasValue)
                {
                    return new Color(colorB.Value.R, colorB.Value.G, colorB.Value.B);
                }
                else
                {
                    return Color.black;
                }
            }
        }

        /// <summary>
        /// Interpolate the colour between the two points.
        /// </summary>
        /// <param name="colorA">The first colour.</param>
        /// <param name="colorB">The second colour.</param>
        /// <param name="ratio">The interpolation value.</param>
        /// <returns>The interpolated colour.</returns>
        private Color InterpolateColor(Colour colorA, Colour colorB, float ratio)
        {
            return new Color(
                colorA.R + (ratio * (colorB.R - colorA.R)),
                colorA.G + (ratio * (colorB.G - colorA.G)),
                colorA.B + (ratio * (colorB.B - colorA.B)));
        }
    }
}