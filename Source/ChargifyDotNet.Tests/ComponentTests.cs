using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class ComponentTests : ChargifyTestBase
    {
        [TestMethod]
        public void Components_Can_Load_PriceBrackets()
        {
            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            var familyComponents = Chargify.GetComponentsForProductFamily(productFamily.ID, false).Values;
            if (familyComponents == null || !familyComponents.Any()) Assert.Inconclusive("Valid compontents could not be found.");

            // Act
            var components = familyComponents.Where(c => c.PricingScheme != PricingSchemeType.Per_Unit && c.Prices != null && c.Prices.Count > 0).ToList();

            // Assert
            Assert.IsNotNull(components);
            //Assert.IsInstanceOfType(components, typeof(List<IComponentInfo>));
            Assert.IsTrue(components.Where(c => c.Prices != null && c.Prices.Count > 0).Count() > 0);
            Assert.IsTrue(components.FirstOrDefault(c => c.Prices != null && c.Prices.Count > 0).Prices.First().StartingQuantity != int.MinValue);
            Assert.IsTrue(components.FirstOrDefault(c => c.Prices != null && c.Prices.Count > 0).Prices.First().EndingQuantity != int.MinValue);
            Assert.IsTrue(components.FirstOrDefault(c => c.Prices != null && c.Prices.Count > 0).Prices.First().UnitPrice != int.MinValue);
        }


        [TestMethod]
        public void Components_Load_ForSubscription()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");

            // Act
            var results = Chargify.GetComponentsForSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void Components_AddUsage_ForSubscription()
        {
            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var component = Chargify.GetComponentsForProductFamily(productFamily.ID, false).FirstOrDefault(c => c.Value.Kind == ComponentType.Metered_Component).Value;
            if (component == null) Assert.Inconclusive("A valid component could not be found.");
            int usageQuantity = 5;
            string usageDescription = "testing";

            // Act
            var usageResult = Chargify.AddUsage(subscription.SubscriptionID, component.ID, usageQuantity, usageDescription);

            // Assert
            Assert.IsNotNull(usageResult);
            //Assert.IsInstanceOfType(usageResult, typeof(IUsage));
            Assert.IsTrue(usageResult.Memo == usageDescription);
            Assert.IsTrue(usageResult.Quantity == usageQuantity);
        }

        /// <summary>
        /// For @praveen-prakash
        /// </summary>
        [TestMethod]
        public void Components_Create_Subscription_Multiple_Components()
        {
            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            Assert.IsNotNull(product, "Product couldn't be found");
            var referenceId = Guid.NewGuid().ToString();
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", referenceId);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);
            // Find components that allow for a simple allocated_quantity = 1 to work for this simple test
            var components = Chargify.GetComponentsForProductFamily(product.ProductFamily.ID).Values.Where(c => c.Kind == ComponentType.Quantity_Based_Component || c.Kind == ComponentType.On_Off_Component);
            var componentsToUse = components.Take(2).ToList();
            var options = new SubscriptionCreateOptions()
            {
                CustomerAttributes = newCustomer,
                CreditCardAttributes = newPaymentInfo,
                ProductHandle = product.Handle,
                Components = new System.Collections.Generic.List<ComponentDetails>
                {
                    new ComponentDetails() { ComponentID = componentsToUse.First().ID, AllocatedQuantity = 1 },
                    new ComponentDetails() { ComponentID = componentsToUse.Last().ID, AllocatedQuantity = 1 }
                }
            };

            // Act            
            var newSubscription = Chargify.CreateSubscription(options);
            var subComponents = Chargify.GetComponentsForSubscription(newSubscription.SubscriptionID);
            var usedComponents = from c in subComponents
                                 where componentsToUse.Any(x => x.ID == c.Value.ComponentID)
                                 select c;

            // Assert
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNotNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.FirstName == newCustomer.FirstName);
            Assert.IsTrue(newSubscription.Customer.LastName == newCustomer.LastName);
            Assert.IsTrue(newSubscription.Customer.Email == newCustomer.Email);
            Assert.IsTrue(newSubscription.Customer.Organization == newCustomer.Organization);
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceId);
            Assert.IsTrue(newSubscription.PaymentProfile.FirstName == newPaymentInfo.FirstName);
            Assert.IsTrue(newSubscription.PaymentProfile.LastName == newPaymentInfo.LastName);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationMonth == newPaymentInfo.ExpirationMonth);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationYear == newPaymentInfo.ExpirationYear);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress == newPaymentInfo.BillingAddress);
            //Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress2 == newPaymentInfo.BillingAddress2);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCity == newPaymentInfo.BillingCity);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCountry == newPaymentInfo.BillingCountry);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingState == newPaymentInfo.BillingState);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingZip == newPaymentInfo.BillingZip);
            Assert.IsTrue(usedComponents.Count() == componentsToUse.Count);
            foreach (var component in usedComponents)
            {
                Assert.IsTrue(componentsToUse.Any(x => x.ID == component.Key));
                //Assert.AreEqual(decimal.Parse(componentsToUse[component.Key]), component.Value.AllocatedQuantity);
            }

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));
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
                BillingAddress = "123 Main St.",
                BillingCity = "New York",
                BillingCountry = "US",
                BillingState = "New York",
                BillingZip = "10001"
            };
            return retVal;
        }
    }
}
