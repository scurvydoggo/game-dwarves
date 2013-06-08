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
        /// Gets an array containing the indices of the given chunk and its 8 neighbours.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The chunk index and its neighbours.</returns>
        public static Vector2I[] GetNeighbours(Vector2I chunk)
        {
            return new Vector2I[]
                {
                    chunk,
                    new Vector2I(chunk.X + 1, chunk.Y),
                    new Vector2I(chunk.X + 1, chunk.Y + 1),
                    new Vector2I(chunk.X, chunk.Y + 1),
                    new Vector2I(chunk.X - 1, chunk.Y + 1),
                    new Vector2I(chunk.X - 1, chunk.Y),
                    new Vector2I(chunk.X - 1, chunk.Y - 1),
                    new Vector2I(chunk.X, chunk.Y - 1),
                    new Vector2I(chunk.X + 1, chunk.Y - 1)
                };
        }

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
            TerrainPoint point = this.Points[pos.X, pos.Y];
            if (point != null)
            {
                return point.GetVoxel(pos.Z);
            }
            else
            {
                return TerrainVoxel.CreateEmpty();
            }
        }
    }
}