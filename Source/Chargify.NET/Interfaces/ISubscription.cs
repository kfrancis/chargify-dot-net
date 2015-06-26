
#region License, Terms and Conditions
//
// ISubscription.cs
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
    /// Subscriptions can have two modes now, automatic and invoicing.
    /// </summary>
    public enum PaymentCollectionMethod
    {
        /// <summary>
        /// This is normal recurring credit card billing
        /// </summary>
        Automatic,
        /// <summary>
        /// Invoices are issued to users, paid and organized by staff
        /// </summary>
        Invoice,
        /// <summary>
        /// The default state if the value could not be parsed, or wasn't sent.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// The state of a Chargify subscription
    /// http://docs.chargify.com/subscription-states
    /// </summary>
    public enum SubscriptionState
    {
        /// <summary>
        /// A trialing subscription is valid, and may transition to 'active' once the trial is ended and payment is recieved.
        /// </summary>
        Trialing,
        /// <summary>
        /// Indicates that the subscription was on a trial, and that the trial time has expired.
        /// </summary>
        Trial_Ended,
        /// <summary>
        /// The 'assessing' subscription state
        /// </summary>
        Assessing,
        /// <summary>
        /// The 'active' subscription state
        /// </summary>
        Active,
        /// <summary>
        /// The 'soft_failure' subscription state
        /// </summary>
        Soft_Failure,
        /// <summary>
        /// The 'past_due' subscription state
        /// </summary>
        Past_Due,
        /// <summary>
        /// The 'suspended' subscription state
        /// </summary>
        Suspended,
        /// <summary>
        /// The 'canceled' subscription state
        /// </summary>
        Canceled,
        /// <summary>
        /// The 'expired' subscription state
        /// </summary>
        Expired,
        /// <summary>
        /// The 'unpaid' subscription state
        /// </summary>
        Unpaid,
        /// <summary>
        /// The 'paid' subscription state
        /// </summary>
        Paid,
        /// <summary>
        /// The 'parital' invoice state
        /// </summary>
        Partial,
        /// <summary>
        /// The 'unknown' subscription state, only internal to this wrapper
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// Interface representing an existing Chargify subscription
    /// </summary>
    public interface ISubscription : IComparable<ISubscription>
    {
        /// <summary>
        /// Timestamp for when the subscription began
        /// <remarks>When it came out of trial, or when it began in the case of no trial period</remarks>
        /// </summary>
        DateTime ActivatedAt { get; }
        /// <summary>
        /// Get the current outstanding subscription balance, in the number of cents.
        /// </summary>
        int BalanceInCents { get; }
        /// <summary>
        /// Get the current outstanding subscription balance, in dollars and cents.
        /// </summary>
        decimal Balance { get; }
        /// <summary>
        /// Is this subscription going to automatically cancel at the end of the current period?
        /// </summary>
        bool CancelAtEndOfPeriod { get; }
        /// <summary>
        /// Seller-provided reason for, or note about, the cancellation.
        /// </summary>
        string CancellationMessage { get; }
        /// <summary>
        /// Get the date and time the subscription was created a Chargify
        /// </summary>
        DateTime Created { get; }
        /// <summary>
        /// Get the date and time relating to the end of the current (recurring) period
        /// <remarks>(ie. when the next regularily scheduled attemped charge will occur)</remarks>
        /// </summary>
        DateTime CurrentPeriodEndsAt { get; }
        /// <summary>
        /// Timestamp giving the expiration date of this subscription (if any)
        /// </summary>
        DateTime ExpiresAt { get; }
        /// <summary>
        /// Get the subscription unique id within Chargify
        /// </summary>
        int SubscriptionID { get; }
        /// <summary>
        /// Get the date and time that indicates when capture of payment will be tried or retried.
        /// </summary>
        DateTime NextAssessmentAt { get; }
        /// <summary>
        /// The type of billing used for this subscription
        /// </summary>
        PaymentCollectionMethod PaymentCollectionMethod { get; }
        /// <summary>
        /// Get the current state of the subscription.
        /// </summary>
        SubscriptionState State { get; }
        /// <summary>
        /// Get the date and time relating to when the trial period (if any) ended
        /// </summary>
        DateTime TrialEndedAt { get; }
        /// <summary>
        /// Get the date and time relating to when the trial period (if any) began
        /// </summary>
        DateTime TrialStartedAt { get; }
        /// <summary>
        /// Get the date and time the subscription was last updated at chargify
        /// </summary>
        DateTime LastUpdated { get; }
        /// <summary>
        /// Get the date and time relating to the start of the current (recurring) period.
        /// </summary>
        DateTime CurrentPeriodStartedAt { get; }
        /// <summary>
        ///  The previous state of this subscription
        /// </summary>
        SubscriptionState PreviousState { get; }
        /// <summary>
        /// The ID of the corresponding payment transaction
        /// </summary>
        int SignupPaymentID { get; }
        /// <summary>
        /// The total subscription revenue (in dollars and cents)
        /// </summary>
        decimal TotalRevenue { get; }
        /// <summary>
        /// The total subscription revenue (in cents)
        /// </summary>
        int TotalRevenueInCents { get; }
        /// <summary>
        /// The revenue accepted upon signup
        /// </summary>
        decimal SignupRevenue { get; }
        /// <summary>
        /// Get the date and time relating to the time the subscription was cancelled due to a "delayed cancel"
        /// </summary>
        DateTime DelayedCancelAt { get; }
        /// <summary>
        /// Get the product for this subscription
        /// </summary>
        IProduct Product { get; }
        /// <summary>
        /// Get the credit card details for this subscription
        /// </summary>
        IPaymentProfileView PaymentProfile { get; }
        /// <summary>
        /// Get the customer details for this subscription
        /// </summary>
        ICustomer Customer { get; }
        /// <summary>
        /// Get the date the subscription was cancelled
        /// </summary>
        DateTime CanceledAt { get; }
        /// <summary>
        /// Get the coupon code currently applied (if applicable) to the subscription
        /// </summary>
        string CouponCode { get; }
        /// <summary>
        /// The version of the product currently subscribed. NOTE: we have not exposed versions 
        /// (yet) elsewhere in the API, but if you change the price of your product the versions 
        /// will increment and existing subscriptions will remain on prior versions (by default, 
        /// to support price grandfathering).
        /// </summary>
        int ProductVersionNumber { get; }
        /// <summary>
        /// At what price was the product on when initial subscribed? (in cents)
        /// </summary>
        int ProductPriceInCents { get; }
        /// <summary>
        /// At what price was the product on when initial subscribed? (in dollars and cents)
        /// </summary>
        decimal ProductPrice { get; }
    }
}
