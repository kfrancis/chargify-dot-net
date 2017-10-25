using System;
using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyNET;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        public void Subscription_Can_Pause_Indefinately()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;

            // Act
            var result = Chargify.PauseSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(SubscriptionState.On_Hold, result.State);
            result = Chargify.ResumeSubscription(subscription.SubscriptionID);
            Assert.AreEqual(SubscriptionState.Active, result.State);
        }

        [Test]
        public void Subscription_Can_Pause_FixedTime()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            var testMinutes = 5;

            // Act
            var result = Chargify.PauseSubscription(subscription.SubscriptionID, DateTime.Now.AddMinutes(testMinutes));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(SubscriptionState.On_Hold, result.State);
            var totalWait = Convert.ToInt32(Math.Ceiling(1000 * 60 * testMinutes + (testMinutes * 0.10f)));
            Debug.WriteLine($"Waiting {TimeSpan.FromMilliseconds(totalWait).TotalMinutes} minutes to test a delay of {testMinutes} minutes ...");
            Thread.Sleep(totalWait);
            result = Chargify.LoadSubscription(subscription.SubscriptionID);
            Assert.AreEqual(SubscriptionState.Active, result.State);
        }

        [Test]
        public void Subscription_Can_Resume()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.On_Hold).Value as Subscription;
            Assert.IsNotNull(subscription, "Can't find any 'on_hold' subscriptions");

            // Act
            var result = Chargify.ResumeSubscription(subscription.SubscriptionID);
                
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(SubscriptionState.Active, result.State);
        }

        [Test]
        public void Subscription_Can_Cancel_Delayed_Product_Change()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null && s.Value.NextProductId <= 0).Value;
            var otherProduct = Chargify.GetProductList().FirstOrDefault(p => p.Key != subscription.Product.ID);
            var updatedSubscription = Chargify.EditSubscriptionProduct(subscription.SubscriptionID, otherProduct.Value.Handle, true);
            Assert.AreEqual(otherProduct.Key, updatedSubscription.NextProductId);

            // Act
            var result = Chargify.CancelDelayedProductChange(updatedSubscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.NextProductId <= 0);
        }

        [Test]
        public void Subscription_Create_UsingOptions_ProductHandle()
        {
            // Arrange
            var exampleCustomer = Chargify.GetCustomerList().Values.DefaultIfEmpty(null).FirstOrDefault();
            Assert.IsNotNull(exampleCustomer, "Customer not found");
            var paymentInfo = GetTestPaymentMethod(exampleCustomer.ToCustomerAttributes() as CustomerAttributes);
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            Assert.IsNotNull(product, "Product not found");
            var options = new SubscriptionCreateOptions()
            {
                CustomerID = exampleCustomer.ChargifyID,
                CreditCardAttributes = paymentInfo,
                ProductHandle = product.Handle
            };

            // Act
            var result = Chargify.CreateSubscription(options);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Handle, result.Product.Handle);
            Assert.AreEqual(exampleCustomer.ChargifyID, result.Customer.ChargifyID);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void Subscription_Create_UsingOptions_TooManyProducts()
        {
            // Arrange
            var exampleCustomer = Chargify.GetCustomerList().Values.DefaultIfEmpty(defaultValue: null).FirstOrDefault();
            var paymentInfo = GetTestPaymentMethod(exampleCustomer.ToCustomerAttributes() as CustomerAttributes);
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var options = new SubscriptionCreateOptions()
            {
                CustomerID = exampleCustomer.ChargifyID,
                CreditCardAttributes = paymentInfo,
                ProductHandle = product.Handle,
                ProductID = product.ID
            };

            // Act
            var result = Chargify.CreateSubscription(options);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void Subscription_Create_UsingOptions_MissingProduct()
        {
            // Arrange
            var exampleCustomer = Chargify.GetCustomerList().Values.DefaultIfEmpty(defaultValue: null).FirstOrDefault();
            var paymentInfo = GetTestPaymentMethod(exampleCustomer.ToCustomerAttributes() as CustomerAttributes);
            var options = new SubscriptionCreateOptions()
            {
                CustomerID = exampleCustomer.ChargifyID,
                CreditCardAttributes = paymentInfo
            };

            // Act
            var result = Chargify.CreateSubscription(options);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void Subscription_Create_UsingOptions_MissingAllDetails()
        {
            // Arrange
            var options = new SubscriptionCreateOptions();

            // Act
            var result = Chargify.CreateSubscription(options);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Subscription_Create_UsingOptions_Null()
        {
            // Arrange
            SubscriptionCreateOptions options = null;

            // Act
            var result = Chargify.CreateSubscription(options);
        }

        [Test]
        public void Subscription_Create_Using_Existing_Customer()
        {
            // Arrange
            var client = Chargify;
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
            var client = Chargify;

            // Act
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value;
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
            var client = Chargify;

            // Act
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;

            // Assert
            Assert.IsNotNull(subscription);
            Assert.IsTrue(subscription.ProductVersionNumber >= 0);
        }

        [Test]
        [ExpectedException(typeof(ChargifyException))]
        public void Subscription_Does_PartialUpdate_Fail()
        {
            // Arrange
            var client = Chargify;
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;

            // Check that the card isn't expired
            var expDate = new DateTime(subscription.PaymentProfile.ExpirationYear, subscription.PaymentProfile.ExpirationMonth, 1);
            if (expDate < DateTime.Now)
            {
                subscription = client.UpdateSubscriptionCreditCard(subscription.SubscriptionID, "1", DateTime.Now.AddMonths(1).Month, DateTime.Now.AddYears(1).Year, "123", subscription.PaymentProfile.BillingAddress, subscription.PaymentProfile.BillingCity, subscription.PaymentProfile.BillingState, subscription.PaymentProfile.BillingZip, subscription.PaymentProfile.BillingCountry);
            }

            string oldAddress = subscription.PaymentProfile.BillingAddress, oldAddress2 = subscription.PaymentProfile.BillingAddress2,
                oldCity = subscription.PaymentProfile.BillingCity,
                oldState = subscription.PaymentProfile.BillingState, oldZip = subscription.PaymentProfile.BillingZip;

            var newAttributes = new CreditCardAttributes()
            {
                BillingAddress = GetNewRandomValue(oldAddress, Faker.Address.StreetAddress),
                BillingAddress2 = GetNewRandomValue(oldAddress2, Faker.Address.SecondaryAddress),
                BillingCity = GetNewRandomValue(oldCity, Faker.Address.City),
                BillingState = GetNewRandomValue(oldState, Faker.Address.UsState),
                BillingZip = GetNewRandomValue(oldZip, Faker.Address.ZipCode),
                BillingCountry = "US"
            };

            // Act
            var result = client.UpdateSubscriptionCreditCard(subscription.SubscriptionID, newAttributes);
        }

        [Test]
        public void Subscription_Can_Update_Payment_FirstLast()
        {
            // Arrange
            var client = Chargify;
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
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
            var client = Chargify;
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
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
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Expired).Value;
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
        public void Subscription_Can_EditProduct_WithDelay()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value as Subscription;
            var otherProduct = Chargify.GetProductList().Values.Where(p => p.Handle != subscription.Product.Handle).FirstOrDefault();

            // Act
            var result = Chargify.EditSubscriptionProduct(subscription.SubscriptionID, otherProduct.Handle, true);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(subscription.Product.Handle, result.Product.Handle);
            Assert.AreNotEqual(int.MinValue, subscription.NextProductId);
            Assert.AreEqual(otherProduct.ID, result.NextProductId);
        }

        [Test]
        public void Subscription_Can_EditProduct_NoDelay()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value as Subscription;
            var otherProduct = Chargify.GetProductList().Values.Where(p => p.Handle != subscription.Product.Handle).FirstOrDefault();

            // Act
            var result = Chargify.EditSubscriptionProduct(subscription.SubscriptionID, otherProduct.Handle);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(otherProduct.Handle, result.Product.Handle);
        }

        [Test]
        public void Subscription_Can_Reactivate_Without_Trial()
        {
            // Arrange
            var trialingProduct = Chargify.GetProductList().Values.FirstOrDefault(p => p.TrialInterval > 0);
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes(Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Phone.Number(), Faker.Company.Name(), referenceId);
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
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes(Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Phone.Number(), Faker.Company.Name(), referenceId);
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
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes(Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Phone.Number(), Faker.Company.Name(), referenceId);
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
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott!", "Pilgrim@", "demonhead_sucks@scottpilgrim.com", "+1 (123) 456-7890", "@Chargify#$%^&@", referenceId);
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
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceId);
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
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes(Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Phone.Number(), Faker.Company.Name(), referenceId);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);

            // Act
            string data = string.Empty;
            Chargify.LogRequest = (requestMethod, address, postedData) =>
            {
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
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceId);
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
            Assert.IsTrue(!string.IsNullOrWhiteSpace(newSubscription.ReferralCode));
            Assert.AreEqual(product.TrialInterval > 0 ? SubscriptionState.Trialing : SubscriptionState.Active, newSubscription.State);
            if (Chargify.UseJSON)
            {
                Assert.AreEqual(true, data.IsJSON());
                Assert.AreEqual(false, data.IsXml());
            }
            else
            {
                Assert.AreEqual(true, data.IsXml());
                Assert.AreEqual(false, data.IsJSON());
            }

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));
        }

        [Test]
        public void Subscription_Create_WithComponent()
        {
            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", referenceId);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);
            var component = Chargify.GetComponentsForProductFamily(productFamily.ID).FirstOrDefault(d => d.Value.Kind == ComponentType.Quantity_Based_Component && d.Value.Prices.Any(p => p.UnitPrice > 0m)).Value;
            Assert.IsNotNull(component, "Couldn't find any usable component.");

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
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceId);
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
            var referenceId = Guid.NewGuid().ToString();
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", referenceId);
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);
            var components = Chargify.GetComponentsForProductFamily(product.ProductFamily.ID);
            var componentsToUse = components.Take(2).ToDictionary(v => v.Key, v => "1");

            // Act
            Assert.IsNotNull(product, "Product couldn't be found");
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
            Assert.IsTrue(newSubscription.Customer.SystemID == referenceId);
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
            Assert.IsTrue(usedComponents.Count() == componentsToUse.Count);
            foreach (var component in usedComponents)
            {
                Assert.IsTrue(componentsToUse.ContainsKey(component.Key));
                //Assert.AreEqual(decimal.Parse(componentsToUse[component.Key]), component.Value.AllocatedQuantity);
            }

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));
        }

        [Test]
        public void Subscription_Create_WithCouponAfterSignup()
        {
            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", Guid.NewGuid().ToString());
            var newPaymentInfo = GetTestPaymentMethod(newCustomer);
            const string couponCode = "68C8FDBA";

            // Act
            var createdSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo);

            // Assert
            Assert.IsNotNull(createdSubscription);
#if !NUNIT
            Assert.IsInstanceOfType(createdSubscription, typeof(ISubscription));
#endif
            Assert.IsTrue(createdSubscription.CouponCode == string.Empty);

            // Act Again
            var updatedSubscription = Chargify.AddCoupon(createdSubscription.SubscriptionID, couponCode);

            // Assert Again
            Assert.IsNotNull(updatedSubscription);
#if !NUNIT
            Assert.IsInstanceOfType(updatedSubscription, typeof(ISubscription));
#endif
            Assert.IsTrue(updatedSubscription.CouponCode == couponCode);
        }

        [Test]
        public void Subscription_UpdateBillingDate()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
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
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value as Subscription;
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
