// ----------------------------------------------------------------------------
// <copyright file="TerrainMeshBuilder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;

    /// <summary>
    /// Builds meshes for the terrain.
    /// </summary>
    public class TerrainMeshBuilder
    {
        /// <summary>
        /// The cache which is re-used when creating meshes.
        /// </summary>
        private CachedIndices cache;

        /// <summary>
        /// Initialises a new instance of the TerrainMeshBuilder class.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        public TerrainMeshBuilder(VoxelTerrain terrain)
        {
            this.Terrain = terrain;
            this.cache = new CachedIndices(this.Terrain.ChunkWidth, this.Terrain.ChunkWidth);
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
            for (int x = chunkOrigin.X; x < chunkOrigin.X + this.Terrain.ChunkWidth; x++)
            {
                for (int y = chunkOrigin.Y; y < chunkOrigin.Y + this.Terrain.ChunkHeight; y++)
                {
                    this.CreateMeshCell(chunk, new Vector2I(x, y), mesh);
                }
            }

            return mesh;
        }

        /// <summary>
        /// Create the cell at the given position.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <param name="position">The position.</param>
        /// <param name="mesh">The mesh data.</param>
        private void CreateMeshCell(Vector2I chunk, Vector2I position, MeshData mesh)
        {
            // Get the corner points in chunk coordinates
            var corner = this.Terrain.WorldToChunk(position.X, position.Y);
            var cornerUp = this.Terrain.WorldToChunk(position.X, position.Y + 1);
            var cornerRight = this.Terrain.WorldToChunk(position.X + 1, position.Y);
            var cornerUpRight = this.Terrain.WorldToChunk(position.X + 1, position.Y + 1);

            // Get the chunk indices for the corner points
            Vector2I chunkUp = chunk;
            Vector2I chunkRight = chunk;
            Vector2I chunkUpRight = chunk;
            if (cornerUp.Y == this.Terrain.ChunkHeight - 1)
            {
                chunkUp.Y++;
                chunkUpRight.Y++;
            }

            if (cornerRight.X == 0)
            {
                chunkRight.X++;
                chunkUpRight.X++;
            }

            // Get the voxels for the corner points
            //Voxel voxel = terrain.Voxels[chunk][TerrainConst.VoxelIndex(corner)];
            //Voxel voxelUp = terrain.Voxels[chunkUp][TerrainConst.VoxelIndex(cornerUp)];
            //Voxel voxelRight = terrain.Voxels[chunkRight][TerrainConst.VoxelIndex(cornerRight)];
            //Voxel voxelUpRight = terrain.Voxels[chunkUpRight][TerrainConst.VoxelIndex(cornerUpRight)];

            // TODO
        }
    }
}