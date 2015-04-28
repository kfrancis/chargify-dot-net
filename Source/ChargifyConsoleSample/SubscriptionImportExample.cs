using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChargifyNET;
using System.Configuration;

namespace ChargifyConsoleSample
{
    public class SubscriptionImportExample
    {
        public SubscriptionImportExample() { }

        public void DoSampleCode()
        {
            ChargifyConnect chargify = new ChargifyConnect();
            chargify.apiKey = ConfigurationManager.AppSettings["CHARGIFY_API_KEY"];
            chargify.Password = ConfigurationManager.AppSettings["CHARGIFY_API_PASSWORD"];
            chargify.URL = ConfigurationManager.AppSettings["CHARGIFY_URL"];
            chargify.UseJSON = true;

            // This could perhaps be read from the gateway?
            ICustomerAttributes charlie = new CustomerAttributes("Charlie", "Guy", "charlie@example.com", "YourCompany", Guid.NewGuid().ToString());

            // This as well, I'm assuming ...
            IPaymentProfileAttributes existingProfile = new PaymentProfileAttributes("12345", "67890", VaultType.AuthorizeNET, 2020, 12, CardType.Visa, "1111");

            // Now create the subscription importing from the vault
            ISubscription charlieSubscription = chargify.CreateSubscription("basic", charlie, DateTime.Now, existingProfile);
        }
    }
}
