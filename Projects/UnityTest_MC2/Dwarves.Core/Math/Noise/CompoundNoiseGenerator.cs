// ----------------------------------------------------------------------------
// <copyright file="CompoundNoiseGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Math.Noise
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Generates additive noise over multiple octaves.
    /// </summary>
    public class CompoundNoiseGenerator : INoiseGenerator
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
        /// The seed value.
        /// </summary>
        private int seed;

        /// <summary>
        /// The number of octaves of noise.
        /// </summary>
        private byte octaves;

        /// <summary>
        /// The base frequency which is the frequency of the lowest octave.
        /// </summary>
        private float baseFrequency;

        /// <summary>
        /// The persistence value, which determines the amplitude for each octave.
        /// </summary>
        private float persistence;

        /// <summary>
        /// Initialises a new instance of the CompoundNoiseGenerator class.
        /// </summary>
        /// <param name="baseGenerator">The base noise generator.</param>
        /// <param name="seed">The seed value.</param>
        /// <param name="octaves">The number of octaves.</param>
        /// <param name="baseFrequency">The base frequency which is the frequency of the lowest octave.</param>
        /// <param name="persistence">The persistence value, which determines the amplitude for each octave.</param>
        public CompoundNoiseGenerator(
            INoiseGenerator baseGenerator,
            int seed,
            byte octaves,
            float baseFrequency,
            float persistence)
        {
            this.BaseGenerator = baseGenerator;
            this.seed = seed;
            this.octaves = octaves;
            this.baseFrequency = baseFrequency;
            this.persistence = persistence;
        }

        /// <summary>
        /// Gets or sets the base noise generator.
        /// </summary>
        public INoiseGenerator BaseGenerator { get; set; }

        /// <summary>
        /// Gets or sets the seed value.
        /// </summary>
        public int Seed
        {
            get
            {
                return this.seed;
            }

            set
            {
                if (this.seed != value)
                {
                    this.seed = value;
                    this.UpdateNoiseProperties();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of octaves.
        /// </summary>
        public byte Octaves
        {
            get
            {
                return this.octaves;
            }

            set
            {
                if (this.octaves != value)
                {
                    this.octaves = value;
                    this.UpdateNoiseProperties();
                }
            }
        }

        /// <summary>
        /// Gets or sets the base frequency which is the frequency of the lowest octave.
        /// </summary>
        public float BaseFrequency
        {
            get
            {
                return this.baseFrequency;
            }

            set
            {
                if (this.baseFrequency != value)
                {
                    this.baseFrequency = value;
                    this.UpdateNoiseProperties();
                }
            }
        }

        /// <summary>
        /// Gets or sets the persistence value, which determines the amplitude for each octave.
        /// </summary>
        public float Persistence
        {
            get
            {
                return this.persistence;
            }

            set
            {
                if (this.persistence != value)
                {
                    this.persistence = value;
                    this.UpdateNoiseProperties();
                }
            }
        }

        /// <summary>
        /// Generate 1D noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <returns>The noise value.</returns>
        public float Generate(float x)
        {
            float total = 0;

            for (byte i = 0; i < this.Octaves; i++)
            {
                int seed = this.seeds[i];
                float frequency = this.frequencies[i];
                float amplitude = this.amplitudes[i];

                total += this.BaseGenerator.Generate(seed, x * frequency) * amplitude;
            }

            return total;
        }

        /// <summary>
        /// Generate 2D noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The noise value.</returns>
        public float Generate(float x, float y)
        {
            float total = 0;

            for (byte i = 0; i < this.Octaves; i++)
            {
                int seed = this.seeds[i];
                float frequency = this.frequencies[i];
                float amplitude = this.amplitudes[i];

                total += this.BaseGenerator.Generate(seed, x * frequency, y * frequency) * amplitude;
            }

            return total;
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
            float total = 0;

            for (byte i = 0; i < this.Octaves; i++)
            {
                int seed = this.seeds[i];
                float frequency = this.frequencies[i];
                float amplitude = this.amplitudes[i];

                total += this.BaseGenerator.Generate(seed, x * frequency, y * frequency, z * frequency) * amplitude;
            }

            return total;
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

        /// <summary>
        /// Update the properties used by the noise generator.
        /// </summary>
        private void UpdateNoiseProperties()
        {
            var random = new Random(this.Seed);

            // Generate the seed for each octave's noise function
            this.seeds = new Dictionary<byte, int>(this.Octaves);
            for (byte i = 0; i < this.Octaves; i++)
            {
                this.seeds[i] = random.Next();
            }

            // Pre-calculate the frequency for each octave
            this.frequencies = new Dictionary<byte, float>(this.Octaves);
            for (byte i = 0; i < this.Octaves; i++)
            {
                this.frequencies[i] = (float)Math.Pow(2, i) * this.BaseFrequency;
            }

            // Pre-calculate the amplitude for each octave
            this.amplitudes = new Dictionary<byte, float>(this.Octaves);
            for (byte i = 0; i < this.Octaves; i++)
            {
                this.amplitudes[i] = (float)Math.Pow(this.Persistence, i);
            }
        }
    }
}