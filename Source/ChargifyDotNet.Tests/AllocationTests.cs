using System;
using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class AllocationTests : ChargifyTestBase
    {
        [TestMethod]
        public void ComponentAllocation_Can_Get_List()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && Chargify.GetComponentsForSubscription(s.Key) != null).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var component = Chargify.GetComponentsForSubscription(subscription.SubscriptionID).FirstOrDefault(c => (c.Value.Kind == "quantity_based_component" || c.Value.Kind == "on_off_component") && c.Value.AllocatedQuantity > 0).Value;
            if (component == null) Assert.Inconclusive("A valid component could not be found.");

            // Act
            var result = Chargify.GetAllocationListForSubscriptionComponent(subscription.SubscriptionID, component.ComponentID);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Dictionary<int, List<IComponentAllocation>>));
            Assert.IsTrue(result.Values.Count > 0, "There is no allocation history");
        }

        [TestMethod]
        public void ComponentAllocation_Can_Create_Using_Object()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var component = Chargify.GetComponentsForSubscription(subscription.SubscriptionID).FirstOrDefault(c => c.Value.Kind == "quantity_based_component" || c.Value.Kind == "on_off_component").Value;
            if (component == null) Assert.Inconclusive("A valid component could not be found.");

            var allocation = new ComponentAllocation() {
                Quantity = 1,
                Memo = Guid.NewGuid().ToString(),
                UpgradeScheme = ComponentUpgradeProrationScheme.Prorate_Delay_Capture,
                DowngradeScheme = ComponentDowngradeProrationScheme.No_Prorate
            };

            // Act
            var result = Chargify.CreateComponentAllocation(subscription.SubscriptionID, component.ComponentID, allocation);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IComponentAllocation));
            Assert.AreEqual(allocation.Quantity, result.Quantity, "The quantities don't match");
            Assert.AreEqual(allocation.Memo, result.Memo, "The memo text differs");
            Assert.AreEqual(allocation.UpgradeScheme, result.UpgradeScheme, "The upgrade scheme received isn't the same as submitted");
            Assert.AreEqual(allocation.DowngradeScheme, result.DowngradeScheme, "The downgrade scheme received isn't the same as submitted");
        }

        [TestMethod]
        public void ComponentAllocation_Can_Create_Using_Quantity_Only()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && Chargify.GetComponentsForSubscription(s.Key) != null).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var component = Chargify.GetComponentsForSubscription(subscription.SubscriptionID).FirstOrDefault(c => c.Value.Kind == "quantity_based_component" && c.Value.AllocatedQuantity > 0).Value; // || c.Value.Kind == "on_off_component"
            if (component == null) Assert.Inconclusive("A valid component could not be found.");

            int quantityToAllocate = (int)component.AllocatedQuantity+1;
            IComponentAllocation result = null;

            // Act
            try
            {
                result = Chargify.CreateComponentAllocation(subscription.SubscriptionID, component.ComponentID, quantityToAllocate);
            }
            catch (ChargifyException cEx)
            {
                Assert.Fail(cEx.ToString());
            }

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IComponentAllocation));
            Assert.AreEqual(quantityToAllocate, result.Quantity);
            Assert.AreEqual(string.Empty, result.Memo);
            // Can't really tell the following, but the default for a site with no changes is to not prorate.
            Assert.AreEqual(ComponentUpgradeProrationScheme.No_Prorate, result.UpgradeScheme);
            Assert.AreEqual(ComponentDowngradeProrationScheme.No_Prorate, result.DowngradeScheme);
        }
    }
}
