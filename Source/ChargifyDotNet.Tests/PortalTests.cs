using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyDotNet.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class PortalTests : ChargifyTestBase
    {
        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Portal_Can_Retrieve_Management_Link(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var customer = Chargify.GetCustomerList().Values.FirstOrDefault(c => c.ChargifyID != int.MinValue);
            Assert.IsNotNull(customer, "No customer found");
            var customerId = customer.ChargifyID;

            // Act
            var result = Chargify.GetManagementLink(customerId);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(BillingManagementInfo));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.URL));
            Assert.IsGreaterThanOrEqualTo(1, result.FetchCount);

            SetJson(!isJson);
        }
    }
}
