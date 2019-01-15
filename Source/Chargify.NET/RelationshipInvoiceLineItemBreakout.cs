using Newtonsoft.Json;

namespace ChargifyNET
{
    public class RelationshipInvoiceLineItemBreakout
    {
        [JsonProperty("uid")]
		public string Uid { get; set; }
        [JsonProperty("eligable_amount")]
		public string EligableAmount { get; set; }
        [JsonProperty("discount_amount")]
		public string DiscountAmount { get; set; }
    }
}