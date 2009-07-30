using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Kayson.Configuration
{
    /// <summary>
    /// Represents a collection of RouteElements in the configuration.
    /// </summary>
    public sealed class RoutesElementCollection : ConfigurationElementCollection, ICollection<RouteElement>
    {
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

        #region ICollection<RouteElement> Members

        void ICollection<RouteElement>.Add(RouteElement item)
        {
            BaseAdd(item);
        }

        void ICollection<RouteElement>.Clear()
        {
            BaseClear();
        }

        bool ICollection<RouteElement>.Contains(RouteElement item)
        {
            return BaseIndexOf(item) > -1;
        }

        void ICollection<RouteElement>.CopyTo(RouteElement[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        int ICollection<RouteElement>.Count
        {
            get { return base.Count; }
        }

        bool ICollection<RouteElement>.IsReadOnly
        {
            get { return base.IsReadOnly(); }
        }

        bool ICollection<RouteElement>.Remove(RouteElement item)
        {
            lock (this)
            {
                int count = Count;
                BaseRemove(item);

                return count != Count;
            }
        }

        #endregion

        #region IEnumerable<RouteElement> Members

        IEnumerator<RouteElement> IEnumerable<RouteElement>.GetEnumerator()
        {
            return (IEnumerator<RouteElement>)base.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion
    }
}
