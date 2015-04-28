
#region License, Terms and Conditions
//
// CreditCardAttributes.cs
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
    /// Class representing credit card attributes associated with a new subscription
    /// </summary>
    [DebuggerDisplay("Full Number: {FullNumber}, CVV: {CVV}, Expiration: {ExpirationMonth}/{ExpirationYear}, Name: {FirstName} {LastName}")]
    public class CreditCardAttributes : PaymentProfileBase, ICreditCardAttributes
    {
        #region Constructors

        /// <summary>
        /// Contructor
        /// </summary>
        public CreditCardAttributes() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FirstName">The first name on the credit card</param>
        /// <param name="LastName">The last name on the credit card</param>
        /// <param name="FullNumber">The full credit card number</param>
        /// <param name="ExpirationYear">The expiration year</param>
        /// <param name="ExpirationMonth">The expiration month</param>
        /// <param name="CVV">The CVV number of the credit card</param>
        /// <param name="BillingAddress">THe billing address of the credit card</param>
        /// <param name="BillingCity">The billing city of the credit card</param>
        /// <param name="BillingState">The billing state or province of the credit card</param>
        /// <param name="BillingZip">The billing zip code or postal code of the credit card</param>
        /// <param name="BillingCountry">The billing country of the credit card</param>
        public CreditCardAttributes(string FirstName, string LastName, string FullNumber, int ExpirationYear, int ExpirationMonth,
                                    string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip,
                                    string BillingCountry)
            : base(FirstName, LastName, FullNumber, ExpirationYear, ExpirationMonth)

        {
            this.CVV = CVV;
            this.BillingAddress = BillingAddress;
            this.BillingCity = BillingCity;
            this.BillingState = BillingState;
            this.BillingZip = BillingZip;
            this.BillingCountry = BillingCountry;
        }

        #endregion

        #region ICreditCardAttributes Members

        /// <summary>
        /// Get or set the CVV number on the credit card
        /// </summary>
        public string CVV { get; set; }

        #endregion

        #region Operators
        
        /// <summary>
        /// Equals operator for two credit card attributes
        /// </summary>
        /// <returns>True if the credit card attributes are equal</returns>
        public static bool operator ==(CreditCardAttributes a, CreditCardAttributes b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            return a.FullNumber == b.FullNumber && 
                   a.CVV == b.CVV && 
                   a.ExpirationMonth == b.ExpirationMonth && 
                   a.ExpirationYear == b.ExpirationYear && 
                   a.BillingAddress == b.BillingAddress && 
                   a.BillingCity == b.BillingCity && 
                   a.BillingState == b.BillingState && 
                   a.BillingZip == b.BillingZip && 
                   a.BillingCountry == b.BillingCountry;
        }

        /// <summary>
        /// Unequal operator for two credit card attributes
        /// </summary>
        /// <returns>True if the credit card attributes are unequal</returns>
        public static bool operator !=(CreditCardAttributes a, CreditCardAttributes b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Equals operator for two credit card attributes
        /// </summary>
        /// <returns>True if the credit card attributes are equal</returns>
        public static bool operator ==(ICreditCardAttributes a, CreditCardAttributes b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            return a.FullNumber == b.FullNumber &&
                   a.CVV == b.CVV &&
                   a.ExpirationMonth == b.ExpirationMonth &&
                   a.ExpirationYear == b.ExpirationYear &&
                   a.BillingAddress == b.BillingAddress &&
                   a.BillingCity == b.BillingCity &&
                   a.BillingState == b.BillingState &&
                   a.BillingZip == b.BillingZip &&
                   a.BillingCountry == b.BillingCountry;
        }

        /// <summary>
        /// Unequal operator for two credit card attributes
        /// </summary>
        /// <returns>True if the credit card attributes are unequal</returns>
        public static bool operator !=(ICreditCardAttributes a, CreditCardAttributes b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Equals operator for two credit card attributes
        /// </summary>
        /// <returns>True if the credit card attributes are equal</returns>
        public static bool operator ==(CreditCardAttributes a, ICreditCardAttributes b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            return a.FullNumber == b.FullNumber &&
                   a.CVV == b.CVV &&
                   a.ExpirationMonth == b.ExpirationMonth &&
                   a.ExpirationYear == b.ExpirationYear &&
                   a.BillingAddress == b.BillingAddress &&
                   a.BillingCity == b.BillingCity &&
                   a.BillingState == b.BillingState &&
                   a.BillingZip == b.BillingZip &&
                   a.BillingCountry == b.BillingCountry;
        }

        /// <summary>
        /// Unequal operator for two credit card attributes
        /// </summary>
        /// <returns>True if the credit card attributes are unequal</returns>
        public static bool operator !=(CreditCardAttributes a, ICreditCardAttributes b)
        {
            return !(a == b);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Represent this object as a string
        /// </summary>
        /// <returns>A string representation of the object</returns>
        public override string ToString()
        {
            return string.Format("{0} CVV: {1}\nName on Card: {2} {3}\nExpires {4}/{5}\nBilling Address:\n\t{6}\n\t{7}\n\t{8}\n\t{9}\n\t{10}", this.FullNumber, this.CVV, this.FirstName, this.LastName, this.ExpirationMonth, this.ExpirationYear, this.BillingAddress, this.BillingCity, this.BillingState, this.BillingCountry, this.BillingZip);
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Equal operator
        /// </summary>
        /// <param name="obj">the object to compare</param>
        /// <returns>true if they are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (typeof(ICreditCardAttributes).IsAssignableFrom(obj.GetType()))
            {
                return (this == (obj as ICreditCardAttributes));
            }
            else
            {
                return base.Equals(obj);
            }
        }
        #endregion
    }
}
