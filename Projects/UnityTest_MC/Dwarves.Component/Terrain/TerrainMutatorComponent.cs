// ----------------------------------------------------------------------------
// <copyright file="TerrainMutatorComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using Dwarves.Core;
    using Dwarves.Core.Terrain;
    using UnityEngine;

    /// <summary>
    /// Component for mutating the terrain.
    /// </summary>
    [RequireComponent(typeof(TerrainComponent))]
    public class TerrainMutatorComponent : MonoBehaviour
    {
        /// <summary>
        /// The core terrain component.
        /// </summary>
        private TerrainComponent cTerrain;

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.cTerrain = this.GetComponent<TerrainComponent>();
        }

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
            if (!this.cTerrain.Terrain.Chunks.TryGetValue(chunkIndex, out chunk))
            {
                // The given position is outside the known world, so do nothing
                return;
            }

            // Update the voxel density
            Voxel voxel = chunk.GetVoxel(VoxelTerrain.GetChunkCoordinates(position));
            voxel.Density0 = 0;
            voxel.Density1 = 0;
            voxel.Density2 = 0;
            voxel.Density3 = 0;

            // Update the mesh
            this.cTerrain.MeshGenerator.UpdateVoxel(this.cTerrain.Terrain, position, true);
        }
    }
}