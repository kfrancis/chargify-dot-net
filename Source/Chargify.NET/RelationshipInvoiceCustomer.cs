using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoiceCustomer
    {
        [JsonProperty("chargify_id")]
		public int ChargifyId { get; }
        [JsonProperty("first_name")]
		public string FirstName { get; }
        [JsonProperty("last_name")]
		public string LastName { get; }
        [JsonProperty("organization")]
		public string Organization { get; }
        [JsonProperty("email")]
		public string Email { get; }
    }
}
