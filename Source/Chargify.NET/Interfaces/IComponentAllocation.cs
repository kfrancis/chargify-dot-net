
#region License, Terms and Conditions
//
// IComponentAllocation.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2013 Clinical Support Systems, Inc. All rights reserved.
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
    /// The scheme used if the proration was an upgrade. This is only present when the allocation was created mid-period.
    /// </summary>
    /// <remarks>
    /// The API uses hyphens for word seperation, I use underscores and replace the hyphens with 
    /// underscores during parsing so I can parse the enumerated values
    /// </remarks>
    public enum ComponentUpgradeProrationScheme
    {
        /// <summary>
        /// A charge is added for the prorated amount due, but the card is not charged until the subscription’s next renewal
        /// </summary>
        Prorate_Delay_Capture,
        /// <summary>
        /// A charge is added and we attempt to charge the credit card on file. If it fails, the charge will be accrued until the next renewal.
        /// </summary>
        Prorate_Attempt_Capture,
        /// <summary>
        /// No charge is added.
        /// </summary>
        No_Prorate,
        /// <summary>
        /// No value (internal to this library)
        /// </summary>
        Unknown
    }

    /// <summary>
    /// The scheme used if the proration was a downgrade. This is only present when the allocation was created mid-period.
    /// </summary>
    /// <remarks>
    /// The API uses hyphens for word seperation, I use underscores and replace the hyphens with 
    /// underscores during parsing so I can parse the enumerated values
    /// </remarks>
    public enum ComponentDowngradeProrationScheme
    {
        /// <summary>
        /// A credit is added for the amount owed.
        /// </summary>
        Prorate,
        /// <summary>
        /// No credit is added
        /// </summary>
        No_Prorate,
        /// <summary>
        /// No value (internal to this library)
        /// </summary>
        Unknown
    }

    /// <summary>
    /// llocations describe a change to the allocated quantity for a particular Component (either Quantity-Based or On/Off) for a particular Subscription.
    /// </summary>
    public interface IComponentAllocation
    {
        /// <summary>
        /// The allocated quantity set in to effect by the allocation
        /// </summary>
        int Quantity { get; set; }
        /// <summary>
        /// The allocated quantity that was in effect before this allocation was created
        /// </summary>
        int PreviousQuantity { get; }
        /// <summary>
        /// The integer component ID for the allocation. This references a component that you have created in your Product setup
        /// </summary>
        int ComponentID { get; }
        /// <summary>
        /// The integer subscription ID for the allocation. This references a unique subscription in your Site
        /// </summary>
        int SubscriptionID { get; }
        /// <summary>
        /// The memo passed when the allocation was created
        /// </summary>
        string Memo { get; set; }
        /// <summary>
        /// The time that the allocation was recorded, in ISO 8601 format and UTC timezone, i.e. 2012-11-20T22:00:37Z
        /// </summary>
        DateTime TimeStamp { get; }
        /// <summary>
        /// The scheme used if the proration was an upgrade. This is only present when the allocation was created mid-period.
        /// </summary>
        ComponentUpgradeProrationScheme UpgradeScheme { get; set; }
        /// <summary>
        /// The scheme used if the proration was a downgrade. This is only present when the allocation was created mid-period.
        /// </summary>
        ComponentDowngradeProrationScheme DowngradeScheme { get; set; }
    }
}