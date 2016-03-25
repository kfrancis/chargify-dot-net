
#region License, Terms and Conditions
//
// Product.cs
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

namespace Chargify
{
    #region Imports
    using System;
    using System.Xml.Serialization;
    #endregion

    [XmlRoot("product")]
    public class Product : IChargifyEntity
    {
        /// <summary>
        /// The product price, in integer cents
        /// </summary>
        public int price_in_cents { get; set; }

        /// <summary>
        /// The product name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The product API handle
        /// </summary>
        public string handle { get; set; }

        /// <summary>
        /// The product description
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// The product family accounting code (has no bearing in Chargify, may be used within your app)
        /// </summary>
        public string accounting_code { get; set; }

        /// <summary>
        /// A string representing the interval unit for this product, either 'month' or 'day'
        /// </summary>
        public IntervalUnit interval_unit { get; set; }

        /// <summary>
        /// The numerical interval. i.e. an interval of ‘30’ coupled with an interval_unit of ‘day’ would mean this product would renew every 30 days
        /// </summary>
        public int interval { get; set; }

        /// <summary>
        /// Nested attributes pertaining to the product family to which this product belongs
        /// </summary>
        [XmlIgnore]
        public ProductFamily product_family { get; set; }

        /// <summary>
        /// The url to which a customer will be returned after a successful signup
        /// </summary>
        [XmlIgnore]
        public string return_url { get; set; }

        /// <summary>
        /// A numerical interval for the length of the trial period of a subscription to this product. See the description of interval for a description of how this value is coupled with an interval unit to calculate the full interval
        /// </summary>
        [XmlIgnore]
        public int? trial_interval { get; set; }

        /// <summary>
        /// The parameters string we will use in constructing your return URL. See the section on "Return URLs and Parameters" https://docs.chargify.com/product-options#return-url-and-parameters
        /// </summary>
        [XmlIgnore]
        public string return_params { get; set; }

        /// <summary>
        /// A numerical interval for the length a subscription to this product will run before it expires. See the description of interval for a description of how this value is coupled with an interval unit to calculate the full interval
        /// </summary>
        [XmlIgnore]
        public string expiration_interval { get; set; }

        /// <summary>
        /// Timestamp indicating when this product was last updated
        /// </summary>
        [XmlIgnore]
        public DateTime? updated_at { get; set; }

        [XmlIgnore]
        public int id { get; set; }

        [XmlIgnore]
        public string update_return_url { get; set; }

        /// <summary>
        /// Timestamp indicating when this product was created
        /// </summary>
        [XmlIgnore]
        public DateTime? created_at { get; set; }

        /// <summary>
        /// A string representing the trial interval unit for this product, either month or day
        /// </summary>
        [XmlIgnore]
        public string trial_interval_unit { get; set; }

        /// <summary>
        /// A string representing the trial interval unit for this product, either month or day
        /// </summary>
        [XmlIgnore]
        public string expiration_interval_unit { get; set; }

        /// <summary>
        /// Timestamp indicating when this product was archived
        /// </summary>
        [XmlIgnore]
        public DateTime? archived_at { get; set; }

        [XmlIgnore]
        public bool request_credit_card { get; set; }

        [XmlIgnore]
        public bool require_credit_card { get; set; }

        [XmlIgnore]
        public int? initial_charge_in_cents { get; set; }

        [XmlIgnore]
        public int? trial_price_in_cents { get; set; }
    }

    public enum IntervalUnit
    {
        [XmlEnum("month")]
        month,
        [XmlEnum("day")]
        day
    }
}
