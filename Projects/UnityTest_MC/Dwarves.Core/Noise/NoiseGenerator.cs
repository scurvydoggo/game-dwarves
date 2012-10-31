// ----------------------------------------------------------------------------
// <copyright file="NoiseGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Noise
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Generates Simplex noise with some configurable settings.
    /// </summary>
    public class NoiseGenerator
    {
        /// <summary>
        /// The noise function seed value for each octave.
        /// </summary>
        private Dictionary<byte, int> seeds;

        /// <summary>
        /// The frequency for each octave.
        /// </summary>
        private Dictionary<byte, float> frequencies;

        /// <summary>
        /// The frequency for each octave.
        /// </summary>
        private Dictionary<byte, float> amplitudes;

        /// <summary>
        /// Initialises a new instance of the NoiseGenerator class.
        /// </summary>
        /// <param name="octaveCount">The number of octaves.</param>
        /// <param name="baseFrequency">The base frequency which is the frequency of the lowest octave.</param>
        /// <param name="persistence">The persistence value, which determines the amplitude for each octave.</param>
        public NoiseGenerator(int seed, byte octaveCount, float baseFrequency, float persistence)
        {
            this.OctaveCount = octaveCount;
            this.BaseFrequency = baseFrequency;
            this.Persistence = persistence;

            // Generate the seed for each octave's noise function
            var random = new Random(seed);
            this.seeds = new Dictionary<byte, int>(this.OctaveCount);
            for (byte i = 0; i < this.OctaveCount; i++)
            {
                this.seeds[i] = random.Next(100000);
            }

            // Pre-calculate the frequency for each octave
            this.frequencies = new Dictionary<byte, float>(this.OctaveCount);
            for (byte i = 0; i < this.OctaveCount; i++)
            {
                this.frequencies[i] = (float)Math.Pow(2, i) * this.BaseFrequency;
            }

            // Pre-calculate the amplitude for each octave
            this.amplitudes = new Dictionary<byte, float>(this.OctaveCount);
            for (byte i = 0; i < this.OctaveCount; i++)
            {
                this.amplitudes[i] = (float)Math.Pow(this.Persistence, i);
            }
        }

        /// <summary>
        /// Gets the number of octaves.
        /// </summary>
        public int OctaveCount { get; private set; }

        /// <summary>
        /// Gets the base frequency which is the frequency of the lowest octave.
        /// </summary>
        public float BaseFrequency { get; private set; }

        /// <summary>
        /// Gets the persistence value, which determines the amplitude for each octave.
        /// </summary>
        public float Persistence { get; private set; }

        /// <summary>
        /// Generate 1D simplex noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <returns>The noise value.</returns>
        public float Generate(float x)
        {
            float total = 0;

            for (byte i = 0; i < this.OctaveCount; i++)
            {
                int seed = this.seeds[i];
                float frequency = this.frequencies[i];
                float amplitude = this.amplitudes[i];

                total += SimplexNoise.Generate(seed, x * frequency) * amplitude;
            }

            return total;
        }

        /// <summary>
        /// Generate 2D simplex noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The noise value.</returns>
        public float Generate(float x, float y)
        {
            float total = 0;

            for (byte i = 0; i < this.OctaveCount; i++)
            {
                int seed = this.seeds[i];
                float frequency = this.frequencies[i];
                float amplitude = this.amplitudes[i];

                total += SimplexNoise.Generate(seed, x * frequency, y * frequency) * amplitude;
            }

            return total;
        }

        /// <summary>
        /// Generate 3D simplex noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <returns>The noise value.</returns>
        public float Generate(float x, float y, float z)
        {
            float total = 0;

            for (byte i = 0; i < this.OctaveCount; i++)
            {
                float frequency = this.frequencies[i];
                float amplitude = this.amplitudes[i];

                total += SimplexNoise.Generate(x * frequency, y * frequency, z * frequency) * amplitude;
            }

            return total;
        }
    }
}