using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public interface IRelationshipInvoicePayment
    {
        DateTime TransactionTime { get; }
        string Memo { get; }
        decimal OriginalAmount { get; }
        decimal AppliedAmount { get; }
        IRelationshipInvoicePaymentMethod PaymentMethod { get; }
        int TransactionID { get; }
        bool Prepayment { get; }
    }
}
