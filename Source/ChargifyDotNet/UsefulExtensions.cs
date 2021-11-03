namespace ChargifyNET
{
    #region Imports

    using ChargifyDotNet;
    using Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;

    #endregion Imports

    /// <summary>
    /// An enumeration of the types of masking styles for the Mask() extension method
    /// of the string class.
    /// </summary>
    public enum MaskStyle
    {
        /// <summary>
        /// Masks all characters within the masking region, regardless of type.
        /// </summary>
        All,

        /// <summary>
        /// Masks only alphabetic and numeric characters within the masking region.
        /// </summary>
        AlphaNumericOnly,
    }

    /// <summary>
    /// Utility class for string manipulation.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Default masking character used in a mask.
        /// </summary>
        public static readonly char DefaultMaskCharacter = '*';

        /// <summary>
        /// Returns true if the string is non-null and at least the specified number of characters.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="length">The minimum length.</param>
        /// <returns>True if string is non-null and at least the length specified.</returns>
        /// <exception>throws ArgumentOutOfRangeException if length is not a non-negative number.</exception>
        public static bool IsLengthAtLeast(this string value, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length,
                                                        "The length must be a non-negative number.");
            }

            return value != null && value.Length >= length;
        }

        /// <summary>
        /// Mask the source string with the mask char except for the last exposed digits.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="maskChar">The character to use to mask the source.</param>
        /// <param name="numExposed">Number of characters exposed in masked value.</param>
        /// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, char maskChar, int numExposed, MaskStyle style)
        {
            var maskedString = sourceValue;

            if (sourceValue.IsLengthAtLeast(numExposed))
            {
                var builder = new StringBuilder(sourceValue.Length);
                int index = maskedString.Length - numExposed;

                if (style == MaskStyle.AlphaNumericOnly)
                {
                    CreateAlphaNumMask(builder, sourceValue, maskChar, index);
                }
                else
                {
                    builder.Append(maskChar, index);
                }

                builder.Append(sourceValue.Substring(index));
                maskedString = builder.ToString();
            }

            return maskedString;
        }

        /// <summary>
        /// Mask the source string with the mask char except for the last exposed digits.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="maskChar">The character to use to mask the source.</param>
        /// <param name="numExposed">Number of characters exposed in masked value.</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, char maskChar, int numExposed)
        {
            return Mask(sourceValue, maskChar, numExposed, MaskStyle.All);
        }

        /// <summary>
        /// Mask the source string with the mask char.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="maskChar">The character to use to mask the source.</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, char maskChar)
        {
            return Mask(sourceValue, maskChar, 0, MaskStyle.All);
        }

        /// <summary>
        /// Mask the source string with the default mask char except for the last exposed digits.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="numExposed">Number of characters exposed in masked value.</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, int numExposed)
        {
            return Mask(sourceValue, DefaultMaskCharacter, numExposed, MaskStyle.All);
        }

        /// <summary>
        /// Mask the source string with the default mask char.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue)
        {
            return Mask(sourceValue, DefaultMaskCharacter, 0, MaskStyle.All);
        }

        /// <summary>
        /// Mask the source string with the mask char.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="maskChar">The character to use to mask the source.</param>
        /// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, char maskChar, MaskStyle style)
        {
            return Mask(sourceValue, maskChar, 0, style);
        }

        /// <summary>
        /// Mask the source string with the default mask char except for the last exposed digits.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="numExposed">Number of characters exposed in masked value.</param>
        /// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, int numExposed, MaskStyle style)
        {
            return Mask(sourceValue, DefaultMaskCharacter, numExposed, style);
        }

        /// <summary>
        /// Mask the source string with the default mask char.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, MaskStyle style)
        {
            return Mask(sourceValue, DefaultMaskCharacter, 0, style);
        }

        /// <summary>
        /// Masks all characters for the specified length.
        /// </summary>
        /// <param name="buffer">String builder to store result in.</param>
        /// <param name="source">The source string to pull non-alpha numeric characters.</param>
        /// <param name="mask">Masking character to use.</param>
        /// <param name="length">Length of the mask.</param>
        private static void CreateAlphaNumMask(StringBuilder buffer, string source, char mask, int length)
        {
            for (int i = 0; i < length; i++)
            {
                buffer.Append(char.IsLetterOrDigit(source[i])
                                ? mask
                                : source[i]);
            }
        }
    }

    /// <summary>
    /// Utf8 String Writer
    /// </summary>
    public class Utf8StringWriter : StringWriter
    {
        /// <summary>
        /// The encoding for this writer
        /// </summary>
        public override Encoding Encoding => Encoding.UTF8;
    }

    /// <summary>
    /// Extension class
    /// </summary>
    public static class UsefulExtensions
    {
        #region Utility Extensions

        private static readonly Regex _guidRegex = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);

        /// <summary>
        /// Retunrs the HtmlEncoded string
        /// </summary>
        /// <param name="input">The string to encode</param>
        /// <returns>The Html-encoded string (using the PCLWebUtility.WebUtility.HtmlEncode method)</returns>
        public static string ToHtmlEncoded(this string input)
        {
            return PCLWebUtility.WebUtility.HtmlEncode(input);
        }

        /// <summary>
        /// Extension method used to copy the response stream for the purposes of parsing
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="output">The output stream</param>
        public static void CopyStream(this Stream input, Stream output)
        {
            byte[] b = new byte[32768];
            int r;
            while ((r = input.Read(b, 0, b.Length)) > 0)
                output.Write(b, 0, r);
        }

        /// <summary>
        /// Determine if the webhook request is valid by calculating the expected signature and comparing it
        /// </summary>
        /// <param name="requestStream">The request stream</param>
        /// <param name="sharedKey">The site shared key from Chargify</param>
        /// <param name="givenSignature">The signature returned along side the webhook for verification</param>
        /// <returns>True if verified, false otherwise.</returns>
        public static bool IsWebhookRequestValid(this Stream requestStream, string sharedKey, string givenSignature = null)
        {
            bool retVal = true;
            string possibleData;
            using (StreamReader sr = new StreamReader(requestStream))
            {
                requestStream.Position = 0;
                possibleData = sr.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(possibleData))
            {
                var calculatedSignature = CalculateHMAC256Signature(possibleData, sharedKey);
                if (calculatedSignature != givenSignature)
                {
                    retVal = false;
                }
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Method to calculate the expected HMAC-SHA256 signture of a body of text using the site's sharedKey
        /// </summary>
        /// <param name="text">The text to run through the hashing algorithm</param>
        /// <param name="secret">The secret used to seed the hash</param>
        /// <returns>The hex hash of the passed in text</returns>
        public static string CalculateHMAC256Signature(string text, string secret)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);

            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(text);
            byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
            string hexaHash = string.Empty;
            foreach (byte b in hashMessage) { hexaHash += String.Format("{0:x2}", b); }

            return hexaHash;
        }

        /// <summary>
        /// Method for checking if the data passed back via the webhook interface are valid
        /// </summary>
        /// <param name="text">The body of the webhook message</param>
        /// <param name="signature">The signature in the header of the message</param>
        /// <param name="sharedKey">The site shared key (used to validate)</param>
        /// <returns>True if the signature matches the calculated value, false otherwise.</returns>
        public static bool IsChargifyWebhookContentValid(this string text, string signature, string sharedKey)
        {
            string unencryptedText = sharedKey + text;
            byte[] unencryptedData = Encoding.UTF8.GetBytes(unencryptedText);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(unencryptedData);
            string hexaHash = string.Empty;
            foreach (byte b in hash) { hexaHash += $"{b:x2}"; }

            // I'm not sure if it could be longer, so just compare the same number as characters.
            return signature == hexaHash.Substring(0, signature.Length);
        }

        /// <summary>
        /// Method for checking if the data passed back via the webhook interface are valid (using HMAC-SHA-256)
        /// </summary>
        /// <param name="text">The body of the webhook message</param>
        /// <param name="signature">The signature in the header of the message</param>
        /// <param name="sharedKey">The site shared key (used to validate)</param>
        /// <returns>True if the signature matches the calculated value, false otherwise.</returns>
        public static bool IsChargifyWebhookContentValidHMAC(string text, string signature, string sharedKey)
        {
            byte[] uncryptedData = Encoding.UTF8.GetBytes(text);

            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(sharedKey));
            byte[] hash = hmac.ComputeHash(uncryptedData);

            return BitConverter.ToString(hash).Replace("-", "").ToLower() == signature.ToLower();
        }

        /// <summary>
        /// Method for calculating the chargify direct signature
        /// </summary>
        /// <param name="text">Should be the concatenation of api_id, timestamp, nonce and data parameters</param>
        /// <param name="api_secret">The api secret issued to the API user by Chargify</param>
        /// <returns>The HMAC-SHA1 signature used to send to Chargify while using the API</returns>
        public static string GetChargifyDirectSignature(this string text, string api_secret)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(api_secret);

            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);
            byte[] messageBytes = encoding.GetBytes(text);
            byte[] hashMessage = hmacsha1.ComputeHash(messageBytes);
            string hexaHash = "";
            foreach (byte b in hashMessage) { hexaHash += String.Format("{0:x2}", b); }

            return hexaHash;
        }

        /// <summary>
        /// Extension method to change concatenated string into token used in hosted page url's
        /// </summary>
        /// <param name="text">The concatenated sting to tokenize</param>
        /// <returns>The first 10 characters of the hex digest SHA-1 of the message string.</returns>
        public static string GetChargifyHostedToken(this string text)
        {
            // Method used as listed here: http://support.chargify.com/faqs/technical/generating-hosted-page-urls
            byte[] data = Encoding.UTF8.GetBytes(text);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            byte[] hash = cryptoTransformSHA1.ComputeHash(data);
            string hexaHash = "";
            foreach (byte b in hash) { hexaHash += String.Format("{0:x2}", b); }
            // Get only the first 10 characters of the SHA-1 hex digest
            return hexaHash.Substring(0, 10);
        }

        /// <summary>
        /// Convert a customer into the attributes structure to use when creating a subscrition
        /// </summary>
        /// <param name="customer">The customer to convert</param>
        /// <returns>The converted result</returns>
        public static ICustomerAttributes ToCustomerAttributes(this ICustomer customer)
        {
            return new CustomerAttributes()
            {
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Organization = customer.Organization,
                Phone = customer.Phone,
                ShippingAddress = customer.ShippingAddress,
                ShippingAddress2 = customer.ShippingAddress2,
                ShippingCity = customer.ShippingCity,
                ShippingCountry = customer.ShippingCountry,
                ShippingState = customer.ShippingState,
                ShippingZip = customer.ShippingZip,
                SystemID = customer.SystemID
            };
        }

        /// <summary>
        /// Method for determining if a string is parsable XML
        /// </summary>
        /// <param name="value">The string value to test</param>
        /// <returns>Returns true if the string is XML parsable, false otherwise.</returns>
        public static bool IsXml(this string value)
        {
            try
            {
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Method for determining if a string is parsable JSON
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>True if the string is JSON parsable, false otherwise.</returns>
        public static bool IsJSON(this string value)
        {
            try
            {
                if (value.StartsWith("[", StringComparison.InvariantCultureIgnoreCase))
                {
                    int position = 0;
                    JsonArray.Parse(value, ref position);
                }
                else
                {
                    JsonObject.Parse(value);
                }

                // If we get here, then all was well.
                return true;
            }
            catch (Exception)
            {
                // Something happened, not parsable.
                return false;
            }
        }

        /// <summary>
        /// Method for determining if a string is a valid GUID
        /// </summary>
        /// <param name="value">The value to examine</param>
        /// <returns>True if the value is a guid, false otherwise.</returns>
        public static bool IsGuid(this string value)
        {
            return !string.IsNullOrEmpty(value) && _guidRegex.IsMatch(value);
        }

        /// <summary>
        /// Method for converting a string to a guid.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The guid object of value.</returns>
        public static Guid ConvertToGuid(this string value)
        {
            return string.IsNullOrEmpty(value) ? Guid.Empty : (_guidRegex.IsMatch(value) ? new Guid(value) : Guid.Empty);
        }

        /// <summary>
        /// Method for converting an XmlDocument to JSON
        /// </summary>
        /// <param name="doc">The XmlDocument to convert</param>
        /// <returns>The JSON result</returns>
        public static string ToJson(this XmlDocument doc)
        {
            var result = XmlToJsonConverter.XmlToJson(doc);
            result = result.Replace(@"\", @"\\");
            return result;
        }

        /// <summary>
        /// Method that will change format money fields to something chargify can use.
        /// </summary>
        /// <param name="value">An amount, as a decimal type, to format for Chargify</param>
        /// <returns>The formatted amount.</returns>
        public static string ToChargifyCurrencyFormat(this decimal value)
        {
            var usCulture = CultureInfo.CreateSpecificCulture("en-US");
            var clonedNumbers = (NumberFormatInfo)usCulture.NumberFormat.Clone();
            clonedNumbers.CurrencyNegativePattern = 2;
            return string.Format(clonedNumbers, "{0:C2}", value).Replace("$", "");
        }

        #endregion Utility Extensions

        #region XML Node and JSON Extensions

        /// <summary>
        /// Method for getting the content of a JsonObject as an enum value of type ComponentDowngradeProrationScheme
        /// </summary>
        /// <param name="obj">The object whose value needs to be extracted</param>
        /// <param name="key">The key of the value to extract</param>
        /// <returns>The enumerated value that corresponds to the keyed extracted value</returns>
        public static ComponentDowngradeProrationScheme GetJSONContentAsProrationDowngradeScheme(this JsonObject obj, string key)
        {
            ComponentDowngradeProrationScheme result = ComponentDowngradeProrationScheme.Unknown;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        result = (ComponentDowngradeProrationScheme)Enum.Parse(typeof(ComponentDowngradeProrationScheme), str.Value.Replace("-", "_"), true);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a ComponentDowngradeProrationScheme
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The ComponentDowngradeProrationScheme value of the node, ComponentDowngradeProrationScheme.Unknown otherwise.</returns>
        public static ComponentDowngradeProrationScheme GetNodeContentAsProrationDowngradeScheme(this XmlNode node)
        {
            ComponentDowngradeProrationScheme result = ComponentDowngradeProrationScheme.Unknown;
            if (node.FirstChild != null)
            {
                if (node.FirstChild.Value != null)
                {
                    result = (ComponentDowngradeProrationScheme)Enum.Parse(typeof(ComponentDowngradeProrationScheme), node.FirstChild.Value.Replace("-", "_"), true);
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as an enum value of type ComponentUpgradeProrationScheme
        /// </summary>
        /// <param name="obj">The object whose value needs to be extracted</param>
        /// <param name="key">The key of the value to extract</param>
        /// <returns>The enumerated value that corresponds to the keyed extracted value, ComponentUpgradeProrationScheme.Unknown otherwise.</returns>
        public static ComponentUpgradeProrationScheme GetJSONContentAsProrationUpgradeScheme(this JsonObject obj, string key)
        {
            ComponentUpgradeProrationScheme result = ComponentUpgradeProrationScheme.Unknown;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        result = (ComponentUpgradeProrationScheme)Enum.Parse(typeof(ComponentUpgradeProrationScheme), str.Value.Replace("-", "_"), true);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a ComponentUpgradeProrationScheme
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The ComponentUpgradeProrationScheme value of the node, ComponentUpgradeProrationScheme.Unknown otherwise.</returns>
        public static ComponentUpgradeProrationScheme GetNodeContentAsProrationUpgradeScheme(this XmlNode node)
        {
            ComponentUpgradeProrationScheme result = ComponentUpgradeProrationScheme.Unknown;
            if (node.FirstChild != null)
            {
                if (node.FirstChild.Value != null)
                {
                    result = (ComponentUpgradeProrationScheme)Enum.Parse(typeof(ComponentUpgradeProrationScheme), node.FirstChild.Value.Replace("-", "_"), true);
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a decimal
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The decimal value of the node</returns>
        public static decimal GetNodeContentAsDecimal(this XmlNode node)
        {
            decimal result = 0;
            if (node.FirstChild != null)
            {
                if (!decimal.TryParse(node.FirstChild.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out result)) result = 0;
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as a decimal
        /// </summary>
        /// <param name="obj">The object whose value needs to be extracted</param>
        /// <param name="key">The key of the string to retrieve</param>
        /// <returns>The decimal value of the object at key</returns>
        public static decimal GetJSONContentAsDecimal(this JsonObject obj, string key)
        {
            decimal result = 0;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        if (decimal.TryParse(str.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                        {
                            return result;
                        }
                    }

                    if (obj[key] is JsonNumber value)
                    {
                        result = new decimal(value.DoubleValue);
                        return result;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as an nullable integer
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The nullable integer value of the node</returns>
        public static int? GetNodeContentAsNullableInt(this XmlNode node)
        {
            if (node.FirstChild != null)
            {
                if (int.TryParse(node.FirstChild.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out int result))
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a string
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The string value of the node</returns>
        public static string GetNodeContentAsString(this XmlNode node)
        {
            string result = string.Empty;
            if (node.FirstChild != null)
            {
                result = node.FirstChild.Value;
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as a string
        /// </summary>
        /// <param name="obj">The object whose value needs to be extracted</param>
        /// <param name="key">The key of the string to retrieve</param>
        /// <returns>The string value of the object at key</returns>
        public static string GetJSONContentAsString(this JsonObject obj, string key)
        {
            string result = string.Empty;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        result = str.Value;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as an long
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The long value of the node</returns>
        public static long GetNodeContentAsLong(this XmlNode node)
        {
            long result = 0;
            if (node.FirstChild != null)
            {
                if (!long.TryParse(node.FirstChild.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out result)) result = 0;
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as an integer
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The integer value of the node</returns>
        public static int GetNodeContentAsInt(this XmlNode node)
        {
            int result = 0;
            if (node.FirstChild != null)
            {
                if (!int.TryParse(node.FirstChild.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out result)) result = 0;
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as a nullable integer
        /// </summary>
        /// <param name="obj">The object whose value/key needs to be extracted</param>
        /// <param name="key">The key of the int to retrieve</param>
        /// <returns>The nullable integer value of the keyed object</returns>
        public static int? GetJSONContentAsNullableInt(this JsonObject obj, string key)
        {
            int? result = null;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonNumber value)
                    {
                        result = value.IntValue;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as an integer
        /// </summary>
        /// <param name="obj">The object whose value/key needs to be extracted</param>
        /// <param name="key">The key of the int to retrieve</param>
        /// <returns>The integer value of the keyed object</returns>
        public static long GetJSONContentAsLong(this JsonObject obj, string key)
        {
            long result = 0;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonNumber value)
                    {
                        result = value.LongValue;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as an integer
        /// </summary>
        /// <param name="obj">The object whose value/key needs to be extracted</param>
        /// <param name="key">The key of the int to retrieve</param>
        /// <returns>The integer value of the keyed object</returns>
        public static int GetJSONContentAsInt(this JsonObject obj, string key)
        {
            int result = 0;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonNumber value)
                    {
                        result = value.IntValue;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the keyed content of a JsonObject as a product family
        /// </summary>
        /// <param name="obj">The object whose value needs to be extracted</param>
        /// <param name="key">The key of the product family to retrieve</param>
        /// <returns>The product family value of the object as keyed</returns>
        public static ProductFamily GetJSONContentAsProductFamily(this JsonObject obj, string key)
        {
            ProductFamily result = null;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonObject familyObj)
                    {
                        result = new ProductFamily(familyObj);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a product family
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The product family value of the node</returns>
        public static ProductFamily GetNodeContentAsProductFamily(this XmlNode node)
        {
            ProductFamily result = null;
            if (node.FirstChild != null)
            {
                // create new product family object
                result = new ProductFamily(node);
            }
            return result;
        }

        /// <summary>
        /// Convert the xml node into a renewal line item
        /// </summary>
        /// <param name="node">The xml node containing renewal line item data</param>
        /// <returns>The renewal line item object, or null</returns>
        public static RenewalLineItem GetNodeContentAsRenewalLineItem(this XmlNode node)
        {
            RenewalLineItem result = null;
            if (node.FirstChild != null)
            {
                result = new RenewalLineItem(node);
            }
            return result;
        }

        public static ComponentPricePoint GetNodeContentAsComponentPricePoint(this XmlNode node)
        {
            ComponentPricePoint result = null;
            if (node.FirstChild != null)
            {
                result = new ComponentPricePoint(node);
            }
            return result;
        }

        /// <summary>
        /// Generic node convert method
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="node">The xml node to convert</param>
        /// <returns>The object if successful, null otherwise</returns>
        public static T ConvertNode<T>(this XmlNode node) where T : class
        {
            using (MemoryStream stm = new MemoryStream())
            using (StreamWriter stw = new StreamWriter(stm))
            {
                stw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                stw.WriteLine(node.OuterXml);
                stw.Flush();

                stm.Position = 0;

                var ser = new XmlSerializer(typeof(T));
                T result = (ser.Deserialize(stm) as T);
                return result;
            }
        }

        /// <summary>
        /// Convert the JsonObject to a list of Renewal Line Items
        /// </summary>
        /// <param name="obj">The json object to convert</param>
        /// <param name="key">The key for this object array</param>
        /// <returns>The list result, or empty list otherwise</returns>
        public static List<RenewalLineItem> GetJSONContentAsRenewalLineItems(this JsonObject obj, string key)
        {
            var renewalLineItems = new List<RenewalLineItem>();
            var renewalLineItemsArray = obj[key] as JsonArray;
            if (renewalLineItemsArray != null)
            {
                foreach (var jsonValue in renewalLineItemsArray.Items)
                {
                    var renewalLineItem = (JsonObject)jsonValue;
                    renewalLineItems.Add(new RenewalLineItem(renewalLineItem));
                }
            }
            // Sanity check, should be equal.
            if (renewalLineItemsArray != null && renewalLineItemsArray.Length != renewalLineItems.Count)
            {
                throw new JsonParseException(string.Format("Unable to parse public signup pages ({0} != {1})", renewalLineItemsArray.Length, renewalLineItems.Count));
            }
            return renewalLineItems;
        }

        /// <summary>
        /// Convert the XmlNode to a list of Renewal Line Items
        /// </summary>
        /// <param name="node">The xml node to convert</param>
        /// <returns>The list result, or empty list otherwise</returns>
        public static List<RenewalLineItem> GetNodeContentAsRenewalLineItems(this XmlNode node)
        {
            var lineItems = new List<RenewalLineItem>();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "line_item":
                        lineItems.Add(childNode.GetNodeContentAsRenewalLineItem());
                        break;
                }
            }
            // Sanity check, should be equal.
            if (node.ChildNodes.Count != lineItems.Count)
            {
                throw new JsonParseException(string.Format("Unable to parse renewal line items ({0} != {1})", node.ChildNodes.Count, lineItems.Count));
            }

            return lineItems;
        }

        /// <summary>
        /// Method of getting the PublicSignupPages object from an XmlNode
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The PublicSignupPages value of the node, empty list otherwise.</returns>
        public static List<IPublicSignupPage> GetNodeContentAsPublicSignupPages(this XmlNode node)
        {
            var publicSignupPages = new List<IPublicSignupPage>();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "public_signup_page":
                        publicSignupPages.Add(childNode.GetNodeContentAsPublicSignupPage());
                        break;
                }
            }
            // Sanity check, should be equal.
            if (node.ChildNodes.Count != publicSignupPages.Count)
            {
                throw new JsonParseException(string.Format("Unable to parse public signup pages ({0} != {1})", node.ChildNodes.Count, publicSignupPages.Count));
            }

            return publicSignupPages;
        }

        /// <summary>
        /// Method of getting the public signup pages object from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing the PublicSignupPages data</param>
        /// <param name="key">They key of the PublicSignupPages object</param>
        /// <returns>The list of PublicSignupPages if possible, empty list otherwise.</returns>
        public static List<IPublicSignupPage> GetJSONContentAsPublicSignupPages(this JsonObject obj, string key)
        {
            var publicSignupPages = new List<IPublicSignupPage>();
            var publicSignupPagesArray = obj[key] as JsonArray;
            if (publicSignupPagesArray != null)
            {
                foreach (var jsonValue in publicSignupPagesArray.Items)
                {
                    var publicSignupPage = (JsonObject)jsonValue;
                    publicSignupPages.Add(new PublicSignupPage(publicSignupPage));
                }
            }
            // Sanity check, should be equal.
            if (publicSignupPagesArray != null && publicSignupPagesArray.Length != publicSignupPages.Count)
            {
                throw new JsonParseException(string.Format("Unable to parse public signup pages ({0} != {1})", publicSignupPagesArray.Length, publicSignupPages.Count));
            }
            return publicSignupPages;
        }

        /// <summary>
        /// Method for getting a Customer object from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing the Customer data</param>
        /// <param name="key">They key of the customer object</param>
        /// <returns>The customer object if possible, null otherwise.</returns>
        public static Customer GetJSONContentAsCustomer(this JsonObject obj, string key)
        {
            Customer result = null;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonObject customerObj)
                    {
                        // create the new customer object
                        result = new Customer(customerObj);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a customer
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The customer of the node</returns>
        public static Customer GetNodeContentAsCustomer(this XmlNode node)
        {
            Customer result = null;
            if (node.FirstChild != null)
            {
                // create new Customer object
                result = new Customer(node);
            }
            return result;
        }

        /// <summary>
        /// Method for retrieving the PaymentProfileView from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the PaymentProfileView value from</param>
        /// <param name="key">The key of the PaymentProfileView field in the JsonObject</param>
        /// <returns>The PaymentProfileView value, null otherwise.</returns>
        public static PaymentProfileView GetJSONContentAsPaymentProfileView(this JsonObject obj, string key)
        {
            PaymentProfileView result = null;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonObject paymentProfileViewObj)
                    {
                        // create the new PaymentProfileView object
                        result = new PaymentProfileView(paymentProfileViewObj);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a PaymentProfileView
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The PaymentProfileView value of the node</returns>
        public static PaymentProfileView GetNodeContentAsPaymentProfileView(this XmlNode node)
        {
            PaymentProfileView result = null;
            if (node.FirstChild != null)
            {
                result = new PaymentProfileView(node);
            }
            return result;
        }

        /// <summary>
        /// Method for retrieving the Product from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the Product value from</param>
        /// <param name="key">The key of the Product field in the JsonObject</param>
        /// <returns>The Product value, null otherwise.</returns>
        public static Product GetJSONContentAsProduct(this JsonObject obj, string key)
        {
            Product result = null;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonObject customerObj)
                    {
                        // create the new product object
                        result = new Product(customerObj);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a Product
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The Product value of the node</returns>
        public static Product GetNodeContentAsProduct(this XmlNode node)
        {
            Product result = null;
            if (node.FirstChild != null)
            {
                result = new Product(node);
            }
            return result;
        }

        public static ComponentPrice GetNodeContentAsComponentPrice(this XmlNode node)
        {
            ComponentPrice result = null;
            if (node.FirstChild != null)
            {
                result = new ComponentPrice(node);
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as a boolean
        /// </summary>
        /// <param name="obj">The object whose value needs to be extracted</param>
        /// <param name="key">The key of the JsonObject to get the bool from</param>
        /// <returns>The boolean value of the object at key</returns>
        public static bool GetJSONContentAsBoolean(this JsonObject obj, string key)
        {
            bool result = false;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonBoolean objVal)
                    {
                        result = objVal.Value;
                    }
                }
            }
            return result;
        }

        public static List<ComponentPricePoint> GetJSONContentAsPricePoints(this JsonObject obj, string key)
        {
            var pricePoints = new List<ComponentPricePoint>();
            var pricePointsArray = obj[key] as JsonArray;
            if (pricePointsArray != null)
            {
                foreach (var jsonValue in pricePointsArray.Items)
                {
                    var pricePoint = (JsonObject)jsonValue;
                    pricePoints.Add(new ComponentPricePoint(pricePoint));
                }
            }
            // Sanity check, should be equal.
            if (pricePointsArray != null && pricePointsArray.Length != pricePoints.Count)
            {
                throw new JsonParseException(string.Format("Unable to parse component price points ({0} != {1})", pricePointsArray.Length, pricePoints.Count));
            }
            return pricePoints;
        }

        public static List<ComponentPrice> GetJSONContentAsPrices(this JsonObject obj, string key)
        {
            var prices = new List<ComponentPrice>();
            var pricesArray = obj[key] as JsonArray;
            if (pricesArray != null)
            {
                foreach (var jsonValue in pricesArray.Items)
                {
                    var price = (JsonObject)jsonValue;
                    prices.Add(new ComponentPrice(price));
                }
            }
            // Sanity check, should be equal.
            if (pricesArray != null && pricesArray.Length != prices.Count)
            {
                throw new JsonParseException(string.Format("Unable to parse component prices ({0} != {1})", pricesArray.Length, prices.Count));
            }
            return prices;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a boolean
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The boolean value of the node</returns>
        public static bool GetNodeContentAsBoolean(this XmlNode node)
        {
            bool result = false;
            if (node.FirstChild != null)
            {
                if (!Boolean.TryParse(node.FirstChild.Value, out result)) result = false;
            }
            return result;
        }

        public static List<ComponentPricePoint> GetNodeContentAsPricePoints(this XmlNode node)
        {
            var pricePoints = new List<ComponentPricePoint>();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "price_point":
                        pricePoints.Add(childNode.GetNodeContentAsComponentPricePoint());
                        break;
                }
            }
            // Sanity check, should be equal.
            if (node.ChildNodes.Count != pricePoints.Count)
            {
                throw new Exception(string.Format("Unable to parse component price points ({0} != {1})", node.ChildNodes.Count, pricePoints.Count));
            }

            return pricePoints;
        }

        public static List<ComponentPrice> GetNodeContentAsComponentPrices(this XmlNode node)
        {
            var prices = new List<ComponentPrice>();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "price":
                        prices.Add(childNode.GetNodeContentAsComponentPrice());
                        break;
                }
            }
            // Sanity check, should be equal.
            if (node.ChildNodes.Count != prices.Count)
            {
                throw new Exception(string.Format("Unable to parse component prices ({0} != {1})", node.ChildNodes.Count, prices.Count));
            }

            return prices;
        }

        /// <summary>
        /// Method for retrieving the DateTime from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the DateTime value from</param>
        /// <param name="key">The key of the DateTime field in the JsonObject</param>
        /// <returns>The DateTime value, DateTime.MinValue otherwise.</returns>
        public static DateTime GetJSONContentAsDateTime(this JsonObject obj, string key)
        {
            DateTime result = DateTime.MinValue;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        string dateStr = str.Value;
                        if (!DateTime.TryParse(dateStr, out result)) result = DateTime.MinValue;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Method for retrieving the DateTime from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the DateTime value from</param>
        /// <param name="key">The key of the DateTime field in the JsonObject</param>
        /// <returns>The DateTime value, DateTime.MinValue otherwise.</returns>
        public static DateTime? GetJSONContentAsNullableDateTime(this JsonObject obj, string key)
        {
            DateTime? retVal = null;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        string dateStr = str.Value;
                        if (DateTime.TryParse(dateStr, out var result)) retVal = result != DateTime.MinValue ? (DateTime?)result : null;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a DateTime object
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The DateTime value of the node</returns>
        public static DateTime GetNodeContentAsDateTime(this XmlNode node)
        {
            DateTime result = DateTime.MinValue;
            if (node.FirstChild != null)
            {
                if (!DateTime.TryParse(node.FirstChild.Value, out result)) result = DateTime.MinValue;
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a DateTime object
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The DateTime value of the node</returns>
        public static DateTime? GetNodeContentAsNullableDateTime(this XmlNode node)
        {
            DateTime? retVal = null;
            if (node.FirstChild != null)
            {
                if (DateTime.TryParse(node.FirstChild.Value, out var result)) retVal = result != DateTime.MinValue ? (DateTime?)result : null;
            }
            return retVal;
        }

        /// <summary>
        /// Method for retrieving the Guid from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the Guid value from</param>
        /// <param name="key">The key of the Guid field in the JsonObject</param>
        /// <returns>The Guid value, Guid.Empty otherwise.</returns>
        public static Guid GetJSONContentAsGuid(this JsonObject obj, string key)
        {
            Guid result = Guid.Empty;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        string guidStr = str.Value;
                        result = guidStr.ConvertToGuid();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a Guid
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The Guid value of the node</returns>
        public static Guid GetNodeContentAsGuid(this XmlNode node)
        {
            Guid result = Guid.Empty;
            if (node.FirstChild != null)
            {
                result = node.FirstChild.Value.ConvertToGuid();
            }
            return result;
        }

        /// <summary>
        /// Method for retrieving the PricingSchemeType from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the PricingSchemeType value from</param>
        /// <param name="key">The key of the PricingSchemeType field in the JsonObject</param>
        /// <returns>The PricingSchemeType value, PricingSchemeType.Unknown otherwise.</returns>
        public static PricingSchemeType GetJSONContentAsPricingSchemeType(this JsonObject obj, string key)
        {
            PricingSchemeType result = PricingSchemeType.Unknown;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        result = (PricingSchemeType)Enum.Parse(typeof(PricingSchemeType), str.Value, true);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a PricingSchemeType
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The PricingSchemeType value of the node, PricingSchemeType.Unknown otherwise.</returns>
        public static PricingSchemeType GetNodeContentAsPricingSchemeType(this XmlNode node)
        {
            PricingSchemeType result = PricingSchemeType.Unknown;
            if (node.FirstChild != null)
            {
                if (node.FirstChild.Value != null)
                {
                    result = (PricingSchemeType)Enum.Parse(typeof(PricingSchemeType), node.FirstChild.Value, true);
                }
            }
            return result;
        }

        /// <summary>
        /// Method for retrieving the ComponentType from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the ComponentType value from</param>
        /// <param name="key">The key of the ComponentType field in the JsonObject</param>
        /// <returns>The ComponentType value, ComponentType.Unknown otherwise.</returns>
        public static ComponentType GetJSONContentAsComponentType(this JsonObject obj, string key)
        {
            ComponentType result = ComponentType.Unknown;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        result = (ComponentType)Enum.Parse(typeof(ComponentType), str.Value, true);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a ComponentType
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The ComponentType value of the node, ComponentType.Unknown otherwise.</returns>
        public static ComponentType GetNodeContentAsComponentType(this XmlNode node)
        {
            ComponentType result = ComponentType.Unknown;
            if (node.FirstChild != null)
            {
                if (node.FirstChild.Value != null)
                {
                    result = (ComponentType)Enum.Parse(typeof(ComponentType), node.FirstChild.Value, true);
                }
            }
            return result;
        }

        /// <summary>
        /// Method for retrieving the PaymentCollectionMethod from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the PaymentCollectionMethod value from</param>
        /// <param name="key">The key of the PaymentCollectionMethod field in the JsonObject</param>
        /// <returns>The PaymentCollectionMethod value, PaymentCollectionMethod.Unknown otherwise.</returns>
        public static PaymentCollectionMethod GetJSONContentAsPaymentCollectionMethod(this JsonObject obj, string key)
        {
            PaymentCollectionMethod result = PaymentCollectionMethod.Unknown;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        if (Enum.TryParse(str.Value, true, out PaymentCollectionMethod parsedResult))
                        {
                            result = parsedResult;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for retrieving the TransactionType from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the TransactionType value from</param>
        /// <param name="key">The key of the TransactionType field in the JsonObject</param>
        /// <returns>The TransactionType value, TransactionType.Unknown otherwise.</returns>
        public static TransactionType GetJSONContentAsTransactionType(this JsonObject obj, string key)
        {
            TransactionType result = TransactionType.Unknown;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        string value = str.Value;
                        result = (TransactionType)Enum.Parse(typeof(TransactionType), value, true);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a TransactionType
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The TransactionType value of the node</returns>
        public static TransactionType GetNodeContentAsTransactionType(this XmlNode node)
        {
            TransactionType result = TransactionType.Unknown;
            if (node.FirstChild != null)
            {
                string nodeText = node.FirstChild.Value;
                result = (TransactionType)Enum.Parse(typeof(TransactionType), nodeText, true);
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a PaymentCollectionMethod
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The PaymentCollectionMethod value of the node</returns>
        public static PaymentCollectionMethod GetNodeContentAsPaymentCollectionMethod(this XmlNode node)
        {
            PaymentCollectionMethod result = PaymentCollectionMethod.Unknown;
            if (node.FirstChild != null)
            {
                if (Enum.TryParse(node.FirstChild.Value, true, out PaymentCollectionMethod parsedResult))
                {
                    result = parsedResult;
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a SubscriptionState
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The SubscriptionState value of the node</returns>
        public static SubscriptionState GetNodeContentAsSubscriptionState(this XmlNode node)
        {
            SubscriptionState result = SubscriptionState.Unknown;
            if (node.FirstChild != null)
            {
                result = (SubscriptionState)Enum.Parse(typeof(SubscriptionState), node.FirstChild.Value, true);
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as it's enum counterpart.
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The enumerated value of the node</returns>
        public static T GetNodeContentAsEnum<T>(this XmlNode node) where T : struct
        {
            T result = default;
            if (node.FirstChild != null)
            {
                if (CheckEnumIsDefined<T>(node.FirstChild.Value))
                {
                    result = (T)Enum.Parse(typeof(T), node.FirstChild.Value, true);
                }
            }
            return result;
        }

        /// <summary>
        /// Method for retrieving the SubscriptionState from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the SubscriptionState value from</param>
        /// <param name="key">The key of the SubscriptionState field in the JsonObject</param>
        /// <returns>The SubscriptionState value, SubscriptionState.Unknown otherwise.</returns>
        public static T GetJSONContentAsEnum<T>(this JsonObject obj, string key) where T : struct
        {
            T result = default;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        if (CheckEnumIsDefined<T>(str.Value))
                        {
                            result = (T)Enum.Parse(typeof(T), str.Value, true);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for retrieving the SubscriptionState from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the SubscriptionState value from</param>
        /// <param name="key">The key of the SubscriptionState field in the JsonObject</param>
        /// <returns>The SubscriptionState value, SubscriptionState.Unknown otherwise.</returns>
        public static SubscriptionState GetJSONContentAsSubscriptionState(this JsonObject obj, string key)
        {
            SubscriptionState result = SubscriptionState.Unknown;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonString str)
                    {
                        result = (SubscriptionState)Enum.Parse(typeof(SubscriptionState), str.Value, true);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a Charge
        /// </summary>
        /// <param name="node">The node whose value need to be parsed</param>
        /// <returns>The Charge, null otherwise</returns>
        public static Charge GetNodeContentAsCharge(this XmlNode node)
        {
            Charge result = null;
            if (node.FirstChild != null)
            {
                result = new Charge(node);
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as a PublicSignupPage
        /// </summary>
        /// <param name="obj">The object to retrieve the PublicSignupPage value from</param>
        /// <param name="key">The key of the PublicSignupPage field in the JsonObject</param>
        /// <returns>The PublicSignupPage value, null otherwise.</returns>
        public static Charge GetJSONContentAsCharge(this JsonObject obj, string key)
        {
            Charge result = null;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonObject chargeObj)
                    {
                        // create the new public signup page object
                        result = new Charge(chargeObj);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a PublicSignupPage
        /// </summary>
        /// <param name="node">The node whose value need to be parsed</param>
        /// <returns>The PublicSignupPage, null otherwise</returns>
        public static PublicSignupPage GetNodeContentAsPublicSignupPage(this XmlNode node)
        {
            PublicSignupPage result = null;
            if (node.FirstChild != null)
            {
                result = new PublicSignupPage(node);
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as a PublicSignupPage
        /// </summary>
        /// <param name="obj">The object to retrieve the PublicSignupPage value from</param>
        /// <param name="key">The key of the PublicSignupPage field in the JsonObject</param>
        /// <returns>The PublicSignupPage value, null otherwise.</returns>
        public static PublicSignupPage GetJSONContentAsPublicSignupPage(this JsonObject obj, string key)
        {
            PublicSignupPage result = null;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonObject publicSignupPageObj)
                    {
                        // create the new public signup page object
                        result = new PublicSignupPage(publicSignupPageObj);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a Transaction
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The Transaction value of the node</returns>
        public static Transaction GetNodeContentAsTransaction(this XmlNode node)
        {
            Transaction result = null;
            if (node.FirstChild != null)
            {
                result = new Transaction(node);
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of a JsonObject as a Transaction
        /// </summary>
        /// <param name="obj">The object to retrieve the Transaction value from</param>
        /// <param name="key">The key of the Transaction field in the JsonObject</param>
        /// <returns>The Transaction value, null otherwise.</returns>
        public static Transaction GetJSONContentAsTransaction(this JsonObject obj, string key)
        {
            Transaction result = null;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    if (obj[key] is JsonObject transactionObj)
                    {
                        // create the new transaction object
                        result = new Transaction(transactionObj);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for retrieving the IntervalUnit from a JsonObject
        /// </summary>
        /// <param name="obj">The object to retrieve the IntervalUnit value from</param>
        /// <param name="key">The key of the IntervalUnit field in the JsonObject</param>
        /// <returns>The IntervalUnit value, IntervalUnit.Unknown otherwise.</returns>
        public static IntervalUnit GetJSONContentAsIntervalUnit(this JsonObject obj, string key)
        {
            IntervalUnit result = IntervalUnit.Unknown;
            if (obj?.ContainsKey(key) == true)
            {
                JsonString str = obj[key] as JsonString;
                if (str.Value != null)
                {
                    if (!Enum.TryParse<IntervalUnit>(str.Value, true, out result))
                    {
                        result = IntervalUnit.Unknown;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Method for getting the content of an XmlNode as a IntervalUnit
        /// </summary>
        /// <param name="node">The node whose value needs to be extracted</param>
        /// <returns>The IntervalUnit value of the node</returns>
        public static IntervalUnit GetNodeContentAsIntervalUnit(this XmlNode node)
        {
            IntervalUnit result = IntervalUnit.Unknown;
            if (node.FirstChild != null)
            {
                if (!Enum.TryParse<IntervalUnit>(node.FirstChild.Value, true, out result))
                {
                    result = IntervalUnit.Unknown;
                }
            }
            return result;
        }

        #endregion XML Node and JSON Extensions

        #region Useful

        /// <summary>
        /// Check if the enum is defined
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <param name="value">The string value</param>
        /// <returns>True if defined, false otherwise</returns>
        public static bool CheckEnumIsDefined<T>(this string value)
        {
            return Enum.GetNames(typeof(T)).Any(x => String.Equals(x, value, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Method for determining if the current application is running under Windows Azure (Dev or Live)
        /// </summary>
        /// <returns>RoleEnvironment.IsAvailable result if possible, false otherwise.</returns>
        public static bool IsRunningAzure()
        {
            bool result = false;
            Assembly a;
            try
            {
                a =
                    Assembly.Load(
                        "Microsoft.WindowsAzure.ServiceRuntime, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                if (a != null)
                {
                    Type classType = a.GetType("Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment");
                    PropertyInfo pi = classType.GetProperty("IsAvailable");
                    // This is the same as calling RoleEnvironment.IsAvailable, but without requiring it be referenced.
                    result = (bool)pi.GetValue(null, null); // This should get the result of IsAvailable
                }
            }
            catch
            {
                // ignored
            }
            return result;
        }

        /// <summary>
        /// Method for accessing RoleEnvironment ConfigurationSettingValues without
        /// needing a reference to the library. These are late bound.
        /// </summary>
        /// <param name="settingName">The key of the config setting to retrieve</param>
        /// <returns>The value of the keyed config string, if applicable. String.Empty otherwise.</returns>
        public static string GetLateBoundRoleEnvironmentValue(string settingName)
        {
            string result = string.Empty;
            try
            {
                Assembly a = Assembly.Load("Microsoft.WindowsAzure.ServiceRuntime, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                if (a != null)
                {
                    Type classType = a.GetType("Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment");
                    MethodInfo mi = classType.GetMethod("GetConfigurationSettingValue");
                    // This is the same as calling RoleEnvironment.GetConfigurationSettingValue(settingName)
                    result = (string)mi.Invoke(null, new object[] { settingName });
                }
            }
            catch
            {
                // ignored
            }
            return result;
        }

        /// <summary>
        /// Method that converts a string to the object, must be supported XML or JSON
        /// This is used completely internally to ChargifyConnect
        /// </summary>
        /// <typeparam name="T">The type of object to parse the string as</typeparam>
        /// <param name="serverResponse">The XML or JSON response from Chargify</param>
        /// <param name="key">The JSON key that corresponds to the object to parse</param>
        /// <returns>The object of type T</returns>
        public static T ConvertResponseTo<T>(this string serverResponse, string key)
            where T : ChargifyBase
        {
            if (serverResponse.IsXml())
            {
                Type[] argTypes = { typeof(string) };
                ConstructorInfo cInfo = typeof(T).GetConstructor(argTypes);
                if (cInfo != null) return (T)cInfo.Invoke(new object[] { serverResponse });
            }
            else if (serverResponse.IsJSON())
            {
                // now build the jsonObject object based on response as JSON
                JsonObject obj = JsonObject.Parse(serverResponse);
                if (!string.IsNullOrEmpty(key))
                {
                    if (obj.ContainsKey(key) && obj.Keys.Count == 1)
                    {
                        Type[] argTypes = { typeof(JsonObject) };
                        ConstructorInfo cInfo = typeof(T).GetConstructor(argTypes);
                        if (cInfo != null) return (T)cInfo.Invoke(new object[] { obj[key] as JsonObject });
                    }
                    else
                    {
                        Type[] argTypes = { typeof(JsonObject) };
                        ConstructorInfo cInfo = typeof(T).GetConstructor(argTypes);
                        if (cInfo != null) return (T)cInfo.Invoke(new object[] { obj });
                    }
                }
                else
                {
                    throw new ArgumentException("No JSON key specified");
                }
            }
            // The object isn't of a known variety
            return null;
        }

        /// <summary>
        /// Nifty method for saving an object (aka updating) in Chargify, for user application.
        /// </summary>
        /// <typeparam name="T">The type of object you wish to save/update</typeparam>
        /// <param name="chargify">The ChargifyConnect object</param>
        /// <param name="objectToBeSaved">The object of type T that is to be saved/updated</param>
        /// <returns>The updated object if applicable, null otherwise.</returns>
        public static T Save<T>(this ChargifyConnect chargify, T objectToBeSaved)
            where T : ChargifyBase
        {
            T result = null;
            // Since there are different signatures for the various Update methods
            // we need to do this on a case per case basis. Can't use reflection here. :(
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    if (!typeof(ICustomer).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    result = (T)chargify.UpdateCustomer(objectToBeSaved as ICustomer);
                    break;

                case "subscription":
                    if (!typeof(ISubscription).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    result = (T)chargify.UpdateSubscription(objectToBeSaved as ISubscription);
                    break;

                case "coupon":
                    if (!typeof(ICoupon).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    result = (T)chargify.UpdateCoupon(objectToBeSaved as ICoupon);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Nifty method for retrieving an object from Chargify, for user application.
        /// </summary>
        /// <typeparam name="T">The type of object you wish to find</typeparam>
        /// <param name="chargify">The ChargifyConnect object</param>
        /// <param name="ID">The ID (or Handle if Product) to find</param>
        /// <param name="parentID">The ID of the parent, optional</param>
        /// <returns>The found object if applicable, null otherwise.</returns>
        public static T Find<T>(this ChargifyConnect chargify, object ID, int? parentID = null)
            where T : ChargifyBase
        {
            T result = null;
            // Since there are different signatures for the various Load methods
            // we need to do this on a case per case basis. Can't use reflection here. :(
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    if (!typeof(ICustomer).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    ICustomer customer = null;
                    // load the object depending on what the ID can be interpreted as,
                    // in this case it's either the ChargifyID (int) or the SystemID (string)
                    if (ID is string) customer = chargify.LoadCustomer(ID as string);
                    else if (ID is int customerId) customer = chargify.LoadCustomer(customerId);
                    result = (T)customer;
                    break;

                case "subscription":
                    if (!typeof(ISubscription).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    ISubscription subscription = null;
                    // load the object depending on what the ID can be interpreted as,
                    // in this case it's only SubscriptionID (int)
                    if (ID is int subscriptionId) subscription = chargify.LoadSubscription(subscriptionId);
                    result = (T)subscription;
                    break;

                case "product":
                    if (!typeof(IProduct).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    IProduct product = null;
                    // load the object depending on what the ID can be interpreted as,
                    // in this case it's either the ProductID (int) or the ProductHandle (string)
                    if (ID is string) product = chargify.LoadProduct(ID as string, true);
                    else if (ID is int productId) product = chargify.LoadProduct(productId.ToString(), false);
                    result = (T)product;
                    break;

                case "statement":
                    if (!typeof(IStatement).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    IStatement statement = null;
                    if (ID is int statementId) statement = chargify.LoadStatement(statementId);
                    result = (T)statement;
                    break;

                case "transaction":
                    if (!typeof(ITransaction).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    ITransaction transaction = null;
                    if (ID is int transactionId) transaction = chargify.LoadTransaction(transactionId);
                    result = (T)transaction;
                    break;

                case "coupon":
                    if (!typeof(ICoupon).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    ICoupon coupon = null;
                    if (ID is int couponId && parentID.HasValue && parentID.Value > 0) coupon = chargify.LoadCoupon(parentID.Value, couponId);
                    result = (T)coupon;
                    break;

                case "productfamily":
                    if (!typeof(IProductFamily).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    IProductFamily productFamily = null;
                    if (ID is int familyId) productFamily = chargify.LoadProductFamily(familyId);
                    result = (T)productFamily;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Method for calculating the revenue of a subscription
        /// </summary>
        /// <param name="chargify">The ChargifyConnect object to use</param>
        /// <param name="subscription">The subscription from which to calculate the revenue</param>
        /// <returns>The decimal value of revenue.</returns>
        public static decimal CalculateSubscriptionRevenue(this ChargifyConnect chargify, ISubscription subscription)
        {
            decimal retValue = 0;
            List<TransactionType> refundKinds = new List<TransactionType>
            {
                TransactionType.Payment,
                TransactionType.Refund
            };
            IDictionary<int, ITransaction> transactions = chargify.GetTransactionsForSubscription(subscription.SubscriptionID, refundKinds);
            if ((transactions != null) && (transactions.Count > 0))
            {
                decimal total = 0;
                var paymentTransactions = from t in transactions.Values
                                          where t.Type == TransactionType.Payment
                                          select t;
                foreach (ITransaction paymentTransaction in paymentTransactions)
                {
                    if (paymentTransaction.Success)
                    {
                        total += paymentTransaction.Amount;
                    }
                }

                var refundTransactions = from t1 in transactions.Values
                                         where t1.Type == TransactionType.Refund
                                         select t1;
                foreach (ITransaction refundTransaction in refundTransactions)
                {
                    total -= refundTransaction.Amount;
                }

                retValue = total;
            }
            return retValue;
        }

        #endregion Useful
    }
}