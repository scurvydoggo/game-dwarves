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
        /// <param name="neckJointJoint">The neck joint info.</param>
        /// <param name="shoulderJoint">The shoulder joint info.</param>
        /// <param name="hipJoint">The hip joint info.</param>
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
            RevoluteJoint neckJointJoint,
            RevoluteJoint shoulderJoint,
            RevoluteJoint hipJoint)
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
            this.NeckJointPosition = neckJointJoint;
            this.ShoulderJointPosition = shoulderJoint;
            this.HipJointPosition = hipJoint;
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

        #region Body Part

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

        #region Joint

        /// <summary>
        /// Gets or sets the body-relative joint position.
        /// </summary>
        public RevoluteJoint NeckJointPosition { get; set; }

        /// <summary>
        /// Gets or sets the body-relative joint position.
        /// </summary>
        public RevoluteJoint ShoulderJointPosition { get; set; }

        /// <summary>
        /// Gets or sets the body-relative joint position.
        /// </summary>
        public RevoluteJoint HipJointPosition { get; set; }

        #endregion

        #region Inner Classes

        /// <summary>
        /// Revolute joint settings.
        /// </summary>
        public class RevoluteJoint
        {
            /// <summary>
            /// Initializes a new instance of the RevoluteJoint class.
            /// </summary>
            /// <param name="position">The body-relative joint position.</param>
            /// <param name="enableLimit">Indicates whether the limit is enabled.</param>
            /// <param name="lowerLimit">The lower joint limit in radians.</param>
            /// <param name="upperLimit">The upper joint limit in radians.</param>
            /// <param name="enableMotor">Indicates whether the motor is enabled.</param>
            /// <param name="maxMotorTorque">The maximum motor torque.</param>
            public RevoluteJoint(
                Vector2 position,
                bool enableLimit,
                float lowerLimit,
                float upperLimit,
                bool enableMotor,
                float maxMotorTorque)
            {
                this.Position = position;
                this.EnableLimit = enableLimit;
                this.LowerLimit = lowerLimit;
                this.UpperLimit = upperLimit;
                this.EnableMotor = enableMotor;
                this.MaxMotorTorque = maxMotorTorque;
            }

            /// <summary>
            /// Gets or sets the body-relative joint position.
            /// </summary>
            public Vector2 Position { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the limit is enabled.
            /// </summary>
            public bool EnableLimit { get; set; }

            /// <summary>
            /// Gets or sets the lower joint limit in radians.
            /// </summary>
            public float LowerLimit { get; set; }

            /// <summary>
            /// Gets or sets the upper joint limit in radians.
            /// </summary>
            public float UpperLimit { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the motor is enabled.
            /// </summary>
            public bool EnableMotor { get; set; }

            /// <summary>
            /// Gets or sets the maximum motor torque.
            /// </summary>
            public float MaxMotorTorque { get; set; }
        }

        #endregion
    }
}