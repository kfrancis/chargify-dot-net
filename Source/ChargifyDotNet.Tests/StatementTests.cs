using System.IO;
using ChargifyDotNetTests.Base;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class StatementTests : ChargifyTestBase
    {
        [TestMethod]
        public void Statements_Can_Get_PDF()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            var statementIDs = Chargify.GetStatementIDs(subscription.SubscriptionID);

            // Act
            var result = Chargify.LoadStatementPDF(statementIDs.FirstOrDefault());

            var tempPath = Path.GetTempPath();
            string filePath = string.Format(@"{0}{1}.pdf", tempPath, statementIDs.FirstOrDefault());
            File.WriteAllBytes(filePath, result);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(byte[]));
            Assert.AreNotEqual(0, result.Length);
            Assert.IsTrue(File.Exists(filePath));

            // Cleanup
            File.Delete(filePath);
            Assert.IsFalse(File.Exists(filePath));
        }
    }
}
