using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace Kayson
{
    /// <summary>
    /// Exception thrown when JSON serialization fails.
    /// </summary>
    [Serializable()]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class JsonSerializationException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonSerializationException() : base() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        public JsonSerializationException(string message) : base(message) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="innerException">The exception that caused this exception to be thrown.</param>
        public JsonSerializationException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type being serialized that caused the exception.</param>
        /// <param name="innerException">The exception that caused this exception to be thrown.</param>
        public JsonSerializationException(Type type, Exception innerException)
            : base(String.Format(CultureInfo.InvariantCulture, "There was an error during JSON serialization of type \"{0}\".", type.Name), innerException) { }

        protected JsonSerializationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
