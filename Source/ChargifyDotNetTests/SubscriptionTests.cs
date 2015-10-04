using System;
using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyNET;
using System.Collections.Generic;
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
    public class SubscriptionTests : ChargifyTestBase
    {
        #region Tests
        [Test]
        public void Subscription_Create_Using_Existing_Customer()
        {
            // Arrange
            var client = this.Chargify;
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var exampleCustomer = client.GetCustomerList().Values.DefaultIfEmpty(defaultValue: null).FirstOrDefault();
            var paymentInfo = GetTestPaymentMethod(exampleCustomer.ToCustomerAttributes() as CustomerAttributes);

            // Act
            var newSubscription = client.CreateSubscription(product.Handle, exampleCustomer.ChargifyID, paymentInfo);

            // Assert
            Assert.IsNotNull(newSubscription);
            Assert.AreEqual(exampleCustomer.FirstName, newSubscription.Customer.FirstName);
            Assert.AreEqual(exampleCustomer.LastName, newSubscription.Customer.LastName);
            Assert.AreEqual(exampleCustomer.ChargifyID, newSubscription.Customer.ChargifyID);
        }

        [Test]
        public void Subscription_Can_Get_PaymentProfile_Id()
        {
            // Arrange
            var client = this.Chargify;

            // Act
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active && s.Value.PaymentProfile != null).Value;
            var loadedSubscription = client.LoadSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(loadedSubscription);
            Assert.IsNotNull(loadedSubscription.PaymentProfile);
            Assert.IsTrue(loadedSubscription.PaymentProfile.Id >= 0);
        }

        [Test]
        public void Subscription_Can_Get_ProductVersion()
        {
            // Arrange
            var client = this.Chargify;

            // Act
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;

            // Assert
            Assert.IsNotNull(subscription);
            Assert.IsTrue(subscription.ProductVersionNumber >= 0);
        }

        [Test]
        public void Subscription_Can_Update_Payment_FirstLast()
        {
            // Arrange
            var client = this.Chargify;
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            var oldFirst = subscription.PaymentProfile.FirstName;
            var oldLast = subscription.PaymentProfile.LastName;
            var newFirst = Guid.NewGuid().ToString();
            var newLast = Guid.NewGuid().ToString();
            var oldAttributes = new CreditCardAttributes(oldFirst, oldLast, "1",
                subscription.PaymentProfile.ExpirationYear, subscription.PaymentProfile.ExpirationMonth, "123", subscription.PaymentProfile.BillingAddress, subscription.PaymentProfile.BillingCity,
                subscription.PaymentProfile.BillingState, subscription.PaymentProfile.BillingZip, subscription.PaymentProfile.BillingCountry);
            var newAttributes = new CreditCardAttributes(newFirst, newLast, "1",
                subscription.PaymentProfile.ExpirationYear, subscription.PaymentProfile.ExpirationMonth, "123", subscription.PaymentProfile.BillingAddress, subscription.PaymentProfile.BillingCity,
                subscription.PaymentProfile.BillingState, subscription.PaymentProfile.BillingZip, subscription.PaymentProfile.BillingCountry);

            // Act
            var updatedSubscription = client.UpdateSubscriptionCreditCard(subscription.SubscriptionID, newAttributes);

            // Assert
            Assert.IsNotNull(updatedSubscription);
            //Assert.IsInstanceOfType(updatedSubscription, typeof(Subscription));
            Assert.AreEqual(newFirst, updatedSubscription.PaymentProfile.FirstName);
            Assert.AreEqual(newLast, updatedSubscription.PaymentProfile.LastName);
            Assert.AreEqual(subscription.PaymentProfile.ExpirationYear, updatedSubscription.PaymentProfile.ExpirationYear);
            Assert.AreEqual(subscription.PaymentProfile.ExpirationMonth, updatedSubscription.PaymentProfile.ExpirationMonth);
            Assert.AreEqual(subscription.PaymentProfile.BillingAddress, updatedSubscription.PaymentProfile.BillingAddress);
            Assert.AreEqual(subscription.PaymentProfile.BillingCity, updatedSubscription.PaymentProfile.BillingCity);
            Assert.AreEqual(subscription.PaymentProfile.BillingState, updatedSubscription.PaymentProfile.BillingState);
            Assert.AreEqual(subscription.PaymentProfile.BillingZip, updatedSubscription.PaymentProfile.BillingZip);
            Assert.AreEqual(subscription.PaymentProfile.BillingCountry, updatedSubscription.PaymentProfile.BillingCountry);

            // Cleanup
            var replacedSubscription = client.UpdateSubscriptionCreditCard(subscription.SubscriptionID, oldAttributes);
            Assert.IsNotNull(replacedSubscription);
            Assert.AreEqual(oldFirst, replacedSubscription.PaymentProfile.FirstName);
            Assert.AreEqual(oldLast, replacedSubscription.PaymentProfile.LastName);
        }

        [Test]
        public void Subscription_Can_Update_PaymentCollectionMethod()
        {
            // Arrange
            var client = this.Chargify;
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            var paymentCollectionMethod = subscription.PaymentCollectionMethod == PaymentCollectionMethod.Automatic ? PaymentCollectionMethod.Invoice : PaymentCollectionMethod.Automatic;

            // Act
            var updatedSubscription = client.UpdatePaymentCollectionMethod(subscription.SubscriptionID, paymentCollectionMethod);

            // Assert
            Assert.IsNotNull(subscription);
#if !NUNIT
            Assert.IsInstanceOfType(subscription, typeof(Subscription));
            Assert.IsInstanceOfType(updatedSubscription, typeof(Subscription));
#endif
            Assert.IsNotNull(updatedSubscription);
            Assert.IsTrue(subscription.PaymentCollectionMethod != updatedSubscription.PaymentCollectionMethod);
        }

        [Test]
        public void Subscription_ReactivateExpired()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Expired).Value;
            if (subscription != null)
            {
                // Act
                if (subscription.BalanceInCents > 0)
                {
                    var resetResult = Chargify.ResetSubscriptionBalance(subscription.SubscriptionID);

                }
                var cancelledResult = Chargify.DeleteSubscription(subscription.SubscriptionID, "");
                var reactivateResult = Chargify.ReactivateSubscription(subscription.SubscriptionID);

                // Assert
                Assert.IsNotNull(reactivateResult);
                Assert.IsTrue(reactivateResult.State == SubscriptionState.Active);
            }
        }

        [Test]
        public void Subscription_Can_Reactivate_Without_Trial()
        {
            // Arrange
            var trialingProduct = Chargify.GetProductList().Values.FirstOrDefault(p => p.TrialInterval > 0);
            var referenceID = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "123-456-7890", "Chargify", referenceID);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);
            var createdSubscription = Chargify.CreateSubscription(trialingProduct.Handle, newCustomer, newPaymentInfo);
            Assert.IsNotNull(createdSubscription);
            var deletedSubscription = Chargify.DeleteSubscription(createdSubscription.SubscriptionID, "Delete for test Subscription_Can_Reactivate_With_Trial");
            Assert.IsNotNull(deletedSubscription);
            var foundSubscription = Chargify.Find<Subscription>(createdSubscription.SubscriptionID);
            Assert.IsTrue(foundSubscription.State == SubscriptionState.Canceled, "Expected cancelled subscription on a trial product");

            // Act
            var result = Chargify.ReactivateSubscription(foundSubscription.SubscriptionID, false);

            // Assert
            Assert.IsNotNull(result);
#if !NUNIT
            Assert.IsInstanceOfType(result, typeof(ISubscription));
#endif
            Assert.IsTrue(result.State != foundSubscription.State);
            Assert.IsTrue(result.State == SubscriptionState.Active);
        }

        [Test]
        public void Subscription_Can_Reactivate_With_Trial()
        {
            // Arrange
            var trialingProduct = Chargify.GetProductList().Values.FirstOrDefault(p => p.TrialInterval > 0);
            var referenceID = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "123-456-7890", "Chargify", referenceID);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);
            var createdSubscription = Chargify.CreateSubscription(trialingProduct.Handle, newCustomer, newPaymentInfo);
            Assert.IsNotNull(createdSubscription);
            var deletedSubscription = Chargify.DeleteSubscription(createdSubscription.SubscriptionID, "Delete for test Subscription_Can_Reactivate_With_Trial");
            Assert.IsNotNull(deletedSubscription);
            var foundSubscription = Chargify.Find<Subscription>(createdSubscription.SubscriptionID);
            Assert.IsTrue(foundSubscription.State == SubscriptionState.Canceled, "Expected cancelled subscription on a trial product");

            // Act
            var result = Chargify.ReactivateSubscription(foundSubscription.SubscriptionID, true, null, null);

            // Assert
            Assert.IsNotNull(result);
#if !NUNIT
            Assert.IsInstanceOfType(result, typeof(ISubscription));
#endif
            Assert.IsTrue(result.State != foundSubscription.State);
            Assert.IsTrue(result.State == SubscriptionState.Trialing);
        }

        [Test]
        public void Subscription_Reactivation()
        {
            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var referenceID = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "123-456-7890", "Chargify", referenceID);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);

            // Act
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo);
            var result = Chargify.DeleteSubscription(newSubscription.SubscriptionID, "testing");
            var foundSubscription = Chargify.Find<Subscription>(newSubscription.SubscriptionID);
            var resultSubscription = Chargify.ReactivateSubscription(foundSubscription.SubscriptionID);

            // Assert
#if !NUNIT
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
            Assert.IsInstanceOfType(foundSubscription, typeof(Subscription));
            Assert.IsInstanceOfType(resultSubscription, typeof(Subscription));
#endif
            Assert.IsNotNull(newSubscription);
            Assert.IsTrue(result);
            Assert.IsNotNull(foundSubscription);
            Assert.IsTrue(foundSubscription.State == SubscriptionState.Canceled);
            Assert.IsNotNull(newSubscription);
            Assert.IsTrue(resultSubscription.State == SubscriptionState.Active);
        }

        [Test]
        public void Subscription_Create_With_SpecialChars()
        {
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var referenceID = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott!", "Pilgrim@", "demonhead_sucks@scottpilgrim.com", "+1 (123) 456-7890", "@Chargify#$%^&@", referenceID);
            newCustomer.ShippingAddress = @"123 Main St.*()-=_+`~";
            newCustomer.ShippingCity = @"Kingston{}[]|;':";
            newCustomer.ShippingState = @"ON<>,.?/";
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);

            // Act
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo);

            // Assert
#if !NUNIT
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
#endif
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNotNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.FirstName == newCustomer.FirstName);
            Assert.IsTrue(newSubscription.Customer.LastName == newCustomer.LastName);
            Assert.IsTrue(newSubscription.Customer.Email == newCustomer.Email);
            Assert.IsTrue(newSubscription.Customer.Phone == newCustomer.Phone);
            Assert.IsTrue(newSubscription.Customer.Organization == newCustomer.Organization);
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceID);
            Assert.IsTrue(newSubscription.PaymentProfile.FirstName == newPaymentInfo.FirstName);
            Assert.IsTrue(newSubscription.PaymentProfile.LastName == newPaymentInfo.LastName);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationMonth == newPaymentInfo.ExpirationMonth);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationYear == newPaymentInfo.ExpirationYear);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress == newPaymentInfo.BillingAddress);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCity == newPaymentInfo.BillingCity);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCountry == newPaymentInfo.BillingCountry);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingState == newPaymentInfo.BillingState);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingZip == newPaymentInfo.BillingZip);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));
        }

        [Test]
        public void Subscription_Create()
        {
            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var referenceID = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "123-456-7890", "Chargify", referenceID);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);

            // Act
            string data = string.Empty;
            Chargify.LogRequest = (requestMethod, address, postedData) => {
                data = postedData;
            };
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo);

            // Assert
#if !NUNIT
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
#endif
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNotNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            Assert.AreEqual(newCustomer.FirstName, newSubscription.Customer.FirstName);
            Assert.AreEqual(newCustomer.LastName, newSubscription.Customer.LastName);
            Assert.AreEqual(newCustomer.Email, newSubscription.Customer.Email);
            Assert.AreEqual(newCustomer.Phone, newSubscription.Customer.Phone);
            Assert.IsTrue(newSubscription.Customer.Organization == newCustomer.Organization);
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceID);
            Assert.IsTrue(newSubscription.PaymentProfile.FirstName == newPaymentInfo.FirstName);
            Assert.IsTrue(newSubscription.PaymentProfile.LastName == newPaymentInfo.LastName);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationMonth == newPaymentInfo.ExpirationMonth);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationYear == newPaymentInfo.ExpirationYear);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress == newPaymentInfo.BillingAddress);
            //Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress2 == newPaymentInfo.BillingAddress2);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCity == newPaymentInfo.BillingCity);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCountry == newPaymentInfo.BillingCountry);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingState == newPaymentInfo.BillingState);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingZip == newPaymentInfo.BillingZip);
            Assert.IsTrue(newSubscription.ProductPriceInCents == product.PriceInCents);
            Assert.IsTrue(newSubscription.ProductPrice == product.Price);
            Assert.AreEqual(product.TrialInterval > int.MinValue ? SubscriptionState.Trialing : SubscriptionState.Active, newSubscription.State);
            Assert.AreEqual(Chargify.UseJSON, Chargify.UseJSON ? data.IsJSON() : data.IsXml());

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));
        }

        [Test]
        public void Subscription_Create_WithComponent()
        {
            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            var referenceID = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", referenceID);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);
            var component = Chargify.GetComponentsForProductFamily(productFamily.ID).FirstOrDefault(d => d.Value.Kind == ComponentType.Quantity_Based_Component && d.Value.PricePerUnit > 0).Value;

            // Act
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo, component.ID, 5);
            var subComponents = Chargify.GetComponentsForSubscription(newSubscription.SubscriptionID);
            var usedComponents = from c in subComponents
                                 where c.Value.ComponentID == component.ID
                                 select c;

            // Assert
#if !NUNIT
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
#endif
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNotNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.FirstName == newCustomer.FirstName);
            Assert.IsTrue(newSubscription.Customer.LastName == newCustomer.LastName);
            Assert.IsTrue(newSubscription.Customer.Email == newCustomer.Email);
            Assert.IsTrue(newSubscription.Customer.Organization == newCustomer.Organization);
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceID);
            Assert.IsTrue(newSubscription.PaymentProfile.FirstName == newPaymentInfo.FirstName);
            Assert.IsTrue(newSubscription.PaymentProfile.LastName == newPaymentInfo.LastName);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationMonth == newPaymentInfo.ExpirationMonth);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationYear == newPaymentInfo.ExpirationYear);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress == newPaymentInfo.BillingAddress);
            //Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress2 == newPaymentInfo.BillingAddress2);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCity == newPaymentInfo.BillingCity);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCountry == newPaymentInfo.BillingCountry);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingState == newPaymentInfo.BillingState);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingZip == newPaymentInfo.BillingZip);
            Assert.IsTrue(usedComponents.Count() == 1);
            Assert.IsTrue(usedComponents.FirstOrDefault().Value.AllocatedQuantity == 5);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));
        }

        [Test]
        public void Subscription_Create_WithTwoComponents()
        {
            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var referenceID = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", referenceID);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);

            Dictionary<int, string> componentsToUse = new Dictionary<int, string>()
            {
                {998, "5"},
                {6776, "1"}
            };

            // Act
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo, componentsToUse);
            var subComponents = Chargify.GetComponentsForSubscription(newSubscription.SubscriptionID);
            var usedComponents = from c in subComponents
                                 where componentsToUse.ContainsKey(c.Value.ComponentID)
                                 select c;

            // Assert
#if !NUNIT
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
#endif
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNotNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.FirstName == newCustomer.FirstName);
            Assert.IsTrue(newSubscription.Customer.LastName == newCustomer.LastName);
            Assert.IsTrue(newSubscription.Customer.Email == newCustomer.Email);
            Assert.IsTrue(newSubscription.Customer.Organization == newCustomer.Organization);
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceID);
            Assert.IsTrue(newSubscription.PaymentProfile.FirstName == newPaymentInfo.FirstName);
            Assert.IsTrue(newSubscription.PaymentProfile.LastName == newPaymentInfo.LastName);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationMonth == newPaymentInfo.ExpirationMonth);
            Assert.IsTrue(newSubscription.PaymentProfile.ExpirationYear == newPaymentInfo.ExpirationYear);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress == newPaymentInfo.BillingAddress);
            //Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress2 == newPaymentInfo.BillingAddress2);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCity == newPaymentInfo.BillingCity);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingCountry == newPaymentInfo.BillingCountry);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingState == newPaymentInfo.BillingState);
            Assert.IsTrue(newSubscription.PaymentProfile.BillingZip == newPaymentInfo.BillingZip);
            Assert.IsTrue(usedComponents.Count() == 2);
            Assert.IsTrue(usedComponents.FirstOrDefault().Value.AllocatedQuantity == 5);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));
        }

        [Test]
        public void Subscription_Create_WithCouponAfterSignup()
        {
            // Arrange
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", Guid.NewGuid().ToString());
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);

            // Act
            var createdSubscription = Chargify.CreateSubscription("premium", newCustomer, newPaymentInfo);

            // Assert
            Assert.IsNotNull(createdSubscription);
#if !NUNIT
            Assert.IsInstanceOfType(createdSubscription, typeof(ISubscription));
#endif
            Assert.IsTrue(createdSubscription.CouponCode == string.Empty);

            // Act Again
            var updatedSubscription = Chargify.AddCoupon(createdSubscription.SubscriptionID, "AC511");

            // Assert Again
            Assert.IsNotNull(updatedSubscription);
#if !NUNIT
            Assert.IsInstanceOfType(updatedSubscription, typeof(ISubscription));
#endif
            Assert.IsTrue(updatedSubscription.CouponCode == "AC511");
        }

        [Test]
        public void Subscription_UpdateBillingDate()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active).Value;
            var billingDate = subscription.NextAssessmentAt;

            // Act
            var updatedSubscription = Chargify.UpdateBillingDateForSubscription(subscription.SubscriptionID, billingDate.AddDays(5));

            // Assert
            Assert.IsTrue(billingDate.AddDays(5) == updatedSubscription.NextAssessmentAt);

            // Cleanup
            var restoredSubscription = Chargify.UpdateBillingDateForSubscription(updatedSubscription.SubscriptionID, billingDate);
            Assert.IsTrue(billingDate == restoredSubscription.NextAssessmentAt);
        }

        [Test, Ignore]
        public void Subscription_Update()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == ChargifyNET.SubscriptionState.Active && s.Value.PaymentProfile != null).Value as Subscription;
            string originalEmail = subscription.Customer.Email;
            subscription.Customer.Email = "newemail@testing.com";

            // Act
            var updatedSubscription = Chargify.Save<Subscription>(subscription);

            // Assert
            Assert.IsNotNull(updatedSubscription);
            Assert.IsNotNull(updatedSubscription.Customer);
            Assert.IsTrue(updatedSubscription.Customer.Email == subscription.Customer.Email);

            // Cleanup
            updatedSubscription.Customer.Email = originalEmail;
            var restoredSubscription = Chargify.Save<Subscription>(updatedSubscription);
            Assert.IsTrue(restoredSubscription.Customer.Email == updatedSubscription.Customer.Email);
        }

        [Test, Ignore]
        public void Subscription_Load_Where_State_Is_TrialEnded()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Trial_Ended).Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");

            // Act
            var retreivedSubscription = Chargify.Find<Subscription>(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(retreivedSubscription);            
            Assert.IsTrue(retreivedSubscription.State == SubscriptionState.Trial_Ended);
#if !NUNIT
            Assert.IsInstanceOfType(retreivedSubscription, typeof(Subscription));
#endif
        }

        [Test]
        public void Subscription_Load_ComponentsFor()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");

            // Act
            var subscriptionComponents = Chargify.GetComponentsForSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(subscriptionComponents);
            Assert.IsTrue(subscriptionComponents.Count > 0);
        }

        [Test]
        public void Subscription_Create_Using_ExistingProfile()
        {
            // Arrange
            var existingSubscription = Chargify.GetSubscriptionList().Values.FirstOrDefault(s => s.State == SubscriptionState.Active && s.PaymentProfile != null && s.PaymentProfile.Id > 0) as Subscription;
            Assert.IsNotNull(existingSubscription, "No applicable subscription found.");
            Assert.IsTrue(existingSubscription.PaymentProfile.Id > 0);

            // Act
            var newSubscription = Chargify.CreateSubscription(existingSubscription.Product.Handle, existingSubscription.Customer.ToCustomerAttributes(), DateTime.MinValue, existingSubscription.PaymentProfile.Id);

            // Assert
            Assert.IsNotNull(newSubscription);
            Assert.AreEqual(existingSubscription.PaymentProfile.Id, newSubscription.PaymentProfile.Id);
        }
        #endregion

        private CreditCardAttributes GetTestPaymentMethod(CustomerAttributes customer)
        {
            var retVal = new CreditCardAttributes()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                ExpirationMonth = 1,
                ExpirationYear = 2020,
                FullNumber = "1",
                CVV = "123",
                BillingAddress = "123 Main St.",
                BillingCity = "New York",
                BillingCountry = "US",
                BillingState = "New York",
                BillingZip = "10001"
            };
            return retVal;
        }
    }
}
