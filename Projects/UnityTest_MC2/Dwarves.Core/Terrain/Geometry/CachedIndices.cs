// ----------------------------------------------------------------------------
// <copyright file="CachedIndices.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;

    /// <summary>
    /// Reference to the vertex indices of the voxels in the current and previous z planes.
    /// </summary>
    public class CachedIndices
    {
        /// <summary>
        /// The cached indices. 1st dimension indicates z index; 2nd indicates x,y position of cell; 3rd indicates
        /// the vertex index of each of the sharable vertices.
        /// </summary>
        private int[][,][] indices;

        /// <summary>
        /// Initialises a new instance of the CachedIndices class.
        /// </summary>
        /// <param name="chunkWidth">The chunk width.</param>
        /// <param name="chunkHeight">The chunk height.</param>
        public CachedIndices(int chunkWidth, int chunkHeight)
        {
            this.indices = new int[2][,][];
            this.indices[0] = new int[chunkWidth, chunkHeight][];
            this.indices[1] = new int[chunkWidth, chunkHeight][];
        }

        /// <summary>
        /// Gets or sets the cached indices at the given position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <returns>The indices.</returns>
        public int[] this[int x, int y, int z]
        {
            get { return this.indices[z & 1][x, y]; }
            set { this.indices[z & 1][x, y] = value; }
        }

        /// <summary>
        /// Gets the position of the cell in the direction from the given position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <param name="direction">The bitmask indicating the direction.</param>
        /// <returns>The neighbouring position.</returns>
        public static Vector3I GetNeighbourPosition(int x, int y, int z, byte direction)
        {
            // Get the directional offset
            int dx = direction & 0x01;
            int dy = (direction >> 1) & 0x01;
            int dz = (direction >> 2) & 0x01;

            // Offset the reused cell from the given position
            return new Vector3I(x - dx, y - dy, z - dz);
        }

        /// <summary>
        /// Gets the cached indices at the position in the direction relative to the given position.
        /// </summary>
        /// <param name="x">The x position from which the direction is applied.</param>
        /// <param name="y">The y position from which the direction is applied.</param>
        /// <param name="z">The z position from which the direction is applied.</param>
        /// <param name="direction">The bitmask indicating the directional of the cached cell.</param>
        /// <returns>The indices.</returns>
        public int[] GetCachedIndices(int x, int y, int z, byte direction)
        {
            Vector3I position = CachedIndices.GetNeighbourPosition(x, y, z, direction);
            return this[position.Z, position.Y, position.Z];
        }
    }
}