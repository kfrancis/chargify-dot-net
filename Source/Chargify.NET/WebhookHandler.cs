#region License, Terms and Conditions
//
// WebhookHandler.cs
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
    /// IHttpHandler which processes any webhook responses FROM chargify.
    /// To implement, inherit this class and override the OnChargifyUpdate method.
    /// </summary>
    public abstract class WebhookHandler : IHttpHandler
    {
        private const string WebhookIdHandle = "X-Chargify-Webhook-Id";
        private const string SignatureHeaderHandle = "X-Chargify-Webhook-Signature";
        private const string SignatureQueryStringHandle = "signature";

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
        /// <example><add verb="POST" path="Update.axd" type="WebhookHandler" /></example>
        /// this request will only be handling POST updates, which is what the Chargify system will
        /// be sending.
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            string possibleData = string.Empty;
            using (StreamReader sr = new StreamReader(context.Request.InputStream))
            {
                possibleData = sr.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(possibleData))
            {
                string signature = context.Request.Headers[SignatureHeaderHandle];
                if (string.IsNullOrEmpty(signature))
                {
                    signature = context.Request.QueryString[SignatureQueryStringHandle];
                }
                // Grab the webhook id as well, since it'll be used for validation.
                int webhookID = int.Parse(context.Request.Headers[WebhookIdHandle]);

                // Now that we have data, signature and webhook id, pass it back.
                OnChargifyUpdate(webhookID, signature, possibleData);
            }
        }

        /// <summary>
        /// Method that gets called when Chargify sends a webhook response to this handler
        /// </summary>
        /// <param name="webhookID">The webhook ID (used for verification)</param>
        /// <param name="signature">The signature that was passed with the data</param>
        /// <param name="data">They data they are sending</param>
        public abstract void OnChargifyUpdate(int webhookID, string signature, string data);

        #endregion
    }
}
