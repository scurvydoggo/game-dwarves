// ----------------------------------------------------------------------------
// <copyright file="Vector3I.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Math
{
    using UnityEngine;

    /// <summary>
    /// An 3D integer vector.
    /// </summary>
    public struct Vector3I
    {
        #region Constants

        /// <summary>
        /// A zero vector.
        /// </summary>
        public static readonly Vector3I Zero = new Vector3I(0, 0, 0);

        /// <summary>
        /// A one vector.
        /// </summary>
        public static readonly Vector3I One = new Vector3I(1, 1, 1);

        /// <summary>
        /// A unit vector in the X direction.
        /// </summary>
        public static readonly Vector3I UnitX = new Vector3I(1, 0, 0);

        /// <summary>
        /// A unit vector in the Y direction.
        /// </summary>
        public static readonly Vector3I UnitY = new Vector3I(0, 1, 0);

        /// <summary>
        /// A unit vector in the Z direction.
        /// </summary>
        public static readonly Vector3I UnitZ = new Vector3I(0, 0, 1);

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Initialises a new instance of the Vector3I struct.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        public Vector3I(int x, int y, int z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
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

        /// <summary>
        /// Gets or sets the z position.
        /// </summary>
        public int Z { get; set; }

        #endregion Properties

        #region Operators

        /// <summary>
        /// Cast a floating point vector to an integer vector.
        /// </summary>
        /// <param name="v">The floating point vector.</param>
        /// <returns>The integer vector.</returns>
        public static explicit operator Vector3I(Vector3 v)
        {
            return new Vector3I((int)v.x, (int)v.y, (int)v.z);
        }

        /// <summary>
        /// Perform the add operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector3I operator +(Vector3I v0, Vector3I v1)
        {
            return new Vector3I(v0.X + v1.X, v0.Y + v1.Y, v0.Z + v1.Z);
        }

        /// <summary>
        /// Perform the subtract operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector3I operator -(Vector3I v0, Vector3I v1)
        {
            return new Vector3I(v0.X - v1.X, v0.Y - v1.Y, v0.Z - v1.Z);
        }

        /// <summary>
        /// Perform the divide operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector3I operator /(Vector3I v0, Vector3I v1)
        {
            return new Vector3I(v0.X / v1.X, v0.Y / v1.Y, v0.Z / v1.Z);
        }

        /// <summary>
        /// Perform the modulo operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="i">The modulo value.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector3I operator %(Vector3I v0, int i)
        {
            return new Vector3I(v0.X % i, v0.Y % i, v0.Z % i);
        }

        /// <summary>
        /// Perform the multiply operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector3I operator *(Vector3I v0, Vector3I v1)
        {
            return new Vector3I(v0.X * v1.X, v0.Y * v1.Y, v0.Z * v1.Z);
        }

        /// <summary>
        /// Perform the multiply operator.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector3I operator *(Vector3I v, int s)
        {
            return new Vector3I(v.X * s, v.Y * s, v.Z * s);
        }

        /// <summary>
        /// Perform the multiply operator.
        /// </summary>
        /// <param name="s">The scalar.</param>
        /// <param name="v">The vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector3I operator *(int s, Vector3I v)
        {
            return v * s;
        }

        /// <summary>
        /// Perform the divide operator.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector3I operator /(Vector3I v, int s)
        {
            return new Vector3I(v.X / s, v.Y / s, v.Z / s);
        }

        /// <summary>
        /// Perform the less than operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>True if the first vector is less than the second vector.</returns>
        public static bool operator <(Vector3I v0, Vector3I v1)
        {
            return v0.X < v1.X && v0.Y < v1.Y && v0.Z < v1.Z;
        }

        /// <summary>
        /// Perform the greater than operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>True if the first vector is greater than the second vector.</returns>
        public static bool operator >(Vector3I v0, Vector3I v1)
        {
            return v0.X > v1.X && v0.Y > v1.Y && v0.Z > v1.Z;
        }

        /// <summary>
        /// Perform the less than or equal operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>True if the first vector is less than or equal the second vector.</returns>
        public static bool operator <=(Vector3I v0, Vector3I v1)
        {
            return v0.X <= v1.X && v0.Y <= v1.Y && v0.Z <= v1.Z;
        }

        /// <summary>
        /// Perform the greater than or equal operator.
        /// </summary>
        /// <param name="v0">The first vector.</param>
        /// <param name="v1">The second vector.</param>
        /// <returns>True if the first vector is greater than or equal the second vector.</returns>
        public static bool operator >=(Vector3I v0, Vector3I v1)
        {
            return v0.X >= v1.X && v0.Y >= v1.Y && v0.Z >= v1.Z;
        }

        #endregion Operators

        #region Equality

        /// <summary>
        /// Determine if the vector is equal.
        /// </summary>
        /// <param name="other">The vector to test.</param>
        /// <returns>True if the vectors are equal.</returns>
        public bool Equals(Vector3I other)
        {
            return other.X == this.X && other.Y == this.Y && other.Z == this.Z;
        }

        /// <summary>
        /// Determine if the object is equal.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else if (obj.GetType() != typeof(Vector3I))
            {
                return false;
            }
            else
            {
                return this.Equals((Vector3I)obj);
            }
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
                result = (result * 397) ^ this.Z;
                return result;
            }
        }

        #endregion Equality
    }
}