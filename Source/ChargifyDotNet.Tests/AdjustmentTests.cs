using System;
using System.Linq;
using ChargifyDotNetTests.Base;
using System.Diagnostics;
using ChargifyDotNet.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class AdjustmentTests : ChargifyTestBase
    {

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Adjustment_Can_Adjust_Zero_Decimal(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            if (subscription == null) Assert.Inconclusive("No subscription found.");
            var amount = 0m;
            var memo = "test kf";
            var preAdjustmentBalance = subscription.Balance;

            // Act
            var result = Chargify.CreateAdjustment(subscription.SubscriptionID, amount, memo);
            var postAdjustmentSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(amount, result.Amount);
            Assert.AreEqual(Convert.ToInt32(amount), result.AmountInCents);
            result.Memo.ShouldBe(memo);
            Assert.AreEqual(preAdjustmentBalance + amount, postAdjustmentSubscription.Balance);

            SetJson(!isJson);

#if !NUNIT
            TestContext.WriteLine("SubscriptionID: {0}", subscription.SubscriptionID);
#else
            Trace.WriteLine(string.Format("SubscriptionID: {0}", subscription.SubscriptionID));
#endif
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Adjustment_Can_Adjust_Zero_Integer(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            if (subscription == null) Assert.Inconclusive("No subscription found.");
            var amount = 0;
            var memo = "test kf";
            var preAdjustmentBalance = subscription.BalanceInCents;

            // Act
            var result = Chargify.CreateAdjustment(subscription.SubscriptionID, amount, memo);
            var postAdjustmentSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(amount, result.Amount);
            Assert.AreEqual(Convert.ToInt32(amount), result.AmountInCents);
            Assert.AreEqual(memo, result.Memo);
            Assert.AreEqual(preAdjustmentBalance + amount, postAdjustmentSubscription.BalanceInCents);

            SetJson(!isJson);

#if !NUNIT
            TestContext.WriteLine("SubscriptionID: {0}", subscription.SubscriptionID);
#else
            Trace.WriteLine(string.Format("SubscriptionID: {0}", subscription.SubscriptionID));
#endif
        }
    }
}
