using System;
using System.Runtime.Serialization;

namespace Kayson
{
    /// <summary>
    /// Exception thrown when the type of request being made is invalid.
    /// </summary>
    [Serializable()]
    public class InvalidRequestTypeException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InvalidRequestTypeException()
            : base("Could not determine the type of request being made.") { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        public InvalidRequestTypeException(string message) : base(message) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="innerException">The exception that caused this exception to be thrown.</param>
        public InvalidRequestTypeException(string message, Exception innerException) : base(message, innerException) { }

        protected InvalidRequestTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
