using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChargifyNET.Json
{
    /// <summary>
    /// Object representing the JsonString
    /// </summary>
    public sealed class JsonString : JsonValue
    {
        string _value;

        /// <summary>
        /// The value of this JsonString
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The value of this JsonString</param>
        public JsonString(string value)
        {
            _value = value;
        }

        static readonly char[] JsonDecodeStopChars = { '\\', '\"' };

        static string JsonDecode(string str, ref int position)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            EatSpaces(str, ref position);

            if (str[position] == '\"')
                position++;
            else
                throw new JsonParseException(str, position);

            while (position < str.Length)
            {
                int nextIndex = str.IndexOfAny(JsonDecodeStopChars, position);

                if (nextIndex < 0)
                {
                    sb.Append(str.Substring(position));
                    position = str.Length;

                    return sb.ToString();
                }
                else if (str[nextIndex] == '\"')
                {
                    string substring = str.Substring(position, nextIndex - position);

                    position = nextIndex + 1;

                    // Optimization: If the StringBuilder is empty - just return the current string
                    if (sb.Length == 0)
                        return substring;

                    // Optimization: If the current substring is empty - no need to append it.
                    if (substring.Length > 0)
                        sb.Append(substring);

                    return sb.ToString();
                }
                else if (str[nextIndex] == '\\')
                {
                    if (nextIndex > position)
                        sb.Append(str.Substring(position, nextIndex - position));

                    position = nextIndex + 1;

                    switch (str[position])
                    {
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 'u':
                            string number = str.Substring(position + 1, 4);
                            position += 4;
                            int code = int.Parse(number, System.Globalization.NumberStyles.HexNumber);
                            char ch = (char)code;
                            sb.Append(ch);
                            break;
                        default:
                            sb.Append(str[position]);
                            break;
                    }

                    position++;
                }
                else
                    throw new Exception("Failed JsonDecode");

                //switch (str[position])
                //{
                //    case '\\':
                //        position++;
                //        switch (str[position])
                //        {
                //            case 'b':
                //                sb.Append('\b');
                //                break;
                //            case 't':
                //                sb.Append('\t');
                //                break;
                //            case 'n':
                //                sb.Append('\n');
                //                break;
                //            case 'f':
                //                sb.Append('\f');
                //                break;
                //            case 'r':
                //                sb.Append('\r');
                //                break;
                //            case 'u':
                //                string number = str.Substring(position + 1, 4);
                //                position += 4;
                //                int code = int.Parse(number, System.Globalization.NumberStyles.HexNumber);
                //                char ch = (char)code;
                //                sb.Append(ch);
                //                break;
                //            default:
                //                sb.Append(str[position]);
                //                break;
                //        }
                //        break;
                //    case '\"':
                //        position++;
                //        return sb.ToString();
                //    default:
                //        sb.Append(str[position]);
                //        break;
                //}

                //position++;
            }

            return sb.ToString();
        }

        internal static JsonString Parse(string str, ref int position)
        {
            EatSpaces(str, ref position);

            if (str[position] != '\"')
                throw new JsonParseException(str, position);

            string jsonString = JsonDecode(str, ref position);

            return new JsonString(jsonString);
        }

        /// <summary>
        /// String value of this JsonString (same as Value)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
