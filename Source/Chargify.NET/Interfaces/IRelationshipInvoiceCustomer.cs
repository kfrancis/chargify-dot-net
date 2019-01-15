using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public interface IRelationshipInvoiceCustomer
    {
        int ChargifyId { get; }
        string FirstName { get; }
        string LastName { get; }
        string Organisation { get; }
        string Email { get; }
    }
}
