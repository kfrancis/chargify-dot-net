using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargify.Products
{
    //public partial class ProductService
    //{
    //    public async Task<IEnumerable<Product>> AllAsync()
    //    {
    //        string url = string.Format("/{0}", ProductService.ProductKey);
    //        return await GetRequestAsync<List<Product>>(url);
    //    }

    //    public async Task<IEnumerable<Product>> AllAsync(int familyId)
    //    {
    //        string url = string.Format("/{0}/{1}/{2}", ProductFamilyService.ProductFamilyKey, familyId, ProductKey);
    //        return await GetRequestAsync<List<Product>>(url);
    //    }

    //    public async Task<Product> SingleAsync(int id)
    //    {
    //        string url = string.Format("/{0}/{1}", ProductKey, id);
    //        return await GetRequestAsync<Product>(url);
    //    }

    //    public async Task<Product> SingleAsync(string handle)
    //    {
    //        string url = string.Format("/{0}/handle/{1}", ProductKey, handle);
    //        return await GetRequestAsync<Product>(url);
    //    }

    //    public async Task<Product> CreateAsync(int familyId, Product product)
    //    {
    //        string url = string.Format("/{0}/{1}/{2}", ProductFamilyService.ProductFamilyKey, familyId, ProductKey);
    //        return await PostRequestAsync<Product, Product>(product, url);
    //    }

    //    public void ArchiveAsync(Product product)
    //    {
    //        throw new NotImplementedException();
    //        //DeleteRequest("/product_families/{0}/products/{1}", product.product_family.id, product.id);
    //    }
    //}
}
