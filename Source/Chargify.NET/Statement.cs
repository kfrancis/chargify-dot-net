
#region License, Terms and Conditions
//
// Statement.cs
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
    using ChargifyNET.Json;
    using System.Diagnostics;
    #endregion

    /// <summary>
    /// The statement object
    /// http://docs.chargify.com/statements
    /// </summary>
    [DebuggerDisplay("ID: {ID}, CreatedAt: {CreatedAt}, ClosedAt: {ClosedAt}, SettledAt: {SettledAt}")]
    public class Statement : ChargifyBase, IStatement, IComparable<Statement>
    {
        #region Field Keys
        private const string BasicHtmlViewKey = "basic_html_view";
        private const string ClosedAtKey = "closed_at";
        private const string CreatedAtKey = "created_at";
        private const string HtmlViewKey = "html_view";
        private const string IDKey = "id";
        private const string OpenedAtKey = "opened_at";
        private const string SettledAtKey = "settled_at";
        private const string SubscriptionIDKey = "subscription_id";
        private const string TextViewKey = "text_view";
        private const string UpdatedAtKey = "updated_at";
        private const string FuturePaymentsKey = "future_payments";
        private const string StartingBalanceKey = "starting_balance_in_cents";
        private const string EndingBalanceKey = "ending_balance_in_cents";
        private const string EventsKey = "events";
        private const string CustomerFirstNameKey = "customer_first_name";
        private const string CustomerLastNameKey = "customer_last_name";
        private const string CustomerOrganizationKey = "customer_organization";
        private const string CustomerShippingAddressKey = "customer_shipping_address";
        private const string CustomerShippingAddress2Key = "customer_shipping_address_2";
        private const string CustomerShippingCityKey = "customer_shipping_city";
        private const string CustomerShippingStateKey = "customer_shipping_state";
        private const string CustomerShippingCountryKey = "customer_shipping_country";
        private const string CustomerShippingZipKey = "customer_shipping_zip";
        private const string CustomerBillingAddressKey = "customer_billing_address";
        private const string CustomerBillingAddress2Key = "customer_billing_address_2";
        private const string CustomerBillingCityKey = "customer_billing_city";
        private const string CustomerBillingStateKey = "customer_billing_state";
        private const string CustomerBillingCountryKey = "customer_billing_country";
        private const string CustomerBillingZipKey = "customer_billing_zip";
        private const string TrasactionsKey = "transactions";
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Statement() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="StatementXML">XML containing statement info (in expected format)</param>
        public Statement(string StatementXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(StatementXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "StatementXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == "statement")
                {
                    this.LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain charge information", "StatementXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statementNode">XML containing statement info (in expected format)</param>
        internal Statement(XmlNode statementNode)
            : base()
        {
            if (statementNode == null) throw new ArgumentNullException("statementNode");
            if (statementNode.Name != "statement") throw new ArgumentException("Not a vaild statement node", "statementNode");
            if (statementNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "statementNode");
            this.LoadFromNode(statementNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statementObject">Json containing statement info (in expected format)</param>
        public Statement(JsonObject statementObject) : base()
        {
            if (statementObject == null) throw new ArgumentNullException("statementObject");
            if (statementObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild statement object", "statementObject");
            this.LoadFromJSON(statementObject);
        }

        /// <summary>
        /// Loads the values for this object from the Json
        /// </summary>
        /// <param name="obj">The JsonObject to retrieve the values from</param>
        private void LoadFromJSON(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case BasicHtmlViewKey:
                        _basicHtmlView = obj.GetJSONContentAsString(key);
                        break;
                    case ClosedAtKey:
                        _closedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case CreatedAtKey:
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case HtmlViewKey:
                        _htmlView = obj.GetJSONContentAsString(key);
                        break;
                    case IDKey:
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case OpenedAtKey:
                        _openedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case SettledAtKey:
                        _settledAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case SubscriptionIDKey:
                        _subscriptionID = obj.GetJSONContentAsInt(key);
                        break;
                    case TextViewKey:
                        _textView = obj.GetJSONContentAsString(key);
                        break;
                    case UpdatedAtKey:
                        _updatedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case FuturePaymentsKey:
                        // TODO: Correct this when the output is corrected
                        _futurePayments = (object)obj.GetJSONContentAsString(key);
                        break;
                    case StartingBalanceKey:
                        _startingBalanceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case EndingBalanceKey:
                        _endingBalanceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case EventsKey:
                        // TODO: Correct this when the output is corrected
                        _events = (object)obj.GetJSONContentAsString(key);
                        break;
                    case CustomerFirstNameKey:
                        _customerFirstName = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerLastNameKey:
                        _customerLastName = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerOrganizationKey:
                        _customerOrganization = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerShippingAddressKey:
                        _customerShippingAddress = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerShippingAddress2Key:
                        _customerShippingAddress2 = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerShippingCityKey:
                        _customerShippingCity = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerShippingStateKey:
                        _customerShippingState = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerShippingCountryKey:
                        _customerShippingCountry = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerShippingZipKey:
                        _customerShippingZip = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerBillingAddressKey:
                        _customerBillingAddress = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerBillingAddress2Key:
                        _customerBillingAddress2 = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerBillingCityKey:
                        _customerBillingCity = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerBillingStateKey:
                        _customerBillingState = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerBillingCountryKey:
                        _customerBillingCountry = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerBillingZipKey:
                        _customerBillingZip = obj.GetJSONContentAsString(key);
                        break;
                    case TrasactionsKey:
                        _transactions = new List<ITransaction>();
                        JsonArray transactionsArray = obj[key] as JsonArray;
                        if (transactionsArray != null)
                        {
                            foreach (JsonObject transaction in transactionsArray.Items)
                            {
                                _transactions.Add(new Transaction(transaction));
                            }
                        }
                        // Sanity check, should be equal.
                        if (transactionsArray.Length != _transactions.Count)
                        {
                            throw new JsonParseException(string.Format("Unable to parse transactions ({0} != {1})", transactionsArray.Length, _transactions.Count));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a statement node
        /// </summary>
        /// <param name="statementNode">The statement node</param>
        private void LoadFromNode(XmlNode statementNode)
        {
            foreach (XmlNode dataNode in statementNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case BasicHtmlViewKey:
                        _basicHtmlView = dataNode.GetNodeContentAsString();
                        break;
                    case ClosedAtKey:
                        _closedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CreatedAtKey:
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case HtmlViewKey:
                        _htmlView = dataNode.GetNodeContentAsString();
                        break;
                    case IDKey:
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case OpenedAtKey:
                        _openedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case SettledAtKey:
                        _settledAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case SubscriptionIDKey:
                        _subscriptionID = dataNode.GetNodeContentAsInt();
                        break;
                    case TextViewKey:
                        _textView = dataNode.GetNodeContentAsString();
                        break;
                    case UpdatedAtKey:
                        _updatedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case FuturePaymentsKey:
                        // TODO: Correct this when the output is corrected
                        _futurePayments = (object)dataNode.GetNodeContentAsString();
                        break;
                    case StartingBalanceKey:
                        _startingBalanceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case EndingBalanceKey:
                        _endingBalanceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case EventsKey:
                        // TODO: Correct this when the output is corrected
                        _events = (object)dataNode.GetNodeContentAsString();
                        break;
                    case CustomerFirstNameKey:
                        _customerFirstName = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerLastNameKey:
                        _customerLastName = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerOrganizationKey:
                        _customerOrganization = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerShippingAddressKey:
                        _customerShippingAddress = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerShippingAddress2Key:
                        _customerShippingAddress2 = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerShippingCityKey:
                        _customerShippingCity = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerShippingStateKey:
                        _customerShippingState = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerShippingCountryKey:
                        _customerShippingCountry = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerShippingZipKey:
                        _customerShippingZip = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerBillingAddressKey:
                        _customerBillingAddress = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerBillingAddress2Key:
                        _customerBillingAddress2 = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerBillingCityKey:
                        _customerBillingCity = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerBillingStateKey:
                        _customerBillingState = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerBillingCountryKey:
                        _customerBillingCountry = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerBillingZipKey:
                        _customerBillingZip = dataNode.GetNodeContentAsString();
                        break;
                    case TrasactionsKey:
                        _transactions = new List<ITransaction>();
                        foreach (XmlNode childNode in dataNode.ChildNodes)
                        {
                            switch (childNode.Name)
                            {
                                case "transaction":
                                    _transactions.Add(childNode.GetNodeContentAsTransaction());
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region IStatement Members

        /// <summary>
        /// The unique identifier for this statement within Chargify
        /// </summary>
        public int ID
        {
            get { return _id; }
        }
        private int _id = int.MinValue;

        /// <summary>
        /// The unique identifier of the subscription associated with this statement
        /// </summary>
        public int SubscriptionID
        {
            get { return _subscriptionID; }
        }
        private int _subscriptionID = int.MinValue;

        /// <summary>
        /// The date that the statement was opened
        /// </summary>
        public DateTime OpenedAt
        {
            get { return _openedAt; }
        }
        private DateTime _openedAt = DateTime.MinValue;

        /// <summary>
        /// The date that the statement was closed
        /// </summary>
        public DateTime ClosedAt
        {
            get { return _closedAt; }
        }
        private DateTime _closedAt = DateTime.MinValue;

        /// <summary>
        /// The date that the statement was settled
        /// </summary>
        public DateTime SettledAt
        {
            get { return _settledAt; }
        }
        private DateTime _settledAt = DateTime.MinValue;

        /// <summary>
        /// A text representation of the statement
        /// </summary>
        public string TextView
        {
            get { return _textView; }
        }
        private string _textView = string.Empty;

        /// <summary>
        /// A simple HTML representation of the statement
        /// </summary>
        public string BasicHtmlView
        {
            get { return _basicHtmlView; }
        }
        private string _basicHtmlView = string.Empty;

        /// <summary>
        /// A more rebust HTML representation of the statment
        /// </summary>
        public string HtmlView
        {
            get { return _htmlView; }
        }
        private string _htmlView = string.Empty;

        /// <summary>
        /// A collection of payments from future sttments that pay charges on this statement
        /// </summary>
        public object FuturePayments
        {
            get { return _futurePayments; }
        }
        private object _futurePayments = null;

        /// <summary>
        /// The subscription's balance at the time the statement was opened (in cents)
        /// </summary>
        public int StartingBalanceInCents
        {
            get { return _startingBalanceInCents; }
        }
        private int _startingBalanceInCents = int.MinValue;

        /// <summary>
        /// The subscription's balance at the time the statement was opened (in dollars and cents)
        /// </summary>
        public decimal StartingBalance
        {
            get { return Convert.ToDecimal(this._startingBalanceInCents) / 100; }
        }

        /// <summary>
        /// The subscription's balance at the time the statement was closed (in cents)
        /// </summary>
        public int EndingBalanceInCents
        {
            get { return _endingBalanceInCents; }
        }
        private int _endingBalanceInCents = int.MinValue;

        /// <summary>
        /// The subscription's balance at the time the statement was closed (in dollars and cents)
        /// </summary>
        public decimal EndingBalance
        {
            get { return Convert.ToDecimal(this._endingBalanceInCents) / 100; }
        }

        /// <summary>
        /// The customer's first name
        /// </summary>
        public string CustomerFirstName
        {
            get { return _customerFirstName; }
        }
        private string _customerFirstName = string.Empty;

        /// <summary>
        /// The customer's last name
        /// </summary>
        public string CustomerLastName
        {
            get { return _customerLastName; }
        }
        private string _customerLastName = string.Empty;

        /// <summary>
        /// The customer's organization
        /// </summary>
        public string CustomerOrganization
        {
            get { return _customerOrganization; }
        }
        private string _customerOrganization = string.Empty;

        /// <summary>
        /// The customer's shipping address, line 1
        /// </summary>
        public string CustomerShippingAddress
        {
            get { return _customerShippingAddress; }
        }
        private string _customerShippingAddress = string.Empty;

        /// <summary>
        /// The customer's shipping address, line 2
        /// </summary>
        public string CustomerShippingAddress2
        {
            get { return _customerShippingAddress2; }
        }
        private string _customerShippingAddress2 = string.Empty;

        /// <summary>
        /// The customer's shipping city
        /// </summary>
        public string CustomerShippingCity
        {
            get { return _customerShippingCity; }
        }
        private string _customerShippingCity = string.Empty;

        /// <summary>
        /// The customer's shipping state or province
        /// </summary>
        public string CustomerShippingState
        {
            get { return _customerShippingState; }
        }
        private string _customerShippingState = string.Empty;

        /// <summary>
        /// The customer's shipping country
        /// </summary>
        public string CustomerShippingCountry
        {
            get { return _customerShippingCountry; }
        }
        private string _customerShippingCountry = string.Empty;

        /// <summary>
        /// The customer's shipping postal code or zip
        /// </summary>
        public string CustomerShippingZip
        {
            get { return _customerShippingZip; }
        }
        private string _customerShippingZip = string.Empty;

        /// <summary>
        /// The customer's billing address, line 1
        /// </summary>
        public string CustomerBillingAddress
        {
            get { return _customerBillingAddress; }
        }
        private string _customerBillingAddress = string.Empty;

        /// <summary>
        /// The customer's billing address, line 2
        /// </summary>
        public string CustomerBillingAddress2
        {
            get { return _customerBillingAddress2; }
        }
        private string _customerBillingAddress2 = string.Empty;

        /// <summary>
        /// The customer's billing city
        /// </summary>
        public string CustomerBillingCity
        {
            get { return _customerBillingCity; }
        }
        private string _customerBillingCity = string.Empty;

        /// <summary>
        /// The customer's billing state or province
        /// </summary>
        public string CustomerBillingState
        {
            get { return _customerBillingState; }
        }
        private string _customerBillingState = string.Empty;

        /// <summary>
        /// The customer's billing country
        /// </summary>
        public string CustomerBillingCountry
        {
            get { return _customerBillingCountry; }
        }
        private string _customerBillingCountry = string.Empty;

        /// <summary>
        /// The customer's billing postal code or zip
        /// </summary>
        public string CustomerBillingZip
        {
            get { return _customerBillingZip; }
        }
        private string _customerBillingZip = string.Empty;

        /// <summary>
        /// A collection of the transactions associated with the statement
        /// </summary>
        public List<ITransaction> Transactions
        {
            get { return _transactions; }
        }
        private List<ITransaction> _transactions = new List<ITransaction>();

        /// <summary>
        /// A collection of the events associated with the statement
        /// </summary>
        public object Events
        {
            get { return _events; }
        }
        private object _events = null;

        /// <summary>
        /// The creation date for this statement
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// The date of last update for this statement
        /// </summary>
        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
        }
        private DateTime _updatedAt = DateTime.MinValue;

        #endregion

        #region IComparable<IStatement> Members

        /// <summary>
        /// Method for comparing this IStatement object to another (using ID)
        /// </summary>
        /// <param name="other">The other IStatement object to compare against.</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(IStatement other)
        {
            return this.ID.CompareTo(other.ID);
        }

        #endregion

        #region IComparable<Statement> Members

        /// <summary>
        /// Method for comparing this Statement object to another (using ID)
        /// </summary>
        /// <param name="other">The other Statement object to compare against.</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(Statement other)
        {
            return this.ID.CompareTo(other.ID);
        }

        #endregion

    }
}
