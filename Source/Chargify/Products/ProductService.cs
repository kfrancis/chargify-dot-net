using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargify
{
    public partial class ProductService : ChargifyApiBase, IProductService
    {
        public static readonly string ProductKey = "products";

        public ProductService(string apiKey, string apiPassword, bool useJson)
            : base(apiKey, apiPassword, useJson)
        { }

        public IEnumerable<Product> All()
        {
            string url = string.Format(format: "/{0}", arg0: ProductKey);
            return GetRequest<List<Product>>(url);
        }

        public IEnumerable<Product> All(int familyId)
        {
            string url = string.Format(format: "/{0}/{1}/{2}", arg0: ProductFamilyService.ProductFamilyKey, arg1: familyId, arg2: ProductKey);
            return GetRequest<List<Product>>(url);
        }

        public Product Single(int id)
        {
            string url = string.Format(format: "/{0}/{1}", arg0: ProductKey, arg1: id);
            return GetRequest<Product>(url);
        }

        public Product Single(string handle)
        {
            string url = string.Format(format: "/{0}/handle/{1}", arg0: ProductKey, arg1: handle);
            return GetRequest<Product>(url);
        }

        public Product Create(int familyId, Product product)
        {
            string url = string.Format(format: "/{0}/{1}/{2}", arg0: ProductFamilyService.ProductFamilyKey, arg1: familyId, arg2: ProductKey);
            return PostRequest<Product, Product>(product, url);
        }

        public void Archive(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> AllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> AllAsync(int familyId)
        {
            throw new NotImplementedException();
        }

        public Task<Product> SingleAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> SingleAsync(string handle)
        {
            throw new NotImplementedException();
        }

        public Task<Product> CreateAsync(int familyId, Product product)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }

    public interface IProductService
    {
        IEnumerable<Product> All();
        Task<IEnumerable<Product>> AllAsync();

        IEnumerable<Product> All(int familyId);
        Task<IEnumerable<Product>> AllAsync(int familyId);

        Product Single(int id);
        Task<Product> SingleAsync(int id);

        Product Single(string handle);
        Task<Product> SingleAsync(string handle);

        Product Create(int familyId, Product product);
        Task<Product> CreateAsync(int familyId, Product product);

        void Archive(Product product);
        Task ArchiveAsync(Product product);
    }
}
