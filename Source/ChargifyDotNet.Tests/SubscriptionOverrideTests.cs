using System;
using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyDotNet.Tests;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class SubscriptionOverrideTests : ChargifyTestBase
    {
        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void SubOverride_Can_Override(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            var now = DateTime.Now;
            var activatedAt = now.AddDays(-4);
            var canceledAt = now;
            var expiresAt = now.AddDays(4);
            var cancellationMessage = Guid.NewGuid().ToString();

            // Act
            var result = Chargify.SetSubscriptionOverride(subscription.SubscriptionID, activatedAt, canceledAt, cancellationMessage, expiresAt);
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

            SetJson(!isJson);
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
