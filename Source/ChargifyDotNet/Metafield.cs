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

    public interface IMetafield
    {
        long DataCount { get; set; }
        object Enum { get; set; }
        string InputType { get; set; }
        string Name { get; set; }
        MetafieldScope Scope { get; set; }
    }

    public interface IMetafieldScope
    {
        string Csv { get; set; }

        List<object> Hosted { get; set; }

        string Invoices { get; set; }

        string Portal { get; set; }

        string Statements { get; set; }
    }

    public class Metafield : ChargifyBase, IMetafield
    {
        [JsonProperty("data_count")]
        public long DataCount { get; set; }

        [JsonProperty("enum")]
        public object Enum { get; set; }

        [JsonProperty("input_type")]
        public string InputType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("scope")]
        public MetafieldScope Scope { get; set; }
    }

    public class MetafieldScope : ChargifyBase, IMetafieldScope
    {
        [JsonProperty("csv")]
        public string Csv { get; set; }

        [JsonProperty("hosted")]
        public List<object> Hosted { get; set; }

        [JsonProperty("invoices")]
        public string Invoices { get; set; }

        [JsonProperty("portal")]
        public string Portal { get; set; }

        [JsonProperty("statements")]
        public string Statements { get; set; }
    }
}
