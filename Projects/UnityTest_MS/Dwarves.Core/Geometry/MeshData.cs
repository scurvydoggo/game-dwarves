// ----------------------------------------------------------------------------
// <copyright file="MeshData.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Geometry
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A mesh.
    /// </summary>
    public class MeshData
    {
        /// <summary>
        /// Initialises a new instance of the MeshData class.
        /// </summary>
        public MeshData()
        {
            this.Vertices = new List<Vector3>();
            this.Normals = new List<Vector3>();
            this.Indices = new List<int>();
            this.Light = new List<Color>();
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        public List<Vector3> Vertices { get; private set; }

        /// <summary>
        /// Gets the normal vectors.
        /// </summary>
        public List<Vector3> Normals { get; private set; }

        /// <summary>
        /// Gets the triangle indices.
        /// </summary>
        public List<int> Indices { get; private set; }

        /// <summary>
        /// Gets the light value at each vertex.
        /// </summary>
        public List<Color> Light { get; private set; }

        /// <summary>
        /// Clear the mesh data.
        /// </summary>
        public void Clear()
        {
            this.Vertices.Clear();
            this.Normals.Clear();
            this.Indices.Clear();
            this.Light.Clear();
        }

        /// <summary>
        /// Gets the index of the latest vertex that was added.
        /// </summary>
        /// <returns>The index.</returns>
        public ushort LatestVertexIndex()
        {
            return (ushort)(this.Vertices.Count - 1);
        }
    }
}