using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public class RelationshipInvoicePayment
    {
        [JsonProperty("TransactionTime")]
		public DateTime TransactionTime { get; }
        [JsonProperty("Memo")]
		public string Memo { get; }
        [JsonProperty("OriginalAmount")]
		public decimal OriginalAmount { get; }
        [JsonProperty("AppliedAmount")]
		public decimal AppliedAmount { get; }
        [JsonProperty("PaymentMethod")]
		public RelationshipInvoicePaymentMethod PaymentMethod { get; }
        [JsonProperty("TransactionID")]
		public int TransactionID { get; }
        [JsonProperty("Prepayment")]
		public bool Prepayment { get; }
    }
}
