// ----------------------------------------------------------------------------
// <copyright file="Program.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestApplication
{
    using Dwarves.Test;
    using UnityEngine;

    /// <summary>
    /// Entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">The application args.</param>
        public static void Main(string[] args)
        {
            var test = new MarchingCubesTest(128);

            Vector3[] vertices;
            int[] triangleIndices;
            test.Load(out vertices, out triangleIndices);
        }
    }
}