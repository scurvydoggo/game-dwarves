// ----------------------------------------------------------------------------
// <copyright file="SpriteSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Subsystem
{
    using Dwarves.Component.Render;
    using Dwarves.Component.Spatial;
    using EntitySystem;
    using EntitySystem.Subsystem;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// System responsible for rending sprites.
    /// </summary>
    public class SpriteSystem : BaseSystem
    {
        /// <summary>
        /// The game resources.
        /// </summary>
        private ResourceManager resources;

        /// <summary>
        /// The graphics device.
        /// </summary>
        private GraphicsDevice graphics;

        /// <summary>
        /// Initializes a new instance of the SpriteSystem class.
        /// </summary>
        /// <param name="entityManager">The EntityManager for the world that this system belongs to.</param>
        /// <param name="resources">The game resources.</param>
        /// <param name="graphics">The graphics device.</param>
        public SpriteSystem(EntityManager entityManager, ResourceManager resources, GraphicsDevice graphics)
            : base(entityManager)
        {
            this.resources = resources;
            this.graphics = graphics;
        }

        /// <summary>
        /// Perform the system's processing.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public override void Process(int delta)
        {
            using (var spriteBatch = new SpriteBatch(this.graphics))
            {
                spriteBatch.Begin();

                foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(SpriteComponent)))
                {
                    var sprite = (SpriteComponent)this.EntityManager.GetComponent(entity, typeof(SpriteComponent));
                    var position =
                        (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));

                    if (sprite != null && position != null)
                    {
                        spriteBatch.Draw(
                            this.resources.SpriteSheet,
                            Vector2.Zero, // TODO
                            this.resources.GetSpriteRectangle(sprite.SpriteName),
                            Color.White);
                    }
                }

                spriteBatch.End();
            }
        }
    }
}
