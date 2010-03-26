

namespace Kayson
{
    using System;

    /// <summary>
    /// Represents the result of an API action.
    /// </summary>
    public class ApiActionResult : ApiResult
    {
        /// <summary>
        /// Gets or sets the return object of the API request, if applicable.
        /// </summary>
        public object Value { get; set; }
    }
}
