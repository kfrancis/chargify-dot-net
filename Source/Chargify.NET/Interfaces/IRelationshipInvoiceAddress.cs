using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public interface IRelationshipInvoiceAddress
    {
        string Street { get; }
        string Line2 { get; }
        string City { get; }
        string State { get; }
        string Zip { get; }
        string Country { get; }
    }
}
