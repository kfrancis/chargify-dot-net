using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoiceAddress
    {
        [JsonProperty("street")]
		public string Street { get; }
        [JsonProperty("line2")]
		public string Line2 { get; }
        [JsonProperty("city")]
		public string City { get; }
        [JsonProperty("state")]
		public string State { get; }
        [JsonProperty("zip")]
		public string Zip { get; }
        [JsonProperty("country")]
		public string Country { get; }
    }
}
