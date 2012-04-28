// ----------------------------------------------------------------------------
// <copyright file="StatisticsComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// Code written by Shawn Hargreaves:
// http://blogs.msdn.com/b/shawnhar/archive/2007/06/08/displaying-the-framerate.aspx
// ----------------------------------------------------------------------------
#if DEBUG
namespace Dwarves.Debug
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A component which renders the performance statistics of the game.
    /// </summary>
    public class StatisticsComponent : DrawableGameComponent
    {
        /// <summary>
        /// The content.
        /// </summary>
        private ContentManager content;

        /// <summary>
        /// The font.
        /// </summary>
        private SpriteFont spriteFont;

        /// <summary>
        /// The number of frames so far in this second.
        /// </summary>
        private int frameCounter = 0;

        /// <summary>
        /// The number of frames in the last second.
        /// </summary>
        private int frameRate = 0;

        /// <summary>
        /// The time that has passed since this second began.
        /// </summary>
        private TimeSpan elapsedTime = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new instance of the StatisticsComponent class.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="position">The on-screen position to render the statistics.</param>
        /// <param name="textColor">The text color.</param>
        public StatisticsComponent(Game game, Vector2 position, Color textColor)
            : base(game)
        {
            this.content = game.Content;
            this.Position = position;
            this.TextColor = textColor;
        }

        /// <summary>
        /// Gets or sets the on-screen position to render the statistics.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Perform a game update step.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this.elapsedTime += gameTime.ElapsedGameTime;

            if (this.elapsedTime > TimeSpan.FromSeconds(1))
            {
                this.elapsedTime -= TimeSpan.FromSeconds(1);
                this.frameRate = this.frameCounter;
                this.frameCounter = 0;
            }
        }

        /// <summary>
        /// Perform a game draw step.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            this.frameCounter++;

            string text = "FPS: " + this.frameRate;

            using (var spriteBatch = new SpriteBatch(this.GraphicsDevice))
            {
                spriteBatch.Begin();

                // Draw the shadow
                spriteBatch.DrawString(this.spriteFont, text, this.Position + new Vector2(1), Color.Black);

                // Draw the text
                spriteBatch.DrawString(this.spriteFont, text, this.Position, this.TextColor);

                spriteBatch.End();
            }
        }

        /// <summary>
        /// Load the content.
        /// </summary>
        protected override void LoadContent()
        {
            this.spriteFont = this.content.Load<SpriteFont>("Font");
        }

        /// <summary>
        /// Unload the content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.content.Unload();
        }
    }
}
#endif