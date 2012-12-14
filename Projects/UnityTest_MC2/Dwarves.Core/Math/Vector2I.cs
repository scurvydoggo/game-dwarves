// ----------------------------------------------------------------------------
// <copyright file="Vector2I.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Math
{
    using UnityEngine;

    /// <summary>
    /// An 2D integer vector.
    /// </summary>
    public struct Vector2I
    {
        #region Constants

        /// <summary>
        /// A vector with zero values.
        /// </summary>
        public static readonly Vector2I Zero = new Vector2I(0, 0);

        /// <summary>
        /// A one vector.
        /// </summary>
        public static readonly Vector2I One = new Vector2I(1, 1);

        /// <summary>
        /// A unit vector in the X direction.
        /// </summary>
        public static readonly Vector2I UnitX = new Vector2I(1, 0);

        /// <summary>
        /// A unit vector in the Y direction.
        /// </summary>
        public static readonly Vector2I UnitY = new Vector2I(0, 1);

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Initialises a new instance of the Vector2I struct.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        public Vector2I(int x, int y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        public int Y { get; set; }

        #endregion Properties

        #region Operators

        /// <summary>
        /// Cast a floating point vector to an integer vector.
        /// </summary>
        /// <param name="v">The floating point vector.</param>
        /// <returns>The integer vector.</returns>
        public static explicit operator Vector2I(Vector2 v)
        {
            return new Vector2I((int)v.x, (int)v.y);
        }

        /// <summary>
        /// Perform the add operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2I operator +(Vector2I v0, Vector2I v1)
        {
            return new Vector2I(v0.X + v1.X, v0.Y + v1.Y);
        }

        /// <summary>
        /// Perform the subtract operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2I operator -(Vector2I v0, Vector2I v1)
        {
            return new Vector2I(v0.X - v1.X, v0.Y - v1.Y);
        }

        /// <summary>
        /// Perform the divide operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2I operator /(Vector2I v0, Vector2I v1)
        {
            return new Vector2I(v0.X / v1.X, v0.Y / v1.Y);
        }

        /// <summary>
        /// Perform the modulo operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="i">The modulo value.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2I operator %(Vector2I v0, int i)
        {
            return new Vector2I(v0.X % i, v0.Y % i);
        }

        /// <summary>
        /// Perform the multiply operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2I operator *(Vector2I v0, Vector2I v1)
        {
            return new Vector2I(v0.X * v1.X, v0.Y * v1.Y);
        }

        /// <summary>
        /// Perform the multiply operator.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2I operator *(Vector2I v, int s)
        {
            return new Vector2I(v.X * s, v.Y * s);
        }

        /// <summary>
        /// Perform the multiply operator.
        /// </summary>
        /// <param name="s">The scalar.</param>
        /// <param name="v">The vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2I operator *(int s, Vector2I v)
        {
            return v * s;
        }

        /// <summary>
        /// Perform the divide operator.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2I operator /(Vector2I v, int s)
        {
            return new Vector2I(v.X / s, v.Y / s);
        }

        /// <summary>
        /// Perform the less than operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>True if the first vector is less than the second vector.</returns>
        public static bool operator <(Vector2I v0, Vector2I v1)
        {
            return v0.X < v1.X && v0.Y < v1.Y;
        }

        /// <summary>
        /// Perform the greater than operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>True if the first vector is greater than the second vector.</returns>
        public static bool operator >(Vector2I v0, Vector2I v1)
        {
            return v0.X > v1.X && v0.Y > v1.Y;
        }

        /// <summary>
        /// Perform the less than or equal operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>True if the first vector is less than or equal the second vector.</returns>
        public static bool operator <=(Vector2I v0, Vector2I v1)
        {
            return v0.X <= v1.X && v0.Y <= v1.Y;
        }

        /// <summary>
        /// Perform the greater than or equal operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>True if the first vector is greater than or equal the second vector.</returns>
        public static bool operator >=(Vector2I v0, Vector2I v1)
        {
            return v0.X >= v1.X && v0.Y >= v1.Y;
        }

        #endregion Operators

        #region Equality

        /// <summary>
        /// Determine if the vector is equal.
        /// </summary>
        /// <param name="other">The vector to test.</param>
        /// <returns>True if the vectors are equal.</returns>
        public bool Equals(Vector2I other)
        {
            return other.X == this.X && other.Y == this.Y;
        }

        /// <summary>
        /// Determine if the object is equal.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj is Vector2I && this.Equals((Vector2I)obj);
        }

        /// <summary>
        /// Get the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.X;
                result = (result * 397) ^ this.Y;
                return result;
            }
        }

        #endregion Equality
    }
}