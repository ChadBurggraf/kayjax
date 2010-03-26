

namespace Kayson
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a custom exception who's message will always be rendered to the client.
    /// </summary>
    [Serializable()]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class KaysonCustomException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the KaysonCustomException class.
        /// </summary>
        public KaysonCustomException() : base() { }

        /// <summary>
        /// Initializes a new instance of the KaysonCustomException class.
        /// </summary>
        /// <param name="message">The message to render to the client.</param>
        public KaysonCustomException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the KaysonCustomException class.
        /// </summary>
        /// <param name="message">The message to render to the client.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public KaysonCustomException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the KaysonCustomException class.
        /// </summary>
        /// <param name="innerException">The inner exception who's message should be rendered to the client.</param>
        public KaysonCustomException(Exception innerException) : base(innerException.Message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the KaysonCustomException class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected KaysonCustomException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
