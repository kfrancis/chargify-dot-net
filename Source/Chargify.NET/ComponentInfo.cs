
#region License, Terms and Conditions
//
// ComponentInfo.cs
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
    using ChargifyNET.Json;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Specfic class when getting information about a component as set to a specific product family
    /// </summary>
    public class ComponentInfo : ChargifyBase, IComponentInfo, IComparable<ComponentInfo>
    {
        #region Field Keys
        private const string CreatedAtKey = "created_at";
        private const string IDKey = "id";
        private const string ComponentIDKey = "component_id";
        private const string NameKey = "name";
		private const string DescriptionKey = "description";
        private const string PricePerUnitInCentsKey = "price_per_unit_in_cents";
        private const string PricingSchemeKey = "pricing_scheme";
        private const string ProductFamilyIDKey = "product_family_id";
        private const string UnitNameKey = "unit_name";
        private const string UpdatedAtKey = "updated_at";
        private const string KindKey = "kind";
        private const string UnitPriceKey = "unit_price";
        private const string PricesKey = "prices";
        private const string ArchivedKey = "archived";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private ComponentInfo() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentInfoXML">An XML string containing a component node</param>
        public ComponentInfo(string componentInfoXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(componentInfoXML);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "componentInfoXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "component")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no component info was found
            throw new ArgumentException("XML does not contain component information", "componentInfoXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentInfoNode">An xml node with component information</param>
        internal ComponentInfo(XmlNode componentInfoNode)
            : base()
        {
            if (componentInfoNode == null) throw new ArgumentNullException("componentInfoNode");
            if (componentInfoNode.Name != "component") throw new ArgumentException("Not a vaild component node", "componentInfoNode");
            if (componentInfoNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "componentInfoNode");
            this.LoadFromNode(componentInfoNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentInfoObject">An JsonObject with component information</param>
        public ComponentInfo(JsonObject componentInfoObject)
            : base()
        {
            if (componentInfoObject == null) throw new ArgumentNullException("componentInfoObject");
            if (componentInfoObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild component object", "componentInfoObject");
            this.LoadFromJSON(componentInfoObject);
        }

        private void LoadFromJSON(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get component info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case CreatedAtKey:
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case IDKey:
                    case ComponentIDKey:
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case NameKey:
                        _name = obj.GetJSONContentAsString(key);
                        break;
					case DescriptionKey:
		                _description = obj.GetJSONContentAsString(key);
		                break;
                    case PricePerUnitInCentsKey:
                        _pricePerUnit = obj.GetJSONContentAsInt(key);
                        break;
                    case PricingSchemeKey:
                        _pricingScheme = obj.GetJSONContentAsPricingSchemeType(key);
                        break;
                    case ProductFamilyIDKey:
                        _productFamilyID = obj.GetJSONContentAsInt(key);
                        break;
                    case UnitNameKey:
                        _unitName = obj.GetJSONContentAsString(key);
                        break;
                    case UpdatedAtKey:
                        _updatedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case KindKey:
                        _kind = obj.GetJSONContentAsComponentType(key);
                        break;
                    case UnitPriceKey:
                        _unitPrice = obj.GetJSONContentAsDecimal(key);
                        break;
                    case PricesKey:
                         _prices = new List<IPriceBracketInfo>();
                        JsonArray pricesArray = obj[key] as JsonArray;
                        if (pricesArray != null)
                        {
                            foreach (JsonObject priceObj in pricesArray.Items)
                            {
                                if (priceObj == null) continue;
                                var bracketInfo = new PriceBracketInfo();

                                foreach (var bracketKey in priceObj.Keys)
                                {
                                    switch (bracketKey)
                                    {
                                        case "starting_quantity":
                                            bracketInfo.StartingQuantity = priceObj.GetJSONContentAsNullableInt(bracketKey) ?? int.MinValue;
                                            break;
                                        case "ending_quantity":
                                            bracketInfo.EndingQuantity = priceObj.GetJSONContentAsNullableInt(bracketKey) ?? int.MaxValue;
                                            break;
                                        case "unit_price":
                                            bracketInfo.UnitPrice = priceObj.GetJSONContentAsDecimal(bracketKey);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                _prices.Add(bracketInfo);
                            }
                        }
                        // Sanity check, should be equal.
                        if (pricesArray != null && pricesArray.Length != _prices.Count)
                        {
                            throw new JsonParseException(string.Format("Unable to parse price brackets ({0} != {1})", pricesArray.Length, _prices.Count));
                        }
                        break;  
                    case ArchivedKey:
                        _archived = obj.GetJSONContentAsBoolean(key);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadFromNode(XmlNode obj)
        {
            // loop through the nodes to get component info
            foreach (XmlNode dataNode in obj.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case CreatedAtKey:
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case IDKey:
                    case ComponentIDKey:
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case NameKey:
                        _name = dataNode.GetNodeContentAsString();
                        break;
					case DescriptionKey:
						_description = dataNode.GetNodeContentAsString();
						break;
                    case PricePerUnitInCentsKey:
                        _pricePerUnit = dataNode.GetNodeContentAsInt();
                        break;
                    case PricingSchemeKey:
                        _pricingScheme = dataNode.GetNodeContentAsPricingSchemeType();
                        break;
                    case ProductFamilyIDKey:
                        _productFamilyID = dataNode.GetNodeContentAsInt();
                        break;
                    case UnitNameKey:
                        _unitName = dataNode.GetNodeContentAsString();
                        break;
                    case UpdatedAtKey:
                        _updatedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case KindKey:
                        _kind = dataNode.GetNodeContentAsComponentType();
                        break;
                    case UnitPriceKey:
                        _unitPrice = dataNode.GetNodeContentAsDecimal();
                        break;
                    case PricesKey:
                        _prices = new List<IPriceBracketInfo>();
                        foreach (XmlNode priceNode in dataNode.ChildNodes)
                        {
                            var bracketInfo = new PriceBracketInfo();
                            foreach (XmlNode bracketNode in priceNode.ChildNodes)
                            {
                                switch (bracketNode.Name)
                                {
                                    case "starting_quantity":
                                        bracketInfo.StartingQuantity = bracketNode.GetNodeContentAsNullableInt() ?? int.MinValue;
                                        break;
                                    case "ending_quantity":
                                        bracketInfo.EndingQuantity = bracketNode.GetNodeContentAsNullableInt() ?? int.MaxValue;
                                        break;
                                    case "unit_price":
                                        bracketInfo.UnitPrice = bracketNode.GetNodeContentAsDecimal();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            _prices.Add(bracketInfo);
                        }
                        break; 
                    case ArchivedKey:
                        _archived = dataNode.GetNodeContentAsBoolean();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region IComponentInfo Members

        /// <summary>
        /// Date and time that this component was created
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// The ID of this component
        /// </summary>
        public int ID
        {
            get { return _id; }
        }
        private int _id = int.MinValue;

        /// <summary>
        /// The name of the component as created by the Chargify user
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        private string _name = string.Empty;

		/// <summary>
		/// The description of the component as created by the Chargify user
		/// </summary>
		public string Description
		{
			get { return _description; }
		}
		private string _description = string.Empty;

        /// <summary>
        /// Price of the component per unit (in cents)
        /// </summary>
        [Obsolete("This value is depreciated since 1.5, please use UnitPrice instead.")]
        public int PricePerUnitInCents
        {
            get { return _pricePerUnit; }
        }
        private int _pricePerUnit = int.MinValue;

        /// <summary>
        /// Price of the component per unit (in dollars and cents)
        /// </summary>
        [Obsolete("This value is depreciated since 1.5, please use UnitPrice instead.")]
        public decimal PricePerUnit
        {
            get { return Convert.ToDecimal(this._pricePerUnit) / 100; }
        }

        /// <summary>
        /// The type of pricing scheme for this component
        /// </summary>
        public PricingSchemeType PricingScheme
        {
            get { return _pricingScheme; }
        }
        private PricingSchemeType _pricingScheme = PricingSchemeType.Unknown;

        /// <summary>
        /// The ID of the product family this component was created for
        /// </summary>
        public int ProductFamilyID
        {
            get { return _productFamilyID; }
        }
        private int _productFamilyID = int.MinValue;

        /// <summary>
        /// The name for the unit this component is measured in.
        /// </summary>
        public string UnitName
        {
            get { return _unitName; }
        }
        private string _unitName = string.Empty;

        /// <summary>
        /// Date/Time that this component was last updated.
        /// </summary>
        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
        }
        private DateTime _updatedAt = DateTime.MinValue;

        /// <summary>
        /// The kind of component, either quantity-based or metered component
        /// </summary>
        public ComponentType Kind
        {
            get { return _kind; }
        }
        private ComponentType _kind = ComponentType.Unknown;

        /// <summary>
        /// The amount the customer will be charged per unit. This field is only populated for 'per_unit' pricing schemes.
        /// </summary>
        public decimal UnitPrice 
        { 
            get { return _unitPrice; } 
        }
        private decimal _unitPrice = decimal.MinValue;

        /// <summary>
        /// An list of price brackets. If the component uses the 'per_unit' pricing scheme, an empty list will be returned.
        /// </summary>
        public List<IPriceBracketInfo> Prices 
        {
            get { return _prices; } 
        }
        private List<IPriceBracketInfo> _prices = new List<IPriceBracketInfo>();

        /// <summary>
        /// Boolean flag describing whether a component is archived or not
        /// </summary>
        public bool Archived { get { return _archived; } }
        private bool _archived = false;

        #endregion

        #region IComparable<IComponentInfo> Members

        /// <summary>
        /// Compare method for ComponentInfo
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>The CompareTo value based on comparing IDs</returns>
        public int CompareTo(IComponentInfo other)
        {
            return this.ID.CompareTo(other.ID);
        }

        #endregion

        #region IComparable<ComponentInfo> Members

        /// <summary>
        /// Compare method for ComponentInfo
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>The CompareTo value based on comparing IDs</returns>
        public int CompareTo(ComponentInfo other)
        {
            return this.ID.CompareTo(other.ID);
        }

        #endregion
    }

    /// <summary>
    /// The description for a single bracket of a components price
    /// </summary>
    public class PriceBracketInfo : ChargifyBase, IPriceBracketInfo
    {
        /// <summary>
        /// The starting quantity for the component
        /// </summary>
        public int StartingQuantity { get; set; }
        /// <summary>
        /// The ending quantity for the component
        /// </summary>
        public int EndingQuantity { get; set; }
        /// <summary>
        /// The unit price for the component
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
