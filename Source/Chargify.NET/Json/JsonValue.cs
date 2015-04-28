using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChargifyNET.Json
{
    /// <summary>
    /// Object representing JSON value
    /// </summary>
    public class JsonValue
    {
        /// <summary>
        /// Eats spaces in the string
        /// </summary>
        /// <param name="str">The string to eat spaces from</param>
        /// <param name="position">The current position</param>
        protected static void EatSpaces(string str, ref int position)
        {

            while ((position < str.Length) &&
                    ((str[position] == ' ') || (str[position] == '\n'))
                  )
                position++;
        }

        /// <summary>
        /// Parses the JSON value into the JsonValue object
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <param name="position">The current position</param>
        /// <returns>The JsonValue if parsed, null otherwise.</returns>
        protected static JsonValue ParseValue(string str, ref int position)
        {
            JsonString.EatSpaces(str, ref position);

            char ch = str[position];

            // JsonString
            if (ch == '\"')
                return JsonString.Parse(str, ref position);
            // JsonObject
            else if (ch == '{')
                return JsonObject.Parse(str, ref position);
            // JsonArray
            else if (ch == '[')
                return JsonArray.Parse(str, ref position);
            // JsonNumber
            else if (JsonNumber.IsNumberPart(ch))
                return JsonNumber.Parse(str, ref position);
            // 'null'
            else if ((str.Length > position + 4) &&
                (str.Substring(position, 4).Equals("null", StringComparison.InvariantCultureIgnoreCase)))
            {
                position += 4;
                return null;
            }
            // 'true'
            else if ((str.Length > position + 4) &&
                (str.Substring(position, 4).Equals("true", StringComparison.InvariantCultureIgnoreCase)))
            {
                position += 4;
                return new JsonBoolean(true);
            }
            // 'false'
            else if ((str.Length > position + 5) &&
                (str.Substring(position, 5).Equals("false", StringComparison.InvariantCultureIgnoreCase)))
            {
                position += 5;
                return new JsonBoolean(false);
            }
            else
                throw new JsonParseException(str, position);
        }
    }
}
