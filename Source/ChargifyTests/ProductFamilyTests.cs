using System;
using System.Collections.Generic;
using Chargify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ChargifyTests
{
    [TestClass]
    public class ProductFamilyTests
    {
        [TestMethod]
        public void ProductFamily_All()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.ProductFamilies.All();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<ProductFamily>));
            Assert.IsTrue(result.Count() > 0);
        }
    }
}
