using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChargifyNET.Json
{
    /// <summary>
    /// Object representing JSON boolean value
    /// </summary>
    public sealed class JsonBoolean : JsonValue
    {
        bool _value;

        /// <summary>
        /// The value of this object (as bool)
        /// </summary>
        public bool Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The value of this object</param>
        public JsonBoolean(bool value)
        {
            _value = value;
        }

        /// <summary>
        /// Returns the boolean string of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
