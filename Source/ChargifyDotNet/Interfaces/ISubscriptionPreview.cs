
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

namespace ChargifyNET
{
    #region Imports
    using System;
    using System.Collections.Generic;
    #endregion

    public interface ISubscriptionPreview
    {
        SubscriptionPreviewResult SubscriptionPreviewResult { get; set; }
    }

    public interface ISubscriptionPreviewResult
    {
        SubscriptionPreviewBillingManifest CurrentBillingManifest { get; set; }

        SubscriptionPreviewBillingManifest NextBillingManifest { get; set; }
    }

    public interface ISubscriptionPreviewBillingManifest
    {
        List<SubscriptionPreviewLineItem> LineItems { get; set; }

        long TotalInCents { get; set; }

        long TotalDiscountInCents { get; set; }

        long TotalTaxInCents { get; set; }

        long SubtotalInCents { get; set; }

        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

        string PeriodType { get; set; }

        long ExistingBalanceInCents { get; set; }
    }

    public interface ISubscriptionPreviewLineItem
    {
        string TransactionType { get; set; }

        string Kind { get; set; }

        long AmountInCents { get; set; }

        string Memo { get; set; }

        long DiscountAmountInCents { get; set; }

        long TaxableAmountInCents { get; set; }
    }
}
