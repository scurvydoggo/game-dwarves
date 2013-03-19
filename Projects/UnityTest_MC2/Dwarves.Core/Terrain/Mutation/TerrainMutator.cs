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
        /// The axis on which the grid segment lies.
        /// </summary>
        public enum Axis
        {
            /// <summary>
            /// The X axis.
            /// </summary>
            X,

            /// <summary>
            /// The Y axis.
            /// </summary>
            Y
        }

        /// <summary>
        /// The direction to dig.
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Dig to the right or upwards, depending on the axis.
            /// </summary>
            RightOrUp,

            /// <summary>
            /// Dig to the left or downwards, depending on the axis.
            /// </summary>
            LeftOrDown
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
        /// Set the density of the given point (from z=0 to z=DigDepth).
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="density">The density.</param>
        public void SetDensity(Vector2I position, byte density)
        {
            Vector2I chunk = this.Terrain.ChunkIndex(position.X, position.Y);

            // Get the voxel array
            IVoxels voxels;
            if (this.Terrain.TryGetChunk(chunk, out voxels))
            {
                for (int z = 0; z < this.DigDepth; z++)
                {
                    // Get the voxel
                    Vector2I chunkPos = this.Terrain.WorldToChunk(position);
                    Voxel voxel = voxels[chunkPos.X, chunkPos.Y, z];

                    // Update the voxel density
                    voxel.Density = density;
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
                float dY = (float)Math.Sqrt(radius2 - (dX * dX));
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
                    float dXA = (float)Math.Sqrt(radius2 - (dYAStep * dYAStep));
                    float xA = origin.x + (x < 0 ? -dXA : dXA);

                    // TODO: Dig along the segment from [xBase, yAStep] to [xBase + 1, yAStep] at xA
                }

                // Step vertically through the cells that the circle passes through before reaching X
                for (int yBStep = (int)Math.Floor(yBPrev) - 1; yBStep > yBBase; yBStep--)
                {
                    // Calculate the X value where the circle cuts the cell boundary at yBGrid
                    float dYBStep = yBStep - origin.y;
                    float dXB = (float)Math.Sqrt(radius2 - (dYBStep * dYBStep));
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
        /// Dig a segment between two adjacent points in the voxel terrain.
        /// </summary>
        /// <param name="bottomLeft">The bottom-left point of the segment.</param>
        /// <param name="value">The value between 0.0 and 1.0, indicating at what point along the segment the terrain
        /// surface passes through.</param>
        /// <param name="axis">The axis along which the segment lies.</param>
        /// <param name="direction">The direction to dig.</param>
        public void DigSegment(Vector2I bottomLeft, float value, Axis axis, Direction direction)
        {
            throw new NotImplementedException();
        }
    }
}