using System;

namespace Kayjax
{
    /// <summary>
    /// Represents a custom exception who's message will always be rendered to the client.
    /// </summary>
    public class KayjaxCustomException : Exception
    {
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
    }
}
