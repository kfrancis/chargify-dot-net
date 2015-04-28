
#region License, Terms and Conditions
//
// Coupon.cs
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
    using ChargifyNET.Json;
    #endregion

    /// <summary>
    /// Object representing coupon in Chargify
    /// </summary>
    public class Coupon : ChargifyBase, ICoupon, IComparable<Coupon>
    {
        #region Field Keys
        private const string AmountInCentsKey = "amount_in_cents";
        private const string CodeKey = "code";
        private const string CreatedAtKey = "created_at";
        private const string DescriptionKey = "description";
        private const string EndDateKey = "end_date";
        private const string IDKey = "id";
        private const string NameKey = "name";
        private const string ProductFamilyIDKey = "product_family_id";
        private const string StartDateKey = "start_date";
        private const string UpdatedAtKey = "updated_at";
        private const string DurationIntervalKey = "duration_interval";
        private const string DurationIntervalUnitKey = "duration_interval_unit";
        private const string DurationPeriodCountKey = "duration_period_count";
        private const string PercentageKey = "percentage";
        private const string RecurringKey = "recurring";
        private const string ArchivedAtKey = "archived_at";
        private const string AllowNegativeBalanceKey = "allow_negative_balance";
        private const string AmountKey = "amount";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Coupon()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CouponXML">XML containing coupon info (in expected format)</param>
        public Coupon(string CouponXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(CouponXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "CouponXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == "coupon")
                {
                    this.LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain coupon information", "CouponXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="couponNode">XML containing coupon info (in expected format)</param>
        internal Coupon(XmlNode couponNode)
            : base()
        {
            if (couponNode == null) throw new ArgumentNullException("CouponNode");
            if (couponNode.Name != "coupon") throw new ArgumentException("Not a vaild coupon node", "couponNode");
            if (couponNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "couponNode");
            this.LoadFromNode(couponNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="couponObject">JsonObject containing coupon info (in expected format)</param>
        public Coupon(JsonObject couponObject)
            : base()
        {
            if (couponObject == null) throw new ArgumentNullException("couponObject");
            if (couponObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild coupon object", "couponObject");
            this.LoadFromJSON(couponObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing coupon data</param>
        private void LoadFromJSON(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get coupon info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case AmountInCentsKey:
                        _amountInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case CodeKey:
                        _code = obj.GetJSONContentAsString(key);
                        break;
                    case CreatedAtKey:
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case DescriptionKey:
                        _description = obj.GetJSONContentAsString(key);
                        break;
                    case EndDateKey:
                        _endDate = obj.GetJSONContentAsDateTime(key);
                        break;
                    case IDKey:
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case NameKey:
                        _name = obj.GetJSONContentAsString(key);
                        break;
                    case ProductFamilyIDKey:
                        _productFamilyID = obj.GetJSONContentAsInt(key);
                        break;
                    case StartDateKey:
                        _startDate = obj.GetJSONContentAsDateTime(key);
                        break;
                    case UpdatedAtKey:
                        _updatedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case DurationIntervalKey:
                        _durationInterval = obj.GetJSONContentAsInt(key);
                        break;
                    case DurationIntervalUnitKey:
                        _durationUnit = obj.GetJSONContentAsString(key);
                        break;
                    case DurationPeriodCountKey:
                        _durationPeriodCount = obj.GetJSONContentAsInt(key);
                        break;
                    case PercentageKey:
                        _percentage = obj.GetJSONContentAsInt(key);
                        break;
                    case RecurringKey:
                        _isRecurring = obj.GetJSONContentAsBoolean(key);
                        break;
                    case ArchivedAtKey:
                        _archivedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case AllowNegativeBalanceKey:
                        _allowNegativeBalance = obj.GetJSONContentAsBoolean(key);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a coupon node
        /// </summary>
        /// <param name="couponNode">The coupon node</param>
        private void LoadFromNode(XmlNode couponNode)
        {
            foreach (XmlNode dataNode in couponNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case AmountInCentsKey:
                        _amountInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case CodeKey:
                        _code = dataNode.GetNodeContentAsString();
                        break;
                    case CreatedAtKey:
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case DescriptionKey:
                        _description = dataNode.GetNodeContentAsString();
                        break;
                    case EndDateKey:
                        _endDate = dataNode.GetNodeContentAsDateTime();
                        break;
                    case IDKey:
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case NameKey:
                        _name = dataNode.GetNodeContentAsString();
                        break;
                    case ProductFamilyIDKey:
                        _productFamilyID = dataNode.GetNodeContentAsInt();
                        break;
                    case StartDateKey:
                        _startDate = dataNode.GetNodeContentAsDateTime();
                        break;
                    case UpdatedAtKey:
                        _updatedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case DurationIntervalKey:
                        _durationInterval = dataNode.GetNodeContentAsInt();
                        break;
                    case DurationIntervalUnitKey:
                        _durationUnit = dataNode.GetNodeContentAsString();
                        break;
                    case DurationPeriodCountKey:
                        _durationPeriodCount = dataNode.GetNodeContentAsInt();
                        break;
                    case PercentageKey:
                        _percentage = dataNode.GetNodeContentAsInt();
                        break;
                    case RecurringKey:
                        _isRecurring = dataNode.GetNodeContentAsBoolean();
                        break;
                    case ArchivedAtKey:
                        _archivedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case AllowNegativeBalanceKey:
                        _allowNegativeBalance = dataNode.GetNodeContentAsBoolean();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region ICoupon Members
        /// <summary>
        /// The amount of the coupon, in cents.
        /// </summary>
        public int AmountInCents
        {
            get { return _amountInCents; }
            set { _amountInCents = value; }
        }
        private int _amountInCents = int.MinValue;

        /// <summary>
        /// The amount of the coupon
        /// </summary>
        public decimal Amount
        {
            get { return Convert.ToDecimal(_amountInCents) / 100; }
            set { _amountInCents = Convert.ToInt32(value * 100); }
        }

        /// <summary>
        /// The string code that represents this coupon
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        private string _code = string.Empty;

        /// <summary>
        /// The date this coupon was created
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// The description of this coupon
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _description = string.Empty;

        /// <summary>
        /// The date that this coupon is no longer valid for use
        /// </summary>
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
        private DateTime _endDate = DateTime.MinValue;

        /// <summary>
        /// The ID of this coupon
        /// </summary>
        public int ID
        {
            get { return _id; }
        }
        private int _id = int.MinValue;

        /// <summary>
        /// The internal name of this coupon in the Chargify site
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _name = string.Empty;

        /// <summary>
        /// The ID of the product family this coupon belongs to
        /// </summary>
        public int ProductFamilyID
        {
            get { return _productFamilyID; }
        }
        private int _productFamilyID = int.MinValue;

        /// <summary>
        /// The date this coupon became active
        /// </summary>
        public DateTime StartDate
        {
            get { return _startDate; }
        }
        private DateTime _startDate = DateTime.MinValue;

        /// <summary>
        /// The date this coupon was last updated
        /// </summary>
        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
        }
        private DateTime _updatedAt = DateTime.MinValue;

        /// <summary>
        /// The coupon duration interval
        /// </summary>
        public int DurationInterval
        {
            get { return _durationInterval; }
            set { _durationInterval = value; }
        }
        private int _durationInterval = int.MinValue;

        /// <summary>
        /// The coupon duration unit 
        /// </summary>
        public string DurationUnit
        {
            get { return _durationUnit; }
            set { _durationUnit = value; }
        }
        private string _durationUnit = string.Empty;

        /// <summary>
        /// The coupon period count
        /// </summary>
        public int DurationPeriodCount
        {
            get { return _durationPeriodCount; }
            set { _durationPeriodCount = value; }
        }
        private int _durationPeriodCount = int.MinValue;

        /// <summary>
        /// If percentage based, the percentage. Int.MinValue otherwise.
        /// </summary>
        public int Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
        }
        private int _percentage = int.MinValue;

        /// <summary>
        /// Is this a recurring coupon?
        /// </summary>
        public bool IsRecurring
        {
            get { return _isRecurring; }
            set { _isRecurring = value; }
        }
        private bool _isRecurring = false;

        /// <summary>
        ///  The date this coupon was archived
        /// </summary>
        public DateTime ArchivedAt { get { return _archivedAt; } }
        private DateTime _archivedAt = DateTime.MinValue;

        /// <summary>
        /// Allow negative balance
        /// </summary>
        public bool AllowNegativeBalance
        {
            get { return _allowNegativeBalance; }
            set { _allowNegativeBalance = value; }
        }
        private bool _allowNegativeBalance = false;

        #endregion

        #region IComparable<ICoupon> Members

        /// <summary>
        /// Method for comparing one coupon to another
        /// </summary>
        public int CompareTo(ICoupon other)
        {
            return this.AmountInCents.CompareTo(other.AmountInCents);
        }

        #endregion

        #region IComparable<Coupon> Members

        /// <summary>
        /// Method for comparing one coupon to another
        /// </summary>
        public int CompareTo(Coupon other)
        {
            return this.AmountInCents.CompareTo(other.AmountInCents);
        }
        #endregion
    }
}
