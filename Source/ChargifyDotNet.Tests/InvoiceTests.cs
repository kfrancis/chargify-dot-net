using System;
using System.Linq;
using ChargifyNET;
using ChargifyDotNetTests.Base;
using Bogus;
using ChargifyDotNet.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class InvoiceTests : ChargifyTestBase
    {
        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Invoices_Can_Get_List(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Act
            var result = Chargify.GetInvoiceList();

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Dictionary<int, Invoice>));
            Assert.IsNotEmpty(result);
            var anInvoice = result.FirstOrDefault().Value;
            Assert.AreEqual(anInvoice.ID, result.FirstOrDefault().Key);
            Assert.AreNotEqual(int.MinValue, anInvoice.ID);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Can_Create_Invoice_Subscription_For_Existing_Customer(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var customers = Chargify.GetCustomerList().Keys;
            var referenceValue = customers.FirstOrDefault(systemID => !string.IsNullOrWhiteSpace(systemID));
            var customer = Chargify.LoadCustomer(referenceValue);
            var products = Chargify.GetProductList().Values;
            var product = products.FirstOrDefault(p => p.PriceInCents > 0 && !p.RequireCreditCard);
            Assert.IsNotNull(product, "No valid product was found.");

            // Act
            var result = Chargify.CreateSubscription(product.Handle, customer.ChargifyID, PaymentCollectionMethod.Remittance);

            // Assert
            //Assert.IsInstanceOfType(result, typeof(Subscription));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Customer);
            Assert.IsNull(result.PaymentProfile);
            Assert.IsGreaterThan(int.MinValue, result.SubscriptionID);
            Assert.IsGreaterThan(int.MinValue, result.Customer.ChargifyID);
            Assert.AreEqual(customer.FirstName, result.Customer.FirstName);
            Assert.AreEqual(customer.LastName, result.Customer.LastName);
            Assert.AreEqual(customer.Email, result.Customer.Email);
            Assert.AreEqual(customer.Phone, result.Customer.Phone);
            Assert.AreEqual(customer.Organization, result.Customer.Organization);
            Assert.AreEqual(customer.SystemID, result.Customer.SystemID);
            Assert.AreEqual(product.PriceInCents, result.ProductPriceInCents);
            Assert.AreEqual(product.Price, result.ProductPrice);
            Assert.AreEqual(PaymentCollectionMethod.Remittance, result.PaymentCollectionMethod);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(result.SubscriptionID, "Automatic cancel due to test"));

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Can_Create_Invoice_Subscription(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var product = Chargify.GetProductList().Values.DefaultIfEmpty(null).FirstOrDefault(p => p.PriceInCents > 0 && p.RequireCreditCard == false);
            if (product == null) { Assert.Inconclusive("No product to test"); return; }
            var referenceID = Guid.NewGuid().ToString();
            var faker = new Faker("en");
            var newCustomer = new CustomerAttributes(faker.Name.FirstName(), faker.Name.LastName(), faker.Internet.Email(), faker.Phone.PhoneNumber(), faker.Company.CompanyName(), referenceID);

            // Act
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, PaymentCollectionMethod.Remittance);

            // Assert
            //Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNull(newSubscription.PaymentProfile);
            Assert.IsGreaterThan(int.MinValue, newSubscription.SubscriptionID);
            Assert.IsGreaterThan(int.MinValue, newSubscription.Customer.ChargifyID);
            Assert.AreEqual(newCustomer.FirstName, newSubscription.Customer.FirstName);
            Assert.AreEqual(newCustomer.LastName, newSubscription.Customer.LastName);
            Assert.AreEqual(newCustomer.Email, newSubscription.Customer.Email);
            Assert.AreEqual(newCustomer.Phone, newSubscription.Customer.Phone);
            Assert.AreEqual(newCustomer.Organization, newSubscription.Customer.Organization);
            Assert.AreEqual(referenceID, newSubscription.Customer.SystemID);
            Assert.AreEqual(product.PriceInCents, newSubscription.ProductPriceInCents);
            Assert.AreEqual(product.Price, newSubscription.ProductPrice);
            Assert.AreEqual(PaymentCollectionMethod.Remittance, newSubscription.PaymentCollectionMethod);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Invoice_Can_Add_Payment(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Invoice_Can_Add_Payment_Cents(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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

            SetJson(!isJson);
        }
    }
}
