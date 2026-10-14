using System;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace ChargifyDotNet.Json
{
    /// <summary>
    /// Class used to convert an XmlDocument into JSON to send back to Chargify
    /// </summary>
    public static class XmlToJsonConverter
    {
        /// <summary>
        /// Does the node have children?
        /// </summary>
        /// <param name="node">The xml element node</param>
        /// <returns>True if the element has children, false otherwise.</returns>
        public static bool HasChildren(this XmlElement node)
        {
            var hasChildrenArray = false;
            var childName = string.Empty;
            foreach (var child in node)
            {
                if (child is XmlElement childElement)
                {
                    if (childName == string.Empty)
                    {
                        childName = childElement.Name;
                    }
                    else
                    {
                        hasChildrenArray = childName == childElement.Name;
                    }
                }
            }
            return hasChildrenArray;
        }

        /// <summary>
        /// Method converts XmlDocument to JSON
        /// </summary>
        /// <param name="xmlDoc">The document to convert</param>
        /// <returns>The JSON equivalent string</returns>
        public static string XmlToJson(XmlDocument xmlDoc)
        {
            //StringBuilder sbJson = new StringBuilder();
            //sbJson.Append("{");
            //XmlToJsoNnode(sbJson, xmlDoc.DocumentElement, true);
            //sbJson.Append("}");
            var converterSettings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
                {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(xmlDoc, Newtonsoft.Json.Formatting.Indented, converterSettings);
        }
    }
}
