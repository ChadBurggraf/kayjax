//-----------------------------------------------------------------------
// <copyright file="RoutesElementCollection.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Represents a collection of RouteElements in the configuration.
    /// </summary>
    public sealed class RoutesElementCollection : ConfigurationElementCollection, ICollection<RouteElement>
    {
        #region Public Instance Properties

        /// <summary>
        /// Gets a value indicating whether the colleciton is read only.
        /// </summary>
        public new bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Adds a new item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(RouteElement item)
        {
            BaseAdd(item);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the given item.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>True if the collection contains the item, false otherwise.</returns>
        public bool Contains(RouteElement item)
        {
            return this.Any(i => i.Pattern.Equals(item.Pattern, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets an enumerator that can be used to enumerate over the collection.
        /// </summary>
        /// <returns>The collection's enumerator.</returns>
        public new IEnumerator<RouteElement> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return BaseGet(i) as RouteElement;
            }
        }

        /// <summary>
        /// Copies the collection to the given array, starting at the given index in the array.
        /// </summary>
        /// <param name="array">The array to copy elements to.</param>
        /// <param name="arrayIndex">The index in the array to start copying at.</param>
        public void CopyTo(RouteElement[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the given item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was found and removed, false otherwise.</returns>
        public bool Remove(RouteElement item)
        {
            bool exists = this.Contains(item);
            BaseRemove(GetElementKey(item));

            return exists;
        }

        #endregion

        #region Protected Instance Methods

        /// <summary>
        /// Creates a new instance of the collection's contained <see cref="ConfigurationElement"/> type.
        /// </summary>
        /// <returns>A new <see cref="ConfigurationElement"/> instance.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new RouteElement();
        }

        /// <summary>
        /// Gets the key for the given configuration element.
        /// </summary>
        /// <param name="element">The configuration element to get the key for.</param>
        /// <returns>The element's key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RouteElement)element).Pattern;
        }

        #endregion
    }
}
