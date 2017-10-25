using System;
using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class ReferralCodeTests : ChargifyTestBase
    {
        [TestMethod]
        public void ReferralCode_Validate()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            Assert.IsNotNull(subscription, "No suitable subscription found.");

            // Act
            var result = Chargify.ValidateReferralCode(subscription.ReferralCode);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(subscription.ReferralCode, result.Code);
            Assert.AreEqual(subscription.SubscriptionID, result.SubscriptionID);
        }
    }
}
