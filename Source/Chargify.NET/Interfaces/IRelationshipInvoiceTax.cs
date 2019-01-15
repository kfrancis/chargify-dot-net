using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public interface IRelationshipInvoiceTax
    {
        string Uid { get; }
        string Title { get; }
        string SourceType { get; }
        int SourceId { get; }
        decimal Percentage { get; }
        decimal TaxableAmount { get; }
        decimal TaxAmount { get; }
        List<IRelationshipInvoiceLineItemBreakout> LineItemBreakouts { get; }
    }
}
