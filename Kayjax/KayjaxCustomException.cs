using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Kayjax
{
    /// <summary>
    /// Represents a custom exception who's message will always be rendered to the client.
    /// </summary>
    [Serializable()]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class KayjaxCustomException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public KayjaxCustomException() : base() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        public KayjaxCustomException(string message) : base(message) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ex">The inner exception who's message should be rendered to the client.</param>
        public KayjaxCustomException(Exception ex) : base(ex.Message, ex) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="innerException">The exception that caused this exception to be thrown.</param>
        public KayjaxCustomException(string message, Exception innerException) : base(message, innerException) { }

        protected KayjaxCustomException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
