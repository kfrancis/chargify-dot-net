using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChargifyNET;

namespace ChargifyConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chargify Sample");
            try
            {
                //Customers customerSample = new Customers();
                //customerSample.DoSampleCode();

                Products productSample = new Products();
                productSample.DoSampleCode();

                //Subscriptions subscriptionSample = new Subscriptions();
                //subscriptionSample.DoSampleCode();

            }
            catch (ChargifyException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
