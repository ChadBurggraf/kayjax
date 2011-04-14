//-----------------------------------------------------------------------
// <copyright file="WriterElementCollection.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Represents a collection of <see cref="WriterElement"/>s in the configuration.
    /// </summary>
    public sealed class WriterElementCollection : ConfigurationElementCollection<WriterElement>
    {
        /// <summary>
        /// Gets a value indicating whether the collection contains the given item.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>True if the collection contains the item, false otherwise.</returns>
        public override bool Contains(WriterElement item)
        {
            return this.Any(e => e.AcceptType == item.AcceptType);
        }

        /// <summary>
        /// Gets the unique key of the given element.
        /// </summary>
        /// <param name="element">The element to get the key of.</param>
        /// <returns>The given element's key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WriterElement)element).AcceptType;
        }
    }
}
