// ----------------------------------------------------------------------------
// <copyright file="SharedIndices.cs" company="Acidwashed Games">
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
    public class SharedIndices
    {
        /// <summary>
        /// The shared indices. 1st dimension indicates z index; 2nd indicates x,y position of cell; 3rd indicates
        /// the vertex index of each of the sharable vertices.
        /// </summary>
        private ushort[][,][] indices;

        /// <summary>
        /// Initialises a new instance of the SharedIndices class.
        /// </summary>
        /// <param name="chunkWidth">The chunk width.</param>
        /// <param name="chunkHeight">The chunk height.</param>
        public SharedIndices(int chunkWidth, int chunkHeight)
        {
            this.indices = new ushort[2][,][];
            this.indices[0] = new ushort[chunkWidth, chunkHeight][];
            this.indices[1] = new ushort[chunkWidth, chunkHeight][];
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    this.indices[0][x, y] = new ushort[4];
                    this.indices[1][x, y] = new ushort[4];
                }
            }
        }

        /// <summary>
        /// Gets or sets the shared vertex index at the given position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="index">The index of the vertex index.</param>
        /// <returns>The vertex index.</returns>
        public ushort this[Vector3I pos, byte index]
        {
            get { return this.indices[pos.Z & 1][pos.X, pos.Y][index]; }
            set { this.indices[pos.Z & 1][pos.X, pos.Y][index] = value; }
        }

        /// <summary>
        /// Gets the shared indices at the position in the direction relative to the given position.
        /// </summary>
        /// <param name="pos">The position from which the direction is applied.</param>
        /// <param name="direction">The bitmask indicating the directional of the shared cell.</param>
        /// <param name="index">The index of the vertex index.</param>
        /// <returns>The vertex index.</returns>
        public ushort GetIndexInDirection(Vector3I pos, byte direction, byte index)
        {
            // Offset the position by the direction mask
            pos.X -= direction & 0x01;
            pos.Y -= (direction >> 1) & 0x01;
            pos.Z -= (direction >> 2) & 0x01;

            return this[pos, index];
        }
    }
}