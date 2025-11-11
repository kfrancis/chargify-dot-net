using System;
using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyDotNet.Tests;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class MigrationTests : ChargifyTestBase
    {
        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Migration_Preview(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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

            SetJson(!isJson);
        }
    }
}
