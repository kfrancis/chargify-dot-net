using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoiceRefund
    {
        [JsonProperty("transaction_id")]
		public int TransactionID { get; }
        [JsonProperty("payment_id")]
		public int PaymentID { get; }
        [JsonProperty("memo")]
		public string Memo { get; }
        [JsonProperty("original_amount")]
		public decimal OriginalAmount { get; }
        [JsonProperty("applied_amount")]
		public decimal AppliedAmount { get; }
    }
}
