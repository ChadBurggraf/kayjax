

namespace Kayson
{
    using System;

    /// <summary>
    /// Represents the result of an API operation.
    /// </summary>
    public class ApiResult
    {
        private string reason;

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the reason for failure, if applicable.
        /// </summary>
        public string Reason
        {
            get { return this.reason ?? (this.reason = String.Empty); }
            set { this.reason = value; }
        }
    }
}
