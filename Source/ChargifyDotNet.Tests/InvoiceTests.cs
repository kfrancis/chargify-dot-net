using System;
using System.Linq;
using ChargifyNET;
using ChargifyDotNetTests.Base;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class InvoiceTests : ChargifyTestBase
    {
        [TestMethod]
        public void Invoices_Can_Get_List()
        {
            // Act
            var result = Chargify.GetInvoiceList();

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Dictionary<int, Invoice>));
            Assert.IsTrue(result.Count > 0);
            var anInvoice = result.FirstOrDefault().Value;
            Assert.IsTrue(result.FirstOrDefault().Key == anInvoice.ID);
            Assert.AreNotEqual(int.MinValue, anInvoice.ID);
        }

        [TestMethod]
        public void Can_Create_Invoice_Subscription_For_Existing_Customer()
        {
            // Arrange
            var customers = Chargify.GetCustomerList().Keys;
            var referenceValue = customers.FirstOrDefault(systemID => !string.IsNullOrWhiteSpace(systemID));
            ICustomer customer = Chargify.LoadCustomer(referenceValue);
            var products = Chargify.GetProductList().Values;
            var product = products.FirstOrDefault(p => p.PriceInCents > 0 && !p.RequireCreditCard);
            Assert.IsNotNull(product, "No valid product was found.");

            // Act
            var result = Chargify.CreateSubscription(product.Handle, customer.ChargifyID, PaymentCollectionMethod.Invoice);

            // Assert
            //Assert.IsInstanceOfType(result, typeof(Subscription));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Customer);
            Assert.IsNull(result.PaymentProfile);
            Assert.IsTrue(result.SubscriptionID > int.MinValue);
            Assert.IsTrue(result.Customer.ChargifyID > int.MinValue);
            Assert.IsTrue(result.Customer.FirstName == customer.FirstName);
            Assert.IsTrue(result.Customer.LastName == customer.LastName);
            Assert.IsTrue(result.Customer.Email == customer.Email);
            Assert.IsTrue(result.Customer.Phone == customer.Phone);
            Assert.IsTrue(result.Customer.Organization == customer.Organization);
            Assert.IsTrue(result.Customer.SystemID == customer.SystemID);
            Assert.IsTrue(result.ProductPriceInCents == product.PriceInCents);
            Assert.IsTrue(result.ProductPrice == product.Price);
            Assert.IsTrue(result.PaymentCollectionMethod == PaymentCollectionMethod.Invoice);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(result.SubscriptionID, "Automatic cancel due to test"));
        }

        [TestMethod]
        public void Can_Create_Invoice_Subscription()
        {
            // Arrange
            var product = Chargify.GetProductList().Values.DefaultIfEmpty(null).FirstOrDefault(p => p.PriceInCents > 0 && p.RequireCreditCard == false);
            if (product == null) { Assert.Inconclusive("No product to test"); return; }
            var referenceID = Guid.NewGuid().ToString();
            var faker = new Faker("en");
            var newCustomer = new CustomerAttributes(faker.Name.FirstName(), faker.Name.LastName(), faker.Internet.Email(), faker.Phone.PhoneNumber(), faker.Company.CompanyName(), referenceID);

            // Act
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, PaymentCollectionMethod.Invoice);

            // Assert
            //Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.FirstName == newCustomer.FirstName);
            Assert.IsTrue(newSubscription.Customer.LastName == newCustomer.LastName);
            Assert.IsTrue(newSubscription.Customer.Email == newCustomer.Email);
            Assert.IsTrue(newSubscription.Customer.Phone == newCustomer.Phone);
            Assert.IsTrue(newSubscription.Customer.Organization == newCustomer.Organization);
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceID);
            Assert.IsTrue(newSubscription.ProductPriceInCents == product.PriceInCents);
            Assert.IsTrue(newSubscription.ProductPrice == product.Price);
            Assert.IsTrue(newSubscription.PaymentCollectionMethod == PaymentCollectionMethod.Invoice);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));
        }

        [TestMethod]
        public void Invoice_Can_Add_Payment()
        {
            // Arrange
            var invoices = Chargify.GetInvoiceList().Values;
            if (!invoices.Any()) Assert.Inconclusive("There are no valid invoices for use in this test.");
            var invoice = invoices.FirstOrDefault(i => i.AmountDue > 0);
            var memo = Guid.NewGuid().ToString();

            // Act
            var result = Chargify.AddInvoicePayment(invoice.ID, invoice.AmountDue, memo);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IPayment));
            Assert.AreEqual(memo, result.Memo);
            Assert.AreEqual(invoice.AmountDue, result.Amount);
            Assert.AreEqual(invoice.AmountDueInCents, result.AmountInCents);
        }

        [TestMethod]
        public void Invoice_Can_Add_Payment_Cents()
        {
            // Arrange
            var invoices = Chargify.GetInvoiceList().Values;
            if (!invoices.Any()) Assert.Inconclusive("There are no valid invoices for use in this test.");
            var invoice = invoices.FirstOrDefault(i => i.AmountDueInCents > 0);
            var memo = Guid.NewGuid().ToString();

            // Act
            var result = Chargify.AddInvoicePayment(invoice.ID, invoice.AmountDueInCents, memo);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IPayment));
            Assert.AreEqual(memo, result.Memo);
            Assert.AreEqual(invoice.AmountDue, result.Amount);
            Assert.AreEqual(invoice.AmountDueInCents, result.AmountInCents);
        }
    }
}
