using System;
using System.Collections.Generic;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class ProductFamilyTests : ChargifyTestBase
    {
        [TestMethod]
        public void ProductFamily_Can_Create_Simple()
        {
            // Arrange
            var newFamily = new ProductFamily() {
                Name = string.Format("Test{0}", Guid.NewGuid().ToString())
            };

            // Act
            var result = Chargify.CreateProductFamily(newFamily);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IProductFamily));
            Assert.AreEqual(newFamily.Name, result.Name, "Name didn't match");
            Assert.AreEqual(newFamily.Name.ToLowerInvariant(), result.Handle, "Handle wasn't as expected");
        }

        [TestMethod]
        public void ProductFamily_Can_Create_Complex()
        {
            // Arrange
            var newFamily = new ProductFamily() {
                Name = string.Format("Test{0}", Guid.NewGuid().ToString()),
                Description = Guid.NewGuid().ToString(),
                AccountingCode = Guid.NewGuid().ToString(),
                Handle = Guid.NewGuid().ToString()
            };

            // Act
            var result = Chargify.CreateProductFamily(newFamily);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IProductFamily));
            Assert.AreEqual(newFamily.Name, result.Name, "Name didn't match");
            Assert.AreEqual(newFamily.Description, result.Description, "Description didn't match");
            Assert.AreEqual(newFamily.Handle, result.Handle, "Handle didn't match");
            //Assert.AreEqual(newFamily.AccountingCode, result.AccountingCode, "Accounting Code didn't match");
        }

        [TestMethod]
        public void ProductFamily_Can_Get_Listing()
        {
            // Act
            var result = Chargify.GetProductFamilyList();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Dictionary<int, IProductFamily>));
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void ProductFamily_Can_Retrieve_ByHandle()
        {
            // Arrange
            var familyListing = Chargify.GetProductFamilyList();

            // Act
            var result = Chargify.LoadProductFamily(familyListing.Values.FirstOrDefault().Handle);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IProductFamily));
            Assert.AreEqual(familyListing.Values.FirstOrDefault().ID, result.ID, "IDs didn't match");
        }

        [TestMethod]
        public void ProductFamily_Can_Retrieve_ByID()
        {
            // Arrange
            var familyListing = Chargify.GetProductFamilyList();

            // Act
            var result = Chargify.LoadProductFamily(familyListing.Values.FirstOrDefault().ID);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IProductFamily));
            Assert.AreEqual(familyListing.Values.FirstOrDefault().ID, result.ID, "IDs didn't match");
        }
    }
}
