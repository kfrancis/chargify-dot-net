using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyNET;

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
            var familyComponents = Chargify.GetComponentsForProductFamily(productFamily.ID, false).Values;

            // Act
            var components = familyComponents.Where(c => c.PricingScheme != PricingSchemeType.Per_Unit && c.Prices != null && c.Prices.Count > 0).ToList();

            // Assert
            Assert.IsNotNull(components);
            Assert.IsInstanceOfType(components, typeof(List<IComponentInfo>));
            Assert.IsTrue(components.Where(c => c.Prices != null && c.Prices.Count > 0).Count() > 0);
            Assert.IsTrue(components.FirstOrDefault(c => c.Prices != null && c.Prices.Count > 0).Prices.First().StartingQuantity != int.MinValue);
            Assert.IsTrue(components.FirstOrDefault(c => c.Prices != null && c.Prices.Count > 0).Prices.First().EndingQuantity != int.MinValue);
            Assert.IsTrue(components.FirstOrDefault(c => c.Prices != null && c.Prices.Count > 0).Prices.First().UnitPrice != int.MinValue);
        }


        [TestMethod]
        public void Components_Load_ForSubscription()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;

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
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            var component = Chargify.GetComponentsForProductFamily(productFamily.ID, false).FirstOrDefault(c => c.Value.Kind == ChargifyNET.ComponentType.Metered_Component).Value;
            int usageQuantity = 5;
            string usageDescription = "testing";

            // Act
            var usageResult = Chargify.AddUsage(subscription.SubscriptionID, component.ID, usageQuantity, usageDescription);

            // Assert
            Assert.IsNotNull(usageResult);
            Assert.IsInstanceOfType(usageResult, typeof(IUsage));
            Assert.IsTrue(usageResult.Memo == usageDescription);
            Assert.IsTrue(usageResult.Quantity == usageQuantity);
        }
    }
}
