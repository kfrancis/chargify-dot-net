namespace ChargifyNET
{
    #region Imports
    using System.IO;
    using System.Web;
    using ChargifyNET.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    #endregion

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
        /// <returns>The Html-encoded string (using the HttpUtility.HtmlEncode method)</returns>
        public static string ToHtmlEncoded(this string input)
        {
            return HttpUtility.HtmlEncode(input);
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
            string possibleData = string.Empty;
            using (StreamReader sr = new StreamReader(requestStream))
            {
                requestStream.Position = 0;
                possibleData = sr.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(possibleData))
            {
                var calculatedSignature = UsefulExtensions.CalculateHMAC256Signature(possibleData, sharedKey);
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
            string hexaHash = "";
            foreach (byte b in hash) { hexaHash += String.Format("{0:x2}", b); }

            // I'm not sure if it could be longer, so just compare the same number as characters.
            return (signature == hexaHash.Substring(0, signature.Length)) ? true : false;
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
                if (value.StartsWith("["))
                {
                    int position = 0;
                    Json.JsonArray array = Json.JsonArray.Parse(value, ref position);
                }
                else
                {
                    Json.JsonObject obj = Json.JsonObject.Parse(value);
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
            return !string.IsNullOrEmpty(value) ? _guidRegex.IsMatch(value) : false;
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
            string result = string.Empty;
            result = XmlToJsonConverter.XmlToJSON(doc);
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
        #endregion

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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
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
                if (!decimal.TryParse(node.FirstChild.Value, out result)) result = 0;
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
                    {
                        if (decimal.TryParse(str.Value, out result))
                        {
                            return result;
                        }
                    }

                    JsonNumber value = obj[key] as JsonNumber;
                    if (value != null)
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
                int result;
                if (int.TryParse(node.FirstChild.Value, out result))
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
                    {
                        result = str.Value;
                    }
                }
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
                if (!int.TryParse(node.FirstChild.Value, out result)) result = 0;
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
                    JsonNumber value = obj[key] as JsonNumber;
                    if (value != null)
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
        public static int GetJSONContentAsInt(this JsonObject obj, string key)
        {
            int result = 0;
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    JsonNumber value = obj[key] as JsonNumber;
                    if (value != null)
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
                    JsonObject familyObj = obj[key] as JsonObject;
                    if (familyObj != null)
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
                    JsonObject customerObj = obj[key] as JsonObject;
                    if (customerObj != null)
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
                    JsonObject customerObj = obj[key] as JsonObject;
                    if (customerObj != null)
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
                    JsonBoolean objVal = obj[key] as JsonBoolean;
                    if (objVal != null)
                    {
                        result = objVal.Value;
                    }
                }
            }
            return result;
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
                    {
                        string dateStr = str.Value;
                        if (!DateTime.TryParse(dateStr, out result)) result = DateTime.MinValue;
                    }
                }
            }

            return result;
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
                    {
                        result = (PaymentCollectionMethod)Enum.Parse(typeof(PaymentCollectionMethod), str.Value, true);
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
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
                result = (PaymentCollectionMethod)Enum.Parse(typeof(PaymentCollectionMethod), node.FirstChild.Value, true);
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
            T result = default(T);
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
            T result = default(T);
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
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
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
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
                    JsonObject chargeObj = obj[key] as JsonObject;
                    if (chargeObj != null)
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
                    JsonObject publicSignupPageObj = obj[key] as JsonObject;
                    if (publicSignupPageObj != null)
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
                    JsonObject transactionObj = obj[key] as JsonObject;
                    if (transactionObj != null)
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
            if (obj != null)
            {
                if (obj.ContainsKey(key))
                {
                    JsonString str = obj[key] as JsonString;
                    if (str != null)
                    {
                        result = (IntervalUnit)Enum.Parse(typeof(IntervalUnit), str.Value, true);
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
                result = (IntervalUnit)Enum.Parse(typeof(IntervalUnit), node.FirstChild.Value, true);
            }
            return result;
        }

        #endregion

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
            Assembly a = null;
            try
            {
                a = Assembly.Load("Microsoft.WindowsAzure.ServiceRuntime, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                if (a != null)
                {
                    Type classType = a.GetType("Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment");
                    PropertyInfo pi = classType.GetProperty("IsAvailable");
                    // This is the same as calling RoleEnvironment.IsAvailable, but without requiring it be referenced.
                    result = (bool)pi.GetValue(null, null); // This should get the result of IsAvailable
                }
            }
            catch (Exception) { }
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
            catch (Exception) { }
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
                Type[] argTypes = new Type[] { typeof(System.String) };
                ConstructorInfo cInfo = typeof(T).GetConstructor(argTypes);
                return (T)cInfo.Invoke(new object[] { serverResponse });
            }
            else if (serverResponse.IsJSON())
            {
                // now build the jsonObject object based on response as JSON
                JsonObject obj = JsonObject.Parse(serverResponse);
                if (!string.IsNullOrEmpty(key))
                {
                    if (obj.ContainsKey(key) && obj.Keys.Count == 1)
                    {
                        Type[] argTypes = new Type[] { typeof(JsonObject) };
                        ConstructorInfo cInfo = typeof(T).GetConstructor(argTypes);
                        return (T)cInfo.Invoke(new object[] { obj[key] as JsonObject });
                    }
                    else
                    {
                        Type[] argTypes = new Type[] { typeof(JsonObject) };
                        ConstructorInfo cInfo = typeof(T).GetConstructor(argTypes);
                        return (T)cInfo.Invoke(new object[] { obj });
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
                default:
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
                    else if (ID is int) customer = chargify.LoadCustomer((int)ID);
                    result = (T)customer;
                    break;
                case "subscription":
                    if (!typeof(ISubscription).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    ISubscription subscription = null;
                    // load the object depending on what the ID can be interpreted as, 
                    // in this case it's only SubscriptionID (int)
                    if (ID is int) subscription = chargify.LoadSubscription((int)ID);
                    result = (T)subscription;
                    break;
                case "product":
                    if (!typeof(IProduct).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    IProduct product = null;
                    // load the object depending on what the ID can be interpreted as, 
                    // in this case it's either the ProductID (int) or the ProductHandle (string)
                    if (ID is string) product = chargify.LoadProduct(ID as string, true);
                    else if (ID is int) product = chargify.LoadProduct(((int)ID).ToString(), false);
                    result = (T)product;
                    break;
                case "statement":
                    if (!typeof(IStatement).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    IStatement statement = null;
                    if (ID is int) statement = chargify.LoadStatement((int)ID);
                    result = (T)statement;
                    break;
                case "transaction":
                    if (!typeof(ITransaction).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    ITransaction transaction = null;
                    if (ID is int) transaction = chargify.LoadTransaction((int)ID);
                    result = (T)transaction;
                    break;
                case "coupon":
                    if (!typeof(ICoupon).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    ICoupon coupon = null;
                    if (ID is int && parentID.HasValue && parentID.Value > 0) coupon = chargify.LoadCoupon(parentID.Value, (int)ID);
                    result = (T)coupon;
                    break;
                case "productfamily":
                    if (!typeof(IProductFamily).IsAssignableFrom(typeof(T))) throw new ArgumentException();
                    IProductFamily productFamily = null;
                    if (ID is int) productFamily = chargify.LoadProductFamily((int)ID);
                    result = (T)productFamily;
                    break;
                default:
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
            List<TransactionType> refundKinds = new List<TransactionType>();
            refundKinds.Add(TransactionType.Payment);
            refundKinds.Add(TransactionType.Refund);
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
        #endregion
    }
}
