
#region License, Terms and Conditions
//
// ChargifyPage.cs
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
#if NETFULL
    #region Imports
    using System.Configuration;
    using System.Web.UI;
    using Configuration;
    #endregion

    /// <summary>
    /// Base Web.UI.Page that contains the Chargify property accessor
    /// </summary>
    public class ChargifyPage : Page
    {
        /// <summary>
        /// The ChargifyConnect object that allows you to make API calls via Chargify.NET
        /// </summary>
        protected ChargifyConnect Chargify
        {
            get
            {
                if (_chargify == null)
                {
                    // new instance
                    _chargify = new ChargifyConnect();
                    bool azureDeployed = UsefulExtensions.IsRunningAzure();
                    if (!azureDeployed)
                    {
                        ChargifyAccountRetrieverSection config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
                        if (config != null)
                        {
                            ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
                            _chargify.apiKey = accountInfo.ApiKey;
                            _chargify.Password = accountInfo.ApiPassword;
                            _chargify.URL = accountInfo.Site;
                            _chargify.SharedKey = accountInfo.SharedKey;
                            _chargify.UseJSON = config.UseJSON;
                        }
                    }
                    else
                    {
                        // Is azure deployed
                        _chargify.apiKey = UsefulExtensions.GetLateBoundRoleEnvironmentValue("CHARGIFY_API_KEY");
                        _chargify.Password = UsefulExtensions.GetLateBoundRoleEnvironmentValue("CHARGIFY_API_PASSWORD");
                        _chargify.URL = UsefulExtensions.GetLateBoundRoleEnvironmentValue("CHARGIFY_SITE_URL");
                        _chargify.SharedKey = UsefulExtensions.GetLateBoundRoleEnvironmentValue("CHARGIFY_SHARED_KEY");
                        if (!string.IsNullOrEmpty(UsefulExtensions.GetLateBoundRoleEnvironmentValue("CHARGIFY_USE_JSON")))
                        {
                            _chargify.UseJSON = bool.Parse(UsefulExtensions.GetLateBoundRoleEnvironmentValue("CHARGIFY_USE_JSON"));
                        }
                        else
                        {
                            _chargify.UseJSON = false;
                        }
                    }

                }
                return _chargify;
            }
        }
        private ChargifyConnect _chargify;
    }
#endif
}
