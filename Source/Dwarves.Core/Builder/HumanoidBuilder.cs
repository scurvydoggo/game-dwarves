using EntitySystem;
namespace Dwarves.Builder
{
    /// <summary>
    /// Abstract class for creating humanoid entities.
    /// </summary>
    public abstract class HumanoidBuilder
    {
        /// <summary>
        /// Initializes an instance of the HumanoidBuilder class.
        /// </summary>
        /// <param name="world">The world context.</param>
        public HumanoidBuilder(WorldContext world)
        {
            this.World = world;
        }

        /// <summary>
        /// Gets or sets the top-level entity of the humanoid being built.
        /// </summary>
        protected Entity Entity { get; set; }

        /// <summary>
        /// Gets the world context.
        /// </summary>
        protected WorldContext World { get; private set; }

        /// <summary>
        /// Create a new humanoid.
        /// </summary>
        public void CreateNew()
        {

        }

        /// <summary>
        /// Get the top-level entity of the humanoid.
        /// </summary>
        /// <returns>The entity.</returns>
        public Entity GetEntity()
        {
            return this.Entity;
        }
    }
}