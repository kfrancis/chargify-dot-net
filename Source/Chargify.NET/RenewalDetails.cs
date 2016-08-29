
#region License, Terms and Conditions
//
// RenewalDetails.cs
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
    using System.Xml;
    using Json;
    #endregion

    /// <summary>
    /// Information that is returned when performing a renewal preview
    /// </summary>
    public class RenewalDetails : ChargifyBase, IRenewalDetails, IComparable<RenewalDetails>
    {
        #region Field Keys
        private const string NextAssessmentAtKey = "next_assessment_at";
        private const string ExistingBalanceInCentsKey = "existing_balance_in_cents";
        private const string SubtotalInCentsKey = "subtotal_in_cents";
        private const string TotalDiscountInCentsKey = "total_discount_in_cents";
        private const string TotalTaxInCentsKey = "total_tax_in_cents";
        private const string TotalInCentsKey = "total_in_cents";
        private const string TotalAmountDueInCentsKey = "total_amount_due_in_cents";
        private const string UncalculatedTaxesKey = "uncalculated_taxes";
        private const string LineItemsKey = "line_items";
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public RenewalDetails()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renewalXml"></param>
        public RenewalDetails(string renewalXml)
        {
            // get the XML into an XML document
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(renewalXml);
            if (xmlDoc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid, expecting children.", nameof(renewalXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in xmlDoc.ChildNodes)
            {
                if (elementNode.Name == "renewal_preview")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain coupon information", nameof(renewalXml));
        }

        /// <summary>
        /// Xml parsing constructor
        /// </summary>
        /// <param name="renewalNode"></param>
        internal RenewalDetails(XmlNode renewalNode)
        {
            if (renewalNode == null) throw new ArgumentNullException(nameof(renewalNode));
            if (renewalNode.Name != "renewal_preview") throw new ArgumentException("Not a vaild renewal preview node", nameof(renewalNode));
            if (renewalNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid, expecting children", nameof(renewalNode));
            LoadFromNode(renewalNode);
        }

        private void LoadFromNode(XmlNode node)
        {
            foreach (XmlNode dataNode in node.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case NextAssessmentAtKey:
                        _nextAssessmentAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case ExistingBalanceInCentsKey:
                        _existingBalanceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case SubtotalInCentsKey:
                        _subtotalInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case TotalDiscountInCentsKey:
                        _totalDiscountInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case TotalTaxInCentsKey:
                        _totalTaxInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case TotalInCentsKey:
                        _totalInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case TotalAmountDueInCentsKey:
                        _totalAmountDueInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case UncalculatedTaxesKey:
                        _uncalculatedTaxes = dataNode.GetNodeContentAsBoolean();
                        break;
                    case LineItemsKey:
                        _lineItems = dataNode.GetNodeContentAsRenewalLineItems();
                        break;
                }
            }
        }

        /// <summary>
        /// Json parsing constructor
        /// </summary>
        /// <param name="obj"></param>
        public RenewalDetails(JsonObject obj)
        {
            LoadFromJson(obj);
        }

        private void LoadFromJson(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get coupon info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case NextAssessmentAtKey:
                        _nextAssessmentAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case ExistingBalanceInCentsKey:
                        _existingBalanceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case SubtotalInCentsKey:
                        _subtotalInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case TotalDiscountInCentsKey:
                        _totalDiscountInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case TotalTaxInCentsKey:
                        _totalTaxInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case TotalInCentsKey:
                        _totalInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case TotalAmountDueInCentsKey:
                        _totalAmountDueInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case UncalculatedTaxesKey:
                        _uncalculatedTaxes = obj.GetJSONContentAsBoolean(key);
                        break;
                    case LineItemsKey:
                        _lineItems = obj.GetJSONContentAsRenewalLineItems(key);
                        break;
                }
            }
        }
        #endregion

        #region IRenewalDetails Implementation
        /// <summary>
        /// A decimal representing the amount of the subscription’s current balance
        /// </summary>
        public decimal ExistingBalance { get { return Convert.ToDecimal(ExistingBalanceInCents) / 100; } }

        /// <summary>
        /// An integer representing the amount of the subscription’s current balance
        /// </summary>
        public int ExistingBalanceInCents
        {
            get
            {
                return _existingBalanceInCents;
            }
        }
        private int _existingBalanceInCents = int.MinValue;

        /// <summary>
        /// An array of <see cref="RenewalLineItem"/> representing the individual transactions that will be created at the next renewal
        /// </summary>
        public List<RenewalLineItem> LineItems
        {
            get
            {
                return _lineItems;
            }
        }
        private List<RenewalLineItem> _lineItems = new List<RenewalLineItem>();

        /// <summary>
        /// The timestamp for the subscription’s next renewal
        /// </summary>
        public DateTime NextAssessmentAt
        {
            get
            {
                return _nextAssessmentAt;
            }
        }
        private DateTime _nextAssessmentAt = DateTime.MinValue;

        /// <summary>
        /// A decimal representing the amount of the total pre-tax, pre-discount charges that will be assessed at the next renewal
        /// </summary>
        public decimal Subtotal { get { return Convert.ToDecimal(SubtotalInCents) / 100; } }

        /// <summary>
        /// An integer representing the amount of the total pre-tax, pre-discount charges that will be assessed at the next renewal
        /// </summary>
        public int SubtotalInCents
        {
            get
            {
                return _subtotalInCents;
            }
        }
        private int _subtotalInCents = int.MinValue;

        /// <summary>
        /// A decimal representing the total amount owed, less any discounts, that will be assessed at the next renewal
        /// </summary>
        public decimal Total { get { return Convert.ToDecimal(TotalInCents) / 100; } }

        /// <summary>
        /// A decimal representing the existing_balance_in_cents plus the total_in_cents
        /// </summary>
        public decimal TotalAmountDue { get { return Convert.ToDecimal(TotalAmountDueInCents) / 100; } }

        /// <summary>
        /// An integer representing the existing_balance_in_cents plus the total_in_cents
        /// </summary>
        public int TotalAmountDueInCents
        {
            get
            {
                return _totalAmountDueInCents;
            }
        }
        private int _totalAmountDueInCents = int.MinValue;

        /// <summary>
        /// A decimal representing the amount of the coupon discounts that will be applied to the next renewal
        /// </summary>
        public decimal TotalDiscount { get { return Convert.ToDecimal(TotalDiscountInCents) / 100; } }

        /// <summary>
        /// An integer representing the amount of the coupon discounts that will be applied to the next renewal
        /// </summary>
        public int TotalDiscountInCents
        {
            get
            {
                return _totalDiscountInCents;
            }
        }
        private int _totalDiscountInCents = int.MinValue;

        /// <summary>
        /// An integer representing the total amount owed, less any discounts, that will be assessed at the next renewal
        /// </summary>
        public int TotalInCents
        {
            get
            {
                return _totalInCents;
            }
        }
        private int _totalInCents = int.MinValue;

        /// <summary>
        /// A decimal representing the total tax charges that will be assessed at the next renewal
        /// </summary>
        public decimal TotalTax { get { return Convert.ToDecimal(TotalTaxInCents) / 100; } }

        /// <summary>
        /// An integer representing the total tax charges that will be assessed at the next renewal
        /// </summary>
        public int TotalTaxInCents
        {
            get
            {
                return _totalTaxInCents;
            }
        }
        private int _totalTaxInCents = int.MinValue;

        /// <summary>
        /// A boolean indicating whether or not additional taxes will be calculated at the time of renewal. 
        /// This will be true if you are using Avalara and the address of the subscription is 
        /// in one of your defined taxable regions.
        /// </summary>
        public bool UncalculatedTaxes
        {
            get
            {
                return _uncalculatedTaxes;
            }
        }
        private bool _uncalculatedTaxes;

        #endregion

        #region IComparable<RenewalDetails>
        /// <summary>
        /// Compare to
        /// </summary>
        /// <param name="other">The other details to compare</param>
        /// <returns></returns>
        public int CompareTo(RenewalDetails other)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}