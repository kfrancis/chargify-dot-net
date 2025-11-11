using ChargifyDotNet.Tests;
using ChargifyDotNetTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class StatTests : ChargifyTestBase
    {
        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Stats_Load(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Act
            var stats = Chargify.GetSiteStatistics();
            var allSubscriptions = Chargify.GetSubscriptionList();

            // Assert
            Assert.IsNotNull(stats);
            //Assert.IsInstanceOfType(stats, typeof(ChargifyNET.SiteStatistics));
            Assert.AreEqual(allSubscriptions.Count, stats.TotalSubscriptions);

            SetJson(!isJson);
        }
    }
}
