// ----------------------------------------------------------------------------
// <copyright file="TerrainTouchComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using Dwarves.Component.Input;
    using Dwarves.Core;
    using UnityEngine;

    /// <summary>
    /// A component that responds to being touched.
    /// </summary>
    [RequireComponent(typeof(TerrainMutatorComponent))]
    public class TerrainTouchComponent : TouchableComponent
    {
        /// <summary>
        /// The terrain mutator component.
        /// </summary>
        private TerrainMutatorComponent cTerrainMutator;

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.cTerrainMutator = this.GetComponent<TerrainMutatorComponent>();
        }

        /// <summary>
        /// Handles the on-touch behaviour for the component.
        /// </summary>
        /// <param name="hitPoint">The point at which the component was touched in world coordinates.</param>
        public override void ProcessTouch(Vector3 hitPoint)
        {
            var position = new Position((int)hitPoint.x, (int)hitPoint.y);
            var offset = new Vector2(hitPoint.x - position.X, hitPoint.y - position.Y);

            // Dig at the touched point
            this.cTerrainMutator.Dig(position, offset);
        }
    }
}