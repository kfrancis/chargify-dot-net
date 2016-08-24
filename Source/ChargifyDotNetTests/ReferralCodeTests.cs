using System;
using System.Linq;
using ChargifyDotNetTests.Base;
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
            var referralCode = "pk2z97";

            // Act
            var result = Chargify.ValidateReferralCode(referralCode);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IReferralCode));

            Assert.IsTrue(result.ID != int.MinValue);
            Assert.IsTrue(result.SiteID != int.MinValue);
            Assert.IsTrue(result.SubscriptionID != int.MinValue);
            Assert.IsTrue(result.Code == referralCode);
        }
    }
}
