using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargify
{
    /// <summary>
    /// Chargify Exception
    /// </summary>
    [Serializable]
    public class ChargifyException : Exception
    {
        /// <summary>
        /// Exception
        /// </summary>
        /// <param name="message">The exception message</param>
        public ChargifyException(string message) : base(message) { }

        /// <summary>
        /// Exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The exception that caused this exception to be thrown</param>
        public ChargifyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
