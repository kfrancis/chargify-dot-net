#region License, Terms and Conditions

//
// ComponentPricePoint.cs
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

namespace ChargifyDotNet
{
    #region Imports
    using ChargifyNET;
    using ChargifyNET.Json;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    #endregion

    public class ComponentPricePoint : ChargifyBase, IComparable<ComponentPricePoint>
    {
        private long _id;
        private bool? _default;
        private string _name;
        private string _type;
        private PricingSchemeType _pricingScheme;
        private long _componentId;
        private string _handle;
        private DateTime? _archivedAt;
        private DateTime _createdAt;
        private DateTime _updatedAt;
        private List<ComponentPrice> _prices;
        private bool? _renewPrepaidAllocation;
        private bool? _rolloverPrepaidRemainder;
        private int _expirationInterval;
        private IntervalUnit _expirationIntervalUnit;
        private PricingSchemeType _overagePricingScheme;
        private List<ComponentPrice> _overagePricingPrices;

        public ComponentPricePoint()
        {
            _pricingScheme = PricingSchemeType.Unknown;
            _prices = new List<ComponentPrice>();
            _overagePricingScheme = PricingSchemeType.Unknown;
            _overagePricingPrices = new List<ComponentPrice>();
        }

        public ComponentPricePoint(string componentPricePointXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(componentPricePointXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(componentPricePointXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "price_point")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain price point information", nameof(componentPricePointXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentPricePointNode">XmlNode containing the data (in expected format)</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ComponentPricePoint(XmlNode componentPricePointNode)
        {
            if (componentPricePointNode == null) throw new ArgumentNullException(nameof(componentPricePointNode));
            if (componentPricePointNode.Name != "price_point") throw new ArgumentException("Not a vaild price point node", nameof(componentPricePointNode));
            if (componentPricePointNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(componentPricePointNode));
            LoadFromNode(componentPricePointNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentPricePointObject">JsonObject containing the data (in expected format)</param>
        public ComponentPricePoint(JsonObject componentPricePointObject)
        {
            if (componentPricePointObject == null) throw new ArgumentNullException(nameof(componentPricePointObject));
            if (componentPricePointObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild price point object", nameof(componentPricePointObject));
            LoadFromJson(componentPricePointObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing product data</param>
        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case "id":
                        _id = obj.GetJSONContentAsLong(key);
                        break;
                    case "default":
                        _default = obj.GetJSONContentAsBoolean(key);
                        break;
                    case "renew_prepaid_allocation":
                        _renewPrepaidAllocation = obj.GetJSONContentAsBoolean(key);
                        break;
                    case "rollover_prepaid_remainder":
                        _rolloverPrepaidRemainder = obj.GetJSONContentAsBoolean(key);
                        break;
                    case "expiration_interval":
                        _expirationInterval = obj.GetJSONContentAsInt(key);
                        break;
                    case "expiration_interval_unit":
                        _expirationIntervalUnit = obj.GetJSONContentAsIntervalUnit(key);
                        break;
                    case "name":
                        _name = obj.GetJSONContentAsString(key);
                        break;
                    case "type":
                        _type = obj.GetJSONContentAsString(key);
                        break;
                    case "pricing_scheme":
                        _pricingScheme = obj.GetJSONContentAsPricingSchemeType(key);
                        break;
                    case "overage_pricing":
                        var overrage = obj[key] as JsonObject;
                        _overagePricingScheme = overrage.GetJSONContentAsPricingSchemeType("pricing_scheme");
                        _overagePricingPrices = overrage.GetJSONContentAsPrices("prices");
                        var overrageArray = overrage["prices"] as JsonArray;
                        // Sanity check, should be equal.
                        if (overrage["prices"] != null && (overrage["prices"] as JsonArray).Length != _overagePricingPrices.Count)
                        {
                            throw new JsonParseException(string.Format("Unable to parse charges ({0} != {1})", overrageArray.Length, _overagePricingPrices.Count));
                        }
                        break;
                    case "component_id":
                        _componentId = obj.GetJSONContentAsLong(key);
                        break;
                    case "handle":
                        _handle = obj.GetJSONContentAsString(key);
                        break;
                    case "created_at":
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case "updated_at":
                        _updatedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case "archived_at":
                        _archivedAt = obj.GetJSONContentAsNullableDateTime(key);
                        break;
                    case "prices":
                        _prices = obj.GetJSONContentAsPrices(key);
                        break;

                }
            }
        }

        private void LoadFromNode(XmlNode productNode)
        {
            // loop through the nodes to get product info
            foreach (XmlNode dataNode in productNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case "id":
                        _id = dataNode.GetNodeContentAsLong();
                        break;
                    case "default":
                        _default = dataNode.GetNodeContentAsBoolean();
                        break;
                    case "renew_prepaid_allocation":
                        _renewPrepaidAllocation = dataNode.GetNodeContentAsBoolean();
                        break;
                    case "rollover_prepaid_remainder":
                        _rolloverPrepaidRemainder = dataNode.GetNodeContentAsBoolean();
                        break;
                    case "name":
                        _name = dataNode.GetNodeContentAsString();
                        break;
                    case "type":
                        _type = dataNode.GetNodeContentAsString();
                        break;
                    case "pricing_scheme":
                        _pricingScheme = dataNode.GetNodeContentAsPricingSchemeType();
                        break;
                    case "overage_pricing":
                        _overagePricingPrices = new List<ComponentPrice>();
                        foreach (XmlNode childNode in dataNode.ChildNodes)
                        {
                            switch (childNode.Name)
                            {
                                case "pricing_scheme":
                                    _overagePricingScheme = childNode.GetNodeContentAsPricingSchemeType();
                                    break;
                                case "price":
                                    _overagePricingPrices.Add(childNode.GetNodeContentAsComponentPrice());
                                    break;
                            }
                        }
                        break;
                    case "component_id":
                        _componentId = dataNode.GetNodeContentAsLong();
                        break;
                    case "handle":
                        _handle = dataNode.GetNodeContentAsString();
                        break;
                    case "created_at":
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case "updated_at":
                        _updatedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case "archived_at":
                        _archivedAt = dataNode.GetNodeContentAsNullableDateTime();
                        break;
                    case "prices":
                        _prices = dataNode.GetNodeContentAsComponentPrices();
                        break;
                }
            }
        }

        #region IComparable<ComponentAttributes> Members

        /// <summary>
        /// Compare this ComponentAttributes to another
        /// </summary>
        public int CompareTo(ComponentPricePoint other)
        {
            return Id.CompareTo(other.Id);
        }

        #endregion

        public long Id { get => _id; set => _id = value; }

        public bool? Default { get => _default; set => _default = value; }

        public bool? RenewPrepaidAllocation { get => _renewPrepaidAllocation; set => _renewPrepaidAllocation = value; }
        public bool? RolloverPrepaidRemainder { get => _rolloverPrepaidRemainder; set => _rolloverPrepaidRemainder = value; }

        public string Name { get => _name; set => _name = value; }

        public string Type { get => _type; set => _type = value; }

        public PricingSchemeType PricingScheme { get => _pricingScheme; set => _pricingScheme = value; }

        public long ComponentId { get => _componentId; set => _componentId = value; }

        public string Handle { get => _handle; set => _handle = value; }
        public int ExpirationInterval { get => _expirationInterval; set => _expirationInterval = value; }
        public IntervalUnit ExpirationIntervalUnit { get => _expirationIntervalUnit; set => _expirationIntervalUnit = value; }

        public DateTime? ArchivedAt { get => _archivedAt; set => _archivedAt = value; }

        public bool IsArchived
        {
            get
            {
                return _archivedAt != null && _archivedAt.HasValue;
            }
        }

        public DateTime CreatedAt { get => _createdAt; set => _createdAt = value; }

        public DateTime UpdatedAt { get => _updatedAt; set => _updatedAt = value; }

        public IList<ComponentPrice> Prices { get => _prices; set => _prices = (List<ComponentPrice>)value; }
        public PricingSchemeType OveragePricingScheme { get => _overagePricingScheme; set => _overagePricingScheme = value; }
        public IList<ComponentPrice> OveragePricingPrices { get => _overagePricingPrices; set => _overagePricingPrices = (List<ComponentPrice>)value; }
    }

    [JsonArray]
    public class ComponentPrice
    {
        private long _id;
        private long _componentId;
        private long _startingQuantity;
        private long _endingQuantity;
        private decimal _unitPrice;
        private long _pricePointId;
        private string _formattedUnitPrice;
        private long _segmentId;

        public ComponentPrice()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentPriceNode">XmlNode containing the data (in expected format)</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal ComponentPrice(XmlNode componentPriceNode)
        {
            if (componentPriceNode == null) throw new ArgumentNullException(nameof(componentPriceNode));
            if (componentPriceNode.Name != "price") throw new ArgumentException("Not a vaild component price node", nameof(componentPriceNode));
            if (componentPriceNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(componentPriceNode));
            LoadFromNode(componentPriceNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentPriceObject">JsonObject containing the data (in expected format)</param>
        internal ComponentPrice(JsonObject componentPriceObject)
        {
            if (componentPriceObject == null) throw new ArgumentNullException(nameof(componentPriceObject));
            if (componentPriceObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild component price object", nameof(componentPriceObject));
            LoadFromJson(componentPriceObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing product data</param>
        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case "id":
                        _id = obj.GetJSONContentAsLong(key);
                        break;
                    case "component_id":
                        _componentId = obj.GetJSONContentAsLong(key);
                        break;
                    case "starting_quantity":
                        _startingQuantity = obj.GetJSONContentAsLong(key);
                        break;
                    case "ending_quantity":
                        _endingQuantity = obj.GetJSONContentAsLong(key);
                        break;
                    case "unit_price":
                        _unitPrice = obj.GetJSONContentAsDecimal(key);
                        break;
                    case "price_point_id":
                        _pricePointId = obj.GetJSONContentAsLong(key);
                        break;
                    case "formatted_unit_price":
                        _formattedUnitPrice = obj.GetJSONContentAsString(key);
                        break;
                    case "segment_id":
                        _segmentId = obj.GetJSONContentAsLong(key);
                        break;

                }
            }
        }

        private void LoadFromNode(XmlNode productNode)
        {
            // loop through the nodes to get product info
            foreach (XmlNode dataNode in productNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case "id":
                        _id = dataNode.GetNodeContentAsLong();
                        break;
                    case "component_id":
                        _componentId = dataNode.GetNodeContentAsLong();
                        break;
                    case "starting_quantity":
                        _startingQuantity = dataNode.GetNodeContentAsLong();
                        break;
                    case "ending_quantity":
                        _endingQuantity = dataNode.GetNodeContentAsLong();
                        break;
                    case "unit_price":
                        _unitPrice = dataNode.GetNodeContentAsDecimal();
                        break;
                    case "price_point_id":
                        _pricePointId = dataNode.GetNodeContentAsLong();
                        break;
                    case "formatted_unit_price":
                        _formattedUnitPrice = dataNode.GetNodeContentAsString();
                        break;
                    case "segment_id":
                        _segmentId = dataNode.GetNodeContentAsLong();
                        break;
                }
            }
        }

        public long Id { get => _id; set => _id = value; }

        public long ComponentId { get => _componentId; set => _componentId = value; }

        public long StartingQuantity { get => _startingQuantity; set => _startingQuantity = value; }

        public long EndingQuantity { get => _endingQuantity; set => _endingQuantity = value; }

        public decimal UnitPrice { get => _unitPrice; set => _unitPrice = value; }

        public long PricePointId { get => _pricePointId; set => _pricePointId = value; }

        public string FormattedUnitPrice { get => _formattedUnitPrice; set => _formattedUnitPrice = value; }

        public long SegmentId { get => _segmentId; set => _segmentId = value; }
    }
}
