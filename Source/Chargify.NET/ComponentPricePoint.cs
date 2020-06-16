using ChargifyNET.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChargifyNET
{
    /// <summary>
    /// Component Price Point for components with multiple price points
    /// </summary>
    public class ComponentPricePoint : ChargifyBase, IComponentPricePoint
    {
        public ComponentPricePoint()
        {
        }

        public ComponentPricePoint(string pricePointXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pricePointXml);
            if (doc.ChildNodes.Count == 0)
            {
                throw new ArgumentException("XML not valid", nameof(pricePointXml));
            }

            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "components")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }

            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain charge information", nameof(pricePointXml));
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
                    case "component_id":
                        ComponentId = obj.GetJSONContentAsInt(key);
                        break;
                    case "price_point":
                        PricePoint = obj.GetJSONContentAsString(key);
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
                    case "component_id":
                        ComponentId = dataNode.GetNodeContentAsInt();
                        break;
                    case "price_point":
                        PricePoint = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }

        public int ComponentId { get; set; }

        public string PricePoint { get; set; }
    }
}
