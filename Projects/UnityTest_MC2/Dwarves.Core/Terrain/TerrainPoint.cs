// ----------------------------------------------------------------------------
// <copyright file="TerrainPoint.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using Dwarves.Core.Lighting;

    /// <summary>
    /// A point in the terrain. The point represents a column of voxels along the Z-axis when viewing a 2D cross
    /// section of the terrain from the side.
    /// </summary>
    public class TerrainPoint
    {
        /// <summary>
        /// The voxels.
        /// </summary>
        private TerrainVoxel[] voxels;

        /// <summary>
        /// Initialises a new instance of the TerrainPoint class.
        /// </summary>
        /// <param name="voxels">The voxels.</param>
        public TerrainPoint(TerrainVoxel[] voxels) : this(voxels, null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the TerrainPoint class.
        /// </summary>
        /// <param name="voxels">The voxels.</param>
        /// <param name="light">The light value.</param>
        public TerrainPoint(TerrainVoxel[] voxels, Colour? light)
        {
            this.voxels = voxels;
            this.Light = light;
        }

        /// <summary>
        /// Gets or sets the light value.
        /// </summary>
        public Colour? Light { get; set; }
        
        /// <summary>
        /// Gets the voxel at the given z depth.
        /// </summary>
        /// <param name="z">The z depth.</param>
        /// <returns>The voxel.</returns>
        public TerrainVoxel GetVoxel(int z)
        {
            if (z >= 0 && z < this.voxels.Length)
            {
                return this.voxels[z];
            }
            else
            {
                return TerrainVoxel.CreateEmpty();
            }
        }
    }
}