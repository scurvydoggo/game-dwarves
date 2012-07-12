// ----------------------------------------------------------------------------
// <copyright file="Edge.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Light
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Indicates the edge orientation.
    /// </summary>
    public enum EdgeOrientation
    {
        /// <summary>
        /// A horizontal edge.
        /// </summary>
        Horizontal,

        /// <summary>
        /// A vertical edge.
        /// </summary>
        Vertical,

        /// <summary>
        /// An angled edge.
        /// </summary>
        Angled,

        /// <summary>
        /// An edge where the end points are the same position.
        /// </summary>
        Point
    }

    /// <summary>
    /// This represents a linear edge which lies between two points.
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// The first point.
        /// </summary>
        private Point point1;

        /// <summary>
        /// The second point.
        /// </summary>
        private Point point2;

        /// <summary>
        /// Initializes a new instance of the Edge class.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        public Edge(Point point1, Point point2)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.UpdateOrientationValue();
        }

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
        /// Gets or sets the first point.
        /// </summary>
        public Point Point1
        {
            get
            {
                return this.point1;
            }

            set
            {
                if (this.point1 != value)
                {
                    this.point1 = value;
                    this.UpdateOrientationValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets the second point.
        /// </summary>
        public Point Point2
        {
            get
            {
                return this.point2;
            }

            set
            {
                if (this.point2 != value)
                {
                    this.point2 = value;
                    this.UpdateOrientationValue();
                }
            }   
        }

        /// <summary>
        /// Gets the orientation of the edge.
        /// </summary>
        public EdgeOrientation Orientation { get; private set; }

        /// <summary>
        /// Update the value of the orientation property.
        /// </summary>
        private void UpdateOrientationValue()
        {
            if (this.point1.Equals(this.point2))
            {
                this.Orientation = EdgeOrientation.Point;
            }
            else if (this.point1.X == this.point2.X)
            {
                this.Orientation = EdgeOrientation.Vertical;
            }
            else if (this.point1.Y == this.point2.Y)
            {
                this.Orientation = EdgeOrientation.Horizontal;
            }
            else
            {
                this.Orientation = EdgeOrientation.Angled;
            }
        }
    }
}