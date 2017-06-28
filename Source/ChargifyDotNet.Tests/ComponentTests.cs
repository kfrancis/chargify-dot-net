using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
