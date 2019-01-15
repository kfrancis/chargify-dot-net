using ChargifyNET.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChargifyNET
{
		public class RelationshipInvoice : ChargifyEntity
    {
        /// <summary>
        /// Unique identifier for the invoice. It is generated automatically by Chargify and has the prefix "inv_" followed by alphanumeric characters.
        /// </summary>
        [JsonProperty("uid")]
		public string Uid { get; set; }

        /// <summary>
        /// ID of the site to which the invoice belongs.
        /// </summary>
        [JsonProperty("site_id")]
		public int SiteID  { get; set; }

        /// <summary>
        /// ID of the customer to which the invoice belongs.
        /// </summary>
        [JsonProperty("customer_id")]
		public int CustomerID  { get; set; }

        /// <summary>
        /// The subscription unique id within Chargify
        /// </summary>
        [JsonProperty("subscription_id")]
		public int SubscriptionID  { get; set; }

        /// <summary>
        /// A unique, identifying string that appears on the invoice and in places the invoice is referenced.
        /// While the UID is long and not appropriate to show to customers, the number is usually shorter and consumable by the customer and the merchant alike.
        /// </summary>
        [JsonProperty("number")]
		public string Number  { get; set; }

        /// <summary>
        /// A monotonically increasing number assigned to invoices as they are created. This number is unique 
        /// within a site and can be used to sort and order invoices.
        /// </summary>
        [JsonProperty("sequence_number")]
		public int SequenceNumber  { get; set; }
        
        /// <summary>
        /// Date the invoice was issued to the customer. This is the date that the invoice was made available for payment.
        /// </summary>
        [JsonProperty("issue_date")]
		public DateTime? IssueDate  { get; set; }
        
        /// <summary>
        /// Date the invoice is due.
        /// </summary>
        [JsonProperty("due_date")]
		public DateTime? DueDate  { get; set; }
        
        /// <summary>
        /// Date the invoice became fully paid.
        /// </summary>
        [JsonProperty("paid_date")]
		public DateTime? PaidDate  { get; set; }
        
        /// <summary>
        /// Current status of the invoice.
        /// </summary>
        [JsonProperty("status")]
		public RelationshipInvoiceStatus Status  { get; set; }

        /// <summary>
        /// The collection method of the invoice, which is either "automatic" (tried and retried on an existing payment method by Chargify) 
        /// or "remittance" (payment must be remitted by the customer or keyed in by the merchant).
        /// </summary>
        [JsonProperty("collection_method")]
		public PaymentCollectionMethod CollectionMethod  { get; set; }

        /// <summary>
        /// A message that is printed on the invoice when it is marked for remittance collection.
        /// It is intended to describe to the customer how they may make payment, and is configured by the merchant.
        /// </summary>
        [JsonProperty("payment_instructions")]
		public string PaymentInstructions  { get; set; }

        /// <summary>
        /// The ISO 4217 currency code (3 character string) representing the currency of invoice transaction.
        /// </summary>
        [JsonProperty("currency")]
		public string Currency  { get; set; }

        /// <summary>
        /// Consolidation level of the invoice, which is applicable to invoice consolidation.
        /// </summary>
        [JsonProperty("consolidation_level")]
		public RelationshipInvoiceCondolidationLevel ConsolidationLevel  { get; set; }

        /// <summary>
        /// For invoices with consolidation_level of child, this specifies the UID of the parent (consolidated) invoice.
        /// </summary>
        [JsonProperty("parent_invoice_uid")]
		public string ParentInvoiceUid  { get; set; }

        /// <summary>
        /// For invoices with consolidation_level of child, this specifies the number of the parent (consolidated) invoice.
        /// </summary>
        [JsonProperty("parent_invoice_number")]
		public int? ParentInvoiceNumber { get; set; }

        /// <summary>
        /// For invoices with consolidation_level of parent, this specifies the ID of the subscription which was the
        /// primary subscription of the subscription group that generated the invoice.
        /// </summary>
        [JsonProperty("group_primary_subscription_id")]
		public string GroupPrimarySubscriptionId  { get; set; }

        /// <summary>
        /// The name of the product subscribed when the invoice was generated.
        /// </summary>
        [JsonProperty("product_name")]
		public string ProductName  { get; set; }

        /// <summary>
        /// The name of the product family subscribed when the invoice was generated.
        /// </summary>
        [JsonProperty("product_family_name")]
		public string ProductFamilyName  { get; set; }

        /// <summary>
        /// Information about the seller (merchant) listed on the masthead of the invoice.
        /// </summary>
        [JsonProperty("seller")]
		public RelationshipInvoiceSeller Seller  { get; set; }

        /// <summary>
        /// Information about the customer who is owner or recipient the invoiced subscription.
        /// </summary>
        [JsonProperty("customer")]
		public RelationshipInvoiceCustomer Customer  { get; set; }

        /// <summary>
        /// The memo printed on invoices of any collection type. This message is in control of the merchant.
        /// </summary>
        [JsonProperty("memo")]
		public string Memo  { get; set; }

        /// <summary>
        /// The invoice billing address.
        /// </summary>
        [JsonProperty("billing_address")]
		public RelationshipInvoiceAddress BillingAddress  { get; set; }

        /// <summary>
        /// The invoice shipping address.
        /// </summary>
        [JsonProperty("shipping_address")]
		public RelationshipInvoiceAddress ShippingAddress  { get; set; }

        /// <summary>
        /// Subtotal of the invoice, which is the sum of all line items before discounts or taxes.
        /// </summary>
        [JsonProperty("subtotal_amount")]
		public decimal SubtotalAmount  { get; set; }

        /// <summary>
        /// Total discount applied to the invoice.
        /// </summary>
        [JsonProperty("discount_amount")]
		public decimal DiscountAmount  { get; set; }

        /// <summary>
        /// Total tax on the invoice.
        /// </summary>
        [JsonProperty("TaxAmount")]
		public decimal TaxAmount  { get; set; }

        /// <summary>
        /// The invoice total, which is subtotal_amount - discount_amount + tax_amount.'
        /// </summary>
        [JsonProperty("total_amount")]
		public decimal TotalAmount  { get; set; }
        
        /// <summary>
        /// The amount of credit (from credit notes) applied to this invoice.
        /// Credits offset the amount due from the customer.
        /// </summary>
        [JsonProperty("credit_amount")]
		public decimal CreditAmount  { get; set; }
        
        /// <summary>
        /// Refund amount
        /// </summary>
        [JsonProperty("refund_amount")]
		public decimal RefundAmount  { get; set; }
        
        /// <summary>
        /// The amount paid on the invoice by the customer.
        /// </summary>
        [JsonProperty("paid_amount")]
		public decimal PaidAmount  { get; set; }
        
        /// <summary>
        /// Amount due on the invoice, which is total_amount - credit_amount - paid_amount.
        /// </summary>
        [JsonProperty("due_amount")]
		public decimal DueAmount  { get; set; }
        
        /// <summary>
        /// Line items on the invoice.
        /// </summary>
        [JsonProperty("line_items")]
		public List<RelationshipInvoiceLineItem> LineItems  { get; set; }

        /// <summary>
        /// Discounts in the invoice
        /// </summary>
        [JsonProperty("discounts")]
		public List<RelationshipInvoiceDiscount> Discounts  { get; set; }
        
        /// <summary>
        /// Taxes on the invoice
        /// </summary>
        [JsonProperty("taxes")]
		public List<RelationshipInvoiceTax> Taxes  { get; set; }
        
        /// <summary>
        /// Credits on the invoice
        /// </summary>
        [JsonProperty("credits")]
		public List<RelationshipInvoiceCredit> Credits  { get; set; }
        
        /// <summary>
        /// Refunds on the invoice
        /// </summary>
        [JsonProperty("refunds")]
		public List<RelationshipInvoiceRefund> Refunds  { get; set; }
        
        /// <summary>
        /// Payments on the invoice
        /// </summary>
        [JsonProperty("payments")]
		public List<RelationshipInvoicePayment> Payments  { get; set; }
        
        /// <summary>
        /// Custom fields
        /// </summary>
        [JsonProperty("customFields")]
		public List<RelationshipInvoiceCustomField> CustomFields  { get; set; }
        
        /// <summary>
        /// The public URL of the invoice
        /// </summary>
        [JsonProperty("public_url")]
		public string PublicUrl  { get; set; }

    }

    /// <summary>
    /// Possible values of the Status field in a Relationship Invoice
    /// </summary>
	public enum RelationshipInvoiceStatus
    {
        draft,
        pending,
        open,
        paid,
        voided,
        canceled
    }

    /// <summary>
    /// Possible values for ConsolidationLevel in a Relationship Invoice
    /// </summary>
	public enum RelationshipInvoiceCondolidationLevel
    {
        /// <summary>
        /// A normal invoice with no consolidation.
        /// </summary>
        none,
        /// <summary>
        /// An invoice segment which has been combined into a consolidated invoice.
        /// </summary>
        child,
        /// <summary>
        /// A consolidated invoice, whose contents are composed of invoice segments.
        /// </summary>
        parent
    }
}
