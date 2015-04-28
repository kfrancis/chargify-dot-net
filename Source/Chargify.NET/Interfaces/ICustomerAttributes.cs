
#region License, Terms and Conditions
//
// ICustomerAttributes.cs
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
    /// Class representing basic attributes for a customer
    /// </summary>
    public interface ICustomerAttributes : IComparable<ICustomerAttributes>
    {
        /// <summary>
        /// Get or set the customer's first name
        /// </summary>
        string FirstName { get; set; }
        /// <summary>
        /// Get or set the customer's last name
        /// </summary>
        string LastName { get; set; }
        /// <summary>
        /// Get or set the customer's email address
        /// </summary>
        string Email { get; set; }
        /// <summary>
        /// The customer's phone number
        /// </summary>
        string Phone { get; set; }
        /// <summary>
        /// Get or set the customer's organization
        /// </summary>
        string Organization { get; set; }
        /// <summary>
        /// Get or set the customer's ID in the calling system
        /// </summary>
        string SystemID { get; set; }
        /// <summary>
        /// Get the full name LastName FirstName for the customer
        /// </summary>
        string FullName { get; }
        /// <summary>
        /// The customers shipping address
        /// </summary>
        string ShippingAddress { get; set; }
        /// <summary>
        /// The customers shipping address
        /// </summary>
        string ShippingAddress2 { get; set; }
        /// <summary>
        /// The customers shipping city
        /// </summary>
        string ShippingCity { get; set; }
        /// <summary>
        /// The customers shipping zip/postal code
        /// </summary>
        string ShippingZip { get; set; }
        /// <summary>
        /// The customers shipping state
        /// </summary>
        string ShippingState { get; set; }
        /// <summary>
        /// The customers shipping country
        /// </summary>
        string ShippingCountry { get; set; }
    }
}
