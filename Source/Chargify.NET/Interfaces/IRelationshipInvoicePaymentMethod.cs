using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public interface IRelationshipInvoicePaymentMethod
    {
        string Details { get; }
        string Kind { get; }
        string Memo { get; }
        string Type { get; }
        string CardBrand { get; }
        string CardExpiration { get; }
        string LastFour { get; }
        string MaskedCardNumber { get; }
    }
}
