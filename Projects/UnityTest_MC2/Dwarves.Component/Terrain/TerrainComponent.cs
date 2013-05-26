// ----------------------------------------------------------------------------
// <copyright file="TerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using System.Collections.Generic;
    using Dwarves.Component.Bounds;
    using Dwarves.Core;
    using Dwarves.Core.Math;
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
        /// The distance (in chunk coordinates) from the viewport at which chunks should begin loading in advance.
        /// </summary>
        public int DistanceChunkBeginLoad;

        /// <summary>
        /// The main camera bounds component.
        /// </summary>
        private CameraBoundsComponent cCameraBounds;

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            // Initialise the terrain sub system
            TerrainSystem.Initialise(
                this.ChunkWidthLog,
                this.ChunkHeightLog,
                this.ChunkDepth,
                this.DigDepth,
                this.SurfaceAmplitude,
                this.Seed,
                this.Octaves,
                this.BaseFrequency,
                this.Persistence);

            // Register event handlers
            TerrainSystem.Instance.Terrain.ChunkAdded += this.Terrain_ChunkAdded;
            TerrainSystem.Instance.Terrain.ChunkRemoved += this.Terrain_ChunkRemoved;

            // Get the main camera bounds component
            this.cCameraBounds = Camera.main.GetComponent<CameraBoundsComponent>();
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            // Get the active chunks
            Dictionary<Vector2I, bool> activeChunks = this.GetActiveChunks();

            // Update the terrain system
            TerrainSystem.Instance.Update(activeChunks);
        }

        /// <summary>
        /// Gets the chunks that are current active. The boolean value indicates whether the chunk is required this
        /// frame.
        /// </summary>
        /// <returns>The active chunks.</returns>
        private Dictionary<Vector2I, bool> GetActiveChunks()
        {
            // bool indicates if the chunk is required or if the loading can be deferred to a background thread
            var activeChunks = new Dictionary<Vector2I, bool>();

            // Get the bounds within the camera
            RectangleI cameraPointBounds = this.cCameraBounds.GetBounds();
            Vector2I top = Metrics.ChunkIndex(cameraPointBounds.X, cameraPointBounds.Y);
            Vector2I bottom = Metrics.ChunkIndex(cameraPointBounds.Right - 1, cameraPointBounds.Bottom - 1);
            var cameraChunkBounds = new RectangleI(
                top.X - 2,
                top.Y + 2,
                bottom.X - top.X + 4,
                top.Y - bottom.Y + 4);

            // Add the chunks that the camera is pointing directly at. These are required to be loaded this frame
            this.PopulateActiveChunks(activeChunks, cameraChunkBounds, true);

            // Add the camera-bordering chunks to begin loading in advance
            var borderBounds = new RectangleI(
                cameraChunkBounds.X - this.DistanceChunkBeginLoad,
                cameraChunkBounds.Y - this.DistanceChunkBeginLoad,
                cameraChunkBounds.Width + (this.DistanceChunkBeginLoad * 2),
                cameraChunkBounds.Height + (this.DistanceChunkBeginLoad * 2));
            this.PopulateActiveChunks(activeChunks, borderBounds, false);

            // Add all other chunks which contain significant actors
            foreach (ActorBoundsComponent actor in GameObject.FindObjectsOfType(typeof(ActorBoundsComponent)))
            {
                var actorBounds = Metrics.WorldToChunk(actor.GetBounds());
                this.PopulateActiveChunks(activeChunks, actorBounds, false);
            }

            return activeChunks;
        }

        /// <summary>
        /// Adds each chunk within the given world bounds to the given dictionary.
        /// </summary>
        /// <param name="chunks">The dictionary being populated.</param>
        /// <param name="chunkBounds">The bounds in chunk coordinates.</param>
        /// <param name="requiredThisFrame">Indicates whether the chunks are required in this frame.</param>
        private void PopulateActiveChunks(
            Dictionary<Vector2I, bool> chunks,
            RectangleI chunkBounds,
            bool requiredThisFrame)
        {
            for (int x = chunkBounds.X; x < chunkBounds.Right; x++)
            {
                for (int y = chunkBounds.Y; y > chunkBounds.Bottom; y--)
                {
                    var chunkIndex = new Vector2I(x, y);
                    bool alreadyRequiredThisFrame;
                    if (!chunks.TryGetValue(chunkIndex, out alreadyRequiredThisFrame))
                    {
                        chunks.Add(chunkIndex, requiredThisFrame);
                    }
                    else if (requiredThisFrame && !alreadyRequiredThisFrame)
                    {
                        chunks[chunkIndex] = requiredThisFrame;
                    }
                }
            }
        }

        /// <summary>
        /// Handle a chunk add event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        private void Terrain_ChunkAdded(object sender, Vector2I chunkIndex)
        {
            GameScheduler.Instance.Invoke(
                () =>
                {
                    // Create the chunk game object
                    var chunkObject = new GameObject(TerrainChunkComponent.GetLabel(chunkIndex));
                    chunkObject.transform.parent = this.transform;
                    TerrainChunkComponent chunkComponent = chunkObject.AddComponent<TerrainChunkComponent>();
                    chunkComponent.Chunk = chunkIndex;
                });
        }

        /// <summary>
        /// Handle a chunk removal event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        private void Terrain_ChunkRemoved(object sender, Vector2I chunkIndex)
        {
            GameScheduler.Instance.Invoke(
                () =>
                {
                    // Find the chunk's component
                    Transform chunkTransform = this.transform.FindChild(TerrainChunkComponent.GetLabel(chunkIndex));
                    if (chunkTransform != null)
                    {
                        // Destroy it!
                        GameObject.Destroy(chunkTransform.gameObject);
                    }
                });
        }
    }
}