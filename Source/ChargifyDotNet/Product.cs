
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    using Json;
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
        public Product()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productXml">An XML string containing a product node</param>
        public Product(string productXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(productXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(productXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "product")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no customer info was found
            throw new ArgumentException("XML does not contain product information", nameof(productXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productNode">An aml node with product information</param>
        internal Product(XmlNode productNode)
        {
            if (productNode == null) throw new ArgumentNullException(nameof(productNode));
            if (productNode.Name != "product") throw new ArgumentException("Not a vaild product node", nameof(productNode));
            if (productNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(productNode));
            LoadFromNode(productNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productObject">JsonObject containing product info (in expected format)</param>
        public Product(JsonObject productObject)
        {
            if (productObject == null) throw new ArgumentNullException(nameof(productObject));
            if (productObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild product object", nameof(productObject));
            LoadFromJson(productObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing product data</param>
        private void LoadFromJson(JsonObject obj)
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
                        _returnUrl = obj.GetJSONContentAsString(key);
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
                    case "product_version":
                        _productVersion = obj.GetJSONContentAsInt(key);
                        break;
                    case "public_signup_pages":
                        _publicSignupPages = obj.GetJSONContentAsPublicSignupPages(key);
                        break;
                    case "initial_charge_after_trial":
                        _initialChargeAfterTrial = obj.GetJSONContentAsBoolean(key);
                        break;
                    case "update_return_url":
                        _updateReturnUrl = obj.GetJSONContentAsString(key);
                        break;
                    case "update_return_params":
                        _updateReturnParams = obj.GetJSONContentAsString(key);
                        break;
                    case "taxable":
                        _taxable = obj.GetJSONContentAsBoolean(key);
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
                        _returnUrl = dataNode.GetNodeContentAsString();
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
                    case "product_version":
                        _productVersion = dataNode.GetNodeContentAsInt();
                        break;
                    case "public_signup_pages":
                        _publicSignupPages = dataNode.GetNodeContentAsPublicSignupPages();
                        break;
                    case "initial_charge_after_trial":
                        _initialChargeAfterTrial = dataNode.GetNodeContentAsBoolean();
                        break;
                    case "update_return_url":
                        _updateReturnUrl = dataNode.GetNodeContentAsString();
                        break;
                    case "update_return_params":
                        _updateReturnParams = dataNode.GetNodeContentAsString();
                        break;
                    case "taxable":
                        _taxable = dataNode.GetNodeContentAsBoolean();
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
            set
            {
                _priceInCents = value;
            }
        }
        private int _priceInCents;

        /// <summary>
        /// Get the price, in dollars and cents.
        /// </summary>
        public decimal Price
        {
            get
            {
                return Convert.ToDecimal(_priceInCents) / 100;
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
            set
            {
                _name = value;
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
        private int _id;

        /// <summary>
        /// Get the handle to this product
        /// </summary>
        public string Handle
        {
            get
            {
                return _handle;
            }
            set
            {
                _handle = value;
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
            set
            {
                _description = value;
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
        private IProductFamily _productFamily;

        /// <summary>
        /// Get the accounting code for this product
        /// </summary>
        public string AccountingCode
        {
            get
            {
                return _accountingCode;
            }
            set
            {
                _accountingCode = value;
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
            set
            {
                _intervalUnit = value;
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
            set
            {
                _interval = value;
            }
        }
        private int _interval;

        /// <summary>
        /// Get the up front charge you have specified, in cents. 
        /// </summary>
        public int InitialChargeInCents
        {
            get { return _initialChargeInCents; }
            set
            {
                _initialChargeInCents = value;
            }
        }
        private int _initialChargeInCents;

        /// <summary>
        /// Get the up front charge for this product, in dollars and cents.
        /// </summary>
        public decimal InitialCharge
        {
            get
            {
                return Convert.ToDecimal(_initialChargeInCents) / 100;
            }
        }

        /// <summary>
        /// Get the price of the trial period for a subscription to this product, in cents.
        /// </summary>
        public int TrialPriceInCents
        {
            get { return _trialPriceInCents; }
            set
            {
                _trialPriceInCents = value;
            }
        }
        private int _trialPriceInCents;

        /// <summary>
        /// Get the price of the trial period for a subscription to this product, in dollars and cents.
        /// </summary>
        public decimal TrialPrice
        {
            get
            {
                return Convert.ToDecimal(_trialPriceInCents) / 100;
            }
        }

        /// <summary>
        /// A numerical interval for the length of the trial period of a subscription to this product.
        /// </summary>
        public int TrialInterval
        {
            get { return _trialInterval; }
            set
            {
                _trialInterval = value;
            }
        }
        private int _trialInterval;

        /// <summary>
        /// A string representing the trial interval unit for this product, either "month" or "day"
        /// </summary>
        public IntervalUnit TrialIntervalUnit
        {
            get { return _trialIntervalUnit; }
            set
            {
                _trialIntervalUnit = value;
            }
        }
        private IntervalUnit _trialIntervalUnit = IntervalUnit.Unknown;

        /// <summary>
        /// A numerical interval for the length a subscription to this product will run before it expires.
        /// </summary>
        public int ExpirationInterval
        {
            get { return _expirationInterval; }
            set
            {
                _expirationInterval = value;
            }
        }
        private int _expirationInterval;

        /// <summary>
        /// A string representing the expiration interval for this product, either "month" or "day"
        /// </summary>
        public IntervalUnit ExpirationIntervalUnit
        {
            get { return _expirationIntervalUnit; }
            set
            {
                _expirationIntervalUnit = value;
            }
        }
        private IntervalUnit _expirationIntervalUnit = IntervalUnit.Unknown;

        /// <summary>
        /// The URL the buyer is returned to after successful purchase.
        /// </summary>
        public string ReturnURL
        {
            get { return _returnUrl; }
            set
            {
                _returnUrl = value;
            }
        }
        private string _returnUrl = string.Empty;

        /// <summary>
        /// The URL the buyer is returned to after successful purchase.
        /// </summary>
        public string UpdateReturnUrl
        {
            get { return _updateReturnUrl; }
        }
        private string _updateReturnUrl = string.Empty;

        /// <summary>
        /// The parameter string chargify will use in constructing the return URL.
        /// </summary>
        public string ReturnParams
        {
            get { return _returnParams; }
            set
            {
                _returnParams = value;
            }
        }
        private string _returnParams = string.Empty;

        /// <summary>
        /// This product requires a credit card
        /// </summary>
        public bool RequireCreditCard
        {
            get { return _requireCreditCard; }
            set
            {
                _requireCreditCard = value;
            }
        }
        private bool _requireCreditCard;

        /// <summary>
        /// This product requests a credit card
        /// </summary>
        public bool RequestCreditCard
        {
            get { return _requestCreditCard; }
            set
            {
                _requestCreditCard = value;
            }
        }
        private bool _requestCreditCard;

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

        /// <summary>
        /// The product version number
        /// </summary>
        public int ProductVersion
        {
            get { return _productVersion; }
        }
        private int _productVersion = int.MinValue;

        /// <summary>
        /// Is this product taxable?
        /// </summary>
        public bool Taxable { get { return _taxable; } }
        private bool _taxable;

        /// <summary>
        /// Paramters for update
        /// </summary>
        public string UpdateReturnParams { get { return _updateReturnParams; } }
        private string _updateReturnParams = string.Empty;

        /// <summary>
        /// Will the setup/initial charge be processed after the trial?
        /// </summary>
        public bool InitialChargeAfterTrial { get { return _initialChargeAfterTrial; } }
        private bool _initialChargeAfterTrial;
        #endregion

        #region Operators

        /// <summary>
        /// Equals operator for two products
        /// </summary>
        /// <returns>True if the products are equal</returns>
        public static bool operator ==(Product a, Product b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null)) { return false; }

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
            if (ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object) a == null) || (b == null)) { return false; }

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
            if (ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if ((a == null) || ((object) b == null)) { return false; }

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

            if (obj is IProduct)
            {
                return (Handle == ((IProduct) obj).Handle);
            }
            return ReferenceEquals(this, obj);
        }

        /// <summary>
        /// Convert object to a string
        /// </summary>
        /// <returns>The string representation of the object</returns>
        public override string ToString()
        {
            return Name;
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
            return string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
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
            return string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}
