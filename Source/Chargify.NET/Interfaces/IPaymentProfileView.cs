
#region License, Terms and Conditions
//
// IPaymentProfileView.cs
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
    /// <summary>
    /// Interface representing viewing information for a credit card
    /// </summary>
    public interface IPaymentProfileView : IPaymentProfileBase
    {
        /// <summary>
        /// Get or set the type of the credit card (Visa, MasterCharge. etc.)
        /// </summary>
        string CardType { get; set; }
        /// <summary>
        /// The name of the bank where the customer's account resides
        /// </summary>
        string BankName { get; set; }
        /// <summary>
        /// The routing number of the bank
        /// </summary>
        string MaskedBankRoutingNumber { get; set; }
        /// <summary>
        /// The customer's bank account number
        /// </summary>
        string MaskedBankAccountNumber { get; set; }
        /// <summary>
        /// Either checking or savings
        /// </summary>
        BankAccountType BankAccountType { get; set; }
        /// <summary>
        /// Either personal or business
        /// </summary>
        BankAccountHolderType BankAccountHolderType { get; set; }
    }
}
