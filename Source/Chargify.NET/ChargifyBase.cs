
#region License, Terms and Conditions
//
// ChargifyBase.cs
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

//setting CLSCompliant attribute to true
using System;
[assembly:CLSCompliant(true)]

namespace ChargifyNET
{
    #region Imports
    using System.Web;
    #endregion

    /// <summary>
    /// Base class for all chargify objects
    /// </summary>
    [Serializable]
    [CLSCompliant(true)]
    public class ChargifyBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ChargifyBase() { }

        /// <summary>
        /// Convert this object to an HTML formatted string
        /// </summary>
        /// <returns>The object represented as an HTML formatted string</returns>
        public virtual string ToHTMLString()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.HtmlEncode(this.ToString()).Replace("\n\t", "<br>&nbsp;&nbsp;&nbsp;&middot;&nbsp;").Replace("\n", "<br>").Replace(" ", "&nbsp;");
            }
            else
            {
                return this.ToString().Replace("\n\t", "<br>&nbsp;&nbsp;&nbsp;&middot;&nbsp;").Replace("\n", "<br>").Replace(" ", "&nbsp;");
            }
        }
    }
}
