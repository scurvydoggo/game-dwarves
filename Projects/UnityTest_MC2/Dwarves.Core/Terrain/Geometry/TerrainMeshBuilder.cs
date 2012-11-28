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
        /// Creates a mesh for the given chunk.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>The mesh.</returns>
        public MeshData CreateMesh(VoxelTerrain terrain, Vector2I chunk)
        {
            var chunkPos = new Vector2I(chunk.X * TerrainConst.ChunkWidth, chunk.Y * TerrainConst.ChunkHeight);
            for (int x = 0; x < TerrainConst.ChunkWidth; x++)
            {
                for (int y = 0; y < TerrainConst.ChunkHeight; y++)
                {
                    this.CreateMeshCell(chunk, new Vector2I(x, y), null /* ?? */);
                }
            }

            return null;
        }

        /// <summary>
        /// Create the cell at the given position.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <param name="position">The position within the chunk.</param>
        /// <param name="p">The mesh data.</param>
        private void CreateMeshCell(Vector2I chunk, Vector2I position, object p)
        {
            // TODO
        }
    }
}