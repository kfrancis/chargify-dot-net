#region License, Terms and Conditions
//
// Metadata.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2010-2014 Clinical Support Systems, Inc. All rights reserved.
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
    #endregion

    /// <summary>
    /// Chargify Metadata is used to add your own meaningful values to subscription or customer records.
    /// Metadata is associated to a customer or subscription, and corresponds to a Metafield. 
    /// When creating a new metadata object for a given record, if the metafield is not present 
    /// it will be created.
    /// </summary>
    /// <remarks>
    /// Metadata values are limited to 2kB in size. 
    /// Additonally, there are limits on the number of unique "names" available per resource.
    /// </remarks>
    public class Metadata : ChargifyBase, IMetadata, IEquatable<Metadata>
    {
        #region Constructors

        /// <summary>
        /// Meaningful values to subscription or customer records
        /// </summary>
        public Metadata()
        {
        }

        /// <summary>
        /// Meaningful values to subscription or customer records
        /// </summary>
        /// <param name="metadataXml">The XML to parse into a metadata result</param>
        public Metadata(string metadataXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(metadataXml);
            if (doc.ChildNodes.Count == 0)
                throw new ArgumentException("XML not valid", nameof(metadataXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "metadata")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no metadata result data was found
            throw new ArgumentException("XML does not contain metadata result information", nameof(metadataXml));
        }

        /// <summary>
        /// Meaningful values to subscription or customer records
        /// </summary>
        /// <param name="metadataNode">The XML document node to use to parse into a metadata result</param>
        public Metadata(XmlNode metadataNode)
        {
            if (metadataNode == null)
                throw new ArgumentNullException(nameof(metadataNode));
            if (metadataNode.Name != "metadatum")
                throw new ArgumentException("Not a vaild metadatum results node", nameof(metadataNode));
            if (metadataNode.ChildNodes.Count == 0)
                throw new ArgumentException("XML not valid", nameof(metadataNode));
            LoadFromNode(metadataNode);
        }

        private void LoadFromNode(XmlNode metadataNode)
        {
            foreach (XmlNode childNode in metadataNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "resource-id":
                        ResourceID = childNode.GetNodeContentAsInt();
                        break;
                    case "name":
                        Name = childNode.GetNodeContentAsString();
                        break;
                    case "value":
                        Value = childNode.GetNodeContentAsString();
                        break;
                }
            }
        }

        #endregion

        #region IMetadata Members

        /// <summary>
        /// The resource id that the metadata belongs to
        /// </summary>
        public int ResourceID { get { return _resourceId; } set { _resourceId = value; } }
        private int _resourceId = int.MinValue;

        /// <summary>
        /// The value of the attribute that was added to the resource
        /// </summary>
        public string Value { get { return _value; } set { _value = value; } }
        private string _value;

        /// <summary>
        /// The name of the attribute that is added to the resource
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }
        private string _name = string.Empty;

        #endregion

        #region Equality
        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 17;
                result = result * 23 + ResourceID.GetHashCode();
                result = result * 23 + ((Value != null) ? Value.GetHashCode() : 0);
                result = result * 23 + ((Name != null) ? Name.GetHashCode() : 0);
                return result;
            }
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Equals(Metadata value)
        {
            if (ReferenceEquals(null, value))
            {
                return false;
            }
            if (ReferenceEquals(this, value))
            {
                return true;
            }
            return ResourceID == value.ResourceID &&
                   Equals(Value, value.Value) &&
                   Equals(Name, value.Name);
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Metadata temp = obj as Metadata;
            if (temp == null)
                return false;
            return Equals(temp);
        }
        #endregion
    }
}