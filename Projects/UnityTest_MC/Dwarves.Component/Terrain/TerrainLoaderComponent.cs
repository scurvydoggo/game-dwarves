// ----------------------------------------------------------------------------
// <copyright file="TerrainLoaderComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Component.Bounds;
    using Dwarves.Core;
    using Dwarves.Core.Noise;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Load;
    using UnityEngine;

    /// <summary>
    /// Component for loading the terrain.
    /// </summary>
    [RequireComponent(typeof(TerrainComponent))]
    public class TerrainLoaderComponent : MonoBehaviour
    {
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
        /// </summary>
        public float BaseFrequency;

        /// <summary>
        /// The persistence value, which determines the amplitude for each octave used by the terrain generator.
        /// </summary>
        public float Persistence;

        /// <summary>
        /// The core terrain component.
        /// </summary>
        private TerrainComponent cTerrain;

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
        /// Gets the noise generator.
        /// </summary>
        public NoiseGenerator NoiseGenerator { get; private set; }

        /// <summary>
        /// Gets the terrain chunk loader.
        /// </summary>
        public ChunkLoader ChunkLoader { get; private set; }

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.cTerrain = this.GetComponent<TerrainComponent>();

            // Initialise the noise generator
            this.NoiseGenerator = new NoiseGenerator(this.Seed, (byte)this.Octaves, this.BaseFrequency, this.Persistence);

            // Create the chunk loader
            this.ChunkLoader = new ChunkLoader(this.NoiseGenerator);
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            this.LoadUnloadActorChunks();
        }

        /// <summary>
        /// Gets the label for the given chunk index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk label.</returns>
        private static string GetChunkLabel(Position chunkIndex)
        {
            return string.Format("Chunk[{0},{1}]", chunkIndex.X, chunkIndex.Y);
        }

        /// <summary>
        /// Check the bounds of each actor in the game world and load/unload the chunks that are new/no longer required.
        /// </summary>
        private void LoadUnloadActorChunks()
        {
            // Check which chunks are currently active
            var activeChunks = new HashSet<Position>();
            foreach (ActorComponent actor in GameObject.FindObjectsOfType(typeof(ActorComponent)))
            {
                // Get the chunk-bounds of the actor
                Rectangle bounds = actor.GetChunkBounds();

                // Step through each chunk index in the actor bounds
                for (int x = bounds.X; x < bounds.Right; x++)
                {
                    for (int y = bounds.Y; y > bounds.Bottom; y--)
                    {
                        activeChunks.Add(new Position(x, y));
                    }
                }
            }

            // Check if any chunks are now off screen and will need to be removed
            var toRemove = new HashSet<Position>();
            foreach (Position chunkIndex in this.cTerrain.Terrain.Chunks.Keys)
            {
                if (!activeChunks.Contains(chunkIndex))
                {
                    toRemove.Add(chunkIndex);
                }
            }

            // Unload chunks that are no longer used
            foreach (Position chunkIndex in toRemove)
            {
                // Destroy the chunk's game object
                ChunkComponent cChunk = this.GetChunkComponent(chunkIndex);
                if (cChunk != null)
                {
                    GameObject.Destroy(cChunk.gameObject);
                }

                // Unload the chunk data
                this.ChunkLoader.UnloadChunk(this.cTerrain.Terrain, chunkIndex);
            }

            // Load the new chunk data
            var meshRequired = new List<Position>();
            var meshBorderRequired = new Dictionary<Position, ChunkNeighbour>();
            foreach (Position chunkIndex in activeChunks)
            {
                if (!this.cTerrain.Terrain.Chunks.ContainsKey(chunkIndex))
                {
                    // Load the chunk data
                    Chunk chunk = this.ChunkLoader.LoadChunk(this.cTerrain.Terrain, chunkIndex);

                    // Create the chunk game object
                    var chunkObject = new GameObject(TerrainLoaderComponent.GetChunkLabel(chunkIndex));
                    chunkObject.transform.parent = this.transform;
                    ChunkComponent chunkComponent = chunkObject.AddComponent<ChunkComponent>();
                    chunkComponent.Chunk = chunk;
                    chunkComponent.ChunkIndex = chunkIndex;

                    // Add the chunk to a list indicating that it's mesh needs to be generated
                    meshRequired.Add(chunkIndex);

                    // Check if any neighbouring chunks are already loaded. In that case, the bordering voxels of the
                    // neighbours need to be re-generated, as previously this space was empty
                    this.CheckNeighbour(
                        new Position(chunkIndex.X, chunkIndex.Y - 1), ChunkNeighbour.Top, meshBorderRequired);
                    this.CheckNeighbour(
                        new Position(chunkIndex.X - 1, chunkIndex.Y), ChunkNeighbour.Right, meshBorderRequired);
                }
            }

            // Generate the meshes for the newly loaded chunks
            foreach (Position chunkIndex in meshRequired)
            {
                this.cTerrain.MeshGenerator.UpdateChunk(chunkIndex);
            }

            // Re-generate the borders of the chunk meshes that have had a neighbour added
            foreach (KeyValuePair<Position, ChunkNeighbour> kvp in meshBorderRequired)
            {
                Position chunkIndex = kvp.Key;
                ChunkNeighbour borders = kvp.Value;

                if ((borders & ChunkNeighbour.Top) != 0)
                {
                    this.cTerrain.MeshGenerator.UpdateChunkBorderTop(chunkIndex);
                }

                if ((borders & ChunkNeighbour.Right) != 0)
                {
                    this.cTerrain.MeshGenerator.UpdateChunkBorderRight(chunkIndex);
                }
            }
        }

        /// <summary>
        /// Gets the chunk component with the given index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk component; Null if it does not exist.</returns>
        private ChunkComponent GetChunkComponent(Position chunkIndex)
        {
            Transform chunkTransform = this.transform.FindChild(TerrainLoaderComponent.GetChunkLabel(chunkIndex));
            return chunkTransform != null ? chunkTransform.GetComponent<ChunkComponent>() : null;
        }

        /// <summary>
        /// Check if the chunk at the given index exists and if so update the dictionary to indicate that the chunk's
        /// border needs to be regenerated.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="neighbour">The border.</param>
        /// <param name="borders">The chunk borders that need to have their mesh re-generated.</param>
        private void CheckNeighbour(
            Position chunkIndex,
            ChunkNeighbour neighbour,
            Dictionary<Position, ChunkNeighbour> borders)
        {
            if (this.cTerrain.Terrain.Chunks.ContainsKey(chunkIndex))
            {
                if (borders.ContainsKey(chunkIndex))
                {
                    borders[chunkIndex] |= neighbour;
                }
                else
                {
                    borders.Add(chunkIndex, neighbour);
                }
            }
        }
    }
}