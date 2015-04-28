
#region License, Terms and Conditions
//
// ICoupon.cs
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
    /// Object representing coupon in Chargify
    /// </summary>
    public interface ICoupon : IComparable<ICoupon>
    {
        /// <summary>
        /// The amount of the coupon, in cents.
        /// </summary>
        int AmountInCents { get; set; }

        /// <summary>
        /// The amount of the coupon, in dollars and cents
        /// </summary>
        decimal Amount { get; set; }

        /// <summary>
        /// The string code that represents this coupon
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// The date this coupon was created
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// The description of this coupon
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The date that this coupon is no longer valid for use
        /// </summary>
        DateTime EndDate { get; set; }

        /// <summary>
        /// The ID of this coupon
        /// </summary>
        int ID { get; }

        /// <summary>
        /// The internal name of this coupon in the Chargify site
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The ID of the product family this coupon belongs to
        /// </summary>
        int ProductFamilyID { get; }

        /// <summary>
        /// The date this coupon became active
        /// </summary>
        DateTime StartDate { get; }

        /// <summary>
        /// The date this coupon was last updated
        /// </summary>
        DateTime UpdatedAt { get; }

        /// <summary>
        ///  The date this coupon was archived
        /// </summary>
        DateTime ArchivedAt { get; }

        /// <summary>
        /// The coupon duration interval
        /// </summary>
        int DurationInterval { get; set; }

        /// <summary>
        /// The coupon duration unit 
        /// </summary>
        string DurationUnit { get; set; }

        /// <summary>
        /// The coupon period count
        /// </summary>
        int DurationPeriodCount { get; set; }

        /// <summary>
        /// If percentage based, the percentage. Int.MinValue otherwise.
        /// </summary>
        int Percentage { get; set; }

        /// <summary>
        /// Is this a recurring coupon?
        /// </summary>
        bool IsRecurring { get; set; }

        /// <summary>
        /// Allow negative balance?
        /// </summary>
        bool AllowNegativeBalance { get; set; }
    }
}
