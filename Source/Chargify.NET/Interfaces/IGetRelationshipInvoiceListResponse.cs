using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    interface IGetRelationshipInvoiceListResponse
    {
        ICollection<RelationshipInvoice> Invoices { get; set; }
    }
}
