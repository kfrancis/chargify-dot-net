
#region License, Terms and Conditions
//
// ICharge.cs
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
    /// The one time charge class
    /// </summary>
    public interface ICharge : IComparable<ICharge>
    {
        /// <summary>
        /// Either true or false, depending on the success of the charge.
        /// <remarks>At this time, all charges that are returned will return true here. 
        /// Flase may be returned in the future when more options are added to the charge creation API</remarks>
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Get the amount (in cents)
        /// </summary>
        int AmountInCents { get; }

        /// <summary>
        /// Get the amount, in dollars and cents.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// The memo for the created charge
        /// </summary>
        string Memo { get; }

        /// <summary>
        /// The date the charge was created
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// The ending balance of the subscription, in cents
        /// </summary>
        int EndingBalanceInCents { get; }

        /// <summary>
        /// The ending balance of the subscription, in dollars and cents (formatted as decimal)
        /// </summary>
        decimal EndingBalance { get; }
        
        /// <summary>
        /// The ID of the charge
        /// </summary>
        int ID { get; }

        /// <summary>
        /// The kind of charge
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// The ID of the payment associated with this charge
        /// </summary>
        int? PaymentID { get; }

        /// <summary>
        /// The product ID the subscription was subscribed to at the time of the charge
        /// </summary>
        int ProductID { get; }

        /// <summary>
        /// The subscription ID that this charge was applied to
        /// </summary>
        int SubscriptionID { get; }

        /// <summary>
        /// The type of charge
        /// </summary>
        string ChargeType { get; }

        /// <summary>
        /// The type of transaction
        /// </summary>
        string TransactionType { get; }
        
        /// <summary>
        /// The ID of the gateway transaction, useful for debugging.
        /// </summary>
        int? GatewayTransactionID { get; }
    }
}