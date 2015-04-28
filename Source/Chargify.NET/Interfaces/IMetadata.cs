#region License, Terms and Conditions
//
// IMetadata.cs
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

namespace ChargifyNET
{
    /// <summary>
    /// Chargify Metadata is used to add your own meaningful values to subscription or customer records.
    /// Metadata is associated to a customer or subscription, and corresponds to a Metafield. 
    /// When creating a new metadata object for a given record, if the metafield is not present 
    /// it will be created.
    /// </summary>
    /// <remarks>
    /// Metadata values are limited to 2kB in size. 
    /// Additonally, there are limits on the number of unique "names" available per resource.
    /// </remarks>
    public interface IMetadata
    {
        /// <summary>
        /// The resource id that the metadata belongs to
        /// </summary>
        int ResourceID { get; set; }

        /// <summary>
        /// The value of the attribute that was added to the resource
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// The name of the attribute that is added to the resource
        /// </summary>
        string Name { get; set; }
    }
}