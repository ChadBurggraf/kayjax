using System;

namespace Kayson
{
    /// <summary>
    /// Exception thrown when JSON serialization fails.
    /// </summary>
    public class JsonSerializationException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type that caused the exception</param>
        /// <param name="ex">The causing exception.</param>
        public JsonSerializationException(Type type, Exception ex)
            : base(String.Format("There was an error during JSON serialization of type \"{0}\".", type.Name), ex) { }
    }
}
