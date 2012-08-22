// ----------------------------------------------------------------------------
// <copyright file="ChunkMesh.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain
{
    using System.Collections;
    using System.Collections.Generic;
    using Dwarves.Core.Mesh;

    /// <summary>
    /// The mesh data for a terrain chunk.
    /// </summary>
    public class ChunkMesh : IEnumerable<KeyValuePair<Position, MeshData[]>>
    {
        /// <summary>
        /// The meshes for each voxel in the chunk.
        /// </summary>
        private Dictionary<Position, MeshData[]> voxelMeshes;

        /// <summary>
        /// Initializes a new instance of the ChunkMesh class.
        /// </summary>
        public ChunkMesh()
        {
            this.voxelMeshes = new Dictionary<Position, MeshData[]>();
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
        public IEnumerator<KeyValuePair<Position, MeshData[]>> GetEnumerator()
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
        /// Update the meshes for the voxel at the given chunk coordinates.
        /// </summary>
        /// <param name="chunkX">The x position.</param>
        /// <param name="chunkY">The y position.</param>
        /// <param name="meshes">The meshes.</param>
        public void SetMesh(int chunkX, int chunkY, MeshData[] meshes)
        {
            this.SetMeshes(new Position(chunkX, chunkY), meshes);
        }

        /// <summary>
        /// Update the meshes for the voxel at the given chunk coordinates.
        /// </summary>
        /// <param name="chunkPos">The position.</param>
        /// <param name="meshes">The meshes.</param>
        public void SetMeshes(Position chunkPos, MeshData[] meshes)
        {
            // Add/Replace the meshes
            MeshData[] existingMeshes;
            if (this.voxelMeshes.TryGetValue(chunkPos, out existingMeshes))
            {
                this.voxelMeshes[chunkPos] = meshes;

                // Decrement the counts for the meshes that were replaced
                foreach (MeshData mesh in existingMeshes)
                {
                    this.VertexCount -= mesh.Vertices.Length;
                    this.TriangleIndicesCount -= mesh.TriangleIndices.Length;
                }
            }
            else
            {
                this.voxelMeshes.Add(chunkPos, meshes);
            }

            // Increment the counts for the new meshes
            foreach (MeshData mesh in meshes)
            {
                this.VertexCount += mesh.Vertices.Length;
                this.TriangleIndicesCount += mesh.TriangleIndices.Length;
            }

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