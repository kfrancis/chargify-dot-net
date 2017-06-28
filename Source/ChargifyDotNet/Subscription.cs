
#region License, Terms and Conditions
//
// Subscription.cs
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

using System;
using System.Diagnostics;
using System.Text;
using System.Xml;
using ChargifyNET.Json;

namespace ChargifyNET
{
    #region Imports



    #endregion

    /// <summary>
    /// Class representing am existing Chargify subscription
    /// </summary>
    [DebuggerDisplay("ID: {SubscriptionID}, Customer: {Customer.FullName}, Product: {Product.ProductFamily.Name}->{Product.Name}")]
    public class Subscription : ChargifyBase, ISubscription, IComparable<Subscription>
    {
        #region Field Keys
        private const string ActivatedAtKey = "activated_at";
        private const string BalanceInCentsKey = "balance_in_cents";
        private const string CancelAtEndOfPeriodKey = "cancel_at_end_of_period";
        private const string CanceledAtKey = "canceled_at";
        private const string CancellationMessageKey = "cancellation_message";
        private const string CreatedAtKey = "created_at";
        private const string CurrentPeriodEndsAtKey = "current_period_ends_at";
        private const string ExpiresAtKey = "expires_at";
        private const string IdKey = "id";
        private const string NextAssessmentAtKey = "next_assessment_at";
        private const string PaymentCollectionMethodKey = "payment_collection_method";
        private const string StateKey = "state";
        private const string TrialEndedAtKey = "trial_ended_at";
        private const string TrialStartedAtKey = "trial_started_at";
        private const string UpdatedAtKey = "updated_at";
        private const string CurrentPeriodStartedAtKey = "current_period_started_at";
        private const string PreviousStateKey = "previous_state";
        private const string SignupPaymentIdKey = "signup_payment_id";
        private const string SignupRevenueKey = "signup_revenue";
        private const string DelayedCancelAtKey = "delayed_cancel_at";
        private const string CouponCodeKey = "coupon_code";
        private const string TotalRevenueInCentsKey = "total_revenue_in_cents";
        private const string CustomerKey = "customer";
        private const string PaymentProfileAsCreditCardKey = "credit_card";
        private const string PaymentProfileAsBankAccountKey = "bank_account";
        private const string PaymentProfileKey = "payment_profile";
        private const string ProductKey = "product";
        private const string ProductVersionNumberKey = "product_version_number";
        private const string ProductPriceInCentsKey = "product_price_in_cents";
        private const string NextProductIdKey = "next_product_id";
        private const string ReferralCodeKey = "referral_code";
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Subscription()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionXml">XML containing subscription info (in expected format)</param>
        public Subscription(string subscriptionXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(subscriptionXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(subscriptionXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "subscription")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no customer info was found
            throw new ArgumentException("XML does not contain subscription information", nameof(subscriptionXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionNode">XML containing subscription info (in expected format)</param>
        internal Subscription(XmlNode subscriptionNode)
        {
            if (subscriptionNode == null) throw new ArgumentNullException(nameof(subscriptionNode));
            if (subscriptionNode.Name != "subscription") throw new ArgumentException("Not a vaild subscription node", nameof(subscriptionNode));
            if (subscriptionNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(subscriptionNode));
            LoadFromNode(subscriptionNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionObject">JsonObject containing subscription info (in expected format)</param>
        public Subscription(JsonObject subscriptionObject)
        {
            if (subscriptionObject == null) throw new ArgumentNullException(nameof(subscriptionObject));
            if (subscriptionObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild subscription node", nameof(subscriptionObject));
            LoadFromJson(subscriptionObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing subscription data</param>
        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IdKey:
                        _subscriptionId = obj.GetJSONContentAsInt(key);
                        break;
                    case StateKey:
                        _state = obj.GetJSONContentAsSubscriptionState(key);
                        break;
                    case CancellationMessageKey:
                        _cancellationMessage = obj.GetJSONContentAsString(key);
                        break;
                    case BalanceInCentsKey:
                        _balanceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case CreatedAtKey:
                        _created = obj.GetJSONContentAsDateTime(key);
                        break;
                    case CurrentPeriodStartedAtKey:
                        _currentPeriodStartedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case CurrentPeriodEndsAtKey:
                        _currentPeriodEndsAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case NextAssessmentAtKey:
                        _nextAssessmentAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case TrialStartedAtKey:
                        _trialStartedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case TrialEndedAtKey:
                        _trialEndedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case UpdatedAtKey:
                        _lastUpdated = obj.GetJSONContentAsDateTime(key);
                        break;
                    case ActivatedAtKey:
                        _activatedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case ExpiresAtKey:
                        _expiresAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case CancelAtEndOfPeriodKey:
                        _cancelAtEndOfPeriod = obj.GetJSONContentAsBoolean(key);
                        break;
                    case SignupPaymentIdKey:
                        _signupPaymentId = obj.GetJSONContentAsInt(key);
                        break;
                    case SignupRevenueKey:
                        _signupRevenue = obj.GetJSONContentAsDecimal(key);
                        break;
                    case DelayedCancelAtKey:
                        _delayedCancelAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case PreviousStateKey:
                        _previousState = obj.GetJSONContentAsSubscriptionState(key);
                        break;
                    case CanceledAtKey:
                        _canceledAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case CouponCodeKey:
                        _couponCode = obj.GetJSONContentAsString(key);
                        break;
                    case ProductKey:
                        _product = obj.GetJSONContentAsProduct(key);
                        break;
                    case TotalRevenueInCentsKey:
                        _totalRevenueInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case PaymentCollectionMethodKey:
                        _paymentCollectionMethod = obj.GetJSONContentAsPaymentCollectionMethod(key);
                        break;
                    case ProductVersionNumberKey:
                        _productVersionNumber = obj.GetJSONContentAsInt(key);
                        break;
                    case ProductPriceInCentsKey:
                        _productPriceInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case PaymentProfileAsCreditCardKey:
                    case PaymentProfileAsBankAccountKey:
                    case PaymentProfileKey:
                        // create new bank account view object.
                        _paymentProfile = obj.GetJSONContentAsPaymentProfileView(key);
                        break;
                    case CustomerKey:
                        _customer = obj.GetJSONContentAsCustomer(key);
                        break;
                    case NextProductIdKey:
                        _nextProductId = obj.GetJSONContentAsInt(key);
                        break;
                    case ReferralCodeKey:
                        _referralCode = obj.GetJSONContentAsString(key);
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
                    case IdKey:
                        _subscriptionId = dataNode.GetNodeContentAsInt();
                        break;
                    case StateKey:
                        _state = dataNode.GetNodeContentAsSubscriptionState();
                        break;
                    case CancellationMessageKey:
                        _cancellationMessage = dataNode.GetNodeContentAsString();
                        break;
                    case BalanceInCentsKey:
                        _balanceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case CreatedAtKey:
                        _created = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CurrentPeriodStartedAtKey:
                        _currentPeriodStartedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CurrentPeriodEndsAtKey:
                        _currentPeriodEndsAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case NextAssessmentAtKey:
                        _nextAssessmentAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case TrialStartedAtKey:
                        _trialStartedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case TrialEndedAtKey:
                        _trialEndedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case UpdatedAtKey:
                        _lastUpdated = dataNode.GetNodeContentAsDateTime();
                        break;
                    case ActivatedAtKey:
                        _activatedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case ExpiresAtKey:
                        _expiresAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CancelAtEndOfPeriodKey:
                        _cancelAtEndOfPeriod = dataNode.GetNodeContentAsBoolean();
                        break;
                    case SignupPaymentIdKey:
                        _signupPaymentId = dataNode.GetNodeContentAsInt();
                        break;
                    case SignupRevenueKey:
                        _signupRevenue = dataNode.GetNodeContentAsDecimal();
                        break;
                    case DelayedCancelAtKey:
                        _delayedCancelAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case PreviousStateKey:
                        _previousState = dataNode.GetNodeContentAsSubscriptionState();
                        break;
                    case CanceledAtKey:
                        _canceledAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CouponCodeKey:
                        _couponCode = dataNode.GetNodeContentAsString();
                        break;
                    case ProductKey:
                        _product = dataNode.GetNodeContentAsProduct();
                        break;
                    case TotalRevenueInCentsKey:
                        _totalRevenueInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case PaymentCollectionMethodKey:
                        _paymentCollectionMethod = dataNode.GetNodeContentAsPaymentCollectionMethod();
                        break;
                    case ProductVersionNumberKey:
                        _productVersionNumber = dataNode.GetNodeContentAsInt();
                        break;
                    case ProductPriceInCentsKey:
                        _productPriceInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case PaymentProfileAsCreditCardKey:
                    case PaymentProfileAsBankAccountKey:
                    case PaymentProfileKey:
                        _paymentProfile = dataNode.GetNodeContentAsPaymentProfileView();
                        break;
                    case CustomerKey:
                        _customer = dataNode.GetNodeContentAsCustomer();
                        break;
                    case NextProductIdKey:
                        _nextProductId = dataNode.GetNodeContentAsInt();
                        break;
                    case ReferralCodeKey:
                        _referralCode = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }

        #endregion

        #region ISubscription Members

        public string ReferralCode
        {
            get
            {
                return _referralCode;
            }
        }
        private string _referralCode = string.Empty;

        /// <summary>
        /// The subscription unique ID within Chargify
        /// </summary>
        public int SubscriptionID
        {
            get
            {
                return _subscriptionId;
            }
        }
        private int _subscriptionId;

        /// <summary>
        /// The current state of the subscription. 
        /// It may be "trailing", "active", "soft_failure", "past_due", "suspended", "closed" or "expired"
        /// </summary>
        public SubscriptionState State
        {
            get
            {
                return _state;
            }
        }
        private SubscriptionState _state = SubscriptionState.Unknown;

        /// <summary>
        /// Gives the current outstanding subscription balance, in the number of cents
        /// </summary>
        public int BalanceInCents
        {
            get
            {
                return _balanceInCents;
            }
        }
        private int _balanceInCents;

        /// <summary>
        /// Gives the current outstanding subscription balance, in the number of dollars and cents
        /// </summary>
        public decimal Balance
        {
            get
            {
                return Convert.ToDecimal(_balanceInCents) / 100;
            }
        }

        /// <summary>
        /// Seller-provided reason for, or note about, the cancellation
        /// </summary>
        public string CancellationMessage
        {
            get
            {
                return _cancellationMessage;
            }
        }
        private string _cancellationMessage = "";

        /// <summary>
        /// Timestamp for when the subscription began
        /// <remarks>i.e. when it came out of trial, or when it began in the case of no trial</remarks>
        /// </summary>
        public DateTime ActivatedAt
        {
            get
            {
                return _activatedAt;
            }
        }
        private DateTime _activatedAt = DateTime.MinValue;

        /// <summary>
        /// Get the date and time the customer was created a Chargify
        /// </summary>
        public DateTime Created
        {
            get
            {
                return _created;
            }
        }
        private DateTime _created = DateTime.MinValue;

        /// <summary>
        /// Timestamp giving the expiration date of this subscription (if any)
        /// </summary>
        public DateTime ExpiresAt
        {
            get
            {
                return _expiresAt;
            }
        }
        private DateTime _expiresAt = DateTime.MinValue;

        /// <summary>
        /// Get the date and time the customer was last updated at chargify
        /// </summary>
        public DateTime LastUpdated
        {
            get
            {
                return _lastUpdated;
            }
        }
        private DateTime _lastUpdated = DateTime.MinValue;

        /// <summary>
        /// Get the date the subscription was cancelled
        /// </summary>
        public DateTime CanceledAt
        {
            get
            {
                return _canceledAt;
            }
        }
        private DateTime _canceledAt = DateTime.MinValue;

        /// <summary>
        /// Get the coupon code currently applied (if applicable) to the subscription
        /// </summary>
        public string CouponCode
        {
            get
            {
                return _couponCode;
            }
        }
        private string _couponCode = string.Empty;

        /// <summary>
        /// Timestamp relating to the start of the current (recurring) period
        /// </summary>
        public DateTime CurrentPeriodStartedAt
        {
            get
            {
                return _currentPeriodStartedAt;
            }
        }
        private DateTime _currentPeriodStartedAt = DateTime.MinValue;

        /// <summary>
        /// Timestamp relating to the end of the current (recurring) period
        /// <remarks>i.e. when the next regularily scheduled attemped charge will occur</remarks>
        /// </summary>
        public DateTime CurrentPeriodEndsAt
        {
            get
            {
                return _currentPeriodEndsAt;
            }
        }
        private DateTime _currentPeriodEndsAt = DateTime.MinValue;

        /// <summary>
        /// Get the date and time that indicates when capture of payment will be tried or retried.
        /// </summary>
        public DateTime NextAssessmentAt
        {
            get
            {
                return _nextAssessmentAt;
            }
        }
        private DateTime _nextAssessmentAt = DateTime.MinValue;

        /// <summary>
        /// Timestamp for when the trial period (if any) began
        /// </summary>
        public DateTime TrialStartedAt
        {
            get
            {
                return _trialStartedAt;
            }
        }
        private DateTime _trialStartedAt = DateTime.MinValue;

        /// <summary>
        /// Timestamp for when the trial period (if any) ended
        /// </summary>
        public DateTime TrialEndedAt
        {
            get
            {
                return _trialEndedAt;
            }
        }
        private DateTime _trialEndedAt = DateTime.MinValue;

        /// <summary>
        /// Get the product for this subscription
        /// </summary>
        public IProduct Product
        {
            get
            {
                return _product;
            }
        }
        private IProduct _product;

        /// <summary>
        /// Get the credit card information for this subscription
        /// </summary>
        public IPaymentProfileView PaymentProfile
        {
            get
            {
                return _paymentProfile;
            }
        }
        private IPaymentProfileView _paymentProfile;

        /// <summary>
        /// Get the customer information for this subscription
        /// </summary>
        public ICustomer Customer
        {
            get
            {
                return _customer;
            }
        }
        private ICustomer _customer;

        /// <summary>
        /// Is this subscription going to automatically cancel at the end of the current period?
        /// </summary>
        public bool CancelAtEndOfPeriod
        {
            get { return _cancelAtEndOfPeriod; }
        }
        private bool _cancelAtEndOfPeriod;

        /// <summary>
        /// The ID of the corresponding payment transaction
        /// </summary>
        public int SignupPaymentID
        {
            get { return _signupPaymentId; }
        }
        private int _signupPaymentId = int.MinValue;

        /// <summary>
        /// The revenue accepted upon signup
        /// </summary>
        public decimal SignupRevenue
        {
            get { return _signupRevenue; }
        }
        private decimal _signupRevenue = decimal.MinValue;

        /// <summary>
        /// Get the date and time relating to the time the subscription was cancelled due to a "delayed cancel"
        /// </summary>
        public DateTime DelayedCancelAt
        {
            get { return _delayedCancelAt; }
        }
        private DateTime _delayedCancelAt = DateTime.MinValue;

        /// <summary>
        ///  The previous state of this subscription
        /// </summary>
        public SubscriptionState PreviousState
        {
            get { return _previousState; }
        }
        private SubscriptionState _previousState = SubscriptionState.Unknown;

        /// <summary>
        /// The total subscription revenue (in dollars and cents)
        /// </summary>
        public decimal TotalRevenue
        {
            get { return Convert.ToDecimal(_totalRevenueInCents) / 100; }
        }
        /// <summary>
        /// The total subscription revenue (in cents)
        /// </summary>
        public int TotalRevenueInCents
        {
            get { return _totalRevenueInCents; }
        }
        private int _totalRevenueInCents = int.MinValue;

        /// <summary>
        /// The type of billing used for this subscription
        /// </summary>
        public PaymentCollectionMethod PaymentCollectionMethod { get { return _paymentCollectionMethod; } }
        private PaymentCollectionMethod _paymentCollectionMethod = PaymentCollectionMethod.Unknown;

        /// <summary>
        /// The version of the product currently subscribed. NOTE: we have not exposed versions 
        /// (yet) elsewhere in the API, but if you change the price of your product the versions 
        /// will increment and existing subscriptions will remain on prior versions (by default, 
        /// to support price grandfathering).
        /// </summary>
        public int ProductVersionNumber
        {
            get
            {
                return _productVersionNumber;
            }
        }

        private int _productVersionNumber;

        /// <summary>
        /// At what price was the product on when initial subscribed? (in cents)
        /// </summary>
        public int ProductPriceInCents
        {
            get { return _productPriceInCents; }
        }
        private int _productPriceInCents = int.MinValue;

        /// <summary>
        /// At what price was the product on when initial subscribed? (in dollars and cents)
        /// </summary>
        public decimal ProductPrice
        {
            get { return Convert.ToDecimal(_productPriceInCents) / 100; }
        }

        /// <summary>
        /// If a delayed product change is scheduled, the ID of the product that the subscription 
        /// will be changed to at the next renewal.
        /// </summary>
        public int NextProductId { get { return _nextProductId; } }
        private int _nextProductId = int.MinValue;

        #endregion

        #region Operators

        /// <summary>
        /// Equals operator for two subscriptions
        /// </summary>
        /// <returns>True if the subscriptions are equal</returns>
        public static bool operator ==(Subscription a, Subscription b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null)) { return false; }

            return (a.SubscriptionID == b.SubscriptionID);
        }

        /// <summary>
        /// Equals operator for two subscriptions
        /// </summary>
        /// <returns>True if the subscriptions are equal</returns>
        public static bool operator ==(Subscription a, ISubscription b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object) a == null) || (b == null)) { return false; }

            return (a.SubscriptionID == b.SubscriptionID);
        }

        /// <summary>
        /// Equals operator for two subscriptions
        /// </summary>
        /// <returns>True if the subscriptions are equal</returns>
        public static bool operator ==(ISubscription a, Subscription b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if ((a == null) || ((object) b == null)) { return false; }

            return (a.SubscriptionID == b.SubscriptionID);
        }

        /// <summary>
        /// Not Equals operator for two subscriptions
        /// </summary>
        /// <returns>True if the subscriptions are not equal</returns>
        public static bool operator !=(Subscription a, Subscription b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Not Equals operator for two subscriptions
        /// </summary>
        /// <returns>True if the subscriptions are not equal</returns>
        public static bool operator !=(Subscription a, ISubscription b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Not Equals operator for two subscriptions
        /// </summary>
        /// <returns>True if the subscriptions are not equal</returns>
        public static bool operator !=(ISubscription a, Subscription b)
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

            if (obj is ISubscription)
            {
                return (SubscriptionID == (obj as ISubscription).SubscriptionID);
            }
            return ReferenceEquals(this, obj);
        }

        /// <summary>
        /// Returns a string representation of the Subscription object.
        /// </summary>
        public override string ToString()
        {
            StringBuilder myString = new StringBuilder();
            if (Customer != null)
            {
                myString.AppendFormat("Customer: {0}", Customer.FullName);
            }
            if (Product != null)
            {
                myString.AppendFormat("{0}Product: {1}", (myString.Length > 0 ? "\n" : ""), Product.Name);
            }
            if (!string.IsNullOrEmpty(State.ToString())) myString.AppendFormat("{0}State: {1}", (myString.Length > 0 ? "\n" : ""), State);
            return myString.ToString();
        }

        #endregion

        #region IComparable<ISubscription> Members

        /// <summary>
        /// Compare this instance to another (by SubscriptionID)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(ISubscription other)
        {
            return SubscriptionID.CompareTo(other.SubscriptionID);
        }

        #endregion

        #region IComparable<Subscription> Members

        /// <summary>
        /// Compare this instance to another (by SubscriptionID)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(Subscription other)
        {
            return SubscriptionID.CompareTo(other.SubscriptionID);
        }

        #endregion
    }
}
