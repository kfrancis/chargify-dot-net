using ChargifyDotNetTests.Base;
using System.Linq;
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
    public class PortalTests : ChargifyTestBase
    {
        [Test]
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
