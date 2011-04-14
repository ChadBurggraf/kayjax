//-----------------------------------------------------------------------
// <copyright file="JsonSerializationException.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when JSON serialization fails.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class JsonSerializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the JsonSerializationException class.
        /// </summary>
        public JsonSerializationException() 
            : base() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the JsonSerializationException class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        public JsonSerializationException(string message) 
            : base(message) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the JsonSerializationException class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="innerException">The exception that caused this exception to be thrown.</param>
        public JsonSerializationException(string message, Exception innerException) 
            : base(message, innerException) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the JsonSerializationException class.
        /// </summary>
        /// <param name="type">The type being serialized that caused the exception.</param>
        /// <param name="innerException">The exception that caused this exception to be thrown.</param>
        public JsonSerializationException(Type type, Exception innerException)
            : base(String.Format(CultureInfo.InvariantCulture, "There was an error during JSON serialization of type \"{0}\".", type.Name), innerException) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the JsonSerializationException class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected JsonSerializationException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        { 
        }
    }
}
