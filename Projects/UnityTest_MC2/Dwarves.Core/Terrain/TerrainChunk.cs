// ----------------------------------------------------------------------------
// <copyright file="TerrainChunk.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using Dwarves.Core.Math;
    using UnityEngine;

    /// <summary>
    /// A terrain chunk.
    /// </summary>
    public class TerrainChunk
    {
        /// <summary>
        /// Initialises a new instance of the TerrainChunk class.
        /// </summary>
        /// <param name="points">The points in the chunk.</param>
        /// <param name="surfacePosition">The surface position relative to this chunk.</param>
        public TerrainChunk(TerrainPoint[,] points, SurfacePosition surfacePosition)
        {
            this.Points = points;
            this.SurfacePosition = surfacePosition;
            this.RebuildRequired = false;
        }

        /// <summary>
        /// Gets the points in the chunk.
        /// </summary>
        public TerrainPoint[,] Points { get; private set; }

        /// <summary>
        /// Gets the surface position relative to this chunk.
        /// </summary>
        public SurfacePosition SurfacePosition { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mesh requires a rebuild.
        /// </summary>
        public bool RebuildRequired { get; set; }

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