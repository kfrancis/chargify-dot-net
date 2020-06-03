﻿using System.Globalization;

namespace ChargifyNET.Json
{
    /// <summary>
    /// Object representing a JsonNumber
    /// </summary>
    public sealed class JsonNumber : JsonValue
    {
        double _value;

        /// <summary>
        /// The long value of this JsonNumber object (if applicable)
        /// </summary>
        public long LongValue
        {
            get { return (long)_value; }
        }

        /// <summary>
        /// The Integer value of this JsonNumber object (if applicable)
        /// </summary>
        public int IntValue
        {
            get { return (int)_value; }
        }

        /// <summary>
        /// The double value of this JsonNumber object (if applicable)
        /// </summary>
        public double DoubleValue
        {
            get { return _value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The double value of the object</param>
        public JsonNumber(double value)
        {
            _value = value;
        }

        /// <summary>
        /// Static method used to determine if a character is a digit or part of a number
        /// </summary>
        /// <param name="ch">The character to examine</param>
        /// <returns>True if it is part of a number, false otherwise.</returns>
        public static bool IsNumberPart(char ch)
        {
            return (char.IsDigit(ch) || (ch == '-') || (ch == '+')
                || (ch == '.') || (ch == 'e') || (ch == 'E'));
        }

        internal static JsonNumber Parse(string str, ref int position)
        {
            EatSpaces(str, ref position);

            int startPos = position;
            while (IsNumberPart(str[position]))
                position++;

            var value = double.Parse(str.Substring(startPos, position - startPos),
                CultureInfo.InvariantCulture);

            return new JsonNumber(value);
        }

        /// <summary>
        /// Method for getting the string representation of the double value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}", DoubleValue);
        }
    }
}
