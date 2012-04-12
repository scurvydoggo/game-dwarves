// ----------------------------------------------------------------------------
// <copyright file="HumanoidAssemblerArgs.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Assembler.Body
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Arguments for the humanoid assembler.
    /// </summary>
    public class HumanoidAssemblerArgs
    {
        /// <summary>
        /// Initializes a new instance of the HumanoidAssemblerArgs class.
        /// </summary>
        /// <param name="spriteFamily">The sprite family.</param>
        /// <param name="collisionGroup">The collision group for the body.</param>
        /// <param name="bodyPosition">The position of the body.</param>
        /// <param name="rightToLeftOffset">The offset from the left leg/arm to the right/leg arm.</param>
        /// <param name="torsoPosition">The body-relative position of the torso.</param>
        /// <param name="headPosition">The body-relative position of the head.</param>
        /// <param name="armPosition">The body-relative position of the arm.</param>
        /// <param name="legPosition">The body-relative position of the leg.</param>
        /// <param name="beardPosition">The body-relative position of the beard.</param>
        /// <param name="neckJointJointPosition">The body-relative joint position of the neck.</param>
        /// <param name="shoulderJointPosition">The body-relative joint position of the shoulder.</param>
        /// <param name="hipJointPosition">The body-relative joint position of the hip.</param>
        public HumanoidAssemblerArgs(
            string spriteFamily,
            short collisionGroup,
            Vector2 bodyPosition,
            Vector2 rightToLeftOffset,
            Vector2 torsoPosition,
            Vector2 headPosition,
            Vector2 armPosition,
            Vector2 legPosition,
            Vector2 beardPosition,
            Vector2 neckJointJointPosition,
            Vector2 shoulderJointPosition,
            Vector2 hipJointPosition)
        {
            this.SpriteFamily = spriteFamily;
            this.CollisionGroup = collisionGroup;
            this.BodyPosition = bodyPosition;
            this.LeftToRightOffset = rightToLeftOffset;
            this.TorsoPosition = torsoPosition;
            this.HeadPosition = headPosition;
            this.ArmPosition = armPosition;
            this.LegPosition = legPosition;
            this.BeardPosition = beardPosition;
            this.NeckJointPosition = neckJointJointPosition;
            this.ShoulderJointPosition = shoulderJointPosition;
            this.HipJointPosition = hipJointPosition;
        }

        #region General

        /// <summary>
        /// Gets or sets the sprites family that this body belongs to.
        /// </summary>
        public string SpriteFamily { get; set; }

        /// <summary>
        /// Gets or sets the collision group for the body.
        /// </summary>
        public short CollisionGroup { get; set; }

        /// <summary>
        /// Gets or sets the position of the body.
        /// </summary>
        public Vector2 BodyPosition { get; set; }

        /// <summary>
        /// Gets or sets the offset from the left leg/arm to the right/leg arm.
        /// </summary>
        public Vector2 LeftToRightOffset { get; set; }

        #endregion

        #region Body Part Positon

        /// <summary>
        /// Gets or sets the body-relative position.
        /// </summary>
        public Vector2 TorsoPosition { get; set; }

        /// <summary>
        /// Gets or sets the body-relative position.
        /// </summary>
        public Vector2 HeadPosition { get; set; }

        /// <summary>
        /// Gets or sets the body-relative position.
        /// </summary>
        public Vector2 ArmPosition { get; set; }

        /// <summary>
        /// Gets or sets the body-relative position.
        /// </summary>
        public Vector2 LegPosition { get; set; }

        /// <summary>
        /// Gets or sets the body-relative position.
        /// </summary>
        public Vector2 BeardPosition { get; set; }

        #endregion

        #region Joint Position

        /// <summary>
        /// Gets or sets the body-relative joint position.
        /// </summary>
        public Vector2 NeckJointPosition { get; set; }

        /// <summary>
        /// Gets or sets the body-relative joint position.
        /// </summary>
        public Vector2 ShoulderJointPosition { get; set; }

        /// <summary>
        /// Gets or sets the body-relative joint position.
        /// </summary>
        public Vector2 HipJointPosition { get; set; }

        #endregion
    }
}