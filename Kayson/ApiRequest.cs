using System;
using System.Diagnostics.CodeAnalysis;
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
        /// <returns>The result of the action.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "I like the name.")]
        public abstract ApiActionResult Do();

        /// <summary>
        /// When implemented in a derived class, validates the values instantiated into the request object.
        /// </summary>
        /// <returns>The result of the validation.</returns>
        public abstract ApiValidationResult Validate();
    }

    #region ApiActionResult Class

    /// <summary>
    /// Represents the result of an API action.
    /// </summary>
    public class ApiActionResult
    {
        private string reason;

        /// <summary>
        /// Gets or sets a value indicating whether the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the reason for failure, if applicable.
        /// </summary>
        public string Reason
        {
            get { return reason ?? (reason = String.Empty); }
            set { reason = value; }
        }

        /// <summary>
        /// Gets or sets the return object of the API request, if applicable.
        /// </summary>
        public object Value { get; set; }
    }

    #endregion

    #region ApiValidationResult Class

    /// <summary>
    /// Represents the result of an API validation call.
    /// </summary>
    public class ApiValidationResult
    {
        private string reason;

        /// <summary>
        /// Gets or sets a value indicating whether the validation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the reason for failure, if applicable.
        /// </summary>
        public string Reason
        {
            get { return reason ?? (reason = String.Empty); }
            set { reason = value; }
        }
    }

    #endregion
}
