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
                Vector2I[] chunksToSync = null;
                if (JobSystem.Instance.Scheduler.ForAllChunks(
                    this.Chunk,
                    (q) => q.State.CanUpdateMeshFilter(out chunksToSync),
                    MissingQueue.Fail))
                {
                    Vector2I[] chunks = chunksToSync != null ? chunksToSync : new Vector2I[] { this.Chunk };
                    JobSystem.Instance.Scheduler.EnqueueChunks(
                        () => this.UpdateMeshFilterJob(chunksToSync),
                        (q) => q.State.ReserveUpdateMeshFilter(),
                        (q) => q.State.UnreserveUpdateMeshFilter(),
                        true,
                        chunks);
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
        /// <param name="chunksToSync">The chunks that need to have their mesh filter updated in the same frame as
        /// this. Null if no chunk sync is required.</param>
        private void UpdateMeshFilterJob(Vector2I[] chunksToSync)
        {
            // The chunk GameObject (this) is destroyed after the logical instance, so check that it still exists
            TerrainChunk chunk;
            if (chunksToSync == null)
            {
                if (TerrainSystem.Instance.Terrain.TryGetChunk(this.Chunk, out chunk))
                {
                    // Copy the mesh data into arrays
                    var mesh = new MeshArrays(
                        this.Chunk,
                        chunk.Mesh.Data.Vertices.ToArray(),
                        chunk.Mesh.Data.Normals.ToArray(),
                        chunk.Mesh.Data.Indices.ToArray(),
                        chunk.Mesh.Data.Light.ToArray());

                    // Update the mesh filter geometry
                    GameScheduler.Instance.Invoke(
                        () =>
                        {
                            this.cMeshFilter.mesh.Clear();
                            this.cMeshFilter.mesh.vertices = mesh.Vertices;
                            this.cMeshFilter.mesh.normals = mesh.Normals;
                            this.cMeshFilter.mesh.triangles = mesh.Triangles;
                            this.cMeshFilter.mesh.colors = mesh.Colors;
                        });
                }
            }
            else
            {
                var meshes = new MeshArrays[chunksToSync.Length];
                for (int i = 0; i < meshes.Length; i++)
                {
                    if (TerrainSystem.Instance.Terrain.TryGetChunk(this.Chunk, out chunk))
                    {
                        // Copy the mesh data into arrays
                        meshes[i] = new MeshArrays(
                            this.Chunk,
                            chunk.Mesh.Data.Vertices.ToArray(),
                            chunk.Mesh.Data.Normals.ToArray(),
                            chunk.Mesh.Data.Indices.ToArray(),
                            chunk.Mesh.Data.Light.ToArray());
                    }
                }

                // Update the mesh filter geometry
                GameScheduler.Instance.Invoke(
                    () =>
                    {
                        foreach (MeshArrays mesh in meshes)
                        {
                            if (mesh == null)
                            {
                                continue;
                            }

                            Transform chunkTransform =
                                this.transform.parent.FindChild(TerrainChunkComponent.GetLabel(mesh.Chunk));
                            if (chunkTransform == null)
                            {
                                continue;
                            }

                            TerrainChunkComponent cChunk = chunkTransform.GetComponent<TerrainChunkComponent>();
                            cChunk.cMeshFilter.mesh.Clear();
                            cChunk.cMeshFilter.mesh.vertices = mesh.Vertices;
                            cChunk.cMeshFilter.mesh.normals = mesh.Normals;
                            cChunk.cMeshFilter.mesh.triangles = mesh.Triangles;
                            cChunk.cMeshFilter.mesh.colors = mesh.Colors;
                        }
                    });
            }
        }

        /// <summary>
        /// The mesh arrays.
        /// </summary>
        private class MeshArrays
        {
            /// <summary>
            /// Initialises a new instance of the MeshArrays class.
            /// </summary>
            /// <param name="chunk">The chunk.</param>
            /// <param name="vertices">The vertices.</param>
            /// <param name="normals">The normal vectors.</param>
            /// <param name="triangles">The triangles.</param>
            /// <param name="colors">The colours.</param>
            public MeshArrays(Vector2I chunk, Vector3[] vertices, Vector3[] normals, int[] triangles, Color[] colors)
            {
                this.Chunk = chunk;
                this.Vertices = vertices;
                this.Normals = normals;
                this.Triangles = triangles;
                this.Colors = colors;
            }

            /// <summary>
            /// Gets the chunk.
            /// </summary>
            public Vector2I Chunk { get; private set; }

            /// <summary>
            /// Gets the vertices.
            /// </summary>
            public Vector3[] Vertices { get; private set; }

            /// <summary>
            /// Gets the normal vectors.
            /// </summary>
            public Vector3[] Normals { get; private set; }

            /// <summary>
            /// Gets the triangles.
            /// </summary>
            public int[] Triangles { get; private set; }

            /// <summary>
            /// Gets the colours.
            /// </summary>
            public Color[] Colors { get; private set; }
        }
    }
}