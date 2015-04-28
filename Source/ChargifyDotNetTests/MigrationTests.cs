using System;
using ChargifyNET;
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
    public class MigrationTests : ChargifyTestBase
    {
        [Test]
        public void Migration_Preview()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;

            // Act
            var migrationResult = Chargify.PreviewMigrateSubscriptionProduct(subscription.SubscriptionID, 1302);

            // Assert
            Assert.IsNotNull(migrationResult);            
            //Assert.IsInstanceOfType(migrationResult, typeof(Migration));
        }
    }
}
