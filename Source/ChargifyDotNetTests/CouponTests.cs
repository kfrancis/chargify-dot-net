using System;
using System.Diagnostics;
using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyNET;
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
    public class CouponTests : ChargifyTestBase
    {
        [Test]
        public void Coupon_Read()
        {
            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            Assert.IsNotNull(productFamily, "No valid product family found.");

            // Act
            var result = Chargify.LoadCoupon(productFamily.ID, 129307);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(ICoupon));
            Assert.IsTrue(result.AmountInCents != int.MinValue);
            Assert.IsTrue(result.AmountInCents > 0);
            Assert.IsTrue(result.Amount == (Convert.ToDecimal(result.AmountInCents)/100));
        }
        [Test]
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
            //Assert.IsInstanceOfType(createdCoupon, typeof(ICoupon));
            Assert.IsTrue(createdCoupon.AllowNegativeBalance == newCoupon.AllowNegativeBalance);
            Assert.IsTrue(createdCoupon.Name == newCoupon.Name);
            Assert.IsTrue(createdCoupon.Description == newCoupon.Description);
            Assert.IsTrue(createdCoupon.Code == newCoupon.Code);
        }

        [Test]
        public void Coupon_Update()
        {
            // Arrange
            var productFamily = Chargify.GetProductFamilyList().Values.FirstOrDefault();
            Assert.IsNotNull(productFamily, "No valid product family found.");
            var foundCoupon = Chargify.LoadCoupon(productFamily.ID, 129307);
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
            Assert.IsNotNull(subscription, "No suitable subscription could be found");

            // Act
            var result = Chargify.RemoveCoupon(subscription.SubscriptionID, subscription.CouponCode);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
