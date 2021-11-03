using System;
using ChargifyNET;
using ChargifyDotNetTests.Base;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class SubscriptionPreviewTests : ChargifyTestBase
    {
        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void SubscriptionPreview_CanCreate(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            Assert.IsNotNull(subscription, "No suitable subscription could be found.");
            var options = new SubscriptionCreateOptions()
            {
                ProductHandle = subscription.Product.Handle,
                CustomerAttributes = subscription.Customer.ToCustomerAttributes() as CustomerAttributes,
                CreditCardAttributes = new CreditCardAttributes() { FullNumber = "1", ExpirationMonth = 1, ExpirationYear = DateTime.Now.Year + 1 }
            };

            // Act
            var result = Chargify.CreateSubscriptionPreview(options);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ISubscriptionPreview));
            Assert.AreEqual(1, result.SubscriptionPreviewResult.CurrentBillingManifest.LineItems.Count);

            SetJson(!isJson);
        }
    }
}
