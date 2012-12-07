// ----------------------------------------------------------------------------
// <copyright file="TerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dwarves.Component.Bounds;
    using Dwarves.Core.Math;
    using Dwarves.Core.Math.Noise;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Engine;
    using Dwarves.Core.Terrain.Generation;
    using Dwarves.Core.Terrain.Geometry;
    using Dwarves.Core.Terrain.Mutation;
    using Dwarves.Core.Terrain.Serialisation;
    using UnityEngine;
    using Terrain = Dwarves.Core.Terrain.VoxelTerrain;

    /// <summary>
    /// Terrain component.
    /// </summary>
    public class TerrainComponent : MonoBehaviour
    {
        /// <summary>
        /// The terrain engine type.
        /// </summary>
        public TerrainEngineType Engine;

        /// <summary>
        /// The chunk width.
        /// </summary>
        public int ChunkWidth;

        /// <summary>
        /// The chunk height.
        /// </summary>
        public int ChunkHeight;

        /// <summary>
        /// The chunk depth.
        /// </summary>
        public int ChunkDepth;

        /// <summary>
        /// The depth level at which the game simulation takes place.
        /// </summary>
        public int WorldDepth;

        /// <summary>
        /// The depth to which digging occurs.
        /// </summary>
        public int DigDepth;

        /// <summary>
        /// The scaling ratio for voxel coordinates to world coordinates (essentially the Level of Detail).
        /// </summary>
        public int Scale;

        /// <summary>
        /// The distance from the mean surface height that the terrain oscillates.
        /// </summary>
        public int SurfaceAmplitude;

        /// <summary>
        /// The seed value used by the terrain generator.
        /// </summary>
        public int Seed;

        /// <summary>
        /// The number of octaves of noise used by the terrain generator.
        /// </summary>
        public int Octaves;

        /// <summary>
        /// The base frequency which is the frequency of the lowest octave used by the terrain generator.
        /// </summary>g
        public float BaseFrequency;

        /// <summary>
        /// The persistence value, which determines the amplitude for each octave used by the terrain generator.
        /// </summary>
        public float Persistence;

        /// <summary>
        /// Bitwise values indicating chunk border.
        /// </summary>
        [Flags]
        private enum ChunkNeighbour
        {
            /// <summary>
            /// The neighbour to the top.
            /// </summary>
            Top = 1,

            /// <summary>
            /// The neighbour to the right.
            /// </summary>
            Right = 2
        }

        /// <summary>
        /// Gets the terrain instance.
        /// </summary>
        public Terrain Terrain { get; private set; }

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
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.Terrain = new Terrain(
                this.Engine, this.ChunkWidth, this.ChunkHeight, this.ChunkDepth, this.WorldDepth, this.Scale);

            // Initialise the serialiser
            this.TerrainSerialiser = new TerrainSerialiser();

            // Initialise the terrain generator
            var simplexGenerator = new SimplexNoiseGenerator();
            var noiseGenerator = new CompoundNoiseGenerator(
                simplexGenerator, this.Seed, (byte)this.Octaves, this.BaseFrequency, this.Persistence);
            this.TerrainGenerator = new TerrainGenerator(noiseGenerator, this.SurfaceAmplitude);

            // Initialise the mutator
            this.TerrainMutator = new TerrainMutator(this.DigDepth);

            // Initialise the mesh builder
            this.TerrainMeshBuilder = new TerrainMeshBuilder();
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            // Check if any chunks need to be loaded or unloaded
            this.LoadUnloadChunks();
        }

        /// <summary>
        /// Load and unload the chunks that are new or are no longer required.
        /// </summary>
        private void LoadUnloadChunks()
        {
            // Determine which chunks are currently active
            var activeChunks = new HashSet<Vector2I>();
            foreach (ActorComponent actor in GameObject.FindObjectsOfType(typeof(ActorComponent)))
            {
                // Get the chunk-bounds of the actor
                RectangleI bounds = actor.GetChunkBounds();

                // Step through each chunk index in the actor bounds
                for (int x = bounds.X; x < bounds.Right; x++)
                {
                    for (int y = bounds.Y; y > bounds.Bottom; y--)
                    {
                        activeChunks.Add(new Vector2I(x, y));
                    }
                }
            }

            // Check if any chunks are now off screen with no actors within and will need to be removed
            foreach (Vector2I chunk in this.Terrain.Voxels.Keys.Where((c) => !activeChunks.Contains(c)).ToArray())
            {
                // Remove the data
                this.Terrain.RemoveChunkData(chunk);

                // Remove the chunk game object
                Transform chunkTransform = this.transform.FindChild(TerrainChunkComponent.GetLabel(chunk));
                if (chunkTransform != null)
                {
                    GameObject.Destroy(chunkTransform.gameObject);
                }
            }

            // Load the new chunk data
            var toRebuild = new HashSet<Vector2I>();
            foreach (Vector2I chunk in activeChunks)
            {
                if (!this.Terrain.Voxels.ContainsKey(chunk))
                {
                    // Attempt to deserialise the chunk
                    if (!this.TerrainSerialiser.TryDeserialise(this.Terrain, chunk))
                    {
                        // The chunk doesn't exist to be serialised, so generate it from scratch
                        this.TerrainGenerator.Generate(this.Terrain, chunk);
                    }

                    // Create the chunk game object
                    var chunkObject = new GameObject(TerrainChunkComponent.GetLabel(chunk));
                    chunkObject.transform.parent = this.transform;
                    TerrainChunkComponent chunkComponent = chunkObject.AddComponent<TerrainChunkComponent>();
                    chunkComponent.Chunk = chunk;

                    // Add this chunk and its dependent neighbours to the set of chunks requiring a mesh rebuild
                    toRebuild.Add(chunk);
                    toRebuild.Add(new Vector2I(chunk.X, chunk.Y - 1));
                    toRebuild.Add(new Vector2I(chunk.X - 1, chunk.Y));
                }
            }

            // Rebuild the meshes as required
            foreach (Vector2I chunk in toRebuild)
            {
                Transform chunkTransform = this.transform.FindChild(TerrainChunkComponent.GetLabel(chunk));
                if (chunkTransform != null)
                {
                    chunkTransform.GetComponent<TerrainChunkComponent>().RebuildMesh();
                }
            }
        }
    }
}