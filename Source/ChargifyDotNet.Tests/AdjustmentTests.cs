using System;
using System.Linq;
using ChargifyDotNet.Tests;
using ChargifyDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class AdjustmentTests : ChargifyTestBase
    {
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Adjustment_Can_Adjust_Zero_Decimal(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            ISubscription subscription;
            using (Step("Get active subscription"))
            {
                subscription = Chargify.GetSubscriptionList(SubscriptionState.Active)
                    .FirstOrDefault().Value;
            }
            if (subscription == null)
            {
                Assert.Inconclusive("No subscription found.");
            }

            var amount = 0m;
            var memo = "test kf";
            var preAdjustmentBalance = subscription.Balance;

            // Act
            IAdjustment result;
            using (Step("Create adjustment"))
            {
                result = Chargify.CreateAdjustment(subscription.SubscriptionID, amount, memo);
            }
            ISubscription postAdjustmentSubscription;
            using (Step("Reload subscription"))
            {
                postAdjustmentSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);
            }

            // Assert
            using (Step("Assertions"))
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(amount, result.Amount);
                Assert.AreEqual(Convert.ToInt32(amount), result.AmountInCents);
                result.Memo.ShouldBe(memo);
                Assert.AreEqual(preAdjustmentBalance + amount, postAdjustmentSubscription.Balance);
            }

            SetJson(!isJson);

            Log("SubscriptionID: {0}", subscription.SubscriptionID);
        }

        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Adjustment_Can_Adjust_Zero_Integer(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            ISubscription subscription;
            using (Step("Get active subscription"))
            {
                subscription = Chargify.GetSubscriptionList()
                    .FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            }
            if (subscription == null)
            {
                Assert.Inconclusive("No subscription found.");
            }

            var amount = 0;
            var memo = "test kf";
            var preAdjustmentBalance = subscription.BalanceInCents;

            // Act
            IAdjustment result;
            using (Step("Create adjustment"))
            {
                result = Chargify.CreateAdjustment(subscription.SubscriptionID, amount, memo);
            }
            ISubscription postAdjustmentSubscription;
            using (Step("Reload subscription"))
            {
                postAdjustmentSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);
            }

            // Assert
            using (Step("Assertions"))
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(amount, result.Amount);
                Assert.AreEqual(Convert.ToInt32(amount), result.AmountInCents);
                Assert.AreEqual(memo, result.Memo);
                Assert.AreEqual(preAdjustmentBalance + amount, postAdjustmentSubscription.BalanceInCents);
            }

            SetJson(!isJson);

            Log("SubscriptionID: {0}", subscription.SubscriptionID);
        }
    }
}
