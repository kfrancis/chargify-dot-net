#region License, Terms and Conditions

//
// IRenewalDetails.cs
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

// ReSharper disable once CheckNamespace
namespace ChargifyNET
{
    #region Imports
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
    using ChargifyNET.Json;
    using Newtonsoft.Json;
    #endregion

    /// <summary>
    /// The output of a renewal preview
    /// </summary>
    public interface IRenewalDetails
    {
        /// <summary>
        /// The timestamp for the subscription’s next renewal
        /// </summary>
        [XmlElement("next_assessment_at"), JsonProperty("next_assessment_at")]
        DateTime NextAssessmentAt { get; }

        /// <summary>
        /// An integer representing the amount of the total pre-tax, pre-discount charges that will be assessed at the next renewal
        /// </summary>
        [XmlElement("subtotal_in_cents"), JsonProperty("subtotal_in_cents")]
        int SubtotalInCents { get; }

        /// <summary>
        /// An decimal representing the amount of the total pre-tax, pre-discount charges that will be assessed at the next renewal
        /// </summary>
        decimal Subtotal { get; }

        /// <summary>
        /// An integer representing the total tax charges that will be assessed at the next renewal
        /// </summary>
        [XmlElement("total_tax_in_cents"), JsonProperty("total_tax_in_cents")]
        int TotalTaxInCents { get; }

        /// <summary>
        /// An decimal representing the total tax charges that will be assessed at the next renewal
        /// </summary>
        decimal TotalTax { get; }

        /// <summary>
        /// An integer representing the amount of the coupon discounts that will be applied to the next renewal
        /// </summary>
        [XmlElement("total_discount_in_cents"), JsonProperty("total_discount_in_cents")]
        int TotalDiscountInCents { get; }

        /// <summary>
        /// An decimal representing the amount of the coupon discounts that will be applied to the next renewal
        /// </summary>
        decimal TotalDiscount { get; }

        /// <summary>
        /// An integer representing the total amount owed, less any discounts, that will be assessed at the next renewal
        /// </summary>
        [XmlElement("total_in_cents"), JsonProperty("total_in_cents")]
        int TotalInCents { get; }

        /// <summary>
        /// An decimal representing the total amount owed, less any discounts, that will be assessed at the next renewal
        /// </summary>
        decimal Total { get; }

        /// <summary>
        /// An integer representing the amount of the subscription’s current balance
        /// </summary>
        [XmlElement("existing_balance_in_cents"), JsonProperty("existing_balance_in_cents")]
        int ExistingBalanceInCents { get; }

        /// <summary>
        /// An decimal representing the amount of the subscription’s current balance
        /// </summary>
        decimal ExistingBalance { get; }

        /// <summary>
        /// An integer representing the existing_balance_in_cents plus the total_in_cents
        /// </summary>
        [XmlElement("total_amount_due_in_cents"), JsonProperty("total_amount_due_in_cents")]
        int TotalAmountDueInCents { get; }

        /// <summary>
        /// An decimal representing the existing_balance_in_cents plus the total_in_cents
        /// </summary>
        decimal TotalAmountDue { get; }

        /// <summary>
        /// A boolean indicating whether or not additional taxes will be calculated at the time of renewal.
        /// This will be true if you are using Avalara and the address of the subscription is
        /// in one of your defined taxable regions.
        /// </summary>
        [XmlElement("uncalculated_taxes"), JsonProperty("uncalculated_taxes")]
        bool UncalculatedTaxes { get; }

        /// <summary>
        /// An array of <see cref="RenewalLineItem"/> representing the individual transactions that will be created at the next renewal
        /// </summary>
        [XmlArray("line_items")]
        [XmlArrayItem("line_item", typeof(RenewalLineItem))]
        [JsonProperty("line_items")]
        List<RenewalLineItem> LineItems { get; }
    }

    /// <summary>
    /// The line item included in a renewal preview response
    /// </summary>
    [XmlRoot("line_item")]
    public class RenewalLineItem
    {
        private const string TransactionTypeKey = "transaction_type";
        private const string AmountInCentsKey = "amount_in_cents";
        private const string DiscountAmountInCentsKey = "discount_amount_in_cents";
        private const string KindKey = "kind";
        private const string MemoKey = "memo";
        private const string ProductHandleKey = "product_handle";
        private const string ProductIdKey = "product_id";
        private const string ProductNameKey = "product_name";
        private const string TaxableAmountInCentsKey = "taxable_amount_in_cents";

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public RenewalLineItem()
        { }

        public RenewalLineItem(string xml)
            : base()
        {
            // get the XML into an XML document
            XmlDocument Doc = new();
            Doc.LoadXml(xml);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(xml));
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
            throw new ArgumentException("XML does not contain expected data", nameof(xml));
        }

        private void LoadFromNode(XmlNode elementNode)
        {
            foreach (XmlNode dataNode in elementNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case AmountInCentsKey:
                        AmountInCents = dataNode.GetNodeContentAsInt();
                        break;

                    case DiscountAmountInCentsKey:
                        DiscountAmountInCents = dataNode.GetNodeContentAsInt();
                        break;

                    case KindKey:
                        Kind = dataNode.GetNodeContentAsString();
                        break;

                    case MemoKey:
                        Memo = dataNode.GetNodeContentAsString();
                        break;

                    case ProductHandleKey:
                        ProductHandle = dataNode.GetNodeContentAsString();
                        break;

                    case ProductIdKey:
                        ProductId = dataNode.GetNodeContentAsLong();
                        break;

                    case ProductNameKey:
                        ProductName = dataNode.GetNodeContentAsString();
                        break;

                    case TaxableAmountInCentsKey:
                        TaxableAmountInCents = dataNode.GetNodeContentAsInt();
                        break;

                    case TransactionTypeKey:
                        TransactionType = dataNode.GetNodeContentAsTransactionType();
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Xml parsing constructor
        /// </summary>
        /// <param name="node"></param>
        public RenewalLineItem(XmlNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.Name != "line_item") throw new ArgumentException("Not a vaild xml node", nameof(node));
            if (node.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(node));
            this.LoadFromNode(node);
        }

        /// <summary>
        /// Json parsing constructor
        /// </summary>
        /// <param name="renewalLineItem"></param>
        public RenewalLineItem(JsonObject renewalLineItem)
        {
            if (renewalLineItem == null) throw new ArgumentNullException(nameof(renewalLineItem));
            if (renewalLineItem.Keys.Count <= 0) throw new ArgumentException("Not a vaild renewal line object", nameof(renewalLineItem));
            this.LoadFromJSON(renewalLineItem);
        }

        private void LoadFromJSON(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get referral code info, and parse it out
            foreach (var key in obj.Keys)
            {
                switch (key)
                {
                    case AmountInCentsKey:
                        AmountInCents = obj.GetJSONContentAsInt(key);
                        break;

                    case DiscountAmountInCentsKey:
                        DiscountAmountInCents = obj.GetJSONContentAsInt(key);
                        break;

                    case KindKey:
                        Kind = obj.GetJSONContentAsString(key);
                        break;

                    case MemoKey:
                        Memo = obj.GetJSONContentAsString(key);
                        break;

                    case ProductHandleKey:
                        ProductHandle = obj.GetJSONContentAsString(key);
                        break;

                    case ProductIdKey:
                        ProductId = obj.GetJSONContentAsInt(key);
                        break;

                    case ProductNameKey:
                        ProductName = obj.GetJSONContentAsString(key);
                        break;

                    case TaxableAmountInCentsKey:
                        TaxableAmountInCents = obj.GetJSONContentAsInt(key);
                        break;

                    case TransactionTypeKey:
                        TransactionType = obj.GetJSONContentAsTransactionType(key);
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of transaction
        /// </summary>
        [XmlElement(TransactionTypeKey), JsonProperty(TransactionTypeKey)]
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// The kind of transaction
        /// </summary>
        [XmlElement(KindKey), JsonProperty(KindKey)]
        public string Kind { get; set; }

        /// <summary>
        /// The amount of the transaction in cents
        /// </summary>
        [XmlElement(AmountInCentsKey), JsonProperty(AmountInCentsKey)]
        public int AmountInCents { get; set; }

        /// <summary>
        /// The amount of the transaction in dollars and cents
        /// </summary>
        public decimal Amount
        { get { return Convert.ToDecimal(AmountInCents) / 100; } }

        /// <summary>
        /// The memo of the transaction
        /// </summary>
        [XmlElement(MemoKey), JsonProperty(MemoKey)]
        public string Memo { get; set; }

        /// <summary>
        /// The discount amount in cents
        /// </summary>
        [XmlElement(DiscountAmountInCentsKey), JsonProperty(DiscountAmountInCentsKey)]
        public int DiscountAmountInCents { get; set; }

        /// <summary>
        /// The discount amount
        /// </summary>
        public decimal DiscountAmount
        { get { return Convert.ToDecimal(DiscountAmountInCents) / 100; } }

        /// <summary>
        /// The taxable amount in cents
        /// </summary>
        [XmlElement(TaxableAmountInCentsKey), JsonProperty(TaxableAmountInCentsKey)]
        public int TaxableAmountInCents { get; set; }

        /// <summary>
        /// The taxable amount
        /// </summary>
        public decimal TaxableAmount
        { get { return Convert.ToDecimal(TaxableAmountInCents) / 100; } }

        [XmlElement(ProductHandleKey), JsonProperty(ProductHandleKey)]
        public string ProductHandle { get; set; }

        [XmlElement(ProductIdKey), JsonProperty(ProductIdKey)]
        public long ProductId { get; set; }

        [XmlElement(ProductNameKey), JsonProperty(ProductNameKey)]
        public string ProductName { get; set; }

        #endregion
    }
}
