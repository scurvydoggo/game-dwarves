// ----------------------------------------------------------------------------
// <copyright file="TerrainManager.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using System.Collections.Generic;
    using System.Linq;
    using Dwarves.Core.Math;
    using Dwarves.Core.Math.Noise;
    using Dwarves.Core.Terrain.Engine;
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
        /// <param name="engine">The terrain engine type.</param>
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
        public TerrainManager(
            TerrainEngineType engine,
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
            this.Terrain = new VoxelTerrain(engine, chunkWidthLog, chunkHeightLog, chunkDepth, scale);

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
        /// Gets the terrain instance.
        /// </summary>
        public VoxelTerrain Terrain { get; private set; }

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
        /// Load and unload the chunks that are new or are no longer required.
        /// </summary>
        /// <param name="activeChunks">The currently active chunks</param>
        public void LoadUnloadChunks(ICollection<Vector2I> activeChunks)
        {
            // Check if any chunks are now off screen with no actors within and will need to be removed
            foreach (Vector2I chunk in this.Terrain.Chunks.Where((c) => !activeChunks.Contains(c)).ToArray())
            {
                // Remove the chunk
                this.Terrain.RemoveChunk(chunk);
            }

            // Load the new chunk data
            foreach (Vector2I chunk in activeChunks)
            {
                if (!this.Terrain.ContainsChunk(chunk))
                {
                    // Attempt to deserialise the chunk
                    if (!this.TerrainSerialiser.TryDeserialise(chunk))
                    {
                        // The chunk doesn't exist to be serialised, so generate it from scratch
                        this.TerrainGenerator.Generate(chunk);
                    }

                    // Flag the neighbouring meshes requiring rebuild
                    IVoxels voxels;
                    if (this.Terrain.TryGetChunk(new Vector2I(chunk.X, chunk.Y - 1), out voxels))
                    {
                        voxels.RebuildRequired = true;
                    }

                    if (this.Terrain.TryGetChunk(new Vector2I(chunk.X - 1, chunk.Y), out voxels))
                    {
                        voxels.RebuildRequired = true;
                    }
                }
            }
        }
    }
}