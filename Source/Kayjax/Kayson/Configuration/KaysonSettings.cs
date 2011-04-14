//-----------------------------------------------------------------------
// <copyright file="KaysonSettings.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson.Configuration
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the Kayson configuration section.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class KaysonSettings : ConfigurationSection
    {
        private static KaysonSettings section = (KaysonSettings)ConfigurationManager.GetSection("kayson") ?? new KaysonSettings();

        /// <summary>
        /// Gets the KaysonSettings section from the configuration.
        /// </summary>
        public static KaysonSettings Section
        {
            get { return section; }
        }

        /// <summary>
        /// Gets the readers defined in the configuration.
        /// </summary>
        [ConfigurationProperty("readers", IsRequired = false)]
        public ReaderElementCollection Readers
        {
            get
            {
                ReaderElementCollection readers = (ReaderElementCollection)this["readers"];

                if (readers == null || readers.Count == 0)
                {
                    readers = GetDefaultReaders();
                    this["readers"] = readers;
                }

                return readers;
            }
        }

        /// <summary>
        /// Gets or sets the application-relative URL of the KaysonHandlerFactory.
        /// </summary>
        [ConfigurationProperty("handlerUrl", IsRequired = false, DefaultValue = "~/kayson.ashx")]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design for storage in configuration.")]
        public string HandlerUrl
        {
            get { return (string)this["handlerUrl"]; }
            set { this["handlerUrl"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is required whem making requests.
        /// </summary>
        [ConfigurationProperty("requireSsl", IsRequired = false, DefaultValue = RequireSslMode.Off)]
        public RequireSslMode RequireSsl
        {
            get { return (RequireSslMode)this["requireSsl"]; }
            set { this["requireSsl"] = value; }
        }

        /// <summary>
        /// Gets the routes defined in the configuration.
        /// </summary>
        [ConfigurationProperty("routes", IsRequired = false)]
        public RouteElementCollection Routes
        {
            get { return (RouteElementCollection)(this["routes"] ?? (this["routes"] = new RouteElementCollection())); }
        }

        /// <summary>
        /// Gets the writers defined in the configuration.
        /// </summary>
        [ConfigurationProperty("writers", IsRequired = false)]
        public WriterElementCollection Writers
        {
            get
            {
                WriterElementCollection writers = (WriterElementCollection)this["writers"];

                if (writers == null || writers.Count == 0)
                {
                    writers = GetDefaultWriters();
                    this["writers"] = writers;
                }

                return writers;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the configuration section is read-only.
        /// </summary>
        /// <returns>True if the section is readon-only, false otherwise.</returns>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// Gets the default readers configuration.
        /// </summary>
        /// <returns>A collection of default readers.</returns>
        private static ReaderElementCollection GetDefaultReaders()
        {
            ReaderElementCollection readers = new ReaderElementCollection();
            readers.Add(new ReaderElement() { ContentType = "application/json", ReaderType = "Kayson.JsonRequestReader, Kayjax" });
            readers.Add(new ReaderElement() { ContentType = "application/x-bplist", ReaderType = "Kayson.BinaryPlistRequestReader, Kayjax" });
            return readers;
        }

        /// <summary>
        /// Gets the default writers configuration.
        /// </summary>
        /// <returns>A collection of default writers.</returns>
        private static WriterElementCollection GetDefaultWriters()
        {
            WriterElementCollection writers = new WriterElementCollection();
            writers.Add(new WriterElement() { AcceptType = "application/json", WriterType = "Kayson.JsonResponseWriter, Kayjax" });
            writers.Add(new WriterElement() { AcceptType = "application/x-bplist", WriterType = "Kayson.BinaryPlistResponseWriter, Kayjax" });
            return writers;
        }
    }
}
