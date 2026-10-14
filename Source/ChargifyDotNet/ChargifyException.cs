
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
        public string Message { get; private set; } = string.Empty;

        /// <summary>
        /// The field the error relates to, when Chargify returned field-keyed errors
        /// (e.g. <c>{"errors":{"amount":["must be greater than 0"]}}</c>). Null otherwise.
        /// </summary>
        public string? Field { get; private set; }

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
            Message = errorNode.Value ?? string.Empty;
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

        /// <summary>
        /// Parses the error messages out of a Chargify error response body.
        /// </summary>
        /// <remarks>
        /// Supports <c>&lt;errors&gt;&lt;error&gt;msg&lt;/error&gt;&lt;/errors&gt;</c>,
        /// <c>{"errors":["msg"]}</c> and <c>{"errors":{"field":["msg"]}}</c>.
        /// Unknown or unparseable content yields an empty list; this method never throws.
        /// </remarks>
        /// <param name="content">The raw response body</param>
        /// <returns>The list of errors, possibly empty</returns>
        internal static List<ChargifyError> ParseErrorsFromContent(string? content)
        {
            var errors = new List<ChargifyError>();
            if (content == null || string.IsNullOrWhiteSpace(content)) return errors;

            try
            {
                var trimmed = content.Trim();
                if (trimmed.StartsWith("<", StringComparison.Ordinal))
                {
                    ParseXmlErrors(trimmed, errors);
                }
                else if (trimmed.StartsWith("{", StringComparison.Ordinal) || trimmed.StartsWith("[", StringComparison.Ordinal))
                {
                    ParseJsonErrors(trimmed, errors);
                }
            }
            catch
            {
                // Never throw from error parsing; the raw body is still available on the exception.
                errors.Clear();
            }

            return errors;
        }

        private static void ParseXmlErrors(string content, List<ChargifyError> errors)
        {
            var xdoc = XDocument.Parse(content);
            var errorElements = xdoc.Descendants("error").ToList();
            if (errorElements.Count == 0)
            {
                // e.g. <errors>Some message</errors>
                errorElements = xdoc.Descendants("errors").Where(e => !e.HasElements).ToList();
            }

            foreach (var element in errorElements)
            {
                AddError(errors, element.Value, null);
            }
        }

        private static void ParseJsonErrors(string content, List<ChargifyError> errors)
        {
            var token = Newtonsoft.Json.Linq.JToken.Parse(content);
            var errorsToken = token is Newtonsoft.Json.Linq.JObject obj ? obj["errors"] : token;
            AddJsonErrors(errors, errorsToken, null);
        }

        private static void AddJsonErrors(List<ChargifyError> errors, Newtonsoft.Json.Linq.JToken? token, string? field)
        {
            switch (token)
            {
                case null:
                    return;
                case Newtonsoft.Json.Linq.JArray array:
                    foreach (var item in array) AddJsonErrors(errors, item, field);
                    return;
                case Newtonsoft.Json.Linq.JObject obj:
                    foreach (var property in obj.Properties()) AddJsonErrors(errors, property.Value, property.Name);
                    return;
                case Newtonsoft.Json.Linq.JValue value when value.Type != Newtonsoft.Json.Linq.JTokenType.Null:
                    AddError(errors, Convert.ToString(value.Value, System.Globalization.CultureInfo.InvariantCulture), field);
                    return;
            }
        }

        private static void AddError(List<ChargifyError> errors, string? message, string? field)
        {
            if (message == null || string.IsNullOrWhiteSpace(message)) return;
            var fullMessage = string.IsNullOrEmpty(field) || string.Equals(field, "base", StringComparison.OrdinalIgnoreCase)
                ? message
                : $"{field}: {message}";
            if (errors.Any(e => e.Message == fullMessage)) return;
            errors.Add(new ChargifyError(fullMessage) { Field = field });
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
            return ParseErrorsFromContent(reader.ReadToEnd());
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

        /// <summary>
        /// Constructor
        /// </summary>
        public ChargifyNetException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The message of this exception</param>
        /// <param name="innerException">The exception that caused this exception</param>
        public ChargifyNetException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown by the chargify system when a request fails
    /// </summary>
    [DebuggerDisplay("Status Description: {StatusDescription}, Status Code: {StatusCode}, Error Messages: {ErrorMessages.Count}")]
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors",
        Justification = "Every ChargifyException carries an HTTP status code; message-only constructors would leave StatusCode meaningless.")]
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
            LastDataPosted = SensitiveDataMasker.Mask(postData);
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
            LastDataPosted = SensitiveDataMasker.Mask(postData);
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
            LastDataPosted = SensitiveDataMasker.Mask(postData);
            // Parse errors from response content if available
            ErrorMessages = ChargifyError.ParseChargifyErrors(response);
        }

        /// <summary>
        /// Constructor for HttpClient usage where the response body has already been read.
        /// </summary>
        /// <param name="response">The HTTP response message (status, reason and headers are taken from it; its content is not read)</param>
        /// <param name="responseBody">The already-read response body</param>
        /// <param name="innerException">The original exception that caused this exception</param>
        /// <param name="postData">The data posted that could have potentially caused the exception.</param>
        public ChargifyException(HttpResponseMessage response, string? responseBody, Exception innerException, string? postData) :
            base($"The server returned '{response.ReasonPhrase}' with the status code {response.StatusCode} ({response.StatusCode:d}) to the last request.", innerException)
        {
            StatusDescription = response.ReasonPhrase ?? response.StatusCode.ToString();
            StatusCode = response.StatusCode;
            LastDataPosted = SensitiveDataMasker.Mask(postData);
            ResponseBody = Truncate(SensitiveDataMasker.Mask(responseBody));
            ErrorMessages = ChargifyError.ParseErrorsFromContent(responseBody);

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var rateLimit = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> allHeaders = response.Headers;
            if (response.Content != null) allHeaders = allHeaders.Concat(response.Content.Headers);
            foreach (var header in allHeaders)
            {
                // Never keep cookies around on an exception that is likely to be logged.
                if (string.Equals(header.Key, "Set-Cookie", StringComparison.OrdinalIgnoreCase)) continue;
                var value = string.Join(", ", header.Value);
                headers[header.Key] = value;
                if (header.Key.StartsWith("X-RateLimit-", StringComparison.OrdinalIgnoreCase)) rateLimit[header.Key] = value;
            }
            ResponseHeaders = headers;
            RateLimitHeaders = rateLimit;

            var retryAfter = response.Headers.RetryAfter;
            if (retryAfter?.Delta != null)
            {
                RetryAfter = retryAfter.Delta;
            }
            else if (retryAfter?.Date != null)
            {
                var delta = retryAfter.Date.Value - DateTimeOffset.UtcNow;
                RetryAfter = delta < TimeSpan.Zero ? TimeSpan.Zero : delta;
            }
        }

        /// <summary>
        /// The maximum number of characters of the response body kept on <see cref="ResponseBody"/>.
        /// </summary>
        public const int MaxResponseBodyLength = 4096;

        private static string? Truncate(string? value)
        {
            if (value == null || value.Length <= MaxResponseBodyLength) return value;
            return value.Substring(0, MaxResponseBodyLength) + "...[truncated]";
        }

        /// <summary>
        /// The raw response body returned by Chargify (card numbers masked, truncated to
        /// <see cref="MaxResponseBodyLength"/> characters). Null if no response was received.
        /// </summary>
        public string? ResponseBody { get; }

        /// <summary>
        /// The response headers returned with the failed response (Set-Cookie excluded). Empty if no response was received.
        /// </summary>
        public IReadOnlyDictionary<string, string> ResponseHeaders { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Any <c>X-RateLimit-*</c> headers returned with the failed response. Empty if none.
        /// </summary>
        public IReadOnlyDictionary<string, string> RateLimitHeaders { get; } = new Dictionary<string, string>();

        /// <summary>
        /// The value of the <c>Retry-After</c> header (typically sent with 429 or 503), if present.
        /// </summary>
        public TimeSpan? RetryAfter { get; }

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
        /// The exception message, followed by any error messages Chargify returned so that
        /// test runners and loggers that only print <see cref="Exception.Message"/> show the reason.
        /// </summary>
        public override string Message
        {
            get
            {
                if (ErrorMessages == null || ErrorMessages.Count == 0) return base.Message;
                return $"{base.Message} Errors: {string.Join("; ", ErrorMessages.Select(e => e.Message).ToArray())}";
            }
        }

        /// <summary>
        /// Pretty output for this exception
        /// </summary>
        /// <returns>Request, response and errors</returns>
        public override string ToString()
        {
            var sanitizedPostData = SensitiveDataMasker.Mask(LastDataPosted);

            var retVal = string.Empty;
            retVal += $"Request: {sanitizedPostData}\n";
            retVal += $"Response: {StatusCode} {StatusDescription}\n";
            retVal += $"Errors: {string.Join(", ", ErrorMessages.Select(e => e.Message).ToArray())}\n";
            if (RetryAfter.HasValue) retVal += $"Retry-After: {RetryAfter.Value}\n";
            if (ResponseBody != null) retVal += $"Response Body: {SensitiveDataMasker.Mask(ResponseBody)}\n";
            return retVal;
        }
    }
}
