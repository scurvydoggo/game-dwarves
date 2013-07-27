// ----------------------------------------------------------------------------
// <copyright file="CellGeometry.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    /// <summary>
    /// The geometry for a cell.
    /// </summary>
    public struct CellGeometry
    {
        /// <summary>
        /// High nibble is vertex count; low nibble is triangle count.
        /// </summary>
        private byte counts;

        /// <summary>
        /// The groups of 3 indexes giving the triangulation.
        /// </summary>
        private byte[] indices;

        /// <summary>
        /// Initialises a new instance of the CellGeometry struct.
        /// </summary>
        /// <param name="counts">High nibble is vertex count; low nibble is triangle count.</param>
        /// <param name="indices">The groups of 3 indexes giving the triangulation.</param>
        public CellGeometry(byte counts, byte[] indices)
        {
            this.counts = counts;
            this.indices = indices;
        }

        /// <summary>
        /// Gets the triangle count.
        /// </summary>
        public long TriangleCount
        {
            get { return this.counts & 0x0F; }
        }

        /// <summary>
        /// Gets the vertex count.
        /// </summary>
        public long VertexCount
        {
            get { return this.counts >> 4; }
        }

        /// <summary>
        /// Gets the groups of 3 indexes giving the triangulation.
        /// </summary>
        public byte[] Indices
        {
            get { return this.indices; }
        }
    }
}