// ----------------------------------------------------------------------------
// <copyright file="RectangleF.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Common
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A floating-point rectangle.
    /// </summary>
    public class RectangleF
    {
        #region Private Variables

        /// <summary>
        /// The top left position.
        /// </summary>
        private Vector2 topLeft;

        /// <summary>
        /// The bottom right position.
        /// </summary>
        private Vector2 bottomRight;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RectangleF class.
        /// </summary>
        /// <param name="topLeft">The top left position.</param>
        /// <param name="bottomRight">The bottom right position.</param>
        public RectangleF(Vector2 topLeft, Vector2 bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }

        /// <summary>
        /// Initializes a new instance of the RectangleF class.
        /// </summary>
        /// <param name="x">The top left x-coordinate.</param>
        /// <param name="y">The top left y-coordinate.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            this.topLeft = new Vector2(x, y);
            this.bottomRight = new Vector2(x + width, y + height);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the top left position.
        /// </summary>
        public Vector2 TopLeft
        {
            get
            {
                return this.topLeft;
            }

            set
            {
                this.topLeft = value;
            }
        }

        /// <summary>
        /// Gets or sets the top right position.
        /// </summary>
        public Vector2 TopRight
        {
            get
            {
                return new Vector2(this.bottomRight.X, this.topLeft.Y);
            }

            set
            {
                this.bottomRight.X = value.X;
                this.topLeft.Y = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the bottom left position.
        /// </summary>
        public Vector2 BottomLeft
        {
            get
            {
                return new Vector2(this.topLeft.X, this.bottomRight.Y);
            }

            set
            {
                this.topLeft.X = value.X;
                this.bottomRight.Y = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the bottom right position.
        /// </summary>
        public Vector2 BottomRight
        {
            get
            {
                return this.bottomRight;
            }

            set
            {
                this.bottomRight = value;
            }
        }

        /// <summary>
        /// Gets the center position.
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return Vector2.Divide(Vector2.Add(this.TopLeft, this.BottomRight), 2);
            }
        }

        /// <summary>
        /// Gets the y-coordinate of the top side.
        /// </summary>
        public float Top
        {
            get
            {
                return this.topLeft.Y;
            }
        }

        /// <summary>
        /// Gets the x-coordinate of the left side.
        /// </summary>
        public float Left
        {
            get
            {
                return this.topLeft.X;
            }
        }

        /// <summary>
        /// Gets the y-coordinate of the bottom side.
        /// </summary>
        public float Bottom
        {
            get
            {
                return this.bottomRight.Y;
            }
        }

        /// <summary>
        /// Gets the x-coordinate of the right side.
        /// </summary>
        public float Right
        {
            get
            {
                return this.bottomRight.X;
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public float Width
        {
            get
            {
                return this.bottomRight.X - this.topLeft.X;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public float Height
        {
            get
            {
                return this.bottomRight.Y - this.topLeft.Y;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determine whether this rectangle contains the given point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>True if this rectangle contains the point.</returns>
        public bool Contains(Vector2 point)
        {
            return
                this.TopLeft.X <= point.X && this.BottomRight.X >= point.X &&
                this.TopLeft.Y <= point.Y && this.BottomRight.Y >= point.Y;
        }

        /// <summary>
        /// Determine whether this rectangle intersects with the given rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to test.</param>
        /// <returns>True if this rectangle intersects with the given rectangle.</returns>
        public bool Intersects(RectangleF rectangle)
        {
            return
                this.Bottom >= rectangle.Top &&
                this.Top <= rectangle.Bottom &&
                this.Right >= rectangle.Left &&
                this.Left <= rectangle.Right;
        }

        #endregion
    }
}