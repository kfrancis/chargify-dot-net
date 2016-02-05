using System;
using System.Collections.Generic;
using System.Linq;
using ChargifyDotNetTests.Base;
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
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class PaymentProfileTests : ChargifyTestBase
    {
        public PaymentProfileTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [SetUp]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Test]
        public void PaymentProfile_CanUpdate()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active && s.Value.PaymentProfile != null).Value;
            var loadedSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);
            TestContext.WriteLine("ID: " + loadedSubscription.PaymentProfile.Id);
            var originalPaymentProfile = loadedSubscription.PaymentProfile as PaymentProfileView;
            originalPaymentProfile.FirstName = Guid.NewGuid().ToString();
            originalPaymentProfile.LastName = Guid.NewGuid().ToString();

            // Act
            var result = Chargify.UpdatePaymentProfile(originalPaymentProfile);

            // Arrange
            Assert.IsNotNull(result);
            Assert.AreEqual(originalPaymentProfile.FirstName, result.FirstName);
            Assert.AreEqual(originalPaymentProfile.LastName, result.LastName);
        }

        [Test]
        public void PaymentProfile_GetSinglePaymentProfileByID()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active && s.Value.PaymentProfile != null).Value;
            var loadedSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);
            TestContext.WriteLine("ID: " + loadedSubscription.PaymentProfile.Id);

            // Act
            var result = Chargify.LoadPaymentProfile(loadedSubscription.PaymentProfile.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(loadedSubscription.PaymentProfile.Id, result.Id);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BillingAddress, result.BillingAddress);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BillingAddress2, result.BillingAddress2);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BillingCity, result.BillingCity);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BillingCountry, result.BillingCountry);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BillingState, result.BillingState);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BillingZip, result.BillingZip);
            Assert.AreEqual(loadedSubscription.PaymentProfile.CardType, result.CardType);
            Assert.AreEqual(loadedSubscription.PaymentProfile.ExpirationMonth, result.ExpirationMonth);
            Assert.AreEqual(loadedSubscription.PaymentProfile.ExpirationYear, result.ExpirationYear);
            Assert.AreEqual(loadedSubscription.PaymentProfile.FirstName, result.FirstName);
            Assert.AreEqual(loadedSubscription.PaymentProfile.FullNumber, result.FullNumber);
            Assert.AreEqual(loadedSubscription.PaymentProfile.LastName, result.LastName);
            Assert.AreEqual(loadedSubscription.PaymentProfile.MaskedBankAccountNumber, result.MaskedBankAccountNumber);
            Assert.AreEqual(loadedSubscription.PaymentProfile.MaskedBankRoutingNumber, result.MaskedBankRoutingNumber);
            Assert.AreEqual(loadedSubscription.PaymentProfile.PaymentMethodNonce, result.PaymentMethodNonce);
            Assert.AreEqual(loadedSubscription.PaymentProfile.PayPalEmail, result.PayPalEmail);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BankAccountHolderType, result.BankAccountHolderType);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BankAccountNumber, result.BankAccountNumber);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BankAccountType, result.BankAccountType);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BankName, result.BankName);
            Assert.AreEqual(loadedSubscription.PaymentProfile.BankRoutingNumber, result.BankRoutingNumber);
        }
    }
}
