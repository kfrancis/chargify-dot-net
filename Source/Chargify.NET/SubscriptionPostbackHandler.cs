
#region License, Terms and Conditions
//
// SubscriptionPostbackHandler.cs
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
    using System.IO;
    using System.Web;
    #endregion

    /// <summary>
    /// IHttpHandler which processes any postback subscription calls FROM chargify and parses the result.
    /// To implement, inherit this class and override the OnChargifyUpdate method.
    /// </summary>
    public abstract class SubscriptionPostbackHandler : IHttpHandler
    {
        
        #region IHttpHandler Members

        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        /// <summary>
        /// Because of the way that this IHttpHander is setup in the web.config,
        /// <example><add verb="POST" path="Update.axd" type="PostbackHandler" /></example>
        /// this request will only be handling POST updates, which is what the Chargify system will
        /// be sending.
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            StreamReader sr = new StreamReader(context.Request.InputStream);
            string possibleData = sr.ReadToEnd();
            sr.Close();
            if (!string.IsNullOrEmpty(possibleData))
            {
                // strip away the json giving us only the values to pass on ..
                string temp = possibleData.Trim('[', ']');
                string[] values = temp.Split(',');
                OnChargifyUpdate(values);
            }
        }

        /// <summary>
        /// The method that gets called when an update occurs
        /// </summary>
        /// <param name="ids">The list of subscription IDs which have been updated/altered</param>
        public abstract void OnChargifyUpdate(string[] ids);

        #endregion
    }
}
