﻿using System.Collections.Generic;

namespace ChargifyNET.Json
{
    /// <summary>
    /// Object representing a JsonArray
    /// </summary>
    public sealed class JsonArray : JsonValue
    {
        readonly List<JsonValue> _items;

        /// <summary>
        /// The JsonValue items in the array
        /// </summary>
        public JsonValue[] Items
        {
            get { return _items.ToArray(); }
        }

        /// <summary>
        /// The number of items in the JSON array
        /// </summary>
        public int Length
        {
            get { return _items.Count; }
        }

        private JsonArray()
        {
            _items = new List<JsonValue>();
        }

        private void Add(JsonValue jsonValue)
        {
            _items.Add(jsonValue);
        }

        internal static JsonArray Parse(string str, ref int position)
        {
            JsonArray jsonArray = new();

            EatSpaces(str, ref position);

            if (str[position] != '[')
                throw new JsonParseException(str, position);

            var firstItem = true;

            while (position < str.Length)
            {
                position++;

                // Check for empty array
                if (firstItem)
                {
                    firstItem = false;
                    EatSpaces(str, ref position);

                    if (str[position] == ']')
                    {
                        position++;
                        return jsonArray;
                    }
                }

                var jsonValue = ParseValue(str, ref position);
                jsonArray.Add(jsonValue);

                EatSpaces(str, ref position);

                if (str[position] == ']')
                {
                    position++;
                    return jsonArray;
                }

                if (str[position] != ',')
                    throw new JsonParseException(str, position);
            }

            throw new JsonParseException(string.Format("Unexpected end of string during reading JSON array: '{0}'", str));
        }
    }
}
