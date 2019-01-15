using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public interface IRelationshipInvoiceDiscount
    {
        string Uid { get; set; }
        string Title { get; set; }
        string Code { get; set; }
        string SourceType { get; set; }
        int SourceId { get; set; }
        string DiscountType { get; set; }
        decimal Percentage { get; set; }
        decimal EligibleAmount { get; set; }
        decimal DiscountAmount { get; set; }
        List<IRelationshipInvoiceLineItemBreakout> LineItemBreakouts { get; set; }
    }
}
