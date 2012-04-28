// ----------------------------------------------------------------------------
// <copyright file="Square.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Common
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A square.
    /// </summary>
    public struct Square
    {
        #region Private Variables

        /// <summary>
        /// The length of the square.
        /// </summary>
        private int length;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Square struct.
        /// </summary>
        /// <param name="x">The top left x-coordinate.</param>
        /// <param name="y">The top left y-coordinate.</param>
        /// <param name="length">The length of the square.</param>
        public Square(int x, int y, int length)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Length = length;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the top left x-coordinate of the square.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the top left y-coordinate of the square.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the length of the square.
        /// </summary>
        public int Length
        {
            get
            {
                return this.length;
            }

            set
            {
                if (this.length < 0)
                {
                    throw new ArgumentOutOfRangeException("Length cannot be less than zero.");
                }

                this.length = value;
            }
        }

        /// <summary>
        /// Gets or sets the top left position of the square.
        /// </summary>
        public Point Location
        {
            get
            {
                return new Point(this.X, this.Y);
            }

            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        /// <summary>
        /// Gets the center position of the square.
        /// </summary>
        public Point Center
        {
            get
            {
                return new Point(this.X + (int)(this.Length / 2), this.Y + (int)(this.Length / 2));
            }
        }

        /// <summary>
        /// Gets the y-coordinate of the top side of the square.
        /// </summary>
        public int Top
        {
            get
            {
                return this.Y;
            }
        }

        /// <summary>
        /// Gets the x-coordinate of the left side of the square.
        /// </summary>
        public int Left
        {
            get
            {
                return this.X;
            }
        }

        /// <summary>
        /// Gets the y-coordinate of the bottom side of the square.
        /// </summary>
        public int Bottom
        {
            get
            {
                return this.Y + this.Length;
            }
        }

        /// <summary>
        /// Gets the x-coordinate of the right side of the square.
        /// </summary>
        public int Right
        {
            get
            {
                return this.X + this.Length;
            }
        }

        #endregion

        #region Method Overrides

        /// <summary>
        /// Gets a value indicating whether the two squares are equal.
        /// </summary>
        /// <param name="s1">The first square.</param>
        /// <param name="s2">The second square.</param>
        /// <returns>True if the squares are equal.</returns>
        public static bool operator ==(Square s1, Square s2)
        {
            return s1.X == s2.X && s1.Y == s2.Y && s1.Length == s2.Length;
        }

        /// <summary>
        /// Gets a value indicating whether the two squares are not equal.
        /// </summary>
        /// <param name="s1">The first square.</param>
        /// <param name="s2">The second square.</param>
        /// <returns>True if the squares are not equal.</returns>
        public static bool operator !=(Square s1, Square s2)
        {
            return !(s1 == s2);
        }

        /// <summary>
        /// Gets a value indicating whether this square is equal to the given object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>True if the object is equal to this square.</returns>
        public override bool Equals(object obj)
        {
            return obj is Square && this == (Square)obj;
        }

        /// <summary>
        /// Gets the hash code for this square.
        /// </summary>
        /// <returns> A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Length.GetHashCode();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determine whether this square contains the given point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>True if this square contains the point.</returns>
        public bool Contains(Point point)
        {
            return
                point.Y >= this.Top &&
                point.X >= this.Left &&
                point.Y <= this.Bottom &&
                point.X <= this.Right;
        }

        /// <summary>
        /// Determine whether this square contains the given square.
        /// </summary>
        /// <param name="other">The square to test.</param>
        /// <returns>True if this square contains the given square.</returns>
        public bool Contains(Square other)
        {
            return
                other.Top >= this.Top &&
                other.Left >= this.Left &&
                other.Bottom <= this.Bottom &&
                other.Right <= this.Right;
        }

        /// <summary>
        /// Determine whether this square contains the given rectangle.
        /// </summary>
        /// <param name="other">The rectangle to test.</param>
        /// <returns>True if this square contains the given rectangle.</returns>      
        public bool Contains(Rectangle other)
        {
            return
                other.Top >= this.Top &&
                other.Left >= this.Left &&
                other.Bottom <= this.Bottom &&
                other.Right <= this.Right;
        }

        /// <summary>
        /// Determine whether this square intersects the given square.
        /// </summary>
        /// <param name="other">The square to test.</param>
        /// <returns>True if this square intersects the given square.</returns>
        public bool Intersects(Square other)
        {
            return
                !(this.Bottom < other.Top ||
                this.Top > other.Bottom ||
                this.Right < other.Left ||
                this.Left > other.Right);
        }

        /// <summary>
        /// Determine whether this square intersects the given rectangle.
        /// </summary>
        /// <param name="other">The rectangle to test.</param>
        /// <returns>True if this square intersects the given rectangle.</returns>
        public bool Intersects(Rectangle other)
        {
            return
                !(this.Bottom < other.Top ||
                this.Top > other.Bottom ||
                this.Right < other.Left ||
                this.Left > other.Right);
        }

        /// <summary>
        /// Gets a square for the top left quadrant.
        /// </summary>
        /// <returns>The square.</returns>
        public Square GetTopLeftQuadrant()
        {
            return new Square(this.X, this.Y, this.Length / 2);
        }

        /// <summary>
        /// Gets a square for the top right quadrant.
        /// </summary>
        /// <returns>The square.</returns>
        public Square GetTopRightQuadrant()
        {
            int half = this.Length / 2;
            return new Square(this.X + half, this.Y, half);
        }

        /// <summary>
        /// Gets a square for the bottom left quadrant.
        /// </summary>
        /// <returns>The square.</returns>
        public Square GetBottomLeftQuadrant()
        {
            int half = this.Length / 2;
            return new Square(this.X, this.Y + half, half);
        }

        /// <summary>
        /// Gets a square for the bottom right quadrant.
        /// </summary>
        /// <returns>The square.</returns>
        public Square GetBottomRightQuadrant()
        {
            int half = this.Length / 2;
            return new Square(this.X + half, this.Y + half, half);
        }

        #endregion
    }
}