using System;
using System.Collections.Generic;
using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using System.Diagnostics;
using ChargifyDotNet.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class MetadataTests : ChargifyTestBase
    {
        [DataTestMethod]
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
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);

            SetJson(!isJson);
        }

        [DataTestMethod]
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
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);

            SetJson(!isJson);
        }

        [DataTestMethod]
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
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);
            Assert.AreEqual(result.TotalCount, result.Metadata.Count);
            Assert.IsTrue(result.Metadata.Any(m => !string.IsNullOrEmpty(m.Name)));

            TestContext.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Read_Subscription(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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

            SetJson(!isJson);
        }

        [DataTestMethod]
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
            Assert.IsTrue(result.CurrentPage != int.MinValue);
            Assert.IsTrue(result.PerPage != int.MinValue);
            Assert.IsTrue(result.TotalCount != int.MinValue);
            Assert.IsTrue(result.TotalPages != int.MinValue);
            Assert.AreEqual(result.TotalCount, result.Metadata.Count);

            SetJson(!isJson);
        }

        [DataTestMethod]
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

        [DataTestMethod]
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
            Assert.IsTrue(result.Count >= 1);
            Assert.IsTrue(result.Select(m => m.Name == metadata.Name).Count() == 1);
            Assert.IsTrue(result.Select(m => m.Value == metadata.Value).Count() == 1);

            SetJson(!isJson);
        }

        [DataTestMethod]
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
                new Metadata() { Name = "one", Value = Guid.NewGuid().ToString() },
                new Metadata() { Name = "two", Value = Guid.NewGuid().ToString() },
                new Metadata() { Name = "three", Value = Guid.NewGuid().ToString() }
            };

            // Act
            var result = Chargify.SetMetadataFor<Customer>(customer.ChargifyID, metadata);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(metadata.Count, result.Count);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Set_Subscription_Single(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList().FirstOrDefault(s => s.Value.State == SubscriptionState.Active).Value as Subscription;
            Assert.IsNotNull(subscription, "No applicable subscription found.");
            Metadata metadata = new() { Name = Guid.NewGuid().ToString(), Value = Guid.NewGuid().ToString() };

            // Act
            var result = Chargify.SetMetadataFor<Subscription>(subscription.SubscriptionID, metadata);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 1);
            Assert.IsTrue(result.Select(m => m.Name == metadata.Name).Count() == 1);
            Assert.IsTrue(result.Select(m => m.Value == metadata.Value).Count() == 1);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Metadata_Can_Set_Subscription_List(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

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

            SetJson(!isJson);
        }
    }
}
