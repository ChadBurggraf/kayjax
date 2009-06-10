using System;
using System.Configuration;

namespace Kayson.Configuration
{
    /// <summary>
    /// Represents a Route configuration element.
    /// </summary>
    public class RouteElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the route's regex pattern.
        /// </summary>
        [ConfigurationProperty("pattern", IsRequired = true, IsKey = true)]
        public string Pattern
        {
            get { return (string)this["pattern"]; }
        }

        /// <summary>
        /// Gets the type that the pattern rewrites to.
        /// </summary>
        [ConfigurationProperty("routesTo", IsRequired = true)]
        public string RoutesTo
        {
            get { return (string)this["routesTo"]; }
        }
    }
}
