
#region License, Terms and Conditions
//
// Adjustment.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2011 Clinical Support Systems, Inc. All rights reserved.
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
    using ChargifyNET.Json;
    #endregion

    /// <summary>
    /// Adjustments allow you to change the current balance of the subscription. Adjustments with positive
    /// amounts make the balance go up, adjustments with negative amounts make the balance go down (like credits)
    /// http://docs.chargify.com/api-adjustments
    /// </summary>
    public class Adjustment : ChargifyBase, IAdjustment, IComparable<Adjustment>
    {
        #region Field Keys
        private const string IDKey = "id";
        private const string SuccessKey = "success";
        private const string MemoKey = "memo";
        private const string AmountInCentsKey = "amount_in_cents";
        private const string EndingBalanceInCentsKey = "ending_balance_in_cents";
        private const string TypeKey = "type";
        private const string TransactionTypeKey = "transaction_type";
        private const string SubscriptionIDKey = "subscription_id";
        private const string CreatedAtKey = "created_at";
        private const string ProductIDKey = "product_id";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Adjustment() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AdjustmentXML">XML containing adjustment info (in expected format)</param>
        public Adjustment(string AdjustmentXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(AdjustmentXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "AdjustmentXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == "adjustment")
                {
                    this.LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain adjustment information", "AdjustmentXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adjustmentNode">XML containing adjustment info (in expected format)</param>
        internal Adjustment(XmlNode adjustmentNode)
            : base()
        {
            if (adjustmentNode == null) throw new ArgumentNullException("adjustmentNode");
            if (adjustmentNode.Name != "adjustment") throw new ArgumentException("Not a vaild adjustment node", "adjustmentNode");
            if (adjustmentNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "adjustmentNode");
            this.LoadFromNode(adjustmentNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adjustmentObject">Json containing adjustment info (in expected format)</param>
        public Adjustment(JsonObject adjustmentObject)
            : base()
        {
            if (adjustmentObject == null) throw new ArgumentNullException("adjustmentObject");
            if (adjustmentObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild charge object", "adjustmentObject");
            this.LoadFromJSON(adjustmentObject);
        }

        private void LoadFromJSON(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IDKey:
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case EndingBalanceInCentsKey:
                        _endingBalanceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case TypeKey:
                        _type = obj.GetJSONContentAsString(key);
                        break;
                    case TransactionTypeKey:
                        _transactionType = obj.GetJSONContentAsTransactionType(key);
                        break;
                    case SubscriptionIDKey:
                        _subscriptionID = obj.GetJSONContentAsInt(key);
                        break;
                    case ProductIDKey:
                        _productID = obj.GetJSONContentAsInt(key);
                        break;
                    case CreatedAtKey:
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
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
        /// Load data from a adjustment node
        /// </summary>
        /// <param name="adjustmentNode">The adjustment node</param>
        private void LoadFromNode(XmlNode adjustmentNode)
        {
            foreach (XmlNode dataNode in adjustmentNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case IDKey:
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case EndingBalanceInCentsKey:
                        _endingBalanceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case TypeKey:
                        _type = dataNode.GetNodeContentAsString();
                        break;
                    case TransactionTypeKey:
                        _transactionType = dataNode.GetNodeContentAsTransactionType();
                        break;
                    case SubscriptionIDKey:
                        _subscriptionID = dataNode.GetNodeContentAsInt();
                        break;
                    case ProductIDKey:
                        _productID = dataNode.GetNodeContentAsInt();
                        break;
                    case CreatedAtKey:
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
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

        #region IAdjustment Members
        /// <summary>
        /// The ID of the adjustment
        /// </summary>
        public int ID 
        {
            get { return _id; }
        }
        private int _id = int.MinValue;

        /// <summary>
        /// The amount of the adjustment (in cents)
        /// </summary>
        public int AmountInCents
        {
            get { return _amountInCents; }
        }
        private int _amountInCents = int.MinValue;

        /// <summary>
        /// The amount of the adjustment
        /// </summary>
        public decimal Amount
        {
            get { return Convert.ToDecimal(this._amountInCents) / 100; }
        }

        /// <summary>
        /// The subscription balance after the adjustment (in cents)
        /// </summary>
        public int EndingBalanceInCents
        {
            get { return _endingBalanceInCents; }
        }
        private int _endingBalanceInCents = int.MinValue;

        /// <summary>
        /// The subscription balance after the adjustment
        /// </summary>
        public decimal EndingBalance
        {
            get { return Convert.ToDecimal(this._endingBalanceInCents) / 100; }
        }

        /// <summary>
        /// The type of the adjustment
        /// </summary>
        public string Type
        {
            get { return _type; }
        }
        private string _type = string.Empty;

        /// <summary>
        /// The type of transaction done by the adjustment
        /// </summary>
        public TransactionType TransactionType
        {
            get { return _transactionType; }
        }
        private TransactionType _transactionType = TransactionType.Unknown;

        /// <summary>
        /// The subscription the adjustment was created for
        /// </summary>
        public int SubscriptionID
        {
            get { return _subscriptionID; }
        }
        private int _subscriptionID = int.MinValue;

        /// <summary>
        /// The subscribed product at the time of the adjustment
        /// </summary>
        public int ProductID
        {
            get { return _productID; }
        }
        private int _productID = int.MinValue;

        /// <summary>
        /// The date the adjustment was created
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// A helpful explaination for the adjustment
        /// </summary>
        public string Memo
        {
            get { return _memo; }
        }
        private string _memo = string.Empty;

        /// <summary>
        /// (Currently, all adjustments return as successful
        /// </summary>
        public bool Success
        {
            get { return _success; }
        }
        private bool _success = false;

        #endregion

        #region IComparable<IAdjustment>
        /// <summary>
        /// Method for comparing two adjustments
        /// </summary>
        /// <param name="other">The adjustment to compare with</param>
        /// <returns>The comparison value</returns>
        public int CompareTo(IAdjustment other)
        {
            return this._amountInCents.CompareTo(other.AmountInCents);
        }
        #endregion

        #region IComparable<Adjustment>
        /// <summary>
        /// Method for comparing two adjustments
        /// </summary>
        /// <param name="other">The adjustment to compare with</param>
        /// <returns>The comparison value</returns>
        public int CompareTo(Adjustment other)
        {
            return this._amountInCents.CompareTo(other.AmountInCents);
        }
        #endregion
    }
}
