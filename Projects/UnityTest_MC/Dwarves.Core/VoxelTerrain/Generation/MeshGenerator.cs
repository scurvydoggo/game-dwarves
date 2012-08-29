// ----------------------------------------------------------------------------
// <copyright file="MeshGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Generation
{
    /// <summary>
    /// Generates the mesh for a chunk.
    /// </summary>
    public class MeshGenerator
    {
        /// <summary>
        /// Initializes a new instance of the MeshGenerator class.
        /// </summary>
        public MeshGenerator()
        {
        }

        /// <summary>
        /// Generate the mesh for the given terrain chunk.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        public void Generate(Terrain terrain, Position chunkIndex)
        {
            Chunk chunk = terrain.GetChunk(chunkIndex);

            // TODO
        }
    }
}