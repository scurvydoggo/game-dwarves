// ----------------------------------------------------------------------------
// <copyright file="DigTerrainTest.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestRig
{
    using Dwarves.Core;
    using UnityEngine;

    /// <summary>
    /// A test for digging terrain.
    /// </summary>
    public class DigTerrainTest : ITest
    {
        /// <summary>
        /// Initialises a new instance of the DigTerrainTest class.
        /// </summary>
        public DigTerrainTest()
        {
            var createTerrain = new CreateTerrainTest();
            createTerrain.Update();
        }

        /// <summary>
        /// Update the component.
        /// </summary>
        public void Update()
        {
            TerrainSystem.Instance.Mutator.DigCircle(new Vector2(0.9f, 0.9f), 10);
        }
    }
}