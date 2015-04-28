using System;
using System.Collections.Generic;
using System.Linq;
using ChargifyDotNetTests.Base;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class CustomerTests : ChargifyTestBase
    {
        [TestMethod]
        public void Customer_CreateCustomer()
        {
            // Arrange
            string referenceID = Guid.NewGuid().ToString();
            var customer = new Customer() 
            { 
                FirstName = "Scott",
                LastName = "Pilgrim",
                Email = "demonhead_sucks@scottpilgrim.com",
                Phone = "123-456-7890",
                Organization = "Chargify",
                SystemID = referenceID,
                ShippingAddress = "Address Line 1",
                ShippingAddress2 = "Address Line 2",
                ShippingCity = "New York",
                ShippingState = "New York",
                ShippingZip = "10001",
                ShippingCountry = "US"
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
            Assert.IsTrue(createdCustomer.SystemID == customer.SystemID);
            Assert.IsTrue(createdCustomer.FirstName == customer.FirstName);
            Assert.IsTrue(createdCustomer.LastName == customer.LastName);
            Assert.IsTrue(createdCustomer.Organization == customer.Organization);
            Assert.IsTrue(createdCustomer.Email == customer.Email);
            Assert.IsTrue(createdCustomer.Phone == customer.Phone);
            Assert.IsTrue(createdCustomer.ShippingAddress == customer.ShippingAddress);
            Assert.IsTrue(createdCustomer.ShippingAddress2 == customer.ShippingAddress2);
            Assert.IsTrue(createdCustomer.ShippingCity == customer.ShippingCity);
            Assert.IsTrue(createdCustomer.ShippingState == customer.ShippingState);
            Assert.IsTrue(createdCustomer.ShippingZip == customer.ShippingZip);
            Assert.IsTrue(createdCustomer.ShippingCountry == customer.ShippingCountry);

            // Can't cleanup, Chargify doesn't support customer deletions
        }

        [TestMethod]
        public void Customer_OddCharactersForOrganization()
        {
            // Arrange
            string referenceID = Guid.NewGuid().ToString();
            var customer = new Customer() 
            { 
                FirstName = "Scott",
                LastName = "Pilgrim",
                Email = "demonhead_sucks@scottpilgrim.com",
                Phone = "123-456-7890",
                Organization = "Chargify&",
                SystemID = referenceID,
                ShippingAddress = "Address Line 1",
                ShippingAddress2 = "Address Line 2",
                ShippingCity = "New York",
                ShippingState = "New York",
                ShippingZip = "10001",
                ShippingCountry = "US"
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
            Assert.IsTrue(createdCustomer.Organization == customer.Organization);
        }

        [TestMethod]
        public void Customer_ReadSingleCustomer()
        {
            var customers = Chargify.GetCustomerList().Keys;
            // Load test customer via reference value (ASP.NET Membership ID)
            var referenceValue = customers.FirstOrDefault(systemID => !string.IsNullOrWhiteSpace(systemID));
            ICustomer customer = Chargify.LoadCustomer(referenceValue);
            Assert.IsNotNull(customer);
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.FirstName) == false);
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.LastName) == false);
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.SystemID) == false);
            Assert.AreEqual(referenceValue, customer.SystemID);

            ICustomer customer1 = Chargify.LoadCustomer(customer.ChargifyID);
            Assert.IsNotNull(customer1);
            Assert.AreEqual(customer.FirstName, customer1.FirstName);
            Assert.AreEqual(customer.LastName, customer1.LastName);
            Assert.AreEqual(customer.SystemID, customer1.SystemID);
        }

        /// <summary>
        /// Get the list of customers
        /// </summary>
        [TestMethod]
        public void Customer_ListCustomers()
        {
            IDictionary<string, ICustomer> customerList = Chargify.GetCustomerList();
            Assert.IsNotNull(customerList);
            Assert.AreNotEqual(0, customerList.Count);
        }

        [TestMethod]
        public void Customer_UpdateCustomer()
        {
            // Load our test customer
            var customers = Chargify.GetCustomerList().Keys;
            // Load test customer via reference value (ASP.NET Membership ID)
            var referenceValue = customers.FirstOrDefault(systemID => !string.IsNullOrWhiteSpace(systemID));
            ICustomer customer = Chargify.LoadCustomer(referenceValue);
            Assert.IsNotNull(customer);

            // Change the customer's "Organization" property
            var oldAddress1 = customer.ShippingAddress;
            var oldAddress2 = customer.ShippingAddress2;
            var newAddress1 = Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpperInvariant();
            var newAddress2 = Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpperInvariant();
            customer.Organization = "TestOrganization";
            customer.ShippingAddress = newAddress1;
            customer.ShippingAddress2 = newAddress2;
            Assert.AreEqual("TestOrganization", customer.Organization);

            // Update the customer and check the result
            ICustomer result = Chargify.UpdateCustomer(customer);
            Assert.IsNotNull(result);
            Assert.AreEqual("TestOrganization", result.Organization);
            Assert.AreEqual(newAddress1, result.ShippingAddress);
            Assert.AreEqual(newAddress2, result.ShippingAddress2);

            // Set it back to "nothing" again
            customer.Organization = string.Empty;
            customer.ShippingAddress = oldAddress1;
            customer.ShippingAddress2 = oldAddress2;
            ICustomer result1 = Chargify.UpdateCustomer(customer);
            Assert.IsNotNull(result1);
            Assert.AreEqual(string.Empty, result1.Organization);
            Assert.AreEqual(oldAddress1, result1.ShippingAddress);
            Assert.AreEqual(oldAddress2, result1.ShippingAddress2);
        }
    }
}
