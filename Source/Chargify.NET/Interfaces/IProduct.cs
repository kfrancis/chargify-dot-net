
#region License, Terms and Conditions
//
// IProduct.cs
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

// ReSharper disable once CheckNamespace
namespace ChargifyNET
{
    #region Imports
    using System;
using System.Collections.Generic;
    #endregion

    /// <summary>
    /// The interval used for the product
    /// </summary>
    public enum IntervalUnit
    {
        /// <summary>
        /// Day
        /// </summary>
        Day,
        /// <summary>
        /// Month
        /// </summary>
        Month,
        /// <summary>
        /// No expiration
        /// </summary>
        Never,
        /// <summary>
        /// Unknown Transaction Type
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// Interface representing a product.  Subscriptions will subscribe to a product
    /// </summary>
    public interface IProduct : IComparable<IProductFamily>
    {
        /// <summary>
        /// Get the price (in cents)
        /// </summary>
        int PriceInCents { get; set; }

        /// <summary>
        /// Get the price, in dollars and cents.
        /// </summary>
        decimal Price { get; }

        /// <summary>
        /// Get the name of this product
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The ID of the product
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Get the handle to this product
        /// </summary>
        string Handle { get; set; }

        /// <summary>
        /// Get the description of the product
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Get the product family for this product
        /// </summary>
        IProductFamily ProductFamily { get; }

        /// <summary>
        /// Get the accounting code for this product
        /// </summary>
        string AccountingCode { get; set; }

        /// <summary>
        /// Get the interval unit (day, month) for this product
        /// </summary>
        IntervalUnit IntervalUnit { get; set; }

        /// <summary>
        /// Get the renewal interval for this product
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// Get the up front charge for this product, in cents. 
        /// </summary>
        int InitialChargeInCents { get; set; }

        /// <summary>
        /// Get the up front charge for this product, in dollars and cents.
        /// </summary>
        decimal InitialCharge { get; }

        /// <summary>
        /// Get the price of the trial period for a subscription to this product, in cents.
        /// </summary>
        int TrialPriceInCents { get; set; }

        /// <summary>
        /// Get the price of the trial period for a subscription to this product, in dollars and cents.
        /// </summary>
        decimal TrialPrice { get; }

        /// <summary>
        /// A numerical interval for the length of the trial period of a subscription to this product.
        /// </summary>
        int TrialInterval { get; set; }

        /// <summary>
        /// The trial interval unit for this product, either "month" or "day"
        /// </summary>
        IntervalUnit TrialIntervalUnit { get; set; }

        /// <summary>
        /// A numerical interval for the length a subscription to this product will run before it expires.
        /// </summary>
        int ExpirationInterval { get; set; }

        /// <summary>
        /// The expiration interval for this product, either "month" or "day"
        /// </summary>
        IntervalUnit ExpirationIntervalUnit { get; set; }

        /// <summary>
        /// The URL the buyer is returned to after successful purchase.
        /// </summary>
        string ReturnURL { get; set; }

        /// <summary>
        /// The parameter string chargify will use in constructing the return URL.
        /// </summary>
        string ReturnParams { get; set; }

        /// <summary>
        /// This product requires a credit card
        /// </summary>
        bool RequireCreditCard { get; set; }

        /// <summary>
        /// This product requests a credit card
        /// </summary>
        bool RequestCreditCard { get; set; }

        /// <summary>
        /// Timestamp indicating when this product was created.
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// Timestamp indicating when this product was updated.
        /// </summary>
        DateTime UpdatedAt { get; }

        /// <summary>
        /// Timestamp indicating when this product was updated.
        /// </summary>
        DateTime ArchivedAt { get; }

        /// <summary>
        /// List of public signup page URLs and the associated ID
        /// </summary>
        List<IPublicSignupPage> PublicSignupPages { get; }

        /// <summary>
        /// The version of the product
        /// </summary>
        int ProductVersion { get; }

        /// <summary>
        /// Is this product taxable?
        /// </summary>
        bool Taxable { get; }

        /// <summary>
        /// The return url upon update
        /// </summary>
        string UpdateReturnUrl { get; }

        /// <summary>
        /// Paramters for update
        /// </summary>
        string UpdateReturnParams { get; }

        /// <summary>
        /// Will the setup/initial charge be processed after the trial?
        /// </summary>
        bool InitialChargeAfterTrial { get; }
    }
}
