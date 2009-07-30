using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Kayson
{
    /// <summary>
    /// Represents a custom exception who's message will always be rendered to the client.
    /// </summary>
    [Serializable()]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class KaysonCustomException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public KaysonCustomException() : base() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message to render to the client.</param>
        public KaysonCustomException(string message) : base(message) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message to render to the client.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public KaysonCustomException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerException">The inner exception who's message should be rendered to the client.</param>
        public KaysonCustomException(Exception innerException) : base(innerException.Message, innerException) { }

        protected KaysonCustomException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
