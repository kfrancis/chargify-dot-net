using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoiceCustomField
    {
        [JsonProperty("name")]
		public string Name { get; }
        [JsonProperty("value")]
		public string Value { get; }
    }
}
