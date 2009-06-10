using System;

namespace Kayson
{
    /// <summary>
    /// Exception thrown when the type of request being made is invalid.
    /// </summary>
    public class InvalidRequestTypeException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InvalidRequestTypeException()
            : base("Could not determine the type of request being made.") { }
    }
}
