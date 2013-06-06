// ----------------------------------------------------------------------------
// <copyright file="TerrainSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dwarves.Core.Math;
    using Dwarves.Core.Math.Noise;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Generation;
    using Dwarves.Core.Terrain.Geometry;
    using Dwarves.Core.Terrain.Mutation;
    using Dwarves.Core.Terrain.Serialisation;

    /// <summary>
    /// The terrain sub-system.
    /// </summary>
    public class TerrainSystem
    {
        /// <summary>
        /// The terrain serialiser.
        /// </summary>
        private TerrainSerialiser serialiser;

        /// <summary>
        /// The point generator.
        /// </summary>
        private TerrainPointGenerator pointGenerator;

        /// <summary>
        /// The surface generator.
        /// </summary>
        private TerrainSurfaceGenerator surfaceGenerator;

        /// <summary>
        /// The mesh builder.
        /// </summary>
        private TerrainMeshBuilder meshBuilder;

        /// <summary>
        /// Initialises a new instance of the TerrainSystem class.
        /// </summary>
        /// <param name="seed">The seed value used by the terrain generator.</param>
        /// <param name="octaves">The number of octaves of noise used by the terrain generator.</param>
        /// <param name="baseFrequency">The base frequency which is the frequency of the lowest octave used by the
        /// terrain generator.</param>
        /// <param name="persistence">The persistence value, which determines the amplitude for each octave used by the
        /// terrain generator.</param>
        private TerrainSystem(int seed, int octaves, float baseFrequency, float persistence)
        {
            // Initialise the noise generator
            var simplexGenerator = new SimplexNoiseGenerator();
            this.Noise = new CompoundNoiseGenerator(simplexGenerator, seed, (byte)octaves, baseFrequency, persistence);

            // Initialise the terrain
            this.Terrain = new DwarfTerrain();

            // Initialise the mutator
            this.Mutator = new TerrainMutator(this.Terrain);

            // Initialise the point/mesh construction objects
            this.serialiser = new TerrainSerialiser();
            this.pointGenerator = new TerrainPointGenerator(this.Noise);
            this.surfaceGenerator = new TerrainSurfaceGenerator(this.Noise);
            this.meshBuilder = new TerrainMeshBuilder(this.Terrain);
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static TerrainSystem Instance { get; private set; }

        /// <summary>
        /// Gets the terrain.
        /// </summary>
        public DwarfTerrain Terrain { get; private set; }

        /// <summary>
        /// Gets the terrain mutator.
        /// </summary>
        public TerrainMutator Mutator { get; private set; }

        /// <summary>
        /// Gets the noise generator for the terrain.
        /// </summary>
        public INoiseGenerator Noise { get; private set; }

        /// <summary>
        /// Initialises the singleton instance.
        /// </summary>
        /// <param name="chunkWidthLog">The power-of-2 chunk width.</param>
        /// <param name="chunkHeightLog">The power-of-2 chunk height.</param>
        /// <param name="chunkDepth">The chunk depth.</param>
        /// <param name="digDepth">The depth to which digging occurs.</param>
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
            int surfaceAmplitude,
            int seed,
            int octaves,
            float baseFrequency,
            float persistence)
        {
            if (TerrainSystem.Instance != null)
            {
                throw new InvalidOperationException("Only one TerrainSystem instance may exist.");
            }

            // Initialise the world metrics
            Metrics.Initialise(chunkWidthLog, chunkHeightLog, chunkDepth, digDepth, surfaceAmplitude);

            // Now initialise the singleton
            TerrainSystem.Instance = new TerrainSystem(seed, octaves, baseFrequency, persistence);
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        /// <param name="activeChunks">The currently active chunks. The boolean value indicates whether the chunks are
        /// required this frame.</param>
        public void Update(Dictionary<Vector2I, bool> activeChunks)
        {
            this.LoadUnloadChunks(activeChunks);
        }

        /// <summary>
        /// Load and unload the chunks that are new or are no longer required.
        /// </summary>
        /// <param name="activeChunks">The currently active chunks. The boolean value indicates whether the chunks are
        /// required in this frame (otherwise loading is deferred to a background thread).</param>
        private void LoadUnloadChunks(Dictionary<Vector2I, bool> activeChunks)
        {
            // Update the active queues on the job system
            JobSystem.Instance.Scheduler.UpdateActiveChunks(activeChunks);

            // Get the current and new chunks
            var currentChunks = new HashSet<Vector2I>(this.Terrain.GetChunksThreadSafe());
            var newChunks = new List<Vector2I>();
            var newChunksAndNeighbours = new HashSet<Vector2I>();
            foreach (KeyValuePair<Vector2I, bool> kvp in activeChunks)
            {
                if (!currentChunks.Contains(kvp.Key))
                {
                    // Add the new chunk
                    newChunks.Add(kvp.Key);

                    // Add the neighbours
                    newChunksAndNeighbours.Add(kvp.Key);
                    foreach (Vector2I neighbour in TerrainChunk.GetNeighbours(kvp.Key))
                    {
                        if (currentChunks.Contains(neighbour))
                        {
                            newChunksAndNeighbours.Add(neighbour);
                        }
                    }
                }
            }

            // Remove the chunks that are no longer used
            var toRemove = new List<Vector2I>();
            foreach (Vector2I chunk in currentChunks)
            {
                if (!activeChunks.ContainsKey(chunk))
                {
                    toRemove.Add(chunk);
                }
            }

            // Enqueue the chunk removal job
            if (toRemove.Count > 0)
            {
                JobSystem.Instance.Scheduler.Enqueue(() => this.RemoveChunksJob(toRemove), false);
            }

            // Determine which surfaces are new and need to be generated
            int[] surfaceHeights = this.Terrain.GetSurfaceHeightIndicesThreadSafe();
            var newSurfaces = new HashSet<int>();
            foreach (Vector2I chunk in newChunks)
            {
                if (!surfaceHeights.Contains(chunk.X))
                {
                    newSurfaces.Add(chunk.X);
                }
            }

            // Enqueue the generate the surface heights job
            if (newSurfaces.Count > 0)
            {
                JobSystem.Instance.Scheduler.Enqueue(() => this.AddSurfaceHeightsJob(newSurfaces), true);
            }

            if (newChunks.Count > 0)
            {
                // Add the new chunks
                JobSystem.Instance.Scheduler.Enqueue(() => this.AddChunksJob(newChunks), true);

                // Load the point data for each new chunk
                foreach (Vector2I chunk in newChunks)
                {
                    JobSystem.Instance.Scheduler.Enqueue(() => this.LoadPointsJob(chunk), true, chunk);
                }
            }

            // Rebuild the new chunks and their neighbours
            foreach (Vector2I chunk in newChunksAndNeighbours)
            {
                Vector2I[] neighbours = TerrainChunk.GetNeighbours(chunk);
                JobSystem.Instance.Scheduler.Enqueue(() => this.RebuildMeshJob(chunk), true, neighbours);
            }
        }

        /// <summary>
        /// Adds surface heights.
        /// </summary>
        /// <param name="xChunks">The chunk x positions.</param>
        private void AddSurfaceHeightsJob(HashSet<int> xChunks)
        {
            foreach (int x in xChunks)
            {
                if (!TerrainSystem.Instance.Terrain.HasSurfaceHeights(x))
                {
                    // Generate the surface heights for this x position
                    float[] heights = this.surfaceGenerator.GenerateSurfaceHeights(x);
                    TerrainSystem.Instance.Terrain.AddSurfaceHeights(x, heights);
                }
            }
        }

        /// <summary>
        /// Adds chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        private void AddChunksJob(List<Vector2I> chunks)
        {
            foreach (Vector2I chunkIndex in chunks)
            {
                if (!TerrainSystem.Instance.Terrain.HasChunk(chunkIndex))
                {
                    TerrainSystem.Instance.Terrain.AddChunk(chunkIndex, new TerrainChunk());
                }
            }
        }

        /// <summary>
        /// Removes chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        private void RemoveChunksJob(List<Vector2I> chunks)
        {
            foreach (Vector2I chunkIndex in chunks)
            {
                TerrainSystem.Instance.Terrain.RemoveChunk(chunkIndex);
            }
        }

        /// <summary>
        /// Loads the point data of a chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        private void LoadPointsJob(Vector2I chunkIndex)
        {
            // Get the chunk
            TerrainChunk chunk = this.Terrain.GetChunk(chunkIndex);

            // Attempt to deserialise the point data
            if (!this.serialiser.TryDeserialisePoints(chunkIndex, chunk))
            {
                // Generate the point data
                float[] heights = this.Terrain.GetSurfaceHeights(chunkIndex.X);
                this.pointGenerator.GeneratePoints(chunkIndex, chunk, heights);
            }
        }

        /// <summary>
        /// Rebuilds the mesh data of a chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        private void RebuildMeshJob(Vector2I chunkIndex)
        {
            // Rebuild the mesh data
            this.meshBuilder.RebuildMesh(chunkIndex);
        }
    }
}