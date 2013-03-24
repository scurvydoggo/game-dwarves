// ----------------------------------------------------------------------------
// <copyright file="DigTerrainTest.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestRig
{
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Engine;
    using Dwarves.Core.Terrain.Mutation;
    using UnityEngine;

    /// <summary>
    /// A test for digging terrain.
    /// </summary>
    public class DigTerrainTest : ITest
    {
        /// <summary>
        /// The terrain mutator.
        /// </summary>
        private TerrainMutator mutator;

        /// <summary>
        /// Initialises a new instance of the DigTerrainTest class.
        /// </summary>
        public DigTerrainTest()
        {
            TerrainManager.Initialise(
                TerrainEngineType.Standard,
                4,
                4,
                5,
                3,
                1,
                10,
                1,
                10,
                10f,
                0.5f);

            this.mutator = new TerrainMutator(TerrainManager.Instance.Terrain, 3);
        }

        /// <summary>
        /// Update the component.
        /// </summary>
        public void Update()
        {
            this.mutator.DigCircle(new Vector2(0.9f, 0.9f), 10);
        }
    }
}