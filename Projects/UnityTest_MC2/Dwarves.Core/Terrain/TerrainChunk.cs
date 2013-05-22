// ----------------------------------------------------------------------------
// <copyright file="TerrainChunk.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using Dwarves.Core.Math;

    /// <summary>
    /// A terrain chunk.
    /// </summary>
    public class TerrainChunk
    {
        /// <summary>
        /// Initialises a new instance of the TerrainChunk class.
        /// </summary>
        public TerrainChunk()
        {
            this.Points = new TerrainPoint[Metrics.ChunkWidth, Metrics.ChunkHeight];
            this.Mesh = new TerrainChunkMesh();
        }

        /// <summary>
        /// Gets the points in the chunk.
        /// </summary>
        public TerrainPoint[,] Points { get; private set; }

        /// <summary>
        /// Gets the surface position relative to this chunk.
        /// </summary>
        public SurfacePosition SurfacePosition { get; internal set; }

        /// <summary>
        /// Gets the mesh.
        /// </summary>
        public TerrainChunkMesh Mesh { get; private set; }

        /// <summary>
        /// Gets the point at the given chunk position.
        /// </summary>
        /// <param name="pos">The chunk position.</param>
        /// <returns>The point.</returns>
        public TerrainPoint GetPoint(Vector3I pos)
        {
            return this.Points[pos.X, pos.Y];
        }

        /// <summary>
        /// Gets the voxel at the given chunk position.
        /// </summary>
        /// <param name="pos">The chunk position.</param>
        /// <returns>The voxel.</returns>
        public TerrainVoxel GetVoxel(Vector3I pos)
        {
            return this.Points[pos.X, pos.Y].GetVoxel(pos.Z);
        }
    }
}