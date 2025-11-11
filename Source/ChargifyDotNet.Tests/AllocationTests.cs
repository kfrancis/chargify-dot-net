using System;
using System.Linq;
using ChargifyDotNet.Tests;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class AllocationTests : ChargifyTestBase
    {

        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void ComponentAllocation_Can_Get_List(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).FirstOrDefault(s => Chargify.GetComponentsForSubscription(s.Key).Any()).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var component = Chargify.GetComponentsForSubscription(subscription.SubscriptionID).FirstOrDefault(c => (c.Value.Kind == "quantity_based_component" || c.Value.Kind == "on_off_component") && c.Value.AllocatedQuantity > 0).Value;
            if (component == null) Assert.Inconclusive("A valid component could not be found.");

            // Act
            var result = Chargify.GetAllocationListForSubscriptionComponent(subscription.SubscriptionID, component.ComponentID);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Dictionary<int, List<IComponentAllocation>>));
            Assert.IsNotEmpty(result.Values, "There is no allocation history");

            SetJson(!isJson);
        }


        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void ComponentAllocation_Can_Create_Using_Object(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            using var s1 = Step("Get active subscription");
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).FirstOrDefault().Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            s1.Complete();

            using var s2 = Step("Get a valid component");
            var component = Chargify.GetComponentsForSubscription(subscription.SubscriptionID).FirstOrDefault(c => c.Value.Kind == "quantity_based_component" || c.Value.Kind == "on_off_component").Value;
            if (component == null) Assert.Inconclusive("A valid component could not be found.");
            s2.Complete();

            using var s3 = Step("Create an allocation");
            var allocation = new ComponentAllocation()
            {
                Quantity = 1,
                Memo = Guid.NewGuid().ToString(),
                UpgradeScheme = ComponentUpgradeProrationScheme.Prorate_Delay_Capture,
                DowngradeScheme = ComponentDowngradeProrationScheme.No_Prorate
            };
            s3.Complete();

            // Act
            using var s4 = Step("Send new allocation");
            var result = Chargify.CreateComponentAllocation(subscription.SubscriptionID, component.ComponentID, allocation);

            // Assert
            using (Step("Assertions"))
            {
                Assert.IsNotNull(result);
                //Assert.IsInstanceOfType(result, typeof(IComponentAllocation));
                Assert.AreEqual(allocation.Quantity, result.Quantity, "The quantities don't match");
                Assert.AreEqual(allocation.Memo, result.Memo, "The memo text differs");
                Assert.AreEqual(allocation.UpgradeScheme, result.UpgradeScheme, "The upgrade scheme received isn't the same as submitted");
                Assert.AreEqual(allocation.DowngradeScheme, result.DowngradeScheme, "The downgrade scheme received isn't the same as submitted");
            }

            SetJson(!isJson);
        }


        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void ComponentAllocation_Can_Create_Using_Quantity_Only(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).FirstOrDefault(s => Chargify.GetComponentsForSubscription(s.Key) != null).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var component = Chargify.GetComponentsForSubscription(subscription.SubscriptionID).FirstOrDefault(c => c.Value.Kind == "quantity_based_component" && c.Value.AllocatedQuantity > 0).Value; // || c.Value.Kind == "on_off_component"
            if (component == null) Assert.Inconclusive("A valid component could not be found.");

            var quantityToAllocate = (int)component.AllocatedQuantity + 1;
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

            SetJson(!isJson);
        }
    }
}
