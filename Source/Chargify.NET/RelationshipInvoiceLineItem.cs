using Newtonsoft.Json;
using System;

namespace ChargifyNET
{
    public class RelationshipInvoiceLineItem
    {
        /// <summary>
        /// Unique identifier for the line item.Useful when cross-referencing the line against individual discounts in the discounts or taxes lists.
        /// </summary>
        [JsonProperty("uid")]
		public string Uid { get; set; }
        /// <summary>
        /// A short descriptor for the charge or item represented by this line.
        /// </summary>
        [JsonProperty("title")]
		public string Title { get; set; }
        /// <summary>
        /// Detailed description for the charge or item represented by this line.May include proration details in plain text.
        /// Note: this string may contain line breaks that are hints for the best display format on the invoice.
        /// </summary>
        [JsonProperty("description")]
		public string Description { get; set; }
        /// <summary>
        /// The quantity or count of units billed by the line item.
        /// This is a decimal number represented as a string. (See "About Decimal Numbers".)
        /// </summary>
        [JsonProperty("quantity")]
		public string Quantity { get; set; }
        /// <summary>
        /// The price per unit for the line item.
        /// When tiered pricing was used(i.e.not every unit was actually priced at the same price) this will be the blended 
        /// average cost per unit and the tiered_unit_price field will be set to true.
        /// </summary>
        [JsonProperty("unit_price")]
		public decimal UnitPrice { get; set; }
        /// <summary>
        /// The line subtotal, generally calculated as quantity* unit_price.This is the canonical amount of record for the 
        /// line - when rounding differences are in play, subtotal_amount takes precedence over the value derived from quantity* unit_price (which may not have the proper precision to exactly equal this amount).
        /// </summary>
        [JsonProperty("subtotal_amount")]
		public decimal SubtotalAmount { get; set; }
        /// <summary>
        /// The approximate discount applied to just this line.
        /// The value is approximated in cases where rounding errors make it difficult to apportion exactly a total discount among many lines. Several lines may have been summed prior to applying the discount to arrive at discount_amount for the invoice - backing that out to the discount on a single line may introduce rounding or precision errors.
        /// </summary>
        [JsonProperty("discount_amount")]
		public decimal DiscountAmount { get; set; }
        /// <summary>
        /// The approximate tax applied to just this line.
        /// The value is approximated in cases where rounding errors make it difficult to apportion exactly a total tax among many lines. Several lines may have been summed prior to applying the tax rate to arrive at tax_amount for the invoice - backing that out to the tax on a single line may introduce rounding or precision errors.
        /// </summary>
        [JsonProperty("tax_amount")]
		public decimal TaxAmount { get; set; }
        /// <summary>
        /// The non-canonical total amount for the line.
        /// subtotal_amount is the canonical amount for a line. The invoice total_amount is derived from the sum of the line subtotal_amounts and discounts or taxes applied thereafter. Therefore, due to rounding or precision errors, the sum of line total_amounts may not equal the invoice total_amount.
        /// </summary>
        [JsonProperty("total_amount")]
		public decimal TotalAmount { get; set; }
        /// <summary>
        /// When true, indicates that the actual pricing scheme for the line was tiered, so the unit_price shown is the blended average for all units.
        /// </summary>
        [JsonProperty("tiered_unit_price")]
		public bool TieredUnitPrice { get; set; }
        /// <summary>
        /// Start date for the period covered by this line.The format is "YYYY-MM-DD".
        /// For periodic charges paid in advance, this date will match the billing date, and the end date will be in the future.
        /// For non-periodic charges, this date and the end date will match.
        /// </summary>
        [JsonProperty("period_range_start")]
		public DateTime PeriodRangeStart { get; set; }
        /// <summary>
        /// End date for the period covered by this line.The format is "YYYY-MM-DD".
        /// For periodic charges paid in advance, this date will match the next (future) billing date.
        /// For periodic charges paid in arrears (e.g.metered charges), this date will be the date of the current billing date.
        /// For non-periodic charges, this date and the start date will match.
        /// </summary>
        [JsonProperty("period_range_end")]
		public DateTime PeriodRangeEnd { get; set; }
        /// <summary>
        /// The ID of the product subscribed when the charge was made.
        /// This may be set even for component charges, so true product-only (non-component) charges will also have a nil component_id.
        /// </summary>
        [JsonProperty("product_id")]
		public int ProductID { get; set; }
        /// <summary>
        /// The version of the product subscribed when the charge was made.
        /// </summary>
        [JsonProperty("product_version")]
		public int ProductVersion { get; set; }
        /// <summary>
        /// The ID of the component being billed. Will be nil for non-component charges.
        /// </summary>
        [JsonProperty("component_id")]
		public int? ComponentID { get; set; }
        /// <summary>
        /// The price point ID of the component being billed. Will be nil for non-component charges.
        /// </summary>
        [JsonProperty("price_point_id")]
		public int? PricePointID { get; set; }
    }
}