
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

namespace ChargifyNET
{
    #region Imports
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
        public string VaultToken
        {
            get { return _vaultToken; }
            set { if (_vaultToken != value) _vaultToken = value; }
        }
        private string _vaultToken = string.Empty;

        /// <summary>
        /// (Only for Authorize.NET CIM storage) The "customerProfileId" for the owner of the
        /// "customerPaymentProfileId" provided as the VaultToken
        /// </summary>
        public string CustomerVaultToken
        {
            get { return _customerVaultToken; }
            set { if (_customerVaultToken != value) _customerVaultToken = value; }
        }
        private string _customerVaultToken = string.Empty;

        /// <summary>
        /// The vault that stores the payment profile with the provided VaultToken
        /// </summary>
        public VaultType CurrentVault
        {
            get { return _currentVault; }
            set { if (_currentVault != value) _currentVault = value; }
        }
        private VaultType _currentVault = VaultType.Unknown;

        /// <summary>
        /// The year of expiration
        /// </summary>
        public int ExpirationYear
        {
            get { return _expirationYear; }
            set { if (_expirationYear != value) _expirationYear = value; }
        }
        private int _expirationYear = int.MinValue;

        /// <summary>
        /// The month of expiration
        /// </summary>
        public int ExpirationMonth
        {
            get { return _expirationMonth; }
            set { if (_expirationMonth != value) _expirationMonth = value; }
        }
        private int _expirationMonth = int.MinValue;

        /// <summary>
        /// (Optional) If you know the card type, you may supply it here so that we may display 
        /// the card type in the UI.
        /// </summary>
        public CardType CardType
        {
            get { return _cardType; }
            set { if (_cardType != value) _cardType = value; }
        }
        private CardType _cardType = CardType.Unknown;

        /// <summary>
        /// (Optional) If you have the last 4 digits of the credit card number, you may supply
        /// them here so we may create a masked card number for display in the UI
        /// </summary>
        public string LastFour
        {
            get { return _lastFour; }
            set { if (_lastFour != value) _lastFour = value; }
        }
        private string _lastFour = string.Empty;

        /// <summary>
        /// The name of the bank where the customer's account resides
        /// </summary>
        public string BankName 
        { 
            get { return _bankName; }
            set { if (_bankName != value) _bankName = value; }
        }
        private string _bankName = string.Empty;

        /// <summary>
        /// The routing number of the bank
        /// </summary>
        public string BankRoutingNumber 
        { 
            get { return _bankRoutingNumber; }
            set { if (_bankRoutingNumber != value) _bankRoutingNumber = value; }
        }
        private string _bankRoutingNumber = string.Empty;

        /// <summary>
        /// The customer's bank account number
        /// </summary>
        public string BankAccountNumber 
        {
            get { return _bankAccountNumber; }
            set { if (_bankAccountNumber != value) _bankAccountNumber = value; }
        }
        private string _bankAccountNumber = string.Empty;

        /// <summary>
        /// Either checking or savings
        /// </summary>
        public BankAccountType BankAccountType 
        { 
            get { return _bankAccountType; }
            set { if (_bankAccountType != value) _bankAccountType = value; }
        }
        private BankAccountType _bankAccountType = BankAccountType.Unknown;

        /// <summary>
        /// Either personal or business
        /// </summary>
        public BankAccountHolderType BankAccountHolderType
        { 
            get { return _bankAccountHolderType; }
            set { if (_bankAccountHolderType != value) _bankAccountHolderType = value; }
        }
        private BankAccountHolderType _bankAccountHolderType = BankAccountHolderType.Unknown;

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
