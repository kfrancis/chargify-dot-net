
#region License, Terms and Conditions
//
// ComponentAttributes.cs
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
    #endregion

    /// <summary>
    /// Specfic class when getting information about a component as set to a specific subscription 
    /// as specified here: http://support.chargify.com/faqs/technical/quantity-based-components
    /// </summary>
    public class ComponentAttributes : ChargifyBase, IComponentAttributes, IComparable<ComponentAttributes>
    {
        #region Field Keys
        private const string AllocatedQuantityKey = "allocated_quantity";
        private const string ComponentIDKey = "component_id";
        private const string SubscriptionIDKey = "subscription_id";
        private const string NameKey = "name";
        private const string UnitNameKey = "unit_name";
        private const string KindKey = "kind";
        private const string PricingSchemeKey = "pricing_scheme";
        private const string UnitBalanceKey = "unit_balance";
        private const string EnabledKey = "enabled";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private ComponentAttributes() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentAttributesXML">An XML string containing a component node</param>
        public ComponentAttributes(string componentAttributesXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(componentAttributesXML);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "componentAttributesXML");
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
            throw new ArgumentException("XML does not contain component information", "componentAttributesXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentAttributeNode">An xml node with usage information</param>
        internal ComponentAttributes(XmlNode componentAttributeNode)
            : base()
        {
            if (componentAttributeNode == null) throw new ArgumentNullException("componentAttributeNode");
            if (componentAttributeNode.Name != "component") throw new ArgumentException("Not a vaild component node", "componentAttributeNode");
            if (componentAttributeNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "componentAttributeNode");
            this.LoadFromNode(componentAttributeNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentAttributesObject">An JsonObject with component information</param>
        public ComponentAttributes(JsonObject componentAttributesObject)
            : base()
        {
            if (componentAttributesObject == null) throw new ArgumentNullException("componentAttributesObject");
            if (componentAttributesObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild component object", "componentAttributesObject");
            this.LoadFromJSON(componentAttributesObject);
        }

        private void LoadFromJSON(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get component info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case AllocatedQuantityKey:
                        _allocatedQuantity = obj.GetJSONContentAsInt(key);
                        break;
                    case ComponentIDKey:
                        _componentID = obj.GetJSONContentAsInt(key);
                        break;
                    case SubscriptionIDKey:
                        _subscriptionID = obj.GetJSONContentAsInt(key);
                        break;
                    case NameKey:
                        _name = obj.GetJSONContentAsString(key);
                        break;
                    case UnitNameKey:
                        _unitName = obj.GetJSONContentAsString(key);
                        break;
                    case KindKey:
                        _kind = obj.GetJSONContentAsString(key);
                        break;
                    case PricingSchemeKey:
                        _pricingScheme = obj.GetJSONContentAsString(key);
                        break;
                    case UnitBalanceKey:
                        // This is only passed back from Chargify when you UPDATE the allocated amount
                        _unitBalance = obj.GetJSONContentAsInt(key);
                        break;
                    case EnabledKey:
                        _enabled = obj.GetJSONContentAsBoolean(key);
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
                    case AllocatedQuantityKey:
                        _allocatedQuantity = dataNode.GetNodeContentAsInt();
                        break;
                    case ComponentIDKey:
                        _componentID = dataNode.GetNodeContentAsInt();
                        break;
                    case SubscriptionIDKey:
                        _subscriptionID = dataNode.GetNodeContentAsInt();
                        break;
                    case NameKey:
                        _name = dataNode.GetNodeContentAsString();
                        break;
                    case UnitNameKey:
                        _unitName = dataNode.GetNodeContentAsString();
                        break;
                    case KindKey:
                        _kind = dataNode.GetNodeContentAsString();
                        break;
                    case PricingSchemeKey:
                        _pricingScheme = dataNode.GetNodeContentAsString();
                        break;
                    case UnitBalanceKey:
                        // This is only passed back from Chargify when you UPDATE the allocated amount
                        _unitBalance = dataNode.GetNodeContentAsInt();
                        break;
                    case EnabledKey:
                        _enabled = dataNode.GetNodeContentAsBoolean();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region IComponentAttributes Members

        /// <summary>
        /// The name of the component as created by the Chargify user
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        private string _name = string.Empty;

        /// <summary>
        /// The kind of component, either quantity-based or metered component
        /// </summary>
        public string Kind
        {
            get { return _kind; }
        }
        private string _kind = string.Empty;

        /// <summary>
        /// The ID of the subscription that this component applies to
        /// </summary>
        public int SubscriptionID
        {
            get { return _subscriptionID; }
        }
        private int _subscriptionID = int.MinValue;

        /// <summary>
        /// The ID of the component itself
        /// </summary>
        public int ComponentID
        {
            get { return _componentID; }
        }
        private int _componentID = int.MinValue;

        /// <summary>
        /// The quantity allocated to this subscription
        /// </summary>
        public int AllocatedQuantity
        {
            // Clamp the response to 0+, since that's what makes sense.
            get { return (_allocatedQuantity < 0 ? 0 : _allocatedQuantity); }
        }
        private int _allocatedQuantity = int.MinValue;

        /// <summary>
        /// The method used to charge, either: per-unit, volume, tiered or stairstep
        /// </summary>
        public string PricingScheme
        {
            get { return _pricingScheme; }
        }
        private string _pricingScheme = string.Empty;

        /// <summary>
        /// The name for the unit this component is measured in.
        /// </summary>
        public string UnitName
        {
            get { return _unitName; }
        }
        private string _unitName = string.Empty;

        /// <summary>
        /// The balance of units of this component against the subscription
        /// </summary>
        public int UnitBalance 
        {
            // Clamp the response to 0+, since that's what makes sense.
            get { return (_unitBalance < 0 ? 0 : _unitBalance); } 
        }
        private int _unitBalance = int.MinValue;

        /// <summary>
        /// The status of whether this component is enabled or disabled.
        /// (On/Off components only)
        /// </summary>
        public bool Enabled 
        {
            get { return _enabled; }
        }
        private bool _enabled = false;

        #endregion

        #region IComparable<ComponentAttributes> Members

        /// <summary>
        /// Compare this ComponentAttributes to another
        /// </summary>
        public int CompareTo(ComponentAttributes other)
        {
            return this.ComponentID.CompareTo(other.ComponentID);
        }

        #endregion

        #region IComparable<IComponentAttributes> Members

        /// <summary>
        /// Compare this IComponentAttributes to another
        /// </summary>
        public int CompareTo(IComponentAttributes other)
        {
            return this.ComponentID.CompareTo(other.ComponentID);
        }

        #endregion
    }
}
