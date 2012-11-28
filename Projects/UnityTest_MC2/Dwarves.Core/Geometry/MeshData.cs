// ----------------------------------------------------------------------------
// <copyright file="MeshData.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Geometry
{
    using UnityEngine;

    /// <summary>
    /// A mesh.
    /// </summary>
    public class MeshData
    {
        /// <summary>
        /// Initialises a new instance of the MeshData class.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="indices">The triangle indices.</param>
        public MeshData(Vector3[] vertices, ushort[] indices)
        {
            this.Vertices = vertices;
            this.Indices = indices;
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        public Vector3[] Vertices { get; private set; }

        /// <summary>
        /// Gets the triangle indices.
        /// </summary>
        public ushort[] Indices { get; private set; }
    }
}