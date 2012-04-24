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
        /// The pixel size of tile sprites.
        /// </summary>
        public const int TileSize = 16;

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
        /// The mapping of sprite names to their source rectangles in the spritesheet.
        /// </summary>
        private Dictionary<string, Rectangle> spriteRectangles;

        /// <summary>
        /// The sprite source rectangle mapping. Sprites are accessed via a set of identifiers that are specified in the
        /// sprite's name. These are (in order):
        ///   Category: The type of sprite (eg. Font, Menu, Body)
        ///   Type: The type of sprite being seeked (eg. Cloud, Leg, Explosion).
        ///   Family: The family of sprite. Null for none. (eg. Dwarf, Goblin).
        ///   Variation: The variation of the sprite. -1 for none. (eg. 1 for Red, 2 for Green, 3 for Blue).
        ///   Sprite name: The full name of the sprite (as referenced in SpriteRectangles).
        /// </summary>
        private Dictionary<Tuple<string, string, string>, Dictionary<int, string>> spriteMap;

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
            this.spriteMap = new Dictionary<Tuple<string, string, string>, Dictionary<int, string>>();
            this.random = new Random();
            this.spriteRectangles = this.content.Load<Dictionary<string, Rectangle>>("Sprite\\SpriteRectangles");
            this.SpriteSheet = this.content.Load<Texture2D>("Sprite\\Sprites");

            // Build the list of available sprite variations
            this.BuildSpriteInfoMap();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the spritesheet.
        /// </summary>
        public Texture2D SpriteSheet { get; private set; }

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
        /// Get the name of the sprite with the given identifying attributes.
        /// </summary>
        /// <param name="category">The sprite category.</param>
        /// <param name="type">The sprite type.</param>
        /// <param name="family">The sprite family; Null if the sprite has no family.</param>
        /// <param name="variation">The variation index of the sprite; -1 if the sprite has no variation.</param>
        /// <returns>The full sprite name.</returns>
        public string GetSpriteName(string category, string type, string family = null, int variation = -1)
        {
            Dictionary<int, string> variations = this.spriteMap[Tuple.Create(category, type, family)];
            if (variation == -1)
            {
                // Get the first element since no variation was specified
                return variations.ElementAt(0).Value;
            }
            else
            {
                // Get the specified variation
                return variations[variation];
            }
        }

        /// <summary>
        /// Get the source rectangle for the sprite with the given name.
        /// </summary>
        /// <param name="spriteName">The sprite name.</param>
        /// <returns>The source rectangle.</returns>
        public Rectangle GetSpriteRectangle(string spriteName)
        {
            return this.spriteRectangles[spriteName];
        }

        /// <summary>
        /// Get the source rectangle for a random variation of the sprite with the given identifying attributes.
        /// </summary>
        /// <param name="category">The sprite category.</param>
        /// <param name="type">The sprite type.</param>
        /// <param name="variation">The variation index of the sprite; -1 if the sprite does not exist.</param>
        /// <returns>The source rectangle.</returns>
        public Rectangle GetRandomSpriteRectangle(string category, string type, out int variation)
        {
            return this.GetRandomSpriteRectangle(category, type, null, out variation);
        }

        /// <summary>
        /// Get the source rectangle for a random variation of the sprite with the given identifying attributes.
        /// </summary>
        /// <param name="category">The sprite category.</param>
        /// <param name="type">The sprite type.</param>
        /// <param name="family">The sprite family; Null if the sprite has no family.</param>
        /// <param name="variation">The variation index of the sprite; -1 if the sprite does not exist.</param>
        /// <returns>The source rectangle.</returns>
        public Rectangle GetRandomSpriteRectangle(string category, string type, string family, out int variation)
        {
            variation = this.GetRandomSpriteVariation(category, type, family);
            string spriteName = this.GetSpriteName(category, type, family, variation);
            return this.GetSpriteRectangle(spriteName);
        }

        /// <summary>
        /// Get the variations for the sprite with the given identifying attributes.
        /// </summary>
        /// <param name="category">The sprite category.</param>
        /// <param name="type">The sprite type.</param>
        /// <param name="family">The sprite family; Null if the sprite has no family.</param>
        /// <returns>The list of sprite variations.</returns>
        public List<int> GetSpriteVariations(string category, string type, string family = null)
        {
            return this.spriteMap[Tuple.Create(category, type, family)].Keys.ToList();
        }

        /// <summary>
        /// Get a random variation of the sprite with the given identifying attributes.
        /// </summary>
        /// <param name="category">The sprite category.</param>
        /// <param name="type">The sprite type.</param>
        /// <param name="family">The sprite family; Null if the sprite has no family.</param>
        /// <returns>The variation; -1 if no variations exist.</returns>
        public int GetRandomSpriteVariation(string category, string type, string family = null)
        {
            List<int> variations = this.GetSpriteVariations(category, type, family);
            if (variations.Count > 0)
            {
                return variations[this.random.Next(0, variations.Count)];
            }
            else
            {
                return -1;
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
            foreach (KeyValuePair<string, Rectangle> kvp in this.spriteRectangles)
            {
                string spriteName = kvp.Key;
                Rectangle rectangle = kvp.Value;

                MatchCollection matches = this.regexSpriteInfo.Matches(spriteName);
                if (matches.Count == 1)
                {
                    string category = matches[0].Groups["category"].Value;
                    string name = matches[0].Groups["name"].Value;
                    string family = matches[0].Groups["family"].Value;
                    int variation = !string.IsNullOrEmpty(matches[0].Groups["variation"].Value) ?
                        int.Parse(matches[0].Groups["variation"].Value) : -1;

                    Tuple<string, string, string> key = Tuple.Create(category, name, family);
                    if (this.spriteMap.ContainsKey(key))
                    {
                        this.spriteMap[key].Add(variation, spriteName);
                    }
                    else
                    {
                        this.spriteMap.Add(key, new Dictionary<int, string> { { variation, spriteName } });
                    }
                }
            }
        }

        #endregion
    }
}
