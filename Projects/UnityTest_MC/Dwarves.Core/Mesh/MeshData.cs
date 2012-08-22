// ----------------------------------------------------------------------------
// <copyright file="CubeType.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Mesh
{
    /// <summary>
    /// Contains data used in contructing unity meshes.
    /// </summary>
    public class MeshData
    {
        /// <summary>
        /// Initializes a new instance of the CubeData class.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="triangles">The triangle indices.</param>
        /// <param name="uvs">The UV coordinates.</param>
        /// <param name="normals">The normals.</param>
        /// <param name="colors">The colors.</param>
        public MeshData(object[] vertices, int[] triangles, object[] uvs, object[] normals, object[] colors)
        {
            this.Vertices = vertices;
            this.Triangles = triangles;
            this.UVs = uvs;
            this.Normals = normals;
            this.Colors = colors;
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        public object[] Vertices { get; private set; }

        /// <summary>
        /// Gets the triangle indices.
        /// </summary>
        public int[] Triangles { get; private set; }

        /// <summary>
        /// Gets the UV coordinates.
        /// </summary>
        public object[] UVs { get; private set; }

        /// <summary>
        /// Gets the normals.
        /// </summary>
        public object[] Normals { get; private set; }

        /// <summary>
        /// Gets the colors.
        /// </summary>
        public object[] Colors { get; private set; }
    }
}