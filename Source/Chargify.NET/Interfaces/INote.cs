
#region License, Terms and Conditions
//
// INote.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2011 Clinical Support Systems, Inc. All rights reserved.
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

// ReSharper disable once CheckNamespace
namespace ChargifyNET
{
    #region Imports
    using System;
    #endregion

    /// <summary>
    /// Notes allow you to record information about a particular Subscription in a free text format.
    /// </summary>
    public interface INote : IComparable<INote>
    {
        /// <summary>
        /// The note's unique ID
        /// </summary>
        int ID { get; }

        /// <summary>
        /// The main text context of the note
        /// </summary>
        string Body { get; set; }

        /// <summary>
        /// The ID of the related subscription
        /// </summary>
        int SubscriptionID { get; set; }

        /// <summary>
        /// The date and time the note was created
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// Last update timestamp
        /// </summary>
        DateTime UpdatedAt { get; }

        /// <summary>
        /// Whether or not it is pinned to the top of the list of notes
        /// </summary>
        bool Sticky { get; set; }
    }
}
