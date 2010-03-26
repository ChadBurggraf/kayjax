

namespace Kayson
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an API response sent to the client after an API request has been processed.
    /// </summary>
    [Serializable(), DataContract()]
    public class ApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the ApiResponse class.
        /// </summary>
        public ApiResponse() { }

        /// <summary>
        /// Initializes a new instance of the ApiResponse class.
        /// </summary>
        /// <param name="success">A value indicating whether the request was successful.</param>
        /// <param name="reason">A reason for request failure if applicable.</param>
        /// <param name="value">A return value containing the results of the request.</param>
        /// <param name="allowed">A value indicating whether the user was granted access to the request.</param>
        public ApiResponse(bool success, string reason, object value, bool allowed)
        {
            this.Success = success;
            this.Reason = reason;
            this.Value = value;
            this.Allowed = allowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user was granted access to the request.
        /// </summary>
        [DataMember(Name = "allowed")]
        public bool Allowed { get; set; }

        /// <summary>
        /// Gets or sets the reason for request failure if applicable.
        /// </summary>
        [DataMember(Name = "reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request was successful.
        /// </summary>
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a return value if applicable.
        /// </summary>
        [DataMember(Name = "value")]
        public object Value { get; set; }
    }
}
