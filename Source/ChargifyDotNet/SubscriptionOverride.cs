﻿
#region License, Terms and Conditions
//
// SubscriptionOverride.cs
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
    /// A subscription override (values which cannot be set normally without specifically performing a special call)
    /// </summary>
    public class SubscriptionOverride : ChargifyBase, ISubscriptionOverride
    {
        #region Field Keys
        private const string ActivatedAtKey = "activated_at";
        private const string CancellationMessageKey = "cancellation_message";
        private const string CanceledAtKey = "canceled_at";
        private const string ExpiresAtKey = "expires_at";
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public SubscriptionOverride()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionOverrideXml">XML containing subscription override info (in expected format)</param>
        public SubscriptionOverride(string subscriptionOverrideXml)
        {
            // get the XML into an XML document
            var xdoc = new XmlDocument();
            xdoc.LoadXml(subscriptionOverrideXml);
            if (xdoc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(subscriptionOverrideXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in xdoc.ChildNodes)
            {
                if (elementNode.Name == "subscription")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no customer info was found
            throw new ArgumentException("XML does not contain subscription override information", nameof(subscriptionOverrideXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionOverrideNode">XML containing subscription override info (in expected format)</param>
        internal SubscriptionOverride(XmlNode subscriptionOverrideNode)
        {
            if (subscriptionOverrideNode == null) throw new ArgumentNullException(nameof(subscriptionOverrideNode));
            if (subscriptionOverrideNode.Name != "subscription") throw new ArgumentException("Not a vaild subscription override node", nameof(subscriptionOverrideNode));
            if (subscriptionOverrideNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(subscriptionOverrideNode));
            LoadFromNode(subscriptionOverrideNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionOverrideObject">JsonObject containing subscription override info (in expected format)</param>
        public SubscriptionOverride(JsonObject subscriptionOverrideObject)
        {
            if (subscriptionOverrideObject == null) throw new ArgumentNullException(nameof(subscriptionOverrideObject));
            if (subscriptionOverrideObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild subscription override node", nameof(subscriptionOverrideObject));
            LoadFromJson(subscriptionOverrideObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing subscription data</param>
        private void LoadFromJson(JsonObject obj)
        {
            foreach (var key in obj.Keys)
            {
                switch (key)
                {
                    case ActivatedAtKey:
                        _activatedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case CancellationMessageKey:
                        _cancellationMessage = obj.GetJSONContentAsString(key);
                        break;
                    case CanceledAtKey:
                        _canceledAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case ExpiresAtKey:
                        _expiresAt = obj.GetJSONContentAsDateTime(key);
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a subscription node
        /// </summary>
        /// <param name="subscriptionNode">The subscription node</param>
        private void LoadFromNode(XmlNode subscriptionNode)
        {
            foreach (XmlNode dataNode in subscriptionNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case ActivatedAtKey:
                        _activatedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CanceledAtKey:
                        _canceledAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CancellationMessageKey:
                        _cancellationMessage = dataNode.GetNodeContentAsString();
                        break;
                    case ExpiresAtKey:
                        _expiresAt = dataNode.GetNodeContentAsDateTime();
                        break;
                }
            }
        }

        #endregion

        #region ISubscriptionOverride Members
        /// <summary>
        /// Can be used to record an external signup date. Chargify uses this field to record when a subscription first goes active (either at signup or at trial end)
        /// </summary>
        public DateTime ActivatedAt
        {
            get
            {
                return _activatedAt;
            }
            set
            {
                _activatedAt = value;
            }
        }
        private DateTime _activatedAt = DateTime.MinValue;

        /// <summary>
        /// Can be used to record an external cancellation date. Chargify sets this field automatically when a subscription is canceled, whether by request or via dunning.
        /// </summary>
        public DateTime CanceledAt
        {
            get
            {
                return _canceledAt;
            }
            set
            {
                _canceledAt = value;
            }
        }
        private DateTime _canceledAt = DateTime.MinValue;

        /// <summary>
        /// Can be used to record a reason for the original cancellation.
        /// </summary>
        public string CancellationMessage
        {
            get
            {
                return _cancellationMessage;
            }
            set
            {
                _cancellationMessage = value;
            }
        }
        private string _cancellationMessage = string.Empty;

        /// <summary>
        /// Can be used to record an external expiration date. Chargify sets this field automatically when a subscription expires (ceases billing) after a prescribed amount of time.
        /// </summary>
        public DateTime ExpiresAt
        {
            get
            {
                return _expiresAt;
            }
            set
            {
                _expiresAt = value;
            }
        }
        private DateTime _expiresAt = DateTime.MinValue;
        #endregion
    }
}
