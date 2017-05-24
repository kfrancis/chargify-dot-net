
#region License, Terms and Conditions
//
// Charge.cs
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
    /// The one-time charge class bump
    /// </summary>
    public class Charge : ChargifyBase, ICharge, IComparable<Charge>
    {
        #region Field Keys
        private const string SuccessKey = "success";
        private const string MemoKey = "memo";
        private const string AmountInCentsKey = "amount_in_cents";
        private const string CreatedAtKey = "created_at";
        private const string EndingBalanceInCentsKey = "ending_balance_in_cents";
        private const string IdKey = "id";
        private const string KindKey = "kind";
        private const string PaymentIdKey = "payment_id";
        private const string ProductIdKey = "product_id";
        private const string SubscriptionIdKey = "subscription_id";
        private const string TypeKey = "type";
        private const string TransactionTypeKey = "transaction_type";
        private const string GatewayTransactionIdKey = "gateway_transaction_id";
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Charge()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chargeXml">XML containing charge info (in expected format)</param>
        public Charge(string chargeXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(chargeXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(chargeXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "charge")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain charge information", nameof(chargeXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chargeNode">XML containing charge info (in expected format)</param>
        internal Charge(XmlNode chargeNode)
        {
            if (chargeNode == null) throw new ArgumentNullException(nameof(chargeNode));
            if (chargeNode.Name != "charge") throw new ArgumentException("Not a vaild charge node", nameof(chargeNode));
            if (chargeNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(chargeNode));
            LoadFromNode(chargeNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chargeObject">Json containing charge info (in expected format)</param>
        public Charge(JsonObject chargeObject)
        {
            if (chargeObject == null) throw new ArgumentNullException(nameof(chargeObject));
            if (chargeObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild charge object", nameof(chargeObject));
            LoadFromJson(chargeObject);
        }

        private void LoadFromJson(JsonObject obj)
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
                    case CreatedAtKey:
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case EndingBalanceInCentsKey:
                        _endingBalanceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case IdKey:
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case KindKey:
                        _kind = obj.GetJSONContentAsString(key);
                        break;
                    case PaymentIdKey:
                        _paymentId = obj.GetJSONContentAsNullableInt(key);
                        break;
                    case ProductIdKey:
                        _productId = obj.GetJSONContentAsInt(key);
                        break;
                    case SubscriptionIdKey:
                        _subscriptionId = obj.GetJSONContentAsInt(key);
                        break;
                    case TypeKey:
                        _chargeType = obj.GetJSONContentAsString(key);
                        break;
                    case TransactionTypeKey:
                        _transactionType = obj.GetJSONContentAsString(key);
                        break;
                    case GatewayTransactionIdKey:
                        _gatewayTransactionId = obj.GetJSONContentAsNullableInt(key);
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
                    case SuccessKey:
                        _success = dataNode.GetNodeContentAsBoolean();
                        break;
                    case MemoKey:
                        _memo = dataNode.GetNodeContentAsString();
                        break;
                    case AmountInCentsKey:
                        _amountInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case CreatedAtKey:
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case EndingBalanceInCentsKey:
                        _endingBalanceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case IdKey:
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case KindKey:
                        _kind = dataNode.GetNodeContentAsString();
                        break;
                    case PaymentIdKey:
                        _paymentId = dataNode.GetNodeContentAsNullableInt();
                        break;
                    case ProductIdKey:
                        _productId = dataNode.GetNodeContentAsInt();
                        break;
                    case SubscriptionIdKey:
                        _subscriptionId = dataNode.GetNodeContentAsInt();
                        break;
                    case TypeKey:
                        _chargeType = dataNode.GetNodeContentAsString();
                        break;
                    case TransactionTypeKey:
                        _transactionType = dataNode.GetNodeContentAsString();
                        break;
                    case GatewayTransactionIdKey:
                        _gatewayTransactionId = dataNode.GetNodeContentAsNullableInt();
                        break;
                }
            }
        }

        #endregion

        #region ICharge Members

        /// <summary>
        /// Either true or false, depending on the success of the charge.
        /// <remarks>At this time, all charges that are returned will return true here. 
        /// Flase may be returned in the future when more options are added to the charge creation API</remarks>
        /// </summary>
        public bool Success
        {
            get { return _success; }
        }
        private bool _success;

        /// <summary>
        /// Get the amount (in cents)
        /// </summary>
        public int AmountInCents
        {
            get { return _amountInCents; }
        }
        private int _amountInCents;

        /// <summary>
        /// Get the amount, in dollars and cents.
        /// </summary>
        public decimal Amount
        {
            get { return Convert.ToDecimal(_amountInCents) / 100; }
        }

        /// <summary>
        /// The memo for the created charge
        /// </summary>
        public string Memo
        {
            get { return _memo; }
        }
        private string _memo = string.Empty;

        /// <summary>
        /// The date the charge was created
        /// </summary>
        public DateTime CreatedAt { get { return _createdAt; } }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// The ending balance of the subscription, in cents
        /// </summary>
        public int EndingBalanceInCents { get { return _endingBalanceInCents; } }
        private int _endingBalanceInCents = int.MinValue;

        /// <summary>
        /// The ending balance of the subscription, in dollars and cents (formatted as decimal)
        /// </summary>
        public decimal EndingBalance { get { return Convert.ToDecimal(_endingBalanceInCents) / 100; } }

        /// <summary>
        /// The ID of the charge
        /// </summary>
        public int ID { get { return _id; } }
        private int _id = int.MinValue;

        /// <summary>
        /// The kind of charge
        /// </summary>
        public string Kind { get { return _kind; } }
        private string _kind = string.Empty;

        /// <summary>
        /// The ID of the payment associated with this charge
        /// </summary>
        public int? PaymentID { get { return _paymentId; } }
        private int? _paymentId;

        /// <summary>
        /// The product ID the subscription was subscribed to at the time of the charge
        /// </summary>
        public int ProductID { get { return _productId; } }
        private int _productId = int.MinValue;

        /// <summary>
        /// The subscription ID that this charge was applied to
        /// </summary>
        public int SubscriptionID { get { return _subscriptionId; } }
        private int _subscriptionId = int.MinValue;

        /// <summary>
        /// The type of charge
        /// </summary>
        public string ChargeType { get { return _chargeType; } }
        private string _chargeType = string.Empty;

        /// <summary>
        /// The type of transaction
        /// </summary>
        public string TransactionType { get { return _transactionType; } }
        private string _transactionType = string.Empty;

        /// <summary>
        /// The ID of the gateway transaction, useful for debugging.
        /// </summary>
        public int? GatewayTransactionID { get { return _gatewayTransactionId; } }
        private int? _gatewayTransactionId;

        #endregion

        #region IComparable<ICharge> Members

        /// <summary>
        /// Compare this instance to another (by AmountInCents)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(ICharge other)
        {
            return AmountInCents.CompareTo(other.AmountInCents);
        }

        #endregion

        #region IComparable<Charge> Members

        /// <summary>
        /// Compare this instance to another (by AmountInCents)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(Charge other)
        {
            return AmountInCents.CompareTo(other.AmountInCents);
        }

        #endregion
    }
}