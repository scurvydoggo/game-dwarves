// ----------------------------------------------------------------------------
// <copyright file="TerrainChunkComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using Dwarves.Core;
using Dwarves.Core.Jobs;
using Dwarves.Core.Math;
using Dwarves.Core.Terrain;
using UnityEngine;

    /// <summary>
    /// Represents a terrain mesh.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TerrainChunkComponent : MonoBehaviour
    {
        /// <summary>
        /// The mesh filter component.
        /// </summary>
        private MeshFilter cMeshFilter;

        /// <summary>
        /// The chunk and its neighbours.
        /// </summary>
        private Vector2I[] chunkAndNeighbours;

        /// <summary>
        /// Gets or sets the chunk index.
        /// </summary>
        public Vector2I Chunk { get; set; }

        /// <summary>
        /// Gets the label for the given chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk label.</returns>
        public static string GetLabel(Vector2I chunkIndex)
        {
            return "Chunk[" + chunkIndex.X + "," + chunkIndex.Y + "]";
        }

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.cMeshFilter = this.GetComponent<MeshFilter>();
            this.chunkAndNeighbours = TerrainChunk.GetNeighboursIncluding(this.Chunk);
            this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Terrain_GrassMud");
        }

        /// <summary>
        /// Called once per frame after the Update method has been called for all components.
        /// </summary>
        public void Update()
        {
            // Rebuild the mesh
            JobSystem.Instance.Scheduler.BeginEnqueueChunks();
            try
            {
                if (JobSystem.Instance.Scheduler.ForAllChunks(
                    this.chunkAndNeighbours,
                    (q) => q.State.CanRebuildMesh(this.Chunk),
                    MissingQueue.Skip))
                {
                    JobSystem.Instance.Scheduler.EnqueueChunks(
                        () => this.RebuildMeshJob(),
                        (q) => q.State.ReserveRebuildMesh(this.Chunk),
                        (q) => q.State.UnreserveRebuildMesh(this.Chunk),
                        true,
                        this.chunkAndNeighbours);
                }
            }
            finally
            {
                JobSystem.Instance.Scheduler.EndEnqueueChunks();
            }
            
            // Update the mesh filter
            JobSystem.Instance.Scheduler.BeginEnqueueChunks();
            try
            {
                if (JobSystem.Instance.Scheduler.ForAllChunks(
                    this.Chunk,
                    (q) => q.State.CanUpdateMeshFilter(),
                    MissingQueue.Skip))
                {
                    JobSystem.Instance.Scheduler.EnqueueChunks(
                        () => this.UpdateMeshFilterJob(),
                        (q) => q.State.ReserveUpdateMeshFilter(),
                        (q) => q.State.UnreserveUpdateMeshFilter(),
                        true,
                        this.Chunk);
                }
            }
            finally
            {
                JobSystem.Instance.Scheduler.EndEnqueueChunks();
            }
        }

        /// <summary>
        /// Rebuild the mesh for this chunk.
        /// </summary>
        private void RebuildMeshJob()
        {
            // The chunk GameObject (this) is destroyed after the logical instance, so check that it still exists
            if (TerrainSystem.Instance.Terrain.HasChunk(this.Chunk))
            {
                TerrainSystem.Instance.MeshBuilder.RebuildMesh(this.Chunk);
            }
        }

        /// <summary>
        /// Update the MeshFilter data for this chunk.
        /// </summary>
        private void UpdateMeshFilterJob()
        {
            // The chunk GameObject (this) is destroyed after the logical instance, so check that it still exists
            TerrainChunk chunk;
            if (TerrainSystem.Instance.Terrain.TryGetChunk(this.Chunk, out chunk))
            {
                // Copy the mesh data into arrays
                Vector3[] vertices = chunk.Mesh.Data.Vertices.ToArray();
                Vector3[] normals = chunk.Mesh.Data.Normals.ToArray();
                int[] triangles = chunk.Mesh.Data.Indices.ToArray();
                Color[] colors = chunk.Mesh.Data.Light.ToArray();

                // Update the mesh filter geometry
                GameScheduler.Instance.Invoke(
                    () =>
                    {
                        this.cMeshFilter.mesh.Clear();
                        this.cMeshFilter.mesh.vertices = chunk.Mesh.Data.Vertices.ToArray();
                        this.cMeshFilter.mesh.normals = chunk.Mesh.Data.Normals.ToArray();
                        this.cMeshFilter.mesh.triangles = chunk.Mesh.Data.Indices.ToArray();
                        this.cMeshFilter.mesh.colors = chunk.Mesh.Data.Light.ToArray();
                    });
            }
        }
    }
}