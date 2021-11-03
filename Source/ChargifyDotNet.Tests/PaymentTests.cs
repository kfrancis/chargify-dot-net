using System;
using ChargifyDotNetTests.Base;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class PaymentTests : ChargifyTestBase
    {
        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Payment_Create(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            int amount = 1234; //$12.34
            string memo = Guid.NewGuid().ToString();
            var prePaymentBalance = subscription.BalanceInCents;

            // Act
            var result = Chargify.AddPayment(subscription.SubscriptionID, amount, memo);
            var postPaymentSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(amount, result.AmountInCents);
            Assert.AreEqual(memo, result.Memo);
            TestContext.WriteLine("SubscriptionID: {0}", subscription.SubscriptionID);

            SetJson(!isJson);
        }
    }
}
