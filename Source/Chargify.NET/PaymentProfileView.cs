
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
    #endregion

    /// <summary>
    /// Class representing view information for a credit card
    /// </summary>
    [DebuggerDisplay("Type: {Type}, Full Number: {FullNumber}, Expiration: {ExpirationMonth}/{ExpirationYear}, Name: {FirstName} {LastName}")]
    public class PaymentProfileView : PaymentProfileBase, IPaymentProfileBase, IPaymentProfileView
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal PaymentProfileView() { }

        #region ICreditCardView Members

        /// <summary>
        /// Get or set the type of the credit card (Visa, Mastercard. etc.)
        /// </summary>
        public string CardType 
        {
            get { return this._cardType; }
            set { if (this._cardType != value) { this._cardType = value; } }
        }
        private string _cardType = string.Empty;

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
        /// The masked routing number of the bank
        /// </summary>
        public string MaskedBankRoutingNumber 
        { 
            get { return _maskedBankRoutingNumber; }
            set { if (_maskedBankRoutingNumber != value) _maskedBankRoutingNumber = value; }
        }
        private string _maskedBankRoutingNumber = string.Empty;

        /// <summary>
        /// The customer's masked bank account number
        /// </summary>
        public string MaskedBankAccountNumber 
        {
            get { return _maskedBankAccountNumber; }
            set { if (_maskedBankAccountNumber != value) _maskedBankAccountNumber = value; }
        }
        private string _maskedBankAccountNumber = string.Empty;

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

        #region Equality
        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Equality Operator
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        #endregion

        /// <summary>
        /// Represent this object as a string
        /// </summary>
        /// <returns>A string representation of the object</returns>
        public override string ToString()
        {
            return string.Format(" {0}: {1}\nName on Card: {2} {3}\nExpires {4}/{5}", this.CardType, this.FullNumber, this.FirstName, this.LastName, this.ExpirationMonth, this.ExpirationYear);
        }

    }
}
