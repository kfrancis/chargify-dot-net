using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargify
{
    public class ChargifyClient
    {
        #region Constructors
        public ChargifyClient()
            : this(Config.ApiKey, Config.ApiPassword, Config.UseJson)
        {
        }

        public ChargifyClient(string apiKey, string apiPassword, bool useJson)
        {
            this.Products = new ProductService(apiKey, apiPassword, useJson);
            this.ProductFamilies = new ProductFamilyService(apiKey, apiPassword, useJson);
        }
        #endregion

        #region Accessors
        
        public ProductService Products { get; private set; }
        public ProductFamilyService ProductFamilies { get; private set; }

        #endregion
    }
}
