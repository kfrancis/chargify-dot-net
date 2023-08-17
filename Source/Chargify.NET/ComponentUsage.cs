﻿
#region License, Terms and Conditions
//
// Component.cs
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
    using System.Xml;
    using Json;
    #endregion

    /// <summary>
    /// Metered Components are a way to offer Customers a product that is billed on a per-usage basis.
    /// </summary>
    public class ComponentUsage : ChargifyBase, IComponentUsage, IComparable<ComponentUsage>
    {
        #region Field Keys
        private const string IdKey = "id";
        private const string QuantityKey = "quantity";
        private const string MemoKey = "memo";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private ComponentUsage()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentXml">An XML string containing a component node</param>
        public ComponentUsage(string componentXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(componentXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(componentXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "usage")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no component info was found
            throw new ArgumentException("XML does not contain component information", nameof(componentXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="usageNode">An xml node with usage information</param>
        internal ComponentUsage(XmlNode usageNode)
        {
            if (usageNode == null) throw new ArgumentNullException(nameof(usageNode));
            if (usageNode.Name != "usage") throw new ArgumentException("Not a vaild usage node", nameof(usageNode));
            if (usageNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(usageNode));
            LoadFromNode(usageNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="usageObject">An JsonObject with usage information</param>
        public ComponentUsage(JsonObject usageObject)
        {
            if (usageObject == null) throw new ArgumentNullException(nameof(usageObject));
            if (usageObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild usage object", nameof(usageObject));
            LoadFromJson(usageObject);
        }

        private void LoadFromJson(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get product info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IdKey:
                        _id = obj.GetJSONContentAsString(key);
                        break;
                    case QuantityKey:
                        _quantity = obj.GetJSONContentAsInt(key);
                        break;
                    case MemoKey:
                        _memo = obj.GetJSONContentAsString(key);
                        break;
                }
            }
        }

        private void LoadFromNode(XmlNode usageNode)
        {
            // loop through the nodes to get product info
            foreach (XmlNode dataNode in usageNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case IdKey:
                        _id = dataNode.GetNodeContentAsString();
                        break;
                    case QuantityKey:
                        _quantity = dataNode.GetNodeContentAsInt();
                        break;
                    case MemoKey:
                        _memo = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }
        #endregion

        #region IMeteredComponent Members

        /// <summary>
        /// The ID for this metered component
        /// </summary>
        public string ID
        {
            get { return _id; }
        }
        private string _id = string.Empty;

        /// <summary>
        /// The amount of units used
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
        }
        private int _quantity;

        /// <summary>
        /// An optional description for this metered component
        /// </summary>
        public string Memo
        {
            get { return _memo; }
        }
        private string _memo = string.Empty;

        #endregion

        #region IComparable<IComponent> Members

        /// <summary>
        /// Compare this IComponent to another
        /// </summary>
        public int CompareTo(IComponentUsage other)
        {
            return string.Compare(ID, other.ID, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region IComparable<Component> Members

        /// <summary>
        /// Compare this Component to another
        /// </summary>
        public int CompareTo(ComponentUsage other)
        {
            return string.Compare(ID, other.ID, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}
