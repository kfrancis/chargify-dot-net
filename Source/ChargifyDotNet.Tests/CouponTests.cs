using System;
using System.Diagnostics;
using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class CouponTests : ChargifyTestBase
    {
        [TestMethod]
        public void Coupon_Read()
        {
            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");

            // Act
            var result = Chargify.LoadCoupon(productFamily.ID, 129307);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(ICoupon));
            Assert.IsTrue(result.AmountInCents != int.MinValue);
            Assert.IsTrue(result.AmountInCents > 0);
            Assert.IsTrue(result.Amount == (Convert.ToDecimal(result.AmountInCents)/100));
        }
        [TestMethod]
        public void Coupon_Create()
        {
            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            string couponCode = Guid.NewGuid().ToString().Replace("-",string.Empty).ToUpperInvariant().Substring(0, 8);
            var newCoupon = new Coupon()
            {
                AllowNegativeBalance = false,
                Code = couponCode,
                Description = couponCode + " description",
                DurationPeriodCount = 2,
                EndDate = DateTime.Today.AddDays(5),
                IsRecurring = true,
                Name = "coupon " + couponCode,
                Percentage = 50,
                AmountInCents = 0
            };

            // Act
            var createdCoupon = Chargify.CreateCoupon(newCoupon, productFamily.ID);

            // Assert
            Assert.IsNotNull(createdCoupon);
            //Assert.IsInstanceOfType(createdCoupon, typeof(ICoupon));
            Assert.IsTrue(createdCoupon.AllowNegativeBalance == newCoupon.AllowNegativeBalance);
            Assert.IsTrue(createdCoupon.Name == newCoupon.Name);
            Assert.IsTrue(createdCoupon.Description == newCoupon.Description);
            Assert.IsTrue(createdCoupon.Code == newCoupon.Code);
        }

        [TestMethod]
        public void Coupon_Update()
        {
            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            var foundCoupon = Chargify.GetAllCoupons(productFamily.ID).FirstOrDefault().Value;
            if (foundCoupon == null) Assert.Inconclusive("A valid coupon could not be found.");
            string originalName = foundCoupon.Name;
            foundCoupon.Name = originalName + "_1";

            // Act
            var updatedCoupon = Chargify.UpdateCoupon(foundCoupon);

            // Assert
            Assert.IsNotNull(updatedCoupon);
            //Assert.IsInstanceOfType(updatedCoupon, typeof(ICoupon));
            Assert.IsTrue(updatedCoupon.Name == originalName + "_1");

            // Cleanup
            updatedCoupon.Name = originalName;
            var restoredCoupon = Chargify.UpdateCoupon(updatedCoupon);
            Assert.IsTrue(restoredCoupon.Name == originalName);
        }

        [TestMethod]
        public void Coupon_Remove()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && !string.IsNullOrEmpty(s.Value.CouponCode)).Value as Subscription;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");

            // Act
            var result = Chargify.RemoveCoupon(subscription.SubscriptionID, subscription.CouponCode);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
