
#region License, Terms and Conditions
//
// ISubscriptionPreview.cs
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

namespace ChargifyDotNet
{
    #region Imports
    using System;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Subscription preview
    /// </summary>
    public interface ISubscriptionPreview
    {
        /// <summary>
        /// Subscription preview result
        /// </summary>
        SubscriptionPreviewResult SubscriptionPreviewResult { get; set; }
    }

    /// <summary>
    /// Subscription preview result
    /// </summary>
    public interface ISubscriptionPreviewResult
    {
        /// <summary>
        /// Current billing manifest
        /// </summary>
        SubscriptionPreviewBillingManifest CurrentBillingManifest { get; set; }

        /// <summary>
        /// Next billing manifest
        /// </summary>
        SubscriptionPreviewBillingManifest NextBillingManifest { get; set; }
    }

    /// <summary>
    /// Subscription preview billing manifest
    /// </summary>
    public interface ISubscriptionPreviewBillingManifest
    {
        /// <summary>
        /// The line items
        /// </summary>
        List<SubscriptionPreviewLineItem> LineItems { get; set; }

        /// <summary>
        /// The total, in cents
        /// </summary>
        long TotalInCents { get; set; }

        /// <summary>
        /// The total discount, in cents
        /// </summary>
        long TotalDiscountInCents { get; set; }

        /// <summary>
        /// The total tax, in cents
        /// </summary>
        long TotalTaxInCents { get; set; }

        /// <summary>
        /// The subtotal before discounts and tax, in cents
        /// </summary>
        long SubtotalInCents { get; set; }

        /// <summary>
        /// The start of the billing period
        /// </summary>
        DateTime StartDate { get; set; }

        /// <summary>
        /// The end of the billing period
        /// </summary>
        DateTime EndDate { get; set; }

        /// <summary>
        /// The billing period type (e.g. "recurring", "prorated")
        /// </summary>
        string PeriodType { get; set; }

        /// <summary>
        /// The subscription balance carried into this period, in cents
        /// </summary>
        long ExistingBalanceInCents { get; set; }
    }

    /// <summary>
    /// A line item in a subscription preview billing manifest
    /// </summary>
    public interface ISubscriptionPreviewLineItem
    {
        /// <summary>
        /// The transaction type (e.g. "charge", "credit", "adjustment")
        /// </summary>
        string TransactionType { get; set; }

        /// <summary>
        /// The kind of line item (e.g. "baseline", "component", "trial")
        /// </summary>
        string Kind { get; set; }

        /// <summary>
        /// The amount, in cents
        /// </summary>
        long AmountInCents { get; set; }

        /// <summary>
        /// The line item description
        /// </summary>
        string Memo { get; set; }

        /// <summary>
        /// The discount applied, in cents
        /// </summary>
        long DiscountAmountInCents { get; set; }

        /// <summary>
        /// The taxable amount, in cents
        /// </summary>
        long TaxableAmountInCents { get; set; }
    }
}
