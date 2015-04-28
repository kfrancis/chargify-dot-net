
#region License, Terms and Conditions
//
// IBillingManagementInfo.cs
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
    /// From http://docs.chargify.com/api-billing-portal
    /// </summary>
    public interface IBillingManagementInfo
    {
        /// <summary>
        /// The customer's management URL
        /// </summary>
        string URL { get; }
        /// <summary>
        /// Number of times this link has been retrieved (at 15 you will be blocked)
        /// </summary>
        int FetchCount { get; }
        /// <summary>
        /// When this link was created
        /// </summary>
        DateTime CreatedAt { get; }
        /// <summary>
        /// When a new link will be available and fetch_count is reset (15 days from when it was created)
        /// </summary>
        DateTime NewLinkAvailableAt { get; }
        /// <summary>
        /// When this link expires (65 days from when it was created)
        /// </summary>
        DateTime ExpiresAt { get; }
    }
}