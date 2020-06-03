using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChargifyNET;
using Newtonsoft.Json;

namespace ChargifyDotNet.RequestDTOs
{
    internal class MetadataRequest
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        #region XmlBuilders

        internal static string GetMetadatumXml(long chargifyId, IList<Metadata> metadatum)
        {
            var metadataXml = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            metadataXml.Append("<metadata type=\"array\">");
            foreach (var metadata in metadatum)
            {
                metadataXml.Append("<metadatum>");
                if (metadata.ResourceID > 0)
                {
                    metadataXml.AppendFormat("<resource-id>{0}</resource-id>", metadata.ResourceID);
                }
                else
                {
                    metadataXml.AppendFormat("<resource-id>{0}</resource-id>", chargifyId);
                }
                metadataXml.AppendFormat("<name>{0}</name>", metadata.Name);
                metadataXml.AppendFormat("<value>{0}</value>", metadata.Value);
                metadataXml.Append("</metadatum>");
            }
            metadataXml.Append("</metadata>");
            return metadataXml.ToString();
        }

        #endregion

        internal static IList<MetadataRequest> GetMetadatumRequest(IList<Metadata> metadatum)
        {
            return metadatum.Select(elem => new MetadataRequest
            {
                Name = elem.Name,
                Value = elem.Value
            }).ToList();
        }
    }
}
