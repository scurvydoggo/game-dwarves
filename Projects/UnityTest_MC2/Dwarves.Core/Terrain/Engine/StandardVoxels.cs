// ----------------------------------------------------------------------------
// <copyright file="StandardVoxels.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Engine
{
    /// <summary>
    /// The standard structure for the voxel data.
    /// </summary>
    public class StandardVoxels : IVoxels
    {
        /// <summary>
        /// The array of voxels.
        /// </summary>
        private Voxel[,,] voxels;

        /// <summary>
        /// Initialises a new instance of the StandardVoxels class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        public StandardVoxels(int width, int height, int depth)
        {
            this.voxels = new Voxel[width, height, depth];
        }

        /// <summary>
        /// Gets or sets the voxel at the given position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <returns>The voxel.</returns>
        public Voxel this[int x, int y, int z]
        {
            get
            {
                return this.voxels[x, y, z];
            }

            set
            {
                this.voxels[x, y, z] = value;
            }
        }
    }
}