#region License, Terms and Conditions
//
// ChargifyConnect.cs
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
    using System.Linq;
    using ChargifyNET.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Xml;
    using System.Globalization;
    #endregion

    #region Enum
    /// <summary>
    /// The type of REST request
    /// </summary>
    public enum HttpRequestMethod
    {
        /// <summary>
        /// Requests a representation of the specified resource
        /// </summary>
        Get,
        /// <summary>
        /// Requests that the server accept the entity enclosed in the request as a new subordinate of the web resource identified by the URI
        /// </summary>
        Post,
        /// <summary>
        /// Requests that the enclosed entity be stored under the supplied URI
        /// </summary>
        Put,
        /// <summary>
        /// Deletes the specified resource
        /// </summary>
        Delete
    }
    #endregion

    /// <summary>
    /// Class containing methods for interfacing with the Chargify API via XML and JSON
    /// </summary>
    public class ChargifyConnect : IChargifyConnect
    {
        #region System Constants
        private const string DateTimeFormat = "yyyy-MM-dd";
        private const string updateShortName = "update_payment";

        #endregion

        #region Constructors

        private int timeout = 180000;

        /// <summary>
        /// Constructor
        /// </summary>
        public ChargifyConnect() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">The Chargify URL</param>
        /// <param name="apiKey">Your Chargify api key</param>
        /// <param name="password">Your Chargify api password</param>
        public ChargifyConnect(string url, string apiKey, string password)
        {
            this.URL = url;
            this.apiKey = apiKey;
            this.Password = password;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">The Chargify URL</param>
        /// <param name="apiKey">Your Chargify api key</param>
        /// <param name="password">Your Chargify api password</param>
        /// <param name="sharedKey">Your Chargify hosted page shared key</param>
        public ChargifyConnect(string url, string apiKey, string password, string sharedKey)
        {
            this.URL = url;
            this.apiKey = apiKey;
            this.Password = password;
            this.SharedKey = sharedKey;
        }

        #endregion

        #region Properties
        private static string UserAgent
        {
            get
            {
                if (_userAgent == null)
                {
                    _userAgent = String.Format("Chargify.NET Client v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                }
                return _userAgent;
            }
        }
        private static string _userAgent;

        /// <summary>
        /// Get or set the API key
        /// </summary>
        public string apiKey { get; set; }

        /// <summary>
        /// Get or set the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Get or set the URL for chargify
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// SharedKey used for url generation
        /// </summary>
        public string SharedKey { get; set; }

        /// <summary>
        /// Should Chargify.NET use JSON for output? XML by default, always XML for input.
        /// </summary>
        public bool UseJSON { get; set; }

        /// <summary>
        /// Should the library require a CVV?
        /// </summary>
        public bool CvvRequired { get { return this._cvvRequired; } set { this._cvvRequired = value; } }
        private bool _cvvRequired = true;

        /// <summary>
        /// Allows you to specify the specific SecurityProtocolType. If not set, then
        /// the default is used.
        /// </summary>
        public SecurityProtocolType? ProtocolType
        {
            get { return this._protocolType; }
            set
            {
                if (value.HasValue)
                {
                    this._protocolType = value;
                }
                else
                {
                    this._protocolType = null;
                }
            }
        }
        private SecurityProtocolType? _protocolType = null;

        /// <summary>
        /// The timeout (in milliseconds) for any call to Chargify. The default is 180000
        /// </summary>
        public int Timeout
        {
            get
            {
                return this.timeout;
            }
            set
            {
                this.timeout = value;
            }
        }

        /// <summary>
        /// Method for determining if the properties have been set to allow this instance to connect correctly.
        /// </summary>
        public bool HasConnected
        {
            get
            {
                bool result = true;
                if (string.IsNullOrEmpty(this.apiKey)) result = false;
                if (string.IsNullOrEmpty(this.Password)) result = false;
                if (string.IsNullOrEmpty(this.URL)) result = false;
                return result;
            }
        }

        /// <summary>
        /// Caller can plug in a delegate for logging raw Chargify requests
        /// </summary>
        public Action<HttpRequestMethod, string, string> LogRequest { get; set; }

        /// <summary>
        /// Caller can plug in a delegate for logging raw Chargify responses
        /// </summary>
        public Action<HttpStatusCode, string, string> LogResponse { get; set; }

        /// <summary>
        /// Get a reference to the last Http Response from the chargify server. This is set after every call to
        /// a Chargify Connect method
        /// </summary>
        public HttpWebResponse LastResponse
        {
            get
            {
                return _lastResponse;
            }
        }
        private HttpWebResponse _lastResponse = null;

        #endregion

        #region Metadata
        /// <summary>
        /// Allows you to set a group of metadata for a specific resource
        /// </summary>
        /// <typeparam name="T">The type of resource. Currently either Subscription or Customer</typeparam>
        /// <param name="chargifyID">The Chargify identifier for the resource</param>
        /// <param name="metadatum">The list of metadatum to set</param>
        /// <returns>The metadata result containing the response</returns>
        public List<IMetadata> SetMetadataFor<T>(int chargifyID, List<Metadata> metadatum)
        {
            // make sure data is valid
            if (metadatum == null) { throw new ArgumentNullException("metadatum"); }
            if (metadatum.Count <= 0) { throw new ArgumentOutOfRangeException("metadatum"); }
            //if (metadatum.Select(m => m.ResourceID < 0).Count() > 0) { throw new ArgumentOutOfRangeException("Metadata.ResourceID"); }
            //if (metadatum.Select(m => string.IsNullOrEmpty(m.Name)).Count() > 0) { throw new ArgumentNullException("Metadata.Name"); }
            //if (metadatum.Select(m => m.Value == null).Count() > 0) { throw new ArgumentNullException("Metadata.Value"); }

            // create XML for creation of metadata
            var metadataXml = new StringBuilder(GetXMLStringIfApplicable());
            metadataXml.Append("<metadata type=\"array\">");
            foreach (var metadata in metadatum)
            {
                metadataXml.Append("<metadatum>");
                if (metadata.ResourceID > 0)
                {
                    metadataXml.AppendFormat("<resource-id>{0}</resource-id>", metadata.ResourceID);
                }
                else
                {
                    metadataXml.AppendFormat("<resource-id>{0}</resource-id>", chargifyID);
                }
                metadataXml.AppendFormat("<name>{0}</name>", metadata.Name);
                metadataXml.AppendFormat("<value>{0}</value>", metadata.Value);
                metadataXml.Append("</metadatum>");
            }
            metadataXml.Append("</metadata>");

            string url = string.Empty;
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    url = string.Format("customers/{0}/metadata.{1}", chargifyID, GetMethodExtension());
                    break;
                case "subscription":
                    url = string.Format("subscriptions/{0}/metadata.{1}", chargifyID, GetMethodExtension());
                    break;
                default:
                    throw new Exception(string.Format("Must be of type '{0}'", string.Join(", ", MetadataTypes.ToArray())));
            }

            // now make the request
            string response = this.DoRequest(url, HttpRequestMethod.Post, metadataXml.ToString());

            var retVal = new List<IMetadata>();

            // now build the object based on response as XML
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(response); // get the XML into an XML document
            if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
            // loop through the child nodes of this node
            foreach (XmlNode parentNode in Doc.ChildNodes)
            {
                if (parentNode.Name == "metadata")
                {
                    foreach (XmlNode childNode in parentNode.ChildNodes)
                    {
                        if (childNode.Name == "metadatum")
                        {
                            IMetadata loadedNode = new Metadata(childNode);
                            retVal.Add(loadedNode);
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Allows you to set a single metadata for a specific resource
        /// </summary>
        /// <typeparam name="T">The type of resource. Currently either Subscription or Customer</typeparam>
        /// <param name="chargifyID">The Chargify identifier for the resource</param>
        /// <param name="metadata">The list of metadata to set</param>
        /// <returns>The metadata result containing the response</returns>
        public List<IMetadata> SetMetadataFor<T>(int chargifyID, Metadata metadata)
        {
            // make sure data is valid
            if (metadata == null) throw new ArgumentNullException("metadata");
            //if (chargifyID < 0 || metadata.ResourceID < 0) throw new ArgumentOutOfRangeException("Metadata.ResourceID");
            if (string.IsNullOrEmpty(metadata.Name)) throw new ArgumentNullException("Metadata.Name");
            if (metadata.Value == null) throw new ArgumentNullException("Metadata.Value");

            // create XML for creation of metadata
            var MetadataXML = new StringBuilder(GetXMLStringIfApplicable());
            MetadataXML.Append("<metadata>");
            if (metadata.ResourceID > 0)
            {
                MetadataXML.AppendFormat("<resource-id>{0}</resource-id>", metadata.ResourceID);
            }
            else
            {
                MetadataXML.AppendFormat("<resource-id>{0}</resource-id>", chargifyID);
            }
            MetadataXML.AppendFormat("<name>{0}</name>", metadata.Name);
            MetadataXML.AppendFormat("<value>{0}</value>", metadata.Value);
            MetadataXML.Append("</metadata>");

            string url = string.Empty;
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    url = string.Format("customers/{0}/metadata.{1}", chargifyID, GetMethodExtension());
                    break;
                case "subscription":
                    url = string.Format("subscriptions/{0}/metadata.{1}", chargifyID, GetMethodExtension());
                    break;
                default:
                    throw new Exception(string.Format("Must be of type '{0}'", string.Join(", ", MetadataTypes.ToArray())));
            }

            // now make the request
            string response = this.DoRequest(url, HttpRequestMethod.Post, MetadataXML.ToString());

            var retVal = new List<IMetadata>();

            // now build the object based on response as XML
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(response); // get the XML into an XML document
            if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
            // loop through the child nodes of this node
            foreach (XmlNode parentNode in Doc.ChildNodes)
            {
                if (parentNode.Name == "metadata")
                {
                    foreach (XmlNode childNode in parentNode.ChildNodes)
                    {
                        if (childNode.Name == "metadatum")
                        {
                            IMetadata loadedNode = new Metadata(childNode);
                            retVal.Add(loadedNode);
                        }
                    }
                }
                else if (parentNode.Name == "metadatum")
                {
                    IMetadata loadedNode = new Metadata(parentNode);
                    retVal.Add(loadedNode);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve all metadata for a specific resource (like a specific customer or subscription).
        /// </summary>
        /// <typeparam name="T">The type of resource. Currently either Subscription or Customer</typeparam>
        /// <param name="resourceID">The Chargify identifier for the resource</param>
        /// <param name="page">Which page to return</param>
        /// <returns>The metadata result containing the response</returns>
        public IMetadataResult GetMetadataFor<T>(int resourceID, int? page)
        {
            string url = string.Empty;
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    url = string.Format("customers/{0}/metadata.{1}", resourceID, GetMethodExtension());
                    break;
                case "subscription":
                    url = string.Format("subscriptions/{0}/metadata.{1}", resourceID, GetMethodExtension());
                    break;
                default:
                    throw new Exception(string.Format("Must be of type '{0}'", string.Join(", ", MetadataTypes.ToArray())));
            }

            string qs = string.Empty;

            // Add the transaction options to the query string ...
            if (page.HasValue && page.Value != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", page); }

            // Construct the url to access Chargify
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = this.DoRequest(url);

            // change the response to the object
            return response.ConvertResponseTo<MetadataResult>("metadata");
        }

        /// <summary>
        /// Returns a list of all metadata for a resource.
        /// </summary>
        /// <typeparam name="T">The type of resource. Currently either Subscription or Customer</typeparam>
        /// <returns>The metadata result containing the response</returns>
        public IMetadataResult GetMetadata<T>()
        {
            string response = string.Empty;
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    response = this.DoRequest(string.Format("customers/metadata.{0}", GetMethodExtension()), HttpRequestMethod.Get, null);
                    break;
                case "subscription":
                    response = this.DoRequest(string.Format("subscriptions/metadata.{0}", GetMethodExtension()), HttpRequestMethod.Get, null);
                    break;
                default:
                    throw new Exception(string.Format("Must be of type '{0}'", string.Join(", ", MetadataTypes.ToArray())));
            }
            // change the response to the object
            return response.ConvertResponseTo<MetadataResult>("metadata");
        }
        private static List<string> MetadataTypes = new List<string> { "Customer", "Subscription" };
        #endregion

        #region Customers

        /// <summary>
        /// Load the requested customer from chargify
        /// </summary>
        /// <param name="ChargifyID">The chargify ID of the customer</param>
        /// <returns>The customer with the specified chargify ID</returns>
        public ICustomer LoadCustomer(int ChargifyID)
        {
            try
            {
                // make sure data is valid
                if (ChargifyID == int.MinValue) throw new ArgumentNullException("ChargifyID");
                // now make the request
                string response = this.DoRequest(string.Format("customers/{0}.{1}", ChargifyID, GetMethodExtension()));
                // change the response to the object
                return response.ConvertResponseTo<Customer>("customer");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Load the requested customer from chargify
        /// </summary>
        /// <param name="SystemID">The system ID of the customer</param>
        /// <returns>The customer with the specified chargify ID</returns>
        public ICustomer LoadCustomer(string SystemID)
        {
            try
            {
                // make sure data is valid
                if (SystemID == string.Empty) throw new ArgumentException("Empty SystemID not allowed", "SystemID");
                // now make the request
                string response = this.DoRequest(string.Format("customers/lookup.{0}?reference={1}", GetMethodExtension(), SystemID));
                // change the response to the object
                return response.ConvertResponseTo<Customer>("customer");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Create a new chargify customer
        /// </summary>
        /// <param name="Customer">
        /// A customer object containing customer attributes.  The customer cannot be an existing saved chargify customer
        /// </param>
        /// <returns>The created chargify customer</returns>
        public ICustomer CreateCustomer(ICustomer Customer)
        {
            // make sure data is valid
            if (Customer == null) throw new ArgumentNullException("Customer");
            if (Customer.IsSaved) throw new ArgumentException("Customer already saved", "Customer");
            return CreateCustomer(Customer.FirstName, Customer.LastName, Customer.Email, Customer.Phone, Customer.Organization, Customer.SystemID,
                                  Customer.ShippingAddress, Customer.ShippingAddress2, Customer.ShippingCity, Customer.ShippingState,
                                  Customer.ShippingZip, Customer.ShippingCountry);
        }

        /// <summary>
        /// Create a new chargify customer
        /// </summary>
        /// <param name="FirstName">The first name of the customer</param>
        /// <param name="LastName">The last name of the customer</param>
        /// <param name="EmailAddress">The email address of the customer</param>
        /// <param name="Phone">The phone number of the customer</param>
        /// <param name="Organization">The organization of the customer</param>
        /// <param name="SystemID">The system ID of the customer</param>
        /// <param name="ShippingAddress">The shipping address of the customer, if applicable.</param>
        /// <param name="ShippingAddress2">The shipping address (line 2) of the customer, if applicable.</param>
        /// <param name="ShippingCity">The shipping city of the customer, if applicable.</param>
        /// <param name="ShippingState">The shipping state of the customer, if applicable.</param>
        /// <param name="ShippingZip">The shipping zip of the customer, if applicable.</param>
        /// <param name="ShippingCountry">The shipping country of the customer, if applicable.</param>
        /// <returns>The created chargify customer</returns>
        public ICustomer CreateCustomer(string FirstName, string LastName, string EmailAddress, string Phone, string Organization, string SystemID,
                                        string ShippingAddress, string ShippingAddress2, string ShippingCity, string ShippingState,
                                        string ShippingZip, string ShippingCountry)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(FirstName)) throw new ArgumentNullException("FirstName");
#if !DEBUG
            if (string.IsNullOrEmpty(LastName)) throw new ArgumentNullException("LastName");
            if (string.IsNullOrEmpty(EmailAddress)) throw new ArgumentNullException("EmailAddress");
            if (SystemID == string.Empty) throw new ArgumentException("Empty SystemID not allowed", "SystemID");
            // make sure that the system ID is unique
            if (this.LoadCustomer(SystemID) != null) throw new ArgumentException("Not unique", "SystemID");
#endif
            // create XML for creation of customer
            var CustomerXML = new StringBuilder(GetXMLStringIfApplicable());
            CustomerXML.Append("<customer>");
            if (!string.IsNullOrEmpty(EmailAddress)) CustomerXML.AppendFormat("<email>{0}</email>", EmailAddress);
            if (!string.IsNullOrEmpty(Phone)) CustomerXML.AppendFormat("<{0}>{1}</{2}>", CustomerAttributes.PhoneKey, Phone, CustomerAttributes.PhoneKey);
            if (!string.IsNullOrEmpty(FirstName)) CustomerXML.AppendFormat("<first_name>{0}</first_name>", FirstName);
            if (!string.IsNullOrEmpty(LastName)) CustomerXML.AppendFormat("<last_name>{0}</last_name>", LastName);
            if (!string.IsNullOrEmpty(Organization)) CustomerXML.AppendFormat("<organization>{0}</organization>", HttpUtility.HtmlEncode(Organization));
            if (!string.IsNullOrEmpty(SystemID)) CustomerXML.AppendFormat("<reference>{0}</reference>", SystemID);
            if (!string.IsNullOrEmpty(ShippingAddress)) CustomerXML.AppendFormat("<address>{0}</address>", ShippingAddress);
            if (!string.IsNullOrEmpty(ShippingAddress2)) CustomerXML.AppendFormat("<address_2>{0}</address_2>", ShippingAddress2);
            if (!string.IsNullOrEmpty(ShippingCity)) CustomerXML.AppendFormat("<city>{0}</city>", ShippingCity);
            if (!string.IsNullOrEmpty(ShippingState)) CustomerXML.AppendFormat("<state>{0}</state>", ShippingState);
            if (!string.IsNullOrEmpty(ShippingZip)) CustomerXML.AppendFormat("<zip>{0}</zip>", ShippingZip);
            if (!string.IsNullOrEmpty(ShippingCountry)) CustomerXML.AppendFormat("<country>{0}</country>", ShippingCountry);
            CustomerXML.Append("</customer>");
            // now make the request
            string response = this.DoRequest(string.Format("customers.{0}", GetMethodExtension()), HttpRequestMethod.Post, CustomerXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Customer>("customer");
        }

        /// <summary>
        /// Create a new chargify customer
        /// </summary>
        /// <param name="FirstName">The first name of the customer</param>
        /// <param name="LastName">The last name of the customer</param>
        /// <param name="EmailAddress">The email address of the customer</param>
        /// <param name="Phone">The phone number of the customer</param>
        /// <param name="Organization">The organization of the customer</param>
        /// <param name="SystemID">The system ID fro the customer</param>
        /// <returns>The created chargify customer</returns>
        public ICustomer CreateCustomer(string FirstName, string LastName, string EmailAddress, string Phone, string Organization, string SystemID)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(FirstName)) throw new ArgumentNullException("FirstName");
            if (string.IsNullOrEmpty(LastName)) throw new ArgumentNullException("LastName");
            if (string.IsNullOrEmpty(EmailAddress)) throw new ArgumentNullException("EmailAddress");
            if (SystemID == string.Empty) throw new ArgumentException("Empty SystemID not allowed", "SystemID");
            // make sure that the system ID is unique
            if (this.LoadCustomer(SystemID) != null) throw new ArgumentException("Not unique", "SystemID");
            // create XML for creation of customer
            var CustomerXML = new StringBuilder(GetXMLStringIfApplicable());
            CustomerXML.Append("<customer>");
            CustomerXML.AppendFormat("<email>{0}</email>", EmailAddress);
            CustomerXML.AppendFormat("<first_name>{0}</first_name>", FirstName);
            CustomerXML.AppendFormat("<last_name>{0}</last_name>", LastName);
            if (!string.IsNullOrEmpty(Phone)) CustomerXML.AppendFormat("<{0}>{1}</{2}>", CustomerAttributes.PhoneKey, Phone, CustomerAttributes.PhoneKey);
            if (!string.IsNullOrEmpty(Organization)) CustomerXML.AppendFormat("<organization>{0}</organization>", HttpUtility.HtmlEncode(Organization));
            if (!string.IsNullOrEmpty(SystemID)) CustomerXML.AppendFormat("<reference>{0}</reference>", SystemID);
            CustomerXML.Append("</customer>");
            // now make the request
            string response = this.DoRequest(string.Format("customers.{0}", GetMethodExtension()), HttpRequestMethod.Post, CustomerXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Customer>("customer");
        }

        /// <summary>
        /// Update the specified chargify customer
        /// </summary>
        /// <param name="Customer">The customer to update</param>
        /// <returns>The updated customer</returns>
        public ICustomer UpdateCustomer(ICustomer Customer)
        {
            // make sure data is OK
            if (Customer == null) throw new ArgumentNullException("Customer");
            if (Customer.ChargifyID == int.MinValue) throw new ArgumentException("Invalid chargify ID detected", "Customer.ChargifyID");
            ICustomer OldCust = this.LoadCustomer(Customer.ChargifyID);
            // create XML for creation of customer
            var customerXml = new StringBuilder(GetXMLStringIfApplicable());
            customerXml.Append("<customer>");
            if (OldCust != null)
            {
                if (OldCust.ChargifyID != Customer.ChargifyID) throw new ArgumentException("Not unique", "Customer.SystemID");
                if (OldCust.FirstName != Customer.FirstName) customerXml.AppendFormat("<first_name>{0}</first_name>", HttpUtility.HtmlEncode(Customer.FirstName));
                if (OldCust.LastName != Customer.LastName) customerXml.AppendFormat("<last_name>{0}</last_name>", HttpUtility.HtmlEncode(Customer.LastName));
                if (OldCust.Email != Customer.Email) customerXml.AppendFormat("<email>{0}</email>", Customer.Email);
                if (OldCust.Organization != Customer.Organization) customerXml.AppendFormat("<organization>{0}</organization>", HttpUtility.HtmlEncode(Customer.Organization));
                if (OldCust.Phone != Customer.Phone) customerXml.AppendFormat("<phone>{0}</phone>", HttpUtility.HtmlEncode(Customer.Phone));
                if (OldCust.SystemID != Customer.SystemID) customerXml.AppendFormat("<reference>{0}</reference>", Customer.SystemID);
                if (OldCust.ShippingAddress != Customer.ShippingAddress) customerXml.AppendFormat("<address>{0}</address>", HttpUtility.HtmlEncode(Customer.ShippingAddress));
                if (OldCust.ShippingAddress2 != Customer.ShippingAddress2) customerXml.AppendFormat("<address_2>{0}</address_2>", HttpUtility.HtmlEncode(Customer.ShippingAddress2));
                if (OldCust.ShippingCity != Customer.ShippingCity) customerXml.AppendFormat("<city>{0}</city>", HttpUtility.HtmlEncode(Customer.ShippingCity));
                if (OldCust.ShippingState != Customer.ShippingState) customerXml.AppendFormat("<state>{0}</state>", HttpUtility.HtmlEncode(Customer.ShippingState));
                if (OldCust.ShippingZip != Customer.ShippingZip) customerXml.AppendFormat("<zip>{0}</zip>", Customer.ShippingZip);
                if (OldCust.ShippingCountry != Customer.ShippingCountry) customerXml.AppendFormat("<country>{0}</country>", HttpUtility.HtmlEncode(Customer.ShippingCountry));
            }
            customerXml.Append("</customer>");

            try
            {
                // now make the request
                string response = this.DoRequest(string.Format("customers/{0}.{1}", Customer.ChargifyID, GetMethodExtension()), HttpRequestMethod.Put, customerXml.ToString());
                // change the response to the object
                return response.ConvertResponseTo<Customer>("customer");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Customer not found");
                throw;
            }
        }

        /// <summary>
        /// Get a list of customers (will return 50 for each page)
        /// </summary>
        /// <param name="PageNumber">The page number to load</param>
        /// <returns>A list of customers for the specified page</returns>
        public IDictionary<string, ICustomer> GetCustomerList(int PageNumber)
        {
            return GetCustomerList(PageNumber, false);
        }

        /// <summary>
        /// Get a list of customers (will return 50 for each page)
        /// </summary>
        /// <param name="PageNumber">The page number to load</param>
        /// <param name="keyByChargifyID">If true, the dictionary will be keyed by Chargify ID and not the reference value.</param>
        /// <returns>A list of customers for the specified page</returns>
        public IDictionary<string, ICustomer> GetCustomerList(int PageNumber, bool keyByChargifyID)
        {
            // make sure data is valid
            if (PageNumber < 1) throw new ArgumentException("Page number must be greater than 1", "PageNumber");
            // now make the request
            string response = this.DoRequest(string.Format("customers.{0}?page={1}", GetMethodExtension(), PageNumber));
            var retValue = new Dictionary<string, ICustomer>();
            if (response.IsXml())
            {
                // now build customer object based on response as XML
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response); // get the XML into an XML document
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "customers")
                    {
                        foreach (XmlNode customerNode in elementNode.ChildNodes)
                        {
                            if (customerNode.Name == "customer")
                            {
                                ICustomer LoadedCustomer = new Customer(customerNode);
                                string key = keyByChargifyID ? LoadedCustomer.ChargifyID.ToString() : LoadedCustomer.SystemID;
                                if (!retValue.ContainsKey(key))
                                {
                                    retValue.Add(key, LoadedCustomer);
                                }
                                else
                                {
                                    //throw new InvalidOperationException("Duplicate systemID values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("customer"))
                    {
                        JsonObject customerObj = (array.Items[i] as JsonObject)["customer"] as JsonObject;
                        ICustomer LoadedCustomer = new Customer(customerObj);
                        string key = keyByChargifyID ? LoadedCustomer.ChargifyID.ToString() : LoadedCustomer.SystemID;
                        if (!retValue.ContainsKey(key))
                        {
                            retValue.Add(key, LoadedCustomer);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate systemID values detected");
                        }
                    }
                }
            }
            // return the dictionary
            return retValue;
        }

        /// <summary>
        /// Get a list of all customers.  Be careful calling this method because a large number of
        /// customers will result in multiple calls to Chargify
        /// </summary>
        /// <returns>A list of customers</returns>
        public IDictionary<string, ICustomer> GetCustomerList()
        {
            return GetCustomerList(false);
        }

        /// <summary>
        /// Get a list of all customers.  Be careful calling this method because a large number of
        /// customers will result in multiple calls to Chargify
        /// </summary>
        /// <param name="keyByChargifyID">If true, the key will be the ChargifyID, otherwise it will be the reference value</param>
        /// <returns>A list of customers</returns>
        public IDictionary<string, ICustomer> GetCustomerList(bool keyByChargifyID)
        {
            var retValue = new Dictionary<string, ICustomer>();
            int PageCount = 1000;
            for (int Page = 1; PageCount > 0; Page++)
            {
                IDictionary<string, ICustomer> PageList = GetCustomerList(Page, keyByChargifyID);
                foreach (ICustomer cust in PageList.Values)
                {
                    string key = keyByChargifyID ? cust.ChargifyID.ToString() : cust.SystemID;
                    if (!retValue.ContainsKey(key))
                    {
                        retValue.Add(key, cust);
                    }
                    else
                    {
                        //throw new InvalidOperationException("Duplicate key values detected");
                    }
                }
                PageCount = PageList.Count;
            }
            return retValue;
        }

        /// <summary>
        /// Delete the specified customer
        /// </summary>
        /// <param name="ChargifyID">The integer identifier of the customer</param>
        /// <returns>True if the customer was deleted, false otherwise.</returns>
        /// <remarks>This method does not currently work, but it will once they open up the API. This will always return false, as Chargify will send a Http Forbidden everytime.</remarks>
        public bool DeleteCustomer(int ChargifyID)
        {
            try
            {
                // make sure data is valid
                if (ChargifyID < 0) throw new ArgumentNullException("ChargifyID");

                // now make the request
                this.DoRequest(string.Format("customers/{0}.{1}", ChargifyID, GetMethodExtension()), HttpRequestMethod.Delete, string.Empty);
                return true;
            }
            catch (ChargifyException cex)
            {
                switch (cex.StatusCode)
                {
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.NotFound:
                        return false;
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Delete the specified customer
        /// </summary>
        /// <param name="SystemID">The system identifier of the customer.</param>
        /// <returns>True if the customer was deleted, false otherwise.</returns>
        /// <remarks>This method does not currently work, but it will once they open up the API. This will always return false, as Chargify will send a Http Forbidden everytime.</remarks>
        public bool DeleteCustomer(string SystemID)
        {
            try
            {
                // make sure data is valid
                if (SystemID == string.Empty) throw new ArgumentException("Empty SystemID not allowed", "SystemID");

                ICustomer customer = LoadCustomer(SystemID);
                if (customer == null) { throw new ArgumentException("Not a known customer", "SystemID"); }

                // now make the request
                this.DoRequest(string.Format("customers/{0}.{1}", customer.ChargifyID, GetMethodExtension()), HttpRequestMethod.Delete, string.Empty);
                return true;
            }
            catch (ChargifyException cex)
            {
                switch (cex.StatusCode)
                {
                    //case HttpStatusCode.Forbidden:
                    //case HttpStatusCode.NotFound:
                    //    return false;
                    default:
                        throw;
                }
            }
        }

        #endregion

        #region Products

        /// <summary>
        /// Method to create a new product and add it to the site
        /// </summary>
        /// <param name="ProductFamilyID">The product family ID, required for adding products</param>
        /// <param name="NewProduct">The new product details</param>
        /// <returns>The completed product information</returns>
        /// <remarks>This is largely undocumented currently, especially the fact that you need the product family ID</remarks>
        public IProduct CreateProduct(int ProductFamilyID, IProduct NewProduct)
        {
            if (NewProduct == null) throw new ArgumentNullException("NewProduct");
            return CreateProduct(ProductFamilyID, NewProduct.Name, NewProduct.Handle, NewProduct.PriceInCents, NewProduct.Interval, NewProduct.IntervalUnit, NewProduct.AccountingCode, NewProduct.Description);
        }

        /// <summary>
        /// Allows the creation of a product
        /// </summary>
        /// <param name="ProductFamilyID">The family to which this product belongs</param>
        /// <param name="Name">The name of the product</param>
        /// <param name="Handle">The handle to be used for this product</param>
        /// <param name="PriceInCents">The price (in cents)</param>
        /// <param name="Interval">The time interval used to determine the recurring nature of this product</param>
        /// <param name="IntervalUnit">Either days, or months</param>
        /// <param name="AccountingCode">The accounting code used for this product</param>
        /// <param name="Description">The product description</param>
        /// <returns>The created product</returns>
        public IProduct CreateProduct(int ProductFamilyID, string Name, string Handle, int PriceInCents, int Interval, IntervalUnit IntervalUnit, string AccountingCode, string Description)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException("Name");
            // create XML for creation of the new product
            var ProductXML = new StringBuilder(GetXMLStringIfApplicable());
            ProductXML.Append("<product>");
            ProductXML.AppendFormat("<name>{0}</name>", HttpUtility.HtmlEncode(Name));
            ProductXML.AppendFormat("<price_in_cents>{0}</price_in_cents>", PriceInCents);
            ProductXML.AppendFormat("<interval>{0}</interval>", Interval);
            ProductXML.AppendFormat("<interval_unit>{0}</interval_unit>", Enum.GetName(typeof(IntervalUnit), IntervalUnit).ToLowerInvariant());
            if (!string.IsNullOrEmpty(Handle)) ProductXML.AppendFormat("<handle>{0}</handle>", Handle);
            if (!string.IsNullOrEmpty(AccountingCode)) ProductXML.AppendFormat("<accounting_code>{0}</accounting_code>", AccountingCode);
            if (!string.IsNullOrEmpty(Description)) ProductXML.AppendFormat("<description>{0}</description>", HttpUtility.HtmlEncode(Description));
            ProductXML.Append("</product>");
            // now make the request
            string response = this.DoRequest(string.Format("product_families/{0}/products.{1}", ProductFamilyID, GetMethodExtension()), HttpRequestMethod.Post, ProductXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Product>("product");
        }

        /// <summary>
        /// Load the requested product from chargify by its handle
        /// </summary>
        /// <param name="Handle">The Chargify ID or handle of the product</param>
        /// <returns>The product with the specified chargify ID</returns>
        public IProduct LoadProduct(string Handle)
        {
            return LoadProduct(Handle, true);
        }

        /// <summary>
        /// Load the requested product from chargify
        /// </summary>
        /// <param name="ProductID">The Chargify ID or handle of the product</param>
        /// <param name="IsHandle">If true, then the ProductID represents the handle, if false the ProductID represents the Chargify ID</param>
        /// <returns>The product with the specified chargify ID</returns>
        public IProduct LoadProduct(string ProductID, bool IsHandle)
        {
            try
            {
                // make sure data is valid
                if (string.IsNullOrEmpty(ProductID)) throw new ArgumentNullException("ProductID");
                // now make the request
                string response;
                if (IsHandle)
                {
                    response = this.DoRequest(string.Format("products/handle/{0}.{1}", ProductID, GetMethodExtension()));
                }
                else
                {
                    response = this.DoRequest(string.Format("products/{0}.{1}", ProductID, GetMethodExtension()));
                }
                // Convert the Chargify response into the object we're looking for
                return response.ConvertResponseTo<Product>("product");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Get a list of products
        /// </summary>
        /// <returns>A list of products (keyed by product handle)</returns>
        public IDictionary<int, IProduct> GetProductList()
        {
            // now make the request
            string response = this.DoRequest(string.Format("products.{0}", GetMethodExtension()));
            // loop through the child nodes of this node
            var retValue = new Dictionary<int, IProduct>();
            if (response.IsXml())
            {
                // now build a product list based on response XML
                // get the XML into an XML document
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response);
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "products")
                    {
                        foreach (XmlNode productNode in elementNode.ChildNodes)
                        {
                            if (productNode.Name == "product")
                            {
                                IProduct LoadedProduct = new Product(productNode);
                                if (!retValue.ContainsKey(LoadedProduct.ID))
                                {
                                    retValue.Add(LoadedProduct.ID, LoadedProduct);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate handle values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("product"))
                    {
                        JsonObject productObj = (array.Items[i] as JsonObject)["product"] as JsonObject;
                        IProduct LoadedProduct = new Product(productObj);
                        if (!retValue.ContainsKey(LoadedProduct.ID))
                        {
                            retValue.Add(LoadedProduct.ID, LoadedProduct);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate handle values detected");
                        }
                    }
                }
            }
            // return the list
            return retValue;
        }

        #endregion

        #region Product Families
        /// <summary>
        /// Method for creating a new product family via the API
        /// </summary>
        /// <param name="newFamily">The new product family details</param>
        /// <returns>The created product family information</returns>
        public IProductFamily CreateProductFamily(IProductFamily newFamily)
        {
            // make sure data is valid
            if (newFamily == null) throw new ArgumentNullException("newFamily");
            if (string.IsNullOrEmpty(newFamily.Name)) throw new ArgumentNullException("Name");
            // create XML for creation of the new product family
            var ProductFamilyXML = new StringBuilder(GetXMLStringIfApplicable());
            ProductFamilyXML.Append("<product_family>");
            ProductFamilyXML.AppendFormat("<name>{0}</name>", HttpUtility.HtmlEncode(newFamily.Name));
            if (!string.IsNullOrEmpty(newFamily.Handle)) ProductFamilyXML.AppendFormat("<handle>{0}</handle>", newFamily.Handle);
            if (!string.IsNullOrEmpty(newFamily.AccountingCode)) ProductFamilyXML.AppendFormat("<accounting_code>{0}</accounting_code>", HttpUtility.HtmlEncode(newFamily.AccountingCode));
            if (!string.IsNullOrEmpty(newFamily.Description)) ProductFamilyXML.AppendFormat("<description>{0}</description>", HttpUtility.HtmlEncode(newFamily.Description));
            ProductFamilyXML.Append("</product_family>");
            // now make the request
            string response = this.DoRequest(string.Format("product_families.{0}", GetMethodExtension()), HttpRequestMethod.Post, ProductFamilyXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<ProductFamily>("product_family");
        }

        /// <summary>
        /// Get a list of product families
        /// </summary>
        /// <returns>A list of product families (keyed by product family id)</returns>
        public IDictionary<int, IProductFamily> GetProductFamilyList()
        {
            // now make the request
            string response = this.DoRequest(string.Format("product_families.{0}", GetMethodExtension()));
            // loop through the child nodes of this node
            var retValue = new Dictionary<int, IProductFamily>();
            if (response.IsXml())
            {
                // now build a product family list based on response XML
                // get the XML into an XML document
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response);
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "product_families")
                    {
                        foreach (XmlNode productFamilyNode in elementNode.ChildNodes)
                        {
                            if (productFamilyNode.Name == "product_family")
                            {
                                IProductFamily LoadedProductFamily = new ProductFamily(productFamilyNode);
                                if (!retValue.ContainsKey(LoadedProductFamily.ID))
                                {
                                    retValue.Add(LoadedProductFamily.ID, LoadedProductFamily);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate id values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("product_family"))
                    {
                        JsonObject productFamilyObj = (array.Items[i] as JsonObject)["product_family"] as JsonObject;
                        IProductFamily LoadedProductFamily = new ProductFamily(productFamilyObj);
                        if (!retValue.ContainsKey(LoadedProductFamily.ID))
                        {
                            retValue.Add(LoadedProductFamily.ID, LoadedProductFamily);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate handle values detected");
                        }
                    }
                }
            }
            // return the list
            return retValue;
        }

        /// <summary>
        /// Load the requested product family from chargify by its handle
        /// </summary>
        /// <param name="Handle">The Chargify ID or handle of the product</param>
        /// <returns>The product family with the specified chargify ID</returns>
        public IProductFamily LoadProductFamily(string Handle)
        {
            return LoadProductFamily(Handle, true);
        }

        /// <summary>
        /// Load the requested product family from chargify by its handle
        /// </summary>
        /// <param name="ID">The Chargify ID of the product</param>
        /// <returns>The product family with the specified chargify ID</returns>
        public IProductFamily LoadProductFamily(int ID)
        {
            return LoadProductFamily(ID.ToString(), false);
        }

        /// <summary>
        /// Load the requested product family from chargify
        /// </summary>
        /// <param name="ProductFamilyIdentifier">The Chargify identifier (ID or handle) of the product family</param>
        /// <param name="IsHandle">If true, then the ProductID represents the handle, if false the ProductFamilyID represents the Chargify ID</param>
        /// <returns>The product family with the specified chargify ID</returns>
        private IProductFamily LoadProductFamily(string ProductFamilyIdentifier, bool IsHandle)
        {
            try
            {
                // make sure data is valid
                if (string.IsNullOrEmpty(ProductFamilyIdentifier)) throw new ArgumentNullException("ProductFamilyID");
                // now make the request
                string response;
                if (IsHandle)
                {
                    response = this.DoRequest(string.Format("product_families/lookup.{0}?handle={1}", GetMethodExtension(), ProductFamilyIdentifier));
                }
                else
                {
                    response = this.DoRequest(string.Format("product_families/{0}.{1}", ProductFamilyIdentifier, GetMethodExtension()));
                }
                // Convert the Chargify response into the object we're looking for
                return response.ConvertResponseTo<ProductFamily>("product_family");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }
        #endregion

        #region Subscriptions

        /// <summary>
        /// Method to get the secure URL (with pretty id) for updating the payment details for a subscription.
        /// </summary>
        /// <param name="FirstName">The first name of the customer to add to the pretty url</param>
        /// <param name="LastName">The last name of the customer to add to the pretty url</param>
        /// <param name="SubscriptionID">The ID of the subscription to update</param>
        /// <returns>The secure url of the update page</returns>
        public string GetPrettySubscriptionUpdateURL(string FirstName, string LastName, int SubscriptionID)
        {
            if (string.IsNullOrEmpty(this.SharedKey)) throw new ArgumentException("SharedKey is required to generate the hosted page url");

            string message = updateShortName + "--" + SubscriptionID + "--" + SharedKey;
            string token = message.GetChargifyHostedToken();
            string prettyID = string.Format("{0}-{1}-{2}", SubscriptionID, FirstName.Trim().ToLower(), LastName.Trim().ToLower());
            string methodString = string.Format("{0}/{1}/{2}", updateShortName, prettyID, token);
            // just in case?
            methodString = HttpUtility.UrlEncode(methodString);
            string updateUrl = string.Format("{0}{1}{2}", this.URL, (this.URL.EndsWith("/") ? "" : "/"), methodString);
            return updateUrl;
        }

        /// <summary>
        /// Method to get the secure URL for updating the payment details for a subscription.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to update</param>
        /// <returns>The secure url of the update page</returns>
        public string GetSubscriptionUpdateURL(int SubscriptionID)
        {
            if (string.IsNullOrEmpty(this.SharedKey)) throw new ArgumentException("SharedKey is required to generate the hosted page url");

            string message = updateShortName + "--" + SubscriptionID + "--" + SharedKey;
            string token = message.GetChargifyHostedToken();
            string methodString = string.Format("{0}/{1}/{2}", updateShortName, SubscriptionID, token);
            methodString = HttpUtility.UrlEncode(methodString);
            string updateURL = string.Format("{0}{1}{2}", this.URL, (this.URL.EndsWith("/") ? "" : "/"), methodString);
            return updateURL;
        }

        /// <summary>
        /// Chargify offers the ability to reactivate a previously canceled subscription. For details
        /// on how reactivation works, and how to reactivate subscriptions through the Admin interface, see
        /// http://support.chargify.com/faqs/features/reactivation
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to reactivate</param>
        /// <returns>The newly reactivated subscription, or nothing.</returns>
        public ISubscription ReactivateSubscription(int SubscriptionID)
        {
            try
            {
                return ReactivateSubscription(SubscriptionID, false);
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Chargify offers the ability to reactivate a previously canceled subscription. For details
        /// on how reactivation works, and how to reactivate subscriptions through the Admin interface, see
        /// http://support.chargify.com/faqs/features/reactivation
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to reactivate</param>
        /// <param name="includeTrial">If true, the reactivated subscription will include a trial if one is available.</param>
        /// <returns>The newly reactivated subscription, or nothing.</returns>
        public ISubscription ReactivateSubscription(int SubscriptionID, bool includeTrial)
        {
            return ReactivateSubscription(SubscriptionID, includeTrial, null, null);
        }

        /// <summary>
        /// Chargify offers the ability to reactivate a previously canceled subscription. For details
        /// on how reactivation works, and how to reactivate subscriptions through the Admin interface, see
        /// http://support.chargify.com/faqs/features/reactivation
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to reactivate</param>
        /// <param name="includeTrial">If true, the reactivated subscription will include a trial if one is available.</param>
        /// <param name="preserveBalance">If true, the existing subscription balance will NOT be cleared/reset before adding the additional reactivation charges.</param>
        /// <param name="couponCode">The coupon code to be applied during reactivation.</param>
        /// <returns>The newly reactivated subscription, or nothing.</returns>
        public ISubscription ReactivateSubscription(int SubscriptionID, bool includeTrial, bool? preserveBalance, string couponCode)
        {
            try
            {
                // make sure data is valid
                if (SubscriptionID < 0) throw new ArgumentNullException("SubscriptionID");
                string requestString = string.Format("subscriptions/{0}/reactivate.{1}", SubscriptionID, GetMethodExtension());

                StringBuilder queryString = new StringBuilder();

                // If includeTrial = true, the reactivated subscription will include a trial if one is available.
                if (includeTrial) { queryString.Append("include_trial=1"); }

                if (preserveBalance.HasValue)
                {
                    if (queryString.Length > 0) queryString.Append("&");
                    queryString.AppendFormat("preserve_balance={0}", preserveBalance.Value ? "1" : "0");
                }

                if (!string.IsNullOrEmpty(couponCode))
                {
                    if (queryString.Length > 0) queryString.Append("&");
                    queryString.AppendFormat("coupon_code={0}", couponCode);
                }

                // Append the query string to the request, if applicable.
                if (queryString.Length > 0) requestString += "?" + queryString.ToString();

                // now make the request
                string response = this.DoRequest(requestString, HttpRequestMethod.Put, string.Empty);
                // change the response to the object
                return response.ConvertResponseTo<Subscription>("subscription");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw; // otherwise
            }
        }

        /// <summary>
        /// Delete a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the sucscription</param>
        /// <param name="CancellationMessage">The message to associate with the subscription</param>
        /// <returns></returns>
        public bool DeleteSubscription(int SubscriptionID, string CancellationMessage)
        {
            try
            {
                // make sure data is valid
                if (SubscriptionID < 0) throw new ArgumentNullException("SubscriptionID");

                StringBuilder SubscriptionXML = new StringBuilder("");
                if (!string.IsNullOrEmpty(CancellationMessage))
                {
                    // create XML for creation of customer
                    SubscriptionXML = new StringBuilder(GetXMLStringIfApplicable());
                    SubscriptionXML.Append("<subscription>");
                    SubscriptionXML.AppendFormat("<cancellation_message>{0}</cancellation_message>", CancellationMessage);
                    SubscriptionXML.Append("</subscription>");
                }
                // now make the request
                this.DoRequest(string.Format("subscriptions/{0}.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Delete, SubscriptionXML.ToString());
                return true;
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return false;
                throw; // otherwise
            }
        }

        /// <summary>
        /// Load the requested customer from chargify
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription</param>
        /// <returns>The subscription with the specified ID</returns>
        public ISubscription LoadSubscription(int SubscriptionID)
        {
            try
            {
                // make sure data is valid
                if (SubscriptionID < 0) throw new ArgumentNullException("SubscriptionID");
                // now make the request
                string response = this.DoRequest(string.Format("subscriptions/{0}.{1}", SubscriptionID, GetMethodExtension()));
                // change the response to the object
                return response.ConvertResponseTo<Subscription>("subscription");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Method that returns a list of subscriptions.
        /// </summary>
        /// <param name="states">A list of the states of subscriptions to return</param>
        /// <returns>Null if there are no results, object otherwise.</returns>
        public IDictionary<int, ISubscription> GetSubscriptionList(List<SubscriptionState> states)
        {
            string qs = "";

            if (states != null)
            {
                foreach (SubscriptionState state in states)
                {
                    // Iterate through them all, except for Unknown - which isn't supported, just used internally.
                    if (state == SubscriptionState.Unknown) break;

                    // Append the kind to the query string ...
                    if (qs.Length > 0) { qs += "&"; }
                    qs += string.Format("kinds[]={0}", state.ToString().ToLower());
                }
            }

            string url = string.Format("subscriptions.{0}", GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = this.DoRequest(url);

            var retValue = new Dictionary<int, ISubscription>();
            if (response.IsXml())
            {
                // now build a transaction list based on response XML
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response);
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "subscriptions")
                    {
                        foreach (XmlNode subscriptionNode in elementNode.ChildNodes)
                        {
                            if (subscriptionNode.Name == "subscription")
                            {
                                ISubscription LoadedSubscription = new Subscription(subscriptionNode);
                                if (!retValue.ContainsKey(LoadedSubscription.SubscriptionID))
                                {
                                    retValue.Add(LoadedSubscription.SubscriptionID, LoadedSubscription);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate SubscriptionID values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("subscription"))
                    {
                        JsonObject subscriptionObj = (array.Items[i] as JsonObject)["subscription"] as JsonObject;
                        ISubscription LoadedSubscription = new Subscription(subscriptionObj);
                        if (!retValue.ContainsKey(LoadedSubscription.SubscriptionID))
                        {
                            retValue.Add(LoadedSubscription.SubscriptionID, LoadedSubscription);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate SubscriptionID values detected");
                        }
                    }
                }
            }
            // return the list
            return retValue;
        }

        /// <summary>
        /// Method that returns a list of subscriptions.
        /// </summary>
        /// <returns>Null if there are no results, object otherwise.</returns>
        public IDictionary<int, ISubscription> GetSubscriptionList()
        {
            var retValue = new Dictionary<int, ISubscription>();
            int PageCount = 1000;
            for (int Page = 1; PageCount > 0; Page++)
            {
                IDictionary<int, ISubscription> PageList = GetSubscriptionList(Page, 50);
                foreach (ISubscription subscription in PageList.Values)
                {
                    if (!retValue.ContainsKey(subscription.SubscriptionID))
                    {
                        retValue.Add(subscription.SubscriptionID, subscription);
                    }
                    else
                    {
                        throw new InvalidOperationException("Duplicate subscriptionID values detected");
                    }
                }
                PageCount = PageList.Count;
            }
            return retValue;
            //return GetSubscriptionList(int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Method that returns a list of subscriptions.
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <returns>Null if there are no results, object otherwise.</returns>
        public IDictionary<int, ISubscription> GetSubscriptionList(int page, int per_page)
        {
            string qs = string.Empty;

            if (page != int.MinValue)
            {
                if (qs.Length > 0) { qs += "&"; }
                qs += string.Format("page={0}", page);
            }

            if (per_page != int.MinValue)
            {
                if (qs.Length > 0) { qs += "&"; }
                qs += string.Format("per_page={0}", per_page);
            }

            string url = string.Format("subscriptions.{0}", GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = this.DoRequest(url);

            var retValue = new Dictionary<int, ISubscription>();
            if (response.IsXml())
            {
                // now build a transaction list based on response XML
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response);
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "subscriptions")
                    {
                        foreach (XmlNode subscriptionNode in elementNode.ChildNodes)
                        {
                            if (subscriptionNode.Name == "subscription")
                            {
                                ISubscription LoadedSubscription = new Subscription(subscriptionNode);
                                if (!retValue.ContainsKey(LoadedSubscription.SubscriptionID))
                                {
                                    retValue.Add(LoadedSubscription.SubscriptionID, LoadedSubscription);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate SubscriptionID values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("subscription"))
                    {
                        JsonObject subscriptionObj = (array.Items[i] as JsonObject)["subscription"] as JsonObject;
                        ISubscription LoadedSubscription = new Subscription(subscriptionObj);
                        if (!retValue.ContainsKey(LoadedSubscription.SubscriptionID))
                        {
                            retValue.Add(LoadedSubscription.SubscriptionID, LoadedSubscription);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate SubscriptionID values detected");
                        }
                    }
                }
            }
            // return the list
            return retValue;
        }

        /// <summary>
        /// Get a list of all subscriptions for a customer.
        /// </summary>
        /// <param name="ChargifyID">The ChargifyID of the customer</param>
        /// <returns>A list of subscriptions</returns>
        public IDictionary<int, ISubscription> GetSubscriptionListForCustomer(int ChargifyID)
        {
            try
            {
                // make sure data is valid
                if (ChargifyID == int.MinValue) throw new ArgumentNullException("ChargifyID");
                // now make the request
                string response = this.DoRequest(string.Format("customers/{0}/subscriptions.{1}", ChargifyID, GetMethodExtension()));
                var retValue = new Dictionary<int, ISubscription>();
                if (response.IsXml())
                {
                    // now build customer object based on response XML
                    XmlDocument Doc = new XmlDocument();
                    Doc.LoadXml(response); // get the XML into an XML document
                    if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                    // loop through the child nodes of this node
                    foreach (XmlNode elementNode in Doc.ChildNodes)
                    {
                        if (elementNode.Name == "subscriptions")
                        {
                            foreach (XmlNode subscriptionNode in elementNode.ChildNodes)
                            {
                                if (subscriptionNode.Name == "subscription")
                                {
                                    ISubscription LoadedSubscription = new Subscription(subscriptionNode);
                                    if (!retValue.ContainsKey(LoadedSubscription.SubscriptionID))
                                    {
                                        retValue.Add(LoadedSubscription.SubscriptionID, LoadedSubscription);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Duplicate SubscriptionID values detected");
                                    }
                                }
                            }
                        }
                    }
                }
                else if (response.IsJSON())
                {
                    // should be expecting an array
                    int position = 0;
                    JsonArray array = JsonArray.Parse(response, ref position);
                    for (int i = 0; i <= array.Length - 1; i++)
                    {
                        if ((array.Items[i] as JsonObject).ContainsKey("subscription"))
                        {
                            JsonObject subscriptionObj = (array.Items[i] as JsonObject)["subscription"] as JsonObject;
                            ISubscription LoadedSubscription = new Subscription(subscriptionObj);
                            if (!retValue.ContainsKey(LoadedSubscription.SubscriptionID))
                            {
                                retValue.Add(LoadedSubscription.SubscriptionID, LoadedSubscription);
                            }
                            else
                            {
                                throw new InvalidOperationException("Duplicate SubscriptionID values detected");
                            }
                        }
                    }
                }
                return retValue;
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Create a new subscription without passing credit card information.
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="ChargifyID">The Chargify ID of the customer</param>
        /// <param name="PaymentCollectionMethod">Optional, type of payment collection method</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, int ChargifyID, PaymentCollectionMethod? PaymentCollectionMethod = PaymentCollectionMethod.Automatic)
        {
            // make sure data is valid
            if (ChargifyID == int.MinValue) throw new ArgumentException("Invalid Customer ID detected", "ChargifyID");
            // Create the subscription
            return CreateSubscription(ProductHandle, ChargifyID, string.Empty, PaymentCollectionMethod);
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="ChargifyID">The Chargify ID of the customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, int ChargifyID, ICreditCardAttributes CreditCardAttributes)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (ChargifyID == int.MinValue) throw new ArgumentException("Invalid Customer ID detected", "ChargifyID");

            return CreateSubscription(ProductHandle, ChargifyID, CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress,
                                      CreditCardAttributes.BillingCity, CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip,
                                      CreditCardAttributes.BillingCountry, string.Empty, CreditCardAttributes.FirstName, CreditCardAttributes.LastName);
        }

        /// <summary>
        /// Create a subscription using a coupon for discounted rate, without using credit card information.
        /// </summary>
        /// <param name="ProductHandle">The product to subscribe to</param>
        /// <param name="ChargifyID">The ID of the Customer to add the subscription for</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>If sucessful, the subscription object. Otherwise null.</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, int ChargifyID, string CouponCode)
        {
            if (ChargifyID == int.MinValue) throw new ArgumentException("Invalid Customer ID detected", "ChargifyID");
            if (string.IsNullOrEmpty(CouponCode)) throw new ArgumentException("CouponCode can't be empty", "CouponCode");
            return CreateSubscription(ProductHandle, ChargifyID, CouponCode, default(PaymentCollectionMethod?));
        }

        /// <summary>
        /// Create a subscription using a coupon for discounted rate
        /// </summary>
        /// <param name="ProductHandle">The product to subscribe to</param>
        /// <param name="ChargifyID">The ID of the Customer to add the subscription for</param>
        /// <param name="CreditCardAttributes">The credit card attributes to use for this transaction</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns></returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, int ChargifyID, ICreditCardAttributes CreditCardAttributes, string CouponCode)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (ChargifyID == int.MinValue) throw new ArgumentException("Invalid Customer ID detected", "ChargifyID");
            if (string.IsNullOrEmpty(CouponCode)) throw new ArgumentException("CouponCode can't be empty", "CouponCode");

            return CreateSubscription(ProductHandle, ChargifyID, CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress,
                                      CreditCardAttributes.BillingCity, CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip,
                                      CreditCardAttributes.BillingCountry, CouponCode);
        }

        /// <summary>
        /// Create a new subscription without requiring credit card information
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, string SystemID)
        {
            if (SystemID == string.Empty) throw new ArgumentException("Invalid system ID detected", "ChargifyID");
            return CreateSubscription(ProductHandle, SystemID, string.Empty);
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, string SystemID, ICreditCardAttributes CreditCardAttributes)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (SystemID == string.Empty) throw new ArgumentException("Invalid system ID detected", "ChargifyID");

            return CreateSubscription(ProductHandle, SystemID, CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress,
                                      CreditCardAttributes.BillingCity, CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip,
                                      CreditCardAttributes.BillingCountry, string.Empty);
        }

        /// <summary>
        /// Create a new subscription without passing credit card info
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, string SystemID, string CouponCode)
        {
            // make sure data is valid            
            if (SystemID == string.Empty) throw new ArgumentException("Invalid system customer ID detected", "ChargifyID");

            return CreateSubscription(ProductHandle, SystemID, CouponCode);
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, string SystemID, ICreditCardAttributes CreditCardAttributes, string CouponCode)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (SystemID == string.Empty) throw new ArgumentException("Invalid system customer ID detected", "ChargifyID");

            return CreateSubscription(ProductHandle, SystemID, CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress,
                                      CreditCardAttributes.BillingCity, CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip,
                                      CreditCardAttributes.BillingCountry, CouponCode);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time without submitting PaymentProfile attributes
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <returns>The xml describing the new subsscription</returns>
        /// <param name="PaymentCollectionMethod">The type of subscription, recurring (automatic) billing, or invoice (if applicable)</param>
        public ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, PaymentCollectionMethod? PaymentCollectionMethod = PaymentCollectionMethod.Automatic)
        {
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName, CustomerAttributes.LastName,
                CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization, string.Empty, DateTime.MinValue, null, PaymentCollectionMethod);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time and import the card data from a specific vault storage
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="ExistingProfile">Data concerning the existing profile in vault storage</param>
        /// <returns>The xml describing the new subscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, DateTime NextBillingAt, IPaymentProfileAttributes ExistingProfile)
        {
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            if (ExistingProfile == null) throw new ArgumentNullException("ExistingProfile");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName,
                                      CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization, string.Empty, NextBillingAt, ExistingProfile, null);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time and use the card data from another payment profile (from the same customer).
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="ExistingProfileID">The ID of the existing payment profile to use when creating the new subscription.</param>
        /// <returns>The new subscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, DateTime NextBillingAt, int ExistingProfileID)
        {
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            if (ExistingProfileID <= 0) throw new ArgumentNullException("ExistingProfileID");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName,
                                      CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization, string.Empty, NextBillingAt, ExistingProfileID);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <returns>The xml describing the new subscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, DateTime NextBillingAt)
        {
            // version bump
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (NextBillingAt == DateTime.MinValue) throw new ArgumentOutOfRangeException("NextBillingAt");
            if (NextBillingAt == null) throw new ArgumentNullException("NextBillingAt");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName,
                                      CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization, CustomerAttributes.ShippingAddress, CustomerAttributes.ShippingCity,
                                      CustomerAttributes.ShippingState, CustomerAttributes.ShippingZip, CustomerAttributes.ShippingCountry, CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress, CreditCardAttributes.BillingCity, CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip,
                                      CreditCardAttributes.BillingCountry, string.Empty, null, NextBillingAt);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes,
                                                       ICreditCardAttributes CreditCardAttributes)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName,
                                      CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization,
                                      CustomerAttributes.ShippingAddress, CustomerAttributes.ShippingCity, CustomerAttributes.ShippingState, CustomerAttributes.ShippingZip, CustomerAttributes.ShippingCountry,
                                      CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress, CreditCardAttributes.BillingCity,
                                      CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip, CreditCardAttributes.BillingCountry, string.Empty, int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="ComponentsWithQuantity">The components to set on the subscription initially</param>
        /// <returns></returns>
        public ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, Dictionary<int, string> ComponentsWithQuantity)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName,
                                      CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization, CustomerAttributes.ShippingAddress, CustomerAttributes.ShippingCity, CustomerAttributes.ShippingState, CustomerAttributes.ShippingZip, CustomerAttributes.ShippingCountry,
                                      CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress, CreditCardAttributes.BillingCity,
                                      CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip, CreditCardAttributes.BillingCountry, string.Empty, ComponentsWithQuantity, null);
        }

        private ISubscription CreateSubscription(string ProductHandle, string NewSystemID, string FirstName, string LastName, string EmailAddress, string Phone,
                                                        string Organization, string ShippingAddress, string ShippingCity, string ShippingState, string ShippingZip, string ShippingCountry,
                                                        string FullNumber, int ExpirationMonth, int ExpirationYear,
                                                        string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip,
                                                        string BillingCountry, string CouponCode, Dictionary<int, string> ComponentsWithQuantity, DateTime? NextBillingAt)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            var product = this.LoadProduct(ProductHandle);
            if (product == null) throw new ArgumentException("The product doesn't exist", ProductHandle);
            // if ((ComponentsWithQuantity.Count < 0)) throw new ArgumentNullException("ComponentsWithQuantity", "No components specified");

            if (string.IsNullOrEmpty(FirstName)) throw new ArgumentNullException("FirstName");
            if (string.IsNullOrEmpty(LastName)) throw new ArgumentNullException("LastName");
            if (string.IsNullOrEmpty(EmailAddress)) throw new ArgumentNullException("EmailAddress");
            if (string.IsNullOrEmpty(FullNumber)) throw new ArgumentNullException("FullNumber");
            //if (NewSystemID == string.Empty) throw new ArgumentNullException("NewSystemID");
            if ((ExpirationMonth <= 0) && (ExpirationMonth > 12)) throw new ArgumentException("Not within range", "ExpirationMonth");
            if (ExpirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", "ExpirationYear");
            if (this._cvvRequired && string.IsNullOrEmpty(CVV)) throw new ArgumentNullException("CVV");
            if (this._cvvRequired && ((CVV.Length < 3) || (CVV.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "CVV");

            //if (!string.IsNullOrEmpty(NewSystemID))
            //{
            //    // make sure that the system ID is unique
            //    if (this.LoadCustomer(NewSystemID) != null) throw new ArgumentException("Not unique", "NewSystemID");
            //}

            // create XML for creation of customer
            var subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            subscriptionXml.Append("<customer_attributes>");
            subscriptionXml.AppendFormat("<first_name>{0}</first_name>", FirstName);
            subscriptionXml.AppendFormat("<last_name>{0}</last_name>", LastName);
            subscriptionXml.AppendFormat("<email>{0}</email>", EmailAddress);
            if (!string.IsNullOrEmpty(Phone)) subscriptionXml.AppendFormat("<phone>{0}</phone>", Phone);
            subscriptionXml.AppendFormat("<organization>{0}</organization>", (Organization != null) ? HttpUtility.HtmlEncode(Organization) : "null");
            subscriptionXml.AppendFormat("<reference>{0}</reference>", NewSystemID.ToString());
            if (!string.IsNullOrEmpty(ShippingAddress)) subscriptionXml.AppendFormat("<address>{0}</address>", ShippingAddress);
            if (!string.IsNullOrEmpty(ShippingCity)) subscriptionXml.AppendFormat("<city>{0}</city>", ShippingCity);
            if (!string.IsNullOrEmpty(ShippingState)) subscriptionXml.AppendFormat("<state>{0}</state>", ShippingState);
            if (!string.IsNullOrEmpty(ShippingZip)) subscriptionXml.AppendFormat("<zip>{0}</zip>", ShippingZip);
            if (!string.IsNullOrEmpty(ShippingCountry)) subscriptionXml.AppendFormat("<country>{0}</country>", ShippingCountry);
            subscriptionXml.Append("</customer_attributes>");
            subscriptionXml.Append("<credit_card_attributes>");
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", FullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", ExpirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", ExpirationYear);
            if (this._cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", CVV); }
            if (!string.IsNullOrEmpty(BillingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", BillingAddress);
            if (!string.IsNullOrEmpty(BillingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", BillingCity);
            if (!string.IsNullOrEmpty(BillingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", BillingState);
            if (!string.IsNullOrEmpty(BillingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", BillingZip);
            if (!string.IsNullOrEmpty(BillingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", BillingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(CouponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); }
            if (NextBillingAt.HasValue) { subscriptionXml.AppendFormat("<next_billing_at>{0}</next_billing_at>", NextBillingAt.Value.ToString("o")); }
            if (ComponentsWithQuantity != null && ComponentsWithQuantity.Count > 0)
            {
                subscriptionXml.Append(@"<components type=""array"">");
                foreach (var item in ComponentsWithQuantity)
                {
                    subscriptionXml.Append("<component>");
                    subscriptionXml.Append(string.Format("<component_id>{0}</component_id>", item.Key));
                    subscriptionXml.Append(string.Format("<allocated_quantity>{0}</allocated_quantity>", item.Value));
                    subscriptionXml.Append("</component>");
                }
                subscriptionXml.Append("</components>");
            }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription, specifying a coupon
        /// </summary>
        /// <param name="ProductHandle">The product to subscribe to</param>
        /// <param name="CustomerAttributes">Details about the customer</param>
        /// <param name="CreditCardAttributes">Payment details</param>
        /// <param name="CouponCode">The coupon to use</param>
        /// <param name="ComponentsWithQuantity">Components to set on the subscription initially</param>
        /// <returns>Details about the subscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, string CouponCode, Dictionary<int, string> ComponentsWithQuantity)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName, CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization,
                                      CustomerAttributes.ShippingAddress, CustomerAttributes.ShippingCity, CustomerAttributes.ShippingState, CustomerAttributes.ShippingZip, CustomerAttributes.ShippingCountry,
                                      CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress, CreditCardAttributes.BillingCity,
                                      CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip, CreditCardAttributes.BillingCountry, CouponCode, ComponentsWithQuantity, null);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="ComponentID">The component to allocate when creating the subscription</param>
        /// <param name="AllocatedQuantity">The quantity to allocate of the component when creating the subscription</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, int ComponentID, int AllocatedQuantity)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName,
                                      CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization,
                                      CustomerAttributes.ShippingAddress, CustomerAttributes.ShippingCity, CustomerAttributes.ShippingState, CustomerAttributes.ShippingZip, CustomerAttributes.ShippingCountry,
                                      CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress, CreditCardAttributes.BillingCity,
                                      CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip, CreditCardAttributes.BillingCountry, string.Empty, ComponentID, AllocatedQuantity);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time using no credit card information
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, string CouponCode)
        {
            // make sure data is valid
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName, CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization, CouponCode, DateTime.MinValue, null, null);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time using no credit card information
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="ExistingProfile">Data concerning the existing profile in vault storage</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, string CouponCode, DateTime NextBillingAt, IPaymentProfileAttributes ExistingProfile)
        {
            // make sure data is valid
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            if (ExistingProfile == null) throw new ArgumentNullException("ExistingProfile");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName, CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization, CouponCode, NextBillingAt, ExistingProfile, null);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time using no credit card information
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="ExistingProfileID">The ID of the data concerning the existing profile in vault storage</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, string CouponCode, DateTime NextBillingAt, int ExistingProfileID)
        {
            // make sure data is valid
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            if (ExistingProfileID <= 0) throw new ArgumentNullException("ExistingProfileID");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName, CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization, CouponCode, NextBillingAt, ExistingProfileID);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes,
                                                       ICreditCardAttributes CreditCardAttributes, string CouponCode)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName,
                                      CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization,
                                      CustomerAttributes.ShippingAddress, CustomerAttributes.ShippingCity, CustomerAttributes.ShippingState, CustomerAttributes.ShippingZip, CustomerAttributes.ShippingCountry,
                                      CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress, CreditCardAttributes.BillingCity,
                                      CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip, CreditCardAttributes.BillingCountry, CouponCode, int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="NextBillingAt">Specify the time of first assessment</param>
        /// <returns>The new subscription object</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, DateTime NextBillingAt, string CouponCode)
        {
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            if (NextBillingAt == null || NextBillingAt == DateTime.MinValue) throw new ArgumentNullException("NextBillingAt");

            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName,
                                      CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization,
                                      CustomerAttributes.ShippingAddress, CustomerAttributes.ShippingCity, CustomerAttributes.ShippingState, CustomerAttributes.ShippingZip, CustomerAttributes.ShippingCountry,
                                      CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress, CreditCardAttributes.BillingCity,
                                      CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip, CreditCardAttributes.BillingCountry, CouponCode, null, NextBillingAt);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="ComponentID">The component to allocate when creating the subscription</param>
        /// <param name="AllocatedQuantity">The quantity to allocate of the component when creating the subscription</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes,
                                                       ICreditCardAttributes CreditCardAttributes, string CouponCode, int ComponentID, int AllocatedQuantity)
        {
            // make sure data is valid
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            if (CustomerAttributes == null) throw new ArgumentNullException("CustomerAttributes");
            return CreateSubscription(ProductHandle, CustomerAttributes.SystemID, CustomerAttributes.FirstName,
                                      CustomerAttributes.LastName, CustomerAttributes.Email, CustomerAttributes.Phone, CustomerAttributes.Organization,
                                      CustomerAttributes.ShippingAddress, CustomerAttributes.ShippingCity, CustomerAttributes.ShippingState, CustomerAttributes.ShippingZip, CustomerAttributes.ShippingCountry,
                                      CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth,
                                      CreditCardAttributes.ExpirationYear, CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress, CreditCardAttributes.BillingCity,
                                      CreditCardAttributes.BillingState, CreditCardAttributes.BillingZip, CreditCardAttributes.BillingCountry, CouponCode, ComponentID, AllocatedQuantity);
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="ChargifyID">The Chargify ID of the customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="PaymentCollectionMethod">Optional, type of payment collection method</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string ProductHandle, int ChargifyID, string CouponCode, PaymentCollectionMethod? PaymentCollectionMethod)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            if (ChargifyID == int.MinValue) throw new ArgumentNullException("ChargifyID");

            // make sure that the system ID is unique
            if (this.LoadCustomer(ChargifyID) == null) throw new ArgumentException("Customer Not Found", "SystemID");

            IProduct subscribingProduct = this.LoadProduct(ProductHandle);
            if (subscribingProduct == null) throw new ArgumentException("Product not found");
            if (subscribingProduct.RequireCreditCard) throw new ChargifyNetException("Product requires credit card information");

            // create XML for creation of customer
            StringBuilder SubscriptionXML = new StringBuilder(GetXMLStringIfApplicable());
            SubscriptionXML.Append("<subscription>");
            SubscriptionXML.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            SubscriptionXML.AppendFormat("<customer_id>{0}</customer_id>", ChargifyID);
            if (!string.IsNullOrEmpty(CouponCode)) { SubscriptionXML.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); }
            if (PaymentCollectionMethod.HasValue)
            {
                if (PaymentCollectionMethod.Value != ChargifyNET.PaymentCollectionMethod.Unknown) { SubscriptionXML.AppendFormat("<payment_collection_method>{0}</payment_collection_method>", Enum.GetName(typeof(PaymentCollectionMethod), PaymentCollectionMethod.Value).ToLowerInvariant()); }
            }
            SubscriptionXML.Append("</subscription>");

            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, SubscriptionXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="ChargifyID">The Chargify ID of the customer</param>
        /// <param name="FullNumber">The full number of the credit card</param>
        /// <param name="ExpirationMonth">The expritation month of the credit card</param>
        /// <param name="ExpirationYear">The expiration year of the credit card</param>
        /// <param name="CVV">The CVV for the credit card</param>
        /// <param name="BillingAddress">The billing address</param>
        /// <param name="BillingCity">The billing city</param>
        /// <param name="BillingState">The billing state or province</param>
        /// <param name="BillingZip">The billing zip code or postal code</param>
        /// <param name="BillingCountry">The billing country</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="FirstName">The first name, as it appears on the credit card</param>
        /// <param name="LastName">The last name, as it appears on the credit card</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string ProductHandle, int ChargifyID, string FullNumber, int ExpirationMonth, int ExpirationYear,
                                                        string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip,
                                                        string BillingCountry, string CouponCode, string FirstName, string LastName)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            if (ChargifyID == int.MinValue) throw new ArgumentNullException("ChargifyID");
            // make sure that the system ID is unique
            if (this.LoadCustomer(ChargifyID) == null) throw new ArgumentException("Customer Not Found", "SystemID");
            if (string.IsNullOrEmpty(FullNumber)) throw new ArgumentNullException("FullNumber");
            if ((ExpirationMonth <= 0) && (ExpirationMonth > 12)) throw new ArgumentException("Not within range", "ExpirationMonth");
            if (ExpirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", "ExpirationYear");
            if (this._cvvRequired && string.IsNullOrEmpty(CVV)) throw new ArgumentNullException("CVV");

            // Since the hosted pages don't necessarily use these - I'm not sure if we should be including them.
            //if (string.IsNullOrEmpty(BillingZip)) throw new ArgumentNullException("BillingZip");
            //if (string.IsNullOrEmpty(BillingAddress)) throw new ArgumentNullException("BillingAddress");
            //if (string.IsNullOrEmpty(BillingCity)) throw new ArgumentNullException("BillingCity");
            //if (string.IsNullOrEmpty(BillingState)) throw new ArgumentNullException("BillingState");
            //if (string.IsNullOrEmpty(BillingCountry)) throw new ArgumentNullException("BillingCountry");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            subscriptionXml.AppendFormat("<customer_id>{0}</customer_id>", ChargifyID);
            subscriptionXml.Append("<credit_card_attributes>");
            if (!string.IsNullOrEmpty(FirstName)) { subscriptionXml.AppendFormat("<first_name>{0}</first_name>", FirstName); }
            if (!string.IsNullOrEmpty(LastName)) { subscriptionXml.AppendFormat("<last_name>{0}</last_name>", LastName); }
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", FullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", ExpirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", ExpirationYear);
            if (this._cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", CVV); }
            if (!string.IsNullOrEmpty(BillingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", BillingAddress);
            if (!string.IsNullOrEmpty(BillingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", BillingCity);
            if (!string.IsNullOrEmpty(BillingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", BillingState);
            if (!string.IsNullOrEmpty(BillingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", BillingZip);
            if (!string.IsNullOrEmpty(BillingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", BillingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(CouponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="ChargifyID">The Chargify ID of the customer</param>
        /// <param name="FullNumber">The full number of the credit card</param>
        /// <param name="ExpirationMonth">The expritation month of the credit card</param>
        /// <param name="ExpirationYear">The expiration year of the credit card</param>
        /// <param name="CVV">The CVV for the credit card</param>
        /// <param name="BillingAddress">The billing address</param>
        /// <param name="BillingCity">The billing city</param>
        /// <param name="BillingState">The billing state or province</param>
        /// <param name="BillingZip">The billing zip code or postal code</param>
        /// <param name="BillingCountry">The billing country</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string ProductHandle, int ChargifyID, string FullNumber, int ExpirationMonth, int ExpirationYear,
                                                        string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip,
                                                        string BillingCountry, string CouponCode)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            if (ChargifyID == int.MinValue) throw new ArgumentNullException("ChargifyID");
            // make sure that the system ID is unique
            if (this.LoadCustomer(ChargifyID) == null) throw new ArgumentException("Customer Not Found", "SystemID");
            if (string.IsNullOrEmpty(FullNumber)) throw new ArgumentNullException("FullNumber");
            if ((ExpirationMonth <= 0) && (ExpirationMonth > 12)) throw new ArgumentException("Not within range", "ExpirationMonth");
            if (ExpirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", "ExpirationYear");
            if (this._cvvRequired && string.IsNullOrEmpty(CVV)) throw new ArgumentNullException("CVV");
            if (this._cvvRequired && ((CVV.Length < 3) || (CVV.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "CVV");

            // Since the hosted pages don't use these - I'm not sure if we should be including them.
            //if (string.IsNullOrEmpty(BillingZip)) throw new ArgumentNullException("BillingZip");
            //if (string.IsNullOrEmpty(BillingAddress)) throw new ArgumentNullException("BillingAddress");
            //if (string.IsNullOrEmpty(BillingCity)) throw new ArgumentNullException("BillingCity");
            //if (string.IsNullOrEmpty(BillingState)) throw new ArgumentNullException("BillingState");
            //if (string.IsNullOrEmpty(BillingCountry)) throw new ArgumentNullException("BillingCountry");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            subscriptionXml.AppendFormat("<customer_id>{0}</customer_id>", ChargifyID);
            subscriptionXml.Append("<credit_card_attributes>");
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", FullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", ExpirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", ExpirationYear);
            if (this._cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", CVV); }
            if (!string.IsNullOrEmpty(BillingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", BillingAddress);
            if (!string.IsNullOrEmpty(BillingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", BillingCity);
            if (!string.IsNullOrEmpty(BillingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", BillingState);
            if (!string.IsNullOrEmpty(BillingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", BillingZip);
            if (!string.IsNullOrEmpty(BillingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", BillingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(CouponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <param name="CouponCode">The discount coupon code</param> 
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string ProductHandle, string SystemID, string CouponCode)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            if (SystemID == string.Empty) throw new ArgumentNullException("SystemID");

            // make sure that the system ID is unique
            if (this.LoadCustomer(SystemID) == null) throw new ArgumentException("Customer Not Found", "SystemID");

            IProduct subscribingProduct = this.LoadProduct(ProductHandle);
            if (subscribingProduct == null) throw new ArgumentException("Product not found", "ProductHandle");
            if (subscribingProduct.RequireCreditCard) throw new ChargifyNetException("Product requires credit card information");

            // create XML for creation of customer
            StringBuilder SubscriptionXML = new StringBuilder(GetXMLStringIfApplicable());
            SubscriptionXML.Append("<subscription>");
            SubscriptionXML.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            SubscriptionXML.AppendFormat("<customer_reference>{0}</customer_reference>", SystemID.ToString());
            if (!string.IsNullOrEmpty(CouponCode)) { SubscriptionXML.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); }
            SubscriptionXML.Append("</subscription>");

            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, SubscriptionXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <param name="FullNumber">The full number of the credit card</param>
        /// <param name="ExpirationMonth">The expritation month of the credit card</param>
        /// <param name="ExpirationYear">The expiration year of the credit card</param>
        /// <param name="CVV">The CVV for the credit card</param>
        /// <param name="BillingAddress">The billing address</param>
        /// <param name="BillingCity">The billing city</param>
        /// <param name="BillingState">The billing state or province</param>
        /// <param name="BillingZip">The billing zip code or postal code</param>
        /// <param name="BillingCountry">The billing country</param>
        /// <param name="CouponCode">The discount coupon code</param> 
        /// <param name="FirstName">The first name, as listed on the credit card</param>
        /// <param name="LastName">The last name, as listed on the credit card</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string ProductHandle, string SystemID, string FullNumber, int ExpirationMonth, int ExpirationYear,
                                                        string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip,
                                                        string BillingCountry, string CouponCode, string FirstName, string LastName)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            var product = this.LoadProduct(ProductHandle);
            if (product == null) throw new ArgumentException("That product doesn't exist", "ProductHandle");
            if (SystemID == string.Empty) throw new ArgumentNullException("SystemID");
            // make sure that the system ID is unique
            if (this.LoadCustomer(SystemID) == null) throw new ArgumentException("Customer Not Found", "SystemID");
            if (product.RequireCreditCard)
            {
                if (string.IsNullOrEmpty(FullNumber)) throw new ArgumentNullException("FullNumber");
                if ((ExpirationMonth <= 0) && (ExpirationMonth > 12)) throw new ArgumentException("Not within range", "ExpirationMonth");
                if (ExpirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", "ExpirationYear");
                if (this._cvvRequired && string.IsNullOrEmpty(CVV)) throw new ArgumentNullException("CVV");
                if (this._cvvRequired && ((CVV.Length < 3) || (CVV.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "CVV");
            }
            // Don't throw exceptions on these, since we don't know if they are absolutely needed.
            //if (string.IsNullOrEmpty(BillingAddress)) throw new ArgumentNullException("BillingAddress");
            //if (string.IsNullOrEmpty(BillingCity)) throw new ArgumentNullException("BillingCity");
            //if (string.IsNullOrEmpty(BillingState)) throw new ArgumentNullException("BillingState");
            //if (string.IsNullOrEmpty(BillingZip)) throw new ArgumentNullException("BillingZip");
            //if (string.IsNullOrEmpty(BillingCountry)) throw new ArgumentNullException("BillingCountry");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            subscriptionXml.AppendFormat("<customer_reference>{0}</customer_reference>", SystemID.ToString());
            if (product.RequireCreditCard)
            {
                subscriptionXml.Append("<credit_card_attributes>");
                if (!string.IsNullOrEmpty(FirstName)) { subscriptionXml.AppendFormat("<first_name>{0}</first_name>", FirstName); }
                if (!string.IsNullOrEmpty(LastName)) { subscriptionXml.AppendFormat("<last_name>{0}</last_name>", LastName); }
                subscriptionXml.AppendFormat("<full_number>{0}</full_number>", FullNumber);
                subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", ExpirationMonth);
                subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", ExpirationYear);
                if (this._cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", CVV); }
                if (!string.IsNullOrEmpty(BillingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", BillingAddress);
                if (!string.IsNullOrEmpty(BillingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", BillingCity);
                if (!string.IsNullOrEmpty(BillingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", BillingState);
                if (!string.IsNullOrEmpty(BillingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", BillingZip);
                if (!string.IsNullOrEmpty(BillingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", BillingCountry);
                subscriptionXml.Append("</credit_card_attributes>");
            }
            if (!string.IsNullOrEmpty(CouponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <param name="FullNumber">The full number of the credit card</param>
        /// <param name="ExpirationMonth">The expritation month of the credit card</param>
        /// <param name="ExpirationYear">The expiration year of the credit card</param>
        /// <param name="CVV">The CVV for the credit card</param>
        /// <param name="BillingAddress">The billing address</param>
        /// <param name="BillingCity">The billing city</param>
        /// <param name="BillingState">The billing state or province</param>
        /// <param name="BillingZip">The billing zip code or postal code</param>
        /// <param name="BillingCountry">The billing country</param>
        /// <param name="CouponCode">The discount coupon code</param> 
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string ProductHandle, string SystemID, string FullNumber, int ExpirationMonth, int ExpirationYear,
                                                        string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip,
                                                        string BillingCountry, string CouponCode)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            if (SystemID == string.Empty) throw new ArgumentNullException("SystemID");
            // make sure that the system ID is unique
            if (this.LoadCustomer(SystemID) == null) throw new ArgumentException("Customer Not Found", "SystemID");
            if (string.IsNullOrEmpty(FullNumber)) throw new ArgumentNullException("FullNumber");
            if ((ExpirationMonth <= 0) && (ExpirationMonth > 12)) throw new ArgumentException("Not within range", "ExpirationMonth");
            if (ExpirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", "ExpirationYear");
            if (this._cvvRequired && string.IsNullOrEmpty(CVV)) throw new ArgumentNullException("CVV");
            if (this._cvvRequired && ((CVV.Length < 3) || (CVV.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "CVV");
            //if (string.IsNullOrEmpty(BillingAddress)) throw new ArgumentNullException("BillingAddress");
            //if (string.IsNullOrEmpty(BillingCity)) throw new ArgumentNullException("BillingCity");
            //if (string.IsNullOrEmpty(BillingState)) throw new ArgumentNullException("BillingState");
            //if (string.IsNullOrEmpty(BillingZip)) throw new ArgumentNullException("BillingZip");
            //if (string.IsNullOrEmpty(BillingCountry)) throw new ArgumentNullException("BillingCountry");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            subscriptionXml.AppendFormat("<customer_reference>{0}</customer_reference>", SystemID.ToString());
            subscriptionXml.Append("<credit_card_attributes>");
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", FullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", ExpirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", ExpirationYear);
            if (this._cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", CVV); }
            if (!string.IsNullOrEmpty(BillingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", BillingAddress);
            if (!string.IsNullOrEmpty(BillingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", BillingCity);
            if (!string.IsNullOrEmpty(BillingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", BillingState);
            if (!string.IsNullOrEmpty(BillingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", BillingZip);
            if (!string.IsNullOrEmpty(BillingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", BillingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(CouponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="NewSystemID">The reference field value of the customer</param>
        /// <param name="FirstName">The first name of the customer</param>
        /// <param name="LastName">The last name of the customer</param>
        /// <param name="EmailAddress">The email address of the customer</param>
        /// <param name="Phone">The phone number of the customer</param>
        /// <param name="Organization">The customer's organization</param>
        /// <param name="CouponCode">The discount coupon code</param> 
        /// <param name="NextBillingAt">The next date that the billing should be processed (DateTime.Min if unspecified)</param>
        /// <param name="PaymentProfileID">The id of the payment profile to use when creating the subscription (existing data)</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string ProductHandle, string NewSystemID, string FirstName, string LastName, string EmailAddress, string Phone,
                                                 string Organization, string CouponCode, DateTime NextBillingAt, int PaymentProfileID)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            if (string.IsNullOrEmpty(FirstName)) throw new ArgumentNullException("FirstName");
            if (string.IsNullOrEmpty(LastName)) throw new ArgumentNullException("LastName");
            if (string.IsNullOrEmpty(EmailAddress)) throw new ArgumentNullException("EmailAddress");

            ICustomer existingCustomer = this.LoadCustomer(NewSystemID);
            IProduct subscribingProduct = this.LoadProduct(ProductHandle);
            if (subscribingProduct == null) throw new ArgumentException("Product not found", "ProductHandle");

            // create XML for creation of customer
            StringBuilder SubscriptionXML = new StringBuilder(GetXMLStringIfApplicable());
            SubscriptionXML.Append("<subscription>");
            SubscriptionXML.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);

            if (existingCustomer == null)
            {
                SubscriptionXML.Append("<customer_attributes>");
                SubscriptionXML.AppendFormat("<first_name>{0}</first_name>", FirstName);
                SubscriptionXML.AppendFormat("<last_name>{0}</last_name>", LastName);
                SubscriptionXML.AppendFormat("<email>{0}</email>", EmailAddress);
                if (!String.IsNullOrEmpty(Phone)) SubscriptionXML.AppendFormat("<phone>{0}</phone>", Phone);
                SubscriptionXML.AppendFormat("<organization>{0}</organization>", (Organization != null) ? HttpUtility.HtmlEncode(Organization) : "null");
                SubscriptionXML.AppendFormat("<reference>{0}</reference>", NewSystemID.ToString());
                SubscriptionXML.Append("</customer_attributes>");
            }
            else
            {
                SubscriptionXML.AppendFormat("<customer_id>{0}</customer_id>", existingCustomer.ChargifyID);
            }

            if (!string.IsNullOrEmpty(CouponCode)) { SubscriptionXML.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); } // Optional
            SubscriptionXML.AppendFormat("<payment_profile_id>{0}</payment_profile_id>", PaymentProfileID);
            if (NextBillingAt != DateTime.MinValue) { SubscriptionXML.AppendFormat("<next_billing_at>{0}</next_billing_at>", NextBillingAt); }
            SubscriptionXML.Append("</subscription>");

            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, SubscriptionXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="NewSystemID">The reference field value of the customer</param>
        /// <param name="FirstName">The first name of the customer</param>
        /// <param name="LastName">The last name of the customer</param>
        /// <param name="EmailAddress">The email address of the customer</param>
        /// <param name="Phone">The phone number of the customer</param>
        /// <param name="Organization">The customer's organization</param>
        /// <param name="CouponCode">The discount coupon code</param> 
        /// <param name="NextBillingAt">The next date that the billing should be processed</param>
        /// <param name="PaymentProfile">The paymentProfile object to use when creating the subscription (existing data)</param>
        /// <param name="PaymentCollectionMethod">The type of subscription, recurring (automatic) billing, or invoice (if applicable)</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string ProductHandle, string NewSystemID, string FirstName, string LastName, string EmailAddress, string Phone,
                                                 string Organization, string CouponCode, DateTime NextBillingAt, IPaymentProfileAttributes PaymentProfile, PaymentCollectionMethod? PaymentCollectionMethod)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            if (string.IsNullOrEmpty(FirstName)) throw new ArgumentNullException("FirstName");
            if (string.IsNullOrEmpty(LastName)) throw new ArgumentNullException("LastName");
            if (string.IsNullOrEmpty(EmailAddress)) throw new ArgumentNullException("EmailAddress");

            //if (!string.IsNullOrEmpty(NewSystemID))
            //{
            //    // make sure that the system ID is unique
            //    if (this.LoadCustomer(NewSystemID) != null) throw new ArgumentException("Not unique", "NewSystemID");
            //}
            IProduct subscribingProduct = this.LoadProduct(ProductHandle);
            if (subscribingProduct == null) throw new ArgumentException("Product not found", "ProductHandle");
            if (subscribingProduct.RequireCreditCard)
            {
                // Product requires credit card and no payment information passed in.
                if (PaymentProfile == null) throw new ChargifyNetException("Product requires credit card information");
            }

            // create XML for creation of customer
            var subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            subscriptionXml.Append("<customer_attributes>");
            subscriptionXml.AppendFormat("<first_name>{0}</first_name>", FirstName);
            subscriptionXml.AppendFormat("<last_name>{0}</last_name>", LastName);
            subscriptionXml.AppendFormat("<email>{0}</email>", EmailAddress);
            if (!String.IsNullOrEmpty(Phone)) subscriptionXml.AppendFormat("<phone>{0}</phone>", Phone);
            subscriptionXml.AppendFormat("<organization>{0}</organization>", (Organization != null) ? HttpUtility.HtmlEncode(Organization) : "null");
            subscriptionXml.AppendFormat("<reference>{0}</reference>", NewSystemID.ToString());
            subscriptionXml.Append("</customer_attributes>");
            if (!string.IsNullOrEmpty(CouponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); } // Optional
            if (PaymentProfile != null)
            {
                // The round-trip "o" format uses ISO 8601 for date/time representation, neat.
                subscriptionXml.AppendFormat("<next_billing_at>{0}</next_billing_at>", NextBillingAt.ToString("o"));
                subscriptionXml.Append("<payment_profile_attributes>");
                subscriptionXml.AppendFormat("<vault_token>{0}</vault_token>", PaymentProfile.VaultToken);
                subscriptionXml.AppendFormat("<customer_vault_token>{0}</customer_vault_token>", PaymentProfile.CustomerVaultToken);
                subscriptionXml.AppendFormat("<current_vault>{0}</current_vault>", PaymentProfile.CurrentVault.ToString().ToLowerInvariant());
                subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", PaymentProfile.ExpirationYear);
                subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", PaymentProfile.ExpirationMonth);
                if (PaymentProfile.CardType != CardType.Unknown) { subscriptionXml.AppendFormat("<card_type>{0}</card_type>", PaymentProfile.CardType.ToString().ToLowerInvariant()); } // Optional
                if (PaymentProfile.LastFour != String.Empty) { subscriptionXml.AppendFormat("<last_four>{0}</last_four>", PaymentProfile.LastFour); } // Optional
                if (PaymentProfile.BankName != String.Empty) { subscriptionXml.AppendFormat("<bank_name>{0}</bank_name>", PaymentProfile.BankName); }
                if (PaymentProfile.BankRoutingNumber != String.Empty) { subscriptionXml.AppendFormat("<bank_routing_number>{0}</bank_routing_number>", PaymentProfile.BankRoutingNumber); }
                if (PaymentProfile.BankAccountNumber != String.Empty) { subscriptionXml.AppendFormat("<bank_account_number>{0}</bank_account_number>", PaymentProfile.BankAccountNumber); }
                if (PaymentProfile.BankAccountType != BankAccountType.Unknown) { subscriptionXml.AppendFormat("<bank_account_type>{0}</bank_account_type>", PaymentProfile.BankAccountType.ToString().ToLowerInvariant()); }
                if (PaymentProfile.BankAccountHolderType != BankAccountHolderType.Unknown) { subscriptionXml.AppendFormat("<bank_account_holder_type>{0}</bank_account_holder_type>", PaymentProfile.BankAccountHolderType.ToString().ToLowerInvariant()); }
                subscriptionXml.Append("</payment_profile_attributes>");
            }
            if (PaymentCollectionMethod.HasValue)
            {
                if (PaymentCollectionMethod.Value != ChargifyNET.PaymentCollectionMethod.Unknown) { subscriptionXml.AppendFormat("<payment_collection_method>{0}</payment_collection_method>", Enum.GetName(typeof(PaymentCollectionMethod), PaymentCollectionMethod.Value).ToLowerInvariant()); }
            }
            subscriptionXml.Append("</subscription>");

            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="NewSystemID">The system ID for the new customer</param>
        /// <param name="FirstName">The first name of the new customer</param>
        /// <param name="LastName">The last nameof the new customer</param>
        /// <param name="EmailAddress">The email address for the new customer</param>
        /// <param name="Phone">The phone number for the customer</param>
        /// <param name="Organization">The organization of the new customer</param>
        /// <param name="ShippingAddress">The shipping address of the customer</param>
        /// <param name="ShippingCity">The shipping city of the customer</param>
        /// <param name="ShippingState">The shipping state of the customer</param>
        /// <param name="ShippingZip">The shipping zip of the customer</param>
        /// <param name="ShippingCountry">The shipping country of the customer</param>
        /// <param name="FullNumber">The full number of the credit card</param>
        /// <param name="ExpirationMonth">The expritation month of the credit card</param>
        /// <param name="ExpirationYear">The expiration year of the credit card</param>
        /// <param name="CVV">The CVV for the credit card</param>
        /// <param name="BillingAddress">The billing address</param>
        /// <param name="BillingCity">The billing city</param>
        /// <param name="BillingState">The billing state or province</param>
        /// <param name="BillingZip">The billing zip code or postal code</param>
        /// <param name="BillingCountry">The billing country</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="ComponentID">The component to add while creating the subscription</param>
        /// <param name="AllocatedQuantity">The quantity of the component to allocate when creating the component usage for the new subscription</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string ProductHandle, string NewSystemID, string FirstName, string LastName, string EmailAddress, string Phone,
                                                        string Organization, string ShippingAddress, string ShippingCity, string ShippingState, string ShippingZip, string ShippingCountry,
                                                        string FullNumber, int ExpirationMonth, int ExpirationYear,
                                                        string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip,
                                                        string BillingCountry, string CouponCode, int ComponentID, int AllocatedQuantity)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");
            var product = this.LoadProduct(ProductHandle);
            if (product == null) throw new ArgumentException("The product doesn't exist", ProductHandle);
            if ((ComponentID != int.MinValue) && (AllocatedQuantity == int.MinValue)) throw new ArgumentNullException("AllocatedQuantity");

            if (string.IsNullOrEmpty(FirstName)) throw new ArgumentNullException("FirstName");
            if (string.IsNullOrEmpty(LastName)) throw new ArgumentNullException("LastName");
            if (string.IsNullOrEmpty(EmailAddress)) throw new ArgumentNullException("EmailAddress");
            if (string.IsNullOrEmpty(FullNumber)) throw new ArgumentNullException("FullNumber");
            //if (NewSystemID == string.Empty) throw new ArgumentNullException("NewSystemID");
            if ((ExpirationMonth <= 0) && (ExpirationMonth > 12)) throw new ArgumentException("Not within range", "ExpirationMonth");
            if (ExpirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", "ExpirationYear");
            if (this._cvvRequired && string.IsNullOrEmpty(CVV)) throw new ArgumentNullException("CVV");
            if (this._cvvRequired && ((CVV.Length < 3) || (CVV.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "CVV");

            // Don't throw exceptions, as there's no product property (yet) to know if the product requires these fields.
            //if (string.IsNullOrEmpty(BillingAddress)) throw new ArgumentNullException("BillingAddress");
            //if (string.IsNullOrEmpty(BillingCity)) throw new ArgumentNullException("BillingCity");
            //if (string.IsNullOrEmpty(BillingState)) throw new ArgumentNullException("BillingState");
            //if (string.IsNullOrEmpty(BillingZip)) throw new ArgumentNullException("BillingZip");
            //if (string.IsNullOrEmpty(BillingCountry)) throw new ArgumentNullException("BillingCountry");

            //if (!string.IsNullOrEmpty(NewSystemID))
            //{
            //    // make sure that the system ID is unique
            //    if (this.LoadCustomer(NewSystemID) != null) throw new ArgumentException("Not unique", "NewSystemID");
            //}

            // create XML for creation of customer
            var subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            subscriptionXml.Append("<customer_attributes>");
            subscriptionXml.AppendFormat("<first_name>{0}</first_name>", FirstName.ToHtmlEncoded());
            subscriptionXml.AppendFormat("<last_name>{0}</last_name>", LastName.ToHtmlEncoded());
            subscriptionXml.AppendFormat("<email>{0}</email>", EmailAddress);
            if (!string.IsNullOrEmpty(Phone)) subscriptionXml.AppendFormat("<phone>{0}</phone>", Phone.ToHtmlEncoded());
            subscriptionXml.AppendFormat("<organization>{0}</organization>", (Organization != null) ? Organization.ToHtmlEncoded() : "null");
            subscriptionXml.AppendFormat("<reference>{0}</reference>", NewSystemID);
            if (!string.IsNullOrEmpty(ShippingAddress)) subscriptionXml.AppendFormat("<address>{0}</address>", ShippingAddress.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(ShippingCity)) subscriptionXml.AppendFormat("<city>{0}</city>", ShippingCity.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(ShippingState)) subscriptionXml.AppendFormat("<state>{0}</state>", ShippingState.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(ShippingZip)) subscriptionXml.AppendFormat("<zip>{0}</zip>", ShippingZip.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(ShippingCountry)) subscriptionXml.AppendFormat("<country>{0}</country>", ShippingCountry.ToHtmlEncoded());
            subscriptionXml.Append("</customer_attributes>");
            subscriptionXml.Append("<credit_card_attributes>");
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", FullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", ExpirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", ExpirationYear);
            if (this._cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", CVV); }
            if (!string.IsNullOrEmpty(BillingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", BillingAddress.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(BillingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", BillingCity.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(BillingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", BillingState.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(BillingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", BillingZip.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(BillingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", BillingCountry.ToHtmlEncoded());
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(CouponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", CouponCode); }
            if (ComponentID != int.MinValue)
            {
                subscriptionXml.Append(@"<components type=""array"">");
                subscriptionXml.Append("<component>");
                subscriptionXml.Append(string.Format("<component_id>{0}</component_id>", ComponentID));
                subscriptionXml.Append(string.Format("<allocated_quantity>{0}</allocated_quantity>", AllocatedQuantity));
                subscriptionXml.Append("</component>");
                subscriptionXml.Append("</components>");
            }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="Subscription">The subscription to update credit card info for</param>
        /// <param name="CreditCardAttributes">The attributes for the updated credit card</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(ISubscription Subscription, ICreditCardAttributes CreditCardAttributes)
        {
            if (Subscription == null) throw new ArgumentNullException("Subscription");
            return UpdateSubscriptionCreditCard(Subscription.SubscriptionID, CreditCardAttributes);
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="CreditCardAttributes">The attributes for the update credit card</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(int SubscriptionID, ICreditCardAttributes CreditCardAttributes)
        {
            // make sure data is OK
            if (CreditCardAttributes == null) throw new ArgumentNullException("CreditCardAttributes");
            return UpdateTheSubscriptionCreditCard(SubscriptionID, CreditCardAttributes.FirstName, CreditCardAttributes.LastName, CreditCardAttributes.FullNumber, CreditCardAttributes.ExpirationMonth, CreditCardAttributes.ExpirationYear,
                                                CreditCardAttributes.CVV, CreditCardAttributes.BillingAddress, CreditCardAttributes.BillingCity, CreditCardAttributes.BillingState,
                                                CreditCardAttributes.BillingZip, CreditCardAttributes.BillingCountry);
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="Subscription">The subscription to update credit card info for</param>
        /// <param name="FullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationMonth">The expiration month of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="CVV">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="BillingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="BillingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="BillingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="BillingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="BillingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(ISubscription Subscription, string FullNumber, int? ExpirationMonth, int? ExpirationYear, string CVV,
                                                                 string BillingAddress, string BillingCity, string BillingState, string BillingZip, string BillingCountry)
        {
            // make sure data is OK
            if (Subscription == null) throw new ArgumentNullException("Subscription");
            return UpdateTheSubscriptionCreditCard(Subscription.SubscriptionID, string.Empty, string.Empty, FullNumber, ExpirationMonth, ExpirationYear, CVV, BillingAddress, BillingCity,
                                                BillingState, BillingZip, BillingCountry);
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="FullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationMonth">The expiration month of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="CVV">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="BillingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="BillingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="BillingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="BillingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="BillingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(int SubscriptionID, string FullNumber, int? ExpirationMonth, int? ExpirationYear, string CVV,
                                                                 string BillingAddress, string BillingCity, string BillingState, string BillingZip, string BillingCountry)
        {

            // make sure data is OK
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");

            return UpdateTheSubscriptionCreditCard(SubscriptionID, string.Empty, string.Empty, FullNumber, ExpirationMonth, ExpirationYear, CVV, BillingAddress, BillingCity,
                                                BillingState, BillingZip, BillingCountry);
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="FirstName">The billing first name (first name on the card)</param>
        /// <param name="LastName">The billing last name (last name on the card)</param>
        /// <param name="FullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationMonth">The expiration month of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="CVV">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="BillingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="BillingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="BillingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="BillingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="BillingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(int SubscriptionID, string FirstName, string LastName, string FullNumber, int? ExpirationMonth, int? ExpirationYear, string CVV,
                                                                 string BillingAddress, string BillingCity, string BillingState, string BillingZip, string BillingCountry)
        {

            // make sure data is OK
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");

            return UpdateTheSubscriptionCreditCard(SubscriptionID, FirstName, LastName, FullNumber, ExpirationMonth, ExpirationYear, CVV, BillingAddress, BillingCity,
                                                BillingState, BillingZip, BillingCountry);
        }

        /// <summary>
        /// Method to update the payment profile
        /// </summary>
        /// <param name="SubscriptionID">The subscription to update</param>
        /// <param name="FirstName">The billing first name</param>
        /// <param name="LastName">The billing last name</param>
        /// <param name="FullNumber">The credit card number</param>
        /// <param name="ExpirationMonth">The expiration month</param>
        /// <param name="ExpirationYear">The expiration year</param>
        /// <param name="CVV">The CVV as written on the back of the card</param>
        /// <param name="BillingAddress">The billing address</param>
        /// <param name="BillingCity">The billing city</param>
        /// <param name="BillingState">The billing state</param>
        /// <param name="BillingZip">The billing zip/postal code</param>
        /// <param name="BillingCountry">The billing country</param>
        /// <returns>The updated subscription</returns>
        private ISubscription UpdateTheSubscriptionCreditCard(int SubscriptionID, string FirstName, string LastName, string FullNumber, int? ExpirationMonth, int? ExpirationYear, string CVV,
                                                                 string BillingAddress, string BillingCity, string BillingState, string BillingZip, string BillingCountry)
        {

            // make sure data is OK
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");
            if (string.IsNullOrEmpty("FullNumber")) throw new ArgumentNullException("FullNumber");
            if (this._cvvRequired && ((CVV.Length < 3) || (CVV.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "CVV");

            // make sure subscription exists
            ISubscription existingSubscription = this.LoadSubscription(SubscriptionID);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", "Subscription.SubscriptionID");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.Append("<credit_card_attributes>");
            if (!string.IsNullOrEmpty(FirstName)) subscriptionXml.AppendFormat("<first_name>{0}</first_name>", FirstName);
            if (!string.IsNullOrEmpty(LastName)) subscriptionXml.AppendFormat("<last_name>{0}</last_name>", LastName);
            if (!string.IsNullOrEmpty(FullNumber)) subscriptionXml.AppendFormat("<full_number>{0}</full_number>", FullNumber);
            if (ExpirationMonth != null) subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", ExpirationMonth);
            if (ExpirationYear != null) subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", ExpirationYear);
            if (this._cvvRequired && !string.IsNullOrEmpty(CVV)) subscriptionXml.AppendFormat("<cvv>{0}</cvv>", CVV);
            if (!string.IsNullOrEmpty(BillingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", BillingAddress);
            if (!string.IsNullOrEmpty(BillingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", BillingCity);
            if (!string.IsNullOrEmpty(BillingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", BillingState);
            if (!string.IsNullOrEmpty(BillingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", BillingZip);
            if (!string.IsNullOrEmpty(BillingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", BillingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = this.DoRequest(string.Format("subscriptions/{0}.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Put, subscriptionXml.ToString());
                // change the response to the object
                return response.ConvertResponseTo<Subscription>("subscription");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Subscription not found");
                throw;
            }
        }

        /// <summary>
        /// Update the specified chargify subscription
        /// </summary>
        /// <param name="Subscription">The subscription to update</param>
        /// <returns>The updated subscriptionn, null otherwise.</returns>
        public ISubscription UpdateSubscription(ISubscription Subscription)
        {
            return UpdateSubscription(Subscription.SubscriptionID, Subscription.Product.Handle, Subscription.Customer.SystemID, Subscription.Customer.FirstName,
                Subscription.Customer.LastName, Subscription.Customer.Email, Subscription.Customer.Phone, Subscription.Customer.Organization, Subscription.PaymentProfile.FullNumber, Subscription.PaymentProfile.ExpirationMonth,
                Subscription.PaymentProfile.ExpirationYear, string.Empty, Subscription.PaymentProfile.BillingAddress, Subscription.PaymentProfile.BillingCity, Subscription.PaymentProfile.BillingState,
                Subscription.PaymentProfile.BillingZip, Subscription.PaymentProfile.BillingCountry);
        }

        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="Subscription">The subscription to migrate</param>
        /// <param name="Product">The product to migrate the subscription to</param>
        /// <param name="IncludeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="IncludeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns></returns>
        public ISubscription MigrateSubscriptionProduct(ISubscription Subscription, IProduct Product, bool IncludeTrial, bool IncludeInitialCharge)
        {
            // make sure data is OK
            if (Subscription == null) throw new ArgumentNullException("Subscription");
            if (Product == null) throw new ArgumentNullException("Product");
            return MigrateSubscriptionProduct(Subscription.SubscriptionID, Product.Handle, IncludeTrial, IncludeInitialCharge);
        }

        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="SubscriptionID">The subscription to migrate</param>
        /// <param name="Product">The product to migrate to</param>
        /// <param name="IncludeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="IncludeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns>The completed subscription if migrated successfully, null otherwise.</returns>
        public ISubscription MigrateSubscriptionProduct(int SubscriptionID, IProduct Product, bool IncludeTrial, bool IncludeInitialCharge)
        {
            // make sure data is OK
            if (Product == null) throw new ArgumentNullException("Product");
            return MigrateSubscriptionProduct(SubscriptionID, Product.Handle, IncludeTrial, IncludeInitialCharge);
        }

        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="Subscription">The subscription to migrate</param>
        /// <param name="ProductHandle">The product handle of the product to migrate to</param>
        /// <param name="IncludeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="IncludeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns>The completed subscription if migrated successfully, null otherwise.</returns>
        public ISubscription MigrateSubscriptionProduct(ISubscription Subscription, string ProductHandle, bool IncludeTrial, bool IncludeInitialCharge)
        {
            // make sure data is OK
            if (Subscription == null) throw new ArgumentNullException("Subscription");
            return MigrateSubscriptionProduct(Subscription.SubscriptionID, ProductHandle, IncludeTrial, IncludeInitialCharge);
        }

        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="SubscriptionID">The subscription to migrate</param>
        /// <param name="ProductHandle">The product handle of the product to migrate to</param>
        /// <param name="IncludeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="IncludeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns>The completed subscription if migrated successfully, null otherwise.</returns>
        public ISubscription MigrateSubscriptionProduct(int SubscriptionID, string ProductHandle, bool IncludeTrial, bool IncludeInitialCharge)
        {
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");

            // make sure subscription exists
            ISubscription existingSubscription = this.LoadSubscription(SubscriptionID);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", "Subscription.SubscriptionID");

            // create XML for creation of customer
            StringBuilder migrationXml = new StringBuilder(GetXMLStringIfApplicable());
            migrationXml.Append("<migration>");
            migrationXml.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            if (IncludeTrial) { migrationXml.Append("<include_trial>1</include_trial>"); }
            if (IncludeInitialCharge) { migrationXml.Append("<include_initial_charge>1</include_initial_charge>"); }
            migrationXml.Append("</migration>");
            try
            {
                // now make the request
                string response = this.DoRequest(string.Format("subscriptions/{0}/migrations.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Post, migrationXml.ToString());
                // change the response to the object
                return response.ConvertResponseTo<Subscription>("subscription");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Subscription not found");
                throw;
            }
        }

        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="Subscription">The suscription to update</param>
        /// <param name="Product">The new product</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription EditSubscriptionProduct(ISubscription Subscription, IProduct Product)
        {
            // make sure data is OK
            if (Subscription == null) throw new ArgumentNullException("Subscription");
            if (Product == null) throw new ArgumentNullException("Product");
            return EditSubscriptionProduct(Subscription.SubscriptionID, Product.Handle);
        }

        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="Product">The new product</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription EditSubscriptionProduct(int SubscriptionID, IProduct Product)
        {
            // make sure data is OK
            if (Product == null) throw new ArgumentNullException("Product");
            return EditSubscriptionProduct(SubscriptionID, Product.Handle);
        }

        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="Subscription">The suscription to update</param>
        /// <param name="ProductHandle">The handle to the new product</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription EditSubscriptionProduct(ISubscription Subscription, string ProductHandle)
        {
            // make sure data is OK
            if (Subscription == null) throw new ArgumentNullException("Subscription");
            return EditSubscriptionProduct(Subscription.SubscriptionID, ProductHandle);
        }

        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="ProductHandle">The handle to the new product</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription EditSubscriptionProduct(int SubscriptionID, string ProductHandle)
        {
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");
            if (string.IsNullOrEmpty(ProductHandle)) throw new ArgumentNullException("ProductHandle");

            // make sure subscription exists
            ISubscription existingSubscription = this.LoadSubscription(SubscriptionID);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", "Subscription.SubscriptionID");

            // create XML for creation of customer
            StringBuilder SubscriptionXML = new StringBuilder(GetXMLStringIfApplicable());
            SubscriptionXML.Append("<subscription>");
            SubscriptionXML.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            SubscriptionXML.Append("</subscription>");
            try
            {
                // now make the request
                string response = this.DoRequest(string.Format("subscriptions/{0}.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Put, SubscriptionXML.ToString());
                // change the response to the object
                return response.ConvertResponseTo<Subscription>("subscription");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Subscription not found");
                throw;
            }
        }

        /// <summary>
        /// Update a subscription changing customer, product and credit card information at the same time
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to update</param>
        /// <param name="ProductHandle">The handle to the product (optional - set to null if not required)</param>
        /// <param name="SystemID">The system ID for the customer (optional - set to Guid.Empty if not required)</param>
        /// <param name="FirstName">The first name of the new customer (optional - set to null if not required)</param>
        /// <param name="LastName">The last name of the new customer (optional - set to null if not required)</param>
        /// <param name="EmailAddress">The email address for the new customer (optional - set to null if not required)</param>
        /// <param name="Phone">The phone number of the customer (optional - set to null if not required)</param>
        /// <param name="Organization">The organization of the new customer (optional - set to null if not required)</param>
        /// <param name="FullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationMonth">The expritation month of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="CVV">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="BillingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="BillingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="BillingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="BillingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="BillingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription UpdateSubscription(int SubscriptionID, string ProductHandle, string SystemID, string FirstName, string LastName, string EmailAddress, string Phone,
                                                 string Organization, string FullNumber, int? ExpirationMonth, int? ExpirationYear,
                                                 string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip,
                                                 string BillingCountry)
        {
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");

            // make sure subscription exists
            ISubscription existingSubscription = this.LoadSubscription(SubscriptionID);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", "Subscription.SubscriptionID");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            //if (!string.IsNullOrEmpty(ProductHandle) && existingSubscription.Product.Handle != ProductHandle)  
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", ProductHandle);
            if (!string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName) || !string.IsNullOrEmpty(EmailAddress) ||
                !string.IsNullOrEmpty(Organization) || SystemID != string.Empty)
            {
                subscriptionXml.Append("<customer_attributes>");
                //if (!string.IsNullOrEmpty(FirstName) && existingSubscription.Customer.FirstName != FirstName) 
                subscriptionXml.AppendFormat("<first_name>{0}</first_name>", FirstName);
                //if (!string.IsNullOrEmpty(LastName) && existingSubscription.Customer.LastName != LastName) 
                subscriptionXml.AppendFormat("<last_name>{0}</last_name>", LastName);
                if (!string.IsNullOrEmpty(EmailAddress) && existingSubscription.Customer.Email != EmailAddress) subscriptionXml.AppendFormat("<email>{0}</email>", EmailAddress);
                if (!string.IsNullOrEmpty(Phone) && existingSubscription.Customer.Phone != Phone) subscriptionXml.AppendFormat("<{0}>{1}</{2}>", CustomerAttributes.PhoneKey, Phone, CustomerAttributes.PhoneKey);
                if (!string.IsNullOrEmpty(Organization) && existingSubscription.Customer.Organization != Organization) subscriptionXml.AppendFormat("<organization>{0}</organization>", HttpUtility.HtmlEncode(Organization));
                if ((SystemID != string.Empty) && (existingSubscription.Customer.SystemID != SystemID)) subscriptionXml.AppendFormat("<reference>{0}</reference>", SystemID.ToString());
                subscriptionXml.Append("</customer_attributes>");
            }

            if (!string.IsNullOrEmpty(FullNumber) || ExpirationMonth == null || ExpirationYear == null || !string.IsNullOrEmpty(CVV) ||
                !string.IsNullOrEmpty(BillingAddress) || !string.IsNullOrEmpty(BillingCity) || !string.IsNullOrEmpty(BillingState) ||
                !string.IsNullOrEmpty(BillingZip) || !string.IsNullOrEmpty(BillingCountry))
            {

                StringBuilder paymentProfileXml = new StringBuilder();
                if ((!string.IsNullOrEmpty(FullNumber)) && (existingSubscription.PaymentProfile.FullNumber != FullNumber)) paymentProfileXml.AppendFormat("<full_number>{0}</full_number>", FullNumber);
                if ((ExpirationMonth == null) && (existingSubscription.PaymentProfile.ExpirationMonth != ExpirationMonth)) paymentProfileXml.AppendFormat("<expiration_month>{0}</expiration_month>", ExpirationMonth);
                if ((ExpirationYear == null) && (existingSubscription.PaymentProfile.ExpirationYear != ExpirationYear)) paymentProfileXml.AppendFormat("<expiration_year>{0}</expiration_year>", ExpirationYear);
                if (this._cvvRequired && !string.IsNullOrEmpty(CVV)) paymentProfileXml.AppendFormat("<cvv>{0}</cvv>", CVV);
                if (!string.IsNullOrEmpty(BillingAddress) && existingSubscription.PaymentProfile.BillingAddress != BillingAddress) paymentProfileXml.AppendFormat("<billing_address>{0}</billing_address>", BillingAddress);
                if (!string.IsNullOrEmpty(BillingCity) && existingSubscription.PaymentProfile.BillingCity != BillingCity) paymentProfileXml.AppendFormat("<billing_city>{0}</billing_city>", BillingCity);
                if (!string.IsNullOrEmpty(BillingState) && existingSubscription.PaymentProfile.BillingState != BillingState) paymentProfileXml.AppendFormat("<billing_state>{0}</billing_state>", BillingState);
                if (!string.IsNullOrEmpty(BillingZip) && existingSubscription.PaymentProfile.BillingZip != BillingZip) paymentProfileXml.AppendFormat("<billing_zip>{0}</billing_zip>", BillingZip);
                if (!string.IsNullOrEmpty(BillingCountry) && existingSubscription.PaymentProfile.BillingCountry != BillingCountry) paymentProfileXml.AppendFormat("<billing_country>{0}</billing_country>", BillingCountry);
                if (paymentProfileXml.Length > 0)
                {
                    subscriptionXml.AppendFormat("<credit_card_attributes>{0}</credit_card_attributes>", paymentProfileXml.ToString());
                }

            }
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = this.DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
                // change the response to the object
                return response.ConvertResponseTo<Subscription>("subscription");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Method to allow users to change the next_assessment_at date
        /// </summary>
        /// <param name="SubscriptionID">The subscription to modify</param>
        /// <param name="NextBillingAt">The date to next bill the customer</param>
        /// <returns>Subscription if successful, null otherwise.</returns>
        public ISubscription UpdateBillingDateForSubscription(int SubscriptionID, DateTime NextBillingAt)
        {
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");

            // make sure subscription exists
            ISubscription existingSubscription = this.LoadSubscription(SubscriptionID);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", "Subscription.SubscriptionID");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<next_billing_at>{0}</next_billing_at>", NextBillingAt.ToString("o"));
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = this.DoRequest(string.Format("subscriptions/{0}.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Put, subscriptionXml.ToString());
                // change the response to the object
                return response.ConvertResponseTo<Subscription>("subscription");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Subscription not found");
                throw;
            }
        }

        /// <summary>
        /// Method to allow users to change the cancel_at_end_of_period flag
        /// </summary>
        /// <param name="SubscriptionID">The subscription to modify</param>
        /// <param name="CancelAtEndOfPeriod">True if the subscription should cancel at the end of the current period</param>
        /// <param name="CancellationMessage">The reason for cancelling the subscription</param>
        /// <returns>Subscription if successful, null otherwise.</returns>
        public ISubscription UpdateDelayedCancelForSubscription(int SubscriptionID, bool CancelAtEndOfPeriod, string CancellationMessage)
        {
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXMLStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<cancel_at_end_of_period>{0}</cancel_at_end_of_period>", CancelAtEndOfPeriod ? "1" : "0");
            if (!String.IsNullOrEmpty(CancellationMessage)) { subscriptionXml.AppendFormat("<cancellation_message>{0}</cancellation_message>", CancellationMessage); }
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = this.DoRequest(string.Format("subscriptions/{0}.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Put, subscriptionXml.ToString());
                // change the response to the object
                return response.ConvertResponseTo<Subscription>("subscription");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Subscription not found");
                throw;
            }
        }

        /// <summary>
        /// Method for reseting a subscription balance to zero (removes outstanding balance). 
        /// Useful when reactivating subscriptions, and making sure not to charge the user
        /// their existing balance when then cancelled.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool ResetSubscriptionBalance(int SubscriptionID)
        {
            try
            {
                // make sure data is valid
                if (SubscriptionID < 0) throw new ArgumentNullException("SubscriptionID");
                // now make the request
                this.DoRequest(string.Format("subscriptions/{0}/reset_balance.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Put, string.Empty);
                return true;
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return false;
                throw cex;
            }
        }

        /// <summary>
        /// Update the collection method of the subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription of who's collection method should be updated</param>
        /// <param name="PaymentCollectionMethod">The collection method to set</param>
        /// <returns>The full details of the updated subscription</returns>
        public ISubscription UpdatePaymentCollectionMethod(int SubscriptionID, PaymentCollectionMethod PaymentCollectionMethod)
        {
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");

            // make sure subscription exists
            ISubscription existingSubscription = this.LoadSubscription(SubscriptionID);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", "Subscription.SubscriptionID");

            // create XML for creation of customer
            StringBuilder SubscriptionXML = new StringBuilder(GetXMLStringIfApplicable());
            SubscriptionXML.Append("<subscription>");
            SubscriptionXML.AppendFormat("<payment_collection_method>{0}</payment_collection_method>", Enum.GetName(typeof(PaymentCollectionMethod), PaymentCollectionMethod).ToLowerInvariant());
            SubscriptionXML.Append("</subscription>");
            try
            {
                // now make the request
                string response = this.DoRequest(string.Format("subscriptions/{0}.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Put, SubscriptionXML.ToString());
                // change the response to the object
                return response.ConvertResponseTo<Subscription>("subscription");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Subscription not found");
                throw;
            }
        }

        #endregion

        #region Subscription Override
        /// <summary>
        /// This API endpoint allows you to set certain subscription fields that are usually managed for you automatically. Some of the fields can be set via the normal Subscriptions Update API, but others can only be set using this endpoint.
        /// </summary>
        /// <param name="SubscriptionID"></param>
        /// <param name="OverrideDetails"></param>
        /// <returns>The details returned by Chargify</returns>
        public bool SetSubscriptionOverride(int SubscriptionID, ISubscriptionOverride OverrideDetails)
        {
            if (OverrideDetails == null) throw new ArgumentNullException("OverrideDetails");
            return SetSubscriptionOverride(SubscriptionID, OverrideDetails.ActivatedAt, OverrideDetails.CanceledAt, OverrideDetails.CancellationMessage, OverrideDetails.ExpiresAt);
        }

        /// <summary>
        /// This API endpoint allows you to set certain subscription fields that are usually managed for you automatically. Some of the fields can be set via the normal Subscriptions Update API, but others can only be set using this endpoint.
        /// </summary>
        /// <param name="SubscriptionID"></param>
        /// <param name="ActivatedAt"></param>
        /// <param name="CanceledAt"></param>
        /// <param name="CancellationMessage"></param>
        /// <param name="ExpiresAt"></param>
        /// <returns>The details returned by Chargify</returns>
        public bool SetSubscriptionOverride(int SubscriptionID, DateTime? ActivatedAt = null, DateTime? CanceledAt = null, string CancellationMessage = null, DateTime? ExpiresAt = null)
        {
            try
            {
                // make sure data is valid
                if (ActivatedAt == null && CanceledAt == null && CancellationMessage == null && ExpiresAt == null)
                {
                    throw new ArgumentException("No valid parameters provided");
                }

                // make sure that the SubscriptionID is unique
                if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("No subscription found with that ID", "SubscriptionID");

                // create XML for creation of a charge
                var OverrideXML = new StringBuilder(GetXMLStringIfApplicable());
                OverrideXML.Append("<subscription>");
                if (ActivatedAt.HasValue) { OverrideXML.AppendFormat("<activated_at>{0}</activated_at>", ActivatedAt.Value.ToString("o")); }
                if (!string.IsNullOrEmpty(CancellationMessage)) { OverrideXML.AppendFormat("<cancellation_message>{0}</cancellation_message>", HttpUtility.HtmlEncode(CancellationMessage)); }
                if (CanceledAt.HasValue) { OverrideXML.AppendFormat("<canceled_at>{0}</canceled_at>", CanceledAt.Value.ToString("o")); }
                if (ExpiresAt.HasValue) { OverrideXML.AppendFormat("<expires_at>{0}</expires_at>", ExpiresAt.Value.ToString("o")); }
                OverrideXML.Append("</subscription>");

                // now make the request
                var result = this.DoRequest(string.Format("subscriptions/{0}/override.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Put, OverrideXML.ToString());
                return result == string.Empty;
            }
            catch (ChargifyException cex)
            {
                switch (cex.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        return false;
                    default:
                        throw;
                }
            }
        }
        #endregion

        #region Migrations
        /// <summary>
        /// Return a preview of charges for a subscription product migrations
        /// </summary>
        /// <param name="SubscriptionID">SubscriptionID</param>
        /// <param name="ProductID">ProductID</param>
        /// <param name="IncludeCoupons">Should the migration preview consider subscription coupons?</param>
        /// <param name="IncludeInitialCharge">Should the migration preview consider a setup fee</param>
        /// <param name="IncludeTrial">Should the migration preview consider the product trial?</param>
        /// <returns></returns>
        public IMigration PreviewMigrateSubscriptionProduct(int SubscriptionID, int ProductID, bool? IncludeTrial, bool? IncludeInitialCharge, bool? IncludeCoupons)
        {
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");
            if (ProductID == int.MinValue) throw new ArgumentNullException("ProductID");

            // make sure subscription exists
            ISubscription existingSubscription = this.LoadSubscription(SubscriptionID);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", "Subscription.SubscriptionID");

            // create XML for creation of customer
            StringBuilder MigrationXML = new StringBuilder(GetXMLStringIfApplicable());
            MigrationXML.Append("<migration>");
            MigrationXML.AppendFormat("<product_id>{0}</product_id>", ProductID);
            if (IncludeTrial.HasValue) { MigrationXML.Append(string.Format("<include_trial>{0}</include_trial>", IncludeTrial.Value ? "1" : "0")); }
            if (IncludeInitialCharge.HasValue) { MigrationXML.Append(string.Format("<include_initial_charge>{0}</include_initial_charge>", IncludeInitialCharge.Value ? "1" : "0")); }
            if (IncludeCoupons.HasValue) { MigrationXML.Append(string.Format("<include_coupons>{0}</include_coupons>", IncludeCoupons.Value ? "1" : "0")); }
            else
            {
                // Default is yes, if unspecified.
                MigrationXML.Append("<include_coupons>1</include_coupons>");
            }
            MigrationXML.Append("</migration>");
            try
            {
                // now make the request
                string response = this.DoRequest(string.Format("subscriptions/{0}/migrations/preview.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Post, MigrationXML.ToString());
                // change the response to the object
                return response.ConvertResponseTo<Migration>("migration");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Migration not found");
                throw;
            }
        }

        /// <summary>
        /// Return a preview of charges for a subscription product migrations
        /// </summary>
        /// <param name="SubscriptionID">Active Subscription</param>
        /// <param name="ProductID">Active Product</param>
        /// <returns>The migration preview data, if able. Null otherwise.</returns>
        public IMigration PreviewMigrateSubscriptionProduct(int SubscriptionID, int ProductID)
        {
            return PreviewMigrateSubscriptionProduct(SubscriptionID, ProductID, null, null, null);
        }

        /// <summary>
        /// Return a preview of charges for a subscription product migrations
        /// </summary>
        /// <param name="Subscription">Active Subscription</param>
        /// <param name="Product">Active Product</param>
        /// <returns>The migration preview data, if able. Null otherwise.</returns>
        public IMigration PreviewMigrateSubscriptionProduct(ISubscription Subscription, IProduct Product)
        {
            if (Subscription == null) throw new ArgumentNullException("Subscription");
            if (Product == null) throw new ArgumentNullException("Product");
            return PreviewMigrateSubscriptionProduct(Subscription.SubscriptionID, Product.ID);
        }
        #endregion

        #region Coupons

        /// <summary>
        /// Method for retrieving information about a coupon using the ID of that coupon.
        /// </summary>
        /// <param name="ProductFamilyID">The ID of the product family that the coupon belongs to</param>
        /// <param name="CouponID">The ID of the coupon</param>
        /// <returns>The object if found, null otherwise.</returns>
        public ICoupon LoadCoupon(int ProductFamilyID, int CouponID)
        {
            try
            {
                // make sure data is valid
                if (ProductFamilyID < 0) throw new ArgumentException("Invalid ProductFamilyID");
                if (CouponID < 0) throw new ArgumentException("Invalid CouponID");
                // now make the request
                string response = this.DoRequest(string.Format("product_families/{0}/coupons/{1}.{2}", ProductFamilyID, CouponID, GetMethodExtension()));
                // change the response to the object
                return response.ConvertResponseTo<Coupon>("coupon");

            }
            catch (ChargifyException cex)
            {
                // Throw if anything but not found, since not found is telling us that it's working correctly
                // but that there just isn't a coupon with that ID.
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw cex;
            }
        }

        /// <summary>
        /// Retrieve the coupon corresponding to the coupon code, useful for coupon validation.
        /// </summary>
        /// <param name="ProductFamilyID">The ID of the product family the coupon belongs to</param>
        /// <param name="CouponCode">The code used to represent the coupon</param>
        /// <returns>The object if found, otherwise null.</returns>
        public ICoupon FindCoupon(int ProductFamilyID, string CouponCode)
        {
            try
            {
                string response = this.DoRequest(string.Format("product_families/{0}/coupons/find.{1}?code={2}", ProductFamilyID, GetMethodExtension(), CouponCode));
                // change the response to the object
                return response.ConvertResponseTo<Coupon>("coupon");
            }
            catch (ChargifyException cex)
            {
                // Throw if anything but not found, since not found is telling us that it's working correctly
                // but that there just isn't a coupon with that code.
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw cex;
            }
        }

        /// <summary>
        /// Method to add a coupon to a subscription using the API
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify</param>
        /// <param name="CouponCode">The code of the coupon to apply to the subscription</param>
        /// <returns>The subscription details if successful, null otherwise.</returns>
        public ISubscription AddCoupon(int SubscriptionID, string CouponCode)
        {
            // make sure that the SubscriptionID is unique
            if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("Not an SubscriptionID", "SubscriptionID");
            if (string.IsNullOrEmpty(CouponCode)) throw new ArgumentException("Coupon code is empty", "CouponCode");
            string response = this.DoRequest(string.Format("subscriptions/{0}/add_coupon.{1}?code={2}", SubscriptionID, GetMethodExtension(), CouponCode), HttpRequestMethod.Post, null);
            // change the response to the object            
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new one-time credit
        /// </summary>
        /// <param name="Coupon">The coupon parameters</param>
        /// <param name="ProductFamilyID">The ID of the product family to add this coupon to.</param>
        /// <returns>The object if successful, null otherwise.</returns>
        public ICoupon CreateCoupon(ICoupon Coupon, int ProductFamilyID)
        {
            if (Coupon == null) throw new ArgumentNullException("Coupon");
            string xml = BuildCouponXML(ProductFamilyID, Coupon.Name, Coupon.Code, Coupon.Description, Coupon.Amount, Coupon.Percentage, Coupon.AllowNegativeBalance,
                Coupon.IsRecurring, Coupon.DurationPeriodCount, Coupon.EndDate);

            string response = this.DoRequest(string.Format("coupons.{0}", GetMethodExtension()), HttpRequestMethod.Post, xml);
            // change the response to the object
            return response.ConvertResponseTo<Coupon>("coupon");
        }

        /// <summary>
        /// Update an existing coupon
        /// </summary>
        /// <param name="Coupon">Coupon object</param>
        /// <returns>The updated coupon if successful, null otherwise.</returns>
        public ICoupon UpdateCoupon(ICoupon Coupon)
        {
            if (Coupon == null) throw new ArgumentNullException("Coupon");
            if (Coupon.ProductFamilyID <= 0) throw new ArgumentOutOfRangeException("Coupon.ProductFamilyID must be > 0");
            if (Coupon.ID <= 0) throw new ArgumentOutOfRangeException("Coupon ID is not valid");

            string xml = BuildCouponXML(Coupon.ProductFamilyID, Coupon.Name, Coupon.Code, Coupon.Description, Coupon.Amount, Coupon.Percentage, Coupon.AllowNegativeBalance,
                Coupon.IsRecurring, Coupon.DurationPeriodCount, Coupon.EndDate);

            string response = this.DoRequest(string.Format("coupons/{0}.{1}", Coupon.ID, GetMethodExtension()), HttpRequestMethod.Put, xml);
            // change the response to the object
            return response.ConvertResponseTo<Coupon>("coupon");
        }

        /// <summary>
        /// Builds the coupon XML based on all the coupon values entered.
        /// </summary>
        /// <param name="ProductFamilyID">The id of the product family the coupon should belong to</param>
        /// <param name="name">The name of the coupon</param>
        /// <param name="code">The code for the coupon</param>
        /// <param name="description">The description of the coupons effect</param>
        /// <param name="amount">The amount of the coupon</param>
        /// <param name="percentage">If percentage based, the percentage the coupon affects.</param>
        /// <param name="allowNegativeBalance">If true, credits will carry forward to the next billing. Otherwise discount may not exceed total charges.</param>
        /// <param name="recurring">This this a recurring coupon?</param>
        /// <param name="durationPeriodCount">How long does the coupon last?</param>
        /// <param name="endDate">At what point will the coupon no longer be valid?</param>
        /// <returns></returns>
        private string BuildCouponXML(int ProductFamilyID, string name, string code, string description, decimal amount, int percentage, bool allowNegativeBalance,
            bool recurring, int durationPeriodCount, DateTime endDate)
        {
            // make sure data is valid
            //if (id <= 0 && !id.Equals(int.MinValue)) throw new ArgumentOutOfRangeException("id", id, "id must be > 0 if specified.");
            // Don't use ID here, since it's only being used to build the URL
            if (ProductFamilyID < 0 && !ProductFamilyID.Equals(int.MinValue)) throw new ArgumentOutOfRangeException("ProductFamilyID", ProductFamilyID, "Product Family must be >= 0");
            if (amount < 0) throw new ArgumentNullException("amount");
            if (percentage < 0) throw new ArgumentNullException("percentage");
            if (amount > 0 && percentage > 0) throw new ArgumentException("Only one of amount or percentage can have a value > 0");
            if (percentage > 100) throw new ArgumentOutOfRangeException("percentage", percentage, "percentage must be between 1 and 100");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
            if (!recurring && durationPeriodCount > 0)
                throw new ArgumentOutOfRangeException("durationPeriodCount", durationPeriodCount, "duration period count must be 0 if not recurring");

            // create XML for creation of a credit
            StringBuilder CouponXML = new StringBuilder(GetXMLStringIfApplicable());
            CouponXML.Append("<coupon>");
            CouponXML.AppendFormat("<name>{0}</name>", HttpUtility.HtmlEncode(name));
            CouponXML.AppendFormat("<code>{0}</code>", code);
            if (!String.IsNullOrEmpty(description)) CouponXML.AppendFormat("<description>{0}</description>", HttpUtility.HtmlEncode(description));
            if (amount > 0) CouponXML.AppendFormat("<amount>{0}</amount>", amount.ToChargifyCurrencyFormat());
            if (percentage > 0) CouponXML.AppendFormat("<percentage>{0}</percentage>", percentage);
            CouponXML.AppendFormat("<allow_negative_balance>{0}</allow_negative_balance>", allowNegativeBalance.ToString().ToLower());
            CouponXML.AppendFormat("<recurring>{0}</recurring>", recurring.ToString().ToLower());
            if (recurring)
            {
                if (durationPeriodCount > 0)
                {
                    CouponXML.AppendFormat("<duration_period_count>{0}</duration_period_count>", durationPeriodCount);
                }
                else
                {
                    CouponXML.Append("<duration_period_count />");
                }
            }
            if (!endDate.Equals(DateTime.MinValue)) CouponXML.AppendFormat("<end_date>{0}</end_date>", endDate.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            if (ProductFamilyID > 0) CouponXML.AppendFormat("<product_family_id>{0}</product_family_id>", ProductFamilyID);
            CouponXML.Append("</coupon>");
            return CouponXML.ToString();

        }
        #endregion

        #region One-Time Charges

        /// <summary>
        /// Create a new one-time charge
        /// </summary>
        /// <param name="SubscriptionID">The subscription that will be charged</param>
        /// <param name="Charge">The charge parameters</param>
        /// <returns></returns>
        public ICharge CreateCharge(int SubscriptionID, ICharge Charge)
        {
            // make sure data is valid
            if (Charge == null) throw new ArgumentNullException("Charge");
            return CreateCharge(SubscriptionID, Charge.Amount, Charge.Memo);
        }

        ///// <summary>
        ///// Create a new one-time charge
        ///// </summary>
        ///// <param name="SubscriptionID">The subscription that will be charged</param>
        ///// <param name="amount">The amount to charge the customer</param>
        ///// <param name="memo"></param>
        ///// <returns></returns>
        //public ICharge CreateCharge(int SubscriptionID, decimal amount, string memo)
        //{
        //    // make sure data is valid
        //    if (amount < 0) throw new ArgumentNullException("Amount"); // Chargify will throw a 422 if a negative number is in this field.
        //    if (string.IsNullOrEmpty(memo)) throw new ArgumentNullException("Memo");
        //    return CreateCharge(SubscriptionID, amount, memo, true, false);
        //}

        /// <summary>
        /// Create a new one-time charge, with options
        /// </summary>
        /// <param name="SubscriptionID">The subscription that will be charged</param>
        /// <param name="amount">The amount to charge the customer</param>
        /// <param name="memo">A description of the charge</param>
        /// <param name="delayCharge">(Optional) Should the charge be billed during the next assessment? Default = false</param>
        /// <param name="useNegativeBalance">(Optional) Should the subscription balance be taken into consideration? Default = true</param>
        /// <returns>The charge details</returns>
        public ICharge CreateCharge(int SubscriptionID, decimal amount, string memo, bool useNegativeBalance = false, bool delayCharge = false)
        {
            // make sure data is valid
            if (amount < 0) throw new ArgumentNullException("Amount"); // Chargify will throw a 422 if a negative number is in this field.
            if (string.IsNullOrEmpty(memo)) throw new ArgumentNullException("Memo");
            // make sure that the SubscriptionID is unique
            if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("Not an SubscriptionID", "SubscriptionID");
            // create XML for creation of a charge
            StringBuilder ChargeXML = new StringBuilder(GetXMLStringIfApplicable());
            ChargeXML.Append("<charge>");
            ChargeXML.AppendFormat("<amount>{0}</amount>", amount.ToChargifyCurrencyFormat());
            ChargeXML.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo));
            ChargeXML.AppendFormat("<delay_capture>{0}</delay_capture>", delayCharge ? "1" : "0");
            ChargeXML.AppendFormat("<use_negative_balance>{0}</use_negative_balance>", !useNegativeBalance ? "1" : "0");
            ChargeXML.Append("</charge>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions/{0}/charges.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Post, ChargeXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Charge>("charge");
        }

        #endregion

        #region One-Time Credits

        /// <summary>
        /// Create a new one-time credit
        /// </summary>
        /// <param name="SubscriptionID">The subscription that will be credited</param>
        /// <param name="Credit">The credit parameters</param>
        /// <returns>The object if successful, null otherwise.</returns>
        public ICredit CreateCredit(int SubscriptionID, ICredit Credit)
        {
            // make sure data is valid
            if (Credit == null) throw new ArgumentNullException("Credit");
            return CreateCredit(SubscriptionID, Credit.Amount, Credit.Memo);
        }

        /// <summary>
        /// Create a new one-time credit
        /// </summary>
        /// <param name="SubscriptionID">The subscription that will be credited</param>
        /// <param name="amount">The amount to credit the customer</param>
        /// <param name="memo">A note regarding the reason for the credit</param>
        /// <returns>The object if successful, null otherwise.</returns>
        public ICredit CreateCredit(int SubscriptionID, decimal amount, string memo)
        {
            // make sure data is valid
            if (amount < 0) throw new ArgumentNullException("Amount");
            if (string.IsNullOrEmpty(memo)) throw new ArgumentNullException("Memo");
            // make sure that the SubscriptionID is unique
            if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("Not an SubscriptionID", "SubscriptionID");
            // create XML for creation of a credit
            StringBuilder CreditXML = new StringBuilder(GetXMLStringIfApplicable());
            CreditXML.Append("<credit>");
            CreditXML.AppendFormat("<amount>{0}</amount>", amount.ToChargifyCurrencyFormat());
            CreditXML.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo));
            CreditXML.Append("</credit>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions/{0}/credits.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Post, CreditXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Credit>("credit");
        }
        #endregion

        #region Components

        /// <summary>
        /// Method to update the allocated amount of a component for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify the allocation for</param>
        /// <param name="ComponentID">The ID of the component</param>
        /// <param name="NewAllocatedQuantity">The amount of component to allocate to the subscription</param>
        /// <returns>The ComponentAttributes object with UnitBalance filled in, null otherwise.</returns>
        public IComponentAttributes UpdateComponentAllocationForSubscription(int SubscriptionID, int ComponentID, int NewAllocatedQuantity)
        {
            // make sure data is valid
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");
            if (ComponentID == int.MinValue) throw new ArgumentNullException("ComponentID");
            if (NewAllocatedQuantity < 0) throw new ArgumentOutOfRangeException("NewAllocatedQuantity");
            // create XML for change of allocation
            StringBuilder AllocationXML = new StringBuilder(GetXMLStringIfApplicable());
            AllocationXML.Append("<component>");
            AllocationXML.AppendFormat("<allocated_quantity type=\"integer\">{0}</allocated_quantity>", NewAllocatedQuantity);
            AllocationXML.Append("</component>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions/{0}/components/{1}.{2}", SubscriptionID, ComponentID, GetMethodExtension()), HttpRequestMethod.Put, AllocationXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<ComponentAttributes>("component");
        }

        /// <summary>
        /// Method to retrieve the current information (including allocation) of a component against a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription in question</param>
        /// <param name="ComponentID">The ID of the component</param>
        /// <returns>The ComponentAttributes object, null otherwise.</returns>
        public IComponentAttributes GetComponentInfoForSubscription(int SubscriptionID, int ComponentID)
        {
            // make sure data is valid
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");
            if (ComponentID == int.MinValue) throw new ArgumentNullException("ComponentID");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions/{0}/components/{1}.{2}", SubscriptionID, ComponentID, GetMethodExtension()));
            // change the response to the object
            return response.ConvertResponseTo<ComponentAttributes>("component");
        }

        /// <summary>
        /// Returns all components "attached" to that subscription.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to query about</param>
        /// <returns>A dictionary of components, if applicable.</returns>
        public IDictionary<int, IComponentAttributes> GetComponentsForSubscription(int SubscriptionID)
        {
            // make sure data is valid
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");

            // now make the request
            string response = this.DoRequest(string.Format("subscriptions/{0}/components.{1}", SubscriptionID, GetMethodExtension()));
            var retValue = new Dictionary<int, IComponentAttributes>();
            if (response.IsXml())
            {
                // now build a product list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response); // get the XML into an XML document
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node

                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "components")
                    {
                        foreach (XmlNode componentNode in elementNode.ChildNodes)
                        {
                            if (componentNode.Name == "component")
                            {
                                IComponentAttributes LoadedComponent = new ComponentAttributes(componentNode);
                                if (!retValue.ContainsKey(LoadedComponent.ComponentID))
                                {
                                    retValue.Add(LoadedComponent.ComponentID, LoadedComponent);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate ComponentID values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("component"))
                    {
                        JsonObject componentObj = (array.Items[i] as JsonObject)["component"] as JsonObject;
                        IComponentAttributes LoadedComponent = new ComponentAttributes(componentObj);
                        if (!retValue.ContainsKey(LoadedComponent.ComponentID))
                        {
                            retValue.Add(LoadedComponent.ComponentID, LoadedComponent);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate ComponentID values detected");
                        }
                    }
                }
            }
            // return the dictionary
            return retValue;
        }

        /// <summary>
        /// Method for getting a list of components for a specific product family
        /// </summary>
        /// <param name="ChargifyID">The product family ID</param>
        /// <param name="includeArchived">Filter flag for archived components</param>
        /// <returns>A dictionary of components if there are results, null otherwise.</returns>
        public IDictionary<int, IComponentInfo> GetComponentsForProductFamily(int ChargifyID, bool includeArchived)
        {
            // make sure data is valid
            if (ChargifyID == int.MinValue) throw new ArgumentNullException("ChargifyID");

            // now make the request
            string response = this.DoRequest(string.Format("product_families/{0}/components.{1}?include_archived={2}", ChargifyID, GetMethodExtension(), includeArchived ? "1" : "0"));
            var retValue = new Dictionary<int, IComponentInfo>();
            if (response.IsXml())
            {
                // now build a product list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response); // get the XML into an XML document
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node

                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "components")
                    {
                        foreach (XmlNode componentNode in elementNode.ChildNodes)
                        {
                            if (componentNode.Name == "component")
                            {
                                IComponentInfo LoadedComponent = new ComponentInfo(componentNode);
                                if (!retValue.ContainsKey(LoadedComponent.ID))
                                {
                                    retValue.Add(LoadedComponent.ID, LoadedComponent);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate id values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("component"))
                    {
                        JsonObject componentObj = (array.Items[i] as JsonObject)["component"] as JsonObject;
                        IComponentInfo LoadedComponent = new ComponentInfo(componentObj);
                        if (!retValue.ContainsKey(LoadedComponent.ID))
                        {
                            retValue.Add(LoadedComponent.ID, LoadedComponent);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate ID values detected");
                        }
                    }
                }
            }
            // return the dictionary
            return retValue;
        }

        /// <summary>
        /// Method for getting a list of components for a specific product family
        /// </summary>
        /// <param name="ChargifyID">The product family ID</param>
        /// <returns>A dictionary of components if there are results, null otherwise.</returns>
        public IDictionary<int, IComponentInfo> GetComponentsForProductFamily(int ChargifyID)
        {
            return GetComponentsForProductFamily(ChargifyID, false);
        }

        /// <summary>
        /// Method for getting a list of component usages for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscription ID to examine</param>
        /// <param name="ComponentID">The ID of the component to examine</param>
        /// <returns>A dictionary of usages if there are results, null otherwise.</returns>
        public IDictionary<string, IComponent> GetComponentList(int SubscriptionID, int ComponentID)
        {
            // make sure data is valid
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");
            if (ComponentID == int.MinValue) throw new ArgumentNullException("ComponentID");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions/{0}/components/{1}/usages.{2}", SubscriptionID, ComponentID, GetMethodExtension()));
            var retValue = new Dictionary<string, IComponent>();
            if (response.IsXml())
            {
                // now build a product list based on response XML
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response); // get the XML into an XML document
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "usages")
                    {
                        foreach (XmlNode usageNode in elementNode.ChildNodes)
                        {
                            if (usageNode.Name == "usage")
                            {
                                IComponent LoadedComponent = new Component(usageNode);
                                if (!retValue.ContainsKey(LoadedComponent.ID))
                                {
                                    retValue.Add(LoadedComponent.ID, LoadedComponent);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate id values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("component"))
                    {
                        JsonObject componentObj = (array.Items[i] as JsonObject)["component"] as JsonObject;
                        IComponent LoadedComponent = new Component(componentObj);
                        if (!retValue.ContainsKey(LoadedComponent.ID))
                        {
                            retValue.Add(LoadedComponent.ID, LoadedComponent);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate ID values detected");
                        }
                    }
                }
            }
            // return the list
            return retValue;
        }

        /// <summary>
        /// Method for adding a metered component usage to the subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to modify</param>
        /// <param name="ComponentID">The ID of the (metered or quantity) component to add a usage of</param>
        /// <param name="Quantity">The number of usages to add</param>
        /// <param name="Memo">The memo for the usage</param>
        /// <returns>The usage added if successful, otherwise null.</returns>
        public IUsage AddUsage(int SubscriptionID, int ComponentID, int Quantity, string Memo)
        {
            // Chargify DOES currently allow a negative value for "quantity", so allow users to call this method that way.
            //if (Quantity < 0) throw new ArgumentNullException("Quantity");
            if (string.IsNullOrEmpty(Memo)) throw new ArgumentNullException("Memo");
            // make sure that the SubscriptionID is unique
            if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("Not an SubscriptionID", "SubscriptionID");
            // create XML for addition of usage
            StringBuilder UsageXML = new StringBuilder(GetXMLStringIfApplicable());
            UsageXML.Append("<usage>");
            UsageXML.AppendFormat("<quantity>{0}</quantity>", Quantity);
            UsageXML.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(Memo));
            UsageXML.Append("</usage>");
            string response = this.DoRequest(string.Format("subscriptions/{0}/components/{1}/usages.{2}", SubscriptionID, ComponentID, GetMethodExtension()), HttpRequestMethod.Post, UsageXML.ToString());
            // change the response to the object            
            return response.ConvertResponseTo<Usage>("usage");
        }

        /// <summary>
        /// Method for turning on or off a component
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify</param>
        /// <param name="ComponentID">The ID of the component (on/off only) to modify</param>
        /// <param name="SetEnabled">True if wanting to turn the component "on", false otherwise.</param>
        /// <returns>IComponentAttributes object if successful, null otherwise.</returns>
        public IComponentAttributes SetComponent(int SubscriptionID, int ComponentID, bool SetEnabled)
        {
            try
            {
                if (ComponentID == int.MinValue) throw new ArgumentException("Not an ComponentID", "ComponentID");
                // make sure that the SubscriptionID is unique
                if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("Not an SubscriptionID", "SubscriptionID");
                // create XML for addition of usage
                StringBuilder ComponentXML = new StringBuilder(GetXMLStringIfApplicable());
                ComponentXML.Append("<component>");
                ComponentXML.AppendFormat("<enabled>{0}</enabled>", SetEnabled.ToString(CultureInfo.InvariantCulture));
                ComponentXML.Append("</component>");
                string response = this.DoRequest(string.Format("subscriptions/{0}/components/{1}.{2}", SubscriptionID, ComponentID, GetMethodExtension()), HttpRequestMethod.Put, ComponentXML.ToString());
                return response.ConvertResponseTo<ComponentAttributes>("component");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        #endregion

        #region Component Allocations
        /// <summary>
        /// Returns the 50 most recent Allocations, ordered by most recent first.
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to scope this request</param>
        /// <param name="ComponentID">The componentID to scope this request</param>
        /// <param name="Page">Pass an integer in the page parameter via the query string to access subsequent pages of 50 transactions</param>
        /// <returns>A dictionary of allocation objects keyed by ComponentID, or null.</returns>
        public IDictionary<int, List<IComponentAllocation>> GetAllocationListForSubscriptionComponent(int SubscriptionID, int ComponentID, int? Page = 0)
        {
            // make sure data is valid
            if (SubscriptionID == int.MinValue) throw new ArgumentNullException("SubscriptionID");
            if (Page.HasValue && Page.Value < 0) throw new ArgumentOutOfRangeException("Page number must be a positive integer", "Page");

            try
            {
                string qs = string.Empty;
                // Add the request options to the query string
                if (Page.Value > 0) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", Page); }

                // now make the request
                string url = string.Format("subscriptions/{0}/components/{1}/allocations.{2}", SubscriptionID, ComponentID, GetMethodExtension());
                if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
                string response = this.DoRequest(url);
                var retValue = new Dictionary<int, List<IComponentAllocation>>();
                if (response.IsXml())
                {
                    // now build a product list based on response XML
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(response); // get the XML into an XML document
                    if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                    // loop through the child nodes of this node

                    foreach (XmlNode elementNode in doc.ChildNodes)
                    {
                        if (elementNode.Name == ComponentAllocation.AllocationsRootKey)
                        {
                            List<IComponentAllocation> childComponentAllocations = new List<IComponentAllocation>();
                            foreach (XmlNode componentAllocationNode in elementNode.ChildNodes)
                            {
                                if (componentAllocationNode.Name == ComponentAllocation.AllocationRootKey)
                                {
                                    IComponentAllocation componentAllocation = new ComponentAllocation(componentAllocationNode);
                                    childComponentAllocations.Add(componentAllocation);
                                }
                            }

                            if (!retValue.ContainsKey(ComponentID) && childComponentAllocations.Count > 0)
                            {
                                retValue.Add(ComponentID, childComponentAllocations);
                            }
                        }
                    }
                }
                else if (response.IsJSON())
                {
                    // should be expecting an array
                    int position = 0;
                    JsonArray array = JsonArray.Parse(response, ref position);
                    List<IComponentAllocation> childComponentAllocations = new List<IComponentAllocation>();
                    for (int i = 0; i <= array.Length - 1; i++)
                    {
                        if ((array.Items[i] as JsonObject).ContainsKey(ComponentAllocation.AllocationRootKey))
                        {
                            JsonObject componentObj = (array.Items[i] as JsonObject)[ComponentAllocation.AllocationRootKey] as JsonObject;
                            IComponentAllocation loadedComponentAllocation = new ComponentAllocation(componentObj);
                            childComponentAllocations.Add(loadedComponentAllocation);
                        }
                    }

                    if (!retValue.ContainsKey(ComponentID) && childComponentAllocations.Count > 0)
                    {
                        retValue.Add(ComponentID, childComponentAllocations);
                    }
                }
                // return the dictionary
                return retValue;
            }
            catch (ChargifyException cEx)
            {
                if (cEx.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="SubscriptionID"></param>
        /// <param name="ComponentID"></param>
        /// <param name="Allocation"></param>
        /// <returns></returns>
        public IComponentAllocation CreateComponentAllocation(int SubscriptionID, int ComponentID, ComponentAllocation Allocation)
        {
            if (Allocation == null) throw new ArgumentNullException("Allocation");
            return CreateComponentAllocation(SubscriptionID, ComponentID, Allocation.Quantity, Allocation.Memo, Allocation.UpgradeScheme, Allocation.DowngradeScheme);
        }

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this quantity allocation to</param>
        /// <param name="ComponentID">The ID of the component to apply this quantity allocation to</param>
        /// <param name="Quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <returns></returns>
        public IComponentAllocation CreateComponentAllocation(int SubscriptionID, int ComponentID, int Quantity)
        {
            return CreateComponentAllocation(SubscriptionID, ComponentID, Quantity, string.Empty, ComponentUpgradeProrationScheme.Unknown, ComponentDowngradeProrationScheme.Unknown);
        }

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this quantity allocation to</param>
        /// <param name="ComponentID">The ID of the component to apply this quantity allocation to</param>
        /// <param name="Quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <param name="Memo">(optional) A memo to record along with the allocation</param>
        /// <returns></returns>
        public IComponentAllocation CreateComponentAllocation(int SubscriptionID, int ComponentID, int Quantity, string Memo)
        {
            return CreateComponentAllocation(SubscriptionID, ComponentID, Quantity, Memo, ComponentUpgradeProrationScheme.Unknown, ComponentDowngradeProrationScheme.Unknown);
        }

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this quantity allocation to</param>
        /// <param name="ComponentID">The ID of the component to apply this quantity allocation to</param>
        /// <param name="Quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <param name="Memo">(optional) A memo to record along with the allocation</param>
        /// <param name="UpgradeScheme">(optional) The scheme used if the proration is an upgrade. Defaults to the site setting if one is not provided.</param>
        /// <param name="DowngradeScheme">(optional) The scheme used if the proration is a downgrade. Defaults to the site setting if one is not provided.</param>
        /// <returns>The component allocation object, null otherwise.</returns>
        public IComponentAllocation CreateComponentAllocation(int SubscriptionID, int ComponentID, int Quantity, string Memo, ComponentUpgradeProrationScheme UpgradeScheme, ComponentDowngradeProrationScheme DowngradeScheme)
        {
            try
            {
                string xml = BuildComponentAllocationXML(Quantity, Memo, UpgradeScheme, DowngradeScheme);

                // perform the request, keep the response
                string response = this.DoRequest(string.Format("subscriptions/{0}/components/{1}/allocations.{2}", SubscriptionID, ComponentID, GetMethodExtension()), HttpRequestMethod.Post, xml);

                // change the response to the object
                return response.ConvertResponseTo<ComponentAllocation>("allocation");
            }
            catch (ChargifyException cEx)
            {
                if (cEx.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Constructs the XML needed to create a component allocation
        /// </summary>
        /// <param name="Quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <param name="Memo">(optional) A memo to record along with the allocation</param>
        /// <param name="UpgradeScheme">(optional) The scheme used if the proration is an upgrade. Defaults to the site setting if one is not provided.</param>
        /// <param name="DowngradeScheme">(optional) The scheme used if the proration is a downgrade. Defaults to the site setting if one is not provided.</param>
        /// <returns>The formatted XML</returns>
        private string BuildComponentAllocationXML(int Quantity, string Memo, ComponentUpgradeProrationScheme UpgradeScheme, ComponentDowngradeProrationScheme DowngradeScheme)
        {
            // make sure data is valid
            if (Quantity < 0 && !Quantity.Equals(int.MinValue)) throw new ArgumentOutOfRangeException("Quantity", Quantity, "Quantity must be valid");

            // create XML for creation of a ComponentAllocation
            StringBuilder ComponentAllocationXML = new StringBuilder(GetXMLStringIfApplicable());
            ComponentAllocationXML.Append("<allocation>");
            ComponentAllocationXML.Append(string.Format("<quantity>{0}</quantity>", Quantity));
            if (!string.IsNullOrEmpty(Memo)) { ComponentAllocationXML.Append(string.Format("<memo>{0}</memo>", HttpUtility.HtmlEncode(Memo))); }
            if (UpgradeScheme != ComponentUpgradeProrationScheme.Unknown)
            {
                ComponentAllocationXML.Append(string.Format("<proration_upgrade_scheme>{0}</proration_upgrade_scheme>", Enum.GetName(typeof(ComponentUpgradeProrationScheme), UpgradeScheme).ToLowerInvariant().Replace("_", "-")));
            }
            if (DowngradeScheme != ComponentDowngradeProrationScheme.Unknown)
            {
                ComponentAllocationXML.Append(string.Format("<proration_downgrade_scheme>{0}</proration_downgrade_scheme>", Enum.GetName(typeof(ComponentDowngradeProrationScheme), DowngradeScheme).ToLowerInvariant().Replace("_", "-")));
            }
            ComponentAllocationXML.Append("</allocation>");
            return ComponentAllocationXML.ToString();

        }
        #endregion

        #region Transactions
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList()
        {
            return GetTransactionList(int.MinValue, int.MinValue, null, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(List<TransactionType> kinds)
        {
            return GetTransactionList(int.MinValue, int.MinValue, kinds, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(List<TransactionType> kinds, int since_id, int max_id)
        {
            return GetTransactionList(int.MinValue, int.MinValue, kinds, since_id, max_id, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="since_date">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="until_date">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(List<TransactionType> kinds, int since_id, int max_id, DateTime since_date, DateTime until_date)
        {
            return GetTransactionList(int.MinValue, int.MinValue, kinds, since_id, max_id, since_date, until_date);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(int page, int per_page)
        {
            return GetTransactionList(page, per_page, null, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(int page, int per_page, List<TransactionType> kinds)
        {
            return GetTransactionList(page, per_page, kinds, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(int page, int per_page, List<TransactionType> kinds, int since_id, int max_id)
        {
            return GetTransactionList(page, per_page, kinds, since_id, max_id, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="since_date">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="until_date">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(int page, int per_page, List<TransactionType> kinds, int since_id, int max_id, DateTime since_date, DateTime until_date)
        {
            string qs = string.Empty;

            // Add the transaction options to the query string ...
            if (page != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", page); }
            if (per_page != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("per_page={0}", per_page); }

            if (kinds != null)
            {
                foreach (TransactionType kind in kinds)
                {
                    // Iterate through them all, except for Unknown - which isn't supported, just used internally.
                    if (kind == TransactionType.Unknown) break;

                    // Append the kind to the query string ...
                    if (qs.Length > 0) { qs += "&"; }
                    qs += string.Format("kinds[]={0}", kind.ToString().ToLower());
                }
            }

            if (since_id != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("since_id={0}", since_id); }
            if (max_id != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("max_id={0}", max_id); }
            if (since_date != DateTime.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("since_date={0}", since_date.ToString(DateTimeFormat)); }
            if (until_date != DateTime.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("until_date={0}", until_date.ToString(DateTimeFormat)); }

            // Construct the url to access Chargify
            string url = string.Format("transactions.{0}", GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = this.DoRequest(url);

            var retValue = new Dictionary<int, ITransaction>();
            if (response.IsXml())
            {
                // now build a transaction list based on response XML
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response);
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "transactions")
                    {
                        foreach (XmlNode transactionNode in elementNode.ChildNodes)
                        {
                            if (transactionNode.Name == "transaction")
                            {
                                ITransaction LoadedTransaction = new Transaction(transactionNode);
                                if (!retValue.ContainsKey(LoadedTransaction.ID))
                                {
                                    retValue.Add(LoadedTransaction.ID, LoadedTransaction);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate id values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("transaction"))
                    {
                        JsonObject transactionObj = (array.Items[i] as JsonObject)["transaction"] as JsonObject;
                        ITransaction LoadedTransaction = new Transaction(transactionObj);
                        if (!retValue.ContainsKey(LoadedTransaction.ID))
                        {
                            retValue.Add(LoadedTransaction.ID, LoadedTransaction);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate ID values detected");
                        }
                    }
                }
            }
            // return the list
            return retValue;
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID)
        {
            if (SubscriptionID < 0) throw new ArgumentNullException("SubscriptionID");

            return GetTransactionsForSubscription(SubscriptionID, int.MinValue, int.MinValue, null, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, List<TransactionType> kinds)
        {
            return GetTransactionsForSubscription(SubscriptionID, int.MinValue, int.MinValue, kinds, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, List<TransactionType> kinds, int since_id, int max_id)
        {
            return GetTransactionsForSubscription(SubscriptionID, int.MinValue, int.MinValue, kinds, since_id, max_id, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="since_date">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="until_date">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, List<TransactionType> kinds, int since_id, int max_id, DateTime since_date, DateTime until_date)
        {
            return GetTransactionsForSubscription(SubscriptionID, int.MinValue, int.MinValue, kinds, since_id, max_id, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, int page, int per_page)
        {
            return GetTransactionsForSubscription(SubscriptionID, page, per_page, null, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, int page, int per_page, List<TransactionType> kinds)
        {
            return GetTransactionsForSubscription(SubscriptionID, page, per_page, kinds, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, int page, int per_page, List<TransactionType> kinds, int since_id, int max_id)
        {
            return GetTransactionsForSubscription(SubscriptionID, page, per_page, kinds, since_id, max_id, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="since_date">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="until_date">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, int page, int per_page, List<TransactionType> kinds, int since_id, int max_id, DateTime since_date, DateTime until_date)
        {
            string qs = string.Empty;

            if (page != int.MinValue)
            {
                if (qs.Length > 0) { qs += "&"; }
                qs += string.Format("page={0}", page);
            }

            if (per_page != int.MinValue)
            {
                if (qs.Length > 0) { qs += "&"; }
                qs += string.Format("per_page={0}", per_page);
            }

            if (kinds != null)
            {
                foreach (TransactionType kind in kinds)
                {
                    // Iterate through them all, except for Unknown - which isn't supported, just used internally.
                    if (kind == TransactionType.Unknown) break;

                    // Append the kind to the query string ...
                    if (qs.Length > 0) { qs += "&"; }
                    qs += string.Format("kinds[]={0}", kind.ToString().ToLower());
                }
            }

            if (since_id != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("since_id={0}", since_id); }
            if (max_id != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("max_id={0}", max_id); }
            if (since_date != DateTime.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("since_date={0}", since_date.ToString(DateTimeFormat)); }
            if (until_date != DateTime.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("until_date={0}", until_date.ToString(DateTimeFormat)); }

            // now make the request
            string url = string.Format("subscriptions/{0}/transactions.{1}", SubscriptionID, GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = this.DoRequest(url);
            var retValue = new Dictionary<int, ITransaction>();
            if (response.IsXml())
            {
                // now build a transaction list based on response XML
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response); // get the XML into an XML document
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "transactions")
                    {
                        foreach (XmlNode transactionNode in elementNode.ChildNodes)
                        {
                            if (transactionNode.Name == "transaction")
                            {
                                ITransaction LoadedTransaction = new Transaction(transactionNode);
                                if (!retValue.ContainsKey(LoadedTransaction.ID))
                                {
                                    retValue.Add(LoadedTransaction.ID, LoadedTransaction);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate id values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i < array.Length; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("transaction"))
                    {
                        JsonObject transactionObj = (array.Items[i] as JsonObject)["transaction"] as JsonObject;
                        ITransaction LoadedTransaction = new Transaction(transactionObj);
                        if (!retValue.ContainsKey(LoadedTransaction.ID))
                        {
                            retValue.Add(LoadedTransaction.ID, LoadedTransaction);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate ID values detected");
                        }
                    }
                }
            }
            // return the list
            return retValue;
        }

        /// <summary>
        /// Load the requested transaction from Chargify
        /// </summary>
        /// <param name="ID">The ID of the transaction</param>
        /// <returns>The transaction with the specified ID</returns>
        public ITransaction LoadTransaction(int ID)
        {
            try
            {
                // make sure data is valid
                if (ID < 0) throw new ArgumentNullException("ID");
                // now make the request
                string response = this.DoRequest(string.Format("transactions/{0}.{1}", ID, GetMethodExtension()));
                // Convert the Chargify response into the object we're looking for
                return response.ConvertResponseTo<Transaction>("transaction");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        #endregion

        #region Refunds
        /// <summary>
        /// Create a refund
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify</param>
        /// <param name="PaymentID">The ID of the payment that the credit will be applied to</param>
        /// <param name="Amount">The amount (in dollars and cents) like 10.00 is $10.00</param>
        /// <param name="Memo">A helpful explanation for the refund.</param>
        /// <returns>The IRefund object indicating successful, or unsuccessful.</returns>
        public IRefund CreateRefund(int SubscriptionID, int PaymentID, decimal Amount, string Memo)
        {
            int AmountInCents = Convert.ToInt32(Amount * 100);
            return CreateRefund(SubscriptionID, PaymentID, AmountInCents, Memo);
        }

        /// <summary>
        /// Create a refund
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify</param>
        /// <param name="PaymentID">The ID of the payment that the credit will be applied to</param>
        /// <param name="AmountInCents">The amount (in cents only) like 100 is $1.00</param>
        /// <param name="Memo">A helpful explanation for the refund.</param>
        /// <returns>The IRefund object indicating successful, or unsuccessful.</returns>
        public IRefund CreateRefund(int SubscriptionID, int PaymentID, int AmountInCents, string Memo)
        {
            if (AmountInCents < 0) throw new ArgumentNullException("AmountInCents");
            if (string.IsNullOrEmpty(Memo)) throw new ArgumentException("Can't have an empty memo", "Memo");
            // make sure that the SubscriptionID is unique
            if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("Not an SubscriptionID", "SubscriptionID");
            // create XML for addition of refund
            StringBuilder RefundXML = new StringBuilder(GetXMLStringIfApplicable());
            RefundXML.Append("<refund>");
            RefundXML.AppendFormat("<payment_id>{0}</payment_id>", PaymentID);
            RefundXML.AppendFormat("<amount_in_cents>{0}</amount_in_cents>", AmountInCents);
            RefundXML.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(Memo));
            RefundXML.Append("</refund>");
            string response = this.DoRequest(string.Format("subscriptions/{0}/refunds.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Post, RefundXML.ToString());
            // change the response to the object            
            return response.ConvertResponseTo<Refund>("refund");
        }
        #endregion

        #region Statements
        /// <summary>
        /// Method for getting a specific statement
        /// </summary>
        /// <param name="StatementID">The ID of the statement to retrieve</param>
        /// <returns>The statement if found, null otherwise.</returns>
        public IStatement LoadStatement(int StatementID)
        {
            try
            {
                // make sure data is valid
                if (StatementID <= 0) throw new ArgumentNullException("StatementID");
                // now make the request
                string response = this.DoRequest(string.Format("statements/{0}.{1}", StatementID, GetMethodExtension()));
                // Convert the Chargify response into the object we're looking for
                return response.ConvertResponseTo<Statement>("statement");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Individual PDF Statements can be retrieved by using the Accept/Content-Type header application/pdf or appending .pdf as the format portion of the URL:
        /// </summary>
        /// <param name="StatementID">The ID of the statement to retrieve the byte[] for</param>
        /// <returns>A byte[] of the PDF data, to be sent to the user in a download</returns>
        public byte[] LoadStatementPDF(int StatementID)
        {
            try
            {
                // make sure data is valid
                if (StatementID <= 0) throw new ArgumentNullException("StatementID");

                // now make the request
                byte[] response = this.DoFileRequest(string.Format("statements/{0}.pdf", StatementID), HttpRequestMethod.Get, string.Empty);

                return response;
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Method for getting a list of statment ids for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to retrieve the statements for</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        public IList<int> GetStatementIDs(int SubscriptionID)
        {
            return GetStatementIDs(SubscriptionID, int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Method for getting a list of statment ids for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to retrieve the statements for</param>
        /// <param name="page">The page number to return</param>
        /// <param name="per_page">The number of results to return per page</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        public IList<int> GetStatementIDs(int SubscriptionID, int page, int per_page)
        {
            string qs = string.Empty;

            // Add the transaction options to the query string ...
            if (page != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", page); }
            if (per_page != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("per_page={0}", per_page); }

            // Construct the url to access Chargify
            string url = string.Format("subscriptions/{0}/statements/ids.{1}", SubscriptionID, GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = this.DoRequest(url);

            var retValue = new List<int>();
            if (response.IsXml())
            {
                // now build a statement list based on response XML
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response);
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "statement_ids")
                    {
                        foreach (XmlNode statementIDNode in elementNode.ChildNodes)
                        {
                            if (statementIDNode.Name == "id")
                            {
                                int statementID = Convert.ToInt32(statementIDNode.InnerText);
                                if (!retValue.Contains(statementID))
                                {
                                    retValue.Add(statementID);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate ID values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                JsonObject statementIDSObj = JsonObject.Parse(response);
                if (!statementIDSObj.ContainsKey("statement_ids"))
                    throw new InvalidOperationException("Returned JSON not valid");

                JsonArray array = (statementIDSObj["statement_ids"]) as JsonArray;
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    JsonNumber statementIDValue = array.Items[i] as JsonNumber;
                    if (statementIDValue == null)
                        throw new InvalidOperationException("Statement ID is not a valid number");
                    if (!retValue.Contains(statementIDValue.IntValue))
                    {
                        retValue.Add(statementIDValue.IntValue);
                    }
                    else
                    {
                        throw new InvalidOperationException("Duplicate ID values detected");
                    }
                }
            }
            return retValue;

        }

        /// <summary>
        /// Method for getting a list of statments for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to retrieve the statements for</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        public IDictionary<int, IStatement> GetStatementList(int SubscriptionID)
        {
            return GetStatementList(SubscriptionID, int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Method for getting a list of statments for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to retrieve the statements for</param>
        /// <param name="page">The page number to return</param>
        /// <param name="per_page">The number of results to return per page</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        public IDictionary<int, IStatement> GetStatementList(int SubscriptionID, int page, int per_page)
        {
            string qs = string.Empty;

            // Add the transaction options to the query string ...
            if (page != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", page); }
            if (per_page != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("per_page={0}", per_page); }

            // Construct the url to access Chargify
            string url = string.Format("subscriptions/{0}/statements.{1}", SubscriptionID, GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = this.DoRequest(url);

            var retValue = new Dictionary<int, IStatement>();
            if (response.IsXml())
            {
                // now build a statement list based on response XML
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(response);
                if (Doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in Doc.ChildNodes)
                {
                    if (elementNode.Name == "statements")
                    {
                        foreach (XmlNode statementNode in elementNode.ChildNodes)
                        {
                            if (statementNode.Name == "statement")
                            {
                                IStatement LoadedStatement = new Statement(statementNode);
                                if (!retValue.ContainsKey(LoadedStatement.ID))
                                {
                                    retValue.Add(LoadedStatement.ID, LoadedStatement);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate ID values detected");
                                }
                            }
                        }
                    }
                }
            }
            else if (response.IsJSON())
            {
                // should be expecting an array
                int position = 0;
                JsonArray array = JsonArray.Parse(response, ref position);
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if ((array.Items[i] as JsonObject).ContainsKey("statement"))
                    {
                        JsonObject statementObj = (array.Items[i] as JsonObject)["statement"] as JsonObject;
                        IStatement LoadedStatement = new Statement(statementObj);
                        if (!retValue.ContainsKey(LoadedStatement.ID))
                        {
                            retValue.Add(LoadedStatement.ID, LoadedStatement);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate ID values detected");
                        }
                    }
                }
            }
            return retValue;
        }
        #endregion

        #region Statistics

        /// <summary>
        /// Method for getting the statstics of a Chargify site
        /// </summary>
        /// <returns>The site statistics if applicable.</returns>
        public ISiteStatistics GetSiteStatistics()
        {
            string response = this.DoRequest("stats.json");
            if (response.IsJSON())
            {
                JsonObject obj = JsonObject.Parse(response);
                ISiteStatistics stats = new SiteStatistics(obj);
                return stats;
            }
            else
            {
                throw new Exception("Json not returned from server");
            }
        }

        #endregion

        #region Adjustments
        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to adjust</param>
        /// <param name="amount">The amount (in dollars and cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        public IAdjustment CreateAdjustment(int SubscriptionID, decimal amount, string memo)
        {
            return CreateAdjustment(SubscriptionID, amount, int.MinValue, memo, AdjustmentMethod.Default);
        }

        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to adjust</param>
        /// <param name="amount">The amount (in dollars and cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <param name="method">A string that toggles how the adjustment should be applied</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        public IAdjustment CreateAdjustment(int SubscriptionID, decimal amount, string memo, AdjustmentMethod method)
        {
            return CreateAdjustment(SubscriptionID, amount, int.MinValue, memo, method);
        }

        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to adjust</param>
        /// <param name="amount_in_cents">The amount (in cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        public IAdjustment CreateAdjustment(int SubscriptionID, int amount_in_cents, string memo)
        {
            return CreateAdjustment(SubscriptionID, decimal.MinValue, amount_in_cents, memo, AdjustmentMethod.Default);
        }

        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to adjust</param>
        /// <param name="amount_in_cents">The amount (in cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <param name="method">A string that toggles how the adjustment should be applied</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        public IAdjustment CreateAdjustment(int SubscriptionID, int amount_in_cents, string memo, AdjustmentMethod method)
        {
            return CreateAdjustment(SubscriptionID, decimal.MinValue, amount_in_cents, memo, method);
        }

        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to adjust</param>
        /// <param name="amount">The amount (in dollars and cents)</param>
        /// <param name="amount_in_cents">The amount (in cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <param name="method">A string that toggles how the adjustment should be applied</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        private IAdjustment CreateAdjustment(int SubscriptionID, decimal amount, int amount_in_cents, string memo, AdjustmentMethod method)
        {
            int value_in_cents = 0;
            if (amount == decimal.MinValue) value_in_cents = amount_in_cents;
            if (amount_in_cents == int.MinValue) value_in_cents = Convert.ToInt32(amount * 100);
            if (value_in_cents == int.MinValue) value_in_cents = 0;
            decimal value = Convert.ToDecimal((double)(value_in_cents) / 100.0);

            // make sure data is valid
            if (string.IsNullOrEmpty(memo)) throw new ArgumentNullException("Memo");
            // make sure that the SubscriptionID is unique
            if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("Not an SubscriptionID", "SubscriptionID");
            // create XML for creation of an adjustment
            StringBuilder AdjustmentXML = new StringBuilder(GetXMLStringIfApplicable());
            AdjustmentXML.Append("<adjustment>");
            AdjustmentXML.AppendFormat("<amount>{0}</amount>", value.ToChargifyCurrencyFormat());
            AdjustmentXML.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo));
            if (method != AdjustmentMethod.Default) { AdjustmentXML.AppendFormat("<adjustment_method>{0}</adjustment_method>", method.ToString().ToLowerInvariant()); }
            AdjustmentXML.Append("</adjustment>");
            // now make the request
            string response = this.DoRequest(string.Format("subscriptions/{0}/adjustments.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Post, AdjustmentXML.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Adjustment>("adjustment");
        }
        #endregion

        #region Billing Portal
        /// <summary>
        /// From http://docs.chargify.com/api-billing-portal
        /// </summary>
        public IBillingManagementInfo GetManagementLink(int ChargifyID)
        {
            try
            {
                // make sure data is valid
                if (ChargifyID < 0) throw new ArgumentNullException("ChargifyID");

                // now make the request
                string response = this.DoRequest(string.Format("portal/customers/{0}/management_link.{1}", ChargifyID, GetMethodExtension()));

                // Convert the Chargify response into the object we're looking for
                return response.ConvertResponseTo<BillingManagementInfo>("management_link");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }
        #endregion

        #region Invoices
        /// <summary>
        /// Gets a list of invoices
        /// </summary>
        /// <returns></returns>
        public IDictionary<int, Invoice> GetInvoiceList()
        {
            // Construct the url to access Chargify
            string url = string.Format("invoices.{0}", GetMethodExtension());
            string response = this.DoRequest(url);
            var retValue = new Dictionary<int, Invoice>();
            if (response.IsXml())
            {
                // now build an invoice list based on response XML
                retValue = GetListedXmlResponse<Invoice>("invoice", response);
            }
            else if (response.IsJSON())
            {
                // now build an invoice list based on response JSON
                retValue = GetListedJSONResponse<Invoice>("invoice", response);
            }
            return retValue;
        }
        #endregion

        #region Sites
        /// <summary>
        /// Clean up a site in test mode.
        /// </summary>
        /// <param name="CleanupScope">What should be cleaned? DEFAULT IS CUSTOMERS ONLY.</param>
        /// <returns>True if complete, false otherwise</returns>
        /// <remarks>If used against a production site, the result will always be false.</remarks>
        public bool ClearTestSite(SiteCleanupScope? CleanupScope = SiteCleanupScope.Customers)
        {
            bool retVal = false;

            try
            {
                var qs = string.Empty;

                if (CleanupScope != null && CleanupScope.HasValue)
                {
                    qs += string.Format("cleanup_scope={0}", Enum.GetName(typeof(SiteCleanupScope), CleanupScope.Value).ToLowerInvariant());
                }
                string url = string.Format("sites/clear_data.{0}", GetMethodExtension());
                if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }

                string response = this.DoRequest(url, HttpRequestMethod.Post, null);

                // All we're expecting back is 200 OK when it works, and 403 FORBIDDEN when it's not being called appropriately.
                retVal = true;
            }
            catch (ChargifyException) { }

            return retVal;
        }
        #endregion

        #region Payments
        /// <summary>
        /// Chargify allows you to record payments that occur outside of the normal flow of payment processing.
        /// These payments are considered external payments.A common case to apply such a payment is when a 
        /// customer pays by check or some other means for their subscription.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this manual payment record to</param>
        /// <param name="Amount">The decimal amount of the payment (ie. 10.00 for $10)</param>
        /// <param name="Memo">The memo to include with the manual payment</param>
        /// <returns>The payment result, null otherwise.</returns>
        public IPayment AddPayment(int SubscriptionID, decimal Amount, string Memo)
        {
            return AddPayment(SubscriptionID, Convert.ToInt32(Amount * 100), Memo);
        }

        /// <summary>
        /// Chargify allows you to record payments that occur outside of the normal flow of payment processing.
        /// These payments are considered external payments.A common case to apply such a payment is when a 
        /// customer pays by check or some other means for their subscription.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this manual payment record to</param>
        /// <param name="AmountInCents">The amount in cents of the payment (ie. $10 would be 1000 cents)</param>
        /// <param name="Memo">The memo to include with the manual payment</param>
        /// <returns>The payment result, null otherwise.</returns>
        public IPayment AddPayment(int SubscriptionID, int AmountInCents, string Memo)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(Memo)) throw new ArgumentNullException("Memo");
            // make sure that the SubscriptionID is unique
            if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("Not an SubscriptionID", "SubscriptionID");

            // create XML for creation of a payment
            var PaymentXML = new StringBuilder(GetXMLStringIfApplicable());
            PaymentXML.Append("<payment>");
            PaymentXML.AppendFormat("<amount_in_cents>{0}</amount_in_cents>", AmountInCents);
            PaymentXML.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(Memo));
            PaymentXML.Append("</payment>");

            // now make the request
            string response = this.DoRequest(string.Format("subscriptions/{0}/payments.{1}", SubscriptionID, GetMethodExtension()), HttpRequestMethod.Post, PaymentXML.ToString());

            // change the response to the object
            return response.ConvertResponseTo<Payment>("payment");
        }

        #endregion

        #region Utility Methods
        private Dictionary<int, T> GetListedJSONResponse<T>(string key, string response)
            where T : class, IChargifyEntity
        {
            var retValue = Activator.CreateInstance<Dictionary<int, T>>();

            // should be expecting an array
            int position = 0;
            JsonArray array = JsonArray.Parse(response, ref position);
            for (int i = 0; i <= array.Length - 1; i++)
            {
                if ((array.Items[i] as JsonObject).ContainsKey("statement"))
                {
                    JsonObject jsonObj = (array.Items[i] as JsonObject)["statement"] as JsonObject;
                    T value = (T)Activator.CreateInstance(typeof(T), jsonObj);
                    if (!retValue.ContainsKey(value.ID))
                    {
                        retValue.Add(value.ID, value);
                    }
                    else
                    {
                        throw new InvalidOperationException("Duplicate ID values detected");
                    }
                }
            }
            return retValue;
        }

        private Dictionary<int, T> GetListedXmlResponse<T>(string key, string response)
            where T : class, IChargifyEntity
        {
            // now build an invoice list based on response XML
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(response);
            if (Doc.ChildNodes.Count == 0)
                throw new InvalidOperationException("Returned XML not valid");

            var retValue = Activator.CreateInstance<Dictionary<int, T>>();

            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == string.Format("{0}s", key))
                {
                    foreach (XmlNode childNode in elementNode.ChildNodes)
                    {
                        if (childNode.Name == key)
                        {
                            T value = (T)Activator.CreateInstance(typeof(T), childNode);
                            if (!retValue.ContainsKey(value.ID))
                            {
                                retValue.Add(value.ID, value);
                            }
                            else
                            {
                                throw new InvalidOperationException("Duplicate ID values detected");
                            }
                        }
                    }
                }
            }

            return retValue;
        }
        #endregion

        #region Request Methods

        /// <summary>
        /// Should the URI method extension be json or xml?
        /// </summary>
        /// <returns>Either "json" or "xml" depending on how UseJSON is set.</returns>
        private string GetMethodExtension()
        {
            return (this.UseJSON == true) ? "json" : "xml";
        }

        private string GetXMLStringIfApplicable()
        {
            string result = string.Empty;
            if (!this.UseJSON)
            {
                result = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            }
            return result;
        }

        /// <summary>
        /// Make a GET request to Chargify
        /// </summary>
        /// <param name="methodString">The method string for the request.  This is appended to the base URL</param>
        /// <returns>The xml response to the request</returns>
        private string DoRequest(string methodString)
        {
            return DoRequest(methodString, HttpRequestMethod.Get, null);
        }

        /// <summary>
        /// Method for retrieving a file via the API
        /// </summary>
        /// <param name="methodString"></param>
        /// <param name="requestMethod"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private byte[] DoFileRequest(string methodString, HttpRequestMethod requestMethod, string postData)
        {
            // make sure values are set
            if (string.IsNullOrEmpty(this.URL)) throw new InvalidOperationException("URL not set");
            if (string.IsNullOrEmpty(this.apiKey)) throw new InvalidOperationException("apiKey not set");
            if (string.IsNullOrEmpty(this.Password)) throw new InvalidOperationException("Password not set");

            if (_protocolType != null)
            {
                ServicePointManager.SecurityProtocol = _protocolType.Value;
            }

            // create the URI
            string addressString = string.Format("{0}{1}{2}", this.URL, (this.URL.EndsWith("/") ? "" : "/"), methodString);
            var uriBuilder = new UriBuilder(addressString)
            {
                Scheme = Uri.UriSchemeHttps,
                Port = -1 // default port for scheme
            };
            Uri address = uriBuilder.Uri;

            // Create the web request
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Timeout = 180000;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(this.apiKey + ":" + this.Password));
            request.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

            // Set type to POST
            request.Method = requestMethod.ToString().ToUpper();
            request.SendChunked = false;
            if (!this.UseJSON)
            {
                request.ContentType = "text/xml";
                request.Accept = "application/xml";
            }
            else
            {
                request.ContentType = "application/json";
                request.Accept = "application/json";
            }

            if (methodString.EndsWith(".pdf"))
            {
                request.ContentType = "application/pdf";
                request.Accept = "application/pdf";
            }

            request.UserAgent = UserAgent;
            // send data
            string dataToPost = postData;
            if (requestMethod == HttpRequestMethod.Post || requestMethod == HttpRequestMethod.Put || requestMethod == HttpRequestMethod.Delete)
            {
                bool hasWritten = false;
                // only write if there's data to write ...
                if (!string.IsNullOrEmpty(postData))
                {
                    if (this.UseJSON == true)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(postData);
                        dataToPost = XmlToJsonConverter.XmlToJSON(doc);
                    }

                    // Wrap the request stream with a text-based writer
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        // Write the XML/JSON text into the stream
                        writer.WriteLine(dataToPost);
                        writer.Close();
                        hasWritten = true;
                    }
                }

                if (!hasWritten && !string.IsNullOrEmpty(postData))
                {
                    request.ContentLength = postData.Length;
                }
                else if (!hasWritten && string.IsNullOrEmpty(postData))
                {
                    request.ContentLength = (postData != null) ? postData.Length : 0;
                }
            }
            // request the data
            try
            {
                if (LogRequest != null)
                {
                    LogRequest(requestMethod, addressString, dataToPost);
                }

                byte[] retValue = { };
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        using (var ms = new MemoryStream())
                        {
                            reader.BaseStream.CopyStream(ms);
                            retValue = ms.ToArray();
                        }
                        _lastResponse = response;
                    }

                    if (LogResponse != null)
                    {
                        LogResponse(response.StatusCode, addressString, string.Empty);
                    }
                }
                // return the result
                return retValue;
            }
            catch (WebException wex)
            {
                Exception newException = null;
                // build exception and set last response
                if (wex.Response != null)
                {
                    using (HttpWebResponse errorResponse = (HttpWebResponse)wex.Response)
                    {
                        newException = new ChargifyException(errorResponse, wex, postData);
                        _lastResponse = errorResponse;

                        if (LogResponse != null)
                        {
                            // Use the ChargifyException ToString override to provide the parsed errors
                            LogResponse(errorResponse.StatusCode, addressString, newException.ToString());
                        }
                    }
                }
                else
                {
                    _lastResponse = null;
                }
                // throw the approriate exception
                if (newException != null)
                {
                    throw newException;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Make a request to Chargify
        /// </summary>
        /// <param name="methodString">The method string for the request.  This is appended to the base URL</param>
        /// <param name="requestMethod">The request method (GET or POST)</param>
        /// <param name="postData">The data included as part of a POST, PUT or DELETE request</param>
        /// <returns>The xml response to the request</returns>
        private string DoRequest(string methodString, HttpRequestMethod requestMethod, string postData)
        {
            // make sure values are set
            if (string.IsNullOrEmpty(this.URL)) throw new InvalidOperationException("URL not set");
            if (string.IsNullOrEmpty(this.apiKey)) throw new InvalidOperationException("apiKey not set");
            if (string.IsNullOrEmpty(this.Password)) throw new InvalidOperationException("Password not set");

            if (_protocolType != null)
            {
                ServicePointManager.SecurityProtocol = _protocolType.Value;
            }

            // create the URI
            string addressString = string.Format("{0}{1}{2}", this.URL, (this.URL.EndsWith("/") ? string.Empty : "/"), methodString);

            var uriBuilder = new UriBuilder(addressString)
            {
                Scheme = Uri.UriSchemeHttps,
                Port = -1 // default port for scheme
            };
            Uri address = uriBuilder.Uri;

            // Create the web request
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Timeout = this.timeout;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(this.apiKey + ":" + this.Password));
            request.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
            request.UserAgent = UserAgent;
            request.SendChunked = false;

            // Set Content-Type and Accept headers
            request.Method = requestMethod.ToString().ToUpper();
            if (!this.UseJSON)
            {
                request.ContentType = "text/xml";
                request.Accept = "application/xml";
            }
            else
            {
                request.ContentType = "application/json";
                request.Accept = "application/json";
            }

            // Send the data (when applicable)
            string dataToPost = postData;
            if (requestMethod == HttpRequestMethod.Post || requestMethod == HttpRequestMethod.Put || requestMethod == HttpRequestMethod.Delete)
            {
                bool hasWritten = false;
                // only write if there's data to write ...
                if (!string.IsNullOrEmpty(postData))
                {
                    if (this.UseJSON == true)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(postData);
                        dataToPost = XmlToJsonConverter.XmlToJSON(doc);
                    }

                    // Wrap the request stream with a text-based writer
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        // Write the XML/JSON text into the stream
                        writer.WriteLine(dataToPost);
                        writer.Close();
                        hasWritten = true;
                    }
                }

                if (!hasWritten && !string.IsNullOrEmpty(postData))
                {
                    request.ContentLength = postData.Length;
                }
                else if (!hasWritten && string.IsNullOrEmpty(postData))
                {
                    request.ContentLength = (postData != null) ? postData.Length : 0;
                }
            }
            // request the data
            try
            {
                if (LogRequest != null)
                {
                    LogRequest(requestMethod, addressString, dataToPost);
                }

                string retValue = string.Empty;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        retValue = reader.ReadToEnd();
                        _lastResponse = response;
                    }

                    if (LogResponse != null)
                    {
                        LogResponse(response.StatusCode, addressString, retValue);
                    }
                }
                // return the result
                return retValue;
            }
            catch (WebException wex)
            {
                Exception newException = null;
                // build exception and set last response
                if (wex.Response != null)
                {
                    using (HttpWebResponse errorResponse = (HttpWebResponse)wex.Response)
                    {
                        newException = new ChargifyException(errorResponse, wex, postData);
                        _lastResponse = errorResponse;

                        if (LogResponse != null)
                        {
                            // Use the ChargifyException ToString override to provide the parsed errors
                            LogResponse(errorResponse.StatusCode, addressString, newException.ToString());
                        }
                    }
                }
                else
                {
                    _lastResponse = null;
                }
                // throw the approriate exception
                if (newException != null)
                {
                    throw newException;
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion
    }
}