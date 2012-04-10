// ----------------------------------------------------------------------------
// <copyright file="SpriteManager.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Render
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Manages the spritesheet and provides source rectangles for named sprites.
    /// </summary>
    public class SpriteManager
    {
        #region Constants

        /// <summary>
        /// Regex for patching the sprite index.
        /// </summary>
        private readonly Regex regexSpriteIndex = new Regex(@"([^\d]+)(\d*)");

        #endregion

        #region Private Variables

        /// <summary>
        /// The variations that are available for a base sprite. A base sprite is the sprite name without the numerical
        /// suffix (eg. for 'head1' the base sprite is 'head'). The dictionary value contains the the suffix numbers
        /// for the base sprite with the corresponding sprite name.
        /// </summary>
        private Dictionary<string, Dictionary<int, string>> variationMap;

        /// <summary>
        /// For random selection of sprite variations.
        /// </summary>
        private Random random;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SpriteManager class.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public SpriteManager(ContentManager content)
        {
            this.SpriteSheet = content.Load<Texture2D>("Sprite\\Sprites");
            this.SpriteMap = content.Load<Dictionary<string, Rectangle>>("Sprite\\SpriteMap");
            this.variationMap = new Dictionary<string, Dictionary<int, string>>();
            this.random = new Random();

            // Build the list of available sprite variations
            this.BuildVariationMap();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the spritesheet.
        /// </summary>
        public Texture2D SpriteSheet { get; private set; }

        /// <summary>
        /// Gets the mapping of sprite names to their source rectangles in the spritesheet.
        /// </summary>
        public Dictionary<string, Rectangle> SpriteMap { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the source rectangle for the sprite with the given name.
        /// </summary>
        /// <param name="spriteName">The sprite name.</param>
        /// <returns>The source rectangle; If the sprite does not exist then a rectangle with all values set to zero is
        /// returned.</returns>
        public Rectangle GetSpriteRectangle(string spriteName)
        {
            if (this.SpriteMap.ContainsKey(spriteName))
            {
                return this.SpriteMap[spriteName];
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// Get the source rectangle for the sprite with the given base name and variation index.
        /// </summary>
        /// <param name="baseSpriteName">The base name of the sprite.</param>
        /// <param name="variation">The variation index of the sprite.</param>
        /// <returns>The source rectangle; If the sprite does not exist then a rectangle with all values set to zero is
        /// returned.</returns>
        public Rectangle GetSpriteRectangle(string baseSpriteName, int variation)
        {
            if (this.variationMap.ContainsKey(baseSpriteName) &&
                this.variationMap[baseSpriteName].ContainsKey(variation))
            {
                return this.SpriteMap[this.variationMap[baseSpriteName][variation]];
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// Get the source rectangle for a random variation of the given base name.
        /// </summary>
        /// <param name="baseSpriteName">The base name of the sprite.</param>
        /// <param name="variation">The variation index of the sprite; -1 if the sprite does not exist.</param>
        /// <returns>The source rectangle; If the sprite does not exist then a rectangle with all values set to zero is
        /// returned.</returns>
        public Rectangle GetRandomSpriteRectangle(string baseSpriteName, out int variation)
        {
            if (this.variationMap.ContainsKey(baseSpriteName))
            {
                Dictionary<int, string> variations = this.variationMap[baseSpriteName];

                // Get a random entry
                KeyValuePair<int, string> kvp = variations.ElementAt(this.random.Next(0, variations.Count));
                variation = kvp.Key;
                return this.SpriteMap[kvp.Value];
            }
            else
            {
                variation = -1;
                return Rectangle.Empty;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Build the list of available indices (variations of a base sprite, such as different length beards).
        /// </summary>
        private void BuildVariationMap()
        {
            // Match on all sprites which have a numerical suffix and add these to the collection
            foreach (string spriteName in this.SpriteMap.Keys)
            {
                MatchCollection matches = this.regexSpriteIndex.Matches(spriteName);
                if (matches.Count == 1 && matches[0].Groups.Count == 2)
                {
                    string baseName = matches[0].Groups[0].Value;
                    int index = int.Parse(matches[0].Groups[1].Value);

                    if (this.variationMap.ContainsKey(baseName) && !this.variationMap[baseName].ContainsKey(index))
                    {
                        this.variationMap[baseName].Add(index, spriteName);
                    }
                    else
                    {
                        this.variationMap.Add(baseName, new Dictionary<int, string> { { index, spriteName } });
                    }
                }
            }
        }

        #endregion
    }
}