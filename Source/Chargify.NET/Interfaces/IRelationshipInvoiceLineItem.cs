using System;

namespace ChargifyNET
{
    public interface IRelationshipInvoiceLineItem
    {
        /// <summary>
        /// Unique identifier for the line item.Useful when cross-referencing the line against individual discounts in the discounts or taxes lists.
        /// </summary>
        string Uid { get; }
        /// <summary>
        /// A short descriptor for the charge or item represented by this line.
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Detailed description for the charge or item represented by this line.May include proration details in plain text.
        /// Note: this string may contain line breaks that are hints for the best display format on the invoice.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The quantity or count of units billed by the line item.
        /// This is a decimal number represented as a string. (See "About Decimal Numbers".)
        /// </summary>
        string Quantity { get; }
        /// <summary>
        /// The price per unit for the line item.
        /// When tiered pricing was used(i.e.not every unit was actually priced at the same price) this will be the blended 
        /// average cost per unit and the tiered_unit_price field will be set to true.
        /// </summary>
        decimal UnitPrice { get; }
        /// <summary>
        /// The line subtotal, generally calculated as quantity* unit_price.This is the canonical amount of record for the 
        /// line - when rounding differences are in play, subtotal_amount takes precedence over the value derived from quantity* unit_price (which may not have the proper precision to exactly equal this amount).
        /// </summary>
        decimal SubtotalAmount { get; }
        /// <summary>
        /// The approximate discount applied to just this line.
        /// The value is approximated in cases where rounding errors make it difficult to apportion exactly a total discount among many lines. Several lines may have been summed prior to applying the discount to arrive at discount_amount for the invoice - backing that out to the discount on a single line may introduce rounding or precision errors.
        /// </summary>
        decimal DiscountAmount { get; }
        /// <summary>
        /// The approximate tax applied to just this line.
        /// The value is approximated in cases where rounding errors make it difficult to apportion exactly a total tax among many lines. Several lines may have been summed prior to applying the tax rate to arrive at tax_amount for the invoice - backing that out to the tax on a single line may introduce rounding or precision errors.
        /// </summary>
        decimal TaxAmount { get; }
        /// <summary>
        /// The non-canonical total amount for the line.
        /// subtotal_amount is the canonical amount for a line. The invoice total_amount is derived from the sum of the line subtotal_amounts and discounts or taxes applied thereafter. Therefore, due to rounding or precision errors, the sum of line total_amounts may not equal the invoice total_amount.
        /// </summary>
        decimal TotalAmount { get; }
        /// <summary>
        /// When true, indicates that the actual pricing scheme for the line was tiered, so the unit_price shown is the blended average for all units.
        /// </summary>
        bool TieredUnitPrice { get; }
        /// <summary>
        /// Start date for the period covered by this line.The format is "YYYY-MM-DD".
        /// For periodic charges paid in advance, this date will match the billing date, and the end date will be in the future.
        /// For non-periodic charges, this date and the end date will match.
        /// </summary>
        DateTime PeriodRangeStart { get; }
        /// <summary>
        /// End date for the period covered by this line.The format is "YYYY-MM-DD".
        /// For periodic charges paid in advance, this date will match the next (future) billing date.
        /// For periodic charges paid in arrears (e.g.metered charges), this date will be the date of the current billing date.
        /// For non-periodic charges, this date and the start date will match.
        /// </summary>
        DateTime PeriodRangeEnd { get; }
        /// <summary>
        /// The ID of the product subscribed when the charge was made.
        /// This may be set even for component charges, so true product-only (non-component) charges will also have a nil component_id.
        /// </summary>
        int ProductID { get; }
        /// <summary>
        /// The version of the product subscribed when the charge was made.
        /// </summary>
        int ProductVersion { get; }
        /// <summary>
        /// The ID of the component being billed. Will be nil for non-component charges.
        /// </summary>
        int? ComponentID { get; }
        /// <summary>
        /// The price point ID of the component being billed. Will be nil for non-component charges.
        /// </summary>
        int? PricePointID { get; }
    }
}