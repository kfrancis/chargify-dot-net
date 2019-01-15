using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoicePaymentMethod
    {
        [JsonProperty("details")]
		public string Details { get; }
        [JsonProperty("kind")]
		public string Kind { get; }
        [JsonProperty("memo")]
		public string Memo { get; }
        [JsonProperty("type")]
		public string Type { get; }
        [JsonProperty("card_brand")]
		public string CardBrand { get; }
        [JsonProperty("card_expiration")]
		public string CardExpiration { get; }
        [JsonProperty("last_four")]
		public string LastFour { get; }
        [JsonProperty("masked_card_number")]
		public string MaskedCardNumber { get; }
    }
}
