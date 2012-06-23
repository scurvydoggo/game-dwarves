// ----------------------------------------------------------------------------
// <copyright file="Edge.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Light
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// This represents a linear edge which lies between two points.
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Initializes a new instance of the Edge class.
        /// </summary>
        /// <param name="x1">The x coordinate of the first point.</param>
        /// <param name="y1">The y coordinate of the first point.</param>
        /// <param name="x2">The x coordinate of the second point.</param>
        /// <param name="y2">The y coordinate of the second point.</param>
        public Edge(int x1, int y1, int x2, int y2)
            : this(new Point(x1, y1), new Point(x2, y2))
        {
        }

        /// <summary>
        /// Initializes a new instance of the LightFront class.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        public Edge(Point point1, Point point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }

        /// <summary>
        /// Gets or sets the first point.
        /// </summary>
        public Point Point1 { get; set; }

        /// <summary>
        /// Gets or sets the second point.
        /// </summary>
        public Point Point2 { get; set; }
    }
}