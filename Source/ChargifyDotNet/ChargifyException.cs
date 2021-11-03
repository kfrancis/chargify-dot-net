
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

namespace ChargifyNET
{
    #region Imports
    using System;
    using System.Net;
    using System.Diagnostics;
    using System.Xml;
    using System.Collections.Generic;
    using System.IO;
    using Json;
    using System.Xml.Linq;
    using System.Linq;
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

        /// <summary>
        /// Method to parse the errors returned from Chargify after an unsuccessful interaction
        /// </summary>
        /// <param name="response">The HttpWebResponse instance which contains the error response</param>
        /// <returns>The list of errors returned from Chargify</returns>
        internal static List<ChargifyError> ParseChargifyErrors(HttpWebResponse response)
        {
            List<ChargifyError> errors = new();
            if (response != null)
            {
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
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
                            var results = from e in xdoc.Descendants("errors")
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
                        if (obj.ContainsKey("errors"))
                        {
                            if (obj["errors"] is JsonArray array && array.Length > 0)
                            {
                                for (var i = 0; i <= array.Length - 1; i++)
                                {
                                    if ((((JsonString)array.Items[i]) != null) && (!string.IsNullOrEmpty(((JsonString)array.Items[i]).Value)))
                                    {
                                        var errorStr = array.Items[i] as JsonString;
                                        ChargifyError anError = new(errorStr);
                                        if (!errors.Contains(anError))
                                        {
                                            errors.Add(anError);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return errors;
            }
            throw new ChargifyNetException("Unknown Error");
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
        /// Constructor
        /// </summary>
        /// <param name="errorResponse">The response that caused the exception</param>
        /// <param name="wex">The original web exception.  This becomes the inner exception of ths exception</param>
        public ChargifyException(HttpWebResponse errorResponse, WebException wex) :
            base($"The server returned '{errorResponse.StatusDescription}' with the status code {errorResponse.StatusCode} ({errorResponse.StatusCode:d}).", wex)
        {
            StatusDescription = errorResponse.StatusDescription;
            StatusCode = errorResponse.StatusCode;
            // if there are any errors, parse them for user consumption
            ErrorMessages = ChargifyError.ParseChargifyErrors(errorResponse);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorResponse">The response that caused the exception</param>
        /// <param name="wex">The original web exception.  This becomes the inner exception of ths exception</param>
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
        /// Get the status description
        /// </summary>
        public string StatusDescription { get; private set; }

        /// <summary>
        /// Get the status code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Get the last data posted that potentially caused the exception
        /// </summary>
        public string LastDataPosted { get; private set; } = string.Empty;

        /// <summary>
        /// List of ChargifyErrors returned from Chargify.
        /// </summary>
        public List<ChargifyError> ErrorMessages { get; private set; }

        ///// <summary>
        ///// Get object data
        ///// </summary>
        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);
        //}

        /// <summary>
        /// Pretty output for this exception
        /// </summary>
        /// <returns>Request, response and errors</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(LastDataPosted))
            {
                if (LastDataPosted.Contains("<full_number>"))
                {
                    var xdoc = XDocument.Parse(LastDataPosted);
                    var fullNumberElement = xdoc.Element("subscription")?.Element("credit_card_attributes")?.Element("full_number");
                    if (fullNumberElement != null)
                    {
                        fullNumberElement.Value = fullNumberElement.Value.Mask('X', 4);
                        LastDataPosted = xdoc.ToString();
                    }
                }
            }
            // Used for the LogResponse Action
            var retVal = string.Empty;
            retVal += string.Format("Request: {0}\n", LastDataPosted);
            retVal += string.Format("Response: {0} {1}\n", StatusCode, StatusDescription);
            retVal += string.Format("Errors: {0}\n", string.Join(", ", ErrorMessages.ToList().Select(e => e.Message).ToArray()));
            return retVal;
        }
    }
}
