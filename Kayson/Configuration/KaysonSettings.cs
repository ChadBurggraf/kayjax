using System;
using System.Configuration;

namespace Kayson.Configuration
{
    /// <summary>
    /// Represents the Kayson configuration section.
    /// </summary>
    public class KaysonSettings : ConfigurationSection
    {
        /// <summary>
        /// Gets the application-relative URL of the KaysonHandlerFactory.
        /// </summary>
        [ConfigurationProperty("handlerUrl", IsRequired = false, DefaultValue = "~/kayson.ashx")]
        public string HandlerUrl
        {
            get { return (string)(this["handlerUrl"] ?? "~/kayson.ashx"); }
        }

        /// <summary>
        /// Gets a value indicating whether SSL is required whem making requests.
        /// </summary>
        [ConfigurationProperty("requireSsl", IsRequired = false, DefaultValue = RequireSSLMode.Off)]
        public RequireSSLMode RequireSsl
        {
            get { return (RequireSSLMode)(this["requireSsl"] ?? RequireSSLMode.Off); }
        }

        /// <summary>
        /// Gets the routes defined in the configuration.
        /// </summary>
        [ConfigurationProperty("routes")]
        public RoutesElementCollection Routes
        {
            get { return (RoutesElementCollection)(this["routes"] ?? new RoutesElementCollection()); }
        }

        /// <summary>
        /// Gets the KaysonSettings section from the configuration.
        /// </summary>
        public static KaysonSettings Section
        {
            get { return (KaysonSettings)ConfigurationManager.GetSection("kayson") ?? new KaysonSettings(); }
        }
    }

    #region RequireSSL Enum

    /// <summary>
    /// Defines the possible modes for the RequireSSL attribute in KaysonSettings.
    /// </summary>
    public enum RequireSSLMode
    {
        /// <summary>
        /// Identifies that require SSL is turned off.
        /// </summary>
        Off,

        /// <summary>
        /// Identifies that require SSL is turned on.
        /// </summary>
        On,

        /// <summary>
        /// Identifies that require SSL is turned on for remote requests only.
        /// </summary>
        RemoteOnly
    }

    #endregion
}
