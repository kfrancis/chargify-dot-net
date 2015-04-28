
#region License, Terms and Conditions
//
// IRefund.cs
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
    /// A representation of a refund with details about it
    /// </summary>
    public interface IRefund : IComparable<IRefund>
    {
        /// <summary>
        /// (Required) The ID of the Payment that the credit will be applied to
        /// </summary>
        int PaymentID { get; }
        /// <summary>
        /// Was the refund successful?
        /// </summary>
        bool Success { get; }
        /// <summary>
        /// The amount of the refund and captured payment, represented in cents
        /// </summary>
        int AmountInCents { get; }
        /// <summary>
        /// The amount of the refund and captured payment, represented in dollars and cents
        /// </summary>
        decimal Amount { get; }
        /// <summary>
        /// The memo created for the refund
        /// </summary>
        string Memo { get; }
    }
}
