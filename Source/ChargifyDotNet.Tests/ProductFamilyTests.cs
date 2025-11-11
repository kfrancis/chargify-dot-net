using System;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using System.Linq;
using ChargifyDotNet.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class ProductFamilyTests : ChargifyTestBase
    {
        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void ProductFamily_Can_Create_Simple(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var newFamily = new ProductFamily()
            {
                Name = string.Format("Test{0}", Guid.NewGuid().ToString())
            };

            // Act
            var result = Chargify.CreateProductFamily(newFamily);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IProductFamily));
            Assert.AreEqual(newFamily.Name, result.Name, "Name didn't match");
            Assert.AreEqual(newFamily.Name.ToLowerInvariant(), result.Handle, "Handle wasn't as expected");

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void ProductFamily_Can_Create_Complex(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var newFamily = new ProductFamily()
            {
                Name = string.Format("Test{0}", Guid.NewGuid().ToString()),
                Description = Guid.NewGuid().ToString(),
                AccountingCode = Guid.NewGuid().ToString(),
                Handle = Guid.NewGuid().ToString()
            };

            // Act
            var result = Chargify.CreateProductFamily(newFamily);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IProductFamily));
            Assert.AreEqual(newFamily.Name, result.Name, "Name didn't match");
            Assert.AreEqual(newFamily.Description, result.Description, "Description didn't match");
            Assert.AreEqual(newFamily.Handle, result.Handle, "Handle didn't match");
            //Assert.AreEqual(newFamily.AccountingCode, result.AccountingCode, "Accounting Code didn't match");

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void ProductFamily_Can_Get_Listing(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Act
            var result = Chargify.GetProductFamilyList();

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Dictionary<int, IProductFamily>));
            Assert.IsNotEmpty(result);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void ProductFamily_Can_Retrieve_ByHandle(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var familyListing = Chargify.GetProductFamilyList();

            // Act
            var result = Chargify.LoadProductFamily(familyListing.Values.FirstOrDefault().Handle);

            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IProductFamily));
            Assert.AreEqual(familyListing.Values.FirstOrDefault().ID, result.ID, "IDs didn't match");

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void ProductFamily_Can_Retrieve_ByID(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var familyListing = Chargify.GetProductFamilyList();

            // Act
            var result = Chargify.LoadProductFamily(familyListing.Values.FirstOrDefault().ID);

            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IProductFamily));
            Assert.AreEqual(familyListing.Values.FirstOrDefault().ID, result.ID, "IDs didn't match");

            SetJson(!isJson);
        }
    }
}
