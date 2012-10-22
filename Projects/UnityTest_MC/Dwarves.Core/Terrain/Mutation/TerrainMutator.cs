// ----------------------------------------------------------------------------
// <copyright file="TerrainMutator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Mutation
{
    using Dwarves.Core.Terrain;
    using UnityEngine;

    /// <summary>
    /// Indicates that the terrain has been mutated.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event args.</param>
    public delegate void MutationEvent(object sender, MutationArgs e);

    /// <summary>
    /// Allows for mutation of terrain voxels.
    /// </summary>
    public class TerrainMutator
    {
        /// <summary>
        /// The terrain.
        /// </summary>
        private VoxelTerrain terrain;

        /// <summary>
        /// Initialises a new instance of the TerrainMutator class.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        public TerrainMutator(VoxelTerrain terrain)
        {
            this.terrain = terrain;
        }

        /// <summary>
        /// Indicates that the terrain has been mutated.
        /// </summary>
        public event MutationEvent MutationOccurred;

        /// <summary>
        /// Dig at the given world position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="offset">The offset indicating the position inside the voxel with values between 0.0 and 1.0.
        /// </param>
        public void Dig(Position position, Vector2 offset)
        {
            // Get the chunk
            Position chunkIndex = VoxelTerrain.GetChunkIndex(position);
            Chunk chunk;
            if (!this.terrain.Chunks.TryGetValue(chunkIndex, out chunk))
            {
                // The given position is outside the known world, so do nothing
                return;
            }

            // Get the current voxel
            Position chunkPos = VoxelTerrain.GetChunkCoordinates(position);
            Voxel oldVoxel = chunk.GetVoxel(chunkPos);

            // Update the voxel density
            Voxel newVoxel = new Voxel(TerrainMaterial.Dirt, byte.MaxValue);
            chunk.SetVoxel(chunkPos, newVoxel);

            // Indicate that a mutation occurred
            this.OnMutationOccurred(new MutationArgs(position));
        }

        /// <summary>
        /// Indicates that the terrain has been mutated.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected void OnMutationOccurred(MutationArgs e)
        {
            if (this.MutationOccurred != null)
            {
                this.MutationOccurred(this, e);
            }
        }
    }
}