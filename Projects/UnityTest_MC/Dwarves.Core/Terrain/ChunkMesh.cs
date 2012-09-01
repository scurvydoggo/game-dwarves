// ----------------------------------------------------------------------------
// <copyright file="ChunkMesh.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using System.Collections;
    using System.Collections.Generic;
    using Dwarves.Core.Mesh;

    /// <summary>
    /// The mesh data for a terrain chunk.
    /// </summary>
    public class ChunkMesh : IEnumerable<KeyValuePair<Position, MeshData>>
    {
        /// <summary>
        /// The mesh for each voxel in the chunk.
        /// </summary>
        private Dictionary<Position, MeshData> voxelMeshes;

        /// <summary>
        /// Initializes a new instance of the ChunkMesh class.
        /// </summary>
        public ChunkMesh()
        {
            this.voxelMeshes = new Dictionary<Position, MeshData>();
            this.MeshChanged = false;
            this.VertexCount = 0;
            this.TriangleIndicesCount = 0;
        }

        /// <summary>
        /// Gets a value indicating whether the mesh has been changed.
        /// </summary>
        public bool MeshChanged { get; private set; }

        /// <summary>
        /// Gets the number of vertices.
        /// </summary>
        public int VertexCount { get; private set; }

        /// <summary>
        /// Gets the number of triangle indices.
        /// </summary>
        public int TriangleIndicesCount { get; private set; }

        /// <summary>
        /// Gets an enumerator that iterates through the block meshes.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<Position, MeshData>> GetEnumerator()
        {
            return this.voxelMeshes.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through the block meshes.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Update the voxel mesh at the given chunk coordinates.
        /// </summary>
        /// <param name="chunkPos">The position.</param>
        /// <param name="mesh">The mesh.</param>
        public void SetMesh(Position chunkPos, MeshData mesh)
        {
            // Add/Replace the mesh
            MeshData existingMesh;
            if (this.voxelMeshes.TryGetValue(chunkPos, out existingMesh))
            {
                this.voxelMeshes[chunkPos] = mesh;

                // Decrement the counts for the mesh that was replaced
                this.VertexCount -= existingMesh.Vertices.Length;
                this.TriangleIndicesCount -= existingMesh.TriangleIndices.Length;
            }
            else
            {
                this.voxelMeshes.Add(chunkPos, mesh);
            }

            // Increment the counts for the new mesh
            this.VertexCount += mesh.Vertices.Length;
            this.TriangleIndicesCount += mesh.TriangleIndices.Length;

            this.MeshChanged = true;
        }

        /// <summary>
        /// Remove the voxel mesh at the given chunk coordinates.
        /// </summary>
        /// <param name="chunkPos">The position.</param>
        public void RemoveMesh(Position chunkPos)
        {
            MeshData mesh;
            if (this.voxelMeshes.TryGetValue(chunkPos, out mesh))
            {
                this.voxelMeshes.Remove(chunkPos);

                // Decrement the counts for the mesh that was removed
                this.VertexCount -= mesh.Vertices.Length;
                this.TriangleIndicesCount -= mesh.TriangleIndices.Length;

                this.MeshChanged = true;
            }
        }

        /// <summary>
        /// Clear the mesh data.
        /// </summary>
        public void ClearMesh()
        {
            this.voxelMeshes = new Dictionary<Position, MeshData>();
            this.VertexCount = 0;
            this.TriangleIndicesCount = 0;
            this.MeshChanged = true;
        }

        /// <summary>
        /// Resets the MeshChanged flag.
        /// </summary>
        public void ResetMeshChanged()
        {
            this.MeshChanged = false;
        }
    }
}