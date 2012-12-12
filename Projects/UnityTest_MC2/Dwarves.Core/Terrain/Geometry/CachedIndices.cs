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
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    this.indices[0][x, y] = new int[4];
                    this.indices[1][x, y] = new int[4];
                }
            }
        }

        /// <summary>
        /// Gets or sets the cached vertex indice at the given position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <param name="index">The index of the vertex indice.</param>
        /// <returns>The vertex indice.</returns>
        public int this[int x, int y, int z, byte index]
        {
            get { return this.indices[z & 1][x, y][index]; }
            set { this.indices[z & 1][x, y][index] = value; }
        }

        /// <summary>
        /// Gets the cached indices at the position in the direction relative to the given position.
        /// </summary>
        /// <param name="x">The x position from which the direction is applied.</param>
        /// <param name="y">The y position from which the direction is applied.</param>
        /// <param name="z">The z position from which the direction is applied.</param>
        /// <param name="direction">The bitmask indicating the directional of the cached cell.</param>
        /// <param name="index">The index of the vertex indice.</param>
        /// <returns>The vertex indice.</returns>
        public int GetRelativeIndice(int x, int y, int z, byte direction, byte index)
        {
            // Get the directional offset
            int dx = direction & 0x01;
            int dy = (direction >> 1) & 0x01;
            int dz = (direction >> 2) & 0x01;

            return this[x - dx, y - dy, z - dz, index];
        }
    }
}