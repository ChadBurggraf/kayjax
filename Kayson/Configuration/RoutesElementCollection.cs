using System;
using System.Configuration;

namespace Kayson.Configuration
{
    /// <summary>
    /// Represents a collection of RouteElements in the configuration.
    /// </summary>
    public class RoutesElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RouteElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RouteElement)element).Pattern;
        }
    }
}
