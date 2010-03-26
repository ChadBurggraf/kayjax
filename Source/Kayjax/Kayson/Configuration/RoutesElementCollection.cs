

namespace Kayson.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a collection of RouteElements in the configuration.
    /// </summary>
    public sealed class RoutesElementCollection : ConfigurationElementCollection, ICollection<RouteElement>
    {
        /// <summary>
        /// Gets the number of elements in the collection
        /// </summary>
        int ICollection<RouteElement>.Count
        {
            get { return base.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the colleciton is read only.
        /// </summary>
        bool ICollection<RouteElement>.IsReadOnly
        {
            get { return base.IsReadOnly(); }
        }

        /// <summary>
        /// Adds a new item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void ICollection<RouteElement>.Add(RouteElement item)
        {
            BaseAdd(item);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        void ICollection<RouteElement>.Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the given item.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>True if the collection contains the item, false otherwise.</returns>
        bool ICollection<RouteElement>.Contains(RouteElement item)
        {
            return BaseIndexOf(item) > -1;
        }

        /// <summary>
        /// Copies the collection to the given array, starting at the given index in the array.
        /// </summary>
        /// <param name="array">The array to copy elements to.</param>
        /// <param name="arrayIndex">The index in the array to start copying at.</param>
        void ICollection<RouteElement>.CopyTo(RouteElement[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets an enumerator that can be used to enumerate over the collection.
        /// </summary>
        /// <returns>The collection's enumerator.</returns>
        IEnumerator<RouteElement> IEnumerable<RouteElement>.GetEnumerator()
        {
            return (IEnumerator<RouteElement>)base.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that can be used to enumerate over the collection.
        /// </summary>
        /// <returns>The collection's enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        /// <summary>
        /// Removes the given item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was found and removed, false otherwise.</returns>
        bool ICollection<RouteElement>.Remove(RouteElement item)
        {
            lock (this)
            {
                int count = Count;
                BaseRemove(item);

                return count != Count;
            }
        }

        /// <summary>
        /// Creates a new configuration element instance.
        /// </summary>
        /// <returns>The newly created instance.</returns>
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
    }
}
