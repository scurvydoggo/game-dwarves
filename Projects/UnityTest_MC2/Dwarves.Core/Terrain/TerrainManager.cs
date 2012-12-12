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
    /// A chunk event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="chunk">The chunk index.</param>
    public delegate void ChunkEvent(object sender, Vector2I chunk);

    /// <summary>
    /// The top manager of a terrain instance.
    /// </summary>
    public class TerrainManager
    {
        public TerrainManager(
            TerrainEngineType engine,
            int chunkWidthLog,
            int chunkHeightLog,
            int chunkDepth,
            int worldDepth,
            int digDepth,
            int scale,
            int surfaceAmplitude,
            int seed,
            int octaves,
            float baseFrequency,
            float persistence)
        {
            this.Terrain = new VoxelTerrain(engine, chunkWidthLog, chunkHeightLog, chunkDepth, worldDepth, scale);

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
        /// Indicates that a chunk was added.
        /// </summary>
        public event ChunkEvent ChunkAdded;

        /// <summary>
        /// Indicates that a chunk was removed.
        /// </summary>
        public event ChunkEvent ChunkRemoved;

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
        public void LoadUnloadChunks(HashSet<Vector2I> activeChunks)
        {
            // Check if any chunks are now off screen with no actors within and will need to be removed
            foreach (Vector2I chunk in this.Terrain.Voxels.Keys.Where((c) => !activeChunks.Contains(c)).ToArray())
            {
                // Remove the data
                this.Terrain.RemoveChunkData(chunk);

                // Notify listeners of chunk removal
                this.OnChunkRemoved(chunk);
            }

            // Load the new chunk data
            var toRebuild = new HashSet<Vector2I>();
            foreach (Vector2I chunk in activeChunks)
            {
                if (!this.Terrain.Voxels.ContainsKey(chunk))
                {
                    // Attempt to deserialise the chunk
                    if (!this.TerrainSerialiser.TryDeserialise(chunk))
                    {
                        // The chunk doesn't exist to be serialised, so generate it from scratch
                        this.TerrainGenerator.Generate(chunk);
                    }

                    // Notify listeners of chunk creation
                    this.OnChunkAdded(chunk);

                    // Remove the neighbouring meshes requiring rebuild
                    this.Terrain.Meshes.Remove(new Vector2I(chunk.X, chunk.Y - 1));
                    this.Terrain.Meshes.Remove(new Vector2I(chunk.X - 1, chunk.Y));
                }
            }
        }

        /// <summary>
        /// Fire the ChunkAdded event.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        protected void OnChunkAdded(Vector2I chunk)
        {
            if (this.ChunkAdded != null)
            {
                this.ChunkAdded(this, chunk);
            }
        }

        /// <summary>
        /// Fire the ChunkRemoved event.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        protected void OnChunkRemoved(Vector2I chunk)
        {
            if (this.ChunkRemoved != null)
            {
                this.ChunkRemoved(this, chunk);
            }
        }
    }
}