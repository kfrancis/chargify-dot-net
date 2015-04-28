using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargify
{
    public class ProductFamily : IChargifyEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public string handle { get; set; }
        public string description { get; set; }
        public string accounting_code { get; set; }
    }
}
