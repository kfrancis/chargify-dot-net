using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChargifyNET;
using System.Configuration;

namespace ChargifyConsoleSample
{
    public class Products
    {
        public Products() { }

        public void DoSampleCode()
        {
            ChargifyConnect chargify = new ChargifyConnect();
            chargify.apiKey = ConfigurationManager.AppSettings["CHARGIFY_API_KEY"];
            chargify.Password = ConfigurationManager.AppSettings["CHARGIFY_API_PASSWORD"];
            chargify.URL = ConfigurationManager.AppSettings["CHARGIFY_URL"];

            var family = chargify.GetProductFamilyList().Values.FirstOrDefault();
            var newprod = chargify.CreateProduct(family.ID, "22 Bananas", "22-bananas", 5000, 1, IntervalUnit.Month, "toys2u-dfw-acct-22-5", "5 Pickups Per Month Test Product HS");

            //// Retrieve a list of all your products
            //IDictionary<int, IProduct> products = chargify.GetProductList();

            //Console.WriteLine("\nProduct List:");
            //foreach (IProduct p in products.Values)
            //{
            //    Console.WriteLine(string.Format("{0}: {1}", p.ProductFamily.Name, p.Name));
            //}
            //Console.WriteLine("");

            //// Find a single product by its handle
            //IProduct product = chargify.LoadProduct(products.First().Value.Handle);
            //Console.WriteLine(string.Format("Found product '{0}'.", product.ToString()));
        }
    }
}
