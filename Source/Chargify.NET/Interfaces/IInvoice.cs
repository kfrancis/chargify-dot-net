
#region License, Terms and Conditions
//
// IInvoice.cs
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
    /// Invoice Billing allows you to bill your customers manually by sending them an invoice each month. Subscriptions with invoice billing enabled will not be charged automatically.
    /// </summary>
    public interface IInvoice : IChargifyEntity
    {
        /// <summary>
        /// The subscription unique id within Chargify
        /// </summary>
        int SubscriptionID { get; }
        /// <summary>
        /// The statement unique id within Chargify
        /// </summary>
        int StatementID { get; }
        /// <summary>
        /// The site unique id within Chargify
        /// </summary>
        int SiteID { get; }
        /// <summary>
        /// The current state of the subscription associated with this invoice. Please see the documentation for Subscription States
        /// </summary>
        SubscriptionState State { get; }
        /// <summary>
        /// Gives the current invoice amount in the number of cents (ie. the sum of charges, in cents)
        /// </summary>
        int TotalAmountInCents { get; }
        /// <summary>
        /// Gives the current invoice amount in the number of cents (ie. the sum of charges, in dollars and cents)
        /// </summary>
        decimal TotalAmount { get; }
        /// <summary>
        /// The date/time when the invoice was paid in full
        /// </summary>
        DateTime PaidAt { get; }
        /// <summary>
        /// The creation date/time for this invoice
        /// </summary>
        DateTime CreatedAt { get; }
        /// <summary>
        /// The date/time of last update for this invoice
        /// </summary>
        DateTime UpdatedAt { get; }
        /// <summary>
        /// Gives the current outstanding invoice balance in the number of cents
        /// </summary>
        int AmountDueInCents { get; }
        /// <summary>
        /// Gives the current outstanding invoice balance in the number of dollars and cents
        /// </summary>
        decimal AmountDue { get; }
        /// <summary>
        /// The unique (to this site) identifier for this invoice
        /// </summary>
        string Number { get; }
        /// <summary>
        /// A list of charges applied to this invoice
        /// </summary>
        List<ICharge> Charges { get; }
        /// <summary>
        /// A list of the financial transactions that modify the amount due
        /// </summary>
        List<IInvoicePaymentAndCredit> PaymentsAndCredits { get; }
    }
}