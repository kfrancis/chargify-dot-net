using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chargify;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using RestSharp.Serializers;
using System.Xml.Linq;

namespace ChargifyTests
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod]
        public void Product_Serialize()
        {
            // Arrange
            Guid randomName = Guid.NewGuid();

            Product newProduct = new Product()
            {
                name = randomName.ToString(),
                price_in_cents = 100,
                interval_unit = IntervalUnit.month,
                interval = 1
            };

            // Act
            string msg = string.Empty;
            var serializer = new DotNetXmlSerializer();
            msg = serializer.Serialize(newProduct);

            // Assert
            XDocument doc = XDocument.Parse(msg);
            XElement nameElement = doc.Root.Elements().FirstOrDefault(e => e.Name == "name");
            XElement priceElement = doc.Root.Elements().FirstOrDefault(e => e.Name == "price_in_cents");
            XElement intervalUnitElement = doc.Root.Elements().FirstOrDefault(e => e.Name == "interval_unit");
            XElement intervalElement = doc.Root.Elements().FirstOrDefault(e => e.Name == "interval");

            Assert.IsTrue(doc.Root.Elements().Count() == 4);
            Assert.IsFalse(string.IsNullOrEmpty(nameElement.Value));
            Assert.IsTrue(randomName.ToString() == nameElement.Value);
            Assert.IsNotNull(priceElement);
            Assert.IsTrue(int.Parse(priceElement.Value) > int.MinValue);
            Assert.AreEqual((int)newProduct.price_in_cents, int.Parse(priceElement.Value));
            Assert.IsFalse(string.IsNullOrEmpty(intervalUnitElement.Value));
            Assert.IsTrue((int)newProduct.interval_unit == (int)Enum.Parse(typeof(IntervalUnit), intervalUnitElement.Value));
        }

        [TestMethod]
        public void Product_All()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.Products.All();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        public void Product_All_WithFamilyId()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.Products.All(464);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<Product>));
            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        public void Product_Single_WithId()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.Products.Single(4775);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Product));
        }

        [TestMethod]
        public void Product_Single_WithHandle()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.Products.Single("free");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Product));
        }

        [TestMethod]
        public void Product_Create()
        {
            // Arrange
            var chargify = new ChargifyClient();
            var newProduct = GetNewProduct();

            // Act
            var result = chargify.Products.Create(464, newProduct);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Product));
            Assert.IsTrue(newProduct.name == result.name);
            Assert.IsTrue(newProduct.price_in_cents == result.price_in_cents);
            Assert.IsTrue(newProduct.interval_unit == result.interval_unit);
            Assert.IsTrue(newProduct.interval == result.interval);
        }

        [TestMethod]
        public void Product_Archive()
        {
            // Arrange
            var chargify = new ChargifyClient();
            var newProduct = GetNewProduct();
            var createdProduct = chargify.Products.Create(464, newProduct);

            // Act
            chargify.Products.Archive(createdProduct);
            var foundProduct = chargify.Products.Single(createdProduct.id);

            // Assert
            Assert.IsNotNull(foundProduct);
            Assert.IsTrue(foundProduct.archived_at.HasValue);
            Assert.IsTrue(foundProduct.archived_at.Value != DateTime.MinValue);
        }

        public Product GetNewProduct()
        {
            Guid randomName = Guid.NewGuid();
            Product newProduct = new Product()
            {
                name = randomName.ToString(),
                price_in_cents = 100,
                interval_unit = IntervalUnit.month,
                interval = 1
            };
            return newProduct;
        }
    }
}
