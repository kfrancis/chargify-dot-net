
#region License, Terms and Conditions
//
// IPublicSignupPage.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2015 Clinical Support Systems, Inc. All rights reserved.
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
    /// Public Pages (formerly known as Hosted Pages) are highly customizable white label pages
    /// that you can use as the public-facing side of your subscription business.
    /// </summary>
    public interface IPublicSignupPage : IComparable<IPublicSignupPage>
    {
        /// <summary>
        /// The ID of the public signup page
        /// </summary>
        int ID { get; }

        /// <summary>
        /// The URL of the public signup page
        /// </summary>
        string URL { get; }

        /// <summary>
        /// The url to which a customer will be returned after a successful signup
        /// </summary>
        string ReturnURL { get; }

        /// <summary>
        /// The params to be appended to the ReturnURL
        /// </summary>
        string ReturnParams { get; }
    }
}