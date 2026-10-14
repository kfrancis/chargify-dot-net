#region License, Terms and Conditions
//
// Metafield.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2010-2014 Clinical Support Systems, Inc. All rights reserved.
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

namespace ChargifyDotNet
{
    #region Imports
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;
    #endregion

    /// <summary>
    /// A custom field (metafield) definition for customers or subscriptions
    /// </summary>
    public interface IMetafield
    {
        /// <summary>
        /// The number of records with a value for this field
        /// </summary>
        long DataCount { get; set; }
        /// <summary>
        /// The allowed values, for dropdown and radio input types
        /// </summary>
        object Enum { get; set; }
        /// <summary>
        /// The input type (e.g. "text", "dropdown", "radio")
        /// </summary>
        string InputType { get; set; }
        /// <summary>
        /// The field name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Where the field is shown
        /// </summary>
        MetafieldScope Scope { get; set; }
    }

    /// <summary>
    /// Where a metafield is shown
    /// </summary>
    public interface IMetafieldScope
    {
        /// <summary>
        /// Whether the field is included in CSV exports ("0"/"1")
        /// </summary>
        string Csv { get; set; }

        /// <summary>
        /// The public signup pages that show the field
        /// </summary>
        List<object> Hosted { get; set; }

        /// <summary>
        /// Whether the field is shown on invoices ("0"/"1")
        /// </summary>
        string Invoices { get; set; }

        /// <summary>
        /// Whether the field is shown in the billing portal ("0"/"1")
        /// </summary>
        string Portal { get; set; }

        /// <summary>
        /// Whether the field is shown on statements ("0"/"1")
        /// </summary>
        string Statements { get; set; }
    }

    /// <summary>
    /// A custom field (metafield) definition for customers or subscriptions
    /// </summary>
    public class Metafield : ChargifyBase, IMetafield
    {
        /// <summary>
        /// The number of records with a value for this field
        /// </summary>
        [JsonProperty("data_count")]
        public long DataCount { get; set; }

        /// <summary>
        /// The allowed values, for dropdown and radio input types
        /// </summary>
        [JsonProperty("enum")]
        public object Enum { get; set; } = null!;

        /// <summary>
        /// The input type (e.g. "text", "dropdown", "radio")
        /// </summary>
        [JsonProperty("input_type")]
        public string InputType { get; set; } = null!;

        /// <summary>
        /// The field name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Where the field is shown
        /// </summary>
        [JsonProperty("scope")]
        public MetafieldScope Scope { get; set; } = null!;
    }

    /// <summary>
    /// Where a metafield is shown
    /// </summary>
    public class MetafieldScope : ChargifyBase, IMetafieldScope
    {
        /// <summary>
        /// Whether the field is included in CSV exports ("0"/"1")
        /// </summary>
        [JsonProperty("csv")]
        public string Csv { get; set; } = null!;

        /// <summary>
        /// The public signup pages that show the field
        /// </summary>
        [JsonProperty("hosted")]
        public List<object> Hosted { get; set; } = null!;

        /// <summary>
        /// Whether the field is shown on invoices ("0"/"1")
        /// </summary>
        [JsonProperty("invoices")]
        public string Invoices { get; set; } = null!;

        /// <summary>
        /// Whether the field is shown in the billing portal ("0"/"1")
        /// </summary>
        [JsonProperty("portal")]
        public string Portal { get; set; } = null!;

        /// <summary>
        /// Whether the field is shown on statements ("0"/"1")
        /// </summary>
        [JsonProperty("statements")]
        public string Statements { get; set; } = null!;
    }
}
