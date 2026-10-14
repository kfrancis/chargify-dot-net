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
    using ChargifyDotNet;
    using ChargifyDotNet.Json;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    #endregion

    /// <summary>
    /// A price point for a component: the pricing scheme and price tiers used to bill it.
    /// </summary>
    public class ComponentPricePoint : ChargifyBase, IComparable<ComponentPricePoint>
    {
        private long _id;
        private bool? _default;
        private string _name = null!;
        private string _type = null!;
        private PricingSchemeType _pricingScheme;
        private long _componentId;
        private string _handle = null!;
        private DateTime? _archivedAt;
        private DateTime _createdAt;
        private DateTime _updatedAt;
        private List<ComponentPrice> _prices = null!;
        private bool? _renewPrepaidAllocation;
        private bool? _rolloverPrepaidRemainder;
        private int _expirationInterval;
        private IntervalUnit _expirationIntervalUnit;
        private PricingSchemeType _overagePricingScheme;
        private List<ComponentPrice> _overagePricingPrices = null!;

        /// <summary>
        /// Constructor. Pricing schemes default to Unknown and price lists are empty.
        /// </summary>
        public ComponentPricePoint()
        {
            _pricingScheme = PricingSchemeType.Unknown;
            _prices = [];
            _overagePricingScheme = PricingSchemeType.Unknown;
            _overagePricingPrices = [];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentPricePointXml">XML containing a price_point element</param>
        public ComponentPricePoint(string componentPricePointXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new();
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
            foreach (var key in obj.Keys)
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
                        if (obj[key] is not JsonObject overrage) break;
                        _overagePricingScheme = overrage.GetJSONContentAsPricingSchemeType("pricing_scheme");
                        _overagePricingPrices = overrage.GetJSONContentAsPrices("prices");
                        var overrageArray = overrage["prices"] as JsonArray;
                        // Sanity check, should be equal.
                        if (overrageArray != null && overrageArray.Length != _overagePricingPrices.Count)
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
                        _overagePricingPrices = [];
                        foreach (XmlNode childNode in dataNode.ChildNodes)
                        {
                            switch (childNode.Name)
                            {
                                case "pricing_scheme":
                                    _overagePricingScheme = childNode.GetNodeContentAsPricingSchemeType();
                                    break;
                                case "price":
                                    var item = childNode.GetNodeContentAsComponentPrice();
                                    if (item is not null) _overagePricingPrices.Add(item);
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
        public int CompareTo(ComponentPricePoint? other)
        {
            if (other is null) return 1;
            return Id.CompareTo(other.Id);
        }

        #endregion

        /// <summary>
        /// The price point ID
        /// </summary>
        public long Id { get => _id; set => _id = value; }

        /// <summary>
        /// Whether this is the component's default price point
        /// </summary>
        public bool? Default { get => _default; set => _default = value; }

        /// <summary>
        /// Prepaid components: whether the allocation renews each period
        /// </summary>
        public bool? RenewPrepaidAllocation { get => _renewPrepaidAllocation; set => _renewPrepaidAllocation = value; }
        /// <summary>
        /// Prepaid components: whether unused quantity rolls over to the next period
        /// </summary>
        public bool? RolloverPrepaidRemainder { get => _rolloverPrepaidRemainder; set => _rolloverPrepaidRemainder = value; }

        /// <summary>
        /// The price point name
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        /// <summary>
        /// The price point type (e.g. "default", "catalog", "custom")
        /// </summary>
        public string Type { get => _type; set => _type = value; }

        /// <summary>
        /// The pricing scheme
        /// </summary>
        public PricingSchemeType PricingScheme { get => _pricingScheme; set => _pricingScheme = value; }

        /// <summary>
        /// The ID of the component this price point belongs to
        /// </summary>
        public long ComponentId { get => _componentId; set => _componentId = value; }

        /// <summary>
        /// The price point handle
        /// </summary>
        public string Handle { get => _handle; set => _handle = value; }
        /// <summary>
        /// Prepaid components: how many expiration interval units the rollover lasts
        /// </summary>
        public int ExpirationInterval { get => _expirationInterval; set => _expirationInterval = value; }
        /// <summary>
        /// Prepaid components: the unit of the expiration interval
        /// </summary>
        public IntervalUnit ExpirationIntervalUnit { get => _expirationIntervalUnit; set => _expirationIntervalUnit = value; }

        /// <summary>
        /// When the price point was archived, or null if it is active
        /// </summary>
        public DateTime? ArchivedAt { get => _archivedAt; set => _archivedAt = value; }

        /// <summary>
        /// True if the price point has been archived
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return _archivedAt != null && _archivedAt.HasValue;
            }
        }

        /// <summary>
        /// When the price point was created
        /// </summary>
        public DateTime CreatedAt { get => _createdAt; set => _createdAt = value; }

        /// <summary>
        /// When the price point was last updated
        /// </summary>
        public DateTime UpdatedAt { get => _updatedAt; set => _updatedAt = value; }

        /// <summary>
        /// The price tiers
        /// </summary>
        public IList<ComponentPrice> Prices { get => _prices; set => _prices = (List<ComponentPrice>)value; }
        /// <summary>
        /// Prepaid components: the pricing scheme for usage above the allocation
        /// </summary>
        public PricingSchemeType OveragePricingScheme { get => _overagePricingScheme; set => _overagePricingScheme = value; }
        /// <summary>
        /// Prepaid components: the price tiers for usage above the allocation
        /// </summary>
        public IList<ComponentPrice> OveragePricingPrices { get => _overagePricingPrices; set => _overagePricingPrices = (List<ComponentPrice>)value; }
    }

    /// <summary>
    /// A price tier within a component price point
    /// </summary>
    [JsonArray]
    public class ComponentPrice
    {
        private long _id;
        private long _componentId;
        private long _startingQuantity;
        private long _endingQuantity;
        private decimal _unitPrice;
        private long _pricePointId;
        private string _formattedUnitPrice = null!;
        private long _segmentId;

        /// <summary>
        /// Constructor
        /// </summary>
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
            foreach (var key in obj.Keys)
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

        /// <summary>
        /// The price tier ID
        /// </summary>
        public long Id { get => _id; set => _id = value; }

        /// <summary>
        /// The ID of the component this tier belongs to
        /// </summary>
        public long ComponentId { get => _componentId; set => _componentId = value; }

        /// <summary>
        /// The first quantity this tier applies to
        /// </summary>
        public long StartingQuantity { get => _startingQuantity; set => _startingQuantity = value; }

        /// <summary>
        /// The last quantity this tier applies to (0 or unset for an open-ended tier)
        /// </summary>
        public long EndingQuantity { get => _endingQuantity; set => _endingQuantity = value; }

        /// <summary>
        /// The price per unit in this tier
        /// </summary>
        public decimal UnitPrice { get => _unitPrice; set => _unitPrice = value; }

        /// <summary>
        /// The ID of the price point this tier belongs to
        /// </summary>
        public long PricePointId { get => _pricePointId; set => _pricePointId = value; }

        /// <summary>
        /// The unit price formatted by Chargify for display
        /// </summary>
        public string FormattedUnitPrice { get => _formattedUnitPrice; set => _formattedUnitPrice = value; }

        /// <summary>
        /// The ID of the event-based billing segment this tier belongs to, if any
        /// </summary>
        public long SegmentId { get => _segmentId; set => _segmentId = value; }
    }
}
