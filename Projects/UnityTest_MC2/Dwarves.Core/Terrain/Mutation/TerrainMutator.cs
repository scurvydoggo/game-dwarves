// ----------------------------------------------------------------------------
// <copyright file="TerrainMutator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Mutation
{
    using Dwarves.Core.Math;
    using UnityEngine;

    using Terrain = Dwarves.Core.Terrain.VoxelTerrain;

    /// <summary>
    /// Mutates voxel terrain.
    /// </summary>
    public class TerrainMutator
    {
        /// <summary>
        /// Initialises a new instance of the TerrainMutator class.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="digDepth">The depth to which digging occurs.</param>
        public TerrainMutator(Terrain terrain, int digDepth)
        {
            this.Terrain = terrain;
            this.DigDepth = digDepth;
        }

        /// <summary>
        /// Gets the terrain.
        /// </summary>
        public VoxelTerrain Terrain { get; private set; }

        /// <summary>
        /// Gets the depth to which digging occurs.
        /// </summary>
        public int DigDepth { get; private set; }

        /// <summary>
        /// Dig at the given position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="offset">The offset indicating the position inside the voxel with values between 0.0 and 1.0.
        /// </param>
        public void Dig(Vector2I pos, Vector2 offset)
        {
            Vector2I chunk = this.Terrain.ChunkIndex(pos.X, pos.Y);

            // Get the voxel array
            IVoxels voxels;
            if (this.Terrain.TryGetChunk(chunk, out voxels))
            {
                // Dig down to the given depth
                for (int z = 0; z < this.DigDepth; z++)
                {
                    Vector2I chunkPos = this.Terrain.WorldToChunk(pos);

                    // Get the voxel
                    Voxel voxel = voxels[chunkPos.X, chunkPos.Y, z];

                    // Update the voxel density
                    voxel.Density = Voxel.DensityMax;
                    voxels[chunkPos.X, chunkPos.Y, z] = voxel;
                }

                // Flag the chunk as requiring a rebuild
                this.Terrain.FlagRebuildRequired(chunk, true);
            }
        }
    }
}