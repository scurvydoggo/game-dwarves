// ----------------------------------------------------------------------------
// <copyright file="InputPositionComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Input
{
    using EntitySystem.Component;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Indicates the position of the input.
    /// </summary>
    public class InputPositionComponent : IComponent
    {
        /// <summary>
        /// The positions of the input.
        /// </summary>
        private Point[] positions;

        /// <summary>
        /// Gets or sets the positions of the input (this can be more than 1 position for multi-touch).
        /// </summary>
        public Point[] Positions
        {
            get
            {
                return this.positions;
            }

            set
            {
                this.PrevPositions = this.positions;
                this.positions = value;
            }
        }

        /// <summary>
        /// Gets the previous positions of the input (this can be more than 1 position for multi-touch).
        /// </summary>
        public Point[] PrevPositions { get; private set; }
    }
}