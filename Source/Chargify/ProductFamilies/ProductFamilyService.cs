using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargify
{
    public class ProductFamilyService: ChargifyApiBase
    {
        public static readonly string ProductFamilyKey = "product_families";

        public ProductFamilyService(string apiKey, string apiPassword, bool useJson)
            : base(apiKey, apiPassword, useJson)
        { }

        public IEnumerable<ProductFamily> All()
        {
            string url = string.Format("/{0}", ProductFamilyKey);
            return GetRequest<List<ProductFamily>>(url);
        }
    }
}
