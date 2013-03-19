// ----------------------------------------------------------------------------
// <copyright file="TerrainMutator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Mutation
{
    using System;
    using Dwarves.Core.Math;
    using UnityEngine;

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
        public TerrainMutator(VoxelTerrain terrain, int digDepth)
        {
            this.Terrain = terrain;
            this.DigDepth = digDepth;
        }

        /// <summary>
        /// The direction to dig.
        /// </summary>
        public enum DigDirection
        {
            /// <summary>
            /// Dig up.
            /// </summary>
            Up,

            /// <summary>
            /// Dig down.
            /// </summary>
            Down,

            /// <summary>
            /// Dig left.
            /// </summary>
            Left,

            /// <summary>
            /// Dig right.
            /// </summary>
            Right
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
        /// Dig at the given point.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="offset">The fractional offset between [0, 0] and [1, 1].</param>
        public void DigPoint(Vector2I position, Vector2 offset)
        {
            Vector2I chunk = this.Terrain.ChunkIndex(position.X, position.Y);

            // Get the voxel array
            IVoxels voxels;
            if (this.Terrain.TryGetChunk(chunk, out voxels))
            {
                // Dig down to the given depth
                for (int z = 0; z < this.DigDepth; z++)
                {
                    Vector2I chunkPos = this.Terrain.WorldToChunk(position);

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

        /// <summary>
        /// Dig a circle in the terrain with the given origin and radius.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="radius">The radius.</param>
        public void DigCircle(Vector2 origin, float radius)
        {
            float radius2 = radius * radius;

            // Begin at the left-most point of the circle moving to the right for each segment on the circumference
            // 'A' refers to the points in the top-half of the circle; 'B' the bottom-half
            float yA, yB;
            float yAPrev = origin.y - 1;    // (Minus 1 here so that in the first iteration it begins on origin.y)
            float yBPrev = origin.y;

            // Determine where the circle cuts each cell boundary in the terrain grid
            int x = (int)Math.Ceiling(origin.x - radius);
            float xEnd = origin.x + radius;
            while (x < xEnd)
            {
                // Calculate the Y values where the circle cuts the cell boundary at X
                float dX = x - origin.x;
                float dY = (float)Math.Sqrt(radius2 - dX * dX);
                yA = dY + origin.y;
                yB = -dY + origin.y;
                
                // Calculate the lower-end of the segment through which the circle cuts the boundary
                int xBase = x - 1;
                int yABase = (int)Math.Floor(yA);
                int yBBase = (int)Math.Floor(yB);

                // Step vertically through the cells that the circle passes through before reaching X
                for (int yAStep = (int)Math.Floor(yAPrev) + 1; yAStep <= yABase; yAStep++)
                {
                    // Calculate the X value where the circle cuts the cell boundary at yAGrid
                    float dYAStep = yAStep - origin.y;
                    float dXA = (float)Math.Sqrt(radius2 - dYAStep * dYAStep);
                    float xA = origin.x + (x < 0 ? -dXA : dXA);

                    // TODO: Dig along the segment from [xBase, yAStep] to [xBase + 1, yAStep] at xA
                }

                // Step vertically through the cells that the circle passes through before reaching X
                for (int yBStep = (int)Math.Floor(yBPrev) - 1; yBStep > yBBase; yBStep--)
                {
                    // Calculate the X value where the circle cuts the cell boundary at yBGrid
                    float dYBStep = yBStep - origin.y;
                    float dXB = (float)Math.Sqrt(radius2 - dYBStep * dYBStep);
                    float xB = origin.x + (x < 0 ? -dXB : dXB);

                    // TODO: Dig along the segment from [xBase, yBStep] to [xBase + 1, yBStep] at xB
                }

                // TODO: Dig along the segment from [xBase, yABase] to [xBase, yABase + 1] at yA
                // TODO: Dig along the segment from [xBase, yBBase] to [xBase, yBBase + 1] at yB

                yAPrev = yA;
                yBPrev = yB;
                x++;
            }
        }

        /// <summary>
        /// Dig a circle in the terrain of the given origin point and radius.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="offset">The fractional offset between [0, 0] and [1, 1].</param>
        public void DigCircle(Vector2I origin, int radius, Vector2 offset)
        {
            int x0 = origin.X;
            int y0 = origin.Y;

            // Step through the points of a circle's octant (from [1, 0] moving CCW) and reflect to the other octants
            int x = radius;
            int y = 0;
            int changeX = 1 - (radius << 1);
            int changeY = 0;
            int radiusError = 0;
            while (x >= y)
            {
                // Dig a line for each octant
                this.DigLine(x + x0, y + y0, x - y, DigDirection.Left, offset);     // From[1, 0] CCW
                this.DigLine(-x + x0, y + y0, x - y, DigDirection.Right, offset);   // From[-1, 0] CW
                this.DigLine(x + x0, -y + y0, x - y, DigDirection.Left, offset);    // From[1, 0] CW
                this.DigLine(-x + x0, -y + y0, x - y, DigDirection.Right, offset);  // From[-1, 0] CCW
                this.DigLine(y + x0, x + y0, x - y - 1, DigDirection.Down, offset);     // From[0, 1] CW
                this.DigLine(-y + x0, x + y0, x - y - 1, DigDirection.Down, offset);    // From[0, 1] CCW
                this.DigLine(y + x0, -x + y0, x - y - 1, DigDirection.Up, offset);      // From[0, -1] CCW
                this.DigLine(-y + x0, -x + y0, x - y - 1, DigDirection.Up, offset);     // From[0, -1] CW

                // Increment y
                y++;

                // Decrement x if it is closer to the point to the left (from perspective of 1st octant)
                radiusError += changeY;
                changeY += 2;
                if ((radiusError << 1) + changeX > 0)
                {
                    x--;
                    radiusError += changeX;
                    changeX += 2;
                }
            }
        }

        /// <summary>
        /// Dig a line.
        /// </summary>
        /// <param name="originX">The X origin.</param>
        /// <param name="originY">The Y origin.</param>
        /// <param name="length">The length.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="offset">The fractional offset between [0, 0] and [1, 1].</param>
        public void DigLine(int originX, int originY, int length, DigDirection direction, Vector2 offset)
        {
            // Sanity check
            if (length <= 0)
            {
                return;
            }

            // Determine the increment values for each step
            int stepX;
            int stepY;
            switch (direction)
            {
                case DigDirection.Right:
                    stepX = 1;
                    stepY = 0;
                    break;

                case DigDirection.Left:
                    stepX = -1;
                    stepY = 0;
                    break;

                case DigDirection.Up:
                    stepX = 0;
                    stepY = 1;
                    break;

                case DigDirection.Down:
                    stepX = 0;
                    stepY = -1;
                    break;

                default:
                    throw new ArgumentException("direction");
            }

            // Step through each point in the line
            int dX = 0;
            int dY = 0;
            for (int l = 0; l <= length; l++)
            {
                this.DigPoint(new Vector2I(originX + dX, originY + dY), offset);
                dX += stepX;
                dY += stepY;
            }
        }
    }
}