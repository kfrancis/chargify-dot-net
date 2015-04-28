
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
    using ChargifyNET.Json;
    #endregion

    /// <summary>
    /// Class representing basic attributes for a customer
    /// </summary>
    [DebuggerDisplay("Name: {Name}, SystemID: {SystemID}")]
    [Serializable]
    public class ProductFamilyAttributes : ChargifyBase, IProductFamilyAttributes, IComparable<IProductFamilyAttributes>
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
        public ProductFamilyAttributes() : base()
        {
        }

        /// <summary>
        /// Constructor, all values specified.
        /// </summary>
        /// <param name="Name">The name of the product family</param>
        /// <param name="Description">The description of the product family</param>
        /// <param name="AccountingCode">The accounting code of the product family</param>
        /// <param name="Handle">The handle of the product family</param>
        public ProductFamilyAttributes(string Name, string Description, string AccountingCode, string Handle)
            : this()
        {
            this.Description = Description;
            this.AccountingCode = AccountingCode;
            this.Handle = Handle;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ProductFamilyAttributesXML">The XML which corresponds to this classes members, to be parsed</param>
        public ProductFamilyAttributes(string ProductFamilyAttributesXML)
        {
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(ProductFamilyAttributesXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "ProductFamilyAttributesXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                switch (elementNode.Name)
                {
                    case "product_family_attributes":
                    case "product_family":
                        this.LoadFromNode(elementNode);
                        break;
                    default:
                        break;
                }
            }

            return;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productFamilyAttributesNode">The product family XML node</param>
        internal ProductFamilyAttributes(XmlNode productFamilyAttributesNode)
            : base()
        {
            if (productFamilyAttributesNode == null) throw new ArgumentNullException("productFamilyAttributesNode");
            if (productFamilyAttributesNode.Name != "product_family") throw new ArgumentException("Not a vaild product family attributes node", "productFamilyAttributesNode");
            if (productFamilyAttributesNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "productFamilyAttributesNode");
            this.LoadFromNode(productFamilyAttributesNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productFamilyAttributesObject">The product family JSON object</param>
        public ProductFamilyAttributes(JsonObject productFamilyAttributesObject): base()
        { 
            if (productFamilyAttributesObject == null) throw new ArgumentNullException("productFamilyAttributesObject");
            if (productFamilyAttributesObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild product family attributes object", "productFamilyAttributesObject");
            this.LoadFromJSON(productFamilyAttributesObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing product family attribute data</param>
        private void LoadFromJSON(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case NameKey:
                        this.Name = obj.GetJSONContentAsString(key);
                        break;
                    case DescriptionKey:
                        this.Description = obj.GetJSONContentAsString(key);
                        break;
                    case HandleKey:
                        this.Handle = obj.GetJSONContentAsString(key);
                        break;
                    case AccountingCodeKey:
                        this.AccountingCode = obj.GetJSONContentAsString(key);
                        break;
                    default:
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
                        this.Name = dataNode.GetNodeContentAsString();
                        break;
                    case DescriptionKey:
                        this.Description = dataNode.GetNodeContentAsString();
                        break;
                    case HandleKey:
                        this.Handle = dataNode.GetNodeContentAsString();
                        break;
                    case AccountingCodeKey:
                        this.AccountingCode = dataNode.GetNodeContentAsString();
                        break;
                    default:
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
            return this.Handle.CompareTo(other.Handle);
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
            return this.Handle.CompareTo(other.Handle);
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