
#region License, Terms and Conditions
//
// PaymentProfileAttributes.cs
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

using System.Xml.Serialization;

namespace ChargifyNET
{
    #region Imports
    using System;
    using System.Diagnostics;
    #endregion

    /// <summary>
    /// Class which can be used to "import" subscriptions via the API into Chargify
    /// Info here: http://support.chargify.com/faqs/api/api-subscription-and-stored-card-token-imports
    /// </summary>
    [DebuggerDisplay("VaultToken: {VaultToken}, CustomerVaultToken: {CustomerVaultToken}")]
    public class PaymentProfileAttributes : ChargifyBase, IPaymentProfileAttributes, IComparable<PaymentProfileAttributes>
    {
        #region Constructor
        /// <summary>
        /// Class which can be used to "import" subscriptions via the API into Chargify
        /// </summary>
        public PaymentProfileAttributes() : base() { /* Nothing */ }

        /// <summary>
        /// Class which can be used to "import" subscriptions via the API into Chargify
        /// </summary>
        /// <param name="VaultToken">The "token" provided by your vault storage for an already stored payment profile</param>
        /// <param name="CustomerVaultToken">The "customerProfileId" for the owner of the "customerPaymentProfileId" provided as the VaultToken</param>
        /// <param name="CurrentVault">The vault that stores the payment profile with the provided VaultToken</param>
        /// <param name="ExpYear">The year of expiration</param>
        /// <param name="ExpMonth">The month of expiration</param>
        /// <param name="CardType">If you know the card type, you may supply it here so that Chargify may display it in the AdminUI</param>
        /// <param name="LastFourDigits">The last four digits of the credit card for use in masked numbers</param>
        public PaymentProfileAttributes(string VaultToken, string CustomerVaultToken, VaultType CurrentVault, int ExpYear, int ExpMonth, CardType CardType, string LastFourDigits) : base() 
        {
            this._vaultToken = VaultToken;
            this._customerVaultToken = CustomerVaultToken;
            this._currentVault = CurrentVault;
            this._expirationYear = ExpYear;
            this._expirationMonth = ExpMonth;
            this._cardType = CardType;
            this._lastFour = LastFourDigits;
        }
        #endregion

        #region IPaymentProfileAttributes Members
        /// <summary>
        /// The "token" provided by your vault storage for an already stored payment profile
        /// </summary>
        [XmlElement("vault_token")]
        public string VaultToken
        {
            get { return _vaultToken; }
            set { if (_vaultToken != value) _vaultToken = value; }
        }
        private string _vaultToken = string.Empty;

        public bool ShouldSerializeVaultToken()
        {
            return !String.IsNullOrEmpty(_vaultToken);
        }

        /// <summary>
        /// (Only for Authorize.NET CIM storage) The "customerProfileId" for the owner of the
        /// "customerPaymentProfileId" provided as the VaultToken
        /// </summary>

        [XmlElement("customer_vault_token")]
        public string CustomerVaultToken
        {
            get { return _customerVaultToken; }
            set { if (_customerVaultToken != value) _customerVaultToken = value; }
        }
        private string _customerVaultToken = string.Empty;
        public bool ShouldSerializeCustomerVaultToken()
        {
            return !String.IsNullOrEmpty(_customerVaultToken);
        }

        /// <summary>
        /// The vault that stores the payment profile with the provided VaultToken
        /// </summary>

        [XmlElement("current_vault")]
        public VaultType CurrentVault
        {
            get { return _currentVault; }
            set { if (_currentVault != value) _currentVault = value; }
        }
        private VaultType _currentVault = VaultType.Unknown;

        public bool ShouldSerializeCurrentVault()
        {
            return _currentVault != VaultType.Unknown;
        }

        /// <summary>
        /// The year of expiration
        /// </summary>
        [XmlElement("expiration_year")]
        public int ExpirationYear
        {
            get { return _expirationYear; }
            set { if (_expirationYear != value) _expirationYear = value; }
        }
        private int _expirationYear = int.MinValue;

        public bool ShouldSerializeExpirationYear()
        {
            return _expirationYear != int.MinValue;
        }

        /// <summary>
        /// The month of expiration
        /// </summary>
        [XmlElement("expiration_month")]
        public int ExpirationMonth
        {
            get { return _expirationMonth; }
            set { if (_expirationMonth != value) _expirationMonth = value; }
        }
        private int _expirationMonth = int.MinValue;

        public bool ShouldSerializeExpirationMonth()
        {
            return _expirationMonth != int.MinValue;
        }

        /// <summary>
        /// (Optional) If you know the card type, you may supply it here so that we may display 
        /// the card type in the UI.
        /// </summary>
        [XmlElement("card_type")]
        public CardType CardType
        {
            get { return _cardType; }
            set { if (_cardType != value) _cardType = value; }
        }
        private CardType _cardType = CardType.Unknown;

        public bool ShouldSerializeCardType()
        {
            return _cardType != CardType.Unknown;
        }

        /// <summary>
        /// (Optional) If you have the last 4 digits of the credit card number, you may supply
        /// them here so we may create a masked card number for display in the UI
        /// </summary>
        [XmlElement("last_four")]
        public string LastFour
        {
            get { return _lastFour; }
            set { if (_lastFour != value) _lastFour = value; }
        }
        private string _lastFour = string.Empty;

        public bool ShouldSerializeLastFour()
        {
            return !String.IsNullOrEmpty(_lastFour);
        }

        /// <summary>
        /// The name of the bank where the customer's account resides
        /// </summary>
        [XmlElement("bank_name")]
        public string BankName 
        { 
            get { return _bankName; }
            set { if (_bankName != value) _bankName = value; }
        }
        private string _bankName = string.Empty;

        public bool ShouldSerializeBankName()
        {
            return !String.IsNullOrEmpty(_bankName);
        }

        /// <summary>
        /// The routing number of the bank
        /// </summary>
        [XmlElement("bank_routing_number")]
        public string BankRoutingNumber 
        { 
            get { return _bankRoutingNumber; }
            set { if (_bankRoutingNumber != value) _bankRoutingNumber = value; }
        }
        private string _bankRoutingNumber = string.Empty;

        public bool ShouldSerializeBankRoutingNumber()
        {
            return !String.IsNullOrEmpty(_bankRoutingNumber);
        }

        /// <summary>
        /// The customer's bank account number
        /// </summary>
        [XmlElement("bank_account_number")]
        public string BankAccountNumber 
        {
            get { return _bankAccountNumber; }
            set { if (_bankAccountNumber != value) _bankAccountNumber = value; }
        }
        private string _bankAccountNumber = string.Empty;

        public bool ShouldSerializeBankAccountNumber()
        {
            return !String.IsNullOrEmpty(_bankAccountNumber);
        }

        /// <summary>
        /// Either checking or savings
        /// </summary>
        [XmlElement("bank_account_type")]
        public BankAccountType BankAccountType 
        { 
            get { return _bankAccountType; }
            set { if (_bankAccountType != value) _bankAccountType = value; }
        }
        private BankAccountType _bankAccountType = BankAccountType.Unknown;

        public bool ShouldSerializeBankAccountType()
        {
            return _bankAccountType != BankAccountType.Unknown;
        }

        /// <summary>
        /// Either personal or business
        /// </summary>
        [XmlElement("bank_account_holder_type")]
        public BankAccountHolderType BankAccountHolderType
        { 
            get { return _bankAccountHolderType; }
            set { if (_bankAccountHolderType != value) _bankAccountHolderType = value; }
        }
        private BankAccountHolderType _bankAccountHolderType = BankAccountHolderType.Unknown;

        public bool ShouldSerializeBankAccountHolderType()
        {
            return _bankAccountHolderType != BankAccountHolderType.Unknown;
        }

        [XmlElement("payment_method_nonce")]
        public string PaymentMethodNonce
        {
            get { return _paymentMethodNonce; }
            set { if (_paymentMethodNonce != value) _paymentMethodNonce = value; }
        }
        private string _paymentMethodNonce = string.Empty;

        public bool ShouldSerializePaymentMethodNonce()
        {
            return !String.IsNullOrEmpty(_paymentMethodNonce);
        }

        [XmlElement("paypal_email")]
        public string PayPalEmail
        {
            get { return _payPalEmail; }
            set { if (_payPalEmail != value) _payPalEmail = value; }
        }
        private string _payPalEmail = string.Empty;

        public bool ShouldSerializePayPalEmail()
        {
            return !String.IsNullOrEmpty(_payPalEmail);
        }

        [XmlElement("first_name")]
        public string FirstName
        {
            get { return _firstName; }
            set { if (_firstName != value) _firstName = value; }
        }
        private string _firstName = string.Empty;
        public bool ShouldSerializeFirstName()
        {
            return !String.IsNullOrEmpty(_firstName);
        }

        [XmlElement("last_name")]
        public string LastName
        {
            get { return _lastName; }
            set { if (_lastName != value) _lastName = value; }
        }
        private string _lastName = string.Empty;

        public bool ShouldSerializeLastName()
        {
            return !String.IsNullOrEmpty(_lastName);
        }

        [XmlElement("billing_address")]
        public string BillingAddress
        {
            get { return _billingAddress; }
            set { if (_billingAddress != value) _billingAddress = value; }
        }
        private string _billingAddress = string.Empty;

        public bool ShouldSerializeBillingAddress()
        {
            return !String.IsNullOrEmpty(_billingAddress);
        }

        [XmlElement("billing_address2")]
        public string BillingAddress2
        {
            get { return _billingAddress2; }
            set { if (_billingAddress2 != value) _billingAddress2 = value; }
        }
        private string _billingAddress2 = string.Empty;

        public bool ShouldSerializeBillingAddress2()
        {
            return !String.IsNullOrEmpty(_billingAddress2);
        }

        [XmlElement("billing_city")]
        public string BillingCity
        {
            get { return _billingCity; }
            set { if (_billingCity != value) _billingCity = value; }
        }
        private string _billingCity = string.Empty;

        public bool ShouldSerializeBillingCity()
        {
            return !String.IsNullOrEmpty(_billingCity);
        }

        [XmlElement("billing_state")]
        public string BillingState
        {
            get { return _billingState; }
            set { if (_billingState != value) _billingState = value; }
        }
        private string _billingState = string.Empty;

        public bool ShouldSerializeBillingState()
        {
            return !String.IsNullOrEmpty(_billingState);
        }

        [XmlElement("billing_zip")]
        public string BillingZip
        {
            get { return _billingZip; }
            set { if (_billingZip != value) _billingZip = value; }
        }
        private string _billingZip = string.Empty;

        public bool ShouldSerializeBillingZip()
        {
            return !String.IsNullOrEmpty(_billingZip);
        }

        [XmlElement("billing_country")]
        public string BillingCountry
        {
            get { return _billingCountry; }
            set { if (_billingCountry != value) _billingCountry = value; }
        }
        private string _billingCountry = string.Empty;

        public bool ShouldSerializeBillingCountry()
        {
            return !String.IsNullOrEmpty(_billingCountry);
        }

        [XmlElement("payment_type")]
        public PaymentProfileType PaymentType
        {
            get { return this._paymentType; }
            set { if (this._paymentType != value) { this._paymentType = value; } }
        }
        private PaymentProfileType _paymentType = PaymentProfileType.Credit_Card;

        #endregion

        #region IComparable<IPaymentProfileAttributes> Members

        /// <summary>
        /// Method for comparing one IPaymentProfileAttributes to another (by vault token)
        /// </summary>
        /// <param name="other">The other IPaymentProfileAttributes to compare with</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(IPaymentProfileAttributes other)
        {
            return this.VaultToken.CompareTo(other.VaultToken);
        }

        #endregion

        #region IComparable<PaymentProfileAttributes> Members

        /// <summary>
        /// Method for comparing one PaymentProfileAttributes to another (by vault token)
        /// </summary>
        /// <param name="other">The other PaymentProfileAttributes to compare with</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(PaymentProfileAttributes other)
        {
            return this.VaultToken.CompareTo(other.VaultToken);
        }

        #endregion
    }
}
