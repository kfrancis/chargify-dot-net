
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
    using ChargifyNET.Json;
    #endregion

    /// <summary>
    /// Class representing a chargify customer
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Full Name: {FullName}, SystemID: {SystemID}, ChargifyID: {ChargifyID}")]
    public class Customer :  CustomerAttributes, ICustomer, IComparable<Customer>
    {
        #region Constructors

        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Customer() : base()
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
        public Customer(string FirstName, string LastName, string Email, string Organization, string SystemID) 
            : base(FirstName, LastName, Email, Organization, SystemID)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CustomerXML">XML containing customer info (in expected format)</param>
        public Customer(string CustomerXML) : base(CustomerXML)
        { 
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(CustomerXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "CustomerXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == "customer")
                {
                    this.LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no customer info was found
            throw new ArgumentException("XML does not contain customer information", "CustomerXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerNode">XML containing customer info (in expected format)</param>
        internal Customer(XmlNode customerNode) : base()
        {
            if (customerNode == null) throw new ArgumentNullException("CustomerNode");
            if (customerNode.Name != "customer") throw new ArgumentException("Not a vaild customer node", "customerNode");
            if (customerNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "customerNode");
            this.LoadFromNode(customerNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerObject">JsonObject containing customer info (in expected format)</param>
        public Customer(JsonObject customerObject)
            : base()
        {
            if (customerObject == null) throw new ArgumentNullException("customerObject");
            if (customerObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild customer object", "customerObject");
            this.LoadFromJSON(customerObject);
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
                    case CustomerAttributes.FirstNameKey:
                        this.FirstName = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.LastNameKey:
                        this.LastName = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.EmailKey:
                        this.Email = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.PhoneKey:
                        this.Phone = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.OrganizationKey:
                        this.Organization = HttpUtility.HtmlDecode(obj.GetJSONContentAsString(key));
                        break;
                    case CustomerAttributes.ReferenceKey:
                        this.SystemID = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.IDKey:
                        _chargifyID = obj.GetJSONContentAsInt(key);
                        break;
                    case CustomerAttributes.CreatedAtKey:
                        _created = obj.GetJSONContentAsDateTime(key);
                        break;
                    case CustomerAttributes.UpdatedAtKey:
                        _lastUpdated = obj.GetJSONContentAsDateTime(key);
                        break;
                    case CustomerAttributes.ShippingAddressKey:
                        this.ShippingAddress = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.ShippingAddress2Key:
                        this.ShippingAddress2 = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.ShippingCountryKey:
                        this.ShippingCountry = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.ShippingCityKey:
                        this.ShippingCity = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.ShippingStateKey:
                        this.ShippingState = obj.GetJSONContentAsString(key);
                        break;
                    case CustomerAttributes.ShippingZipKey:
                        this.ShippingZip = obj.GetJSONContentAsString(key);
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
                    case CustomerAttributes.FirstNameKey:
                        this.FirstName = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.LastNameKey:
                        this.LastName = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.EmailKey:
                        this.Email = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.PhoneKey:
                        this.Phone = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.OrganizationKey:
                        this.Organization = HttpUtility.HtmlDecode(dataNode.GetNodeContentAsString());                        
                        break;
                    case CustomerAttributes.ReferenceKey:
                        this.SystemID = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.IDKey:
                        _chargifyID = dataNode.GetNodeContentAsInt();
                        break;
                    case CustomerAttributes.CreatedAtKey:
                        _created = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CustomerAttributes.UpdatedAtKey:
                        _lastUpdated = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CustomerAttributes.ShippingAddressKey:
                        this.ShippingAddress = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.ShippingAddress2Key:
                        this.ShippingAddress2 = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.ShippingCountryKey:
                        this.ShippingCountry = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.ShippingCityKey:
                        this.ShippingCity = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.ShippingStateKey:
                        this.ShippingState = dataNode.GetNodeContentAsString();
                        break;
                    case CustomerAttributes.ShippingZipKey:
                        this.ShippingZip = dataNode.GetNodeContentAsString();
                        break;
                    default:
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
                return _chargifyID;
            }
        }
        private int _chargifyID = int.MinValue;
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
                return !(this.ChargifyID == int.MinValue);
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
            if (System.Object.ReferenceEquals(a, b)) return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
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
            if (System.Object.ReferenceEquals(a, b)) return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
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
            if (System.Object.ReferenceEquals(a, b)) return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
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
            if (System.Object.ReferenceEquals(this, obj)) return true;

            // If one is null, but not both, return false.
            if (((object)this == null) || ((object)obj == null))
            {
                return false;
            }

            // Return true if the fields match.
            return this.ChargifyID == ((Customer)obj).ChargifyID &&
                   this.Created == ((Customer)obj).Created &&
                   this.Email == ((Customer)obj).Email &&
                   this.FirstName == ((Customer)obj).FirstName &&
                   this.LastName == ((Customer)obj).LastName &&
                   this.Organization == ((Customer)obj).Organization &&
                   this.ShippingAddress == ((Customer)obj).ShippingAddress &&
                   this.ShippingAddress2 == ((Customer)obj).ShippingAddress2 &&
                   this.ShippingCity == ((Customer)obj).ShippingCity &&
                   this.ShippingCountry == ((Customer)obj).ShippingCountry &&
                   this.ShippingState == ((Customer)obj).ShippingState &&
                   this.ShippingZip == ((Customer)obj).ShippingZip &&
                   this.SystemID == ((Customer)obj).SystemID;
        }

        /// <summary>
        /// Convert object to a string
        /// </summary>
        /// <returns>The string representation of the object</returns>
        public override string ToString()
        {
            return this.FullName;
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
            return this.FullName.CompareTo(other.FullName);
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
            return this.FullName.CompareTo(other.FullName);
        }

        #endregion
    }
}
