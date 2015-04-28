using System;
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

            // Act
            var result = Chargify.LoadCoupon(productFamily.ID, 3809);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ICoupon));
            Assert.IsTrue(result.AmountInCents != int.MinValue);
            Assert.IsTrue(result.AmountInCents > 0);
            Assert.IsTrue(result.Amount == (Convert.ToDecimal(result.AmountInCents)/100));
        }
        [TestMethod]
        public void Coupon_Create()
        {
            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
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
            Assert.IsInstanceOfType(createdCoupon, typeof(ICoupon));
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
            var foundCoupon = Chargify.LoadCoupon(productFamily.ID, 3809);
            string originalName = foundCoupon.Name;
            foundCoupon.Name = originalName + "_1";

            // Act
            var updatedCoupon = Chargify.UpdateCoupon(foundCoupon);

            // Assert
            Assert.IsNotNull(updatedCoupon);
            Assert.IsInstanceOfType(updatedCoupon, typeof(ICoupon));
            Assert.IsTrue(updatedCoupon.Name == originalName + "_1");

            // Cleanup
            updatedCoupon.Name = originalName;
            var restoredCoupon = Chargify.UpdateCoupon(updatedCoupon);
            Assert.IsTrue(restoredCoupon.Name == originalName);
        }
    }
}
