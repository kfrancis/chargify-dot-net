
#region License, Terms and Conditions
//
// ChargifyException.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2010 Clinical Support Systems, Inc. All rights reserved.
// 
//  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW:
//
//  Permission is hereby granted, free of charge, to any person obtaining a
//  copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation
//  the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the
//  Software is furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//  DEALINGS IN THE SOFTWARE.
//
#endregion

namespace ChargifyDotNet
{
    #region Imports
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Xml;
    using System.Xml.Linq;
    using Json;
    #endregion

    /// <summary>
    /// Error returned from Chargify after an unsuccessful operation
    /// </summary>
    [Serializable]
    public class ChargifyError
    {
        /// <summary>
        /// The message from Chargify
        /// </summary>
        public string Message { get; private set; }

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        internal ChargifyError() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The string error message to relay to the user</param>
        internal ChargifyError(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorNode">The XML node that contains the error message</param>
        internal ChargifyError(XmlNode errorNode)
        {
            Message = errorNode.Value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorStr">The JsonString obj that contains the error message</param>
        internal ChargifyError(JsonString errorStr)
        {
            Message = errorStr.Value;
        }

        #endregion
        public static List<ChargifyError> ParseChargifyErrors(HttpResponseMessage response)
        {
            var errors = new List<ChargifyError>();

            try
            {
                if (response.Content != null)
                {
                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        // Parse the response content similar to how you parse HttpWebResponse
                        // This depends on your existing ParseChargifyErrors implementation
                        errors = ParseErrorsFromContent(responseContent);
                    }
                }
            }
            catch
            {
                // If parsing fails, return empty list
            }

            return errors;
        }

        private static List<ChargifyError> ParseErrorsFromContent(string content)
        {
            // Move your existing parsing logic here so it can be shared
            // between the HttpWebResponse and HttpResponseMessage versions
            var errors = new List<ChargifyError>();
            // ... your parsing logic
            return errors;
        }

        /// <summary>
        /// Method to parse the errors returned from Chargify after an unsuccessful interaction
        /// </summary>
        /// <param name="response">The HttpWebResponse instance which contains the error response</param>
        /// <returns>The list of errors returned from Chargify</returns>
        internal static List<ChargifyError> ParseChargifyErrors(HttpWebResponse response)
        {
            List<ChargifyError> errors = [];
            if (response == null)
            {
                throw new ChargifyNetException("Unknown Error");
            }

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                return errors;
            }

            using StreamReader reader = new(responseStream);
            var errorResponse = reader.ReadToEnd();

            // Response is frequently " " ...
            if (string.IsNullOrEmpty(errorResponse.Trim())) return errors;

            if (errorResponse.IsXml())
            {
                // New way - Linq-y
                var xdoc = XDocument.Parse(errorResponse);
                if (xdoc.Descendants("error").Any())
                {
                    var results = from e in xdoc.Descendants("error")
                        select new ChargifyError
                        {
                            Message = e.Value
                        };
                    errors = results.ToList();
                }
                else
                {
                    var results = from e in xdoc.Descendants(nameof(errors))
                        select new ChargifyError
                        {
                            Message = e.Value
                        };
                    errors = results.ToList();
                }
            }
            else if (errorResponse.IsJSON())
            {
                // slightly different json response from the usual
                var position = 0;
                var obj = JsonObject.Parse(errorResponse, ref position);
                if (!obj.ContainsKey(nameof(errors)) || obj[nameof(errors)] is not JsonArray { Length: > 0 } array)
                {
                    return errors;
                }

                for (var i = 0; i <= array.Length - 1; i++)
                {
                    if ((JsonString)array.Items[i] == null ||
                        string.IsNullOrEmpty(((JsonString)array.Items[i]).Value))
                    {
                        continue;
                    }

                    var errorStr = array.Items[i] as JsonString;
                    ChargifyError anError = new(errorStr);
                    if (!errors.Contains(anError))
                    {
                        errors.Add(anError);
                    }
                }
            }
            return errors;
        }
    }

    /// <summary>
    /// Exception thrown by Chargify.NET library when things don't add up
    /// </summary>
    [Serializable]
    public class ChargifyNetException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The message of this exception</param>
        public ChargifyNetException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Exception thrown by the chargify system when a request fails
    /// </summary>
    [DebuggerDisplay("Status Description: {StatusDescription}, Status Code: {StatusCode}, Error Messages: {ErrorMessages.Count}")]
    [Serializable]
    public class ChargifyException : Exception
    {
        /// <summary>
        /// Constructor for HttpWebResponse (legacy WebRequest usage)
        /// </summary>
        /// <param name="errorResponse">The response that caused the exception</param>
        /// <param name="wex">The original web exception.  This becomes the inner exception to this exception</param>
        public ChargifyException(HttpWebResponse errorResponse, WebException wex) :
            base($"The server returned '{errorResponse.StatusDescription}' with the status code {errorResponse.StatusCode} ({errorResponse.StatusCode:d}).", wex)
        {
            StatusDescription = errorResponse.StatusDescription;
            StatusCode = errorResponse.StatusCode;
            // if there are any errors, parse them for user consumption
            ErrorMessages = ChargifyError.ParseChargifyErrors(errorResponse);
        }

        /// <summary>
        /// Constructor for HttpWebResponse with post data (legacy WebRequest usage)
        /// </summary>
        /// <param name="errorResponse">The response that caused the exception</param>
        /// <param name="wex">The original web exception.  This becomes the inner exception to this exception</param>
        /// <param name="postData">The data posted that could have potentially caused the exception.</param>
        public ChargifyException(HttpWebResponse errorResponse, WebException wex, string postData) :
            base($"The server returned '{errorResponse.StatusDescription}' with the status code {errorResponse.StatusCode} ({errorResponse.StatusCode:d}) to the last request.", wex)
        {
            StatusDescription = errorResponse.StatusDescription;
            StatusCode = errorResponse.StatusCode;
            LastDataPosted = postData;
            // if there are any errors, parse them for user consumption
            ErrorMessages = ChargifyError.ParseChargifyErrors(errorResponse);
        }

        /// <summary>
        /// Constructor for HttpClient usage
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <param name="statusDescription">The status description</param>
        /// <param name="innerException">The original exception that caused this exception</param>
        public ChargifyException(HttpStatusCode statusCode, string statusDescription, Exception innerException) :
            base($"The server returned '{statusDescription}' with the status code {statusCode} ({statusCode:d}).", innerException)
        {
            StatusDescription = statusDescription;
            StatusCode = statusCode;
            ErrorMessages = new List<ChargifyError>();
        }

        /// <summary>
        /// Constructor for HttpClient usage with post data
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <param name="statusDescription">The status description</param>
        /// <param name="innerException">The original exception that caused this exception</param>
        /// <param name="postData">The data posted that could have potentially caused the exception.</param>
        public ChargifyException(HttpStatusCode statusCode, string statusDescription, Exception innerException, string? postData) :
            base($"The server returned '{statusDescription}' with the status code {statusCode} ({statusCode:d}) to the last request.", innerException)
        {
            StatusDescription = statusDescription;
            StatusCode = statusCode;
            LastDataPosted = postData;
            ErrorMessages = new List<ChargifyError>();
        }

        /// <summary>
        /// Constructor for HttpClient usage with HttpResponseMessage
        /// </summary>
        /// <param name="response">The HTTP response message</param>
        /// <param name="innerException">The original exception that caused this exception</param>
        public ChargifyException(HttpResponseMessage response, Exception innerException) :
            base($"The server returned '{response.ReasonPhrase}' with the status code {response.StatusCode} ({response.StatusCode:d}).", innerException)
        {
            StatusDescription = response.ReasonPhrase ?? response.StatusCode.ToString();
            StatusCode = response.StatusCode;
            // Parse errors from response content if available
            ErrorMessages = ChargifyError.ParseChargifyErrors(response);
        }

        /// <summary>
        /// Constructor for HttpClient usage with HttpResponseMessage and post data
        /// </summary>
        /// <param name="response">The HTTP response message</param>
        /// <param name="innerException">The original exception that caused this exception</param>
        /// <param name="postData">The data posted that could have potentially caused the exception.</param>
        public ChargifyException(HttpResponseMessage response, Exception innerException, string postData) :
            base($"The server returned '{response.ReasonPhrase}' with the status code {response.StatusCode} ({response.StatusCode:d}) to the last request.", innerException)
        {
            StatusDescription = response.ReasonPhrase ?? response.StatusCode.ToString();
            StatusCode = response.StatusCode;
            LastDataPosted = postData;
            // Parse errors from response content if available
            ErrorMessages = ChargifyError.ParseChargifyErrors(response);
        }

        /// <summary>
        /// Get the status description
        /// </summary>
        public string StatusDescription { get; }

        /// <summary>
        /// Get the status code
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Get the last data posted that potentially caused the exception
        /// </summary>
        public string? LastDataPosted { get; } = string.Empty;

        /// <summary>
        /// List of ChargifyErrors returned from Chargify.
        /// </summary>
        public List<ChargifyError> ErrorMessages { get; }

        /// <summary>
        /// Pretty output for this exception
        /// </summary>
        /// <returns>Request, response and errors</returns>
        public override string ToString()
        {
            var sanitizedPostData = LastDataPosted;
            if (!string.IsNullOrEmpty(sanitizedPostData) && sanitizedPostData.Contains("<full_number>"))
            {
                var xdoc = XDocument.Parse(sanitizedPostData);
                var fullNumberElement = xdoc.Element("subscription")?.Element("credit_card_attributes")?.Element("full_number");
                if (fullNumberElement != null)
                {
                    fullNumberElement.Value = fullNumberElement.Value.Mask('X', 4);
                    sanitizedPostData = xdoc.ToString();
                }
            }

            // Used for the LogResponse Action
            var retVal = string.Empty;
            retVal += $"Request: {sanitizedPostData}\n";
            retVal += $"Response: {StatusCode} {StatusDescription}\n";
            retVal += $"Errors: {string.Join(", ", ErrorMessages.Select(e => e.Message).ToArray())}\n";
            return retVal;
        }
    }
}
