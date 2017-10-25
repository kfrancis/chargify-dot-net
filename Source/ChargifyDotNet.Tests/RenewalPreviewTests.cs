using System;
using ChargifyNET;
using ChargifyDotNetTests.Base;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class RenewalPreviewTests : ChargifyTestBase
    {
        [TestMethod]
        public void RenewalPreview_CanCreate()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            Assert.IsNotNull(subscription, "No suitable subscription could be found.");

            // Act
            var result = Chargify.PreviewRenewal(subscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IRenewalDetails));
            Assert.AreNotEqual(DateTime.MinValue, result.NextAssessmentAt);
            Assert.AreNotEqual(int.MinValue, result.ExistingBalanceInCents);
            Assert.AreNotEqual(int.MinValue, result.SubtotalInCents);
            Assert.AreNotEqual(int.MinValue, result.TotalDiscountInCents);
            Assert.AreNotEqual(int.MinValue, result.TotalTaxInCents);
            Assert.AreNotEqual(int.MinValue, result.TotalInCents);
            Assert.AreNotEqual(int.MinValue, result.TotalAmountDueInCents);
            Assert.IsTrue(result.UncalculatedTaxes == true || result.UncalculatedTaxes == false);
            Assert.IsNotNull(result.LineItems);
            Assert.IsTrue(result.LineItems.Any());
        }
    }
}
