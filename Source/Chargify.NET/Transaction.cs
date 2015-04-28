
#region License, Terms and Conditions
//
// Transaction.cs
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
    using ChargifyNET.Json;
    #endregion

    /// <summary>
    /// Transaction for a subscription/product for a customer.
    /// </summary>
    [DebuggerDisplay("Type: {Type}, Amount: {Amount}, SubscriptionID: {SubscriptionID}, Success: {Success}")]
    public class Transaction : ChargifyBase, ITransaction, IComparable<Transaction>
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private Transaction() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TransactionXML">An XML string containing a transaction node</param>
        public Transaction(string TransactionXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(TransactionXML);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "TransactionXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "transaction")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no transaction info was found
            throw new ArgumentException("XML does not contain transaction information", "TransactionXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transactionNode">An xml node with transaction information</param>
        internal Transaction(XmlNode transactionNode)
            : base()
        {
            if (transactionNode == null) throw new ArgumentNullException("transactionNode");
            if (transactionNode.Name != "transaction") throw new ArgumentException("Not a vaild transaction node", "transactionNode");
            if (transactionNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "transactionNode");
            LoadFromNode(transactionNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transactionObject">JsonObject containing transaction info (in expected format)</param>
        public Transaction(JsonObject transactionObject)
            : base()
        {
            if (transactionObject == null) throw new ArgumentNullException("transactionObject");
            if (transactionObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild transaction object", "transactionObject");
            this.LoadFromJSON(transactionObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing transaction data</param>
        private void LoadFromJSON(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case "transaction_type":
                        _type = obj.GetJSONContentAsTransactionType(key);
                        break;
                    case "id":
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case "amount_in_cents":
                        _amountInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case "created_at":
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case "starting_balance_in_cents":
                        _startingBalanceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case "ending_balance_in_cents":
                        _endingBalanceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case "memo":
                        _memo = obj.GetJSONContentAsString(key);
                        break;
                    case "subscription_id":
                        _subscriptionID = obj.GetJSONContentAsInt(key);
                        break;
                    case "product_id":
                        _productID = obj.GetJSONContentAsInt(key);
                        break;
                    case "success":
                        _success = obj.GetJSONContentAsBoolean(key);
                        break;
                    case "payment_id":
                        _paymentID = obj.GetJSONContentAsInt(key);
                        break;
                    case "kind":
                        _kind = obj.GetJSONContentAsEnum<TransactionChargeKind>(key);
                        break;
                    case "gateway_transaction_id":
                        _gatewayTransactionID = obj.GetJSONContentAsString(key);
                        break;
                    case "gateway_order_id":
                        _gatewayOrderID = obj.GetJSONContentAsString(key);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a transaction node
        /// </summary>
        /// <param name="transactionNode">The transaction node</param>
        private void LoadFromNode(XmlNode transactionNode)
        {
            // loop through the nodes to get product info
            foreach (XmlNode dataNode in transactionNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case "transaction_type":
                        _type = dataNode.GetNodeContentAsTransactionType();
                        break;
                    case "id":
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case "amount_in_cents":
                        _amountInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case "created_at":
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case "starting_balance_in_cents":
                        _startingBalanceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case "ending_balance_in_cents":
                        _endingBalanceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case "memo":
                        _memo = dataNode.GetNodeContentAsString();
                        break;
                    case "subscription_id":
                        _subscriptionID = dataNode.GetNodeContentAsInt();
                        break;
                    case "product_id":
                        _productID = dataNode.GetNodeContentAsInt();
                        break;
                    case "success":
                        _success = dataNode.GetNodeContentAsBoolean();
                        break;
                    case "payment_id":
                        _paymentID = dataNode.GetNodeContentAsInt();
                        break;
                    case "kind":
                        _kind = dataNode.GetNodeContentAsEnum<TransactionChargeKind>();
                        break;
                    case "gateway_transaction_id":
                        _gatewayTransactionID = dataNode.GetNodeContentAsString();
                        break;
                    case "gateway_order_id":
                        _gatewayOrderID = dataNode.GetNodeContentAsString();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region ITransaction Members

        /// <summary>
        /// The type of transaction
        /// </summary>
        public TransactionType Type
        {
            get { return _type; }
        }
        private TransactionType _type = TransactionType.Unknown;

        /// <summary>
        /// The unique identifier for the Transaction
        /// </summary>
        public int ID
        {
            get { return _id; }
        }
        private int _id = 0;

        /// <summary>
        /// The amount in cents for the Transaction
        /// </summary>
        public int AmountInCents
        {
            get { return _amountInCents; }
        }
        private int _amountInCents = 0;

        /// <summary>
        ///  The amount (in dollars and cents) for the Transaction
        /// </summary>
        public decimal Amount
        {
            get { return Convert.ToDecimal(this._amountInCents) / 100; }
        }

        /// <summary>
        /// Timestamp indicating when the Transaction was created
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// he initial balance on the subscription before the Transaction has been processed, in cents.
        /// </summary>
        public int StartingBalanceInCents
        {
            get { return _startingBalanceInCents; }
        }
        private int _startingBalanceInCents = int.MinValue;

        /// <summary>
        /// he initial balance on the subscription before the Transaction has been processed, in dollars and cents.
        /// </summary>
        public decimal StartingBalance
        {
            get { return Convert.ToDecimal(this._startingBalanceInCents) / 100; }
        }

        /// <summary>
        /// The remaining balance on the subscription after the Transaction has been processed, in cents.
        /// </summary>
        public int EndingBalanceInCents
        {
            get { return _endingBalanceInCents; }
        }
        private int _endingBalanceInCents = 0;

        /// <summary>
        /// The remaining balance on the subscription after the Transaction has been processed, in dollars and cents.
        /// </summary>
        public decimal EndingBalance
        {
            get { return Convert.ToDecimal(this._endingBalanceInCents) / 100; }
        }

        /// <summary>
        /// A note about the Transaction
        /// </summary>
        public string Memo
        {
            get { return _memo; }
        }
        private string _memo = string.Empty;

        /// <summary>
        /// The unique identifier for the associated Subscription
        /// </summary>
        public int SubscriptionID
        {
            get { return _subscriptionID; }
        }
        private int _subscriptionID = 0;

        /// <summary>
        /// The unique identifier for the associated Product
        /// </summary>
        public int ProductID
        {
            get { return _productID; }
        }
        private int _productID = 0;

        /// <summary>
        /// Whether or not the Transaction was successful.
        /// </summary>
        public bool Success
        {
            get { return _success; }
        }
        private bool _success = false;

        /// <summary>
        /// The unique identifier for the payment being explicitly refunded (in whole or in part) by this transaction. 
        /// Will be null for all transaction types except for “Refund”. May be null even for Refunds. 
        /// For partial refunds, more than one Refund transaction may reference the same payment_id
        /// </summary>
        public int PaymentID
        {
            get { return _paymentID; }
        }
        private int _paymentID = int.MinValue;

        /// <summary>
        /// The specific "subtype" for the transaction_type
        /// </summary>
        public TransactionChargeKind? Kind
        {
            get { return _kind; }
        }
        private TransactionChargeKind? _kind = null;

        /// <summary>
        /// The transaction ID from the remote gateway (i.e. Authorize.Net), if one exists
        /// </summary>
        public string GatewayTransactionID
        {
            get { return _gatewayTransactionID; }
        }
        private string _gatewayTransactionID = string.Empty;

        /// <summary>
        /// A gateway-specific identifier for the transaction, separate from the gateway_transaction_id
        /// </summary>
        public string GatewayOrderID
        {
            get { return _gatewayOrderID; }
        }
        private string _gatewayOrderID = string.Empty;        

        #endregion

        #region IComparable<ITransaction> Members

        /// <summary>
        /// Compare this instance to another (by ID)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(ITransaction other)
        {
            return this.ID.CompareTo(other.ID);
        }

        #endregion

        #region IComparable<Transaction> Members

        /// <summary>
        /// Compare this instance to another (by ID)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(Transaction other)
        {
            return this.ID.CompareTo(other.ID);
        }

        #endregion
    }
}
