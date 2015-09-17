
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
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Xml;
    using System.Collections.Generic;
    using System.IO;
    using ChargifyNET.Json;
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
            this.Message = message;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorNode">The XML node that contains the error message</param>
        internal ChargifyError(XmlNode errorNode)
        {
            this.Message = errorNode.Value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorStr">The JsonString obj that contains the error message</param>
        internal ChargifyError(JsonString errorStr)
        {
            this.Message = errorStr.Value;
        }

        #endregion

        /// <summary>
        /// Method to parse the errors returned from Chargify after an unsuccessful interaction
        /// </summary>
        /// <param name="response">The HttpWebResponse instance which contains the error response</param>
        /// <returns>The list of errors returned from Chargify</returns>
        internal static List<ChargifyError> ParseChargifyErrors(HttpWebResponse response)
        {
            if (response != null)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string errorResponse = reader.ReadToEnd();
                    List<ChargifyError> errors = new List<ChargifyError>();

                    // Response is frequently " " ...
                    if (string.IsNullOrEmpty(errorResponse.Trim())) return errors;

                    if (errorResponse.IsXml())
                    {
                        // New way - Linq-y
                        XDocument xdoc = XDocument.Parse(errorResponse);
                        if (xdoc.Descendants("error").Count() > 0)
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
                        int position = 0;
                        JsonObject obj = JsonObject.Parse(errorResponse, ref position);
                        if (obj.ContainsKey("errors"))
                        {
                            JsonArray array = obj["errors"] as JsonArray;
                            for (int i = 0; i <= array.Length - 1; i++)
                            {
                                if (((array.Items[i] as JsonString) != null) && (!string.IsNullOrEmpty((array.Items[i] as JsonString).Value)))
                                {
                                    JsonString errorStr = array.Items[i] as JsonString;
                                    ChargifyError anError = new ChargifyError(errorStr);
                                    if (!errors.Contains(anError))
                                    {
                                        errors.Add(anError);
                                    }
                                }
                            }
                        }
                    }
                    return errors;
                }
            }
            else
            {
                throw new ChargifyNetException("Unknown Error");
            }
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
            base(string.Format("The server returned '{0}' with the status code {1} ({1:d}).", errorResponse.StatusDescription, errorResponse.StatusCode), wex)
        {
            _statusDescription = errorResponse.StatusDescription;
            _statusCode = errorResponse.StatusCode;
            // if there are any errors, parse them for user consumption
            _errors = ChargifyError.ParseChargifyErrors(errorResponse);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorResponse">The response that caused the exception</param>
        /// <param name="wex">The original web exception.  This becomes the inner exception of ths exception</param>
        /// <param name="postData">The data posted that could have potentially caused the exception.</param>
        public ChargifyException(HttpWebResponse errorResponse, WebException wex, string postData) :
            base(string.Format("The server returned '{0}' with the status code {1} ({1:d}) when posting '{2}'.", errorResponse.StatusDescription, errorResponse.StatusCode, postData), wex)
        {
            _statusDescription = errorResponse.StatusDescription;
            _statusCode = errorResponse.StatusCode;
            _postData = postData;
            // if there are any errors, parse them for user consumption
            _errors = ChargifyError.ParseChargifyErrors(errorResponse);
        }

        /// <summary>
        /// Get the status description
        /// </summary>
        public string StatusDescription
        {
            get
            {
                return _statusDescription;
            }
        }
        private string _statusDescription = "";

        /// <summary>
        /// Get the status code
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                return _statusCode;
            }
        }
        private HttpStatusCode _statusCode = HttpStatusCode.Accepted;

        /// <summary>
        /// Get the last data posted that potentially caused the exception
        /// </summary>
        public string LastDataPosted
        {
            get
            {
                return _postData;
            }
        }
        private string _postData = string.Empty;

        /// <summary>
        /// List of ChargifyErrors returned from Chargify.
        /// </summary>
        public List<ChargifyError> ErrorMessages
        {
            get
            {
                return _errors;
            }
        }
        private List<ChargifyError> _errors = null;

        /// <summary>
        /// Get object data
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Pretty output for this exception
        /// </summary>
        /// <returns>Request, response and errors</returns>
        public override string ToString()
        {
            // Used for the LogResponse Action
            string retVal = string.Empty;
            retVal += string.Format("Request: {0}\n", LastDataPosted);
            retVal += string.Format("Response: {0} {1}\n", StatusCode.ToString(), StatusDescription.ToString());
            retVal += string.Format("Errors: {0}\n", string.Join(", ", _errors.ToList().Select(e => e.Message).ToArray()));
            return retVal;
        }
    }
}
