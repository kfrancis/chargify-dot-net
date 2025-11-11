using System;
using System.Diagnostics;
using System.Linq;
using ChargifyDotNet.Tests;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class CouponTests : ChargifyTestBase
    {
        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Coupon_Read(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            var coupons = Chargify.GetAllCoupons(productFamily.ID);
            var couponID = coupons.FirstOrDefault(c => c.Value.AmountInCents > 0).Key;

            // Act
            var result = Chargify.LoadCoupon(productFamily.ID, couponID);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AmountInCents != int.MinValue);
            Assert.IsTrue(result.AmountInCents > 0);
            Assert.IsTrue(result.Amount == (Convert.ToDecimal(result.AmountInCents) / 100));
            Assert.AreEqual(result.ID, couponID);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Coupon_Read_Percentage(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            var coupons = Chargify.GetAllCoupons(productFamily.ID);
            var coupon = coupons.FirstOrDefault(c => c.Value.Percentage > 0).Value;
            var couponID = coupon.ID;

            // Act
            var result = Chargify.LoadCoupon(productFamily.ID, couponID);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Percentage != decimal.MinValue);
            Assert.IsTrue(result.Percentage > 0);
            Assert.AreEqual(coupon.Percentage, result.Percentage);
            Assert.AreEqual(result.ID, couponID);

            TestContext.WriteLine($"Coupon percentage: {coupon.Percentage}");

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Coupon_Create(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            var couponCode = Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpperInvariant().Substring(0, 8);
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

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Coupon_Update(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            if (productFamily == null) Assert.Inconclusive("A valid product family could not be found.");
            var foundCoupon = Chargify.GetAllCoupons(productFamily.ID).FirstOrDefault().Value;
            if (foundCoupon == null) Assert.Inconclusive("A valid coupon could not be found.");
            var originalName = foundCoupon.Name;
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

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Coupon_Remove(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active && !string.IsNullOrEmpty(s.Value.CouponCode)).Value as Subscription;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");

            // Act
            var result = Chargify.RemoveCoupon(subscription.SubscriptionID, subscription.CouponCode);

            // Assert
            Assert.IsTrue(result);

            SetJson(!isJson);
        }
    }
}
