// ----------------------------------------------------------------------------
// <copyright file="Program.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestApplication
{
    using Dwarves.Core;
    using Dwarves.Core.Noise;
    using Dwarves.Core.Terrain;
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
            TestNoiseGenerator();
        }

        /// <summary>
        /// Run a test for the marching cubes algorithm.
        /// </summary>
        private static void TestMarchingCubes()
        {
            var terrain = new VoxelTerrain();
            var chunkIndex = new Position(0, 0);
            var noiseGenerator = new NoiseGenerator(1, 16, 0.001f, 0.25f);

            // Load the chunk
            var chunkLoader = new ChunkLoader(noiseGenerator);
            chunkLoader.LoadChunk(terrain, chunkIndex);

            // Generate the mesh
            var meshGenerator = new MarchingCubesGenerator(terrain, 127);
            meshGenerator.UpdateChunk(chunkIndex);
        }

        /// <summary>
        /// Run a test for the noise generator.
        /// </summary>
        private static void TestNoiseGenerator()
        {
            int seed = 3;
            byte octaves = 16;
            float baseFrequency = 0.0025f;
            float persistence = 0.25f;
            var generator = new NoiseGenerator(seed, octaves, baseFrequency, persistence);

            float[] vals = new float[1000];
            for (int x = 0; x < vals.Length; x++)
            {
                vals[x] = generator.Generate(x);
            }
        }
    }
}