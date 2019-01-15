using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoiceTax
    {
        [JsonProperty("uid")]
		public string Uid { get; }
        [JsonProperty("title")]
		public string Title { get; }
        [JsonProperty("source_type")]
		public string SourceType { get; }
        [JsonProperty("source_id")]
		public int SourceId { get; }
        [JsonProperty("percentage")]
		public decimal Percentage { get; }
        [JsonProperty("taxable_amount")]
		public decimal TaxableAmount { get; }
        [JsonProperty("tax_amount")]
		public decimal TaxAmount { get; }
        [JsonProperty("line_item_breakouts")]
		public List<RelationshipInvoiceLineItemBreakout> LineItemBreakouts { get; }
    }
}
