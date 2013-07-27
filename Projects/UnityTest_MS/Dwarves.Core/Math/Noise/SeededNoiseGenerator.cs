// ----------------------------------------------------------------------------
// <copyright file="SeededNoiseGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Math.Noise
{
    using System;

    /// <summary>
    /// Generates noise with seed value provided at initialisation.
    /// </summary>
    public class SeededNoiseGenerator : INoiseGenerator
    {
        /// <summary>
        /// The base noise generator.
        /// </summary>
        private INoiseGenerator baseGenerator;

        /// <summary>
        /// The seed value.
        /// </summary>
        private int seed;

        /// <summary>
        /// Initialises a new instance of the SeededNoiseGenerator class.
        /// </summary>
        /// <param name="baseGenerator">The base noise generator.</param>
        /// <param name="seed">The seed value.</param>
        public SeededNoiseGenerator(INoiseGenerator baseGenerator, int seed)
        {
            this.baseGenerator = baseGenerator;
            this.seed = seed;
        }

        /// <summary>
        /// Generate 1D noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <returns>The noise value.</returns>
        public float Generate(float x)
        {
            return this.baseGenerator.Generate(x, this.seed);
        }

        /// <summary>
        /// Generate 2D noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The noise value.</returns>
        public float Generate(float x, float y)
        {
            return this.baseGenerator.Generate(x, y, this.seed);
        }

        /// <summary>
        /// Generate 3D noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <returns>The noise value.</returns>
        public float Generate(float x, float y, float z)
        {
            return this.baseGenerator.Generate(x, y, z, this.seed);
        }

        /// <summary>
        /// Generate 4D noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <param name="w">The w position.</param>
        /// <returns>The noise value.</returns>
        public float Generate(float x, float y, float z, float w)
        {
            throw new NotSupportedException();
        }
    }
}
