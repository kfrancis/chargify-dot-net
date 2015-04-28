using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChargifyNET.Json
{
    /// <summary>
    /// Methods useful for parsing and getting types
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Method for getting a JsonString as a System.String
        /// </summary>
        /// <param name="jsonObject">The object to get a string from</param>
        /// <param name="fieldName">The field to retrieve</param>
        /// <returns>The string if applicable, null otherwise.</returns>
        public static string GetJsonStringAsString(JsonObject jsonObject, string fieldName)
        {
            return GetJsonStringAsString(jsonObject, fieldName, null);
        }

        /// <summary>
        /// Method for getting a System.String from a JsonString
        /// </summary>
        /// <param name="jsonObject">The object to get a string from</param>
        /// <param name="fieldName">The field to retrieve</param>
        /// <param name="defaultValue">The default value to return if not found.</param>
        /// <returns>The string if applicable, defaultValue otherwise.</returns>
        public static string GetJsonStringAsString(JsonObject jsonObject, string fieldName, string defaultValue)
        {
            if (jsonObject.ContainsKey(fieldName) &&
                (jsonObject[fieldName] is JsonString))
                return ((JsonString)jsonObject[fieldName]).Value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Method useful for validating that an object can be converted into a type
        /// </summary>
        /// <param name="jsonObject">The jsonObject to validate</param>
        /// <param name="fieldName">The field of the JsonObject to validate</param>
        /// <param name="fieldType">The type to try to validate against</param>
        public static void ValidateJsonField(JsonObject jsonObject, string fieldName, Type fieldType)
        {
            if (jsonObject == null)
                throw new NullReferenceException("jsonObject");

            if (jsonObject.ContainsKey(fieldName) == false)
                throw new JsonException(string.Format("Could not find key: '{0}' in JSON object", fieldName));

            if ((jsonObject[fieldName] == null) && (fieldType != null))
                throw new NullReferenceException(string.Format("jsonObject field '{0}' == NULL", fieldName));

            if (jsonObject[fieldName].GetType() != fieldType)
                throw new JsonException(string.Format("JSON field: '{0}', type is '{1}', but expecting: '{2}'", fieldName, jsonObject[fieldName].GetType(), fieldType));
        }
    }
}
