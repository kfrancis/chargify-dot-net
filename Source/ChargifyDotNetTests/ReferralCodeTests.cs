using System;
using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyNET;
#if NUNIT
using NUnit.Framework;
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
    public class ReferralCodeTests : ChargifyTestBase
    {
        [Test]
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
