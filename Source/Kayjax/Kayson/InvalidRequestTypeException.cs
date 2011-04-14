//-----------------------------------------------------------------------
// <copyright file="InvalidRequestTypeException.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when the type of request being made is invalid.
    /// </summary>
    [Serializable]
    public class InvalidRequestTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidRequestTypeException class.
        /// </summary>
        public InvalidRequestTypeException()
            : base("Could not determine the type of request being made.") 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the InvalidRequestTypeException class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        public InvalidRequestTypeException(string message) 
            : base(message) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the InvalidRequestTypeException class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="innerException">The exception that caused this exception to be thrown.</param>
        public InvalidRequestTypeException(string message, Exception innerException) 
            : base(message, innerException) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the InvalidRequestTypeException class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidRequestTypeException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        { 
        }
    }
}
