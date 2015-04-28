
#region License, Terms and Conditions
//
// IStatement.cs
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
    #endregion

    /// <summary>
    /// The statment object.
    /// http://docs.chargify.com/statements
    /// </summary>
    public interface IStatement : IComparable<IStatement>
    {
        /// <summary>
        /// The unique identifier for this statement within Chargify
        /// </summary>
        int ID { get; }
        /// <summary>
        /// The unique identifier of the subscription associated with this statement
        /// </summary>
        int SubscriptionID { get; }
        /// <summary>
        /// The date that the statement was opened
        /// </summary>
        DateTime OpenedAt { get; }
        /// <summary>
        /// The date that the statement was closed
        /// </summary>
        DateTime ClosedAt { get; }
        /// <summary>
        /// The date that the statement was settled
        /// </summary>
        DateTime SettledAt { get; }
        /// <summary>
        /// A text representation of the statement
        /// </summary>
        string TextView { get; }
        /// <summary>
        /// A simple HTML representation of the statement
        /// </summary>
        string BasicHtmlView { get; }
        /// <summary>
        /// A more rebust HTML representation of the statment
        /// </summary>
        string HtmlView { get; }
        /// <summary>
        /// A collection of payments from future sttments that pay charges on this statement
        /// </summary>
        object FuturePayments { get; }
        /// <summary>
        /// The subscription's balance at the time the statement was opened (in cents)
        /// </summary>
        int StartingBalanceInCents { get; }
        /// <summary>
        /// The subscription's balance at the time the statement was opened (in dollars and cents)
        /// </summary>
        decimal StartingBalance { get; }
        /// <summary>
        /// The subscription's balance at the time the statement was closed (in cents)
        /// </summary>
        int EndingBalanceInCents { get; }
        /// <summary>
        /// The subscription's balance at the time the statement was closed (in dollars and cents)
        /// </summary>
        decimal EndingBalance { get; }
        /// <summary>
        /// The customer's first name
        /// </summary>
        string CustomerFirstName { get; }
        /// <summary>
        /// The customer's last name
        /// </summary>
        string CustomerLastName { get; }
        /// <summary>
        /// The customer's organization
        /// </summary>
        string CustomerOrganization { get; }
        /// <summary>
        /// The customer's shipping address, line 1
        /// </summary>
        string CustomerShippingAddress { get; }
        /// <summary>
        /// The customer's shipping address, line 2
        /// </summary>
        string CustomerShippingAddress2 { get; }
        /// <summary>
        /// The customer's shipping city
        /// </summary>
        string CustomerShippingCity { get; }
        /// <summary>
        /// The customer's shipping state or province
        /// </summary>
        string CustomerShippingState { get; }
        /// <summary>
        /// The customer's shipping country
        /// </summary>
        string CustomerShippingCountry { get; }
        /// <summary>
        /// The customer's shipping postal code or zip
        /// </summary>
        string CustomerShippingZip { get; }
        /// <summary>
        /// The customer's billing address, line 1
        /// </summary>
        string CustomerBillingAddress { get; }
        /// <summary>
        /// The customer's billing address, line 2
        /// </summary>
        string CustomerBillingAddress2 { get; }
        /// <summary>
        /// The customer's billing city
        /// </summary>
        string CustomerBillingCity { get; }
        /// <summary>
        /// The customer's billing state or province
        /// </summary>
        string CustomerBillingState { get; }
        /// <summary>
        /// The customer's billing country
        /// </summary>
        string CustomerBillingCountry { get; }
        /// <summary>
        /// The customer's billing postal code or zip
        /// </summary>
        string CustomerBillingZip { get; }
        /// <summary>
        /// A collection of the transactions associated with the statement
        /// </summary>
        List<ITransaction> Transactions { get; }
        /// <summary>
        /// A collection of the events associated with the statement
        /// </summary>
        object Events { get; }
        /// <summary>
        /// The creation date for this statement
        /// </summary>
        DateTime CreatedAt { get; }
        /// <summary>
        /// The date of last update for this statement
        /// </summary>
        DateTime UpdatedAt { get; }
    }
}
