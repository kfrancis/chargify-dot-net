
#region License, Terms and Conditions
//
// PublicSignupPage.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2015 Clinical Support Systems, Inc. All rights reserved.
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
    using System.Diagnostics;
    using System.Xml;
    using Json;
    #endregion

    /// <summary>
    /// Public Pages (formerly known as Hosted Pages) are highly customizable white label pages
    /// that you can use as the public-facing side of your subscription business.
    /// </summary>
    [DebuggerDisplay("ID: {ID}, URL: {URL}")]
    public class PublicSignupPage : ChargifyBase, IPublicSignupPage, IComparable<PublicSignupPage>
    {
        #region Field Keys
        private const string IdKey = "id";
        private const string UrlKey = "url";
        private const string ReturnUrlKey = "return_url";
        private const string ReturnParamsKey = "return_params";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private PublicSignupPage()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publicSignupPageXml">An XML string containing a node</param>
        public PublicSignupPage(string publicSignupPageXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(publicSignupPageXml);
            if (doc.ChildNodes.Count == 0)
                throw new ArgumentException("XML not valid", nameof(publicSignupPageXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "public_signup_page")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain information", nameof(publicSignupPageXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publicSignupPageNode">An xml node with data</param>
        internal PublicSignupPage(XmlNode publicSignupPageNode)
        {
            if (publicSignupPageNode == null)
                throw new ArgumentNullException(nameof(publicSignupPageNode));
            if (publicSignupPageNode.Name != "public_signup_page")
                throw new ArgumentException("Not a vaild xml node", nameof(publicSignupPageNode));
            if (publicSignupPageNode.ChildNodes.Count == 0)
                throw new ArgumentException("XML not valid", nameof(publicSignupPageNode));

            LoadFromNode(publicSignupPageNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publicSignupPageJson">JsonObject containing json info (in expected format)</param>
        public PublicSignupPage(JsonObject publicSignupPageJson)
        {
            if (publicSignupPageJson == null)
                throw new ArgumentNullException(nameof(publicSignupPageJson));
            if (publicSignupPageJson.Keys.Count <= 0)
                throw new ArgumentException("Not a vaild json object", nameof(publicSignupPageJson));

            LoadFromJson(publicSignupPageJson);
        }

        private void LoadFromNode(XmlNode node)
        {
            foreach (XmlNode dataNode in node.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case IdKey:
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case UrlKey:
                        _url = dataNode.GetNodeContentAsString();
                        break;
                    case ReturnUrlKey:
                        _returnUrl = dataNode.GetNodeContentAsString();
                        break;
                    case ReturnParamsKey:
                        _returnParams = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }

        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IdKey:
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case UrlKey:
                        _url = obj.GetJSONContentAsString(key);
                        break;
                    case ReturnUrlKey:
                        _returnUrl = obj.GetJSONContentAsString(key);
                        break;
                    case ReturnParamsKey:
                        _returnParams = obj.GetJSONContentAsString(key);
                        break;
                }
            }
        }
        #endregion

        #region IPublicSignupPage Members

        /// <summary>
        /// The ID of the public signup page
        /// </summary>
        public int ID { get { return _id; } }
        private int _id = int.MinValue;

        /// <summary>
        /// The URL to the public signup page
        /// </summary>
        public string URL { get { return _url; } }
        private string _url = string.Empty;

        /// <summary>
        /// The url to which a customer will be returned after a successful signup
        /// </summary>
        public string ReturnURL { get { return _returnUrl; } }
        private string _returnUrl = string.Empty;

        /// <summary>
        /// The params to be appended to the return_url
        /// </summary>
        public string ReturnParams { get { return _returnParams; } }
        private string _returnParams = string.Empty;
        #endregion

        #region IComparible<IPublicSignupPage> Members
        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="other">The object to compare to</param>
        /// <returns></returns>
        public int CompareTo(IPublicSignupPage other)
        {
            return ID.CompareTo(other.ID);
        }
        #endregion

        #region IComparible<PubicSignupPage> Members
        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="other">The object to compare to</param>
        /// <returns></returns>
        public int CompareTo(PublicSignupPage other)
        {
            return ID.CompareTo(other.ID);
        }
        #endregion
    }
}