
#region License, Terms and Conditions
//
// BillingManagementInfo.cs
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
    using System.Xml;
    using Json;
    #endregion

    /// <summary>
    /// From http://docs.chargify.com/api-billing-portal
    /// </summary>
    public class BillingManagementInfo : ChargifyBase, IBillingManagementInfo, IComparable<BillingManagementInfo>
    {
        #region Field Keys
        private const string CreatedAtKey = "created_at";
        private const string UrlKey = "url";
        private const string FetchCountKey = "fetch_count";
        private const string NewLinkAvailableAtKey = "new_link_available_at";
        private const string ExpiresAtKey = "expires_at";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private BillingManagementInfo()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="billingManagementInfoXml">An XML string containing a billingManagementInfo node</param>
        public BillingManagementInfo(string billingManagementInfoXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(billingManagementInfoXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(billingManagementInfoXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "management_link")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain billing management information", nameof(billingManagementInfoXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="billingManagementInfoNode">An xml node with component information</param>
        internal BillingManagementInfo(XmlNode billingManagementInfoNode)
        {
            if (billingManagementInfoNode == null) throw new ArgumentNullException(nameof(billingManagementInfoNode));
            if (billingManagementInfoNode.Name != "management_link") throw new ArgumentException("Not a vaild billing management node", nameof(billingManagementInfoNode));
            if (billingManagementInfoNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(billingManagementInfoNode));
            LoadFromNode(billingManagementInfoNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="billingManagementInfoObject">An JsonObject with billing management information</param>
        public BillingManagementInfo(JsonObject billingManagementInfoObject)
        {
            if (billingManagementInfoObject == null) throw new ArgumentNullException(nameof(billingManagementInfoObject));
            if (billingManagementInfoObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild management object", nameof(billingManagementInfoObject));
            LoadFromJson(billingManagementInfoObject);
        }

        private void LoadFromJson(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get component info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case CreatedAtKey:
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case UrlKey:
                        _url = obj.GetJSONContentAsString(key);
                        break;
                    case NewLinkAvailableAtKey:
                        _newLinkAvailableAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case FetchCountKey:
                        _fetchCount = obj.GetJSONContentAsInt(key);
                        break;
                    case ExpiresAtKey:
                        _expiresAt = obj.GetJSONContentAsDateTime(key);
                        break;
                }
            }
        }

        private void LoadFromNode(XmlNode obj)
        {
            // loop through the nodes to get component info
            foreach (XmlNode dataNode in obj.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case CreatedAtKey:
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case UrlKey:
                        _url = dataNode.GetNodeContentAsString();
                        break;
                    case FetchCountKey:
                        _fetchCount = dataNode.GetNodeContentAsInt();
                        break;
                    case ExpiresAtKey:
                        _expiresAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case NewLinkAvailableAtKey:
                        _newLinkAvailableAt = dataNode.GetNodeContentAsDateTime();
                        break;
                }
            }
        }
        #endregion

        #region IBillingManagementInfo Members

        /// <summary>
        /// The customer's management URL
        /// </summary>
        public string URL
        {
            get { return _url; }
        }
        private string _url = string.Empty;

        /// <summary>
        /// Number of times this link has been retrieved (at 15 you will be blocked)
        /// </summary>
        public int FetchCount
        {
            get { return _fetchCount; }
        }
        private int _fetchCount = int.MinValue;

        /// <summary>
        /// When this link was created
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// When a new link will be available and fetch_count is reset (15 days from when it was created)
        /// </summary>
        public DateTime NewLinkAvailableAt
        {
            get { return _newLinkAvailableAt; }
        }
        private DateTime _newLinkAvailableAt = DateTime.MinValue;

        /// <summary>
        /// When this link expires (65 days from when it was created)
        /// </summary>
        public DateTime ExpiresAt
        {
            get { return _expiresAt; }
        }
        private DateTime _expiresAt = DateTime.MinValue;

        #endregion

        #region ICompare Members
        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(BillingManagementInfo other)
        {
            // compare
            return string.Compare(URL, other.URL, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IBillingManagementInfo other)
        {
            return string.Compare(URL, other.URL, StringComparison.CurrentCultureIgnoreCase);
        }
        #endregion
    }
}
