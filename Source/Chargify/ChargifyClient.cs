
#region License, Terms and Conditions
//
// ChargifyClient.cs
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

namespace Chargify
{
    #region Imports

    

    #endregion

    public class ChargifyClient
    {
        #region Constructors
        public ChargifyClient()
            : this(Config.ApiKey, Config.ApiPassword, Config.UseJson)
        {
        }

        public ChargifyClient(string apiKey, string apiPassword, bool useJson)
        {
            this.Products = new ProductService(apiKey, apiPassword, useJson);
            this.ProductFamilies = new ProductFamilyService(apiKey, apiPassword, useJson);
        }
        #endregion

        #region Accessors

        public ProductService Products { get; private set; }
        public ProductFamilyService ProductFamilies { get; private set; }

        #endregion
    }
}
