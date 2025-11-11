using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ChargifyDotNet;
using Shouldly;
using Bogus;
using System.Collections.Generic;
using Bogus.Extensions;
using ChargifyDotNet.Tests;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class ComponentTests : ChargifyTestBase
    {
        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Components_Can_Load_PriceBrackets(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            var familyComponents = Chargify.GetComponentsForProductFamily(productFamily.ID, false).Values;
            if (familyComponents == null || !familyComponents.Any()) Assert.Inconclusive("Valid components could not be found.");

            // Act
            var components = familyComponents.Where(c => c.PricingScheme != PricingSchemeType.Per_Unit && c.Prices != null && c.Prices.Count > 0).ToList();

            // Assert
            Assert.IsNotNull(components);
            //Assert.IsInstanceOfType(components, typeof(List<IComponentInfo>));
            Assert.IsTrue(components.Where(c => c.Prices.Any()).Any());
            Assert.IsTrue(components.FirstOrDefault(c => c.Prices != null && c.Prices.Count > 0).Prices.First().StartingQuantity != int.MinValue);
            Assert.IsTrue(components.FirstOrDefault(c => c.Prices != null && c.Prices.Count > 0).Prices.First().EndingQuantity != int.MinValue);
            Assert.IsTrue(components.FirstOrDefault(c => c.Prices != null && c.Prices.Count > 0).Prices.First().UnitPrice != int.MinValue);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Components_Load_ForSubscription(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");

            // Act
            var results = Chargify.GetComponentsForSubscription(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(results);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Components_AddUsage_ForSubscription(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var component = Chargify.GetComponentsForProductFamily(productFamily.ID, false).FirstOrDefault(c => c.Value.Kind == ComponentType.Metered_Component).Value;
            if (component == null) Assert.Inconclusive("A valid component could not be found.");
            var usageQuantity = 5;
            var usageDescription = "testing";

            // Act
            var usageResult = Chargify.AddUsage(subscription.SubscriptionID, component.ID, usageQuantity, usageDescription);

            // Assert
            Assert.IsNotNull(usageResult);
            //Assert.IsInstanceOfType(usageResult, typeof(IUsage));
            Assert.IsTrue(usageResult.Memo == usageDescription);
            Assert.IsTrue(usageResult.Quantity == usageQuantity);

            SetJson(!isJson);
        }

        /// <summary>
        /// For @praveen-prakash
        /// </summary>
        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Components_Create_Subscription_Multiple_Components(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            Assert.IsNotNull(product, "Product couldn't be found");
            var referenceId = Guid.NewGuid().ToString();
            var newCustomer = new CustomerAttributes("Scott", "Pilgrim", "demonhead_sucks@scottpilgrim.com", "Chargify", referenceId);
            var newPaymentInfo = ComponentTests.GetTestPaymentMethod(newCustomer);
            // Find components that allow for a simple allocated_quantity = 1 to work for this simple test
            var components = Chargify.GetComponentsForProductFamily(product.ProductFamily.ID).Values.Where(c => c.Kind == ComponentType.Quantity_Based_Component || c.Kind == ComponentType.On_Off_Component);
            var componentsToUse = components.Take(2).ToList();
            var options = new SubscriptionCreateOptions()
            {
                CustomerAttributes = newCustomer,
                CreditCardAttributes = newPaymentInfo,
                ProductHandle = product.Handle,
                Components = new System.Collections.Generic.List<ComponentDetails>
                {
                    new ComponentDetails() { ComponentID = componentsToUse.First().ID, AllocatedQuantity = 1 },
                    new ComponentDetails() { ComponentID = componentsToUse.Last().ID, AllocatedQuantity = 1 }
                }
            };

            // Act            
            var newSubscription = Chargify.CreateSubscription(options);
            var subComponents = Chargify.GetComponentsForSubscription(newSubscription.SubscriptionID);
            var usedComponents = from c in subComponents
                                 where componentsToUse.Any(x => x.ID == c.Value.ComponentID)
                                 select c;

            // Assert
            Assert.IsInstanceOfType(newSubscription, typeof(Subscription));
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
                Assert.IsTrue(componentsToUse.Any(x => x.ID == component.Key));
                //Assert.AreEqual(decimal.Parse(componentsToUse[component.Key]), component.Value.AllocatedQuantity);
            }

            // Cleanup
            Assert.IsTrue(Chargify.DeleteSubscription(newSubscription.SubscriptionID, "Automatic cancel due to test"));

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]

        [TestMethod]
        public void PricePoints_Create(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.Product.ID == 5830949).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var subscriptionComponent = Chargify.GetComponentInfoForSubscription(subscription.SubscriptionID, 1526150);

            var priceFaker = new Faker<ComponentPrice>()
                .RuleFor(c => c.ComponentId, f => subscriptionComponent.ComponentID)
                .RuleFor(c => c.StartingQuantity, f => f.Random.Number(1, 5000))
                .RuleFor(c => c.UnitPrice, f => f.Finance.Amount());

            var pricePointFaker = new Faker<ComponentPricePoint>()
                .RuleFor(c => c.Name, f => f.Lorem.Word())
                .RuleFor(c => c.PricingScheme, f => f.PickRandomWithout<PricingSchemeType>(PricingSchemeType.Unknown))
                .RuleFor(c => c.Handle, (f, c) => $"{c.Name}-handle")
                .RuleFor(c => c.ComponentId, f => subscriptionComponent.ComponentID)
                .RuleFor(c => c.RenewPrepaidAllocation, f => f.Random.Bool().OrNull(f, .95f))
                .RuleFor(c => c.ExpirationIntervalUnit, f => f.PickRandomWithout<IntervalUnit>())
                .RuleFor(c => c.ExpirationInterval, (f, c) => c.ExpirationIntervalUnit != IntervalUnit.Unknown && c.ExpirationIntervalUnit != IntervalUnit.Never ? f.Random.Number(0, 6) : int.MinValue)
                .RuleFor(c => c.RolloverPrepaidRemainder, (f, c) => c.ExpirationInterval > 0 ? true : (bool?)null)
                .RuleFor(c => c.Prices, f => priceFaker.GenerateBetween(1, f.Random.Number(2, 4)));
            //.RuleFor(c => c.OveragePricingScheme, f => f.Random.Bool(0.1f) ? f.PickRandom<PricingSchemeType>() : PricingSchemeType.Unknown)
            //.RuleFor(c => c.OveragePricingPrices, (f, c) => c.OveragePricingScheme != PricingSchemeType.Unknown ? priceFaker.GenerateBetween(1, f.Random.Number(2, 4)) : null);

            var newPricePoint = pricePointFaker.Generate(1).Single();
            newPricePoint = ComponentTests.FixPricePoints(newPricePoint);

            // Act
            var result = Chargify.CreatePricePoint(subscriptionComponent.ComponentID, newPricePoint);

            // Assert
            result.ShouldNotBeNull();
            result.ComponentId.ShouldBe(newPricePoint.ComponentId);
            result.Handle.ShouldBe(newPricePoint.Handle);
            result.Name.ShouldBe(newPricePoint.Name);
            result.PricingScheme.ShouldBe(newPricePoint.PricingScheme);
            result.Prices.Count.ShouldBe(newPricePoint.Prices.Count);

            SetJson(!isJson);
        }

        /// <summary>
        /// While we're generating random numbers, we need to fix them up to make them look reasonable.
        /// </summary>
        /// <param name="pricePoint"></param>
        /// <returns></returns>
        private static ComponentPricePoint FixPricePoints(ComponentPricePoint pricePoint)
        {
            if (pricePoint.PricingScheme == PricingSchemeType.Volume ||
                pricePoint.PricingScheme == PricingSchemeType.Tiered ||
                pricePoint.PricingScheme == PricingSchemeType.Stairstep)
            {
                if (pricePoint.Prices != null && pricePoint.Prices.Any())
                {
                    pricePoint.Prices = FixStartEndQuantities(pricePoint.Prices.ToList());
                }

                if (pricePoint.OveragePricingPrices != null && pricePoint.OveragePricingPrices.Any())
                {
                    pricePoint.OveragePricingPrices = FixStartEndQuantities(pricePoint.OveragePricingPrices.ToList());
                }
            }
            else
            {
                if (pricePoint.Prices != null && pricePoint.Prices.Any())
                {
                    pricePoint.Prices = FixStartEndQuantities(pricePoint.Prices.Take(1).ToList());
                }

                if (pricePoint.OveragePricingPrices != null && pricePoint.OveragePricingPrices.Any())
                {
                    pricePoint.OveragePricingPrices = FixStartEndQuantities(pricePoint.OveragePricingPrices.Take(1).ToList());
                }
            }

            return pricePoint;
        }

        private static List<ComponentPrice> FixStartEndQuantities(List<ComponentPrice> priceList)
        {
            var listCopy = priceList.ToList();
            for (var i = 0; i <= listCopy.Count - 1; i++)
            {
                if (i == 0)
                {
                    listCopy[i].StartingQuantity = 1;
                }
                else
                {
                    listCopy[i - 1].EndingQuantity = listCopy[i].StartingQuantity - 1;
                    listCopy[i].StartingQuantity = listCopy[i - 1].EndingQuantity + 1;
                }
            }
            return listCopy;
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void PricePoints_PromoteToDefault(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.Product.ID == 5830949).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var subscriptionComponent = Chargify.GetComponentInfoForSubscription(subscription.SubscriptionID, 1526150);

            var pricePoints = Chargify.GetPricePoints(subscriptionComponent.ComponentID).Values;

            var defaultPricePoint = pricePoints.FirstOrDefault(x => x.Default == true);
            var firstNonDefaultPricePoint = pricePoints.FirstOrDefault(x => x.Default != true);

            var result = Chargify.SetPricePointDefault(defaultPricePoint.ComponentId, firstNonDefaultPricePoint.Id);

            Assert.IsTrue(result, "Expected SetPricePointDefault to return true");

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void PricePoints_CreateBulk(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.Product.ID == 5830949).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var subscriptionComponent = Chargify.GetComponentInfoForSubscription(subscription.SubscriptionID, 1526150);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void PricePoints_Update(string method)
        {
            var isJson = method == "json";

            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.Product.ID == 5830949).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var subscriptionComponent = Chargify.GetComponentInfoForSubscription(subscription.SubscriptionID, 1526150);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void PricePoints_Archive(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.Product.ID == 5830949).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var subscriptionComponent = Chargify.GetComponentInfoForSubscription(subscription.SubscriptionID, 1526150);

            var pricePoints = Chargify.GetPricePoints(subscriptionComponent.ComponentID)?.Values.ToList();

            var defaultPricePoint = pricePoints.FirstOrDefault(x => !x.IsArchived);
            if (defaultPricePoint == null) Assert.Inconclusive("No unarchived price points to unarchive.");

            var result = Chargify.ArchivePricePoint(subscriptionComponent.ComponentID, defaultPricePoint.Id);

            result.ShouldNotBeNull();
            result.ArchivedAt.ShouldNotBeNull();
            result.IsArchived.ShouldBeTrue();

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void PricePoints_Read(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.Product.ID == 5830949).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var subscriptionComponent = Chargify.GetComponentInfoForSubscription(subscription.SubscriptionID, 1526150);

            // Act
            var result = Chargify.GetPricePoints(subscriptionComponent.ComponentID);

            result.ShouldNotBeNull();
            result.ShouldAllBe(x => x.Value.ComponentId == subscriptionComponent.ComponentID);
            result.Values.Count.ShouldBeGreaterThan(0);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void PricePoints_Unarchive(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.Product.ID == 5830949).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var subscriptionComponent = Chargify.GetComponentInfoForSubscription(subscription.SubscriptionID, 1526150);

            var pricePoints = Chargify.GetPricePoints(subscriptionComponent.ComponentID)?.Values.ToList();

            var defaultPricePoint = pricePoints.FirstOrDefault(x => x.IsArchived);
            if (defaultPricePoint == null) Assert.Inconclusive("No archived price points to unarchive.");

            var result = Chargify.UnarchivePricePoint(subscriptionComponent.ComponentID, defaultPricePoint.Id);

            result.ShouldNotBeNull();
            result.ArchivedAt.ShouldBeNull();
            result.IsArchived.ShouldBeFalse();

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void PricePoints_UpdatePrices(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.Product.ID == 5830949).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var subscriptionComponent = Chargify.GetComponentInfoForSubscription(subscription.SubscriptionID, 1526150);

            var pricePoints = Chargify.GetPricePoints(subscriptionComponent.ComponentID)?.Values.ToList();
            var firstPricePoint = pricePoints.FirstOrDefault();
            if (firstPricePoint == null) Assert.Inconclusive();

            var firstPrice = firstPricePoint.Prices.FirstOrDefault();
            firstPrice.UnitPrice += 1;

            //Chargify.UpdatePriceInPricePoint(subscriptionComponent.ComponentID, firstPrice);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void PricePoints_CreatePrices(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && s.Value.Product.ID == 5830949).Value;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var subscriptionComponent = Chargify.GetComponentInfoForSubscription(subscription.SubscriptionID, 1526150);

            SetJson(!isJson);
        }

        private static CreditCardAttributes GetTestPaymentMethod(CustomerAttributes customer)
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
