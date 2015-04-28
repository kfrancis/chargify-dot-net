
#region License, Terms and Conditions
//
// ComponentAllocation.cs
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
    using System.Xml;
    using ChargifyNET.Json;

    #endregion

    /// <summary>
    /// Specific class when getting or setting information specfic to a components allocation history
    /// </summary>
    /// <remarks>See http://docs.chargify.com/api-allocations </remarks>
    public class ComponentAllocation : ChargifyBase, IComponentAllocation, IComparable<ComponentAllocation>
    {
        #region Field Keys
        /// <summary>
        /// The XML or JSON key of which the child values correspond to the members of the ComponentAllocation class
        /// </summary>
        public static readonly string AllocationRootKey = "allocation";
        /// <summary>
        /// The XML key which represents a collection of ComponentAllocation's
        /// </summary>
        public static readonly string AllocationsRootKey = "allocations";
        private const string ComponentIDKey = "component_id";
        private const string SubscriptionIDKey = "subscription_id";
        private const string QuantityKey = "quantity";
        private const string PreviousQuantityKey = "previous_quantity";
        private const string MemoKey = "memo";
        private const string TimestampKey = "timestamp";
        private const string ProrationUpgradeSchemeKey = "proration_upgrade_scheme";
        private const string ProrationDowngradeSchemeKey = "proration_downgrade_scheme";
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ComponentAllocation()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentAllocationXML">The raw XML containing the component allocation node</param>
        public ComponentAllocation(string componentAllocationXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(componentAllocationXML);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "componentAllocationXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "allocation")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no component info was found
            throw new ArgumentException("XML does not contain component allocation information", "componentAllocationXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentAllocationObject">The JSON component allocation object</param>
        public ComponentAllocation(JsonObject componentAllocationObject)
            : base()
        {
            if (componentAllocationObject == null) throw new ArgumentNullException("componentAllocationObject");
            if (componentAllocationObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild component allocation object", "componentAllocationObject");
            this.LoadFromJSON(componentAllocationObject);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentAllocationNode">The XML component allocation node</param>
        internal ComponentAllocation(XmlNode componentAllocationNode)
            : base()
        {
            if (componentAllocationNode == null) throw new ArgumentNullException("componentAllocationNode");
            if (componentAllocationNode.Name != "allocation") throw new ArgumentException("Not a vaild component allocation node", "componentAllocationNode");
            if (componentAllocationNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "componentAllocationNode");
            this.LoadFromNode(componentAllocationNode);
        }

        private void LoadFromJSON(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get component allocation info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case ComponentIDKey:
                        this._componentID = obj.GetJSONContentAsInt(key);
                        break;
                    case SubscriptionIDKey:
                        this._subscriptionID = obj.GetJSONContentAsInt(key);
                        break;
                    case QuantityKey:
                        this.Quantity = obj.GetJSONContentAsInt(key);
                        break;
                    case PreviousQuantityKey:
                        this._previousQuantity = obj.GetJSONContentAsInt(key);
                        break;
                    case MemoKey:
                        this.Memo = obj.GetJSONContentAsString(key);
                        break;
                    case ProrationUpgradeSchemeKey:
                        this.UpgradeScheme = obj.GetJSONContentAsProrationUpgradeScheme(key);
                        break;
                    case ProrationDowngradeSchemeKey:
                        this.DowngradeScheme = obj.GetJSONContentAsProrationDowngradeScheme(key);
                        break;
                    case TimestampKey:
                        this._timeStamp = obj.GetJSONContentAsDateTime(key);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadFromNode(XmlNode obj)
        {
            // loop through the nodes to get component allocation info
            foreach (XmlNode dataNode in obj.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case ComponentIDKey:
                        this._componentID = dataNode.GetNodeContentAsInt();
                        break;
                    case SubscriptionIDKey:
                        this._subscriptionID = dataNode.GetNodeContentAsInt();
                        break;
                    case QuantityKey:
                        this.Quantity = dataNode.GetNodeContentAsInt();
                        break;
                    case PreviousQuantityKey:
                        this._previousQuantity = dataNode.GetNodeContentAsInt();
                        break;
                    case MemoKey:
                        this.Memo = dataNode.GetNodeContentAsString();
                        break;
                    case ProrationUpgradeSchemeKey:
                        this.UpgradeScheme = dataNode.GetNodeContentAsProrationUpgradeScheme();
                        break;
                    case ProrationDowngradeSchemeKey:
                        this.DowngradeScheme = dataNode.GetNodeContentAsProrationDowngradeScheme();
                        break;
                    case TimestampKey:
                        this._timeStamp = dataNode.GetNodeContentAsDateTime();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region IComponentAllocation Members
        /// <summary>
        /// The allocated quantity set in to effect by the allocation
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The allocated quantity that was in effect before this allocation was created
        /// </summary>
        public int PreviousQuantity { get { return this._previousQuantity; } }
        private int _previousQuantity = int.MinValue;

        /// <summary>
        /// The integer component ID for the allocation. This references a component that you have created in your Product setup
        /// </summary>
        public int ComponentID { get { return this._componentID; } }
        private int _componentID = int.MinValue;

        /// <summary>
        /// The integer subscription ID for the allocation. This references a unique subscription in your Site
        /// </summary>
        public int SubscriptionID { get { return this._subscriptionID; } }
        private int _subscriptionID = int.MinValue;

        /// <summary>
        /// The memo passed when the allocation was created
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// The time that the allocation was recorded, in ISO 8601 format and UTC timezone, i.e. 2012-11-20T22:00:37Z
        /// </summary>
        public DateTime TimeStamp { get { return this._timeStamp; } }
        private DateTime _timeStamp = DateTime.MinValue;

        /// <summary>
        /// The scheme used if the proration was an upgrade. This is only present when the allocation was created mid-period.
        /// </summary>
        public ComponentUpgradeProrationScheme UpgradeScheme { get; set; }

        /// <summary>
        /// The scheme used if the proration was a downgrade. This is only present when the allocation was created mid-period.
        /// </summary>
        public ComponentDowngradeProrationScheme DowngradeScheme { get; set; }

        #endregion

        #region Compare
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ComponentAllocation other)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
        #endregion
    }
}