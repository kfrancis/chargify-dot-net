
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
    using System.Xml.Serialization;
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
        protected PaymentProfileBase()
        {
            Id = int.MinValue;
            FirstName = string.Empty;
            LastName = string.Empty;
            FullNumber = string.Empty;
            ExpirationMonth = int.MinValue;
            ExpirationYear = int.MinValue;
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="firstName">The first name on the credit card</param>
        /// <param name="lastName">The last name on the credit card</param>
        /// <param name="fullNumber">The full credit card number</param>
        /// <param name="expirationYear">The expiration year</param>
        /// <param name="expirationMonth">The expiration month</param>
        protected PaymentProfileBase(string firstName, string lastName, string fullNumber, int expirationYear, int expirationMonth)
        {
            Id = int.MinValue;
            FirstName = firstName;
            LastName = lastName;
            FullNumber = fullNumber;
            ExpirationMonth = expirationMonth;
            ExpirationYear = expirationYear;
        }

        #endregion

        #region ICreditCardBase Members

        /// <summary>
        /// The Chargify-assigned ID of the stored card.
        /// </summary>
        [XmlIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Get or set the first name on the card
        /// </summary>
        [XmlElement("first_name")]
        public string FirstName { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeFirstName()
        {
            return !string.IsNullOrEmpty(FirstName);
        }

        /// <summary>
        /// Get or set the last name on the card
        /// </summary>
        [XmlElement("last_name")]
        public string LastName { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeLastName()
        {
            return !string.IsNullOrEmpty(LastName);
        }

        /// <summary>
        /// Get or set the full credit card number
        /// </summary>
        [XmlElement("full_number")]
        public string FullNumber { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeFullNumber()
        {
            return !string.IsNullOrEmpty(FullNumber);
        }

        /// <summary>
        /// Get or set the expiration month
        /// </summary>
        [XmlElement("expiration_month")]
        public int ExpirationMonth { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeExpirationMonth()
        {
            return ExpirationMonth != int.MinValue;
        }

        /// <summary>
        /// Get or set the expiration year
        /// </summary>
        [XmlElement("expiration_year")]
        public int ExpirationYear { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeExpirationYear()
        {
            return ExpirationYear != int.MinValue;
        }

        /// <summary>
        /// Get or set the billing address of the credit card
        /// </summary>
        [XmlElement("billing_address")]
        public string BillingAddress { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeBillingAddress()
        {
            return !string.IsNullOrEmpty(BillingAddress);
        }

        /// <summary>
        /// Get or set the billing address 2 of the credit card
        /// </summary>
        [XmlElement("billing_address_2")]
        public string BillingAddress2 { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeBillingAddress2()
        {
            return !string.IsNullOrEmpty(BillingAddress2);
        }

        /// <summary>
        /// Get or set the billing city of the credit card
        /// </summary>
        [XmlElement("billing_city")]
        public string BillingCity { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeBillingCity()
        {
            return !string.IsNullOrEmpty(BillingCity);
        }

        /// <summary>
        /// Get or set the billing state or province of the credit card
        /// </summary>
        [XmlElement("billing_state")]
        public string BillingState { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeBillingState()
        {
            return !string.IsNullOrEmpty(BillingState);
        }

        /// <summary>
        /// Get or set the billing zip code or postal code of the credit card
        /// </summary>
        [XmlElement("billing_zip")]
        public string BillingZip { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeBillingZip()
        {
            return !string.IsNullOrEmpty(BillingZip);
        }

        /// <summary>
        /// Get or set the billing country of the credit card
        /// </summary>
        [XmlElement("billing_country")]
        public string BillingCountry { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeBillingCountry()
        {
            return !string.IsNullOrEmpty(BillingCountry);
        }

        #endregion
    }
}
