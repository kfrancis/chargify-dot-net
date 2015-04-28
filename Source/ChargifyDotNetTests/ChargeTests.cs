using System;
using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class ChargeTests : ChargifyTestBase
    {
        [TestMethod]
        public void Charges_Can_Charge_Successfully()
        {
            // Arrange
            var client = this.Chargify;
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;

            // Act
            bool balanceResult = false;
            if (subscription.BalanceInCents > 0)
            {
                balanceResult = client.ResetSubscriptionBalance(subscription.SubscriptionID);
            }
            var result = client.CreateCharge(subscription.SubscriptionID, 1.00m, "Test Charge");
            var retrievedSubscription = client.Find<Subscription>(subscription.SubscriptionID);

            // Assert
            if (subscription.BalanceInCents > 0) {
                Assert.IsNotNull(balanceResult);
                Assert.IsTrue(balanceResult == true);
            }
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Charge));
            Assert.IsTrue(result.SubscriptionID == subscription.SubscriptionID);
            Assert.IsTrue(result.ProductID == subscription.Product.ID);
            Assert.IsTrue(result.Kind == "one_time");
            Assert.IsTrue(result.ChargeType == "Charge");
            Assert.IsTrue(result.TransactionType == "charge");
            Assert.IsTrue(result.PaymentID != int.MinValue);
            Assert.IsTrue(result.ID != int.MinValue);
            Assert.IsTrue(result.Success == true);
            Assert.IsTrue(retrievedSubscription.BalanceInCents == 0, "Expected $0, returned {0:C2}", retrievedSubscription.Balance);
        }

        [TestMethod]
        public void Charges_Can_Charge_And_Use_Negative_Balance()
        {
            // Arrange
            var client = this.Chargify;
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            var amountToCharge = 1.00m;

            // Act
            bool balanceResult = false;
            if (subscription.BalanceInCents > 0)
            {
                balanceResult = client.ResetSubscriptionBalance(subscription.SubscriptionID);
            }
            var adjustmentResult = client.CreateAdjustment(subscription.SubscriptionID,amountToCharge, "Credit before charge");
            var result = client.CreateCharge(subscription.SubscriptionID, amountToCharge, "Test Charge", useNegativeBalance: true);
            //var retrievedSubscription = client.Find<Subscription>(subscription.SubscriptionID);

            // Assert
            if (subscription.BalanceInCents > 0)
            {
                Assert.IsNotNull(balanceResult);
                Assert.IsTrue(balanceResult == true);
            }
            Assert.IsNotNull(adjustmentResult);
            Assert.IsInstanceOfType(adjustmentResult, typeof(Adjustment));
            Assert.IsTrue(adjustmentResult.Amount == amountToCharge);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Charge));
            Assert.IsTrue(result.SubscriptionID == subscription.SubscriptionID);
            Assert.IsTrue(result.ProductID == subscription.Product.ID);
            Assert.IsTrue(result.Kind == "one_time");
            Assert.IsTrue(result.ChargeType == "Charge");
            Assert.IsTrue(result.TransactionType == "charge");
            //Assert.IsTrue(result.EndingBalance == amountToCharge);  // TODO: Test currently fails here, bug in Chargify.
            //Assert.IsTrue(result.PaymentID.HasValue == false);
            //Assert.IsTrue(result.ID != int.MinValue);
            //Assert.IsTrue(result.Success == true);
        }

        [TestMethod]
        public void Charges_Can_Charge_With_Delay()
        {
            // Arrange
            var client = this.Chargify;
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            var amountToCharge = 1.00m;

            // Act
            bool balanceResult = false;
            if (subscription.BalanceInCents > 0)
            {
                balanceResult = client.ResetSubscriptionBalance(subscription.SubscriptionID);
            }
            var result = client.CreateCharge(subscription.SubscriptionID, amountToCharge, "Test Charge", delayCharge: true);
            var retrievedSubscription = client.Find<Subscription>(subscription.SubscriptionID);

            // Assert
            if (subscription.BalanceInCents > 0)
            {
                Assert.IsNotNull(balanceResult);
                Assert.IsTrue(balanceResult == true);
            }
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Charge));
            Assert.IsTrue(result.SubscriptionID == subscription.SubscriptionID);
            Assert.IsTrue(result.ProductID == subscription.Product.ID);
            Assert.IsTrue(result.Amount == amountToCharge);
            Assert.IsTrue(result.Kind == "delay_capture");
            Assert.IsTrue(result.ChargeType == "Charge");
            Assert.IsTrue(result.TransactionType == "charge");
            Assert.IsTrue(result.PaymentID.HasValue == false);
            Assert.IsTrue(result.ID != int.MinValue);
            Assert.IsTrue(result.Success == true);
            Assert.IsTrue(result.EndingBalance == amountToCharge, "Expected {0:C2}, received {1:C2}", amountToCharge, result.EndingBalance);
            Assert.IsTrue(retrievedSubscription.Balance == amountToCharge, "Expected {0:C2}, balance is {1:C2}", amountToCharge, retrievedSubscription.Balance);
        }
    }
}
