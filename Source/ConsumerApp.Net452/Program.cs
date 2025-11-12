using ChargifyNET;
using ChargifyNET.Configuration;
using System;
using System.Linq;

namespace ConsumerApp.Net452
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // Retrieve configuration via factory (web.config/app.config section)
                var retriever = ChargifyConfigFactory.Create();
                var account = retriever.GetDefaultOrFirst();

                var chargify = new ChargifyConnect(account)
                {
                    ProtocolType = System.Net.SecurityProtocolType.Tls12
                };

                var products = chargify.GetProductList().Values;
                Console.WriteLine("Products:\n\n{0}", string.Join(Environment.NewLine, products.Select(x => x.Name)));
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
