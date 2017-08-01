#region License, Terms and Conditions
//
// HttpRequestMethod.cs
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
    /// <summary>
    /// The type of REST request
    /// </summary>
    public enum HttpRequestMethod
    {
        /// <summary>
        /// Requests a representation of the specified resource
        /// </summary>
        Get,
        /// <summary>
        /// Requests that the server accept the entity enclosed in the request as a new subordinate of the web resource identified by the URI
        /// </summary>
        Post,
        /// <summary>
        /// Requests that the enclosed entity be stored under the supplied URI
        /// </summary>
        Put,
        /// <summary>
        /// Deletes the specified resource
        /// </summary>
        Delete
    }

    /// <summary>
    /// The type of payment profile
    /// </summary>
    public enum PaymentProfileType
    {
        /// <summary>
        /// Credit card
        /// </summary>
        [XmlEnum(Name = "credit_card")]
        Credit_Card,
        /// <summary>
        /// Direct bank account
        /// </summary>
        [XmlEnum(Name = "bank_account")]
        Bank_Account,
        /// <summary>
        /// Paypal account
        /// </summary>
        [XmlEnum(Name = "paypal")]
        PayPal_Account
    }
}