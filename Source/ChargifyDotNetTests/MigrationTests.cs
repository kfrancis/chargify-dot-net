using System;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChargifyDotNetTests.Base;
using System.Linq;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class MigrationTests : ChargifyTestBase
    {
        [TestMethod]
        public void Migration_Preview()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;

            // Act
            var migrationResult = Chargify.PreviewMigrateSubscriptionProduct(subscription.SubscriptionID, 1302);

            // Assert
            Assert.IsNotNull(migrationResult);
            Assert.IsInstanceOfType(migrationResult, typeof(Migration));
        }
    }
}
