

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
        public RoutesElementCollection Routes
        {
            get { return (RoutesElementCollection)(this["routes"] ?? (this["routes"] = new RoutesElementCollection())); }
        }

        /// <summary>
        /// Gets the KaysonSettings section from the configuration.
        /// </summary>
        public static KaysonSettings Section
        {
            get { return (KaysonSettings)ConfigurationManager.GetSection("kayson") ?? new KaysonSettings(); }
        }
    }
}
