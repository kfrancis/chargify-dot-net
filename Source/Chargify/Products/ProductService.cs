using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargify
{
    public class ProductService : ChargifyApiBase
    {
        public static readonly string ProductKey = "products";

        public ProductService(string apiKey, string apiPassword, bool useJson)
            : base(apiKey, apiPassword, useJson)
        { }

        public IEnumerable<Product> All()
        {
            string url = string.Format("/{0}", ProductService.ProductKey);
            return GetRequest<List<Product>>(url);
        }

        public IEnumerable<Product> All(int familyId)
        {
            string url = string.Format("/{0}/{1}/{2}", ProductFamilyService.ProductFamilyKey, familyId, ProductService.ProductKey);
            return GetRequest<List<Product>>(url);
        }

        public Product Single(int id)
        {
            string url = string.Format("/{0}/{1}", ProductService.ProductKey, id);
            return GetRequest<Product>(url);
        }

        public Product Single(string handle)
        {
            string url = string.Format("/{0}/handle/{1}", ProductService.ProductKey, handle);
            return GetRequest<Product>(url);
        }

        public Product Create(int familyId, Product product)
        {
            string url = string.Format("/{0}/{1}/{2}", ProductFamilyService.ProductFamilyKey, familyId, ProductService.ProductKey);
            return PostRequest<Product, Product>(product, url);
        }

        public void Archive(Product product)
        {
            throw new NotImplementedException();
            //DeleteRequest("/product_families/{0}/products/{1}", product.product_family.id, product.id);
        }
    }
}
