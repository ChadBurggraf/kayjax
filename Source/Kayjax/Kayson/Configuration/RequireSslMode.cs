

namespace Kayson.Configuration
{
    using System;

    /// <summary>
    /// Defines the possible modes for the RequireSSL attribute in KaysonSettings.
    /// </summary>
    public enum RequireSslMode
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
}
