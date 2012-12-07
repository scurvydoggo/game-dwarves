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
        /// <param name="digDepth">The depth to which digging occurs.</param>
        public TerrainMutator(int digDepth)
        {
            this.DigDepth = digDepth;
        }

        /// <summary>
        /// Gets the depth to which digging occurs.
        /// </summary>
        public int DigDepth { get; private set; }

        /// <summary>
        /// Dig at the given position.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="position">The position.</param>
        /// <param name="offset">The offset indicating the position inside the voxel with values between 0.0 and 1.0.
        /// </param>
        public void Dig(Terrain terrain, Vector2I position, Vector2 offset)
        {
            // Get the voxel array
            IVoxels voxels;
            if (terrain.Voxels.TryGetValue(terrain.ChunkIndex(position.X, position.Y), out voxels))
            {
                for (int z = terrain.WorldDepth; z < terrain.WorldDepth + this.DigDepth; z++)
                {
                    Vector2I pos = terrain.WorldToChunk(position.X, position.Y);

                    // Get the voxel
                    Voxel voxel = voxels[pos.X, pos.Y, z];

                    // Update the voxel density
                    voxel.Density = Voxel.DensityMax;
                    voxels[pos.X, pos.Y, z] = voxel;
                }
            }
        }
    }
}