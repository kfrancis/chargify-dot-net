using System;
using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
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

        [TestMethod]
        public void PaymentProfile_CanUpdate()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value;
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

        [TestMethod]
        public void PaymentProfile_Can_Perform_PartialUpdate()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value;
            var loadedSubscription = Chargify.LoadSubscription(subscription.SubscriptionID);

            var paymentProfile = loadedSubscription.PaymentProfile as PaymentProfileView;
            paymentProfile.BillingAddress = GetNewRandomValue(subscription.PaymentProfile.BillingAddress, Faker.Address.StreetAddress);
            paymentProfile.BillingAddress2 = GetNewRandomValue(subscription.PaymentProfile.BillingAddress2, Faker.Address.StreetAddress);
            paymentProfile.BillingCity = GetNewRandomValue(subscription.PaymentProfile.BillingCity, Faker.Address.StreetAddress);
            paymentProfile.BillingState = GetNewRandomValue(subscription.PaymentProfile.BillingState, Faker.Address.StreetAddress);
            paymentProfile.BillingZip = GetNewRandomValue(subscription.PaymentProfile.BillingZip, Faker.Address.StreetAddress);

            // Act
            var result = Chargify.UpdatePaymentProfile(paymentProfile);

            // Arrange
            Assert.IsNotNull(result);
            Assert.AreEqual(subscription.PaymentProfile.Id, result.Id);
            Assert.AreEqual(paymentProfile.BillingAddress, result.BillingAddress);
            Assert.AreEqual(paymentProfile.BillingAddress2, result.BillingAddress2);
            Assert.AreEqual(paymentProfile.BillingCity, result.BillingCity);
            Assert.AreEqual(paymentProfile.BillingState, result.BillingState);
            Assert.AreEqual(paymentProfile.BillingZip, result.BillingZip);
            Assert.AreNotEqual(subscription.PaymentProfile.BillingAddress, result.BillingAddress);
            Assert.AreNotEqual(subscription.PaymentProfile.BillingAddress2, result.BillingAddress2);
            Assert.AreNotEqual(subscription.PaymentProfile.BillingCity, result.BillingCity);
            Assert.AreNotEqual(subscription.PaymentProfile.BillingState, result.BillingState);
            Assert.AreNotEqual(subscription.PaymentProfile.BillingZip, result.BillingZip);
        }

        [TestMethod]
        public void PaymentProfile_GetSinglePaymentProfileByID()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value;
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
