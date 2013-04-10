// ----------------------------------------------------------------------------
// <copyright file="TerrainManager.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dwarves.Core.Math;
    using Dwarves.Core.Math.Noise;
    using Dwarves.Core.Terrain.Generation;
    using Dwarves.Core.Terrain.Geometry;
    using Dwarves.Core.Terrain.Mutation;
    using Dwarves.Core.Terrain.Serialisation;
    using UnityEngine;

    /// <summary>
    /// The top manager of a terrain instance.
    /// </summary>
    public class TerrainManager
    {
        /// <summary>
        /// Initialises a new instance of the TerrainManager class.
        /// </summary>
        /// <param name="chunkWidthLog">The power-of-2 chunk width.</param>
        /// <param name="chunkHeightLog">The power-of-2 chunk height.</param>
        /// <param name="chunkDepth">The chunk depth.</param>
        /// <param name="digDepth">The depth to which digging occurs.</param>
        /// <param name="scale">The scaling ratio for voxel coordinates to world coordinates (essentially the Level of
        /// Detail).</param>
        /// <param name="surfaceAmplitude">The distance from the mean surface height that the terrain oscillates.
        /// </param>
        /// <param name="seed">The seed value used by the terrain generator.</param>
        /// <param name="octaves">The number of octaves of noise used by the terrain generator.</param>
        /// <param name="baseFrequency">The base frequency which is the frequency of the lowest octave used by the
        /// terrain generator.</param>
        /// <param name="persistence">The persistence value, which determines the amplitude for each octave used by the
        /// terrain generator.</param>
        private TerrainManager(
            int chunkWidthLog,
            int chunkHeightLog,
            int chunkDepth,
            int digDepth,
            int scale,
            int surfaceAmplitude,
            int seed,
            int octaves,
            float baseFrequency,
            float persistence)
        {
            // Initialise the terrain
            this.Terrain = new DwarfTerrain(chunkWidthLog, chunkHeightLog, chunkDepth, scale);

            // Initialise the serialiser
            this.TerrainSerialiser = new TerrainSerialiser(this.Terrain);

            // Initialise the terrain generator
            var simplexGenerator = new SimplexNoiseGenerator();
            var noiseGenerator = new CompoundNoiseGenerator(
                simplexGenerator, seed, (byte)octaves, baseFrequency, persistence);
            this.TerrainGenerator = new TerrainGenerator(this.Terrain, noiseGenerator, surfaceAmplitude);

            // Initialise the mutator
            this.TerrainMutator = new TerrainMutator(this.Terrain, digDepth);

            // Initialise the mesh builder
            this.TerrainMeshBuilder = new TerrainMeshBuilder(this.Terrain);
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static TerrainManager Instance { get; private set; }

        /// <summary>
        /// Gets the terrain instance.
        /// </summary>
        public DwarfTerrain Terrain { get; private set; }

        /// <summary>
        /// Gets the terrain serialiser.
        /// </summary>
        public TerrainSerialiser TerrainSerialiser { get; private set; }

        /// <summary>
        /// Gets the terrain generator.
        /// </summary>
        public TerrainGenerator TerrainGenerator { get; private set; }

        /// <summary>
        /// Gets the terrain mutator.
        /// </summary>
        public TerrainMutator TerrainMutator { get; private set; }

        /// <summary>
        /// Gets the terrain mesh builder.
        /// </summary>
        public TerrainMeshBuilder TerrainMeshBuilder { get; private set; }

        /// <summary>
        /// Initialises the singleton instance.
        /// </summary>
        /// <param name="chunkWidthLog">The power-of-2 chunk width.</param>
        /// <param name="chunkHeightLog">The power-of-2 chunk height.</param>
        /// <param name="chunkDepth">The chunk depth.</param>
        /// <param name="digDepth">The depth to which digging occurs.</param>
        /// <param name="scale">The scaling ratio for voxel coordinates to world coordinates (essentially the Level of
        /// Detail).</param>
        /// <param name="surfaceAmplitude">The distance from the mean surface height that the terrain oscillates.
        /// </param>
        /// <param name="seed">The seed value used by the terrain generator.</param>
        /// <param name="octaves">The number of octaves of noise used by the terrain generator.</param>
        /// <param name="baseFrequency">The base frequency which is the frequency of the lowest octave used by the
        /// terrain generator.</param>
        /// <param name="persistence">The persistence value, which determines the amplitude for each octave used by the
        /// terrain generator.</param>
        public static void Initialise(
            int chunkWidthLog,
            int chunkHeightLog,
            int chunkDepth,
            int digDepth,
            int scale,
            int surfaceAmplitude,
            int seed,
            int octaves,
            float baseFrequency,
            float persistence)
        {
            if (TerrainManager.Instance != null)
            {
                throw new InvalidOperationException("Only one TerrainManager instance may exist.");
            }
            
            TerrainManager.Instance = new TerrainManager(
                chunkWidthLog,
                chunkHeightLog,
                chunkDepth,
                digDepth,
                scale,
                surfaceAmplitude,
                seed,
                octaves,
                baseFrequency,
                persistence);
        }

        /// <summary>
        /// Load and unload the chunks that are new or are no longer required.
        /// </summary>
        /// <param name="activeChunks">The currently active chunks</param>
        public void LoadUnloadChunks(ICollection<Vector2I> activeChunks)
        {
            // Check if any chunks are now off screen with no actors within and will need to be removed
            var toRemove = new List<Vector2I>();
            foreach (Vector2I chunkIndex in this.Terrain.Chunks)
            {
                if (!activeChunks.Contains(chunkIndex))
                {
                    toRemove.Add(chunkIndex);
                }
            }

            // Remove the chunks
            foreach (Vector2I chunkIndex in toRemove)
            {
                this.Terrain.RemoveChunk(chunkIndex);
            }

            // Load the new chunk data
            foreach (Vector2I chunkIndex in activeChunks)
            {
                if (!this.Terrain.ContainsChunk(chunkIndex))
                {
                    // Deserialise/generate the chunk
                    TerrainChunk chunk;
                    if (!this.TerrainSerialiser.TryDeserialise(chunkIndex, out chunk))
                    {
                        chunk = this.TerrainGenerator.CreateChunk(chunkIndex);
                    }

                    // Add the chunk
                    this.Terrain.AddChunk(chunk, chunkIndex);
                }
            }
        }
    }
}