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
    public class MigrationTests : ChargifyTestBase
    {
        [Test]
        public void Migration_Preview()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            var alternateProduct = Chargify.GetProductList().Values.FirstOrDefault(p => p.ID != subscription.Product.ID);
            Assert.IsNotNull(alternateProduct, "No suitable alternative product found.");

            // Act
            try
            {
                var migrationResult = Chargify.PreviewMigrateSubscriptionProduct(subscription.SubscriptionID, alternateProduct.ID);

                // Assert
                Assert.IsNotNull(migrationResult);
                //Assert.IsInstanceOfType(migrationResult, typeof(Migration));
            }
            catch (ChargifyException chEx)
            {
                Assert.Fail(string.Join(", ", chEx.ErrorMessages));
            }

        }
    }
}
