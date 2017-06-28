using System;
using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class SubscriptionOverrideTests : ChargifyTestBase
    {
        [TestMethod]
        public void SubOverride_Can_Override()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            DateTime now = DateTime.Now;
            DateTime activatedAt = now.AddDays(-4);
            DateTime canceledAt = now;
            DateTime expiresAt = now.AddDays(4);
            string cancellationMessage = Guid.NewGuid().ToString();

            // Act
            bool result = Chargify.SetSubscriptionOverride(subscription.SubscriptionID, activatedAt, canceledAt, cancellationMessage, expiresAt);
            var retrievedSubscription = Chargify.Find<Subscription>(subscription.SubscriptionID);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(IsDateTimeEqual(activatedAt, retrievedSubscription.ActivatedAt));
            Assert.IsFalse(IsDateTimeEqual(subscription.ActivatedAt, retrievedSubscription.ActivatedAt));
            Assert.IsTrue(IsDateTimeEqual(canceledAt, retrievedSubscription.CanceledAt));
            Assert.IsFalse(IsDateTimeEqual(subscription.CanceledAt, retrievedSubscription.CanceledAt));
            Assert.AreEqual(cancellationMessage, retrievedSubscription.CancellationMessage);
            Assert.AreNotEqual(subscription.CancellationMessage, retrievedSubscription.CancellationMessage);
            Assert.IsTrue(IsDateTimeEqual(expiresAt, retrievedSubscription.ExpiresAt));
            Assert.IsFalse(IsDateTimeEqual(subscription.ExpiresAt, retrievedSubscription.ExpiresAt));
        }

        public static bool IsDateTimeEqual(DateTime expected, DateTime actual)
        {
            return ((expected.Year == actual.Year)
            && (expected.Month == actual.Month)
            && (expected.Day == actual.Day)
            && (expected.Hour == actual.Hour)
            && (expected.Minute == actual.Minute)
            && (expected.Second == actual.Second));
        }
    }
}
