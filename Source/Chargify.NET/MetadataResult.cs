#region License, Terms and Conditions
//
// MetadataResult.cs
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
    using System.Collections.Generic;
    using System.Xml;
    using ChargifyNET.Json;
    #endregion

    /// <summary>
    /// An array of Metadata
    /// </summary>
    public class MetadataResult : ChargifyBase, IMetadataResult
    {
        #region Field Keys

        private const string TotalCountKey = "total-count";
        private const string CurrentPageKey = "current-page";
        private const string TotalPagesKey = "total-pages";
        private const string PerPageKey = "per-page";
        private const string MetadataKey = "metadata";

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public MetadataResult()
            : base()
        {
        }

        /// <summary>
        /// Constructor (xml)
        /// </summary>
        /// <param name="MetadataResultXML"></param>
        public MetadataResult(string MetadataResultXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(MetadataResultXML);
            if (doc.ChildNodes.Count == 0)
                throw new ArgumentException("XML not valid", "MetadataResultXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "results")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no metadata result data was found
            throw new ArgumentException("XML does not contain metadata result information", "MetadataResultXML");
        }

        /// <summary>
        /// Constructor (xml)
        /// </summary>
        /// <param name="MetadataResultNode"></param>
        public MetadataResult(XmlNode MetadataResultNode)
            : base()
        {
            if (MetadataResultNode == null)
                throw new ArgumentNullException("MetadataResultNode");
            if (MetadataResultNode.Name != "results")
                throw new ArgumentException("Not a vaild metadata results node", "MetadataResultNode");
            if (MetadataResultNode.ChildNodes.Count == 0)
                throw new ArgumentException("XML not valid", "MetadataResultNode");
            LoadFromNode(MetadataResultNode);
        }

        /// <summary>
        /// Constructor (json)
        /// </summary>
        /// <param name="MetadataResultObject"></param>
        public MetadataResult(JsonObject MetadataResultObject)
            : base()
        {
            if (MetadataResultObject == null)
                throw new ArgumentNullException("MetadataResultObject");
            if (MetadataResultObject.Keys.Count <= 0)
                throw new ArgumentException("Not a vaild metadata results object", "MetadataResultObject");
            LoadFromJSON(MetadataResultObject);
        }

        private void LoadFromNode(XmlNode parentNode)
        {
            foreach (XmlNode dataNode in parentNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case TotalCountKey:
                        _totalCount = dataNode.GetNodeContentAsInt();
                        break;
                    case CurrentPageKey:
                        _currentPage = dataNode.GetNodeContentAsInt();
                        break;
                    case TotalPagesKey:
                        _totalPages = dataNode.GetNodeContentAsInt();
                        break;
                    case PerPageKey:
                        _perPage = dataNode.GetNodeContentAsInt();
                        break;
                    case MetadataKey:
                        if (dataNode.FirstChild != null)
                        {
                            _metadata = new List<IMetadata>();
                            // There's no constructor that takes in an XmlNode, so parse it here.
                            foreach (XmlNode metadataNode in dataNode.ChildNodes)
                            {
                                var newObj = new Metadata();
                                var hasData = false;
                                foreach (XmlNode metadatumNode in metadataNode.ChildNodes)
                                {
                                    switch (metadatumNode.Name)
                                    {
                                        case "resource-id":
                                            hasData = true;
                                            newObj.ResourceID = metadatumNode.GetNodeContentAsInt();
                                            break;
                                        case "name":
                                            hasData = true;
                                            newObj.Name = metadatumNode.GetNodeContentAsString();
                                            break;
                                        case "value":
                                            hasData = true;
                                            newObj.Value = metadatumNode.GetNodeContentAsString();
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                if (hasData)
                                {
                                    _metadata.Add(newObj);
                                }
                            }

                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadFromJSON(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                string theKey = key;
                if (key.Contains("_"))
                    theKey = key.Replace("_", "-"); // Chargify seems to return different keys based on xml or json return type
                switch (theKey)
                {
                    case TotalCountKey:
                        _totalCount = obj.GetJSONContentAsInt(key);
                        break;
                    case CurrentPageKey:
                        _currentPage = obj.GetJSONContentAsInt(key);
                        break;
                    case TotalPagesKey:
                        _totalPages = obj.GetJSONContentAsInt(key);
                        break;
                    case PerPageKey:
                        _perPage = obj.GetJSONContentAsInt(key);
                        break;
                    case MetadataKey:
                        _metadata = new List<IMetadata>();
                        JsonArray viewObj = obj[key] as JsonArray;
                        if (viewObj != null && viewObj.Items != null && viewObj.Length > 0)
                        {
                            foreach (JsonValue item in viewObj.Items)
                            {
                                var newObj = new Metadata();
                                var hasData = false;
                                var itemObj = (JsonObject)item;
                                foreach (string subItemKey in itemObj.Keys)
                                {
                                    switch (subItemKey)
                                    {
                                        case "resource_id":
                                            hasData = true;
                                            newObj.ResourceID = obj.GetJSONContentAsInt(subItemKey);
                                            break;
                                        case "name":
                                            hasData = true;
                                            newObj.Name = obj.GetJSONContentAsString(subItemKey);
                                            break;
                                        case "value":
                                            hasData = true;
                                            newObj.Value = obj.GetJSONContentAsString(subItemKey);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                if (hasData)
                                {
                                    _metadata.Add(newObj);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region IMetadataResult Members

        /// <summary>
        /// The total number of metadatum
        /// </summary>
        public int TotalCount
        {
            get
            {
                return this._totalCount;
            }
        }

        private int _totalCount = int.MinValue;

        /// <summary>
        /// The current page being returned
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return this._currentPage;
            }
        }

        private int _currentPage = int.MinValue;

        /// <summary>
        /// The total number of pages (based on per-page and total count)
        /// </summary>
        public int TotalPages
        {
            get
            {
                return this._totalPages;
            }
        }

        private int _totalPages = int.MinValue;

        /// <summary>
        /// How many metadata are being returned per paged result
        /// </summary>
        public int PerPage
        {
            get
            {
                return this._perPage;
            }
        }

        private int _perPage = int.MinValue;

        /// <summary>
        /// The list of metadata contained in this response
        /// </summary>
        public List<IMetadata> Metadata
        {
            get
            {
                return this._metadata;
            }
        }

        private List<IMetadata> _metadata = new List<IMetadata>();

        #endregion
    }
}