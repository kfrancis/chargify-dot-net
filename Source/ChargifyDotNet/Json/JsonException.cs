using System;

namespace ChargifyDotNet.Json
{
    /// <summary>
    /// Exception for dealing with Json
    /// </summary>
    public class JsonException : Exception
    {
        /// <summary>
        /// JsonException constructor
        /// </summary>
        /// <param name="message">The message to display</param>
        public JsonException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// JsonException constructor
        /// </summary>
        public JsonException()
        {
        }

        /// <summary>
        /// JsonException constructor
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="innerException">The exception that caused this exception</param>
        public JsonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception for dealing with Json parsing
    /// </summary>
    public class JsonParseException : JsonException
    {
        /// <summary>
        /// JsonParseException Constructor
        /// </summary>
        /// <param name="message">The message to display</param>
        public JsonParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// JsonParseException constructor
        /// </summary>
        public JsonParseException()
        {
        }

        /// <summary>
        /// JsonParseException constructor
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="innerException">The exception that caused this exception</param>
        public JsonParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// JsonParseException constructor
        /// </summary>
        /// <param name="input">The JSON being parsed</param>
        /// <param name="position">The position where an error occurred parsing it</param>
        public JsonParseException(string input, int position)
            : this(string.Format("Unexpected character '{0}' at position {1}, input: '{2}'", input[position], position, input))
        {
        }
    }
}
