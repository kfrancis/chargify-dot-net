
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
    using System.Xml;
    using ChargifyNET.Json;
    using System.Web;
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
        internal const string ReferenceKey = "reference";
        internal const string IDKey = "id";
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
        public CustomerAttributes() : base()
        {
        }

        /// <summary>
        /// Constructor.  Values specified
        /// </summary>
        /// <param name="FirstName">The customer's first name</param>
        /// <param name="LastName">The customer's last name</param>
        /// <param name="Email">The customer's email address</param>
        /// <param name="Organization">The customer's organization</param>
        /// <param name="SystemID">The customer's system ID</param>
        public CustomerAttributes(string FirstName, string LastName, string Email, string Organization, string SystemID) : base()
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Organization = Organization;
            this.SystemID = SystemID;
        }

        /// <summary>
        /// Constructor.  Values specified
        /// </summary>
        /// <param name="FirstName">The customer's first name</param>
        /// <param name="LastName">The customer's last name</param>
        /// <param name="Email">The customer's email address</param>
        /// <param name="Phone">The customer's phone number</param>
        /// <param name="Organization">The customer's organization</param>
        /// <param name="SystemID">The customer's system ID</param>
        public CustomerAttributes(string FirstName, string LastName, string Email, string Phone, string Organization, string SystemID)
            : base()
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Phone = Phone;
            this.Organization = Organization;
            this.SystemID = SystemID;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CustomerAttributesXML">XML containing customer info (in expected format)</param>
        public CustomerAttributes(string CustomerAttributesXML)
            : base()
        { 
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(CustomerAttributesXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "CustomerAttributesXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                switch (elementNode.Name)
                {
                    case "customer_attributes":
                    case "customer":
                        this.LoadFromNode(elementNode);
                        break;
                    default:
                        break;
                }
            }

            return;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerAttributesNode">XML node containing customer info (in expected format)</param>
        internal CustomerAttributes(XmlNode customerAttributesNode) : base()
        {
            if (customerAttributesNode == null) throw new ArgumentNullException("CustomerAttributesNode");
            if (customerAttributesNode.Name != "customer_attributes") throw new ArgumentException("Not a vaild customer attributes node", "customerAttributesNode");
            if (customerAttributesNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "customerAttributesNode");
            this.LoadFromNode(customerAttributesNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerAttributesObject">JsonObject containing customer info (in expected format)</param>
        public CustomerAttributes(JsonObject customerAttributesObject)
            : base()
        {
            if (customerAttributesObject == null) throw new ArgumentNullException("customerAttributesObject");
            if (customerAttributesObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild customer attributes object", "customerAttributesObject");
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing customer attribute data</param>
        private void LoadFromJSON(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case FirstNameKey:
                        this.FirstName = obj.GetJSONContentAsString(key);
                        break;
                    case LastNameKey:
                        this.LastName = obj.GetJSONContentAsString(key);
                        break;
                    case EmailKey:
                        this.Email = obj.GetJSONContentAsString(key);
                        break;
                    case PhoneKey:
                        this.Phone = obj.GetJSONContentAsString(key);
                        break;
                    case OrganizationKey:
                        this.Organization = HttpUtility.HtmlDecode(obj.GetJSONContentAsString(key));
                        break;
                    case ReferenceKey:
                        _systemID = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingAddressKey:
                        this.ShippingAddress = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingAddress2Key:
                        this.ShippingAddress2 = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingCityKey:
                        this.ShippingCity = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingStateKey:
                        this.ShippingState = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingZipKey:
                        this.ShippingZip = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingCountryKey:
                        this.ShippingCountry = obj.GetJSONContentAsString(key);
                        break;
                    default:
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
                        this.FirstName = dataNode.GetNodeContentAsString();
                        break;
                    case LastNameKey:
                        this.LastName = dataNode.GetNodeContentAsString();
                        break;
                    case EmailKey:
                        this.Email = dataNode.GetNodeContentAsString();
                        break;
                    case PhoneKey:
                        this.Phone = dataNode.GetNodeContentAsString();
                        break;
                    case OrganizationKey:
                        this.Organization = HttpUtility.HtmlDecode(dataNode.GetNodeContentAsString());
                        break;
                    case ReferenceKey:
                        _systemID = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingAddressKey:
                        this.ShippingAddress = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingAddress2Key:
                        this.ShippingAddress2 = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingCityKey:
                        this.ShippingCity = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingStateKey:
                        this.ShippingState = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingZipKey:
                        this.ShippingZip = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingCountryKey:
                        this.ShippingCountry = dataNode.GetNodeContentAsString();
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region ICustomerAttribute Members

        /// <summary>
        /// Get or set the customer's first name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Get or set the customer's last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Get or set the customer's email address
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The customer's phone number
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Get or set the customer's organization
        /// </summary>
        public string Organization { get; set; }
        /// <summary>
        /// The customers shipping address
        /// </summary>
        public string ShippingAddress { get; set; }
        /// <summary>
        /// The customers shipping address 2
        /// </summary>
        public string ShippingAddress2 { get; set; }
        /// <summary>
        /// The customers shipping city
        /// </summary>
        public string ShippingCity { get; set; }
        /// <summary>
        /// The customers shipping zip/postal code
        /// </summary>
        public string ShippingZip { get; set; }
        /// <summary>
        /// The customers shipping state
        /// </summary>
        public string ShippingState { get; set; }
        /// <summary>
        /// The customers shipping country
        /// </summary>
        public string ShippingCountry { get; set; }
        /// <summary>
        /// Get or set the customer's ID in the calling system
        /// </summary>
        public string SystemID
        {
            get
            {
                return _systemID;
            }
            set
            {
                _systemID = value;
            }
        }
        private string _systemID = string.Empty;

        /// <summary>
        /// Get the full name LastName FirstName for the customer
        /// </summary>
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.LastName, this.FirstName).Trim();
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
            return this.FullName.CompareTo(other.FullName);
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
            return this.FullName.CompareTo(other.FullName);
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Returns the string equivalent of the CustomerAttribute object.
        /// </summary>
        /// <returns>The full name of the customer</returns>
        public override string ToString()
        {
            return this.FullName;
        }

        #endregion
    }
}
