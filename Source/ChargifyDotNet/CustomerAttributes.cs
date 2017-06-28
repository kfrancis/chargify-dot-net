
#region License, Terms and Conditions
//
// CustomerAttributes.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2010 Clinical Support Systems, Inc. All rights reserved.
// 
//  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW:
//
//  Permission is hereby granted, free of charge, to any person obtaining a
//  copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation
//  the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the
//  Software is furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//  DEALINGS IN THE SOFTWARE.
//
#endregion

namespace ChargifyNET
{
    #region Imports
    using System;
    using System.Diagnostics;
    using System.Web;
    using System.Xml;
    using System.Xml.Serialization;
    using Json;
    #endregion

    /// <summary>
    /// Class representing basic attributes for a customer
    /// </summary>
    [DebuggerDisplay("Full Name: {FullName}, SystemID: {SystemID}")]
    [Serializable]
    public class CustomerAttributes : ChargifyBase, ICustomerAttributes, IComparable<CustomerAttributes>
    {
        #region Field Keys
        internal const string FirstNameKey = "first_name";
        internal const string LastNameKey = "last_name";
        internal const string EmailKey = "email";
        internal const string PhoneKey = "phone";
        internal const string OrganizationKey = "organization";
        internal const string VatNumberKey = "vat_number";
        internal const string ReferenceKey = "reference";
        internal const string IdKey = "id";
        internal const string CreatedAtKey = "created_at";
        internal const string UpdatedAtKey = "updated_at";
        internal const string ShippingAddressKey = "address";
        internal const string ShippingAddress2Key = "address_2";
        internal const string ShippingCityKey = "city";
        internal const string ShippingStateKey = "state";
        internal const string ShippingZipKey = "zip";
        internal const string ShippingCountryKey = "country";
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public CustomerAttributes()
        {
        }

        /// <summary>
        /// Constructor.  Values specified
        /// </summary>
        /// <param name="firstName">The customer's first name</param>
        /// <param name="lastName">The customer's last name</param>
        /// <param name="email">The customer's email address</param>
        /// <param name="organization">The customer's organization</param>
        /// <param name="systemId">The customer's system ID</param>
        public CustomerAttributes(string firstName, string lastName, string email, string organization, string systemId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Organization = organization;
            SystemID = systemId;
        }

        /// <summary>
        /// Constructor.  Values specified
        /// </summary>
        /// <param name="firstName">The customer's first name</param>
        /// <param name="lastName">The customer's last name</param>
        /// <param name="email">The customer's email address</param>
        /// <param name="phone">The customer's phone number</param>
        /// <param name="organization">The customer's organization</param>
        /// <param name="systemId">The customer's system ID</param>
        public CustomerAttributes(string firstName, string lastName, string email, string phone, string organization, string systemId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Organization = organization;
            SystemID = systemId;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerAttributesXml">XML containing customer info (in expected format)</param>
        public CustomerAttributes(string customerAttributesXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(customerAttributesXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(customerAttributesXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                switch (elementNode.Name)
                {
                    case "customer_attributes":
                    case "customer":
                        LoadFromNode(elementNode);
                        break;
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerAttributesNode">XML node containing customer info (in expected format)</param>
        internal CustomerAttributes(XmlNode customerAttributesNode)
        {
            if (customerAttributesNode == null) throw new ArgumentNullException(nameof(customerAttributesNode));
            if (customerAttributesNode.Name != "customer_attributes") throw new ArgumentException("Not a vaild customer attributes node", nameof(customerAttributesNode));
            if (customerAttributesNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(customerAttributesNode));
            LoadFromNode(customerAttributesNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerAttributesObject">JsonObject containing customer info (in expected format)</param>
        public CustomerAttributes(JsonObject customerAttributesObject)
        {
            if (customerAttributesObject == null) throw new ArgumentNullException(nameof(customerAttributesObject));
            if (customerAttributesObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild customer attributes object", nameof(customerAttributesObject));
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing customer attribute data</param>
        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case FirstNameKey:
                        FirstName = obj.GetJSONContentAsString(key);
                        break;
                    case LastNameKey:
                        LastName = obj.GetJSONContentAsString(key);
                        break;
                    case EmailKey:
                        Email = obj.GetJSONContentAsString(key);
                        break;
                    case PhoneKey:
                        Phone = obj.GetJSONContentAsString(key);
                        break;
                    case OrganizationKey:
                        Organization = PCLWebUtility.WebUtility.HtmlDecode(obj.GetJSONContentAsString(key));
                        break;
                    case ReferenceKey:
                        _systemId = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingAddressKey:
                        ShippingAddress = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingAddress2Key:
                        ShippingAddress2 = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingCityKey:
                        ShippingCity = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingStateKey:
                        ShippingState = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingZipKey:
                        ShippingZip = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingCountryKey:
                        ShippingCountry = obj.GetJSONContentAsString(key);
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a customer node
        /// </summary>
        /// <param name="customerNode">The customer node</param>
        private void LoadFromNode(XmlNode customerNode)
        {
            foreach (XmlNode dataNode in customerNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case FirstNameKey:
                        FirstName = dataNode.GetNodeContentAsString();
                        break;
                    case LastNameKey:
                        LastName = dataNode.GetNodeContentAsString();
                        break;
                    case EmailKey:
                        Email = dataNode.GetNodeContentAsString();
                        break;
                    case PhoneKey:
                        Phone = dataNode.GetNodeContentAsString();
                        break;
                    case OrganizationKey:
                        Organization = PCLWebUtility.WebUtility.HtmlDecode(dataNode.GetNodeContentAsString());
                        break;
                    case ReferenceKey:
                        _systemId = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingAddressKey:
                        ShippingAddress = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingAddress2Key:
                        ShippingAddress2 = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingCityKey:
                        ShippingCity = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingStateKey:
                        ShippingState = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingZipKey:
                        ShippingZip = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingCountryKey:
                        ShippingCountry = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }

        #endregion

        #region ICustomerAttribute Members

        /// <summary>
        /// Get or set the customer's first name
        /// </summary>
        [XmlElement("first_name")]
        public string FirstName { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeFirstName()
        {
            return !string.IsNullOrEmpty(FirstName);
        }

        /// <summary>
        /// Get or set the customer's last name
        /// </summary>
        [XmlElement("last_name")]
        public string LastName { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeLastName()
        {
            return !string.IsNullOrEmpty(LastName);
        }

        /// <summary>
        /// Get or set the customer's email address
        /// </summary>
        [XmlElement("email")]
        public string Email { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeEmail()
        {
            return !string.IsNullOrEmpty(Email);
        }

        /// <summary>
        /// The customer's phone number
        /// </summary>
        [XmlElement("phone")]
        public string Phone { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializePhone()
        {
            return !string.IsNullOrEmpty(Phone);
        }

        /// <summary>
        /// Get or set the customer's organization
        /// </summary>
        [XmlElement("organization")]
        public string Organization { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeOrganization()
        {
            return !string.IsNullOrEmpty(Organization);
        }

        /// <summary>
        /// The customers vat number
        /// </summary>
        [XmlIgnore]
        public string VatNumber { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeVatNumber()
        {
            return !string.IsNullOrEmpty(VatNumber);
        }

        /// <summary>
        /// The customers shipping address
        /// </summary>
        [XmlElement("address")]
        public string ShippingAddress { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeShippingAddress()
        {
            return !string.IsNullOrEmpty(ShippingAddress);
        }

        /// <summary>
        /// The customers shipping address 2
        /// </summary>
        [XmlElement("address_2")]
        public string ShippingAddress2 { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeShippingAddress2()
        {
            return !string.IsNullOrEmpty(ShippingAddress2);
        }

        /// <summary>
        /// The customers shipping city
        /// </summary>
        [XmlElement("city")]
        public string ShippingCity { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeShippingCity2()
        {
            return !string.IsNullOrEmpty(ShippingCity);
        }

        /// <summary>
        /// The customers shipping zip/postal code
        /// </summary>
        [XmlElement("zip")]
        public string ShippingZip { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeShippingZip()
        {
            return !string.IsNullOrEmpty(ShippingZip);
        }

        /// <summary>
        /// The customers shipping state
        /// </summary>
        [XmlElement("state")]
        public string ShippingState { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeShippingState()
        {
            return !string.IsNullOrEmpty(ShippingState);
        }

        /// <summary>
        /// The customers shipping country
        /// </summary>
        [XmlElement("country")]
        public string ShippingCountry { get; set; }
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeShippingCountry()
        {
            return !string.IsNullOrEmpty(ShippingCountry);
        }

        /// <summary>
        /// Get or set the customer's ID in the calling system
        /// </summary>
        [XmlElement("reference")]
        public string SystemID
        {
            get
            {
                return _systemId;
            }
            set
            {
                _systemId = value;
            }
        }
        private string _systemId = string.Empty;
        /// <summary>
        /// Ignore, used for determining if the value should be serialized
        /// </summary>
        public bool ShouldSerializeSystemId()
        {
            return !string.IsNullOrEmpty(SystemID);
        }

        /// <summary>
        /// Get the full name LastName FirstName for the customer
        /// </summary>
        [XmlIgnore]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", LastName, FirstName).Trim();
            }
        }

        #endregion

        #region IComparable<ICustomer> Members

        /// <summary>
        /// Compare this instance to another (by FullName)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(ICustomerAttributes other)
        {
            return string.Compare(FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region IComparable<Customer> Members

        /// <summary>
        /// Compare this instance to another (by FullName)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(CustomerAttributes other)
        {
            return string.Compare(FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Returns the string equivalent of the CustomerAttribute object.
        /// </summary>
        /// <returns>The full name of the customer</returns>
        public override string ToString()
        {
            return FullName;
        }

        #endregion
    }
}
