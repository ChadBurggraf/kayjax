using System;
using System.Runtime.Serialization;

namespace Kayson
{
    /// <summary>
    /// Base class for API requests.
    /// </summary>
    [DataContract()]
    public abstract class ApiRequest
    {
        /// <summary>
        /// When implemented in a derived class, performs an API action.
        /// </summary>
        /// <param name="output">Contains the output value.</param>
        /// <param name="reason">Contains the reason for failure if applicable.</param>
        /// <returns>True if the action succeeded, false otherwise.</returns>
        public abstract bool Do(out object output, out string reason);

        /// <summary>
        /// When implemented in a derived class, validates the values instantiated into the request object.
        /// </summary>
        /// <param name="reason">Contains the reason for failure if applicable.</param>
        /// <returns>True if the request is valid, false otherwise.</returns>
        public abstract bool Validate(out string reason);
    }
}
