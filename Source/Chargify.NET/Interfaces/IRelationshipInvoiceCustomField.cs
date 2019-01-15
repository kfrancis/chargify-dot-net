using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyNET
{
    public interface IRelationshipInvoiceCustomField
    {
        string Name { get; }
        string Value { get; }
    }
}
