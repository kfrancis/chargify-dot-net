using Bogus;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using ChargifyDotNet.Tests;
using Shouldly;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class SubscriptionTests : ChargifyTestBase
    {
        #region Tests

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Pause_Indefinately(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;

            // Act
            var result = Chargify.PauseSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(SubscriptionState.On_Hold, result.State);
            result = Chargify.ResumeSubscription(subscription.SubscriptionID);
            Assert.AreEqual(SubscriptionState.Active, result.State);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Pause_FixedTime(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var testDays = 5;

            // Act
            var result = Chargify.PauseSubscription(subscription.SubscriptionID, DateTime.Now.AddDays(testDays));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(SubscriptionState.On_Hold, result.State);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Resume(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.On_Hold).Value as Subscription;
            Assert.IsNotNull(subscription, "Can't find any 'on_hold' subscriptions");

            // Act
            var result = Chargify.ResumeSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(SubscriptionState.Active, result.State);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Cancel_Delayed_Product_Change(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_UsingOptions_ProductHandle(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var exampleCustomer = Chargify.GetCustomerList().Values.DefaultIfEmpty(null).FirstOrDefault();
            Assert.IsNotNull(exampleCustomer, "Customer not found");
            var paymentInfo = SubscriptionTests.GetTestPaymentMethod(exampleCustomer.ToCustomerAttributes() as CustomerAttributes);
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

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_UsingOptions_TooManyProducts(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);
            var exampleCustomer = Chargify.GetCustomerList().Values.DefaultIfEmpty(defaultValue: null).FirstOrDefault();
            var paymentInfo = SubscriptionTests.GetTestPaymentMethod(exampleCustomer.ToCustomerAttributes() as CustomerAttributes);
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var options = new SubscriptionCreateOptions()
            {
                CustomerID = exampleCustomer.ChargifyID,
                CreditCardAttributes = paymentInfo,
                ProductHandle = product.Handle,
                ProductID = product.ID
            };
            try
            {
                var _ = Chargify.CreateSubscription(options);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException) { }
            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_UsingOptions_MissingProduct(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);
            var exampleCustomer = Chargify.GetCustomerList().Values.DefaultIfEmpty(defaultValue: null).FirstOrDefault();
            var paymentInfo = SubscriptionTests.GetTestPaymentMethod(exampleCustomer.ToCustomerAttributes() as CustomerAttributes);
            var options = new SubscriptionCreateOptions()
            {
                CustomerID = exampleCustomer.ChargifyID,
                CreditCardAttributes = paymentInfo
            };
            try
            {
                var _ = Chargify.CreateSubscription(options);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException) { }
            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_UsingOptions_MissingAllDetails(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);
            var options = new SubscriptionCreateOptions();
            try
            {
                var _ = Chargify.CreateSubscription(options);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException) { }
            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_UsingOptions_Null(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);
            SubscriptionCreateOptions options = null;
            try
            {
                var _ = Chargify.CreateSubscription(options);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException) { }
            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_Using_Existing_Customer(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var client = Chargify;
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var exampleCustomer = client.GetCustomerList().Values.DefaultIfEmpty(defaultValue: null).FirstOrDefault();
            var paymentInfo = SubscriptionTests.GetTestPaymentMethod(exampleCustomer.ToCustomerAttributes() as CustomerAttributes);

            // Act
            var newSubscription = client.CreateSubscription(product.Handle, exampleCustomer.ChargifyID, paymentInfo);

            // Assert
            Assert.IsNotNull(newSubscription);
            Assert.AreEqual(exampleCustomer.FirstName, newSubscription.Customer.FirstName);
            Assert.AreEqual(exampleCustomer.LastName, newSubscription.Customer.LastName);
            Assert.AreEqual(exampleCustomer.ChargifyID, newSubscription.Customer.ChargifyID);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Load_By_State(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscriptions = Chargify.GetSubscriptionList().Where(s => s.Value.State == SubscriptionState.Active);

            // Act
            var result = Chargify.GetSubscriptionList(SubscriptionState.Active);

            // Assert
            Assert.AreEqual(result.Count, subscriptions.Count());

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Load_many_results_By_State(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var state = SubscriptionState.Active;

            // Act
            var result = Chargify.GetSubscriptionList(state);

            // Assert
            Assert.IsTrue(result.Any());
            Assert.IsTrue(result.All(s => s.Value.State == state));

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Get_PaymentProfile_Id(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var client = Chargify;

            // Act
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value;
            var loadedSubscription = client.LoadSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(loadedSubscription);
            Assert.IsNotNull(loadedSubscription.PaymentProfile);
            Assert.IsTrue(loadedSubscription.PaymentProfile.Id >= 0);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Get_ProductVersion(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var client = Chargify;

            // Act
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;

            // Assert
            Assert.IsNotNull(subscription);
            Assert.IsTrue(subscription.ProductVersionNumber >= 0);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Does_PartialUpdate_Fail(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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
                BillingAddress = GetNewRandomValue(oldAddress, Faker.Address.StreetAddress, false),
                BillingAddress2 = GetNewRandomValue(oldAddress2, Faker.Address.SecondaryAddress),
                BillingCity = GetNewRandomValue(oldCity, Faker.Address.City),
                BillingState = GetNewRandomValue(oldState, Faker.Address.StateAbbr),
                BillingZip = GetNewRandomValue(oldZip, Faker.Address.ZipCode),
                BillingCountry = "US"
            };
            try
            {
                var _ = client.UpdateSubscriptionCreditCard(subscription.SubscriptionID, newAttributes);
                Assert.Fail("Expected ChargifyException was not thrown");
            }
            catch (ChargifyException) { }
            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Update_Payment_FirstLast(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Update_PaymentCollectionMethod(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var client = Chargify;
            var subscription = client.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            var paymentCollectionMethod = subscription.PaymentCollectionMethod == PaymentCollectionMethod.Automatic ? PaymentCollectionMethod.Remittance : PaymentCollectionMethod.Automatic;

            // Act
            var updatedSubscription = client.UpdatePaymentCollectionMethod(subscription.SubscriptionID, paymentCollectionMethod);

            // Assert
            Assert.IsNotNull(subscription);
            Assert.IsInstanceOfType(subscription, typeof(Subscription));
            Assert.IsInstanceOfType(updatedSubscription, typeof(Subscription));
            Assert.IsNotNull(updatedSubscription);
            Assert.IsTrue(subscription.PaymentCollectionMethod != updatedSubscription.PaymentCollectionMethod);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_ReactivateExpired(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_EditProduct_WithDelay(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_EditProduct_NoDelay(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value as Subscription;
            var otherProduct = Chargify.GetProductList().Values.Where(p => p.Handle != subscription.Product.Handle).FirstOrDefault();

            // Act
            var result = Chargify.EditSubscriptionProduct(subscription.SubscriptionID, otherProduct.Handle);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(otherProduct.Handle, result.Product.Handle);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Be_Purged(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var trialingProduct = Chargify.GetProductList().Values.FirstOrDefault(p => p.TrialInterval > 0);
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;

            var newCustomer = new CustomerAttributes(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.Email(), Faker.Phone.PhoneNumber(), Faker.Company.CompanyName(), referenceId);
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);
            var createdSubscription = Chargify.CreateSubscription(trialingProduct.Handle, newCustomer, newPaymentInfo);
            Assert.IsNotNull(createdSubscription);

            Chargify.PurgeSubscription(createdSubscription.SubscriptionID);
            var purgedSubscription = Chargify.Find<Subscription>(createdSubscription.SubscriptionID);

            Assert.IsNull(purgedSubscription);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Reactivate_Without_Trial(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var trialingProduct = Chargify.GetProductList().Values.FirstOrDefault(p => p.TrialInterval > 0 && !string.IsNullOrEmpty(p.Handle));
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;

            var newCustomer = new CustomerAttributes(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.Email(), Faker.Phone.PhoneNumber(), Faker.Company.CompanyName(), referenceId);
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);
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
            Assert.IsInstanceOfType(result, typeof(ISubscription));
            Assert.IsTrue(result.State != foundSubscription.State);
            Assert.IsTrue(result.State == SubscriptionState.Active);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Can_Reactivate_With_Trial(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var trialingProduct = Chargify.GetProductList().Values.FirstOrDefault(p => p.TrialInterval > 0 && !string.IsNullOrEmpty(p.Handle));
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.Email(), Faker.Phone.PhoneNumber(), Faker.Company.CompanyName(), referenceId);
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);
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
            Assert.IsInstanceOfType(result, typeof(ISubscription));
            Assert.IsTrue(result.State != foundSubscription.State);
            Assert.IsTrue(result.State == SubscriptionState.Trialing);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Reactivation(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.Email(), Faker.Phone.PhoneNumber(), Faker.Company.CompanyName(), referenceId);
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);

            // Act
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo);
            var result = Chargify.DeleteSubscription(newSubscription.SubscriptionID, "testing");
            var foundSubscription = Chargify.Find<Subscription>(newSubscription.SubscriptionID);
            var resultSubscription = Chargify.ReactivateSubscription(foundSubscription.SubscriptionID);

            // Assert
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
            Assert.IsInstanceOfType(foundSubscription, typeof(Subscription));
            Assert.IsInstanceOfType(resultSubscription, typeof(Subscription));
            Assert.IsNotNull(newSubscription);
            Assert.IsTrue(result);
            Assert.IsNotNull(foundSubscription);
            Assert.IsTrue(foundSubscription.State == SubscriptionState.Canceled);
            Assert.IsNotNull(newSubscription);
            Assert.IsTrue(resultSubscription.State == SubscriptionState.Active);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_With_SpecialChars(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott!", "Pilgrim@", "demonhead_sucks@scottpilgrim.com", "+1 (123) 456-7890", "@Chargify#$%^&@", referenceId)
            {
                ShippingAddress = @"123 Main St.*()-=_+`~",
                ShippingCity = @"Kingston{}[]|;':",
                ShippingState = @"ON<>,.?/"
            };
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);

            // Act
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo);

            // Assert
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNotNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            newSubscription.Customer.FirstName.ShouldBe(newCustomer.FirstName);
            newSubscription.Customer.LastName.ShouldBe(newCustomer.LastName);
            newSubscription.Customer.Email.ShouldBe(newCustomer.Email);
            newSubscription.Customer.Phone.ShouldBe(newCustomer.Phone);
            newSubscription.Customer.Organization.ShouldBe(newCustomer.Organization);
            newSubscription.Customer.SystemID.ShouldBe(referenceId);
            newSubscription.PaymentProfile.FirstName.ShouldBe(newPaymentInfo.FirstName);
            newSubscription.PaymentProfile.LastName.ShouldBe(newPaymentInfo.LastName);
            newSubscription.PaymentProfile.ExpirationMonth.ShouldBe(newPaymentInfo.ExpirationMonth);
            newSubscription.PaymentProfile.ExpirationYear.ShouldBe(newPaymentInfo.ExpirationYear);
            newSubscription.PaymentProfile.BillingAddress.ShouldBe(newPaymentInfo.BillingAddress);
            newSubscription.PaymentProfile.BillingCity.ShouldBe(newPaymentInfo.BillingCity);
            newSubscription.PaymentProfile.BillingCountry.ShouldBe(newPaymentInfo.BillingCountry);
            newSubscription.PaymentProfile.BillingState.ShouldBe(newPaymentInfo.BillingState);
            newSubscription.PaymentProfile.BillingZip.ShouldBe(newPaymentInfo.BillingZip);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_IsFullNumberMasked(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.Email(), Faker.Phone.PhoneNumber(), Faker.Company.CompanyName(), referenceId);
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);
            newPaymentInfo.FullNumber = "4444444444444444";

            // Act
            try
            {
                var data = string.Empty;
                Chargify.LogRequest = (requestMethod, address, postedData) =>
                {
                    data = postedData;
                };
                var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo);

                Assert.Fail("Subscription should not have been created, since credit card number is not 1, 2 or 3 ending");
            }
            catch (ChargifyException chEx)
            {
                Assert.IsFalse(chEx.LastDataPosted.Contains(newPaymentInfo.FullNumber), chEx.LastDataPosted);
                Assert.IsTrue(chEx.LastDataPosted.Contains(newPaymentInfo.FullNumber.Mask('X', 4)));
            }

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.Email(), Faker.Phone.PhoneNumber(), Faker.Company.CompanyName(), referenceId);
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);

            // Act
            var data = string.Empty;
            Chargify.LogRequest = (requestMethod, address, postedData) =>
            {
                data = postedData;
            };
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo);

            // Assert
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNotNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            Assert.AreEqual(newCustomer.FirstName, newSubscription.Customer.FirstName);
            Assert.AreEqual(newCustomer.LastName, newSubscription.Customer.LastName);
            Assert.AreEqual(newCustomer.Email, newSubscription.Customer.Email);
            Assert.AreEqual(newCustomer.Phone, newSubscription.Customer.Phone);
            newSubscription.Customer.Organization.ShouldBe(newCustomer.Organization);
            newSubscription.Customer.SystemID.ShouldBe(referenceId);
            newSubscription.PaymentProfile.FirstName.ShouldBe(newPaymentInfo.FirstName);
            newSubscription.PaymentProfile.LastName.ShouldBe(newPaymentInfo.LastName);
            newSubscription.PaymentProfile.ExpirationMonth.ShouldBe(newPaymentInfo.ExpirationMonth);
            newSubscription.PaymentProfile.ExpirationYear.ShouldBe(newPaymentInfo.ExpirationYear);
            newSubscription.PaymentProfile.BillingAddress.ShouldBe(newPaymentInfo.BillingAddress);
            //Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress2 == newPaymentInfo.BillingAddress2);
            newSubscription.PaymentProfile.BillingCity.ShouldBe(newPaymentInfo.BillingCity);
            newSubscription.PaymentProfile.BillingCountry.ShouldBe(newPaymentInfo.BillingCountry);
            newSubscription.PaymentProfile.BillingState.ShouldBe(newPaymentInfo.BillingState);
            newSubscription.PaymentProfile.BillingZip.ShouldBe(newPaymentInfo.BillingZip);
            Assert.IsTrue(newSubscription.ProductPriceInCents == product.PriceInCents);
            Assert.IsTrue(newSubscription.ProductPrice == product.Price);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(newSubscription.ReferralCode));
            Assert.AreEqual(product.TrialInterval > 0 ? SubscriptionState.Trialing : SubscriptionState.Active, newSubscription.State);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_WithComponent(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            var referenceId = Guid.NewGuid().ToString();
            var expMonth = DateTime.Now.AddMonths(1).Month;
            var expYear = DateTime.Now.AddMonths(12).Year;
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", referenceId);
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);
            var component = Chargify.GetComponentsForProductFamily(productFamily.ID).FirstOrDefault(d => d.Value.Kind == ComponentType.Quantity_Based_Component && d.Value.Prices.Any(p => p.UnitPrice > 0m)).Value;
            Assert.IsNotNull(component, "Couldn't find any usable component.");

            // Act
            var newSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo, component.ID, 5);
            var subComponents = Chargify.GetComponentsForSubscription(newSubscription.SubscriptionID);
            var usedComponents = from c in subComponents
                                 where c.Value.ComponentID == component.ID
                                 select c;

            // Assert
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNotNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            newSubscription.Customer.FirstName.ShouldBe(newCustomer.FirstName);
            newSubscription.Customer.LastName.ShouldBe(newCustomer.LastName);
            newSubscription.Customer.Email.ShouldBe(newCustomer.Email);
            newSubscription.Customer.Organization.ShouldBe(newCustomer.Organization);
            newSubscription.Customer.SystemID.ShouldBe(referenceId);
            newSubscription.PaymentProfile.FirstName.ShouldBe(newPaymentInfo.FirstName);
            newSubscription.PaymentProfile.LastName.ShouldBe(newPaymentInfo.LastName);
            newSubscription.PaymentProfile.ExpirationMonth.ShouldBe(newPaymentInfo.ExpirationMonth);
            newSubscription.PaymentProfile.ExpirationYear.ShouldBe(newPaymentInfo.ExpirationYear);
            newSubscription.PaymentProfile.BillingAddress.ShouldBe(newPaymentInfo.BillingAddress);
            //Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress2 == newPaymentInfo.BillingAddress2);
            newSubscription.PaymentProfile.BillingCity.ShouldBe(newPaymentInfo.BillingCity);
            newSubscription.PaymentProfile.BillingCountry.ShouldBe(newPaymentInfo.BillingCountry);
            newSubscription.PaymentProfile.BillingState.ShouldBe(newPaymentInfo.BillingState);
            newSubscription.PaymentProfile.BillingZip.ShouldBe(newPaymentInfo.BillingZip);
            usedComponents.Count().ShouldBe(1);
            Assert.IsTrue(usedComponents.FirstOrDefault().Value.AllocatedQuantity == 5);

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_WithTwoComponents(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var referenceId = Guid.NewGuid().ToString();
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", referenceId);
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);
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
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
            Assert.IsNotNull(newSubscription);
            Assert.IsNotNull(newSubscription.Customer);
            Assert.IsNotNull(newSubscription.PaymentProfile);
            Assert.IsTrue(newSubscription.SubscriptionID > int.MinValue);
            Assert.IsTrue(newSubscription.Customer.ChargifyID > int.MinValue);
            newSubscription.Customer.FirstName.ShouldBe(newCustomer.FirstName);
            newSubscription.Customer.LastName.ShouldBe(newCustomer.LastName);
            newSubscription.Customer.Email.ShouldBe(newCustomer.Email);
            newSubscription.Customer.Organization.ShouldBe(newCustomer.Organization);
            newSubscription.Customer.SystemID.ShouldBe(referenceId);
            newSubscription.PaymentProfile.FirstName.ShouldBe(newPaymentInfo.FirstName);
            newSubscription.PaymentProfile.LastName.ShouldBe(newPaymentInfo.LastName);
            newSubscription.PaymentProfile.ExpirationMonth.ShouldBe(newPaymentInfo.ExpirationMonth);
            newSubscription.PaymentProfile.ExpirationYear.ShouldBe(newPaymentInfo.ExpirationYear);
            newSubscription.PaymentProfile.BillingAddress.ShouldBe(newPaymentInfo.BillingAddress);
            //Assert.IsTrue(newSubscription.PaymentProfile.BillingAddress2 == newPaymentInfo.BillingAddress2);
            newSubscription.PaymentProfile.BillingCity.ShouldBe(newPaymentInfo.BillingCity);
            newSubscription.PaymentProfile.BillingCountry.ShouldBe(newPaymentInfo.BillingCountry);
            newSubscription.PaymentProfile.BillingState.ShouldBe(newPaymentInfo.BillingState);
            newSubscription.PaymentProfile.BillingZip.ShouldBe(newPaymentInfo.BillingZip);
            usedComponents.Count().ShouldBe(componentsToUse.Count);
            foreach (var component in usedComponents)
            {
                Assert.IsTrue(componentsToUse.ContainsKey(component.Key));
            }

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_WithCouponAfterSignup(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", Guid.NewGuid().ToString());
            var newPaymentInfo = SubscriptionTests.GetTestPaymentMethod(newCustomer);
            const string CouponCode = "68C8FDBA";

            // Act
            var createdSubscription = Chargify.CreateSubscription(product.Handle, newCustomer, newPaymentInfo);

            // Assert
            Assert.IsNotNull(createdSubscription);
            Assert.IsInstanceOfType(createdSubscription, typeof(Subscription));
            Assert.IsTrue(createdSubscription.CouponCode == string.Empty);

            // Act Again
            var updatedSubscription = Chargify.AddCoupon(createdSubscription.SubscriptionID, CouponCode);

            // Assert Again
            Assert.IsNotNull(updatedSubscription);
            Assert.IsInstanceOfType(updatedSubscription, typeof(ISubscription));
            Assert.IsTrue(updatedSubscription.CouponCode == CouponCode);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_UpdateBillingDate(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            var billingDate = subscription.NextAssessmentAt;

            // Act
            var updatedSubscription = Chargify.UpdateBillingDateForSubscription(subscription.SubscriptionID, billingDate.AddDays(5));

            // Assert
            billingDate.AddDays(5).ShouldBe(updatedSubscription.NextAssessmentAt);

            // Cleanup
            var restoredSubscription = Chargify.UpdateBillingDateForSubscription(updatedSubscription.SubscriptionID, billingDate);
            Assert.IsTrue(billingDate == restoredSubscription.NextAssessmentAt);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Can_create_delayed_cancel(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            var existingSubscription = Chargify.GetSubscriptionList().Values.FirstOrDefault(s => s.State == SubscriptionState.Active && s.PaymentProfile != null && s.PaymentProfile.Id > 0) as Subscription;
            ValidateRun(() => existingSubscription != null, "No applicable subscription found.");
            ValidateRun(() => existingSubscription.PaymentProfile.Id > 0, "No payment profile found");

            var newSubscription = Chargify.CreateSubscription(existingSubscription.Product.Handle, existingSubscription.Customer.ToCustomerAttributes(), DateTime.MinValue, existingSubscription.PaymentProfile.Id);
            ValidateRun(() => newSubscription != null, "No new subscription was created. Cannot test cancellation");

            var updatedSubscription = Chargify.UpdateDelayedCancelForSubscription(newSubscription.SubscriptionID, true, "Testing Delayed Cancel");

            Assert.IsTrue(updatedSubscription.CancelAtEndOfPeriod);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Can_undo_delayed_cancel(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            var existingSubscription = Chargify.GetSubscriptionList().Values.FirstOrDefault(s => s.State == SubscriptionState.Active && s.PaymentProfile != null && s.PaymentProfile.Id > 0) as Subscription;
            ValidateRun(() => existingSubscription != null, "No applicable subscription found.");
            ValidateRun(() => existingSubscription.PaymentProfile.Id > 0, "No payment profile found");

            var newSubscription = Chargify.CreateSubscription(existingSubscription.Product.Handle, existingSubscription.Customer.ToCustomerAttributes(), DateTime.MinValue, existingSubscription.PaymentProfile.Id);
            ValidateRun(() => newSubscription != null, "No new subscription was created. Cannot test cancellation");

            var cancelledSubscription = Chargify.UpdateDelayedCancelForSubscription(newSubscription.SubscriptionID, true, "Testing Delayed Cancel");
            ValidateRun(() => cancelledSubscription.CancelAtEndOfPeriod, "Subscription is not cancelled at end of period. No opportunity to test uncancel");

            var updatedSubscription = Chargify.UpdateDelayedCancelForSubscription(cancelledSubscription.SubscriptionID,
                false, "Testing Undo Delayed Cancel");

            Assert.IsFalse(updatedSubscription.CancelAtEndOfPeriod);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Chargify_exception_is_thrown_when_setting_delayed_cancel_of_invalid_subscription_to_true(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);
            try
            {
                Chargify.UpdateDelayedCancelForSubscription(GetRandomNegativeInt(), true, "No subscription exists by this number");
                Assert.Fail("Expected ChargifyException was not thrown");
            }
            catch (ChargifyException exception)
            {
                Assert.AreEqual("Subscription not found", exception.ErrorMessages.First().Message);
            }
            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Chargify_exception_is_thrown_when_setting_delayed_cancel_of_invalid_subscription_to_false(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);
            try
            {
                Chargify.UpdateDelayedCancelForSubscription(GetRandomNegativeInt(), false, "No subscription exists by this number");
                Assert.Fail("Expected ChargifyException was not thrown");
            }
            catch (ChargifyException exception)
            {
                Assert.AreEqual("Subscription not found", exception.ErrorMessages.First().Message);
            }
            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod, Ignore]
        public void Subscription_Update(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.PaymentProfile != null).Value as Subscription;
            var originalEmail = subscription.Customer.Email;
            subscription.Customer.Email = "newemail@testing.com";

            // Act
            var updatedSubscription = Chargify.Save<Subscription>(subscription);

            // Assert
            Assert.IsNotNull(updatedSubscription);
            Assert.IsNotNull(updatedSubscription.Customer);
            updatedSubscription.Customer.Email.ShouldBe(subscription.Customer.Email);

            // Cleanup
            updatedSubscription.Customer.Email = originalEmail;
            var restoredSubscription = Chargify.Save<Subscription>(updatedSubscription);
            restoredSubscription.Customer.Email.ShouldBe(updatedSubscription.Customer.Email);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod, Ignore]
        public void Subscription_Load_Where_State_Is_TrialEnded(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Trial_Ended).Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");

            // Act
            var retreivedSubscription = Chargify.Find<Subscription>(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(retreivedSubscription);
            Assert.IsTrue(retreivedSubscription.State == SubscriptionState.Trial_Ended);
            Assert.IsInstanceOfType(retreivedSubscription, typeof(Subscription));

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Load_ComponentsFor(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");

            // Act
            var subscriptionComponents = Chargify.GetComponentsForSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(subscriptionComponents);
            Assert.IsTrue(subscriptionComponents.Count > 0);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Subscription_Create_Using_ExistingProfile(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var existingSubscription = Chargify.GetSubscriptionList().Values.FirstOrDefault(s => s.State == SubscriptionState.Active && s.PaymentProfile != null && s.PaymentProfile.Id > 0) as Subscription;
            Assert.IsNotNull(existingSubscription, "No applicable subscription found.");
            Assert.IsTrue(existingSubscription.PaymentProfile.Id > 0);

            // Act
            var newSubscription = Chargify.CreateSubscription(existingSubscription.Product.Handle, existingSubscription.Customer.ToCustomerAttributes(), DateTime.MinValue, existingSubscription.PaymentProfile.Id);

            // Assert
            Assert.IsNotNull(newSubscription);
            Assert.AreEqual(existingSubscription.PaymentProfile.Id, newSubscription.PaymentProfile.Id);

            SetJson(!isJson);
        }
        #endregion

        private static CreditCardAttributes GetTestPaymentMethod(CustomerAttributes customer)
        {
            var retVal = new CreditCardAttributes()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                ExpirationMonth = DateTime.Now.AddMonths(1).Month,
                ExpirationYear = DateTime.Now.AddYears(1).Year,
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
