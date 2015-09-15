
#region License, Terms and Conditions
//
// IPayment.cs
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
    /// Manual Payment
    /// </summary>
    /// <remarks>https://docs.chargify.com/api-payments</remarks>
    public interface IPayment : IComparable<IPayment>
    {
        /// <summary>
        /// The amount of the payment
        /// </summary>
        int AmountInCents { get; }

        /// <summary>
        /// The amount of the payment (in dollars and cents)
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// The date the payment was created
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// The ending balance of the subscription after the payment
        /// </summary>
        int EndingBalanceInCents { get; }

        /// <summary>
        /// The ID of the payment
        /// </summary>
        int ID { get; }

        /// <summary>
        /// The kind of payment
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// The payment memo
        /// </summary>
        string Memo { get; }

        /// <summary>
        /// The ID of the payment
        /// </summary>
        int? PaymentID { get; }

        /// <summary>
        /// The ID of the product
        /// </summary>
        int ProductID { get; }

        /// <summary>
        /// The balance of the subscription before the payment
        /// </summary>
        int StartingBalanceInCents { get; }

        /// <summary>
        /// The subscription ID
        /// </summary>
        int SubscriptionID { get; }

        /// <summary>
        /// Was the payment successful?
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// The type of payment
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The type of transaction
        /// </summary>
        string TransactionType { get; }

        /// <summary>
        /// The related gateway transaction ID
        /// </summary>
        int? GatewayTransactionID { get; }
    }
}
