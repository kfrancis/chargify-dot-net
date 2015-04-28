
#region License, Terms and Conditions
//
// Credit.cs
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using System.Xml;
    using ChargifyNET.Json;
    #endregion

    /// <summary>
    /// One-time (or one-off) credit for arbitrary amount
    /// </summary>
    public class Credit : ChargifyBase, ICredit, IComparable<Credit>
    {
        #region Field Keys
        private const string SuccessKey = "success";
        private const string MemoKey = "memo";
        private const string AmountInCentsKey = "amount_in_cents";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Credit()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CreditXML">XML containing credit info (in expected format)</param>
        public Credit(string CreditXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(CreditXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "CreditXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == "credit")
                {
                    this.LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain credit information", "CreditXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creditNode">XML containing credit info (in expected format)</param>
        internal Credit(XmlNode creditNode)
            : base()
        {
            if (creditNode == null) throw new ArgumentNullException("CreditNode");
            if (creditNode.Name != "credit") throw new ArgumentException("Not a vaild credit node", "creditNode");
            if (creditNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "creditNode");
            this.LoadFromNode(creditNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creditObject">JsonObject containing credit info (in expected format)</param>
        public Credit(JsonObject creditObject)
        {
            if (creditObject == null) throw new ArgumentNullException("creditObject");
            if (creditObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild credit object", "creditObject");
            this.LoadFromJSON(creditObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject with credit data</param>
        private void LoadFromJSON(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case SuccessKey:
                        _success = obj.GetJSONContentAsBoolean(key);
                        break;
                    case MemoKey:
                        _memo = obj.GetJSONContentAsString(key);
                        break;
                    case AmountInCentsKey:
                        _amountInCents = obj.GetJSONContentAsInt(key);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a credit node
        /// </summary>
        /// <param name="creditNode">The credit node</param>
        private void LoadFromNode(XmlNode creditNode)
        {
            foreach (XmlNode dataNode in creditNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case SuccessKey:
                        _success = dataNode.GetNodeContentAsBoolean();
                        break;
                    case MemoKey:
                        _memo = dataNode.GetNodeContentAsString();
                        break;
                    case AmountInCentsKey:
                        _amountInCents = dataNode.GetNodeContentAsInt();
                        break;
                    default:
                        break;

                }
            }
        }
        #endregion

        #region ICredit Members

        /// <summary>
        /// Either true or false, depending on the success of the credit
        /// </summary>
        public bool Success
        {
            get { return _success; }
        }
        private bool _success = false;

        /// <summary>
        /// Get the amount, in cents
        /// </summary>
        public int AmountInCents
        {
            get { return _amountInCents; }
        }
        private int _amountInCents = int.MinValue;

        /// <summary>
        /// Get the amount, in dollars and cents
        /// </summary>
        public decimal Amount
        {
            get { return Convert.ToDecimal(this._amountInCents) / 100; }
        }

        /// <summary>
        /// The memo for the created credit
        /// </summary>
        public string Memo
        {
            get { return _memo; }
        }
        private string _memo = string.Empty;


        #endregion

        #region IComparable<ICredit> Members

        /// <summary>
        /// Compare this credit to another
        /// </summary>
        public int CompareTo(ICredit other)
        {
            return this.AmountInCents.CompareTo(other.AmountInCents);
        }

        #endregion

        #region IComparable<Credit> Members

        /// <summary>
        /// Compare this credit to another
        /// </summary>
        public int CompareTo(Credit other)
        {
            return this.AmountInCents.CompareTo(other.AmountInCents);
        }

        #endregion
    }
}
