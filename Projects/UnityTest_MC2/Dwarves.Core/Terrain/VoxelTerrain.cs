// ----------------------------------------------------------------------------
// <copyright file="VoxelTerrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using System.Collections.Generic;
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;

    /// <summary>
    /// Represents the terrain.
    /// </summary>
    public class VoxelTerrain
    {
        /// <summary>
        /// Mask for converting from world coordinates to chunk coordinates.
        /// </summary>
        private const int ChunkWidthMask = TerrainConst.ChunkWidth - 1;

        /// <summary>
        /// Mask for converting from world coordinates to chunk coordinates.
        /// </summary>
        private const int ChunkHeightMask = TerrainConst.ChunkHeight - 1;

        /// <summary>
        /// Initialises a new instance of the VoxelTerrain class.
        /// </summary>
        public VoxelTerrain()
        {
            this.Voxels = new Dictionary<Vector2I, Voxel[]>();
            this.Meshes = new Dictionary<Vector2I, Mesh>();
            this.SurfaceHeights = new Dictionary<int, float[]>();
        }

        /// <summary>
        /// Gets the voxel data organised by chunk.
        /// </summary>
        public Dictionary<Vector2I, Voxel[]> Voxels { get; private set; }

        /// <summary>
        /// Gets the mesh data organised by chunk.
        /// </summary>
        public Dictionary<Vector2I, Mesh> Meshes { get; private set; }

        /// <summary>
        /// Gets the surface heights for each chunk x-position.
        /// </summary>
        public Dictionary<int, float[]> SurfaceHeights { get; private set; }

        /// <summary>
        /// Get the index of the chunk at the given world coordinates.
        /// </summary>
        /// <param name="worldX">The x position.</param>
        /// <param name="worldY">The y position.</param>
        /// <returns>The chunk index.</returns>
        public static Vector2I GetChunkIndex(int worldX, int worldY)
        {
            return new Vector2I(worldX >> TerrainConst.ChunkWidthLog, worldY >> TerrainConst.ChunkHeightLog);
        }

        /// <summary>
        /// Convert the world coordinates into chunk coordinates.
        /// </summary>
        /// <param name="worldX">The x position.</param>
        /// <param name="worldY">The y position.</param>
        /// <returns>The position in chunk coordinates.</returns>
        public static Vector2I GetChunkCoordinates(int worldX, int worldY)
        {
            return new Vector2I(worldX & VoxelTerrain.ChunkWidthMask, worldY & VoxelTerrain.ChunkHeightMask);
        }
    }
}