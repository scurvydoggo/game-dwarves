// ----------------------------------------------------------------------------
// <copyright file="IIndexedObject.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Common
{
    /// <summary>
    /// An item in an indexed collection.
    /// </summary>
    public interface IIndexedObject
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        int Index { get; set; }
    }
}