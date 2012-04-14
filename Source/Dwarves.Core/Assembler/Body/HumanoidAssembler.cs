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
    using Dwarves.Component.Spatial;
    using EntitySystem;
    using FarseerPhysics.Common;
    using FarseerPhysics.Common.Decomposition;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Dynamics.Joints;
    using FarseerPhysics.Factories;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Assembles humanoid body components.
    /// </summary>
    public class HumanoidAssembler
    {
        #region Private Variables

        /// <summary>
        /// The world context.
        /// </summary>
        private WorldContext world;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the HumanoidAssembler class.
        /// </summary>
        /// <param name="world">The world context.</param>
        public HumanoidAssembler(WorldContext world)
        {
            this.world = world;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assemble the body for the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="args">The assembler args.</param>
        public void AssembleBody(Entity entity, HumanoidAssemblerArgs args)
        {
            // Randomise the sprite variations
            int clothesVariation = this.world.Resources.GetRandomSpriteVariation("body", "torso", args.SpriteFamily);
            int headVariation = this.world.Resources.GetRandomSpriteVariation("body", "head", args.SpriteFamily);
            int beardVariation = this.world.Resources.GetRandomSpriteVariation("body", "beard", args.SpriteFamily);

            // Create the body parts
            BodyPartInfo torso = this.AssembleBodyPart(
                entity,
                BodyPart.Torso,
                "torso",
                args.SpriteFamily,
                args.BodyPosition + args.TorsoPosition,
                args.CollisionGroup,
                clothesVariation);
            BodyPartInfo head = this.AssembleBodyPart(
                entity,
                BodyPart.Head,
                "head",
                args.SpriteFamily,
                args.BodyPosition + args.HeadPosition,
                args.CollisionGroup,
                clothesVariation);
            BodyPartInfo leftArm = this.AssembleBodyPart(
                entity,
                BodyPart.Arm,
                "arm",
                args.SpriteFamily,
                args.BodyPosition + args.ArmPosition,
                args.CollisionGroup,
                clothesVariation);
            BodyPartInfo rightArm = this.AssembleBodyPart(
                entity,
                BodyPart.Arm,
                "arm",
                args.SpriteFamily,
                args.BodyPosition + args.ArmPosition + args.LeftToRightOffset,
                args.CollisionGroup,
                clothesVariation);
            BodyPartInfo leftLeg = this.AssembleBodyPart(
                entity,
                BodyPart.Leg,
                "leg",
                args.SpriteFamily,
                args.BodyPosition + args.LegPosition,
                args.CollisionGroup);
            BodyPartInfo rightLeg = this.AssembleBodyPart(
                entity,
                BodyPart.Leg,
                "leg",
                args.SpriteFamily,
                args.BodyPosition + args.LegPosition + args.LeftToRightOffset,
                args.CollisionGroup);
            BodyPartInfo beard = this.AssembleBodyPart(
                entity,
                BodyPart.Beard,
                "beard",
                args.SpriteFamily,
                args.BodyPosition + args.BeardPosition,
                args.CollisionGroup,
                beardVariation,
                false);

            // Create the joints
            this.CreateFixedJoint(head, beard);
            this.CreateRotationalJoint(torso, head, args.NeckJointPosition - args.HeadPosition);
            this.CreateRotationalJoint(torso, leftArm, args.ShoulderJointPosition - args.ArmPosition);
            this.CreateRotationalJoint(torso, rightArm, args.ShoulderJointPosition - args.ArmPosition);
            this.CreateRotationalJoint(torso, leftLeg, args.HipJointPosition - args.LegPosition);
            this.CreateRotationalJoint(torso, rightLeg, args.HipJointPosition - args.LegPosition);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create the body part entity.
        /// </summary>
        /// <param name="bodyEntity">The body entity that the torso belongs to.</param>
        /// <param name="bodyPart">The type of body part.</param>
        /// <param name="spriteType">The sprite type.</param>
        /// <param name="spriteFamily">The sprite family.</param>
        /// <param name="position">The world position of the body part.</param>
        /// <param name="collisionGroup">The collision group of the body part.</param>
        /// <param name="spriteVariation">The sprite variation.</param>
        /// <param name="isPhysical">Indicates whether the body part can collide with physics objects.</param>
        /// <returns>The body part entity and the component.</returns>
        private BodyPartInfo AssembleBodyPart(
            Entity bodyEntity,
            BodyPart bodyPart,
            string spriteType,
            string spriteFamily,
            Vector2 position,
            short collisionGroup,
            int spriteVariation = -1,
            bool isPhysical = true)
        {
            Entity entity = this.world.EntityManager.CreateEntity();

            // Add the body part
            var bodyPartComponent = new BodyPartComponent(bodyEntity, bodyPart);
            this.world.EntityManager.AddComponent(entity, bodyPartComponent);

            // Add the sprite
            string spriteName =
                this.world.Resources.GetSpriteName("body", spriteType, spriteFamily, spriteVariation);
            this.world.EntityManager.AddComponent(entity, new SpriteComponent(spriteName));

            Body body;
            if (isPhysical)
            {
                // Get the texture data for the sprite
                Rectangle rectangle = this.world.Resources.GetSpriteRectangle(spriteName);
                uint[] spriteData = new uint[rectangle.Width * rectangle.Height];
                this.world.Resources.SpriteSheet.GetData(0, rectangle, spriteData, 0, spriteData.Length);

                // Create a polygon for the sprite texture
                Vertices vertices = PolygonTools.CreatePolygon(spriteData, rectangle.Width, true);

                // Scale the vertices from pixels to physics-world units
                var scale = new Vector2(DwarfConst.PixelsToMeters, -DwarfConst.PixelsToMeters);
                vertices.Scale(ref scale);

                // Translate the polygon to the centroid
                Vector2 origin = vertices.GetCentroid();
                Vector2 originTranslate = -origin;
                vertices.Translate(ref originTranslate);

                // Partition into smaller polygons to split concave segments
                List<Vertices> convexVertices = BayazitDecomposer.ConvexPartition(vertices);

                // Create the body
                body = BodyFactory.CreateCompoundPolygon(this.world.Physics, convexVertices, 1.0f);
            }
            else
            {
                // Create a tiny rectangle since this isn't a physical body. A body is just created so that the body
                // part can be welded onto other parts
                body = BodyFactory.CreateRectangle(this.world.Physics, 0.001f, 0.001f, 1.0f);
            }

            body.IsStatic = false;
            body.CollisionGroup = collisionGroup;
            body.IsSensor = !isPhysical;

            // Add the physics component
            var physicsComponent = new PhysicsComponent(body);
            this.world.EntityManager.AddComponent(entity, physicsComponent);

            // Create the position component
            this.world.EntityManager.AddComponent(entity, new PositionComponent(position, body));

            return new BodyPartInfo(entity, bodyPartComponent, physicsComponent);
        }

        /// <summary>
        /// Create a rotational joint between the two body parts.
        /// </summary>
        /// <param name="bodyPartA">The first body part being joined.</param>
        /// <param name="bodyPartB">The second body part being joined.</param>
        /// <param name="positionB">The position of the joint relative to the second body part.</param>
        private void CreateRotationalJoint(BodyPartInfo bodyPartA, BodyPartInfo bodyPartB, Vector2 positionB)
        {
            // Create the joint
            Joint joint = JointFactory.CreateRevoluteJoint(
                this.world.Physics,
                bodyPartA.PhysicsComponent.Body,
                bodyPartB.PhysicsComponent.Body,
                positionB);

            // Register the joint with the two body parts
            bodyPartA.BodyPartComponent.Joints.Add(joint);
            bodyPartB.BodyPartComponent.Joints.Add(joint);
        }

        /// <summary>
        /// Create a fixed joint between the two body parts.
        /// </summary>
        /// <param name="bodyPartA">The first body part being joined.</param>
        /// <param name="bodyPartB">The second body part being joined.</param>
        private void CreateFixedJoint(BodyPartInfo bodyPartA, BodyPartInfo bodyPartB)
        {
            // Create the joint
            Joint joint = JointFactory.CreateWeldJoint(
                this.world.Physics,
                bodyPartA.PhysicsComponent.Body,
                bodyPartB.PhysicsComponent.Body,
                Vector2.Zero,
                Vector2.Zero);

            // Register the joint with the two body parts
            bodyPartA.BodyPartComponent.Joints.Add(joint);
            bodyPartB.BodyPartComponent.Joints.Add(joint);
        }

        #endregion

        #region Inner Classes

        /// <summary>
        /// Body part info.
        /// </summary>
        private class BodyPartInfo
        {
            /// <summary>
            /// Initializes a new instance of the BodyPartInfo class.
            /// </summary>
            /// <param name="entity">The body part entity.</param>
            /// <param name="bodyPartComponent">The body part component.</param>
            /// <param name="physicsComponent">The physics component.</param>
            public BodyPartInfo(Entity entity, BodyPartComponent bodyPartComponent, PhysicsComponent physicsComponent)
            {
                this.Entity = entity;
                this.BodyPartComponent = bodyPartComponent;
                this.PhysicsComponent = physicsComponent;
            }

            /// <summary>
            /// Gets the body part entity.
            /// </summary>
            public Entity Entity { get; private set; }

            /// <summary>
            /// Gets the body part component.
            /// </summary>
            public BodyPartComponent BodyPartComponent { get; private set; }

            /// <summary>
            /// Gets the physics component.
            /// </summary>
            public PhysicsComponent PhysicsComponent { get; private set; }
        }

        #endregion
    }
}