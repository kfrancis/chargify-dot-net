
#region License, Terms and Conditions
//
// IAdjustment.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2011 Clinical Support Systems, Inc. All rights reserved.
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
    /// The method to use when adjusting
    /// </summary>
    public enum AdjustmentMethod
    {
        /// <summary>
        /// Add (or subtract) from the subscription balance
        /// </summary>
        Default,
        /// <summary>
        /// Set the subscription balance to the AmountInCents
        /// </summary>
        Target
    }

    /// <summary>
    /// Adjustments allow you to change the current balance of a subscription.
    /// </summary>
    public interface IAdjustment : IComparable<IAdjustment>
    {
        /// <summary>
        /// The ID of the adjustment
        /// </summary>
        int ID { get; }
        /// <summary>
        /// (Currently, all adjustments return as successful
        /// </summary>
        bool Success { get; }
        /// <summary>
        /// A helpful explaination for the adjustment
        /// </summary>
        string Memo { get; }
        /// <summary>
        /// The amount of the adjustment (in cents)
        /// </summary>
        int AmountInCents { get; }
        /// <summary>
        /// The amount of the adjustment
        /// </summary>
        decimal Amount { get; }
        /// <summary>
        /// The subscription balance after the adjustment (in cents)
        /// </summary>
        int EndingBalanceInCents { get; }
        /// <summary>
        /// The subscription balance after the adjustment
        /// </summary>
        decimal EndingBalance { get; }
        /// <summary>
        /// The type of the adjustment
        /// </summary>
        string Type { get; }
        /// <summary>
        /// The type of transaction done by the adjustment
        /// </summary>
        TransactionType TransactionType { get; }
        /// <summary>
        /// The subscription the adjustment was created for
        /// </summary>
        int SubscriptionID { get; }
        /// <summary>
        /// The subscribed product at the time of the adjustment
        /// </summary>
        int ProductID { get; }
        /// <summary>
        /// The date the adjustment was created
        /// </summary>
        DateTime CreatedAt { get; }
    }
}
