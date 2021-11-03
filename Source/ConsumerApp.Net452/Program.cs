﻿using ChargifyNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerApp.Net452
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Don't forget to see the v1 API key and subdomain
                var chargify = new ChargifyConnect("https://subdomain.chargify.com/", "", "X");
                chargify.ProtocolType = System.Net.SecurityProtocolType.Tls12;
                var products = chargify.GetProductList().Values;
                Console.WriteLine("Products:\n\n{0}", string.Join(System.Environment.NewLine, products.Select(x => x.Name)));
                Console.Write(System.Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
