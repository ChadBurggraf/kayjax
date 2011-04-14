//-----------------------------------------------------------------------
// <copyright file="KayjaxCustomException.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayjax
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a custom exception who's message will always be rendered to the client.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class KayjaxCustomException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the KayjaxCustomException class.
        /// </summary>
        public KayjaxCustomException() 
            : base() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the KayjaxCustomException class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        public KayjaxCustomException(string message) 
            : base(message) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the KayjaxCustomException class.
        /// </summary>
        /// <param name="ex">The inner exception who's message should be rendered to the client.</param>
        public KayjaxCustomException(Exception ex) 
            : base(ex.Message, ex) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the KayjaxCustomException class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="innerException">The exception that caused this exception to be thrown.</param>
        public KayjaxCustomException(string message, Exception innerException) 
            : base(message, innerException) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the KayjaxCustomException class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected KayjaxCustomException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        { 
        }
    }
}
