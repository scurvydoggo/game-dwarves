// ----------------------------------------------------------------------------
// <copyright file="HumanoidAssembler.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Assembler.Body
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
    /// Assembles humanoid body components.
    /// </summary>
    public class HumanoidAssembler
    {
        /// <summary>
        /// The world context.
        /// </summary>
        private WorldContext world;

        /// <summary>
        /// Initializes a new instance of the HumanoidAssembler class.
        /// </summary>
        /// <param name="world">The world context.</param>
        public HumanoidAssembler(WorldContext world)
        {
            this.world = world;
        }

        /// <summary>
        /// Assemble the body for the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="args">The assembler args.</param>
        public void AssembleBody(Entity entity, HumanoidAssemblerArgs args)
        {
            // Randomise the sprite variations
            int varTorso = this.world.Resources.GetRandomSpriteVariation("body", "torso", args.Family);
            int headVar = this.world.Resources.GetRandomSpriteVariation("body", "head", args.Family);
            int beardVar = this.world.Resources.GetRandomSpriteVariation("body", "beard", args.Family);

            // Create the body parts
            Entity torso = this.AssembleBodyPart(entity, BodyPart.Torso, args.Family, "torso", varTorso);
            Entity head = this.AssembleBodyPart(entity, BodyPart.Head, args.Family, "head", headVar);
            Entity leftUpperArm = this.AssembleBodyPart(entity, BodyPart.UpperArm, args.Family, "arm_upper", varTorso);
            Entity rightUpperArm = this.AssembleBodyPart(entity, BodyPart.UpperArm, args.Family, "arm_upper", varTorso);
            Entity leftLowerArm = this.AssembleBodyPart(entity, BodyPart.LowerArm, args.Family, "arm_lower", varTorso);
            Entity rightLowerArm = this.AssembleBodyPart(entity, BodyPart.LowerArm, args.Family, "arm_lower", varTorso);
            Entity leftLeg = this.AssembleBodyPart(entity, BodyPart.Leg, args.Family, "leg");
            Entity rightLeg = this.AssembleBodyPart(entity, BodyPart.Leg, args.Family, "leg");
            Entity beard = this.AssembleBodyPart(entity, BodyPart.Beard, args.Family, "beard", beardVar, false);
        }

        /// <summary>
        /// Create the body part entity.
        /// </summary>
        /// <param name="bodyEntity">The body entity that the torso belongs to.</param>
        /// <param name="bodyPart">The type of body part.</param>
        /// <param name="spriteFamily">The sprite family.</param>
        /// <param name="spriteType">The sprite type.</param>
        /// <param name="spriteVariation">The sprite variation.</param>
        /// <param name="hasPhysics">Indicates whether the body part has physics.</param>
        /// <returns>The body part entity.</returns>
        public Entity AssembleBodyPart(
            Entity bodyEntity,
            BodyPart bodyPart,
            string spriteFamily,
            string spriteType,
            int spriteVariation = -1,
            bool hasPhysics = true)
        {
            Entity entity = this.world.EntityManager.CreateEntity();

            // Add the body part
            this.world.EntityManager.AddComponent(entity, new BodyPartComponent(bodyEntity, bodyPart));

            // Add the sprite
            string spriteName =
                this.world.Resources.GetSpriteName("body", spriteType, spriteFamily, spriteVariation);
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
                body.IsStatic = false;

                // Add the physics component
                this.world.EntityManager.AddComponent(entity, new PhysicsComponent(body));
            }

            return entity;
        }
    }
}