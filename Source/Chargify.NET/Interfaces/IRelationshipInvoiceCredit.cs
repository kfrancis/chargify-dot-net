using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public interface IRelationshipInvoiceCredit
    {
        string Uid { get; }
        string CreditNoteNumber { get; }
        string CreditNoteUid { get; }
        DateTime TransactionTime { get; }
        string Memo { get; }
        decimal OriginalAmount { get; }
        decimal AppliedAmount { get; }
    }
}
