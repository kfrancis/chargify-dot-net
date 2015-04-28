
#region License, Terms and Conditions
//
// Product.cs
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
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Class representing a product.  Subscriptions will subscribe to a product
    /// </summary>
    [DebuggerDisplay("Name: {ProductFamily.Name}->{Name}, Price: {PriceInCents}, Interval: {Interval}, Handle: {Handle}, ID: {ID}")]
    public class Product : ChargifyBase, IProduct, IComparable<Product>
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Product() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ProductXML">An XML string containing a product node</param>
        public Product(string ProductXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(ProductXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "ProductXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == "product")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no customer info was found
            throw new ArgumentException("XML does not contain product information", "ProductXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productNode">An aml node with product information</param>
        internal Product(XmlNode productNode)
            : base()
        {
            if (productNode == null) throw new ArgumentNullException("productNode");
            if (productNode.Name != "product") throw new ArgumentException("Not a vaild product node", "productNode");
            if (productNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "productNode");
            LoadFromNode(productNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productObject">JsonObject containing product info (in expected format)</param>
        public Product(JsonObject productObject)
            : base()
        {
            if (productObject == null) throw new ArgumentNullException("productObject");
            if (productObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild product object", "productObject");
            this.LoadFromJSON(productObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing product data</param>
        private void LoadFromJSON(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case "id":
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case "name":
                        _name = obj.GetJSONContentAsString(key);
                        break;
                    case "handle":
                        _handle = obj.GetJSONContentAsString(key);
                        break;
                    case "accounting_code":
                        _accountingCode = obj.GetJSONContentAsString(key);
                        break;
                    case "description":
                        _description = obj.GetJSONContentAsString(key);
                        break;
                    case "interval":
                        _interval = obj.GetJSONContentAsInt(key);
                        break;
                    case "interval_unit":
                        _intervalUnit = obj.GetJSONContentAsIntervalUnit(key);
                        break;
                    case "price_in_cents":
                        _priceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case "product_family":
                        // get the sub section "product family" for this product
                        _productFamily = obj.GetJSONContentAsProductFamily(key);
                        break;
                    case "initial_charge_in_cents":
                        _initialChargeInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case "trial_price_in_cents":
                        _trialPriceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case "trial_interval":
                        _trialInterval = obj.GetJSONContentAsInt(key);
                        break;
                    case "trial_interval_unit":
                        _trialIntervalUnit = obj.GetJSONContentAsIntervalUnit(key);
                        break;
                    case "expiration_interval":
                        _expirationInterval = obj.GetJSONContentAsInt(key);
                        break;
                    case "expiration_interval_unit":
                        _expirationIntervalUnit = obj.GetJSONContentAsIntervalUnit(key);
                        break;
                    case "return_url":
                        _returnURL = obj.GetJSONContentAsString(key);
                        break;
                    case "return_params":
                        _returnParams = obj.GetJSONContentAsString(key);
                        break;
                    case "require_credit_card":
                        _requireCreditCard = obj.GetJSONContentAsBoolean(key);
                        break;
                    case "request_credit_card":
                        _requestCreditCard = obj.GetJSONContentAsBoolean(key);
                        break;
                    case "created_at":
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case "updated_at":
                        _updatedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case "archived_at":
                        _archivedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case "public_signup_pages":
                        if (_publicSignupPages == null)
                        {
                            _publicSignupPages = new List<IPublicSignupPage>();
                        }
                        JsonArray publicSignupPagesArray = obj[key] as JsonArray;
                        if (publicSignupPagesArray != null)
                        {
                            foreach (JsonObject publicSignupPage in publicSignupPagesArray.Items)
                            {
                                _publicSignupPages.Add(new PublicSignupPage(publicSignupPage));
                            }
                        }
                        // Sanity check, should be equal.
                        if (publicSignupPagesArray.Length != _publicSignupPages.Count)
                        {
                            throw new JsonParseException(string.Format("Unable to parse public signup pages ({0} != {1})", publicSignupPagesArray.Length, _publicSignupPages.Count));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadFromNode(XmlNode productNode)
        {
            // loop through the nodes to get product info
            foreach (XmlNode dataNode in productNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case "id":
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case "name":
                        _name = dataNode.GetNodeContentAsString();
                        break;
                    case "handle":
                        _handle = dataNode.GetNodeContentAsString();
                        break;
                    case "accounting_code":
                        _accountingCode = dataNode.GetNodeContentAsString();
                        break;
                    case "description":
                        _description = dataNode.GetNodeContentAsString();
                        break;
                    case "interval":
                        _interval = dataNode.GetNodeContentAsInt();
                        break;
                    case "interval_unit":
                        _intervalUnit = dataNode.GetNodeContentAsIntervalUnit();
                        break;
                    case "price_in_cents":
                        _priceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case "product_family":
                        // get the sub section "product family" for this product
                        _productFamily = dataNode.GetNodeContentAsProductFamily();
                        break;
                    case "initial_charge_in_cents":
                        _initialChargeInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case "trial_price_in_cents":
                        _trialPriceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case "trial_interval":
                        _trialInterval = dataNode.GetNodeContentAsInt();
                        break;
                    case "trial_interval_unit":
                        _trialIntervalUnit = dataNode.GetNodeContentAsIntervalUnit();
                        break;
                    case "expiration_interval":
                        _expirationInterval = dataNode.GetNodeContentAsInt();
                        break;
                    case "expiration_interval_unit":
                        _expirationIntervalUnit = dataNode.GetNodeContentAsIntervalUnit();
                        break;
                    case "return_url":
                        _returnURL = dataNode.GetNodeContentAsString();
                        break;
                    case "return_params":
                        _returnParams = dataNode.GetNodeContentAsString();
                        break;
                    case "require_credit_card":
                        _requireCreditCard = dataNode.GetNodeContentAsBoolean();
                        break;
                    case "request_credit_card":
                        _requestCreditCard = dataNode.GetNodeContentAsBoolean();
                        break;
                    case "created_at":
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case "updated_at":
                        _updatedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case "archived_at":
                        _archivedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case "public_signup_pages":
                        if (_publicSignupPages == null)
                        {
                            _publicSignupPages = new List<IPublicSignupPage>();
                        }
                        foreach (XmlNode childNode in dataNode.ChildNodes)
                        {
                            switch (childNode.Name)
                            {
                                case "public_signup_page":
                                    _publicSignupPages.Add(childNode.GetNodeContentAsPublicSignupPage());
                                    break;
                                default:
                                    break;
                            }
                        }
                        // Sanity check, should be equal.
                        if (dataNode.ChildNodes.Count != _publicSignupPages.Count)
                        {
                            throw new JsonParseException(string.Format("Unable to parse public signup pages ({0} != {1})", dataNode.ChildNodes.Count, _publicSignupPages.Count));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region IProduct Members

        /// <summary>
        /// Get the price (in cents)
        /// </summary>
        public int PriceInCents
        {
            get
            {
                return _priceInCents;
            }
        }
        private int _priceInCents = 0;

        /// <summary>
        /// Get the price, in dollars and cents.
        /// </summary>
        public decimal Price
        {
            get
            {
                return Convert.ToDecimal(this._priceInCents) / 100;
            }
        }

        /// <summary>
        /// Get the name of this product
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
        private string _name = string.Empty;

        /// <summary>
        /// The ID of the product
        /// </summary>
        public int ID
        {
            get { return _id; }
        }
        private int _id = 0;

        /// <summary>
        /// Get the handle to this product
        /// </summary>
        public string Handle
        {
            get
            {
                return _handle;
            }
        }
        private string _handle = string.Empty;

        /// <summary>
        /// Get the description of the product
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
        }
        private string _description = string.Empty;

        /// <summary>
        /// Get the product family for this product
        /// </summary>
        public IProductFamily ProductFamily
        {
            get
            {
                return _productFamily;
            }
        }
        private IProductFamily _productFamily = null;

        /// <summary>
        /// Get the accounting code for this product
        /// </summary>
        public string AccountingCode
        {
            get
            {
                return _accountingCode;
            }
        }
        private string _accountingCode = string.Empty;

        /// <summary>
        /// Get the interval unit (day, month) for this product
        /// </summary>
        public IntervalUnit IntervalUnit
        {
            get
            {
                return _intervalUnit;
            }
        }
        private IntervalUnit _intervalUnit = IntervalUnit.Unknown;

        /// <summary>
        /// Get the renewal interval for this product
        /// </summary>
        public int Interval
        {
            get
            {
                return _interval;
            }
        }
        private int _interval = 0;

        /// <summary>
        /// Get the up front charge you have specified, in cents. 
        /// </summary>
        public int InitialChargeInCents
        {
            get { return _initialChargeInCents; }
        }
        private int _initialChargeInCents = 0;

        /// <summary>
        /// Get the up front charge for this product, in dollars and cents.
        /// </summary>
        public decimal InitialCharge
        {
            get
            {
                return Convert.ToDecimal(this._initialChargeInCents) / 100;
            }
        }

        /// <summary>
        /// Get the price of the trial period for a subscription to this product, in cents.
        /// </summary>
        public int TrialPriceInCents
        {
            get { return _trialPriceInCents; }
        }
        private int _trialPriceInCents = 0;

        /// <summary>
        /// Get the price of the trial period for a subscription to this product, in dollars and cents.
        /// </summary>
        public decimal TrialPrice
        {
            get
            {
                return Convert.ToDecimal(this._trialPriceInCents) / 100; ;
            }
        }

        /// <summary>
        /// A numerical interval for the length of the trial period of a subscription to this product.
        /// </summary>
        public int TrialInterval
        {
            get { return _trialInterval; }
        }
        private int _trialInterval = 0;

        /// <summary>
        /// A string representing the trial interval unit for this product, either "month" or "day"
        /// </summary>
        public IntervalUnit TrialIntervalUnit
        {
            get { return _trialIntervalUnit; }
        }
        private IntervalUnit _trialIntervalUnit = IntervalUnit.Unknown;

        /// <summary>
        /// A numerical interval for the length a subscription to this product will run before it expires.
        /// </summary>
        public int ExpirationInterval
        {
            get { return 2; }
        }
        private int _expirationInterval = 0;

        /// <summary>
        /// A string representing the expiration interval for this product, either "month" or "day"
        /// </summary>
        public IntervalUnit ExpirationIntervalUnit
        {
            get { return _expirationIntervalUnit; }
        }
        private IntervalUnit _expirationIntervalUnit = IntervalUnit.Unknown;

        /// <summary>
        /// The URL the buyer is returned to after successful purchase.
        /// </summary>
        public string ReturnURL
        {
            get { return _returnURL; }
        }
        private string _returnURL = string.Empty;

        /// <summary>
        /// The parameter string chargify will use in constructing the return URL.
        /// </summary>
        public string ReturnParams
        {
            get { return _returnParams; }
        }
        private string _returnParams = string.Empty;

        /// <summary>
        /// This product requires a credit card
        /// </summary>
        public bool RequireCreditCard
        {
            get { return _requireCreditCard; }
        }
        private bool _requireCreditCard = false;

        /// <summary>
        /// This product requests a credit card
        /// </summary>
        public bool RequestCreditCard
        {
            get { return _requestCreditCard; }
        }
        private bool _requestCreditCard = false;

        /// <summary>
        /// Timestamp indicating when this product was created.
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// Timestamp indicating when this product was updated.
        /// </summary>
        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
        }
        private DateTime _updatedAt = DateTime.MinValue;

        /// <summary>
        /// Timestamp indicating when this product was updated.
        /// </summary>
        public DateTime ArchivedAt
        {
            get { return _archivedAt; }
        }
        private DateTime _archivedAt = DateTime.MinValue;

        /// <summary>
        /// List of public signup page URLs and the associated ID
        /// </summary>
        public List<IPublicSignupPage> PublicSignupPages { get { return _publicSignupPages; } }
        private List<IPublicSignupPage> _publicSignupPages = new List<IPublicSignupPage>();
        #endregion

        #region Operators

        /// <summary>
        /// Equals operator for two products
        /// </summary>
        /// <returns>True if the products are equal</returns>
        public static bool operator ==(Product a, Product b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            return (a.Handle == b.Handle);
        }

        /// <summary>
        /// Unequal operator for two products
        /// </summary>
        /// <returns>True if the products are unequal</returns>
        public static bool operator !=(Product a, Product b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Equals operator for two products
        /// </summary>
        /// <returns>True if the products are equal</returns>
        public static bool operator ==(Product a, IProduct b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            return (a.Handle == b.Handle);
        }

        /// <summary>
        /// Unequals operator for two products
        /// </summary>
        /// <returns>True if the products are unequal</returns>
        public static bool operator !=(Product a, IProduct b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Equals operator for two products
        /// </summary>
        /// <returns>True if the products are equal</returns>
        public static bool operator ==(IProduct a, Product b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            return (a.Handle == b.Handle);
        }

        /// <summary>
        /// Unequal operator for two products
        /// </summary>
        /// <returns>True if the products are unequal</returns>
        public static bool operator !=(IProduct a, Product b)
        {
            return !(a == b);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Get Hash code
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (typeof(IProduct).IsAssignableFrom(obj.GetType()))
            {
                return (this.Handle == (obj as IProduct).Handle);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Convert object to a string
        /// </summary>
        /// <returns>The string representation of the object</returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region IComparable<IProductFamily> Members

        /// <summary>
        /// Compare this instance to another (by FullName)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(IProductFamily other)
        {
            return this.Name.CompareTo(other.Name);
        }

        #endregion

        #region IComparable<Product> Members

        /// <summary>
        /// Compare this instance to another (by FullName)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(Product other)
        {
            return this.Name.CompareTo(other.Name);
        }

        #endregion
    }
}
