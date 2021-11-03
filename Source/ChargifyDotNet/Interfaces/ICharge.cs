﻿
#region License, Terms and Conditions
//
// ICharge.cs
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
    using Newtonsoft.Json;
    #region Imports
    using System;
    using System.Xml.Serialization;
    #endregion


    /// <summary>
    /// Charge create/input options
    /// </summary>
    public interface IChargeCreateOptions
    {
        /// <summary>
        /// The amount of the charge
        /// </summary>
        decimal? Amount { get; set; }
        /// <summary>
        /// The amount of the charge (in cents)
        /// </summary>
        int? AmountInCents { get; set; }
        /// <summary>
        /// The charge memo/description
        /// </summary>
        string Memo { get; set; }
        /// <summary>
        /// Should the negative balance be used when processing this charge?
        /// </summary>
        bool? UseNegativeBalance { get; set; }
        /// <summary>
        /// Should the charge be delayed until the next assessment date?
        /// </summary>
        bool? DelayCapture { get; set; }
        /// <summary>
        /// Should the charge accrue on the balance if the charge failed now
        /// </summary>
        bool? AccrueOnFailure { get; set; }
        /// <summary>
        /// Is the charge taxable?
        /// </summary>
        bool? Taxable { get; set; }
        /// <summary>
        /// The collection method for this charge
        /// </summary>
        PaymentCollectionMethod PaymentCollectionMethod { get; set; }
    }

    /// <summary>
    /// The charge create/input options
    /// </summary>
    [XmlType("charge"), JsonObject("charge")]
    [Serializable]
    public class ChargeCreateOptions : IChargeCreateOptions
    {
        /// <summary>
        /// The amount of the charge
        /// </summary>
        [XmlElement("amount"), JsonProperty("amount")]
        public decimal? Amount { get; set; }
        /// <summary>
        /// Ignore, used to determine if the field should be serialized
        /// </summary>
        public bool ShouldSerializeAmount()
        {
            return Amount.HasValue && !AmountInCents.HasValue;
        }

        /// <summary>
        /// The amount of the charge (in cents)
        /// </summary>
        [XmlElement("amount_in_cents"), JsonProperty("amount_in_cents")]
        public int? AmountInCents { get; set; }
        /// <summary>
        /// Ignore, used to determine if the field should be serialized
        /// </summary>
        public bool ShouldSerializeAmountInCents()
        {
            return AmountInCents.HasValue && !Amount.HasValue;
        }

        /// <summary>
        /// The charge memo/description
        /// </summary>
        [XmlElement("memo"), JsonProperty("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// Ignore, used to determine if the field should be serialized
        /// </summary>
        public bool ShouldSerializeMemo()
        {
            return !string.IsNullOrWhiteSpace(Memo);
        }

        /// <summary>
        /// Should the negative balance be used when processing this charge?
        /// </summary>
        [XmlElement("use_negative_balance"), JsonProperty("use_negative_balance")]
        public bool? UseNegativeBalance { get; set; }

        /// <summary>
        /// Ignore, used to determine if the field should be serialized
        /// </summary>
        public bool ShouldSerializeUseNegativeBalance()
        {
            return UseNegativeBalance.HasValue;
        }

        /// <summary>
        /// Should the charge be delayed until the next assessment date?
        /// </summary>
        [XmlElement("delay_capture"), JsonProperty("delay_capture")]
        public bool? DelayCapture { get; set; }

        /// <summary>
        /// Ignore, used to determine if the field should be serialized
        /// </summary>
        public bool ShouldSerializeDelayCapture()
        {
            return DelayCapture.HasValue;
        }

        /// <summary>
        /// Should the charge accrue on the balance if the charge failed now
        /// </summary>
        [XmlElement("accrue_on_failure"), JsonProperty("accrue_on_failure")]
        public bool? AccrueOnFailure { get; set; }

        /// <summary>
        /// Ignore, used to determine if the field should be serialized
        /// </summary>
        public bool ShouldSerializeAccrueOnFailure()
        {
            return AccrueOnFailure.HasValue;
        }

        /// <summary>
        /// Is the charge taxable?
        /// </summary>
        [XmlElement("taxable"), JsonProperty("taxable")]
        public bool? Taxable { get; set; }

        /// <summary>
        /// Ignore, used to determine if the field should be serialized
        /// </summary>
        public bool ShouldSerializeTaxable()
        {
            return Taxable.HasValue;
        }

        /// <summary>
        /// The collection method for this charge
        /// </summary>
        [XmlElement("payment_collection_method"), JsonProperty("payment_collection_method")]
        public PaymentCollectionMethod PaymentCollectionMethod { get; set; }

        /// <summary>
        /// Ignore, used to determine if the field should be serialized
        /// </summary>
        public bool ShouldSerializePaymentCollectionMethod()
        {
            return PaymentCollectionMethod != PaymentCollectionMethod.Unknown;
        }
    }

    /// <summary>
    /// The one time charge class
    /// </summary>
    public interface ICharge : IComparable<ICharge>
    {
        /// <summary>
        /// Either true or false, depending on the success of the charge.
        /// <remarks>At this time, all charges that are returned will return true here. 
        /// Flase may be returned in the future when more options are added to the charge creation API</remarks>
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Get the amount (in cents)
        /// </summary>
        int AmountInCents { get; }

        /// <summary>
        /// Get the amount, in dollars and cents.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// The memo for the created charge
        /// </summary>
        string Memo { get; }

        /// <summary>
        /// The date the charge was created
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// The ending balance of the subscription, in cents
        /// </summary>
        int EndingBalanceInCents { get; }

        /// <summary>
        /// The ending balance of the subscription, in dollars and cents (formatted as decimal)
        /// </summary>
        decimal EndingBalance { get; }

        /// <summary>
        /// The ID of the charge
        /// </summary>
        int ID { get; }

        /// <summary>
        /// The kind of charge
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// The ID of the payment associated with this charge
        /// </summary>
        int? PaymentID { get; }

        /// <summary>
        /// The product ID the subscription was subscribed to at the time of the charge
        /// </summary>
        int ProductID { get; }

        /// <summary>
        /// The subscription ID that this charge was applied to
        /// </summary>
        int SubscriptionID { get; }

        /// <summary>
        /// The type of charge
        /// </summary>
        string ChargeType { get; }

        /// <summary>
        /// The type of transaction
        /// </summary>
        string TransactionType { get; }

        /// <summary>
        /// The ID of the gateway transaction, useful for debugging.
        /// </summary>
        int? GatewayTransactionID { get; }
    }
}