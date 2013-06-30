// ----------------------------------------------------------------------------
// <copyright file="TerrainSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core.Math;
    using Dwarves.Core.Math.Noise;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Generation;
    using Dwarves.Core.Terrain.Geometry;
    using Dwarves.Core.Terrain.Mutation;
    using Dwarves.Core.Terrain.Serialisation;
    using Dwarves.Core.Jobs;

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
            this.MeshBuilder = new TerrainMeshBuilder(this.Terrain);
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
        /// Gets the mesh builder.
        /// </summary>
        public TerrainMeshBuilder MeshBuilder { get; private set; }

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
        /// <param name="activeChunks">The currently active chunks.</param>
        public void Update(HashSet<Vector2I> activeChunks)
        {
            this.LoadUnloadChunks(activeChunks);
        }

        /// <summary>
        /// Load and unload the chunks that are new or are no longer required.
        /// </summary>
        /// <param name="activeChunks">The currently active chunks.</param>
        private void LoadUnloadChunks(HashSet<Vector2I> activeChunks)
        {
            // Update the active queues on the job system
            JobSystem.Instance.Scheduler.UpdateActiveChunks(activeChunks);

            // Get the current chunks
            var currentChunks = new HashSet<Vector2I>(this.Terrain.GetChunksThreadSafe());

            // Get the new chunks
            List<Vector2I> newChunks = null;
            foreach (Vector2I chunk in activeChunks)
            {
                if (!currentChunks.Contains(chunk))
                {
                    if (newChunks == null)
                    {
                        newChunks = new List<Vector2I>();
                    }

                    // Add the new chunk
                    newChunks.Add(chunk);
                }
            }

            // Get the chunks that are no longer used
            List<Vector2I> toRemove = null;
            foreach (Vector2I chunk in currentChunks)
            {
                if (!activeChunks.Contains(chunk))
                {
                    if (toRemove == null)
                    {
                        toRemove = new List<Vector2I>();
                    }

                    toRemove.Add(chunk);
                }
            }

            // Enqueue the chunk removal job
            if (toRemove != null)
            {
                Guid id = Guid.NewGuid();
                JobSystem.Instance.Scheduler.EnqueueMaster(
                    () => this.RemoveChunksJob(toRemove),
                    (q) => q.State.CanRemoveChunks(toRemove),
                    (q) => q.State.ReserveRemoveChunks(toRemove, id),
                    (q) => q.State.UnreserveAddRemoveChunks(toRemove, id),
                    false);
            }

            // Enqueue the new chunk jobs
            if (newChunks != null)
            {
                // Add the chunks
                Guid id = Guid.NewGuid();
                var addChunksSet = new List<Vector2I>(newChunks);
                JobSystem.Instance.Scheduler.EnqueueMaster(
                    () => this.AddChunksJob(addChunksSet),
                    (q) => q.State.CanAddChunks(addChunksSet),
                    (q) => q.State.ReserveAddChunks(addChunksSet, id),
                    (q) => q.State.UnreserveAddRemoveChunks(addChunksSet, id),
                    true);

                // Load the point data for each new chunk
                foreach (Vector2I chunk in newChunks)
                {
                    // Get an array of the chunk and its neighbours that have been loaded
                    var neighbours = new List<Vector2I>(TerrainChunk.GetNeighboursExcluding(chunk));
                    JobSystem.Instance.Scheduler.TrimChunks(neighbours, (q) => q.State.LoadPointsCompleted);
                    var chunks = new Vector2I[neighbours.Count + 1];
                    chunks[0] = chunk;
                    neighbours.CopyTo(chunks, 1);

                    JobSystem.Instance.Scheduler.Enqueue(
                        () => this.LoadPointsJob(chunk),
                        (q) => q.State.CanLoadPoints(chunk),
                        (q) => q.State.ReserveLoadPoints(chunk, new ChunkSync(chunks)),
                        (q) => q.State.UnreserveLoadPoints(chunk),
                        true,
                        chunks);
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
                // Add the surface heights for the chunk's x position
                if (!TerrainSystem.Instance.Terrain.SurfaceHeights.ContainsKey(chunkIndex.X))
                {
                    float[] heights = this.surfaceGenerator.GenerateSurfaceHeights(chunkIndex.X);
                    TerrainSystem.Instance.Terrain.SurfaceHeights.Add(chunkIndex.X, heights);
                }

                TerrainSystem.Instance.Terrain.AddChunk(chunkIndex, new TerrainChunk());
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

                // Remove the surface heights if they are no longer required
                if (!TerrainSystem.Instance.Terrain.HasChunkAtX(chunkIndex.X))
                {
                    TerrainSystem.Instance.Terrain.SurfaceHeights.Remove(chunkIndex.X);
                }
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
                float[] heights = this.Terrain.SurfaceHeights[chunkIndex.X];
                this.pointGenerator.GeneratePoints(chunkIndex, chunk, heights);
            }
        }
    }
}