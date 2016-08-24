
#region License, Terms and Conditions
//
// IComponentInfo.cs
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
    #endregion

    /// <summary>
    /// The type of component as defined in Chargify's documentation
    /// </summary>
    public enum ComponentType
    {
        /// <summary>
        /// Metered Component
        /// </summary>
        Metered_Component,
        /// <summary>
        /// Quantity Based Component
        /// </summary>
        Quantity_Based_Component,
        /// <summary>
        /// On/Off Component
        /// </summary>
        On_Off_Component,
        /// <summary>
        /// Unknown Transaction Type
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// The type of pricing scheme
    /// </summary>
    public enum PricingSchemeType
    {
        /// <summary>
        /// Per unit based scheme
        /// </summary>
        Per_Unit,
        /// <summary>
        /// Volume based scheme
        /// </summary>
        Volume,
        /// <summary>
        /// Tiered based scheme
        /// </summary>
        Tiered,
        /// <summary>
        /// Stairstep based scheme
        /// </summary>
        Stairstep,
        /// <summary>
        /// Unknown Transaction Type
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// Specfic class when getting information about a component as set to a specific product family
    /// </summary>
    public interface IComponentInfo : IComparable<IComponentInfo>
    {
        /// <summary>
        /// Date and time that this component was created
        /// </summary>
        DateTime CreatedAt { get; }
        /// <summary>
        /// The ID of this component
        /// </summary>
        int ID { get; }
        /// <summary>
        /// The name of the component as created by the Chargify user
        /// </summary>
        string Name { get; }

		/// <summary>
		/// The description of the component as created by the Chargify user
		/// </summary>
		string Description { get; }

        /// <summary>
        /// Price of the component per unit (in cents)
        /// </summary>
        int PricePerUnitInCents { get; }
        /// <summary>
        /// Price of the component per unit (in dollars and cents)
        /// </summary>
        decimal PricePerUnit { get; }
        /// <summary>
        /// The type of pricing scheme setup for this component
        /// </summary>
        PricingSchemeType PricingScheme { get; }
        /// <summary>
        /// The ID of the product family this component was created for
        /// </summary>
        int ProductFamilyID { get; }
        /// <summary>
        /// The name for the unit this component is measured in.
        /// </summary>
        string UnitName { get; }
        /// <summary>
        /// Date/Time that this component was last updated.
        /// </summary>
        DateTime UpdatedAt { get; }
        /// <summary>
        /// The kind of component, either quantity-based or metered component
        /// </summary>
        ComponentType Kind { get; }
        /// <summary>
        /// The amount the customer will be charged per unit. This field is only populated for 'per_unit' pricing schemes.
        /// </summary>
        decimal UnitPrice { get; }
        /// <summary>
        /// An list of price brackets. If the component uses the 'per_unit' pricing scheme, an empty list will be returned.
        /// </summary>
        List<IPriceBracketInfo> Prices { get; }
        /// <summary>
        /// Boolean flag describing whether a component is archived or not
        /// </summary>
        bool Archived { get; }
    }

    /// <summary>
    /// For those compoments that have price brackets, this is the info
    /// </summary>
    public interface IPriceBracketInfo
    {
        /// <summary>
        /// The starting quantity for the component
        /// </summary>
        int StartingQuantity { get; set; }
        /// <summary>
        /// The ending quantity for the component
        /// </summary>
        int EndingQuantity { get; set; }
        /// <summary>
        /// The unit price for the component
        /// </summary>
        decimal UnitPrice { get; set; }
    }
}
