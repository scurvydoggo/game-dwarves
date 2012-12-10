// ----------------------------------------------------------------------------
// <copyright file="CachedCells.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;

    /// <summary>
    /// Reference to the vertices of the cells in the current and previous z planes.
    /// </summary>
    public class CachedCells
    {
        /// <summary>
        /// Initialises a new instance of the CachedCells class.
        /// </summary>
        /// <param name="chunkWidth">The chunk width.</param>
        /// <param name="chunkHeight">The chunk height.</param>
        public CachedCells(int chunkWidth, int chunkHeight)
        {
            this.Cells = new CachedCell[2][,];
            this.Cells[0] = new CachedCell[chunkWidth, chunkHeight];
            this.Cells[1] = new CachedCell[chunkWidth, chunkHeight];
        }

        /// <summary>
        /// Gets the cached cells. 1st dimension indicates z index; 2nd indicates x,y position of cell.
        /// </summary>
        public CachedCell[][,] Cells { get; private set; }

        /// <summary>
        /// Gets the position of the cell in the direction from the given position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="y">The z position.</param>
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
        /// Gets the cached cell at the position in the direction relative to the given position.
        /// </summary>
        /// <param name="x">The current x position.</param>
        /// <param name="y">The current y position.</param>
        /// <param name="y">The current z position.</param>
        /// <param name="direction">The bitmask indicating the directional of the cached cell.</param>
        /// <returns>The cached cell.</returns>
        public CachedCell GetCachedCell(int x, int y, int z, byte direction)
        {
            Vector3I position = CachedCells.GetNeighbourPosition(x, y, z, direction);
            return this.Cells[position.Z & 1][position.X, position.Y];
        }

        /// <summary>
        /// Set the cached cell at the given position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <param name="cell">The cached cell.</param>
        public void SetCachedCell(int x, int y, int z, CachedCell cell)
        {
            this.Cells[z & 1][x, y] = cell;
        }
    }
}