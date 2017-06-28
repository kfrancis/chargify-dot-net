
#region License, Terms and Conditions
//
// Usage.cs
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
    /// A usage is a record of a customer using a metered component
    /// </summary>
    public class Usage : ChargifyBase, IUsage, IComparable<Usage>
    {
        #region Constructors
        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Usage()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="usageXml">XML containing usage info (in expected format)</param>
        public Usage(string usageXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(usageXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(usageXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "usage")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain charge information", nameof(usageXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="usageNode">XML containing usage info (in expected format)</param>
        internal Usage(XmlNode usageNode)
        {
            if (usageNode == null) throw new ArgumentNullException(nameof(usageNode));
            if (usageNode.Name != "usage") throw new ArgumentException("Not a vaild usage node", nameof(usageNode));
            if (usageNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(usageNode));
            LoadFromNode(usageNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="usageObject">JsonObject containing usage info (in expected format)</param>
        public Usage(JsonObject usageObject)
        {
            if (usageObject == null) throw new ArgumentNullException(nameof(usageObject));
            if (usageObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild usage object", nameof(usageObject));
            LoadFromJson(usageObject);
        }

        /// <summary>
        /// Load usage data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing usage data</param>
        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case "id":
                        _id = obj.GetJSONContentAsString(key);
                        break;
                    case "memo":
                        _memo = obj.GetJSONContentAsString(key);
                        break;
                    case "quantity":
                        _quantity = obj.GetJSONContentAsInt(key);
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a subscription node
        /// </summary>
        /// <param name="subscriptionNode">The subscription node</param>
        private void LoadFromNode(XmlNode subscriptionNode)
        {
            foreach (XmlNode dataNode in subscriptionNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case "id":
                        _id = dataNode.GetNodeContentAsString();
                        break;
                    case "memo":
                        _memo = dataNode.GetNodeContentAsString();
                        break;
                    case "quantity":
                        _quantity = dataNode.GetNodeContentAsInt();
                        break;
                }
            }
        }
        #endregion

        #region IUsage Members

        /// <summary>
        /// The ID of the usage element
        /// </summary>
        public string ID
        {
            get { return _id; }
        }
        private string _id = string.Empty;

        /// <summary>
        /// The usage quantity
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
        }
        private int _quantity = int.MinValue;

        /// <summary>
        /// A note containing information about the usage
        /// </summary>
        public string Memo
        {
            get { return _memo; }
        }
        private string _memo = string.Empty;

        #endregion

        #region IComparable<IUsage> Members

        /// <summary>
        /// Compare this usage to another usage
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Usage other)
        {
            return string.Compare(ID, other.ID, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Compare this usage to another usage
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IUsage other)
        {
            return string.Compare(ID, other.ID, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}
