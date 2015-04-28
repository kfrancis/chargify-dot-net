using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChargifyNET;
using System.Configuration;

namespace ChargifyConsoleSample
{
    public class Subscriptions
    {
        public Subscriptions() { }

        public void DoSampleCode()
        {
            ChargifyConnect chargify = new ChargifyConnect();
            chargify.apiKey = ConfigurationManager.AppSettings["CHARGIFY_API_KEY"];
            chargify.Password = ConfigurationManager.AppSettings["CHARGIFY_API_PASSWORD"];
            chargify.URL = ConfigurationManager.AppSettings["CHARGIFY_URL"];

            // Create a new customer and a subscription for him
            ICustomerAttributes scottPilgrim = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", Guid.NewGuid().ToString());
            
            ICreditCardAttributes scottsPaymentInfo = new CreditCardAttributes();
            scottsPaymentInfo.FirstName = scottPilgrim.FirstName;
            scottsPaymentInfo.LastName = scottPilgrim.LastName;
            scottsPaymentInfo.ExpirationMonth = 1;
            scottsPaymentInfo.ExpirationYear = 2020;
            scottsPaymentInfo.FullNumber = "1";
            scottsPaymentInfo.CVV = "123";
            scottsPaymentInfo.BillingAddress = "123 Main St.";
            scottsPaymentInfo.BillingCity = "New York";
            scottsPaymentInfo.BillingCountry = "US";
            scottsPaymentInfo.BillingState = "New York";
            scottsPaymentInfo.BillingZip = "10001";

            ISubscription newSubscription = chargify.CreateSubscription("basic", scottPilgrim, scottsPaymentInfo);
            if (newSubscription != null)
            {
                // subscription success.
                Console.WriteLine("Subscription succeeded.");
            }
            else
            {
                // subscription failure.
                Console.WriteLine("Update customer failed with response: ", chargify.LastResponse.ToString());
            }

            ICharge oneTimeChargeResults = chargify.CreateCharge(newSubscription.SubscriptionID, 123.45m, "Testing One-Time Charge");
            if (oneTimeChargeResults != null)
            {
                // one-time charge success.
                Console.WriteLine(string.Format("Charge succeeded: {0}", oneTimeChargeResults.Success.ToString()));
            }
            else
            {
                // one time charge failure.
                Console.WriteLine("One-time charge failed with response: ", chargify.LastResponse.ToString());
            }

            IDictionary<int, ITransaction> transactions = chargify.GetTransactionsForSubscription(newSubscription.SubscriptionID, new List<TransactionType>() { TransactionType.Payment });
            // Grab the last payment transaction, which we will refund (will be the one-time charge we just assessed)
            ITransaction firstTransaction = transactions.First().Value;
            IRefund chargeRefund = chargify.CreateRefund(newSubscription.SubscriptionID, firstTransaction.ID, firstTransaction.AmountInCents, "Test Refund");

            if (chargeRefund != null)
            {
                Console.WriteLine("Refund was: " + (chargeRefund.Success ? "successful" : "unsuccessful"));
            }

            bool result = chargify.DeleteSubscription(newSubscription.SubscriptionID, "Testing Reactivation");
            if (result)
            {
                ISubscription reactivatedSubscription = chargify.ReactivateSubscription(newSubscription.SubscriptionID);
                if (reactivatedSubscription != null)
                {
                    Console.WriteLine("Reactivation succeeded!");
                }
                else
                {
                    Console.WriteLine("Reactivation failed with response: ", chargify.LastResponse.ToString());
                }

                // Currently a bug if you say "true" for the last two parameters. Being worked on.
                reactivatedSubscription = chargify.MigrateSubscriptionProduct(reactivatedSubscription.SubscriptionID, "ultimate", true, true);
                if (reactivatedSubscription != null)
                {
                    Console.WriteLine("Migration succeeded!");
                }
                else
                {

                }
            }
            else
            {
                Console.WriteLine("Cancellation failed with response: ", chargify.LastResponse.ToString());
            }

            //IDictionary<int, ITransaction> transactions = chargify.GetTransactionsForSubscription(newSubscription.SubscriptionID);
            //if ((transactions != null) && (transactions.Count > 0))
            //{
            //    foreach (ITransaction transaction in transactions.Values)
            //    {
            //        Console.WriteLine(string.Format("Date: {0}, Who: {1}, Type: {2}, Memo: {3}, Amount: {4}", transaction.CreatedAt, transaction.SubscriptionID, transaction.ProductID, transaction.Memo, transaction.Amount));
            //    }
            //}
        }
    }
}
