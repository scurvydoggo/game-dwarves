// ----------------------------------------------------------------------------
// <copyright file="RectI.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// An integer-based rectangle.
/// </summary>
public struct RectI
{
    /// <summary>
    /// Initializes a new instance of the Vector2I struct.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y posititon.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public RectI(int x, int y, int width, int height)
        : this()
    {
        this.X = x;
        this.Y = y;
		this.Width = width;
		this.Height = height;
    }

    /// <summary>
    /// Gets or sets the x position.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the y position.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    public int Height { get; set; }
}