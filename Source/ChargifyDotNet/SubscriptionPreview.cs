
#region License, Terms and Conditions
//
// SubscriptionPreview.cs
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
    using ChargifyNET.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    #endregion

    public class SubscriptionPreview : ChargifyBase, ISubscriptionPreview
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xml"></param>
        public SubscriptionPreview(string xml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            if (doc.ChildNodes.Count == 0)
                throw new ArgumentException("XML not valid", nameof(xml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "subscription_preview")
                {
                    LoadFromNode(xml);
                    return;
                }
            }
            // if we get here, then no metadata result data was found
            throw new ArgumentException("XML does not contain expected nodes", nameof(xml));
        }

        public SubscriptionPreview(JsonObject json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            if (json.Keys.Count <= 0) throw new ArgumentException("Not a vaild json object", nameof(json));
            LoadFromJson(json);
        }

        private void LoadFromJson(JsonObject json)
        {
            var settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters = {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                }
            };
            SubscriptionPreviewResult = JsonConvert.DeserializeObject<SubscriptionPreviewResult>(json.ToString(), settings) as SubscriptionPreviewResult;
        }

        private void LoadFromNode(string elementNode)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SubscriptionPreviewResult));
            using (var reader = new StringReader(elementNode))
            {
                SubscriptionPreviewResult = (SubscriptionPreviewResult)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("subscription_preview")]
        public SubscriptionPreviewResult SubscriptionPreviewResult { get; set; }
    }

    [XmlRoot("subscription_preview")]
    public class SubscriptionPreviewResult : ISubscriptionPreviewResult
    {
        [JsonProperty("current_billing_manifest"), XmlElement("current_billing_manifest")]
        public SubscriptionPreviewBillingManifest CurrentBillingManifest { get; set; }

        [JsonProperty("next_billing_manifest"), XmlElement("next_billing_manifest")]
        public SubscriptionPreviewBillingManifest NextBillingManifest { get; set; }
    }

    public class SubscriptionPreviewBillingManifest : ISubscriptionPreviewBillingManifest
    {
        [JsonProperty("line_items"), XmlArray("line_items"), XmlArrayItem("line_item")]
        public List<SubscriptionPreviewLineItem> LineItems { get; set; }

        [JsonProperty("total_in_cents"), XmlElement("total_in_cents")]
        public long TotalInCents { get; set; }

        [XmlIgnore]
        public decimal Total => Convert.ToDecimal(TotalInCents) / 100;

        [JsonProperty("total_discount_in_cents"), XmlElement("total_discount_in_cents")]
        public long TotalDiscountInCents { get; set; }

        [XmlIgnore]
        public decimal TotalDiscount => Convert.ToDecimal(TotalDiscountInCents) / 100;

        [JsonProperty("total_tax_in_cents"), XmlElement("total_tax_in_cents")]
        public long TotalTaxInCents { get; set; }

        [XmlIgnore]
        public decimal TotalTax => Convert.ToDecimal(TotalTaxInCents) / 100;

        [JsonProperty("subtotal_in_cents"), XmlElement("subtotal_in_cents")]
        public long SubtotalInCents { get; set; }

        [XmlIgnore]
        public decimal Subtotal => Convert.ToDecimal(SubtotalInCents) / 100;

        [JsonProperty("start_date"), XmlElement("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date"), XmlElement("end_date")]
        public DateTime EndDate { get; set; }

        [JsonProperty("period_type"), XmlElement("period_type")]
        public string PeriodType { get; set; }

        [JsonProperty("existing_balance_in_cents"), XmlElement("existing_balance_in_cents")]
        public long ExistingBalanceInCents { get; set; }

        [XmlIgnore]
        public decimal ExistingBalance => Convert.ToDecimal(ExistingBalanceInCents) / 100;
    }

    public class SubscriptionPreviewLineItem : ISubscriptionPreviewLineItem
    {
        [JsonProperty("transaction_type"), XmlElement("transaction_type")]
        public string TransactionType { get; set; }

        [JsonProperty("kind"), XmlElement("kind")]
        public string Kind { get; set; }

        [JsonProperty("amount_in_cents"), XmlElement("amount_in_cents")]
        public long AmountInCents { get; set; }

        [XmlIgnore]
        public decimal Amount => Convert.ToDecimal(AmountInCents) / 100;

        [JsonProperty("memo"), XmlElement("memo")]
        public string Memo { get; set; }

        [JsonProperty("discount_amount_in_cents"), XmlElement("discount_amount_in_cents")]
        public long DiscountAmountInCents { get; set; }

        [XmlIgnore]
        public decimal DiscountAmount => Convert.ToDecimal(DiscountAmountInCents) / 100;

        [JsonProperty("taxable_amount_in_cents"), XmlElement("taxable_amount_in_cents")]
        public long TaxableAmountInCents { get; set; }

        [XmlIgnore]
        public decimal TaxableAmount => Convert.ToDecimal(TaxableAmountInCents) / 100;
    }
}
