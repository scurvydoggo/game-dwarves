// ----------------------------------------------------------------------------
// <copyright file="TerrainMutator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Mutation
{
    using System;
    using Dwarves.Core.Jobs;
    using Dwarves.Core.Math;
    using UnityEngine;

    /// <summary>
    /// Mutates voxel terrain.
    /// </summary>
    public class TerrainMutator
    {
        #region Private Variables

        /// <summary>
        /// The terrain.
        /// </summary>
        private DwarfTerrain terrain;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initialises a new instance of the TerrainMutator class.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        public TerrainMutator(DwarfTerrain terrain)
        {
            this.terrain = terrain;
        }

        #endregion Constructor

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

        #endregion Enums

        #region Public Methods

        /// <summary>
        /// Dig a circle in the terrain with the given origin and radius.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="radius">The radius.</param>
        public void DigCircle(Vector2 origin, float radius)
        {
            Vector2I originI = new Vector2I((int)Math.Round(origin.x), (int)Math.Round(origin.y));
            int radiusI = (int)radius;
            Vector2I chunk = Metrics.ChunkIndex(originI.X, originI.Y);

            // Get the affected chunks
            var worldBounds = new RectangleI(
                (int)(origin.x - radius - 0.5f) - 1,
                (int)(origin.y + radius + 0.5f) + 1,
                (int)((radius * 2) + 0.5f) + 3,
                (int)((radius * 2) + 0.5f) + 3);
            RectangleI chunkBounds = Metrics.WorldToChunk(worldBounds);
            Vector2I[] chunks = TerrainChunk.GetChunks(chunkBounds);
            SynchronisedUpdate toSync = chunks.Length > 1 ? new SynchronisedUpdate(chunks) : null;

            // Enqueue the job
            JobSystem.Instance.Scheduler.BeginEnqueueChunks();
            try
            {
                if (JobSystem.Instance.Scheduler.ForAllChunks(
                    chunks,
                    (q) => q.State.CanDigCircle(chunk, originI, radiusI),
                    MissingQueue.Skip))
                {
                    JobSystem.Instance.Scheduler.EnqueueChunks(
                        () => this.DigCircleJob(origin, radius),
                        (q) => q.State.ReserveDigCircle(chunk, originI, radiusI, toSync),
                        (q) => q.State.UnreserveDigCircle(chunk, originI, radiusI),
                        false,
                        chunks);
                }
            }
            finally
            {
                JobSystem.Instance.Scheduler.EndEnqueueChunks();
            }
        }

        #endregion Public Methods

        #region Mutation Methods

        /// <summary>
        /// Dig a circle in the terrain with the given origin and radius.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="radius">The radius.</param>
        private void DigCircleJob(Vector2 origin, float radius)
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
                        this.SetDensityLine(o, o.X - s.X - 1, Axis.X, Direction.LeftOrDown, TerrainPoint.DensityMax);
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
                        this.SetDensityLine(o, o.X - s.X - 1, Axis.X, Direction.LeftOrDown, TerrainPoint.DensityMax);
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
                        this.SetDensityLine(o, s.X - o.X, Axis.X, Direction.RightOrUp, TerrainPoint.DensityMax);
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
                        this.SetDensityLine(o, s.X - o.X, Axis.X, Direction.RightOrUp, TerrainPoint.DensityMax);
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
                xBase = x - 1;
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
                this.SetDensityLine(o, s.X - o.X, Axis.X, Direction.RightOrUp, TerrainPoint.DensityMax);
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
                this.SetDensityLine(o, s.X - o.X, Axis.X, Direction.RightOrUp, TerrainPoint.DensityMax);
                this.DigSegment(s, Axis.X, Direction.RightOrUp, xB - xBase);
            }
        }

        #endregion Mutation Methods

        #region Helper Methods

        /// <summary>
        /// Dig a segment between two adjacent points in the voxel terrain.
        /// </summary>
        /// <param name="bottomLeft">The bottom-left point of the segment.</param>
        /// <param name="axis">The axis along which the segment lies.</param>
        /// <param name="direction">The direction to dig.</param>
        /// <param name="intersection">A value between 0.0 and 1.0 indicating at what point along the segment the
        /// terrain surface passes through. 0.0 indicates the bottom/left of the segment; 1.0 indicates the top/right
        /// of the segment.</param>
        private void DigSegment(Vector2I bottomLeft, Axis axis, Direction direction, float intersection)
        {
            byte dBottomLeft, dTopRight;
            if (direction == Direction.RightOrUp)
            {
                if (intersection > 0.5f)
                {
                    dBottomLeft = TerrainPoint.DensityMax;
                    dTopRight = (byte)(TerrainPoint.DensityMax * (intersection - 0.5f));
                }
                else
                {
                    dBottomLeft = (byte)(TerrainPoint.DensityMax * (intersection + 0.5f));
                    dTopRight = TerrainPoint.DensityMin;
                }
            }
            else
            {
                if (intersection > 0.5f)
                {
                    dBottomLeft = TerrainPoint.DensityMin;
                    dTopRight = (byte)(TerrainPoint.DensityMax * (1.5f - intersection));
                }
                else
                {
                    dBottomLeft = (byte)(TerrainPoint.DensityMax * (0.5f - intersection));
                    dTopRight = TerrainPoint.DensityMax;
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

        /// <summary>
        /// Set the foreground density of the given point.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="density">The density.</param>
        private void SetDensityPoint(Vector2I position, byte density)
        {
            Vector2I chunkIndex = Metrics.ChunkIndex(position.X, position.Y);

            // Get the chunk
            TerrainChunk chunk;
            if (this.terrain.TryGetChunk(chunkIndex, out chunk))
            {
                Vector2I chunkPos = Metrics.WorldToChunk(position);
                TerrainPoint point = chunk.Points[chunkPos.X, chunkPos.Y];
                if (point != null)
                {
                    if (point.Foreground < density)
                    {
                        point.Foreground = density;
                    }
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
        private void SetDensityLine(Vector2I start, int length, Axis axis, Direction direction, byte density)
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

        #endregion Helper Methods
    }
}