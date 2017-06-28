
#region License, Terms and Conditions
//
// ProductFamilyAttributes.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2013 Clinical Support Systems, Inc. All rights reserved.
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
    using System.Diagnostics;
    using System.Xml;
    using Json;
    #endregion

    /// <summary>
    /// Class representing basic attributes for a customer
    /// </summary>
    [DebuggerDisplay("Name: {Name}, Handle: {Handle}")]
    [Serializable]
    public class ProductFamilyAttributes : ChargifyBase, IProductFamilyAttributes
    {
        #region Field Keys
        internal const string NameKey = "name";
        internal const string DescriptionKey = "description";
        internal const string HandleKey = "handle";
        internal const string AccountingCodeKey = "accounting_code";
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductFamilyAttributes()
        {
        }

        /// <summary>
        /// Constructor, all values specified.
        /// </summary>
        /// <param name="name">The name of the product family</param>
        /// <param name="description">The description of the product family</param>
        /// <param name="accountingCode">The accounting code of the product family</param>
        /// <param name="handle">The handle of the product family</param>
        public ProductFamilyAttributes(string name, string description, string accountingCode, string handle)
            : this()
        {
            Name = name;
            Description = description;
            AccountingCode = accountingCode;
            Handle = handle;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productFamilyAttributesXml">The XML which corresponds to this classes members, to be parsed</param>
        public ProductFamilyAttributes(string productFamilyAttributesXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(productFamilyAttributesXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(productFamilyAttributesXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                switch (elementNode.Name)
                {
                    case "product_family_attributes":
                    case "product_family":
                        LoadFromNode(elementNode);
                        break;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain information", nameof(productFamilyAttributesXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productFamilyAttributesNode">The product family XML node</param>
        internal ProductFamilyAttributes(XmlNode productFamilyAttributesNode)
        {
            if (productFamilyAttributesNode == null) throw new ArgumentNullException(nameof(productFamilyAttributesNode));
            if (productFamilyAttributesNode.Name != "product_family") throw new ArgumentException("Not a vaild product family attributes node", nameof(productFamilyAttributesNode));
            if (productFamilyAttributesNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(productFamilyAttributesNode));
            LoadFromNode(productFamilyAttributesNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productFamilyAttributesObject">The product family JSON object</param>
        public ProductFamilyAttributes(JsonObject productFamilyAttributesObject)
        {
            if (productFamilyAttributesObject == null) throw new ArgumentNullException(nameof(productFamilyAttributesObject));
            if (productFamilyAttributesObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild product family attributes object", nameof(productFamilyAttributesObject));
            LoadFromJson(productFamilyAttributesObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing product family attribute data</param>
        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case NameKey:
                        Name = obj.GetJSONContentAsString(key);
                        break;
                    case DescriptionKey:
                        Description = obj.GetJSONContentAsString(key);
                        break;
                    case HandleKey:
                        Handle = obj.GetJSONContentAsString(key);
                        break;
                    case AccountingCodeKey:
                        AccountingCode = obj.GetJSONContentAsString(key);
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a product family  node
        /// </summary>
        /// <param name="customerNode">The product family node</param>
        private void LoadFromNode(XmlNode customerNode)
        {
            foreach (XmlNode dataNode in customerNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case NameKey:
                        Name = dataNode.GetNodeContentAsString();
                        break;
                    case DescriptionKey:
                        Description = dataNode.GetNodeContentAsString();
                        break;
                    case HandleKey:
                        Handle = dataNode.GetNodeContentAsString();
                        break;
                    case AccountingCodeKey:
                        AccountingCode = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }

        #endregion

        #region IComparable<IProductFamilyAttributes> Members
        /// <summary>
        /// Compare this instance to another (by Handle)
        /// </summary>
        /// <param name="other">The other instance to compare against</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(IProductFamilyAttributes other)
        {
            return string.Compare(Handle, other.Handle, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region IComparable<ProductFamilyAttributes> Members
        /// <summary>
        /// Compare this instance to another (by Handle)
        /// </summary>
        /// <param name="other">The other instance to compare against</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(ProductFamilyAttributes other)
        {
            return string.Compare(Handle, other.Handle, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region IProductFamilyAttribute Members
        /// <summary>
        /// The accounting code of the product family
        /// </summary>
        public string AccountingCode { get; set; }

        /// <summary>
        /// The descrition of the product family
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The handle of the product family
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// The name of the product family
        /// </summary>
        public string Name { get; set; }
        #endregion
    }
}