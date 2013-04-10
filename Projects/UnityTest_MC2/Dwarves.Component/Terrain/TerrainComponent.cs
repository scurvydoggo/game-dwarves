// ----------------------------------------------------------------------------
// <copyright file="TerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using System.Collections.Generic;
    using Dwarves.Component.Bounds;
    using Dwarves.Core.Math;
    using Dwarves.Core.Terrain;
    using UnityEngine;

    /// <summary>
    /// Terrain component.
    /// </summary>
    public class TerrainComponent : MonoBehaviour
    {
        /// <summary>
        /// The power-of-2 chunk width.
        /// </summary>
        public int ChunkWidthLog;

        /// <summary>
        /// The power-of-2 chunk height.
        /// </summary>
        public int ChunkHeightLog;

        /// <summary>
        /// The chunk depth.
        /// </summary>
        public int ChunkDepth;

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
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            TerrainManager.Initialise(
                this.ChunkWidthLog,
                this.ChunkHeightLog,
                this.ChunkDepth,
                this.DigDepth,
                this.Scale,
                this.SurfaceAmplitude,
                this.Seed,
                this.Octaves,
                this.BaseFrequency,
                this.Persistence);

            // Register event handlers
            TerrainManager.Instance.Terrain.ChunkAdded += this.Terrain_ChunkAdded;
            TerrainManager.Instance.Terrain.ChunkRemoved += this.Terrain_ChunkRemoved;
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
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

            // Load and unload chunks
            TerrainManager.Instance.LoadUnloadChunks(activeChunks);
        }

        /// <summary>
        /// Handle a chunk add event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        private void Terrain_ChunkAdded(object sender, Vector2I chunkIndex)
        {
            // Create the chunk game object
            var chunkObject = new GameObject(TerrainChunkComponent.GetLabel(chunkIndex));
            chunkObject.transform.parent = this.transform;
            TerrainChunkComponent chunkComponent = chunkObject.AddComponent<TerrainChunkComponent>();
            chunkComponent.Chunk = chunkIndex;
        }

        /// <summary>
        /// Handle a chunk removal event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        private void Terrain_ChunkRemoved(object sender, Vector2I chunkIndex)
        {
            // Find the chunk's component
            Transform chunkTransform = this.transform.FindChild(TerrainChunkComponent.GetLabel(chunkIndex));
            if (chunkTransform != null)
            {
                // Destroy it!
                GameObject.Destroy(chunkTransform.gameObject);
            }
        }
    }
}