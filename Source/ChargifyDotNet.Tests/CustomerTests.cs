﻿using System;
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
        private const string StringWithNonUnicodeCharacters = "ÄäËëÏïÖöÜüŸß";
        private const string StringWithAmpersand = "Foo&Bar";

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_CreateWithError(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName()
            };

            // Act
            try
            {
                Chargify.CreateCustomer(customer);
                Assert.Fail("Error was expected, but not received");
            }
            catch (ChargifyException chEx)
            {
                Assert.IsNotNull(chEx.ErrorMessages);
                Assert.AreEqual(2, chEx.ErrorMessages.Count);
                Assert.IsTrue(chEx.ErrorMessages.Any(e => e.Message == "Last name: cannot be blank."));
                Assert.IsTrue(chEx.ErrorMessages.Any(e => e.Message == "Email address: cannot be blank."));
            }

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_CreateCustomer(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = Faker.Company.CompanyName(),
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US",
                TaxExempt = true
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            //Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
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
            Assert.IsTrue(createdCustomer.TaxExempt);

            Chargify.DeleteCustomer(createdCustomer.ChargifyID);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Can_revoke_billing_portal_access(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = Faker.Company.CompanyName(),
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US",
                TaxExempt = true
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);
            Chargify.RevokeBillingPortalAccess(createdCustomer.ChargifyID);
            // Assert

            try
            {
                Chargify.GetManagementLink(createdCustomer.ChargifyID);
                Assert.Fail("Error was expected, but not received");
            }
            catch (ChargifyException chEx)
            {
                Assert.IsNotNull(chEx.ErrorMessages);
                Assert.AreEqual(1, chEx.ErrorMessages.Count);
                Assert.IsTrue(chEx.ErrorMessages.Any(e => e.Message.Contains("Billing Portal")), $"Found '{string.Join(", ", chEx.ErrorMessages.Select(x => x.Message))}'");
                //todo: Need to run test to find out the exact error message
            }

            Chargify.DeleteCustomer(createdCustomer.ChargifyID);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Can_enable_billing_portal_access(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = Faker.Company.CompanyName(),
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US",
                TaxExempt = true
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);
            Chargify.RevokeBillingPortalAccess(createdCustomer.ChargifyID);
            Chargify.EnableBillingPortalAccess(createdCustomer.ChargifyID);

            // Assert
            var managementLink = Chargify.GetManagementLink(createdCustomer.ChargifyID);
            Assert.IsNotNull(managementLink);


            Chargify.DeleteCustomer(createdCustomer.ChargifyID);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Can_revoke_billing_portal_access_by_system_id(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = Faker.Company.CompanyName(),
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US",
                TaxExempt = true
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);
            Chargify.RevokeBillingPortalAccess(createdCustomer.SystemID);
            // Assert

            try
            {
                Chargify.GetManagementLink(createdCustomer.ChargifyID);
                Assert.Fail("Error was expected, but not received");
            }
            catch (ChargifyException chEx)
            {
                Assert.IsNotNull(chEx.ErrorMessages);
                Assert.AreEqual(1, chEx.ErrorMessages.Count);
                Assert.IsTrue(chEx.ErrorMessages.Any(e => e.Message.Contains("Billing Portal")));
                //todo: Need to run test to find out the exact error message
            }

            Chargify.DeleteCustomer(createdCustomer.ChargifyID);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Can_enable_billing_portal_access_by_system_Id(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = Faker.Company.CompanyName(),
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US",
                TaxExempt = true
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);
            Chargify.RevokeBillingPortalAccess(createdCustomer.ChargifyID);
            Chargify.EnableBillingPortalAccess(createdCustomer.SystemID);

            // Assert
            var managementLink = Chargify.GetManagementLink(createdCustomer.ChargifyID);
            Assert.IsNotNull(managementLink);


            Chargify.DeleteCustomer(createdCustomer.ChargifyID);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_AmpersandForOrganization(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = "Chargify&",
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US"
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            //Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
            Assert.IsTrue(createdCustomer.Organization == customer.Organization);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_NonUnicodeForOrganization(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = "Chargify&" + StringWithNonUnicodeCharacters,
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US"
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            //Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
            Assert.IsTrue(createdCustomer.Organization == customer.Organization);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_AmpersandCharactersForFirstName(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = StringWithAmpersand,
                LastName = Faker.Name.LastName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = "",
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US"
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            //Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
            Assert.IsTrue(createdCustomer.FirstName == customer.FirstName);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_NonUnicodeCharactersForFirstName(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = StringWithNonUnicodeCharacters,
                LastName = Faker.Name.LastName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = "",
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US"
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            //Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
            Assert.IsTrue(createdCustomer.FirstName == customer.FirstName);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_AmpersandCharactersForLastName(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = StringWithAmpersand,
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = "",
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US"
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            //Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
            Assert.IsTrue(createdCustomer.FirstName == customer.FirstName);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_NonUnicodeCharactersForLastName(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = StringWithNonUnicodeCharacters,
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Organization = "",
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US"
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            //Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
            Assert.IsTrue(createdCustomer.FirstName == customer.FirstName);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_NonUnicodeCharactersForEmailAddress(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var referenceID = Guid.NewGuid().ToString();
            var customer = new Customer()
            {
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                Email = $"{StringWithNonUnicodeCharacters}@gmail.com",
                Phone = Faker.Phone.PhoneNumber(),
                Organization = "",
                SystemID = referenceID,
                ShippingAddress = Faker.Address.StreetAddress(false),
                ShippingAddress2 = Faker.Address.SecondaryAddress(),
                ShippingCity = Faker.Address.City(),
                ShippingState = Faker.Address.StateAbbr(),
                ShippingZip = Faker.Address.ZipCode(),
                ShippingCountry = "US"
            };

            // Act
            var createdCustomer = Chargify.CreateCustomer(customer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            //Assert.IsInstanceOfType(createdCustomer, typeof(Customer));
            Assert.IsTrue(createdCustomer.FirstName == customer.FirstName);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_ReadSingleCustomer(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            var customers = Chargify.GetCustomerList().Keys;
            // Load test customer via reference value (ASP.NET Membership ID)
            var referenceValue = customers.FirstOrDefault(systemID => !string.IsNullOrWhiteSpace(systemID));
            var customer = Chargify.LoadCustomer(referenceValue);
            Assert.IsNotNull(customer);
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.FirstName) == false);
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.LastName) == false);
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.SystemID) == false);
            Assert.AreEqual(referenceValue, customer.SystemID);

            var customer1 = Chargify.LoadCustomer(customer.ChargifyID);
            Assert.IsNotNull(customer1);
            Assert.AreEqual(customer.FirstName, customer1.FirstName);
            Assert.AreEqual(customer.LastName, customer1.LastName);
            Assert.AreEqual(customer.SystemID, customer1.SystemID);

            SetJson(!isJson);
        }

        /// <summary>
        /// Get the list of customers
        /// </summary>
        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_ListCustomers(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            var customerList = Chargify.GetCustomerList();
            Assert.IsNotNull(customerList);
            Assert.AreNotEqual(0, customerList.Count);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_Update_Nothing(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var customers = Chargify.GetCustomerList().Keys;
            var referenceValue = customers.FirstOrDefault(systemID => !string.IsNullOrWhiteSpace(systemID));
            var customer = Chargify.LoadCustomer(referenceValue);

            // Act
            var updatedCustomer = Chargify.UpdateCustomer(customer);

            // Assert
            Assert.IsNotNull(updatedCustomer);

            SetJson(!isJson);
        }

        [DataTestMethod]
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Customer_UpdateCustomer(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Load our test customer
            var customers = Chargify.GetCustomerList().Keys;
            // Load test customer via reference value (ASP.NET Membership ID)
            var referenceValue = customers.FirstOrDefault(systemID => !string.IsNullOrWhiteSpace(systemID));
            var customer = Chargify.LoadCustomer(referenceValue);
            Assert.IsNotNull(customer);

            // Change the customer's "Organization" property
            var oldAddress1 = customer.ShippingAddress;
            var oldAddress2 = customer.ShippingAddress2;
            var newAddress1 = Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpperInvariant();
            var newAddress2 = Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpperInvariant();
            var company = Faker.Company.CompanyName();
            customer.Organization = company;
            customer.ShippingAddress = newAddress1;
            customer.ShippingAddress2 = newAddress2;
            Assert.AreEqual(company, customer.Organization);

            // Update the customer and check the result
            var result = Chargify.UpdateCustomer(customer);
            Assert.IsNotNull(result);
            Assert.AreEqual(company, result.Organization);
            Assert.AreEqual(newAddress1, result.ShippingAddress);
            Assert.AreEqual(newAddress2, result.ShippingAddress2);

            // Set it back to "nothing" again
            customer.Organization = string.Empty;
            customer.ShippingAddress = oldAddress1;
            customer.ShippingAddress2 = oldAddress2;
            var result1 = Chargify.UpdateCustomer(customer);
            Assert.IsNotNull(result1);
            Assert.AreEqual(string.Empty, result1.Organization);
            Assert.AreEqual(oldAddress1, result1.ShippingAddress);
            Assert.AreEqual(oldAddress2, result1.ShippingAddress2);

            SetJson(!isJson);
        }
    }
}
