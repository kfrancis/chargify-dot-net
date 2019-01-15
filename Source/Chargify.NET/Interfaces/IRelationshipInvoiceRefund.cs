using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public interface IRelationshipInvoiceRefund
    {
        int TransactionID { get; }
        int PaymentID { get; }
        string Memo { get; }
        decimal OriginalAmount { get; }
        decimal AppliedAmount { get; }
    }
}
