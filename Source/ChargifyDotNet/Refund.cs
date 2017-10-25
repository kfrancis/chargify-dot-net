
#region License, Terms and Conditions
//
// Refund.cs
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
    using System.Diagnostics;
    using System.Xml;
    using Json;
    #endregion

    /// <summary>
    /// A representation of a refund with details about it
    /// Info here: http://support.chargify.com/faqs/api/api-refunds
    /// </summary>
    [DebuggerDisplay("ID: {PaymentID}, Amount: {Amount}, Success: {Success}")]
    public class Refund : ChargifyBase, IRefund, IComparable<Refund>
    {
        #region Field Keys
        private const string PaymentIdKey = "id";
        private const string SuccessKey = "success";
        private const string AmountInCentsKey = "amount_in_cents";
        private const string MemoKey = "memo";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private Refund()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="refundXml">An XML string containing a refund node</param>
        public Refund(string refundXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(refundXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(refundXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "refund")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no refund info was found
            throw new ArgumentException("XML does not contain refund information", nameof(refundXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="refundNode">An xml node with refund information</param>
        internal Refund(XmlNode refundNode)
        {
            if (refundNode == null) throw new ArgumentNullException(nameof(refundNode));
            if (refundNode.Name != "refund") throw new ArgumentException("Not a vaild refund node", nameof(refundNode));
            if (refundNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(refundNode));
            LoadFromNode(refundNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="refundObject">An JsonObject with refund information</param>
        public Refund(JsonObject refundObject)
        {
            if (refundObject == null) throw new ArgumentNullException(nameof(refundObject));
            if (refundObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild refund object", nameof(refundObject));
            LoadFromJson(refundObject);
        }

        private void LoadFromJson(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get component info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case PaymentIdKey:
                        _paymentId = obj.GetJSONContentAsInt(key);
                        break;
                    case SuccessKey:
                        _success = obj.GetJSONContentAsBoolean(key);
                        break;
                    case AmountInCentsKey:
                        _amountInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case MemoKey:
                        _memo = obj.GetJSONContentAsString(key);
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
                    case PaymentIdKey:
                        _paymentId = dataNode.GetNodeContentAsInt();
                        break;
                    case SuccessKey:
                        _success = dataNode.GetNodeContentAsBoolean();
                        break;
                    case AmountInCentsKey:
                        _amountInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case MemoKey:
                        _memo = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }
        #endregion

        #region IRefund Members

        /// <summary>
        /// (Required) The ID of the Payment that the credit will be applied to
        /// </summary>
        public int PaymentID
        {
            get { return _paymentId; }
        }
        private int _paymentId = int.MinValue;

        /// <summary>
        /// Was the refund successful?
        /// </summary>
        public bool Success
        {
            get { return _success; }
        }
        private bool _success;

        /// <summary>
        /// The amount of the refund and captured payment, represented in cents
        /// </summary>
        public int AmountInCents
        {
            get { return _amountInCents; }
        }
        private int _amountInCents = int.MinValue;

        /// <summary>
        /// The amount of the refund and captured payment, represented in dollars and cents
        /// </summary>
        public decimal Amount
        {
            get { return Convert.ToDecimal(_amountInCents) / 100; }
        }

        /// <summary>
        /// The memo created for the refund
        /// </summary>
        public string Memo
        {
            get { return _memo; }
        }
        private string _memo = string.Empty;

        #endregion

        #region IComparable<IRefund> Members

        /// <summary>
        /// Method for comparing this IRefund object to another (using AmountInCents)
        /// </summary>
        /// <param name="other">The other IRefund object to compare against.</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(IRefund other)
        {
            return AmountInCents.CompareTo(other.AmountInCents);
        }

        #endregion

        #region IComparable<Refund> Members

        /// <summary>
        /// Method for comparing this Refund object to another (using AmountInCents)
        /// </summary>
        /// <param name="other">The other Refund object to compare against.</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(Refund other)
        {
            return AmountInCents.CompareTo(other.AmountInCents);
        }

        #endregion
    }
}
