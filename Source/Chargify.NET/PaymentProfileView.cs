
#region License, Terms and Conditions
//
// CreditCardView.cs
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
    using System.Diagnostics;
    using System.Xml;
    using System;
    using Json;
    #endregion

    /// <summary>
    /// Class representing view information for a credit card
    /// </summary>
    [DebuggerDisplay("Type: {CardType}, Full Number: {FullNumber}, Expiration: {ExpirationMonth}/{ExpirationYear}, Name: {FirstName} {LastName}")]
    public class PaymentProfileView : PaymentProfileBase, IPaymentProfileBase, IPaymentProfileView
    {
        #region Field Keys
        private const string IdKey = "id";
        private const string CustomerIdKey = "customer_id";
        private const string FirstNameKey = "first_name";
        private const string LastNameKey = "last_name";
        private const string FullNumberKey = "full_number";
        private const string ExpirationMonthKey = "expiration_month";
        private const string ExpirationYearKey = "expiration_year";
        private const string CvvKey = "cvv";
        private const string BillingAddressKey = "billing_address";
        private const string BillingAddress2Key = "billing_address_2";
        private const string BillingCityKey = "billing_city";
        private const string BillingCountryKey = "billing_country";
        private const string BillingStateKey = "billing_state";
        private const string BillingZipKey = "billing_zip";
        private const string BankNameKey = "bank_name";
        private const string BankRoutingNumberKey = "bank_routing_number";
        private const string BankAccountNumberKey = "bank_account_number";
        private const string BankAccountTypeKey = "bank_account_type";
        private const string BankAccountHolderTypeKey = "bank_account_holder_type";
        private const string PaymentMethodNonceKey = "payment_method_nonce";
        private const string PayPalEmailKey = "paypal_email";
        private const string PaymentTypeKey = "payment_type";
        private const string CardTypeKey = "card_type";
        private const string MaskedCardNumberKey = "masked_card_number";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentProfileView()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paymentProfileXml"></param>
        public PaymentProfileView(string paymentProfileXml)
        {
            // get the XML into an XML document
            var doc = new XmlDocument();
            doc.LoadXml(paymentProfileXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(paymentProfileXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "payment_profile" || elementNode.Name == "bank_account" || elementNode.Name == "credit_card")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no customer info was found
            throw new ArgumentException("XML does not contain payment_profile information", nameof(paymentProfileXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paymentProfileNode">XML containing payment_profile info (in expected format)</param>
        internal PaymentProfileView(XmlNode paymentProfileNode)
        {
            if (paymentProfileNode == null) throw new ArgumentNullException(nameof(paymentProfileNode));
            if (paymentProfileNode.Name != "payment_profile" && paymentProfileNode.Name != "credit_card" && paymentProfileNode.Name != "bank_account") throw new ArgumentException("Not a vaild payment_profile node", nameof(paymentProfileNode));
            if (paymentProfileNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(paymentProfileNode));
            LoadFromNode(paymentProfileNode);
        }

        /// <summary>
        /// Json Constructor
        /// </summary>
        /// <param name="paymentProfileObject">The json object containing payment profile information</param>
        public PaymentProfileView(JsonObject paymentProfileObject)
        {
            if (paymentProfileObject == null) throw new ArgumentNullException(nameof(paymentProfileObject));
            if (paymentProfileObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild PaymentProfile node", nameof(paymentProfileObject));
            LoadFromJson(paymentProfileObject);
        }

        private void LoadFromNode(XmlNode rootNode)
        {
            foreach (XmlNode dataNode in rootNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case IdKey:
                        Id = dataNode.GetNodeContentAsInt();
                        break;
                    case CustomerIdKey:
                        _customerId = dataNode.GetNodeContentAsInt();
                        break;
                    case FirstNameKey:
                        FirstName = dataNode.GetNodeContentAsString();
                        break;
                    case LastNameKey:
                        LastName = dataNode.GetNodeContentAsString();
                        break;
                    case FullNumberKey:
                        FullNumber = dataNode.GetNodeContentAsString();
                        break;
                    case ExpirationMonthKey:
                        ExpirationMonth = dataNode.GetNodeContentAsInt();
                        break;
                    case ExpirationYearKey:
                        ExpirationYear = dataNode.GetNodeContentAsInt();
                        break;
                    case CvvKey:
                        _cvv = dataNode.GetNodeContentAsString();
                        break;
                    case BillingAddressKey:
                        BillingAddress = dataNode.GetNodeContentAsString();
                        break;
                    case BillingAddress2Key:
                        BillingAddress2 = dataNode.GetNodeContentAsString();
                        break;
                    case BillingCityKey:
                        BillingCity = dataNode.GetNodeContentAsString();
                        break;
                    case BillingCountryKey:
                        BillingCountry = dataNode.GetNodeContentAsString();
                        break;
                    case BillingStateKey:
                        BillingState = dataNode.GetNodeContentAsString();
                        break;
                    case BillingZipKey:
                        BillingZip = dataNode.GetNodeContentAsString();
                        break;
                    case BankNameKey:
                        _bankName = dataNode.GetNodeContentAsString();
                        break;
                    case BankRoutingNumberKey:
                        _bankRoutingNumber = dataNode.GetNodeContentAsString();
                        break;
                    case BankAccountNumberKey:
                        _bankAccountNumber = dataNode.GetNodeContentAsString();
                        break;
                    case BankAccountTypeKey:
                        _bankAccountType = dataNode.GetNodeContentAsEnum<BankAccountType>();                        
                        break;
                    case BankAccountHolderTypeKey:
                        _bankAccountHolderType = dataNode.GetNodeContentAsEnum<BankAccountHolderType>();
                        break;
                    case PaymentMethodNonceKey:
                        _paymentMethodNonce = dataNode.GetNodeContentAsString();
                        break;
                    case PayPalEmailKey:
                        _payPalEmail = dataNode.GetNodeContentAsString();
                        break;
                    case PaymentTypeKey:
                        _paymentType = dataNode.GetNodeContentAsEnum<PaymentProfileType>();
                        break;
                    case CardTypeKey:
                        _cardType = dataNode.GetNodeContentAsString();
                        break;
                    case MaskedCardNumberKey:
                        _maskedCardNumber = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }

        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IdKey:
                        Id = obj.GetJSONContentAsInt(key);
                        break;
                    case CustomerIdKey:
                        _customerId = obj.GetJSONContentAsInt(key);
                        break;
                    case FirstNameKey:
                        FirstName = obj.GetJSONContentAsString(key);
                        break;
                    case LastNameKey:
                        LastName = obj.GetJSONContentAsString(key);
                        break;
                    case FullNumberKey:
                        FullNumber = obj.GetJSONContentAsString(key);
                        break;
                    case ExpirationMonthKey:
                        ExpirationMonth = obj.GetJSONContentAsInt(key);
                        break;
                    case ExpirationYearKey:
                        ExpirationYear = obj.GetJSONContentAsInt(key);
                        break;
                    case CvvKey:
                        _cvv = obj.GetJSONContentAsString(key);
                        break;
                    case BillingAddressKey:
                        BillingAddress = obj.GetJSONContentAsString(key);
                        break;
                    case BillingAddress2Key:
                        BillingAddress2 = obj.GetJSONContentAsString(key);
                        break;
                    case BillingCityKey:
                        BillingCity = obj.GetJSONContentAsString(key);
                        break;
                    case BillingCountryKey:
                        BillingCountry = obj.GetJSONContentAsString(key);
                        break;
                    case BillingStateKey:
                        BillingState = obj.GetJSONContentAsString(key);
                        break;
                    case BillingZipKey:
                        BillingZip = obj.GetJSONContentAsString(key);
                        break;
                    case BankNameKey:
                        _bankName = obj.GetJSONContentAsString(key);
                        break;
                    case BankRoutingNumberKey:
                        _bankRoutingNumber = obj.GetJSONContentAsString(key);
                        break;
                    case BankAccountNumberKey:
                        _bankAccountNumber = obj.GetJSONContentAsString(key);
                        break;
                    case BankAccountTypeKey:
                        _bankAccountType = obj.GetJSONContentAsEnum<BankAccountType>(key);
                        break;
                    case BankAccountHolderTypeKey:
                        _bankAccountHolderType = obj.GetJSONContentAsEnum<BankAccountHolderType>(key);
                        break;
                    case PaymentMethodNonceKey:
                        _paymentMethodNonce = obj.GetJSONContentAsString(key);
                        break;
                    case PayPalEmailKey:
                        _payPalEmail = obj.GetJSONContentAsString(key);
                        break;
                    case PaymentTypeKey:
                        _paymentType = obj.GetJSONContentAsEnum<PaymentProfileType>(key);
                        break;
                    case CardTypeKey:
                        _cardType = obj.GetJSONContentAsString(key);
                        break;
                    case MaskedCardNumberKey:
                        _maskedCardNumber = obj.GetJSONContentAsString(key);
                        break;
                }
            }
        }
        #endregion

        #region ICreditCardView Members
        /// <summary>
        /// Default is credit_card. May be bank_account or credit_card or paypal_account
        /// </summary>
        public PaymentProfileType PaymentType
        {
            get { return _paymentType; }
            set { _paymentType = value; }
        }
        private PaymentProfileType _paymentType = PaymentProfileType.Credit_Card;

        /// <summary>
        /// The ID to the customer associated with this payment profile
        /// </summary>
        public int CustomerID
        {
            get { return _customerId; }
            set { _customerId = value; }
        }
        private int _customerId = int.MinValue;

        /// <summary>
        /// The card verification value
        /// </summary>
        public string CVV
        {
            get { return _cvv; }
            set { _cvv = value; }
        }
        private string _cvv = string.Empty;

        /// <summary>
        /// Get or set the type of the credit card (Visa, Mastercard. etc.)
        /// </summary>
        public string CardType
        {
            get { return _cardType; }
            set { _cardType = value; }
        }
        private string _cardType = string.Empty;

        /// <summary>
        /// The name of the bank where the customer's account resides
        /// </summary>
        public string BankName
        {
            get { return _bankName; }
            set { _bankName = value; }
        }
        private string _bankName = string.Empty;

        /// <summary>
        /// The masked routing number of the bank
        /// </summary>
        public string MaskedBankRoutingNumber
        {
            get { return _maskedBankRoutingNumber; }
            set { _maskedBankRoutingNumber = value; }
        }
        private string _maskedBankRoutingNumber = string.Empty;

        /// <summary>
        /// The routing number of the bank
        /// </summary>
        public string BankRoutingNumber
        {
            get { return _bankRoutingNumber; }
            set { _bankRoutingNumber = value; }
        }
        private string _bankRoutingNumber = string.Empty;

        /// <summary>
        /// The customer's masked bank account number
        /// </summary>
        public string MaskedBankAccountNumber
        {
            get { return _maskedBankAccountNumber; }
            set { _maskedBankAccountNumber = value; }
        }
        private string _maskedBankAccountNumber = string.Empty;

        /// <summary>
        /// The customer's bank account number
        /// </summary>
        public string BankAccountNumber
        {
            get { return _bankAccountNumber; }
            set { _bankAccountNumber = value; }
        }
        private string _bankAccountNumber = string.Empty;
        /// <summary>
        /// Either checking or savings
        /// </summary>
        public BankAccountType BankAccountType
        {
            get { return _bankAccountType; }
            set { _bankAccountType = value; }
        }
        private BankAccountType _bankAccountType = BankAccountType.Unknown;

        /// <summary>
        /// Either personal or business
        /// </summary>
        public BankAccountHolderType BankAccountHolderType
        {
            get { return _bankAccountHolderType; }
            set { _bankAccountHolderType = value; }
        }
        private BankAccountHolderType _bankAccountHolderType = BankAccountHolderType.Unknown;

        /// <summary>
        /// The customer's masked credit card number
        /// </summary>
        public string MaskedCardNumber
        {
            get { return _maskedCardNumber; }
            set { _maskedCardNumber = value; }
        }
        private string _maskedCardNumber = string.Empty;

        /// <summary>
        /// The nonce value
        /// </summary>
        public string PaymentMethodNonce
        {
            get { return _paymentMethodNonce; }
            set { _paymentMethodNonce = value; }
        }
        private string _paymentMethodNonce = string.Empty;

        /// <summary>
        /// The paypal account attributed to this profile
        /// </summary>
        public string PayPalEmail
        {
            get { return _payPalEmail; }
            set { _payPalEmail = value; }
        }
        private string _payPalEmail = string.Empty;
        #endregion

        /// <summary>
        /// Represent this object as a string
        /// </summary>
        /// <returns>A string representation of the object</returns>
        public override string ToString()
        {
            return string.Format(" {0}: {1}\nName on Card: {2} {3}\nExpires {4}/{5}", CardType, FullNumber, FirstName, LastName, ExpirationMonth, ExpirationYear);
        }

    }
}
