//-----------------------------------------------------------------------
// <copyright file="RouteElement.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents a Route configuration element.
    /// </summary>
    public class RouteElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the route's regex pattern.
        /// </summary>
        [ConfigurationProperty("pattern", IsRequired = true, IsKey = true)]
        public string Pattern
        {
            get { return (string)this["pattern"]; }
            set { this["pattern"] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IRequestReader"/> type to use when reading
        /// requests to this route, if overriding the default.
        /// </summary>
        [ConfigurationProperty("readerType", IsRequired = false)]
        public string ReaderType
        {
            get { return (string)this["readerType"]; }
            set { this["readerType"] = value; }
        }

        /// <summary>
        /// Gets or sets the type that the pattern rewrites to.
        /// </summary>
        [ConfigurationProperty("routesTo", IsRequired = true)]
        public string RoutesTo
        {
            get { return (string)this["routesTo"]; }
            set { this["routesTo"] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IResponseWriter"/> type to use when writing
        /// responses from this route, if overriding the default.
        /// </summary>
        [ConfigurationProperty("writerType", IsRequired = false)]
        public string WriterType
        {
            get { return (string)this["writerType"]; }
            set { this["writerType"] = value; }
        }
    }
}
