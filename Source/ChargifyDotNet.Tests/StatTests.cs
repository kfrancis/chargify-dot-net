using ChargifyDotNetTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class StatTests : ChargifyTestBase
    {
        [TestMethod]
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
