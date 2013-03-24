﻿// ----------------------------------------------------------------------------
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
        #region Constructor

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

        #endregion

        #region Enums

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

        #endregion

        #region Properties

        /// <summary>
        /// Gets the terrain.
        /// </summary>
        public VoxelTerrain Terrain { get; private set; }

        /// <summary>
        /// Gets the depth to which digging occurs.
        /// </summary>
        public int DigDepth { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the density of the given point (from z=0 to z=DigDepth).
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="density">The density.</param>
        public void SetDensityPoint(Vector2I position, byte density)
        {
            Vector2I chunk = this.Terrain.ChunkIndex(position.X, position.Y);

            // Get the voxel array
            IVoxels voxels;
            if (this.Terrain.TryGetChunk(chunk, out voxels))
            {
                bool digOccurred = false;
                for (int z = 0; z < this.DigDepth; z++)
                {
                    // Get the voxel
                    Vector2I chunkPos = this.Terrain.WorldToChunk(position);
                    Voxel voxel = voxels[chunkPos.X, chunkPos.Y, z];

                    // Update the voxel density
                    if (voxel.Density < density)
                    {
                        voxel.Density = density;
                        voxels[chunkPos.X, chunkPos.Y, z] = voxel;
                        digOccurred = true;
                    }
                }

                // Flag the chunk as requiring a rebuild
                if (digOccurred)
                {
                    this.Terrain.FlagRebuildRequired(chunk, true);
                }
            }
        }

        /// <summary>
        /// Set the density at each point along a straight line from the start point.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="length">The length.</param>
        /// <param name="axis">The axis along which the line lies.</param>
        /// <param name="direction">The direction to dig.</param>
        /// <param name="density">The density.</param>
        public void SetDensityLine(Vector2I start, int length, Axis axis, Direction direction, byte density)
        {
            if (length < 0)
            {
                return;
            }

            if (axis == Axis.X)
            {
                if (direction == Direction.RightOrUp)
                {
                    for (int x = start.X; x < start.X + length; x++)
                    {
                        this.SetDensityPoint(new Vector2I(x, start.Y), density);
                    }
                }
                else
                {
                    for (int x = start.X; x > start.X - length; x--)
                    {
                        this.SetDensityPoint(new Vector2I(x, start.Y), density);
                    }
                }
            }
            else
            {
                if (direction == Direction.RightOrUp)
                {
                    for (int y = start.Y; y < start.Y + length; y++)
                    {
                        this.SetDensityPoint(new Vector2I(start.X, y), density);
                    }
                }
                else
                {
                    for (int y = start.Y; y > start.Y - length; y--)
                    {
                        this.SetDensityPoint(new Vector2I(start.X, y), density);
                    }
                }
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
            var originI = new Vector2I((int)Math.Round(origin.x), (int)Math.Round(origin.y));

            // Begin at the left-most point of the circle moving to the right for each segment on the circumference
            // 'A' refers to the points in the top-half of the circle; 'B' the bottom-half
            float yAPrev = origin.y;
            float yBPrev = yAPrev;
            float yA, yB;

            // Determine where the circle cuts each cell boundary in the terrain grid
            int x = (int)Math.Ceiling(origin.x - radius);
            float xEnd = origin.x + radius;
            int xBase = x - 1;
            while (x < xEnd)
            {
                // Calculate the Y values where the circle cuts the cell boundary at X
                float dX = x - origin.x;
                float dY = (float)Math.Sqrt(radius2 - (dX * dX));
                yA = dY + origin.y;
                yB = -dY + origin.y;
                
                // Calculate the lower-end of the segment through which the circle cuts the boundary
                xBase = x - 1;
                int yABase = (int)Math.Floor(yA);
                int yBBase = (int)Math.Floor(yB);

                // Step vertically through the cells that the circle passes through before reaching X
                if (((int)dX) <= 0)
                {
                    for (int yAStep = (int)Math.Floor(yAPrev) + 1; yAStep <= yABase; yAStep++)
                    {
                        float dYAStep = yAStep - origin.y;
                        float dXA = (float)Math.Sqrt(radius2 - (dYAStep * dYAStep));
                        float xA = -dXA + origin.x;

                        // Dig along the segment from [xBase, yAStep] to [xBase + 1, yAStep] at xA
                        var o = new Vector2I(originI.X, yAStep);
                        var s = new Vector2I(xBase, yAStep);
                        this.SetDensityLine(o, o.X - s.X - 1, Axis.X, Direction.LeftOrDown, Voxel.DensityMax);
                        this.DigSegment(s, Axis.X, Direction.LeftOrDown, xA - xBase);
                    }

                    for (int yBStep = (int)Math.Floor(yBPrev); yBStep > yBBase; yBStep--)
                    {
                        float dYBStep = yBStep - origin.y;
                        float dXB = (float)Math.Sqrt(radius2 - (dYBStep * dYBStep));
                        float xB = -dXB + origin.x;

                        // Dig along the segment from [xBase, yBStep] to [xBase + 1, yBStep] at xB
                        var o = new Vector2I(originI.X, yBStep);
                        var s = new Vector2I(xBase, yBStep);
                        this.SetDensityLine(o, o.X - s.X - 1, Axis.X, Direction.LeftOrDown, Voxel.DensityMax);
                        this.DigSegment(s, Axis.X, Direction.LeftOrDown, xB - xBase);
                    }
                }
                else if (((int)dX) >= 0)
                {
                    for (int yAStep = (int)Math.Floor(yAPrev); yAStep > yABase; yAStep--)
                    {
                        float dYAStep = yAStep - origin.y;
                        float dXA = (float)Math.Sqrt(radius2 - (dYAStep * dYAStep));
                        float xA = dXA + origin.x;

                        // Dig along the segment from [xBase, yAStep] to [xBase + 1, yAStep] at xA
                        var o = new Vector2I(originI.X, yAStep);
                        var s = new Vector2I(xBase, yAStep);
                        this.SetDensityLine(o, s.X - o.X, Axis.X, Direction.RightOrUp, Voxel.DensityMax);
                        this.DigSegment(s, Axis.X, Direction.RightOrUp, xA - xBase);
                    }

                    for (int yBStep = (int)Math.Floor(yBPrev) + 1; yBStep <= yBBase; yBStep++)
                    {
                        float dYBStep = yBStep - origin.y;
                        float dXB = (float)Math.Sqrt(radius2 - (dYBStep * dYBStep));
                        float xB = dXB + origin.x;

                        // Dig along the segment from [xBase, yBStep] to [xBase + 1, yBStep] at xB
                        var o = new Vector2I(originI.X, yBStep);
                        var s = new Vector2I(xBase, yBStep);
                        this.SetDensityLine(o, s.X - o.X, Axis.X, Direction.RightOrUp, Voxel.DensityMax);
                        this.DigSegment(s, Axis.X, Direction.RightOrUp, xB - xBase);
                    }
                }

                // Dig along the segment from [xBase + 1, yABase] to [xBase + 1, yABase + 1] at yA
                this.DigSegment(new Vector2I(xBase + 1, yABase), Axis.Y, Direction.RightOrUp, yA - yABase);

                // Dig along the segment from [xBase + 1, yBBase] to [xBase + 1, yBBase + 1] at yB
                this.DigSegment(new Vector2I(xBase + 1, yBBase), Axis.Y, Direction.LeftOrDown, yB - yBBase);

                yAPrev = yA;
                yBPrev = yB;
                x++;
            }

            // Dig the last vertical segments of the circle
            for (int yAStep = (int)Math.Floor(yAPrev); yAStep > origin.y; yAStep--)
            {
                float dYAStep = yAStep - origin.y;
                float dXA = (float)Math.Sqrt(radius2 - (dYAStep * dYAStep));
                float xA = dXA + origin.x;

                // Dig along the segment from [xBase, yAStep] to [xBase + 1, yAStep] at xA
                var o = new Vector2I(originI.X, yAStep);
                var s = new Vector2I(xBase, yAStep);
                this.SetDensityLine(o, s.X - o.X, Axis.X, Direction.RightOrUp, Voxel.DensityMax);
                this.DigSegment(s, Axis.X, Direction.RightOrUp, xA - xBase);
            }

            for (int yBStep = (int)Math.Floor(yBPrev) + 1; yBStep <= origin.y; yBStep++)
            {
                float dYBStep = yBStep - origin.y;
                float dXB = (float)Math.Sqrt(radius2 - (dYBStep * dYBStep));
                float xB = dXB + origin.x;

                // Dig along the segment from [xBase, yBStep] to [xBase + 1, yBStep] at xB
                var o = new Vector2I(originI.X, yBStep);
                var s = new Vector2I(xBase, yBStep);
                this.SetDensityLine(o, s.X - o.X, Axis.X, Direction.RightOrUp, Voxel.DensityMax);
                this.DigSegment(s, Axis.X, Direction.RightOrUp, xB - xBase);
            }
        }

        /// <summary>
        /// Dig a segment between two adjacent points in the voxel terrain.
        /// </summary>
        /// <param name="bottomLeft">The bottom-left point of the segment.</param>
        /// <param name="axis">The axis along which the segment lies.</param>
        /// <param name="direction">The direction to dig.</param>
        /// <param name="intersection">A value between 0.0 and 1.0 indicating at what point along the segment the
        /// terrain surface passes through. 0.0 indicates the bottom/left of the segment; 1.0 indicates the top/right
        /// of the segment.</param>
        public void DigSegment(Vector2I bottomLeft, Axis axis, Direction direction, float intersection)
        {
            byte dBottomLeft, dTopRight;
            if (direction == Direction.RightOrUp)
            {
                if (intersection > 0.5f)
                {
                    dBottomLeft = Voxel.DensityMax;
                    dTopRight = (byte)(Voxel.DensityMax * (intersection - 0.5f));
                }
                else
                {
                    dBottomLeft = (byte)(Voxel.DensityMax * (intersection + 0.5f));
                    dTopRight = Voxel.DensityMin;
                }
            }
            else
            {
                if (intersection > 0.5f)
                {
                    dBottomLeft = Voxel.DensityMin;
                    dTopRight = (byte)(Voxel.DensityMax * (intersection - 0.5f));
                }
                else
                {
                    dBottomLeft = (byte)(Voxel.DensityMax * (0.5f - intersection));
                    dTopRight = Voxel.DensityMax;
                }
            }

            this.SetDensityPoint(bottomLeft, dBottomLeft);
            if (axis == Axis.X)
            {
                this.SetDensityPoint(new Vector2I(bottomLeft.X + 1, bottomLeft.Y), dTopRight);
            }
            else
            {
                this.SetDensityPoint(new Vector2I(bottomLeft.X, bottomLeft.Y + 1), dTopRight);
            }
        }

        #endregion
    }
}