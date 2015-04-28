using System;
using ChargifyDotNetTests.Base;
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
    public class StatTests : ChargifyTestBase
    {
        [Test]
        public void Stats_Load()
        {
            // Act
            var stats = Chargify.GetSiteStatistics();
            var allSubscriptions = Chargify.GetSubscriptionList();

            // Assert
            Assert.IsNotNull(stats);
            //Assert.IsInstanceOfType(stats, typeof(ChargifyNET.SiteStatistics));
            Assert.IsTrue(stats.TotalSubscriptions == allSubscriptions.Count);
        }
    }
}
