// ----------------------------------------------------------------------------
// <copyright file="Program.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestApplication
{
    using Dwarves.Core;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Generation;
    using Dwarves.Core.Terrain.Generation.MarchingCubes;
    using Dwarves.Core.Terrain.Load;

    /// <summary>
    /// Entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">The application args.</param>
        public static void Main(string[] args)
        {
            TestMarchingCubes();
        }

        /// <summary>
        /// Run a test for the marching cubes algorithm.
        /// </summary>
        private static void TestMarchingCubes()
        {
            var terrain = new VoxelTerrain();

            var chunkIndex = new Position(0, 0);

            // Load the chunk
            var chunkLoader = new ChunkLoader(0);
            chunkLoader.LoadChunk(terrain, chunkIndex, ChunkUsage.RenderingAndPhysics);

            // Generate the mesh
            var meshGenerator = new MarchingCubesGenerator(127);
            meshGenerator.UpdateChunk(terrain, chunkIndex);
        }
    }
}