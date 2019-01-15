using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoiceSeller
    {
        [JsonProperty("name")]
		public string Name { get; }
        [JsonProperty("address")]
		public RelationshipInvoiceAddress Address { get; }
        [JsonProperty("phone")]
		public string Phone { get; }

    }
}
