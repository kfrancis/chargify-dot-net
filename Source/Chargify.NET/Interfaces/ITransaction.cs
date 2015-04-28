
#region License, Terms and Conditions
//
// ITransaction.cs
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
    using System.Linq;
    using System.Text;
    #endregion

    /// <summary>
    /// The specific "subtype" for the transaction_type
    /// </summary>
    public enum TransactionChargeKind
    {
        /// <summary>
        /// A normal charge
        /// </summary>
        Baseline,
        /// <summary>
        /// A one-time charge, captured immediately from payment source (credit card)
        /// </summary>
        One_Time,
        /// <summary>
        /// A one-time charge accrued to the subscription to be captured at next normal billing/renewal
        /// </summary>
        Delay_Capture,
        /// <summary>
        /// A initial/upfront/startup charge added according to the product setup settings
        /// </summary>
        Initial,
        /// <summary>
        /// A charge from usage of a metered component
        /// </summary>
        Metered,
        /// <summary>
        /// A charge from usage of a metered component
        /// </summary>
        Metered_Component,
        /// <summary>
        /// A charge from a quantity-based component allocation
        /// </summary>
        Quantity_Based_Component,
        /// <summary>
        /// A charge from an on/off component allocation
        /// </summary>
        On_Off_Component,
        /// <summary>
        /// A calculated tax charge
        /// </summary>
        Tax
    }

    /// <summary>
    /// The following is a list of available transaction types
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Charge Transaction
        /// </summary>
        Charge,
        /// <summary>
        /// Refund Transaction
        /// </summary>
        Refund,
        /// <summary>
        /// Payment Transaction
        /// </summary>
        Payment,
        /// <summary>
        /// Credit Transaction
        /// </summary>
        Credit,
        /// <summary>
        /// Transaction Authorization
        /// </summary>
        Payment_Authorization,
        /// <summary>
        /// Info Transaction
        /// </summary>
        Info,
        /// <summary>
        /// Adjustment Transaction
        /// </summary>
        Adjustment,
        /// <summary>
        /// Unknown Transaction Type
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// Transaction for a subscription/product for a customer.
    /// </summary>
    public interface ITransaction
    {
        /// <summary>
        /// The type of transaction
        /// </summary>
        TransactionType Type { get; }
        /// <summary>
        /// The unique identifier for the Transaction
        /// </summary>
        int ID { get; }
        /// <summary>
        /// The amount in cents for the Transaction
        /// </summary>
        int AmountInCents { get; }
        /// <summary>
        ///  The amount (in dollars and cents) for the Transaction
        /// </summary>
        decimal Amount { get; }
        /// <summary>
        /// Timestamp indicating when the Transaction was created
        /// </summary>
        DateTime CreatedAt { get; }
        /// <summary>
        /// The initial balance on the subscription before the Transaction has been processed, in cents.
        /// </summary>
        int StartingBalanceInCents { get; }
        /// <summary>
        /// The initial balance on the subscription before the Transaction has been processed, in dollars and cents.
        /// </summary>
        decimal StartingBalance { get; }
        /// <summary>
        /// The remaining balance on the subscription after the Transaction has been processed, in cents.
        /// </summary>
        int EndingBalanceInCents { get; }
        /// <summary>
        /// The remaining balance on the subscription after the Transaction has been processed, in dollars and cents.
        /// </summary>
        decimal EndingBalance { get; }
        /// <summary>
        /// A note about the Transaction
        /// </summary>
        string Memo { get; }
        /// <summary>
        /// The unique identifier for the associated Subscription
        /// </summary>
        int SubscriptionID { get; }
        /// <summary>
        /// The unique identifier for the associated Product
        /// </summary>
        int ProductID { get; }
        /// <summary>
        /// Whether or not the Transaction was successful.
        /// </summary>
        bool Success { get; }
        /// <summary>
        /// The ID of the payment
        /// </summary>
        int PaymentID { get; }
        /// <summary>
        /// The type (kind) of transaction
        /// </summary>
        TransactionChargeKind? Kind { get; }
        /// <summary>
        /// The gateway's ID for this transaction (will be different than the Chargify ID)
        /// </summary>
        string GatewayTransactionID { get; }
        /// <summary>
        /// The gateway's order ID for this transaction
        /// </summary>
        string GatewayOrderID { get; }

    }
}
