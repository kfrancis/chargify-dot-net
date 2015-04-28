using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using ChargifyNET;

namespace ChargifyConsoleSample
{
    public class Customers
    {
        public Customers() { }

        public void DoSampleCode()
        {
            ChargifyConnect chargify = new ChargifyConnect();
            chargify.apiKey = ConfigurationManager.AppSettings["CHARGIFY_API_KEY"];
            chargify.Password = ConfigurationManager.AppSettings["CHARGIFY_API_PASSWORD"];
            chargify.URL = ConfigurationManager.AppSettings["CHARGIFY_URL"];
            chargify.UseJSON = true;

            // Retrieve a dictionary of all your customers
            IDictionary<string, ICustomer> customerList = chargify.GetCustomerList();

            if (customerList.Count > 0) 
            {
                ICustomer customer = customerList.First().Value as ICustomer;

                Console.WriteLine(string.Format("First Name: {0}", customer.FirstName));
                Console.WriteLine(string.Format("Last Name: {0}", customer.LastName));

                customer.FirstName = "Miguel";

                customer = chargify.UpdateCustomer(customer);

                if (customer != null)
                {
                    // update success
                    Console.WriteLine("Customer update succeeded.");
                }
                else
                {
                    // update failure.
                    Console.WriteLine("Update customer failed with response: ", chargify.LastResponse.ToString());
                }

                Console.WriteLine(string.Format("First Name: {0}", customer.FirstName));
            }

            // You can create a new customer in two steps
            //ICustomer newCustomer = new Customer("Charlie", "Bull", "charlie@example.com", "Chargify", Guid.NewGuid());
            //chargify.CreateCustomer(newCustomer);

            // Or just one .. 
            ICustomer newCustomer = chargify.CreateCustomer("Charlie", "Bull", "charlie@example.com", string.Empty, "Chargify", Guid.NewGuid().ToString());
        }
    }
}
