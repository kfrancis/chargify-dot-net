
#region License, Terms and Conditions
//
// ProductFamily.cs
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
    using System.Diagnostics;
    using System.Xml;
using ChargifyNET.Json;
    #endregion

    /// <summary>
    /// Class representing a product family
    /// </summary>
    [DebuggerDisplay("ID: {ID}, Name: {Name}, Handle: {Handle}")]
    public class ProductFamily : ProductFamilyAttributes, IProductFamily, IComparable<ProductFamily>
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ProductFamily() : base() { }

        /// <summary>
        /// Constructor, values specified.
        /// </summary>
        /// <param name="Name">The name of the product family</param>
        /// <param name="Description">The description of the product family</param>
        /// <param name="AccountingCode">The accounting number of the product family</param>
        /// <param name="Handle">The handle of the product family</param>
        public ProductFamily(string Name, string Description, string AccountingCode, string Handle)
            : base(Name, Description, AccountingCode, Handle)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ProductFamilyXML">The xml data containing information about the product family (to be parsed)</param>
        public ProductFamily(string ProductFamilyXML)
        {
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(ProductFamilyXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "ProductFamilyXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == "product_family")
                {
                    this.LoadNodeData(elementNode);
                    return;
                }
            }
            // if we get here, then no product family data was found
            throw new ArgumentException("XML does not contain product family information", "ProductFamilyXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productFamilyNode">An xml node containing product family information</param>
        internal ProductFamily(XmlNode productFamilyNode) : base()
        {
            if (productFamilyNode == null) throw new ArgumentNullException("productFamilyNode");
            if (productFamilyNode.Name != "product_family") throw new ArgumentException("Not a vaild product family node", "productFamilyNode");
            if (productFamilyNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "productFamilyNode");
            this.LoadNodeData(productFamilyNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productFamilyObject">JsonObject containing product family info (in expected format)</param>
        public ProductFamily(JsonObject productFamilyObject)
            : base()
        {
            if (productFamilyObject == null) throw new ArgumentNullException("productFamilyObject");
            if (productFamilyObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild product family object. No keys!", "productFamilyObject");
            this.LoadJSONData(productFamilyObject);
        }

        /// <summary>
        /// Load product family data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing product family data</param>
        private void LoadJSONData(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case "accounting_code":
                        this.AccountingCode = obj.GetJSONContentAsString(key);
                        break;
                    case "description":
                        this.Description = obj.GetJSONContentAsString(key);
                        break;
                    case "id":
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case "handle":
                        this.Handle = obj.GetJSONContentAsString(key);
                        break;
                    case "name":
                        this.Name = obj.GetJSONContentAsString(key);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadNodeData(XmlNode familyNode)
        {
            // loop through subnodes to get data
            foreach (XmlNode dataNode in familyNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case "accounting_code":
                        this.AccountingCode = dataNode.GetNodeContentAsString();
                        break;
                    case "description":
                        this.Description = dataNode.GetNodeContentAsString();
                        break;
                    case "id":
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case "handle":
                        this.Handle = dataNode.GetNodeContentAsString();
                        break;
                    case "name":
                        this.Name = dataNode.GetNodeContentAsString();
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region IProductFamily Members


        /// <summary>
        /// Get the id of the product family
        /// </summary>
        public int ID
        {
            get { return _id; }
        }
        private int _id = 0;


        #endregion

        #region Operators

        /// <summary>
        /// Equals operator for two product families
        /// </summary>
        /// <returns>True if the products are equal</returns>
        public static bool operator ==(ProductFamily a, ProductFamily b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            return (a.Handle == b.Handle);
        }

        /// <summary>
        /// Equals operator for two product families
        /// </summary>
        /// <returns>True if the products are equal</returns>
        public static bool operator ==(ProductFamily a, IProductFamily b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            return (a.Handle == b.Handle);
        }

        /// <summary>
        /// Equals operator for two product families
        /// </summary>
        /// <returns>True if the products are equal</returns>
        public static bool operator ==(IProductFamily a, ProductFamily b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            return (a.Handle == b.Handle);
        }

        /// <summary>
        /// Not Equals operator for two product families
        /// </summary>
        /// <returns>True if the products are not equal</returns>
        public static bool operator !=(ProductFamily a, ProductFamily b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Not Equals operator for two product families
        /// </summary>
        /// <returns>True if the products are not equal</returns>
        public static bool operator !=(ProductFamily a, IProductFamily b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Not Equals operator for two product families
        /// </summary>
        /// <returns>True if the products are not equal</returns>
        public static bool operator !=(IProductFamily a, ProductFamily b)
        {
            return !(a == b);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Get Hash code
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (typeof(IProductFamily).IsAssignableFrom(obj.GetType()))
            {
                return (this.Handle == (obj as IProductFamily).Handle);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Convert object to a string
        /// </summary>
        /// <returns>The string representation of the object</returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region IComparable<IProductFamily> Members

        /// <summary>
        /// Compare this instance to another (by Handle)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(IProductFamily other)
        {
            return this.Handle.CompareTo(other.Handle);
        }

        #endregion

        #region IComparable<ProductFamily> Members

        /// <summary>
        /// Compare this instance to another (by Handle)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(ProductFamily other)
        {
            return this.Handle.CompareTo(other.Handle);
        }

        #endregion
    }
}
