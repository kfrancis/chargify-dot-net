namespace ChargifyNET
{
    public interface IRelationshipInvoiceLineItemBreakout
    {
        string Uid { get; set; }
        string EligableAmount { get; set; }
        string DiscountAmount { get; set; }
    }
}