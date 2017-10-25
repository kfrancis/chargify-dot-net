using System;
using System.Text;
using System.Xml;
using System.Collections;

namespace ChargifyNET.Json
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
            bool hasChildrenArray = false;
            string childName = string.Empty;
            foreach (object child in node)
            {
                var childElement = child as XmlElement;
                if (childElement != null)
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
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("{");
            XmlToJsoNnode(sbJson, xmlDoc.DocumentElement, true);
            sbJson.Append("}");
            return sbJson.ToString();
        }

        //  XmlToJSONnode:  Output an XmlElement, possibly as part of a higher array
        private static void XmlToJsoNnode(StringBuilder sbJson, XmlElement node, bool showNodeName)
        {
            if (showNodeName)
                sbJson.Append("\"" + SafeJson(node.Name) + "\": ");
            sbJson.Append("{");
            // Build a sorted list of key-value pairs
            //  where   key is case-sensitive nodeName
            //          value is an ArrayList of string or XmlElement
            //  so that we know whether the nodeName is an array or not.
            SortedList childNodeNames = new SortedList();

            //  Add in all node attributes
            foreach (XmlAttribute attr in node.Attributes)
                StoreChildNode(childNodeNames, attr.Name, attr.InnerText);

            //  Add in all nodes
            foreach (XmlNode cnode in node.ChildNodes)
            {
                if (cnode is XmlText)
                    StoreChildNode(childNodeNames, "value", cnode.InnerText);
                else if (cnode is XmlElement)
                    StoreChildNode(childNodeNames, cnode.Name, cnode);
            }

            // Now output all stored info
            foreach (string childname in childNodeNames.Keys)
            {
                ArrayList alChild = (ArrayList)childNodeNames[childname];
                if (alChild.Count == 1 && (alChild[0] is string))
                    OutputNode(childname, alChild[0], sbJson, true);
                else
                {
                    var alChildElement = alChild[0] as XmlElement;
                    //var alParentElementHasChildren = alChildElement != null ? (alChildElement.ParentNode as XmlElement) != null ? (alChildElement.ParentNode as XmlElement).HasChildren() : false : false;
                    bool hasChildrenArray = alChildElement.HasChildren();
                    sbJson.Append(" \"" + SafeJson(childname) + string.Format("\": {0} ", hasChildrenArray ? "[" : string.Empty));   
                    foreach (object child in alChild)
                        OutputNode(childname, child, sbJson, false);
                    sbJson.Remove(sbJson.Length - 2, 2);
                    sbJson.AppendFormat(" {0},", hasChildrenArray ? "]" : string.Empty);
                }
            }
            sbJson.Remove(sbJson.Length - 2, 2);
            sbJson.Append(" }");
        }

        //  StoreChildNode: Store data associated with each nodeName
        //                  so that we know whether the nodeName is an array or not.
        private static void StoreChildNode(SortedList childNodeNames, string nodeName, object nodeValue)
        {
            // Pre-process contraction of XmlElement-s
            var nodeElement = nodeValue as XmlElement;
            if (nodeElement != null)
            {
                // Convert  <aa></aa> into "aa":null
                //          <aa>xx</aa> into "aa":"xx"
                XmlNode cnode = (XmlNode)nodeValue;
                if (cnode.Attributes != null && cnode.Attributes.Count == 0)
                {
                    XmlNodeList children = cnode.ChildNodes;
                    if (children.Count == 0)
                        nodeValue = null;
                    else if (children.Count == 1 && (children[0] is XmlText))
                        nodeValue = ((XmlText)(children[0])).InnerText;
                }
            }
            // Add nodeValue to ArrayList associated with each nodeName
            // If nodeName doesn't exist then add it
            object oValuesAl = childNodeNames[nodeName];
            ArrayList valuesAl;
            if (oValuesAl == null)
            {
                valuesAl = new ArrayList();
                childNodeNames[nodeName] = valuesAl;
            }
            else
                valuesAl = (ArrayList)oValuesAl;
            if (nodeValue != null) valuesAl.Add(nodeValue);
        }

        private static void OutputNode(string childname, object alChild, StringBuilder sbJson, bool showNodeName)
        {
            if (alChild == null)
            {
                if (showNodeName)
                    sbJson.Append("\"" + SafeJson(childname) + "\": ");
                sbJson.Append("null");
            }
            else if (alChild is string)
            {
                if (showNodeName)
                    sbJson.Append("\"" + SafeJson(childname) + "\": ");
                string sChild = (string)alChild;
                sChild = sChild.Trim();
                sbJson.Append("\"" + SafeJson(sChild) + "\"");
            }
            else
                XmlToJsoNnode(sbJson, (XmlElement)alChild, showNodeName);
            sbJson.Append(", ");
        }

        // Make a string safe for JSON
        private static string SafeJson(string sIn)
        {
            StringBuilder sbOut = new StringBuilder(sIn.Length);
            foreach (char ch in sIn)
            {
                if (Char.IsControl(ch) || ch == '\'')
                {
                    int ich = ch;
                    sbOut.Append(@"\u" + ich.ToString("x4"));
                    continue;
                }
                else if (ch == '\"' || ch == '\\' || ch == '/')
                {
                    sbOut.Append('\\');
                }
                sbOut.Append(ch);
            }
            return sbOut.ToString();
        }
    }
}
