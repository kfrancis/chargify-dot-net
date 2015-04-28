using System;
using System.Linq;
using ChargifyDotNetTests.Base;
#if NUNIT
using NUnit.Framework;
using System.Diagnostics;
#else
using TestFixture = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using TestFixtureSetUp = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using SetUp = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace ChargifyDotNetTests
{
    [TestFixture]
    public class AdjustmentTests : ChargifyTestBase
    {
        [Test]
        public void Adjustment_Can_Adjust_Zero_Decimal()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            decimal amount = 0m;
            string memo = "test kf";
            var preAdjustmentBalance = subscription.Balance;

            // Act
            var result = Chargify.CreateAdjustment(subscription.SubscriptionID, amount, memo);
            var postAdjustmentSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(amount, result.Amount);
            Assert.AreEqual(Convert.ToInt32(amount), result.AmountInCents);
            Assert.AreEqual(memo, result.Memo);
            Assert.AreEqual(preAdjustmentBalance+amount, postAdjustmentSubscription.Balance);

#if !NUNIT
            TestContext.WriteLine("SubscriptionID: {0}", subscription.SubscriptionID);
#else
            Trace.WriteLine(string.Format("SubscriptionID: {0}", subscription.SubscriptionID));
#endif
        }

        [Test]
        public void Adjustment_Can_Adjust_Zero_Integer()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            int amount = 0;
            string memo = "test kf";
            var preAdjustmentBalance = subscription.BalanceInCents;

            // Act
            var result = Chargify.CreateAdjustment(subscription.SubscriptionID, amount, memo);
            var postAdjustmentSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(amount, result.Amount);
            Assert.AreEqual(Convert.ToInt32(amount), result.AmountInCents);
            Assert.AreEqual(memo, result.Memo);
            Assert.AreEqual(preAdjustmentBalance+amount, postAdjustmentSubscription.BalanceInCents);

#if !NUNIT
            TestContext.WriteLine("SubscriptionID: {0}", subscription.SubscriptionID);
#else
            Trace.WriteLine(string.Format("SubscriptionID: {0}", subscription.SubscriptionID));
#endif
        }
    }
}
