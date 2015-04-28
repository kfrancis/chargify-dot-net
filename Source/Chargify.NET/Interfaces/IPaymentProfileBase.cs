
#region License, Terms and Conditions
//
// IPaymentProfileBase.cs
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
    /// <summary>
    /// Base class for the credit card types
    /// </summary>
    public interface IPaymentProfileBase
    {
        /// <summary>
        /// The Chargify-assigned ID of the stored card.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Get or set the first name on the card
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        /// Get or set the last name on the card
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        /// Get or set the full credit card number
        /// </summary>
        string FullNumber { get; set; }

        /// <summary>
        /// Get or set the expiration month
        /// </summary>
        int ExpirationMonth { get; set; }

        /// <summary>
        /// Get or set the expiration year
        /// </summary>
        int ExpirationYear { get; set; }

        /// <summary>
        /// Get or set the billing address of the credit card
        /// </summary>
        string BillingAddress { get; set; }

        /// <summary>
        /// Get or set the billing address 2 of the credit card
        /// </summary>
        string BillingAddress2 { get; set; }

        /// <summary>
        /// Get or set the billing city of the credit card
        /// </summary>
        string BillingCity { get; set; }

        /// <summary>
        /// Get or set the billing state or province of the credit card
        /// </summary>
        string BillingState { get; set; }

        /// <summary>
        /// Get or set the billing zip code or postal code of the credit card
        /// </summary>
        string BillingZip { get; set; }

        /// <summary>
        /// Get or set the billing country of the credit card
        /// </summary>
        string BillingCountry { get; set; }
    }
}
