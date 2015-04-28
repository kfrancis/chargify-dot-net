using System;
using System.Collections.Generic;
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
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            var component = Chargify.GetComponentsForSubscription(subscription.SubscriptionID).FirstOrDefault(c => (c.Value.Kind == "quantity_based_component" || c.Value.Kind == "on_off_component") && c.Value.AllocatedQuantity > 0).Value;

            // Act
            var result = Chargify.GetAllocationListForSubscriptionComponent(subscription.SubscriptionID, component.ComponentID);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Dictionary<int, List<IComponentAllocation>>));
            Assert.IsTrue(result.Values.Count > 0, "There is no allocation history");
        }

        [TestMethod]
        public void ComponentAllocation_Can_Create_Using_Object()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            var component = Chargify.GetComponentsForSubscription(subscription.SubscriptionID).FirstOrDefault(c => c.Value.Kind == "quantity_based_component" || c.Value.Kind == "on_off_component").Value;

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
            Assert.IsInstanceOfType(result, typeof(IComponentAllocation));
            Assert.AreEqual(allocation.Quantity, result.Quantity, "The quantities don't match");
            Assert.AreEqual(allocation.Memo, result.Memo, "The memo text differs");
            Assert.AreEqual(allocation.UpgradeScheme, result.UpgradeScheme, "The upgrade scheme received isn't the same as submitted");
            Assert.AreEqual(allocation.DowngradeScheme, result.DowngradeScheme, "The downgrade scheme received isn't the same as submitted");
        }

        [TestMethod]
        public void ComponentAllocation_Can_Create_Using_Quantity_Only()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            var component = Chargify.GetComponentsForSubscription(subscription.SubscriptionID).FirstOrDefault(c => c.Value.Kind == "quantity_based_component").Value; // || c.Value.Kind == "on_off_component"

            int quantityToAllocate = 1;
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
            Assert.IsInstanceOfType(result, typeof(IComponentAllocation));
            Assert.AreEqual(quantityToAllocate, result.Quantity);
            Assert.AreEqual(string.Empty, result.Memo);
            // Can't really tell the following, but the default for a site with no changes is to not prorate.
            Assert.AreEqual(ComponentUpgradeProrationScheme.No_Prorate, result.UpgradeScheme);
            Assert.AreEqual(ComponentDowngradeProrationScheme.No_Prorate, result.DowngradeScheme);
        }
    }
}
