using System;
using System.Collections.Generic;
using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyDotNet;
using System.Diagnostics;
using ChargifyDotNet.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class MetadataTests : ChargifyTestBase
    {
        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_List_Customer(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Act
            var result = Chargify.GetMetadata<Customer>();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(int.MinValue, result.CurrentPage);
            Assert.AreNotEqual(int.MinValue, result.PerPage);
            Assert.AreNotEqual(int.MinValue, result.TotalCount);
            Assert.AreNotEqual(int.MinValue, result.TotalPages);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_List_Subscription(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Act
            var result = Chargify.GetMetadata<Subscription>();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(int.MinValue, result.CurrentPage);
            Assert.AreNotEqual(int.MinValue, result.PerPage);
            Assert.AreNotEqual(int.MinValue, result.TotalCount);
            Assert.AreNotEqual(int.MinValue, result.TotalPages);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Read_Specific_Subscription(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().Values.FirstOrDefault(s => Chargify.GetMetadataFor<Subscription>(s.SubscriptionID, null).TotalCount > 0) as Subscription;
            if (subscription == null) Assert.Inconclusive("No valid subscription with metadata found for this test.");
            SetJson(true);

            var result = Chargify.GetMetadataFor<Subscription>(subscription.SubscriptionID, null);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(int.MinValue, result.CurrentPage);
            Assert.AreNotEqual(int.MinValue, result.PerPage);
            Assert.AreNotEqual(int.MinValue, result.TotalCount);
            Assert.AreNotEqual(int.MinValue, result.TotalPages);
            Assert.HasCount(result.TotalCount, result.Metadata);
            Assert.IsTrue(result.Metadata.Any(m => !string.IsNullOrEmpty(m.Name)));

            TestContext.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Read_Subscription(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).FirstOrDefault().Value as Subscription;
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
            Assert.AreNotEqual(int.MinValue, result.CurrentPage);
            Assert.AreNotEqual(int.MinValue, result.PerPage);
            Assert.AreNotEqual(int.MinValue, result.TotalCount);
            Assert.AreNotEqual(int.MinValue, result.TotalPages);
            Assert.HasCount(result.TotalCount, result.Metadata);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Read_Customer(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var customer = Chargify.GetCustomerList().FirstOrDefault().Value as Customer;
            Assert.IsNotNull(customer, "No applicable customer found.");

            // Act
            var result = Chargify.GetMetadataFor<Customer>(customer.ChargifyID, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(int.MinValue, result.CurrentPage);
            Assert.AreNotEqual(int.MinValue, result.PerPage);
            Assert.AreNotEqual(int.MinValue, result.TotalCount);
            Assert.AreNotEqual(int.MinValue, result.TotalPages);
            Assert.HasCount(result.TotalCount, result.Metadata);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod, Ignore]
        public void Metadata_Can_Update(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // something?

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Set_Customers_Single(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var customer = Chargify.GetCustomerList().FirstOrDefault().Value as Customer;
            Assert.IsNotNull(customer, "No applicable customer found.");
            Metadata metadata = new() { Name = Guid.NewGuid().ToString(), Value = Guid.NewGuid().ToString() };

            // Act
            var result = Chargify.SetMetadataFor<Customer>(customer.ChargifyID, metadata);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsGreaterThanOrEqualTo(1, result.Count);
            Assert.AreEqual(1, result.Select(m => m.Name == metadata.Name).Count());
            Assert.AreEqual(1, result.Select(m => m.Value == metadata.Value).Count());

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Set_Customers_List(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var customer = Chargify.GetCustomerList().FirstOrDefault().Value as Customer;
            Assert.IsNotNull(customer, "No applicable customer found.");
            var metadata = new List<Metadata>()
            {
                new() { Name = "one", Value = Guid.NewGuid().ToString() },
                new() { Name = "two", Value = Guid.NewGuid().ToString() },
                new() { Name = "three", Value = Guid.NewGuid().ToString() }
            };

            // Act
            var result = Chargify.SetMetadataFor<Customer>(customer.ChargifyID, metadata);

            // Assert
            Assert.IsNotNull(result);
            Assert.HasCount(metadata.Count, result);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Set_Subscription_Single(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).FirstOrDefault().Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");
            Metadata metadata = new() { Name = Guid.NewGuid().ToString(), Value = Guid.NewGuid().ToString() };

            // Act
            var result = Chargify.SetMetadataFor<Subscription>(subscription.SubscriptionID, metadata);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsGreaterThanOrEqualTo(1, result.Count);
            Assert.AreEqual(1, result.Select(m => m.Name == metadata.Name).Count());
            Assert.AreEqual(1, result.Select(m => m.Value == metadata.Value).Count());

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Set_Subscription_List(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).FirstOrDefault().Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");
            var metadata = new List<Metadata>()
            {
                new() { Name = "one", Value = Guid.NewGuid().ToString() },
                new() { Name = "two", Value = Guid.NewGuid().ToString() },
                new() { Name = "three", Value = Guid.NewGuid().ToString() }
            };

            // Act
            var result = Chargify.SetMetadataFor<Subscription>(subscription.SubscriptionID, metadata);

            // Assert
            Assert.IsNotNull(result);
            Assert.HasCount(metadata.Count, result);

            SetJson(!isJson);
        }
    }
}
