//-----------------------------------------------------------------------
// <copyright file="WriterElement.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents a writer element in the configuration.
    /// </summary>
    public sealed class WriterElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the accept type used to select the writer.
        /// </summary>
        [ConfigurationProperty("acceptType", IsRequired = true, IsKey = true)]
        public string AcceptType
        {
            get { return (string)this["acceptType"]; }
            set { this["acceptType"] = value; }
        }

        /// <summary>
        /// Gets or sets the writer type to use.
        /// </summary>
        [ConfigurationProperty("writerType", IsRequired = true)]
        public string WriterType
        {
            get { return (string)this["writerType"]; }
            set { this["writerType"] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the configuration element is read-only.
        /// </summary>
        /// <returns>True if the element is read-only, false otherwise.</returns>
        public override bool IsReadOnly()
        {
            return false;
        }
    }
}
