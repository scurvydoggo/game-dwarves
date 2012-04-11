// ----------------------------------------------------------------------------
// <copyright file="ResourceManager.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Manages the game resources.
    /// </summary>
    public class ResourceManager
    {
        #region Constants

        /// <summary>
        /// Regex for parsing sprite info.
        /// </summary>
        private readonly Regex regexSpriteInfo =
            new Regex(@"(?<category>[^-]+)-(?<name>[^-]+)-?(?<family>[^-\d]*)-?(?<variation>[\d]*)");

        #endregion

        #region Private Variables

        /// <summary>
        /// The game content manager.
        /// </summary>
        private ContentManager content;

        /// <summary>
        /// The sprite source rectangle mapping. Sprites are accessed via a set of identifiers that are specified in the
        /// sprite's name. These are (in order):
        ///   Category: The type of sprite (eg. Font, Menu, Body)
        ///   Family: The family of sprite. Null for none. (eg. Dwarf, Goblin).
        ///   Name: The sprite being seeked (eg. Cloud, Leg, Explosion).
        ///   Variation: The variation number of the sprite. -1 for none. (eg. 1 for Red, 2 for Green, 3 for Blue).
        /// </summary>
        private Dictionary<Tuple<string, string, string>, Dictionary<int, Rectangle>> spriteMap;

        /// <summary>
        /// Provides random selection of resources.
        /// </summary>
        private Random random;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ResourceManager class.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public ResourceManager(ContentManager content)
        {
            this.content = content;
            this.spriteMap = new Dictionary<Tuple<string, string, string>, Dictionary<int, Rectangle>>();
            this.random = new Random();

            this.SpriteSheet = this.content.Load<Texture2D>("Sprite\\Sprites");
            this.SpriteRectangles = this.content.Load<Dictionary<string, Rectangle>>("Sprite\\SpriteMap");

            // Build the list of available sprite variations
            this.BuildSpriteInfoMap();
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
        public Dictionary<string, Rectangle> SpriteRectangles { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load the resource with the given name.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="assetName">The name of the resource.</param>
        /// <returns>The resource.</returns>
        public T Load<T>(string assetName)
        {
            return this.content.Load<T>(assetName);
        }

        /// <summary>
        /// Get the source rectangle for the sprite with the given identifying attributes.
        /// </summary>
        /// <param name="category">The sprite category.</param>
        /// <param name="name">The sprite name.</param>
        /// <param name="family">The sprite family; Null if the sprite has no family.</param>
        /// <param name="variation">The variation index of the sprite; -1 if the sprite has no variation.</param>
        /// <returns>The source rectangle; Null if the sprite does not exist.</returns>
        public Rectangle? GetSpriteRectangle(string category, string name, string family = null, int variation = -1)
        {
            Rectangle? rectangle = null;

            var key = Tuple.Create(category, name, family);
            if (this.spriteMap.ContainsKey(key))
            {
                Dictionary<int, Rectangle> variations = this.spriteMap[key];
                if (variations.Count > 0)
                {
                    if (variation == -1)
                    {
                        // Get the first element since no variation was specified
                        rectangle = variations.ElementAt(0).Value;
                    }
                    else
                    {
                        // Get the specified variation
                        if (variations.ContainsKey(variation))
                        {
                            rectangle = this.spriteMap[key][variation];
                        }
                    }
                }
            }

            return rectangle;
        }

        /// <summary>
        /// Get the source rectangle for a random variation of the sprite with the given identifying attributes.
        /// </summary>
        /// <param name="category">The sprite category.</param>
        /// <param name="name">The sprite name.</param>
        /// <param name="variation">The variation index of the sprite; -1 if the sprite does not exist.</param>
        /// <returns>The source rectangle; Null if the sprite does not exist.</returns>
        public Rectangle? GetRandomSpriteRectangle(string category, string name, out int variation)
        {
            return this.GetRandomSpriteRectangle(category, name, null, out variation);
        }

        /// <summary>
        /// Get the source rectangle for a random variation of the sprite with the given identifying attributes.
        /// </summary>
        /// <param name="category">The sprite category.</param>
        /// <param name="name">The sprite name.</param>
        /// <param name="family">The sprite family; Null if the sprite has no family.</param>
        /// <param name="variation">The variation index of the sprite; -1 if the sprite does not exist.</param>
        /// <returns>The source rectangle; Null if the sprite does not exist.</returns>
        public Rectangle? GetRandomSpriteRectangle(string category, string name, string family, out int variation)
        {
            List<int> variations = this.GetSpriteVariations(category, name, family);
            if (variations.Count > 0)
            {
                variation = this.random.Next(0, variations.Count);
                return this.GetSpriteRectangle(category, name, family, variation);
            }
            else
            {
                variation = -1;
                return null;
            }
        }

        /// <summary>
        /// Get the variation numbers for the sprite with the given identifying attributes.
        /// </summary>
        /// <param name="category">The sprite category.</param>
        /// <param name="name">The sprite name.</param>
        /// <param name="family">The sprite family; Null if the sprite has no family.</param>
        /// <returns>The list of sprite variation numbers.</returns>
        public List<int> GetSpriteVariations(string category, string name, string family = null)
        {
            var key = Tuple.Create(category, name, family);
            if (this.spriteMap.ContainsKey(key))
            {
                return this.spriteMap[key].Keys.ToList();
            }
            else
            {
                return new List<int>();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Build the list of available indices (variations of a base sprite, such as different length beards).
        /// </summary>
        private void BuildSpriteInfoMap()
        {
            // Match on all sprites which have a numerical suffix and add these to the collection
            foreach (KeyValuePair<string, Rectangle> kvp in this.SpriteRectangles)
            {
                string spriteName = kvp.Key;
                Rectangle rectangle = kvp.Value;

                MatchCollection matches = this.regexSpriteInfo.Matches(spriteName);
                if (matches.Count == 1)
                {
                    string category = matches[0].Groups["category"].Value;
                    string name = matches[0].Groups["name"].Value;
                    string family = matches[0].Groups["family"].Value;
                    string variationStr = !string.IsNullOrEmpty(matches[0].Groups["family"].Value) ?
                        matches[0].Groups["family"].Value : null;
                    int variation = !string.IsNullOrEmpty(matches[0].Groups["variation"].Value) ?
                        int.Parse(matches[0].Groups["variation"].Value) : -1;

                    Tuple<string, string, string> key = Tuple.Create(category, name, family);
                    if (this.spriteMap.ContainsKey(key))
                    {
                        this.spriteMap[key].Add(variation, rectangle);
                    }
                    else
                    {
                        this.spriteMap.Add(key, new Dictionary<int, Rectangle> { { variation, rectangle } });
                    }
                }
            }
        }

        #endregion
    }
}
