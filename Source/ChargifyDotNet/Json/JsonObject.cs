﻿using ChargifyDotNet;
using System;
using System.Collections.Generic;

namespace ChargifyNET.Json
{
    /// <summary>
    /// Object representing a JsonObject
    /// </summary>
    public sealed class JsonObject : JsonValue
    {
        readonly Dictionary<string, JsonValue> _value;

        /// <summary>
        /// Enumerator into this object
        /// </summary>
        /// <param name="key">The string key to evaluate</param>
        /// <returns>The JsonValue at key</returns>
        public JsonValue this[string key]
        {
            get
            {
                if (key == null)
                    throw new NullReferenceException("key");

                if (ContainsKey(key) == false)
                    throw new JsonException("Key does not exists: " + key);

                return _value[key];
            }
        }

        /// <summary>
        /// Returns the collection of keys from this object
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return _value.Keys;
            }
        }

        JsonObject()
        {
            _value = new Dictionary<string, JsonValue>();
        }

        /// <summary>
        /// Method used to attempt to get a value from this Object
        /// </summary>
        /// <param name="key">The proposed key</param>
        /// <param name="value">The value to retrieve it to</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool TryGetValue(string key, out JsonValue value)
        {
            value = null;

            if (key == null)
                throw new NullReferenceException("key");

            if (ContainsKey(key) == false)
                return false;

            value = this[key];

            return true;
        }

        /// <summary>
        /// Does this object contain the specified key?
        /// </summary>
        /// <param name="key">The key in question</param>
        /// <returns>True if found, false otherwise. NullReferenceException if no key passed in.</returns>
        public bool ContainsKey(string key)
        {
            if (key == null)
                throw new NullReferenceException("key");

            return _value.ContainsKey(key);
        }

        void Add(string key, JsonValue value)
        {
            if (key == null)
                throw new NullReferenceException("key");

            if (ContainsKey(key))
                throw new JsonException(string.Format("Key '{0}' already exists in JsonObject", key));

            _value.Add(key, value);
        }

        /// <summary>
        /// Parsing method, used to parse JSON string into JsonObject
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <returns>JsonObject if applicable, null otherwise. JsonParseException if not properly formatted.</returns>
        public static JsonObject Parse(string str)
        {
            if (str == null)
                return null;

            if (str.Trim().EndsWith("}") == false)
                throw new JsonParseException("Json string does not terminates with '}': " + str);

            var startPos = 0;
            return Parse(str, ref startPos);
        }

        internal static JsonObject Parse(string str, ref int position)
        {
            EatSpaces(str, ref position);

            if (position >= str.Length)
                return null;

            if (str[position] != '{')
                throw new JsonParseException(str, position);

            JsonObject jsonObject = new();

            // Read all the pairs
            var continueReading = true;

            // Read starting '{'
            position++;
            while (continueReading)
            {
                EatSpaces(str, ref position);
                if (str[position] != '}')
                {
                    // Read string
                    var jsonString = JsonString.Parse(str, ref position);
                    var key = jsonString.Value;

                    // Read seperator ':'
                    EatSpaces(str, ref position);
                    if (str[position] != ':')
                        throw new JsonParseException(str, position);
                    position++;

                    // Read value
                    var value = ParseValue(str, ref position);

                    jsonObject.Add(key, value);
                }

                EatSpaces(str, ref position);
                if (str[position] == '}')
                    continueReading = false;
                else if (str[position] != ',')
                    throw new JsonParseException(str, position);

                // Skip "," between pair of items
                position++;
            }

            return jsonObject;
        }
    }
}
