// -----------------------------------------------------------------------
// <copyright file="SubscriptionGroup.cs" company="Loaded Reports">
// Copyright (c) Loaded Reports. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using ChargifyNET;
using ChargifyNET.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ChargifyNET
{
    public class SubscriptionGroup : ChargifyBase, ISubscriptionGroup, IComparable<SubscriptionGroup>
    {
        #region Field Keys
        private const string CustomerIdKey = "customer_id";
        private const string PaymentProfileKey = "payment_profile";
        private const string SubscriptionIdsKey = "subscription_ids";
        private const string SubscriptionIdKey = "subscription_id";
        private const string PaymentCollectionMethodKey = "payment_collection_method";
        private const string CreatedAtKey = "created_at";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public SubscriptionGroup()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionGroupXml">XML containing adjustment info (in expected format)</param>
        public SubscriptionGroup(string subscriptionGroupXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(subscriptionGroupXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(subscriptionGroupXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "subscription_group")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain subscription group information", nameof(subscriptionGroupXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionGroupNode">XML containing subscription info (in expected format)</param>
        internal SubscriptionGroup(XmlNode subscriptionGroupNode)
        {
            if (subscriptionGroupNode == null) throw new ArgumentNullException(nameof(subscriptionGroupNode));
            if (subscriptionGroupNode.Name != "subscription_group") throw new ArgumentException("Not a vaild adjustment node", nameof(subscriptionGroupNode));
            if (subscriptionGroupNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(subscriptionGroupNode));
            LoadFromNode(subscriptionGroupNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionGroupObject">Json containing subscription info (in expected format)</param>
        public SubscriptionGroup(JsonObject subscriptionGroupObject)
        {
            if (subscriptionGroupObject == null) throw new ArgumentNullException(nameof(subscriptionGroupObject));
            if (subscriptionGroupObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild charge object", nameof(subscriptionGroupObject));
            LoadFromJson(subscriptionGroupObject);
        }

        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case CustomerIdKey:
                        CustomerId = obj.GetJSONContentAsInt(key);
                        break;
                    case PaymentProfileKey:
                        PaymentProfile = obj.GetJSONContentAsPaymentProfileView(key);
                        break;
                    case PaymentCollectionMethodKey:
                        PaymentCollectionMethod = obj.GetJSONContentAsString(key);
                        break;
                    case CreatedAtKey:
                        CreatedAt = obj.GetJSONContentAsString(key);
                        break;
                    case SubscriptionIdsKey:
                        SubscriptionIds = LoadSubscriptionIdsFromObject(obj);
                        break;
                }
            }
        }

        private int[] LoadSubscriptionIdsFromObject(JsonObject obj)
        {
            List<int> result = new List<int>();
            if (obj != null)
            {
                if (obj.ContainsKey(SubscriptionIdsKey))
                {
                    JsonArray value = obj[SubscriptionIdsKey] as JsonArray;
                    if (value != null)
                    {
                        foreach (JsonValue arrayValue in value.Items)
                        {
                            result.Add(((JsonObject)arrayValue).GetJSONContentAsInt(SubscriptionIdKey));
                        }
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Load data from a adjustment node
        /// </summary>
        /// <param name="adjustmentNode">The adjustment node</param>
        private void LoadFromNode(XmlNode adjustmentNode)
        {
            foreach (XmlNode dataNode in adjustmentNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case CustomerIdKey:
                        CustomerId = dataNode.GetNodeContentAsInt();
                        break;
                    case PaymentProfileKey:
                        PaymentProfile = dataNode.GetNodeContentAsPaymentProfileView();
                        break;
                    case SubscriptionIdsKey:
                        SubscriptionIds = LoadSubscriptionIdsFromNode(dataNode);
                        break;
                    case CreatedAtKey:
                        CreatedAt = dataNode.GetNodeContentAsString();
                        break;
                    case PaymentCollectionMethodKey:
                        PaymentCollectionMethod = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }

        private int[] LoadSubscriptionIdsFromNode(XmlNode subscriptionIdsNode)
        {
            List<int> rst = new List<int>();
            foreach (XmlNode dataNode in subscriptionIdsNode.ChildNodes)
            {
                if (dataNode.Name == SubscriptionIdKey)
                {
                    rst.Add(dataNode.GetNodeContentAsInt());
                }
            }
            return rst.ToArray();
        }
        #endregion

        public int CustomerId { get; set; }
        public IPaymentProfileView PaymentProfile { get; set; }
        public int[] SubscriptionIds { get; set; }
        public string PaymentCollectionMethod { get; set; }
        public string CreatedAt { get; set; }

        #region IComparable<ISubscriptionGroup>
        /// <summary>
        /// Method for comparing two subscription groups
        /// </summary>
        /// <param name="other">The adjustment to compare with</param>
        /// <returns>The comparison value</returns>
        public int CompareTo(ISubscriptionGroup other)
        {
            return CustomerId.CompareTo(other.CustomerId);
        }
        #endregion

        #region IComparable<Adjustment>
        /// <summary>
        /// Method for comparing two subscription groups
        /// </summary>
        /// <param name="other">The adjustment to compare with</param>
        /// <returns>The comparison value</returns>
        public int CompareTo(SubscriptionGroup other)
        {
            return CustomerId.CompareTo(other.CustomerId);
        }
        #endregion
    }
}
