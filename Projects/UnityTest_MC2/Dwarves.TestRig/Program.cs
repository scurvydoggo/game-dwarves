// ----------------------------------------------------------------------------
// <copyright file="Program.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestRig
{
    using System.Collections.Generic;
    using Dwarves.Core.Math;
    using Dwarves.Core.Terrain;
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
            TerrainManager terrain = Program.CreateTerrain();

            while (true)
            {
                terrain.LoadUnloadChunks(new List<Vector2I>(new Vector2I[] { new Vector2I(0, 0) }));
            }
        }

        /// <summary>
        /// Create the terrain component.
        /// </summary>
        /// <returns>The terrain component.</returns>
        private static TerrainManager CreateTerrain()
        {
            return new TerrainManager(
                TerrainEngineType.Standard,
                4,
                4,
                5,
                1,
                3,
                1,
                10,
                1,
                10,
                10f,
                0.5f);
        }
    }
}