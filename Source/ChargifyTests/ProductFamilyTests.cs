using System;
using System.Collections.Generic;
using Chargify;
using System.Linq;
#if NUNIT
using NUnit.Framework;
#else
using TestFixture = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using TestFixtureSetUp = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using SetUp = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
#endif

namespace ChargifyTests
{
    [TestFixture]
    public class ProductFamilyTests
    {
        [Test]
        public void ProductFamily_All()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.ProductFamilies.All();

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IEnumerable<ProductFamily>));
            Assert.IsTrue(result.Count() > 0);
        }

        [Test]
        public async Task ProductFamily_All_Async()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = await chargify.ProductFamilies.AllAsync();

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IEnumerable<ProductFamily>));
            Assert.IsTrue(result.Count() > 0);
        }
    }
}
