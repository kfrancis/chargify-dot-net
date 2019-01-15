using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoiceCredit
    {
        [JsonProperty("uid")]
		public string Uid { get; }
        [JsonProperty("credit_note_number")]
		public string CreditNoteNumber { get; }
        [JsonProperty("credit_note_uid")]
		public string CreditNoteUid { get; }
        [JsonProperty("transaction_time")]
		public DateTime TransactionTime { get; }
        [JsonProperty("memo")]
		public string Memo { get; }
        [JsonProperty("original_amount")]
		public decimal OriginalAmount { get; }
        [JsonProperty("applied_amount")]
		public decimal AppliedAmount { get; }
    }
}
