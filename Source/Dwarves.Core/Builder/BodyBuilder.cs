// ----------------------------------------------------------------------------
// <copyright file="BodyBuilder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Builder
{
    using System;
    using Dwarves.Component.Game;
    using EntitySystem;

    /// <summary>
    /// Abstract class for building game bodies (eg. dwarf, goblin).
    /// </summary>
    public abstract class BodyBuilder : EntityBuilder
    {
        /// <summary>
        /// Initializes a new instance of the BodyBuilder class.
        /// </summary>
        /// <param name="world">The world context.</param>
        public BodyBuilder(WorldContext world)
            : base(world)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the builder supports building heads.
        /// </summary>
        public abstract bool SupportsBuildHead { get; }

        /// <summary>
        /// Gets a value indicating whether the builder supports building arms.
        /// </summary>
        public abstract bool SupportsBuildArm { get; }

        /// <summary>
        /// Gets a value indicating whether the builder supports building legs.
        /// </summary>
        public abstract bool SupportsBuildLeg { get; }

        /// <summary>
        /// Build a torso on the body.
        /// </summary>
        /// <param name="args">The build arguments.</param>
        public void BuildTorso(Args args = null)
        {
            this.BuildTorso(this.World.EntityManager.CreateEntity(), args);
        }

        /// <summary>
        /// Build a head on the body.
        /// </summary>
        /// <param name="args">The build arguments.</param>
        public void BuildHead(Args args = null)
        {
            if (this.SupportsBuildHead)
            {
                this.BuildHead(this.World.EntityManager.CreateEntity(), args);
            }
            else
            {
                throw new NotSupportedException("Torso cannot be built.");
            }
        }

        /// <summary>
        /// Build an arm on the body.
        /// </summary>
        /// <param name="args">The build arguments.</param>
        public void BuildArm(Args args)
        {
            if (this.SupportsBuildArm)
            {
                this.BuildArm(this.World.EntityManager.CreateEntity(), args);
            }
            else
            {
                throw new NotSupportedException("Arm cannot be built.");
            }
        }

        /// <summary>
        /// Build a leg on the body.
        /// </summary>
        /// <param name="args">The build arguments.</param>
        public void BuildLeg(Args args)
        {
            if (this.SupportsBuildLeg)
            {
                this.BuildLeg(this.World.EntityManager.CreateEntity(), args);
            }
            else
            {
                throw new NotSupportedException("Leg cannot be built.");
            }
        }

        /// <summary>
        /// Build a torso on the body.
        /// </summary>
        /// <param name="torsoEntity">The torso entity.</param>
        /// <param name="args">The build arguments.</param>
        protected void BuildTorso(Entity torsoEntity, Args args)
        {
            // Add the body part component
            this.World.EntityManager.AddComponent(
                torsoEntity,
                new BodyPartComponent(this.CurrentEntity, BodyPart.Torso));
        }

        /// <summary>
        /// Build a head on the body.
        /// </summary>
        /// <param name="headEntity">The head entity.</param>
        /// <param name="args">The build arguments.</param>
        protected virtual void BuildHead(Entity headEntity, Args args)
        {
            // Add the body part component
            this.World.EntityManager.AddComponent(
                headEntity,
                new BodyPartComponent(this.CurrentEntity, BodyPart.Head));
        }

        /// <summary>
        /// Build an arm on the body.
        /// </summary>
        /// <param name="armEntity">The arm entity.</param>
        /// <param name="args">The build arguments.</param>
        protected virtual void BuildArm(Entity armEntity, Args args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            // Add the body part component
            this.World.EntityManager.AddComponent(
                armEntity,
                new BodyPartComponent(this.CurrentEntity, args.LeftSide ? BodyPart.LeftArm : BodyPart.RightArm));
        }

        /// <summary>
        /// Build a leg on the body.
        /// </summary>
        /// <param name="legEntity">The leg entity.</param>
        /// <param name="args">The build arguments.</param>
        protected virtual void BuildLeg(Entity legEntity, Args args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            // Add the body part component
            this.World.EntityManager.AddComponent(
                legEntity,
                new BodyPartComponent(this.CurrentEntity, args.LeftSide ? BodyPart.LeftLeg : BodyPart.RightLeg));
        }

        /// <summary>
        /// The body builder arguments.
        /// </summary>
        public class Args
        {
            /// <summary>
            /// Initializes a new instance of the Args class.
            /// </summary>
            /// <param name="leftSide">Indicates whether the body part is on the left side (if applicable).</param>
            public Args(bool leftSide)
            {
                this.LeftSide = leftSide;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the body part is on the left side (if applicable).
            /// </summary>
            public bool LeftSide { get; set; }
        }
    }
}