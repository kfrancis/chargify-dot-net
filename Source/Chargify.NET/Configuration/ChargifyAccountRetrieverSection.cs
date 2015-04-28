
#region License, Terms and Conditions
//
// ChargifyAccountRetrieverSection.cs
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

namespace ChargifyNET.Configuration
{
    #region Imports
    using System.Configuration;
    using System;
    #endregion

    /// <summary>
    /// Class that deals with getting the account elements in web.config
    /// </summary>
    public class ChargifyAccountRetrieverSection : ConfigurationSection
    {
        /// <summary>
        /// The name of the default account, to be used in GetDefaultOrFirst() method.
        /// </summary>
        [ConfigurationProperty("defaultAccount")]
        public string DefaultAccount
        {
            get { return (string)base["defaultAccount"]; }
            set { base["defaultAccount"] = value; }
        }

        /// <summary>
        /// Should Chargify.NET use JSON or XML. UseJSON = false by default (ie XML by default)
        /// </summary>
        [ConfigurationProperty("useJSON")]
        public bool UseJSON
        {
            get 
            {
                if (base["useJSON"] != null) { return (bool)base["useJSON"]; }
                else { return false; }
            }
            set { base["useJSON"] = value; }
        }

        /// <summary>
        /// The collection of Chargify Account elements
        /// </summary>
        [ConfigurationProperty("accounts", IsDefaultCollection=true)]
        public ChargifyAccountElementCollection Accounts
        {
            get { return (ChargifyAccountElementCollection)this["accounts"]; }
            set { this["accounts"] = value; }
        }

        /// <summary>
        /// Method that gets the default (as specified via the DefaultAccount property) or the first
        /// </summary>
        public ChargifyAccountElement GetDefaultOrFirst()
        {
            ChargifyAccountElement result = null;
            if (!string.IsNullOrEmpty(this.DefaultAccount))
            {
                foreach (ChargifyAccountElement element in this.Accounts)
                {
                    if (element.Name == this.DefaultAccount)
                    {
                        result = element;
                        break;
                    }
                }
            }
            else
            {
                // If there are account elements in the web.config then ..
                if (this.Accounts.Count > 0)
                {
                    result = this.Accounts[0] as ChargifyAccountElement;
                }
            }

            if (result == null) { throw new ConfigurationErrorsException("No accounts listed for Chargify in web.config"); }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Method for getting the shared key for the default site
        /// </summary>
        /// <returns>The shared key for the default site, if applicable</returns>
        public string GetSharedKeyForDefaultOrFirstSite()
        {
            ChargifyAccountElement result = null;
            if (!string.IsNullOrEmpty(this.DefaultAccount))
            {
                foreach (ChargifyAccountElement element in this.Accounts)
                {
                    if (element.Name == this.DefaultAccount)
                    {
                        result = element;
                        break;
                    }
                }
            }
            else
            {
                // If there are account elements in the web.config then ..
                if (this.Accounts.Count > 0)
                {
                    result = this.Accounts[0] as ChargifyAccountElement;
                }
            }

            if (result == null) { throw new ConfigurationErrorsException("No accounts listed for Chargify in web.config"); }
            else
            {
                return result.SharedKey;
            }
        }
    }
}
