// ----------------------------------------------------------------------------
// <copyright file="DebugDrawSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Subsystem
{
    using Dwarves.Component.Screen;
    using Dwarves.Component.Spatial;
    using EntitySystem;
    using EntitySystem.Subsystem;
    using FarseerPhysics.DebugViews;
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// System responsible for rendering the debug view.
    /// </summary>
    public class DebugDrawSystem : BaseSystem
    {
        /// <summary>
        /// The debug view.
        /// </summary>
        private DebugViewXNA debugView;

        /// <summary>
        /// Initializes a new instance of the DebugDrawSystem class.
        /// </summary>
        /// <param name="entityManager">The EntityManager for the world that this system belongs to.</param>
        /// <param name="world">The physics world.</param>
        /// <param name="device">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public DebugDrawSystem(EntityManager entityManager, World world, GraphicsDevice device, ContentManager content)
            : base(entityManager)
        {
            this.debugView = new DebugViewXNA(world);
            this.debugView.LoadContent(device, content);

            // Set the debug flags
            ////this.debugView.AppendFlags(DebugViewFlags.AABB);
            ////this.debugView.AppendFlags(DebugViewFlags.CenterOfMass);
            ////this.debugView.AppendFlags(DebugViewFlags.ContactNormals);
            ////this.debugView.AppendFlags(DebugViewFlags.ContactPoints);
            ////this.debugView.AppendFlags(DebugViewFlags.Controllers);
            ////this.debugView.AppendFlags(DebugViewFlags.DebugPanel);
            ////this.debugView.AppendFlags(DebugViewFlags.Joint);
            ////this.debugView.AppendFlags(DebugViewFlags.Pair);
            ////this.debugView.AppendFlags(DebugViewFlags.PerformanceGraph);
            ////this.debugView.AppendFlags(DebugViewFlags.PolygonPoints);
            ////this.debugView.AppendFlags(DebugViewFlags.Shape);
        }

        /// <summary>
        /// Perform the system's processing.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public override void Process(int delta)
        {
            // Process each entity with a camera component
            foreach (Entity entity in this.EntityManager.GetEntitiesWithComponent(typeof(CameraComponent)))
            {
                // Get the entity components
                var camera = (CameraComponent)this.EntityManager.GetComponent(entity, typeof(CameraComponent));
                var position = (PositionComponent)this.EntityManager.GetComponent(entity, typeof(PositionComponent));
                var scale = (ScaleComponent)this.EntityManager.GetComponent(entity, typeof(ScaleComponent));

                // Calculate the projection matrix
                Matrix projection = this.CalculateProjection(camera.ProjectionWidth, camera.ProjectionHeight);

                // Calculate the view matrix
                Matrix view = this.CalculateView(position.Position, scale.Scale);

                // Render the view
                this.debugView.RenderDebugData(ref projection, ref view);
            }
        }

        /// <summary>
        /// Calcualte the projection matrix.
        /// </summary>
        /// <param name="projectionWidth">The center x position.</param>
        /// <param name="projectionHeight">The center y position.</param>
        /// <returns>The projection matrix.</returns>
        private Matrix CalculateProjection(float projectionWidth, float projectionHeight)
        {
            float halfWidth = projectionWidth / 2;
            float halfHeight = projectionHeight / 2;

            return Matrix.CreateOrthographicOffCenter(-halfWidth, halfWidth, -halfHeight, halfHeight, -1, 1);
        }

        /// <summary>
        /// Calcualte the view matrix.
        /// </summary>
        /// <param name="centerPosition">The center position.</param>
        /// <param name="zoom">The zoom ratio.</param>
        /// <returns>The view matrix.</returns>
        private Matrix CalculateView(Vector2 centerPosition, float zoom)
        {
            return
                Matrix.CreateTranslation(new Vector3(-centerPosition.X, -centerPosition.Y, 0)) *
                Matrix.CreateScale(zoom);
        }
    }
}