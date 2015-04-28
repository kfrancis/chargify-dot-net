
#region License, Terms and Conditions
//
// PaymentProfileBase.cs
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
    #endregion

    /// <summary>
    /// Base class for the payment profiles
    /// </summary>
    public class PaymentProfileBase : ChargifyBase, IPaymentProfileBase
    {
        #region Constructors

        /// <summary>
        /// Protected Constructor
        /// </summary>
        protected PaymentProfileBase() : base()
        {
            this.Id = int.MinValue;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.FullNumber = string.Empty;
            this.ExpirationMonth = 1;
            this.ExpirationYear = DateTime.Now.Year;
        }
        
        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="FirstName">The first name on the credit card</param>
        /// <param name="LastName">The last name on the credit card</param>
        /// <param name="FullNumber">The full credit card number</param>
        /// <param name="ExpirationYear">The expiration year</param>
        /// <param name="ExpirationMonth">The expiration month</param>
        protected PaymentProfileBase(string FirstName, string LastName, string FullNumber, int ExpirationYear, int ExpirationMonth) : base()
        {
            this.Id = int.MinValue;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.FullNumber = FullNumber;
            this.ExpirationMonth = ExpirationMonth;
            this.ExpirationYear = ExpirationYear;
        }

        #endregion

        #region ICreditCardBase Members

        /// <summary>
        /// The Chargify-assigned ID of the stored card.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Get or set the first name on the card
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Get or set the last name on the card
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Get or set the full credit card number
        /// </summary>
        public string FullNumber { get; set; }

        /// <summary>
        /// Get or set the expiration month
        /// </summary>
        public int ExpirationMonth { get; set; }

        /// <summary>
        /// Get or set the expiration year
        /// </summary>
        public int ExpirationYear { get; set; }

        /// <summary>
        /// Get or set the billing address of the credit card
        /// </summary>
        public string BillingAddress { get; set; }

        /// <summary>
        /// Get or set the billing address 2 of the credit card
        /// </summary>
        public string BillingAddress2 { get; set; }

        /// <summary>
        /// Get or set the billing city of the credit card
        /// </summary>
        public string BillingCity { get; set; }

        /// <summary>
        /// Get or set the billing state or province of the credit card
        /// </summary>
        public string BillingState { get; set; }

        /// <summary>
        /// Get or set the billing zip code or postal code of the credit card
        /// </summary>
        public string BillingZip { get; set; }

        /// <summary>
        /// Get or set the billing country of the credit card
        /// </summary>
        public string BillingCountry { get; set; }

        #endregion
    }
}
