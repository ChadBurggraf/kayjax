//-----------------------------------------------------------------------
// <copyright file="ApiResponse.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an API response sent to the client after an API request has been processed.
    /// </summary>
    [DataContract]
    public class ApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the ApiResponse class.
        /// </summary>
        public ApiResponse() 
        {
            this.Success = true;
            this.Reason = String.Empty;
            this.Allowed = true;
            this.StatusCode = 200;
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
        /// Gets or sets the HTTP status code to send with the response.
        /// </summary>
        [IgnoreDataMember]
        public int StatusCode { get; set; }

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
