﻿
#region License, Terms and Conditions
//
// Payment.cs
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
    /// Chargify allows you to record payments that occur outside of the normal flow of payment processing.
    /// These payments are considered external payments.A common case to apply such a payment is when a customer pays by check or some other means for their subscription.
    /// </summary>
    public class Payment : ChargifyBase, IPayment, IComparable<Payment>
    {
        #region Field Keys
        private const string AmountInCentsKey = "amount_in_cents";
        private const string CreatedAtKey = "created_at";
        private const string EndingBalanceInCentsKey = "ending_balance_in_cents";
        private const string IdKey = "id";
        private const string KindKey = "kind";
        private const string MemoKey = "memo";
        private const string PaymentIdKey = "payment_id";
        private const string ProductIdKey = "product_id";
        private const string StartingBalanceInCentsKey = "starting_balance_in_cents";
        private const string SubscriptionIdKey = "subscription_id";
        private const string SuccessKey = "success";
        private const string TypeKey = "type";
        private const string TransactionTypeKey = "transaction_type";
        private const string GatewayTransactionIdKey = "gateway_transaction_id";
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Payment()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paymentXml">XML containing payment info (in expected format)</param>
        public Payment(string paymentXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new();
            doc.LoadXml(paymentXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(paymentXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "payment")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain payment information", nameof(paymentXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paymentNode">XML containing payment info (in expected format)</param>
        internal Payment(XmlNode paymentNode)
        {
            if (paymentNode == null) throw new ArgumentNullException(nameof(paymentNode));
            if (paymentNode.Name != "payment") throw new ArgumentException("Not a vaild payment node", nameof(paymentNode));
            if (paymentNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(paymentNode));
            LoadFromNode(paymentNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paymentObject">JsonObject containing payment info (in expected format)</param>
        public Payment(JsonObject paymentObject)
        {
            if (paymentObject == null) throw new ArgumentNullException(nameof(paymentObject));
            if (paymentObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild payment object", nameof(paymentObject));
            LoadFromJson(paymentObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing object data</param>
        private void LoadFromJson(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get object info, and parse it out
            foreach (var key in obj.Keys)
            {
                switch (key)
                {
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
                    case MemoKey:
                        _memo = obj.GetJSONContentAsString(key);
                        break;
                    case PaymentIdKey:
                        _paymentId = obj.GetJSONContentAsNullableInt(key);
                        break;
                    case ProductIdKey:
                        _productId = obj.GetJSONContentAsInt(key);
                        break;
                    case StartingBalanceInCentsKey:
                        _startingBalanceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case SubscriptionIdKey:
                        _subscriptionId = obj.GetJSONContentAsInt(key);
                        break;
                    case SuccessKey:
                        _success = obj.GetJSONContentAsBoolean(key);
                        break;
                    case TypeKey:
                        _type = obj.GetJSONContentAsString(key);
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
        /// Load data from a payment node
        /// </summary>
        /// <param name="paymentNode">The payment node</param>
        private void LoadFromNode(XmlNode paymentNode)
        {
            foreach (XmlNode dataNode in paymentNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
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
                    case MemoKey:
                        _memo = dataNode.GetNodeContentAsString();
                        break;
                    case PaymentIdKey:
                        _paymentId = dataNode.GetNodeContentAsNullableInt();
                        break;
                    case ProductIdKey:
                        _productId = dataNode.GetNodeContentAsInt();
                        break;
                    case StartingBalanceInCentsKey:
                        _startingBalanceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case SubscriptionIdKey:
                        _subscriptionId = dataNode.GetNodeContentAsInt();
                        break;
                    case SuccessKey:
                        _success = dataNode.GetNodeContentAsBoolean();
                        break;
                    case TypeKey:
                        _type = dataNode.GetNodeContentAsString();
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

        #region Implementation
        /// <summary>
        /// The amount of the payment
        /// </summary>
        public int AmountInCents { get { return _amountInCents; } }
        private int _amountInCents = int.MinValue;

        /// <summary>
        /// The amount of the payment
        /// </summary>
        public decimal Amount { get { return Convert.ToDecimal(_amountInCents) / 100; } }

        /// <summary>
        /// The date the payment was created
        /// </summary>
        public DateTime CreatedAt { get { return _createdAt; } }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// The ending balance of the subscription after the payment
        /// </summary>
        public int EndingBalanceInCents { get { return _endingBalanceInCents; } }
        private int _endingBalanceInCents = int.MinValue;

        /// <summary>
        /// The ID of the payment
        /// </summary>
        public int ID { get { return _id; } }
        private int _id = int.MinValue;

        /// <summary>
        /// The kind of payment
        /// </summary>
        public string Kind { get { return _kind; } }
        private string _kind = string.Empty;

        /// <summary>
        /// The payment memo
        /// </summary>
        public string Memo { get { return _memo; } }
        private string _memo = string.Empty;

        /// <summary>
        /// The ID of the payment
        /// </summary>
        public int? PaymentID { get { return _paymentId; } }
        private int? _paymentId;

        /// <summary>
        /// The ID of the product
        /// </summary>
        public int ProductID { get { return _productId; } }
        private int _productId = int.MinValue;

        /// <summary>
        /// The balance of the subscription before the payment
        /// </summary>
        public int StartingBalanceInCents { get { return _startingBalanceInCents; } }
        private int _startingBalanceInCents = int.MinValue;

        /// <summary>
        /// The subscription ID
        /// </summary>
        public int SubscriptionID { get { return _subscriptionId; } }
        private int _subscriptionId = int.MinValue;

        /// <summary>
        /// Was the payment successful?
        /// </summary>
        public bool Success { get { return _success; } }
        private bool _success;

        /// <summary>
        /// The type of payment
        /// </summary>
        public string Type { get { return _type; } }
        private string _type = string.Empty;

        /// <summary>
        /// The type of transaction
        /// </summary>
        public string TransactionType { get { return _transactionType; } }
        private string _transactionType = string.Empty;

        /// <summary>
        /// The related gateway transaction ID
        /// </summary>
        public int? GatewayTransactionID { get { return _gatewayTransactionId; } }
        private int? _gatewayTransactionId;
        #endregion

        #region IComparible<Payment> Members
        /// <summary>
        /// Compare the payments
        /// </summary>
        /// <param name="other">The other payment</param>
        /// <returns></returns>
        public int CompareTo(IPayment other)
        {
            return ID.CompareTo(other.ID);
        }

        /// <summary>
        /// Compare the payments
        /// </summary>
        /// <param name="other">The other payment</param>
        /// <returns></returns>
        public int CompareTo(Payment other)
        {
            return ID.CompareTo(other.ID);
        }
        #endregion
    }
}
