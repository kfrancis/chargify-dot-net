
#region License, Terms and Conditions
//
// ReferralCode.cs
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
    /// Object representing coupon in Chargify
    /// </summary>
    public class ReferralCode : ChargifyBase, IReferralCode, IComparable<ReferralCode>
    {
        #region Field Keys
        private const string IDKey = "id";
        private const string SiteIDKey= "site-id";
        private const string SubscriptionIDKey = "subscription-id";
        private const string CodeKey = "code";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public ReferralCode()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ReferralCodeXML">XML containing referral code info (in expected format)</param>
        public ReferralCode(string ReferralCodeXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(ReferralCodeXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "ReferralCodeXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == "referral-code")
                {
                    this.LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain coupon information", "ReferralCodeXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="couponNode">XML containing coupon info (in expected format)</param>
        internal ReferralCode(XmlNode couponNode)
            : base()
        {
            if (couponNode == null) throw new ArgumentNullException("ReferralCodeNode");
            if (couponNode.Name != "referral-code") throw new ArgumentException("Not a vaild coupon node", "referral-code");
            if (couponNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "referral-code");
            this.LoadFromNode(couponNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="couponObject">JsonObject containing coupon info (in expected format)</param>
        public ReferralCode(JsonObject couponObject)
            : base()
        {
            if (couponObject == null) throw new ArgumentNullException("referralCodeObject");
            if (couponObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild coupon object", "referralCodeObject");
            this.LoadFromJSON(couponObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing referral code data</param>
        private void LoadFromJSON(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get referral code info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IDKey:
                        _iD = obj.GetJSONContentAsInt(key);
                        break;
                    case SiteIDKey:
                        _siteID = obj.GetJSONContentAsInt(key);
                        break;
                    case SubscriptionIDKey:
                        _subscriptionID = obj.GetJSONContentAsInt(key);
                        break;
                    case CodeKey:
                        _code = obj.GetJSONContentAsString(key);
                        break; 
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a coupon node
        /// </summary>
        /// <param name="couponNode">The coupon node</param>
        private void LoadFromNode(XmlNode couponNode)
        {
            foreach (XmlNode dataNode in couponNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case IDKey:
                        _iD = dataNode.GetNodeContentAsInt();
                        break;
                    case SiteIDKey:
                        _siteID = dataNode.GetNodeContentAsInt();
                        break;
                    case SubscriptionIDKey:
                        _subscriptionID = dataNode.GetNodeContentAsInt();
                        break;
                    case CodeKey:
                        _code = dataNode.GetNodeContentAsString();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region IReferralCode Members

        /// <summary>
        /// The referral id
        /// </summary>
        public int ID
        {
            get { return _iD; }
            set { _iD = value; }
        }
        private int _iD = int.MinValue;

        /// <summary>
        /// The site id
        /// </summary>
        public int SiteID
        {
            get { return _siteID; }
            set { _siteID = value; }
        }
        private int _siteID = int.MinValue;

        /// <summary>
        /// The subscription id
        /// </summary>
        public int SubscriptionID
        {
            get { return _subscriptionID; }
            set { _subscriptionID = value; }
        }
        private int _subscriptionID = int.MinValue;

        /// <summary>
        /// The string code that represents this coupon
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        private string _code = string.Empty;

        #endregion

        #region IComparable<IReferralCode> Members

        /// <summary>
        /// Method for comparing one coupon to another
        /// </summary>
        public int CompareTo(IReferralCode other)
        {
            return this.Code.CompareTo(other.Code);
        }

        #endregion

        #region IComparable<Coupon> Members

        /// <summary>
        /// Method for comparing one coupon to another
        /// </summary>
        public int CompareTo(ReferralCode other)
        {
            return this.Code.CompareTo(other.Code);
        }
        #endregion
    }
}