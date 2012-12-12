// ----------------------------------------------------------------------------
// <copyright file="Program.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestRig
{
    using Dwarves.Component.Terrain;
    using Dwarves.Core.Terrain.Engine;

    /// <summary>
    /// The main class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The application entry point.
        /// </summary>
        /// <param name="args">The application args.</param>
        private static void Main(string[] args)
        {
            TerrainComponent terrain = Program.CreateTerrain();

            while (true)
            {
                terrain.Update();
            }
        }

        /// <summary>
        /// Create the terrain component.
        /// </summary>
        /// <returns>The terrain component.</returns>
        private static TerrainComponent CreateTerrain()
        {
            var terrain = new TerrainComponent();
            terrain.Engine = TerrainEngineType.Standard;
            terrain.ChunkWidthLog = 4;
            terrain.ChunkHeightLog = 4;
            terrain.ChunkDepth = 5;
            terrain.WorldDepth = 1;
            terrain.DigDepth = 3;
            terrain.Scale = 1;
            terrain.SurfaceAmplitude = 10;
            terrain.Seed = 1;
            terrain.Octaves = 10;
            terrain.BaseFrequency = 10f;
            terrain.Persistence = 0.5f;
            terrain.Start();

            return terrain;
        }
    }
}