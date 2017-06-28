
#region License, Terms and Conditions
//
// Customer.cs
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
    using Json;
    #endregion

    /// <summary>
    /// Class representing a chargify customer
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Full Name: {FullName}, SystemID: {SystemID}, ChargifyID: {ChargifyID}")]
    public class Customer : CustomerAttributes, ICustomer, IComparable<Customer>
    {
        #region Constructors

        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Customer()
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
        public Customer(string firstName, string lastName, string email, string organization, string systemId)
            : base(firstName, lastName, email, organization, systemId)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerXml">XML containing customer info (in expected format)</param>
        public Customer(string customerXml) : base(customerXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(customerXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(customerXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "customer")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no customer info was found
            throw new ArgumentException("XML does not contain customer information", nameof(customerXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerNode">XML containing customer info (in expected format)</param>
        internal Customer(XmlNode customerNode)
        {
            if (customerNode == null) throw new ArgumentNullException(nameof(customerNode));
            if (customerNode.Name != "customer") throw new ArgumentException("Not a vaild customer node", nameof(customerNode));
            if (customerNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(customerNode));
            LoadFromNode(customerNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerObject">JsonObject containing customer info (in expected format)</param>
        public Customer(JsonObject customerObject)
        {
            if (customerObject == null) throw new ArgumentNullException(nameof(customerObject));
            if (customerObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild customer object", nameof(customerObject));
            LoadFromJSON(customerObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing customer data</param>
        private void LoadFromJSON(JsonObject obj)
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
                        SystemID = obj.GetJSONContentAsString(key);
                        break;
                    case IdKey:
                        _chargifyId = obj.GetJSONContentAsInt(key);
                        break;
                    case CreatedAtKey:
                        _created = obj.GetJSONContentAsDateTime(key);
                        break;
                    case UpdatedAtKey:
                        _lastUpdated = obj.GetJSONContentAsDateTime(key);
                        break;
                    case ShippingAddressKey:
                        ShippingAddress = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingAddress2Key:
                        ShippingAddress2 = obj.GetJSONContentAsString(key);
                        break;
                    case ShippingCountryKey:
                        ShippingCountry = obj.GetJSONContentAsString(key);
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
                    case VatNumberKey:
                        VatNumber = obj.GetJSONContentAsString(key);
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
                        SystemID = dataNode.GetNodeContentAsString();
                        break;
                    case IdKey:
                        _chargifyId = dataNode.GetNodeContentAsInt();
                        break;
                    case CreatedAtKey:
                        _created = dataNode.GetNodeContentAsDateTime();
                        break;
                    case UpdatedAtKey:
                        _lastUpdated = dataNode.GetNodeContentAsDateTime();
                        break;
                    case ShippingAddressKey:
                        ShippingAddress = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingAddress2Key:
                        ShippingAddress2 = dataNode.GetNodeContentAsString();
                        break;
                    case ShippingCountryKey:
                        ShippingCountry = dataNode.GetNodeContentAsString();
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
                    case VatNumberKey:
                        VatNumber = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }

        #endregion

        #region ICustomer Members

        /// <summary>
        /// Get the customer's chargify ID
        /// </summary>
        public int ChargifyID
        {
            get
            {
                return _chargifyId;
            }
        }
        private int _chargifyId = int.MinValue;
        /// <summary>
        /// Get the date and time the customer was created a Chargify
        /// </summary>
        public DateTime Created
        {
            get
            {
                return _created;
            }
        }
        private DateTime _created = DateTime.MinValue;
        /// <summary>
        /// Get the date and time the customer was last updated at chargify
        /// </summary>
        public DateTime LastUpdated
        {
            get
            {
                return _lastUpdated;
            }
        }
        private DateTime _lastUpdated = DateTime.MinValue;

        /// <summary>
        /// Get a boolean value that indicates whether or not this customer is currently saved
        /// in the Chargify system
        /// </summary>
        public bool IsSaved
        {
            get
            {
                return !(ChargifyID == int.MinValue);
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Equals operator for two customers
        /// </summary>
        /// <returns>True if the Customers are equal</returns>
        public static bool operator ==(Customer a, Customer b)
        {
            // If both are null or both are the same instance, return true.
            if (ReferenceEquals(a, b)) return true;

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            // Return true if the fields match.
            return a.ChargifyID == b.ChargifyID &&
                   a.Created == b.Created &&
                   a.Email == b.Email &&
                   a.FirstName == b.FirstName &&
                   a.LastName == b.LastName &&
                   a.Organization == b.Organization &&
                   a.ShippingAddress == b.ShippingAddress &&
                   a.ShippingAddress2 == b.ShippingAddress2 &&
                   a.ShippingCity == b.ShippingCity &&
                   a.ShippingCountry == b.ShippingCountry &&
                   a.ShippingState == b.ShippingState &&
                   a.ShippingZip == b.ShippingZip &&
                   a.SystemID == b.SystemID;
        }

        /// <summary>
        /// Equals operator for two customers
        /// </summary>
        /// <returns>True if the Customers are equal</returns>
        public static bool operator ==(Customer a, ICustomer b)
        {
            // If both are null or both are the same instance, return true.
            if (ReferenceEquals(a, b)) return true;

            // If one is null, but not both, return false.
            if (((object) a == null) || (b == null))
            {
                return false;
            }

            // Return true if the fields match.
            return a.ChargifyID == b.ChargifyID &&
                   a.Created == b.Created &&
                   a.Email == b.Email &&
                   a.FirstName == b.FirstName &&
                   a.LastName == b.LastName &&
                   a.Organization == b.Organization &&
                   a.ShippingAddress == b.ShippingAddress &&
                   a.ShippingAddress2 == b.ShippingAddress2 &&
                   a.ShippingCity == b.ShippingCity &&
                   a.ShippingCountry == b.ShippingCountry &&
                   a.ShippingState == b.ShippingState &&
                   a.ShippingZip == b.ShippingZip &&
                   a.SystemID == b.SystemID;
        }

        /// <summary>
        /// Equals operator for two customers
        /// </summary>
        /// <returns>True if the Customers are equal</returns>
        public static bool operator ==(ICustomer a, Customer b)
        {
            // If both are null or both are the same instance, return true.
            if (ReferenceEquals(a, b)) return true;

            // If one is null, but not both, return false.
            if ((a == null) || ((object) b == null))
            {
                return false;
            }

            // Return true if the fields match.
            return a.ChargifyID == b.ChargifyID &&
                   a.Created == b.Created &&
                   a.Email == b.Email &&
                   a.FirstName == b.FirstName &&
                   a.LastName == b.LastName &&
                   a.Organization == b.Organization &&
                   a.ShippingAddress == b.ShippingAddress &&
                   a.ShippingAddress2 == b.ShippingAddress2 &&
                   a.ShippingCity == b.ShippingCity &&
                   a.ShippingCountry == b.ShippingCountry &&
                   a.ShippingState == b.ShippingState &&
                   a.ShippingZip == b.ShippingZip &&
                   a.SystemID == b.SystemID;
        }

        /// <summary>
        /// Not Equals operator for two customers
        /// </summary>
        /// <returns>True if the Customers are not equal</returns>
        public static bool operator !=(Customer a, Customer b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Not Equals operator for two customers
        /// </summary>
        /// <returns>True if the Customers are not equal</returns>
        public static bool operator !=(Customer a, ICustomer b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Not Equals operator for two customers
        /// </summary>
        /// <returns>True if the Customers are not equal</returns>
        public static bool operator !=(ICustomer a, Customer b)
        {
            return !(a == b);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Get Hash code
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        public override bool Equals(object obj)
        {
            // If both are null or both are the same instance, return true.
            if (ReferenceEquals(this, obj)) return true;

            // If one is null, but not both, return false.
            if (obj == null)
            {
                return false;
            }

            // Return true if the fields match.
            return ChargifyID == ((Customer) obj).ChargifyID &&
                   Created == ((Customer) obj).Created &&
                   Email == ((Customer) obj).Email &&
                   FirstName == ((Customer) obj).FirstName &&
                   LastName == ((Customer) obj).LastName &&
                   Organization == ((Customer) obj).Organization &&
                   ShippingAddress == ((Customer) obj).ShippingAddress &&
                   ShippingAddress2 == ((Customer) obj).ShippingAddress2 &&
                   ShippingCity == ((Customer) obj).ShippingCity &&
                   ShippingCountry == ((Customer) obj).ShippingCountry &&
                   ShippingState == ((Customer) obj).ShippingState &&
                   ShippingZip == ((Customer) obj).ShippingZip &&
                   SystemID == ((Customer) obj).SystemID;
        }

        /// <summary>
        /// Convert object to a string
        /// </summary>
        /// <returns>The string representation of the object</returns>
        public override string ToString()
        {
            return FullName;
        }

        #endregion

        #region IComparable<ICustomer> Members

        /// <summary>
        /// Compare this instance to another (by FullName)
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(ICustomer other)
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
        public int CompareTo(Customer other)
        {
            return string.Compare(FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}
