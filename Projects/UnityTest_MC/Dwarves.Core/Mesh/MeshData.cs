// ----------------------------------------------------------------------------
// <copyright file="MeshData.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Mesh
{
    using UnityEngine;

    /// <summary>
    /// Contains data used in constructing unity meshes.
    /// </summary>
    public class MeshData
    {
        /// <summary>
        /// Initialises a new instance of the MeshData class.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="triangles">The triangle indices.</param>
        /// <param name="uvs">The UV coordinates.</param>
        /// <param name="normals">The normals.</param>
        /// <param name="colors">The colours.</param>
        public MeshData(Vector3[] vertices, int[] triangles, Vector2[] uvs, object[] normals, object[] colors)
        {
            this.Vertices = vertices;
            this.TriangleIndices = triangles;
            this.UVs = uvs;
            this.Normals = normals;
            this.Colors = colors;
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        public Vector3[] Vertices { get; private set; }

        /// <summary>
        /// Gets the triangle indices.
        /// </summary>
        public int[] TriangleIndices { get; private set; }

        /// <summary>
        /// Gets the UV coordinates.
        /// </summary>
        public Vector2[] UVs { get; private set; }

        /// <summary>
        /// Gets the normals.
        /// </summary>
        public object[] Normals { get; private set; }

        /// <summary>
        /// Gets the colours.
        /// </summary>
        public object[] Colors { get; private set; }
    }
}