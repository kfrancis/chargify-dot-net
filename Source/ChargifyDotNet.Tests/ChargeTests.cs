﻿using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class ChargeTests : ChargifyTestBase
    {
        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Charges_Can_Charge_With_Tax(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile.MaskedCardNumber.EndsWith("1") && s.Value.Balance == 0m).Value as Subscription;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var chargeOptions = new ChargeCreateOptions() { Amount = 1.23m, Taxable = true, Memo = Guid.NewGuid().ToString() };

            // Act
            var result = Chargify.CreateCharge(subscription.SubscriptionID, chargeOptions);

            // Assert
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(chargeOptions.Amount, result.Amount);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Charges_Can_Charge_Successfully(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var client = Chargify;
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile.MaskedCardNumber.EndsWith("1") && s.Value.Balance == 0m).Value as Subscription;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");

            // Act
            var balanceResult = false;
            if (subscription.BalanceInCents > 0)
            {
                balanceResult = client.ResetSubscriptionBalance(subscription.SubscriptionID);
            }
            var result = client.CreateCharge(subscription.SubscriptionID, 1.00m, "Test Charge");
            var retrievedSubscription = client.Find<Subscription>(subscription.SubscriptionID);
            TestContext.WriteLine($"{retrievedSubscription.SubscriptionID}");

            // Assert
            if (subscription.BalanceInCents > 0)
            {
                Assert.IsNotNull(balanceResult);
                Assert.IsTrue(balanceResult == true);
            }
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Charge));
            Assert.IsTrue(result.SubscriptionID == subscription.SubscriptionID);
            Assert.IsTrue(result.ProductID == subscription.Product.ID);
            Assert.IsTrue(result.Kind == "one_time");
            Assert.IsTrue(result.ChargeType == "Charge");
            Assert.IsTrue(result.TransactionType == "charge");
            Assert.IsTrue(result.PaymentID != int.MinValue);
            Assert.IsTrue(result.ID != int.MinValue);
            Assert.IsTrue(result.Success == true);
            Assert.IsTrue(retrievedSubscription.BalanceInCents == 0, "Expected $0, returned {0:C2}", retrievedSubscription.Balance);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Charges_Can_Charge_And_Use_Negative_Balance(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var client = Chargify;
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var amountToCharge = 1.00m;

            // Act
            var balanceResult = false;
            if (subscription.BalanceInCents > 0)
            {
                balanceResult = client.ResetSubscriptionBalance(subscription.SubscriptionID);
            }
            var adjustmentResult = client.CreateAdjustment(subscription.SubscriptionID, amountToCharge, "Credit before charge");
            var result = client.CreateCharge(subscription.SubscriptionID, amountToCharge, "Test Charge", useNegativeBalance: true);
            //var retrievedSubscription = client.Find<Subscription>(subscription.SubscriptionID);

            // Assert
            if (subscription.BalanceInCents > 0)
            {
                Assert.IsNotNull(balanceResult);
                Assert.IsTrue(balanceResult == true);
            }
            Assert.IsNotNull(adjustmentResult);
            //Assert.IsInstanceOfType(adjustmentResult, typeof(Adjustment));
            Assert.IsTrue(adjustmentResult.Amount == amountToCharge);
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Charge));
            Assert.IsTrue(result.SubscriptionID == subscription.SubscriptionID);
            Assert.IsTrue(result.ProductID == subscription.Product.ID);
            Assert.IsTrue(result.Kind == "one_time");
            Assert.IsTrue(result.ChargeType == "Charge");
            Assert.IsTrue(result.TransactionType == "charge");
            //Assert.IsTrue(result.EndingBalance == amountToCharge);  // TODO: Test currently fails here, bug in Chargify.
            //Assert.IsTrue(result.PaymentID.HasValue == false);
            //Assert.IsTrue(result.ID != int.MinValue);
            //Assert.IsTrue(result.Success == true);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Charges_Can_Charge_With_Delay(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var client = Chargify;
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var amountToCharge = 1.00m;

            // Act
            var balanceResult = false;
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
            //Assert.IsInstanceOfType(result, typeof(Charge));
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

            SetJson(!isJson);
        }
    }
}
