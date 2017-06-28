using ChargifyDotNetTests.Base;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class PortalTests : ChargifyTestBase
    {
        [TestMethod]
        public void Portal_Can_Retrieve_Management_Link()
        {
            // Arrange
            var customerId = Chargify.GetCustomerList().Values.Where(c => c.ChargifyID != int.MinValue).FirstOrDefault().ChargifyID;

            // Act
            var result = Chargify.GetManagementLink(customerId);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(BillingManagementInfo));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.URL));
            Assert.IsTrue(result.FetchCount >= 1);
        }
    }
}
