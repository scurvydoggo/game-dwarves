// ----------------------------------------------------------------------------
// <copyright file="TerrainPoint.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using Dwarves.Core.Lighting;

    /// <summary>
    /// A point in the terrain. The point represents a column of voxels along the Z-axis when viewing a 2D cross
    /// section of the terrain from the side.
    /// </summary>
    public class TerrainPoint
    {
        /// <summary>
        /// The minimum density value. This lies inside the surface.
        /// </summary>
        public const byte DensityMin = 0x00;

        /// <summary>
        /// The maximum density value. This lies outside the surface.
        /// </summary>
        public const byte DensityMax = 0xFF;

        /// <summary>
        /// The density at which the surface lies (aka the isolevel).
        /// </summary>
        public const byte DensitySurface = 0x7F;

        /// <summary>
        /// Initialises a new instance of the TerrainPoint class.
        /// </summary>
        public TerrainPoint()
            : this(TerrainPoint.DensityMin, TerrainPoint.DensityMin, TerrainMaterial.Undefined, null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the TerrainPoint class.
        /// </summary>
        /// <param name="foreground">The density of the foreground.</param>
        /// <param name="background">The density of the background.</param>
        /// <param name="material">The material.</param>
        /// <param name="light">The light value.</param>
        public TerrainPoint(byte foreground, byte background, TerrainMaterial material, Colour? light)
        {
            this.Foreground = foreground;
            this.Background = background;
            this.Material = material;
            this.Light = light;
        }

        /// <summary>
        /// Gets or sets the density of the foreground, which is the portion of the terrain that can be dug.
        /// </summary>
        public byte Foreground { get; set; }

        /// <summary>
        /// Gets or sets the density of the background, which is the portion of the terrain that cannot be dug.
        /// </summary>
        public byte Background { get; set; }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        public TerrainMaterial Material { get; set; }

        /// <summary>
        /// Gets or sets the light value.
        /// </summary>
        public Colour? Light { get; set; }
        
        /// <summary>
        /// Gets the density at the given z depth.
        /// </summary>
        /// <param name="z">The z depth.</param>
        /// <returns>The density.</returns>
        public byte GetDensity(int z)
        {
            if (z >= 0)
            {
                if (z < Metrics.DigDepth)
                {
                    return this.Foreground;
                }
                else if (z < Metrics.ChunkDepth)
                {
                    return this.Background;
                }
            }

            // Return the 'air' density
            return TerrainPoint.DensityMax;
        }
    }
}