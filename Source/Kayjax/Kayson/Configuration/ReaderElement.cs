//-----------------------------------------------------------------------
// <copyright file="ReaderElement.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents a reader element in the configuration.
    /// </summary>
    public sealed class ReaderElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the content type the reader reads.
        /// </summary>
        [ConfigurationProperty("contentType", IsRequired = true, IsKey = true)]
        public string ContentType
        {
            get { return (string)this["contentType"]; }
            set { this["contentType"] = value; }
        }

        /// <summary>
        /// Gets or sets the reader type to use.
        /// </summary>
        [ConfigurationProperty("readerType", IsRequired = true)]
        public string ReaderType
        {
            get { return (string)this["readerType"]; }
            set { this["readerType"] = value; }
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
