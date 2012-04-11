// ----------------------------------------------------------------------------
// <copyright file="BodyAssembler.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Assembler
{
    using System.Collections.Generic;
    using Dwarves.Component.Game;
    using Dwarves.Component.Physics;
    using Dwarves.Component.Render;
    using EntitySystem;
    using FarseerPhysics.Common;
    using FarseerPhysics.Common.Decomposition;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Factories;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Assembles body components.
    /// </summary>
    public abstract class BodyAssembler
    {
        /// <summary>
        /// The world context.
        /// </summary>
        private WorldContext world;

        /// <summary>
        /// Initializes a new instance of the BodyAssembler class.
        /// </summary>
        /// <param name="world">The world context.</param>
        public BodyAssembler(WorldContext world)
        {
            this.world = world;
        }

        /// <summary>
        /// Gets the sprite family for this body.
        /// </summary>
        protected abstract string SpriteFamily { get; }

        /// <summary>
        /// Assemble the body for the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void AssembleBody(Entity entity)
        {
            // Get the clothes variation
            int variation = this.GetRandomClothesVariation(this.SpriteFamily);

            // Create the torso
            this.AssembleBodyPart(entity, BodyPart.Torso, "head", variation, true);
        }

        /// <summary>
        /// Create the body part entity.
        /// </summary>
        /// <param name="bodyEntity">The body entity that the torso belongs to.</param>
        /// <param name="bodyPart">The type of body part.</param>
        /// <param name="spriteType">The sprite type.</param>
        /// <param name="spriteVariation">The sprite variation.</param>
        /// <param name="hasPhysics">Indicates whether the body part has physics.</param>
        /// <returns>The body part entity.</returns>
        public Entity AssembleBodyPart(
            Entity bodyEntity,
            BodyPart bodyPart,
            string spriteType,
            int spriteVariation,
            bool hasPhysics)
        {
            Entity entity = this.world.EntityManager.CreateEntity();

            // Add the body part
            this.world.EntityManager.AddComponent(entity, new BodyPartComponent(bodyEntity, bodyPart));

            // Add the sprite
            string spriteName =
                this.world.Resources.GetSpriteName("body", spriteType, this.SpriteFamily, spriteVariation);
            this.world.EntityManager.AddComponent(entity, new SpriteComponent(spriteName));

            // Create the physics component
            if (hasPhysics)
            {
                // Get the texture data for the sprite
                Rectangle rectangle = this.world.Resources.GetSpriteRectangle(spriteName);
                uint[] spriteData = new uint[rectangle.Width * rectangle.Height];
                this.world.Resources.SpriteSheet.GetData(0, rectangle, spriteData, 0, spriteData.Length);

                // Create a polygon for the sprite texture
                Vertices vertices = PolygonTools.CreatePolygon(spriteData, rectangle.Width, true);

                // Scale the vertices from pixels to physics-world units
                var scale = new Vector2(-WorldContext.PixelsToMeters);
                vertices.Scale(ref scale);

                // Translate the polygon to the centroid
                Vector2 origin = vertices.GetCentroid();
                Vector2 originTranslate = -origin;
                vertices.Translate(ref originTranslate);

                // Partition into smaller polygons to split concave segments
                List<Vertices> convexVertices = BayazitDecomposer.ConvexPartition(vertices);

                // Create a single body with multiple fixtures
                Body body = BodyFactory.CreateCompoundPolygon(this.world.Physics, convexVertices, 1.0f);

                // Add the physics component
                this.world.EntityManager.AddComponent(entity, new PhysicsComponent(body));
            }

            return entity;
        }

        /// <summary>
        /// Get a random variation of the body's clothes.
        /// </summary>
        /// <param name="family">The sprite family.</param>
        /// <returns>The variation.</returns>
        private int GetRandomClothesVariation(string family)
        {
            return this.world.Resources.GetRandomSpriteVariation("body", "torso", family);
        }

        /// <summary>
        /// Body assembler args.
        /// </summary>
        public class Args
        {
            /// <summary>
            /// Gets or sets the body's sprite family.
            /// </summary>
            public string SpriteFamily { get; set; }
        }
    }
}