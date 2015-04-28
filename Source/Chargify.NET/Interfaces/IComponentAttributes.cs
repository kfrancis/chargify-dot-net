
#region License, Terms and Conditions
//
// IComponentAttributes.cs
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
    /// Specfic class when getting information about a component as set to a specific subscription 
    /// as specified here: http://support.chargify.com/faqs/technical/quantity-based-components
    /// </summary>
    public interface IComponentAttributes
    {
        /// <summary>
        /// The name of the component as created by the Chargify user
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The kind of component, either quantity-based or metered component
        /// </summary>
        string Kind { get; }
        /// <summary>
        /// The ID of the subscription that this component applies to
        /// </summary>
        int SubscriptionID { get; }
        /// <summary>
        /// The ID of the component itself
        /// </summary>
        int ComponentID { get; }
        /// <summary>
        /// The quantity allocated to this subscription
        /// </summary>
        int AllocatedQuantity { get; }
        /// <summary>
        /// The method used to charge, either: per-unit, volume, tiered or stairstep
        /// </summary>
        string PricingScheme { get; }
        /// <summary>
        /// The name for the unit this component is measured in.
        /// </summary>
        string UnitName { get; }
        /// <summary>
        /// The balance of units of this component against the subscription
        /// </summary>
        int UnitBalance { get; }
        /// <summary>
        /// The status of whether this component is enabled or disabled.
        /// (On/Off components only)
        /// </summary>
        bool Enabled { get; }
    }
}
