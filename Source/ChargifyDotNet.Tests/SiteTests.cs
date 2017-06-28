using System;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class SiteTests : ChargifyTestBase
    {
        [TestInitialize]
        public void Startup()
        {
            int familyId = int.MinValue;
            var productFamilyList = Chargify.GetProductFamilyList();
            if (productFamilyList.Count == 0)
            {
                var newProductFamilyId = Guid.NewGuid().ToString();
                var newProductFamily = Chargify.CreateProductFamily(new ProductFamily(newProductFamilyId, newProductFamilyId, null, null));
                familyId = newProductFamily.ID;
            }
            else
            {
                familyId = productFamilyList.FirstOrDefault().Key;
            }

            var productId = int.MinValue;
            var productHandle = string.Empty;
            var productList = Chargify.GetProductList();
            if (productList.Count == 0)
            {
                var newProductId = Guid.NewGuid().ToString();
                var newProduct = Chargify.CreateProduct(familyId, newProductId, newProductId.Replace("-", "_"), 100, 1, IntervalUnit.Month, null, newProductId);
                productId = newProduct.ID;
                productHandle = newProduct.Handle;
            }
            else
            {
                productId = productList.FirstOrDefault().Key;
                productHandle = productList.FirstOrDefault().Value.Handle;
            }

            var customerId = int.MinValue;
            var customerList = Chargify.GetCustomerList();
            string referenceID = Guid.NewGuid().ToString();
            Customer customer = null;
            if (customerList.Count == 0)
            {
                var newCustomer = new Customer()
                {
                    FirstName = Faker.Name.FirstName(),
                    LastName = Faker.Name.LastName(),
                    Email = Faker.Internet.Email(),
                    Phone = Faker.Phone.PhoneNumber(),
                    Organization = Faker.Company.CompanyName(),
                    SystemID = referenceID,
                    ShippingAddress = Faker.Address.StreetAddress(false),
                    ShippingAddress2 = Faker.Address.SecondaryAddress(),
                    ShippingCity = Faker.Address.City(),
                    ShippingState = Faker.Address.StateAbbr(),
                    ShippingZip = Faker.Address.ZipCode(),
                    ShippingCountry = "US"
                };

                customer = Chargify.CreateCustomer(newCustomer) as Customer;
                customerId = customer.ChargifyID;
            }
            else
            {
                customer = customerList.FirstOrDefault().Value as Customer;
                customerId = customer.ChargifyID;
            }

            var subscriptionList = Chargify.GetSubscriptionList();
            if (subscriptionList.Count == 0)
            {
                var expMonth = DateTime.Now.AddMonths(1).Month;
                var expYear = DateTime.Now.AddMonths(12).Year;
                var newPaymentInfo = GetTestPaymentMethod(customer);
                var newSubscription = Chargify.CreateSubscription(productHandle, customer.ChargifyID, newPaymentInfo);
            }
        }

        [TestMethod, Ignore]
        public void Sites_Can_Clear_All()
        {
            // Arrange
            var productFamilyCount = Chargify.GetProductFamilyList().Count;
            var productCount = Chargify.GetProductList().Count;
            var subscriptionCount = Chargify.GetSubscriptionList().Count;
            var customerCount = Chargify.GetCustomerList().Count;

            // Act
            var result = Chargify.ClearTestSite(SiteCleanupScope.All);
            var newProductFamilyCount = Chargify.GetProductFamilyList().Count;
            var newProductCount = Chargify.GetProductList().Count;
            var newSubscriptionCount = Chargify.GetSubscriptionList().Count;
            var newCustomerCount = Chargify.GetCustomerList().Count;

            // Assert
            //Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue(result);
            Assert.AreNotEqual(productFamilyCount, newProductFamilyCount);
            Assert.AreNotEqual(productCount, newProductCount);
            Assert.AreNotEqual(subscriptionCount, newSubscriptionCount);
            Assert.AreNotEqual(customerCount, newCustomerCount);
            Assert.AreEqual(0, newProductFamilyCount);
            Assert.AreEqual(0, newProductCount);
            Assert.AreEqual(0, newSubscriptionCount);
            Assert.AreEqual(0, newCustomerCount);
        }

        [TestMethod, Ignore]
        public void Sites_Can_Clear_Customers()
        {
            // Arrange
            var productFamilyCount = Chargify.GetProductFamilyList().Count;
            var productCount = Chargify.GetProductList().Count;
            var subscriptionCount = Chargify.GetSubscriptionList().Count;
            var customerCount = Chargify.GetCustomerList().Count;

            // Act
            var result = Chargify.ClearTestSite(SiteCleanupScope.Customers);
            var newProductFamilyCount = Chargify.GetProductFamilyList().Count;
            var newProductCount = Chargify.GetProductList().Count;
            var newSubscriptionCount = Chargify.GetSubscriptionList().Count;
            var newCustomerCount = Chargify.GetCustomerList().Count;

            // Assert
            //Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue(result);
            Assert.AreEqual(productFamilyCount, newProductFamilyCount);
            Assert.AreEqual(productCount, newProductCount);
            Assert.AreNotEqual(subscriptionCount, newSubscriptionCount);
            Assert.AreNotEqual(customerCount, newCustomerCount);
            Assert.AreNotEqual(0, newProductFamilyCount);
            Assert.AreNotEqual(0, newProductCount);
            Assert.AreEqual(0, newSubscriptionCount);
            Assert.AreEqual(0, newCustomerCount);
        }

        [TestMethod, Ignore]
        public void Sites_Default_Clear_Clears_Customers()
        {
            // Arrange
            var productFamilyCount = Chargify.GetProductFamilyList().Count;
            var productCount = Chargify.GetProductList().Count;
            var subscriptionCount = Chargify.GetSubscriptionList().Count;
            var customerCount = Chargify.GetCustomerList().Count;

            // Act
            var result = Chargify.ClearTestSite();
            var newProductFamilyCount = Chargify.GetProductFamilyList().Count;
            var newProductCount = Chargify.GetProductList().Count;
            var newSubscriptionCount = Chargify.GetSubscriptionList().Count;
            var newCustomerCount = Chargify.GetCustomerList().Count;

            // Assert
            //Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue(result);
            Assert.AreEqual(productFamilyCount, newProductFamilyCount);
            Assert.AreEqual(productCount, newProductCount);
            Assert.AreNotEqual(subscriptionCount, newSubscriptionCount);
            Assert.AreNotEqual(customerCount, newCustomerCount);
            Assert.AreNotEqual(0, newProductFamilyCount);
            Assert.AreNotEqual(0, newProductCount);
            Assert.AreEqual(0, newSubscriptionCount);
            Assert.AreEqual(0, newCustomerCount);
        }

        private CreditCardAttributes GetTestPaymentMethod(CustomerAttributes customer)
        {
            var retVal = new CreditCardAttributes()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                ExpirationMonth = 1,
                ExpirationYear = 2020,
                FullNumber = "1",
                CVV = "123",
                BillingAddress = Faker.Address.StreetAddress(false),
                BillingCity = Faker.Address.City(),
                BillingCountry = "US",
                BillingState = Faker.Address.StateAbbr(),
                BillingZip = Faker.Address.ZipCode()
            };
            return retVal;
        }
    }
}
