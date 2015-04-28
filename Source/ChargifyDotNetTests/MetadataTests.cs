using System;
using System.Collections.Generic;
using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyNET;
#if NUNIT
using NUnit.Framework;
using System.Diagnostics;
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
    public class MetadataTests : ChargifyTestBase
    {
        [Test]
        public void Metadata_Can_List_Customer_XML()
        {
            // Act
            var result = Chargify.GetMetadata<Customer>();

            // Assert
            Assert.IsNotNull(result);           
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);
        }

        [Test]
        public void Metadata_Can_List_Customer_JSON()
        {
            // Arrange
            SetJson(true);

            // Act
            var result = Chargify.GetMetadata<Customer>();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);

            SetJson(false);
        }

        [Test]
        public void Metadata_Can_List_Subscription_XML()
        {
            // Act
            var result = Chargify.GetMetadata<Subscription>();

            // Assert
            Assert.IsNotNull(result);     
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);
        }

        [Test]
        public void Metadata_Can_List_Subscription_JSON()
        {
            // Arrange
            SetJson(true);

            // Act
            var result = Chargify.GetMetadata<Subscription>();

            // Assert
            Assert.IsNotNull(result);    
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);
        
            SetJson(false);
        }

        [Test]
        public void Metadata_Can_Read_Subscription()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");
#if !NUNIT
            TestContext.WriteLine("SubscriptionID: {0}", subscription.SubscriptionID);
#else
            Trace.WriteLine(string.Format("SubscriptionID: {0}", subscription.SubscriptionID));
#endif
            
            // Act
            var result = Chargify.GetMetadataFor<Subscription>(subscription.SubscriptionID, null);

            // Assert
            Assert.IsNotNull(result);    
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);
            Assert.AreEqual(result.TotalCount, result.Metadata.Count);
        }

        [Test]
        public void Metadata_Can_Read_Customer()
        {
            // Arrange
            var customer = Chargify.GetCustomerList().FirstOrDefault().Value as Customer;
            Assert.IsNotNull(customer, "No applicable customer found.");
            
            // Act
            var result = Chargify.GetMetadataFor<Customer>(customer.ChargifyID, null);

            // Assert
            Assert.IsNotNull(result);    
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);
            Assert.AreEqual(result.TotalCount, result.Metadata.Count);
        }

        [Test]
        public void Metadata_Can_Update()
        {
            
        }

        [Test]
        public void Metadata_Can_Set_Customers_Single()
        {
            // Arrange
            var customer = Chargify.GetCustomerList().FirstOrDefault().Value as Customer;
            Assert.IsNotNull(customer, "No applicable customer found.");
            Metadata metadata = new Metadata() { Name = Guid.NewGuid().ToString(), Value = Guid.NewGuid().ToString() };
            
            // Act
            var result = Chargify.SetMetadataFor<Customer>(customer.ChargifyID, metadata);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 1);
            Assert.IsTrue(result.Select(m => m.Name == metadata.Name).Count() == 1);
            Assert.IsTrue(result.Select(m => m.Value == metadata.Value).Count() == 1);
        }

        [Test]
        public void Metadata_Can_Set_Customers_List()
        {
            // Arrange
            var customer = Chargify.GetCustomerList().FirstOrDefault().Value as Customer;
            Assert.IsNotNull(customer, "No applicable customer found.");
            var metadata = new List<Metadata>()
            {
                new Metadata() { Name = "one", Value = Guid.NewGuid().ToString() },
                new Metadata() { Name = "two", Value = Guid.NewGuid().ToString() },
                new Metadata() { Name = "three", Value = Guid.NewGuid().ToString() }
            };
            
            // Act
            var result = Chargify.SetMetadataFor<Customer>(customer.ChargifyID, metadata);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(metadata.Count, result.Count);
        }

        [Test]
        public void Metadata_Can_Set_Subscription_Single()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");
            Metadata metadata = new Metadata() { Name = Guid.NewGuid().ToString(), Value = Guid.NewGuid().ToString() };
            
            // Act
            var result = Chargify.SetMetadataFor<Subscription>(subscription.SubscriptionID, metadata);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 1);
            Assert.IsTrue(result.Select(m => m.Name == metadata.Name).Count() == 1);
            Assert.IsTrue(result.Select(m => m.Value == metadata.Value).Count() == 1);
        }

        [Test]
        public void Metadata_Can_Set_Subscription_List()
        {
            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");
            var metadata = new List<Metadata>()
            {
                new Metadata() { Name = "one", Value = Guid.NewGuid().ToString() },
                new Metadata() { Name = "two", Value = Guid.NewGuid().ToString() },
                new Metadata() { Name = "three", Value = Guid.NewGuid().ToString() }
            };
            
            // Act
            var result = Chargify.SetMetadataFor<Subscription>(subscription.SubscriptionID, metadata);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(metadata.Count, result.Count);
        }
    }
}
