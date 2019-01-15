using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoiceDiscount
    {
        [JsonProperty("uid")]
		public string Uid { get; set; }
        [JsonProperty("title")]
		public string Title { get; set; }
        [JsonProperty("code")]
		public string Code { get; set; }
        [JsonProperty("source_type")]
		public string SourceType { get; set; }
        [JsonProperty("sourceId")]
		public int SourceId { get; set; }
        [JsonProperty("discount_type")]
		public string DiscountType { get; set; }
        [JsonProperty("percentage")]
		public decimal Percentage { get; set; }
        [JsonProperty("eligible_amount")]
		public decimal EligibleAmount { get; set; }
        [JsonProperty("discount_amount")]
		public decimal DiscountAmount { get; set; }
        [JsonProperty("line_item_breakouts")]
		public List<RelationshipInvoiceLineItemBreakout> LineItemBreakouts { get; set; }
    }
}
