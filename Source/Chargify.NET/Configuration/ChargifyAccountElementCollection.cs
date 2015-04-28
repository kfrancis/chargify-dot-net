
#region License, Terms and Conditions
//
// ChargifyAccountElementCollection.cs
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
    #endregion

    /// <summary>
    /// The collection of Chargify Account elements in web.config
    /// </summary>
    [ConfigurationCollection(typeof(ChargifyAccountElement))]
    public class ChargifyAccountElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Create a new configuration element of type ChargifyAccountElement
        /// </summary>        
        protected override ConfigurationElement CreateNewElement()
        {
            return new ChargifyAccountElement();
        }

        /// <summary>
        /// Get the element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ChargifyAccountElement)element).Name;
        }

        /// <summary>
        /// Get the account element by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ChargifyAccountElement this[int index]
        {
            get
            {
                return (ChargifyAccountElement)base.BaseGet(index);
            }
            set
            {
                if (base.BaseGet(index) != null)
                    base.BaseRemoveAt(index);

                this.BaseAdd(index, value);
            }
        }
    }
}
