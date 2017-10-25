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
    using Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Xml;
    using System.Globalization;
    #endregion

    /// <summary>
    /// Class containing methods for interfacing with the Chargify API via XML and JSON
    /// </summary>
    public class ChargifyConnect : IChargifyConnect
    {
        #region System Constants
        private const string DateTimeFormat = "yyyy-MM-dd";
        private const string UpdateShortName = "update_payment";

        #endregion

        #region Constructors

        private int _timeout = 180000;

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
            URL = url;
            this.apiKey = apiKey;
            Password = password;
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
            URL = url;
            this.apiKey = apiKey;
            Password = password;
            SharedKey = sharedKey;
        }

        #endregion

        #region Properties
        private static string UserAgent => _userAgent ??
                                           (_userAgent =
                                               string.Format("Chargify.NET Client v" +
                                                             System.Reflection.Assembly.GetExecutingAssembly().GetName().Version));

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
        public bool CvvRequired { get { return _cvvRequired; } set { _cvvRequired = value; } }
        private bool _cvvRequired = true;

        /// <summary>
        /// Allows you to specify the specific SecurityProtocolType. If not set, then
        /// the default is used.
        /// </summary>
        public SecurityProtocolType? ProtocolType
        {
            get { return _protocolType; }
            set
            {
                if (value.HasValue)
                {
                    _protocolType = value;
                }
                else
                {
                    _protocolType = null;
                }
            }
        }
        private SecurityProtocolType? _protocolType;

        /// <summary>
        /// The timeout (in milliseconds) for any call to Chargify. The default is 180000
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
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
                if (string.IsNullOrEmpty(apiKey)) result = false;
                if (string.IsNullOrEmpty(Password)) result = false;
                if (string.IsNullOrEmpty(URL)) result = false;
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
        private HttpWebResponse _lastResponse;

        #endregion

        #region Metadata
        /// <summary>
        /// Allows you to set a group of metadata for a specific resource
        /// </summary>
        /// <typeparam name="T">The type of resource. Currently either Subscription or Customer</typeparam>
        /// <param name="chargifyId">The Chargify identifier for the resource</param>
        /// <param name="metadatum">The list of metadatum to set</param>
        /// <returns>The metadata result containing the response</returns>
        public List<IMetadata> SetMetadataFor<T>(int chargifyId, List<Metadata> metadatum)
        {
            // make sure data is valid
            if (metadatum == null) { throw new ArgumentNullException("metadatum"); }
            if (metadatum.Count <= 0) { throw new ArgumentOutOfRangeException("metadatum"); }
            //if (metadatum.Select(m => m.ResourceID < 0).Count() > 0) { throw new ArgumentOutOfRangeException("Metadata.ResourceID"); }
            //if (metadatum.Select(m => string.IsNullOrEmpty(m.Name)).Count() > 0) { throw new ArgumentNullException("Metadata.Name"); }
            //if (metadatum.Select(m => m.Value == null).Count() > 0) { throw new ArgumentNullException("Metadata.Value"); }

            // create XML for creation of metadata
            var metadataXml = new StringBuilder(GetXmlStringIfApplicable());
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
                    metadataXml.AppendFormat("<resource-id>{0}</resource-id>", chargifyId);
                }
                metadataXml.AppendFormat("<name>{0}</name>", metadata.Name);
                metadataXml.AppendFormat("<value>{0}</value>", metadata.Value);
                metadataXml.Append("</metadatum>");
            }
            metadataXml.Append("</metadata>");

            string url;
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    url = $"customers/{chargifyId}/metadata.{GetMethodExtension()}";
                    break;
                case "subscription":
                    url = $"subscriptions/{chargifyId}/metadata.{GetMethodExtension()}";
                    break;
                default:
                    throw new Exception($"Must be of type '{string.Join(", ", _metadataTypes.ToArray())}'");
            }

            // now make the request
            string response = DoRequest(url, HttpRequestMethod.Post, metadataXml.ToString());

            var retVal = new List<IMetadata>();

            // now build the object based on response as XML
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response); // get the XML into an XML document
            if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
            // loop through the child nodes of this node
            foreach (XmlNode parentNode in doc.ChildNodes)
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
        /// <param name="chargifyId">The Chargify identifier for the resource</param>
        /// <param name="metadata">The list of metadata to set</param>
        /// <returns>The metadata result containing the response</returns>
        public List<IMetadata> SetMetadataFor<T>(int chargifyId, Metadata metadata)
        {
            // make sure data is valid
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            //if (chargifyID < 0 || metadata.ResourceID < 0) throw new ArgumentOutOfRangeException("Metadata.ResourceID");
            if (string.IsNullOrEmpty(metadata.Name)) throw new ArgumentNullException(nameof(metadata), "Metadata.Name");
            if (metadata.Value == null) throw new ArgumentNullException(nameof(metadata), "Metadata.Value");

            // create XML for creation of metadata
            var metadataXml = new StringBuilder(GetXmlStringIfApplicable());
            metadataXml.Append("<metadata>");
            if (metadata.ResourceID > 0)
            {
                metadataXml.AppendFormat("<resource-id>{0}</resource-id>", metadata.ResourceID);
            }
            else
            {
                metadataXml.AppendFormat("<resource-id>{0}</resource-id>", chargifyId);
            }
            metadataXml.AppendFormat("<name>{0}</name>", metadata.Name);
            metadataXml.AppendFormat("<value>{0}</value>", metadata.Value);
            metadataXml.Append("</metadata>");

            string url;
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    url = $"customers/{chargifyId}/metadata.{GetMethodExtension()}";
                    break;
                case "subscription":
                    url = $"subscriptions/{chargifyId}/metadata.{GetMethodExtension()}";
                    break;
                default:
                    throw new Exception($"Must be of type '{string.Join(", ", _metadataTypes.ToArray())}'");
            }

            // now make the request
            string response = DoRequest(url, HttpRequestMethod.Post, metadataXml.ToString());

            var retVal = new List<IMetadata>();

            // now build the object based on response as XML
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response); // get the XML into an XML document
            if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
            // loop through the child nodes of this node
            foreach (XmlNode parentNode in doc.ChildNodes)
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
        /// <param name="resourceId">The Chargify identifier for the resource</param>
        /// <param name="page">Which page to return</param>
        /// <returns>The metadata result containing the response</returns>
        public IMetadataResult GetMetadataFor<T>(int resourceId, int? page)
        {
            string url;
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    url = string.Format("customers/{0}/metadata.{1}", resourceId, GetMethodExtension());
                    break;
                case "subscription":
                    url = string.Format("subscriptions/{0}/metadata.{1}", resourceId, GetMethodExtension());
                    break;
                default:
                    throw new Exception(string.Format("Must be of type '{0}'", string.Join(", ", _metadataTypes.ToArray())));
            }

            string qs = string.Empty;

            // Add the transaction options to the query string ...
            if (page.HasValue && page.Value != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", page); }

            // Construct the url to access Chargify
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = DoRequest(url);

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
            string response;
            switch (typeof(T).Name.ToLowerInvariant())
            {
                case "customer":
                    response = DoRequest(string.Format("customers/metadata.{0}", GetMethodExtension()), HttpRequestMethod.Get, null);
                    break;
                case "subscription":
                    response = DoRequest(string.Format("subscriptions/metadata.{0}", GetMethodExtension()), HttpRequestMethod.Get, null);
                    break;
                default:
                    throw new Exception(string.Format("Must be of type '{0}'", string.Join(", ", _metadataTypes.ToArray())));
            }
            // change the response to the object
            return response.ConvertResponseTo<MetadataResult>("metadata");
        }
        private static List<string> _metadataTypes = new List<string> { "Customer", "Subscription" };
        #endregion

        #region Customers

        /// <summary>
        /// Load the requested customer from chargify
        /// </summary>
        /// <param name="chargifyId">The chargify ID of the customer</param>
        /// <returns>The customer with the specified chargify ID</returns>
        public ICustomer LoadCustomer(int chargifyId)
        {
            try
            {
                // make sure data is valid
                if (chargifyId == int.MinValue) throw new ArgumentNullException("chargifyId");
                // now make the request
                string response = DoRequest(string.Format("customers/{0}.{1}", chargifyId, GetMethodExtension()));
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
        /// <param name="systemId">The system ID of the customer</param>
        /// <returns>The customer with the specified chargify ID</returns>
        public ICustomer LoadCustomer(string systemId)
        {
            try
            {
                // make sure data is valid
                if (systemId == string.Empty) throw new ArgumentException("Empty SystemID not allowed", "systemId");
                // now make the request
                string response = DoRequest(string.Format("customers/lookup.{0}?reference={1}", GetMethodExtension(), systemId));
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
        /// <param name="customer">
        /// A customer object containing customer attributes.  The customer cannot be an existing saved chargify customer
        /// </param>
        /// <returns>The created chargify customer</returns>
        public ICustomer CreateCustomer(ICustomer customer)
        {
            // make sure data is valid
            if (customer == null) throw new ArgumentNullException("customer");
            if (customer.IsSaved) throw new ArgumentException("Customer already saved", "customer");
            return CreateCustomer(customer.FirstName, customer.LastName, customer.Email, customer.Phone, customer.Organization, customer.SystemID,
                                  customer.ShippingAddress, customer.ShippingAddress2, customer.ShippingCity, customer.ShippingState,
                                  customer.ShippingZip, customer.ShippingCountry);
        }

        /// <summary>
        /// Create a new chargify customer
        /// </summary>
        /// <param name="firstName">The first name of the customer</param>
        /// <param name="lastName">The last name of the customer</param>
        /// <param name="emailAddress">The email address of the customer</param>
        /// <param name="phone">The phone number of the customer</param>
        /// <param name="organization">The organization of the customer</param>
        /// <param name="systemId">The system ID of the customer</param>
        /// <param name="shippingAddress">The shipping address of the customer, if applicable.</param>
        /// <param name="shippingAddress2">The shipping address (line 2) of the customer, if applicable.</param>
        /// <param name="shippingCity">The shipping city of the customer, if applicable.</param>
        /// <param name="shippingState">The shipping state of the customer, if applicable.</param>
        /// <param name="shippingZip">The shipping zip of the customer, if applicable.</param>
        /// <param name="shippingCountry">The shipping country of the customer, if applicable.</param>
        /// <returns>The created chargify customer</returns>
        public ICustomer CreateCustomer(string firstName, string lastName, string emailAddress, string phone, string organization, string systemId,
                                        string shippingAddress, string shippingAddress2, string shippingCity, string shippingState,
                                        string shippingZip, string shippingCountry)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException(nameof(firstName));
#if !DEBUG
            if (string.IsNullOrEmpty(lastName)) throw new ArgumentNullException(nameof(lastName));
            if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException(nameof(emailAddress));
            if (systemId == string.Empty) throw new ArgumentException("Empty systemId not allowed", nameof(systemId));
            // make sure that the system ID is unique
            if (this.LoadCustomer(systemId) != null) throw new ArgumentException("Not unique", nameof(systemId));
#endif
            // create XML for creation of customer
            var customerXml = new StringBuilder(GetXmlStringIfApplicable());
            customerXml.Append("<customer>");
            if (!string.IsNullOrEmpty(emailAddress)) customerXml.AppendFormat("<email>{0}</email>", emailAddress);
            if (!string.IsNullOrEmpty(phone)) customerXml.AppendFormat("<{0}>{1}</{2}>", CustomerAttributes.PhoneKey, phone, CustomerAttributes.PhoneKey);
            if (!string.IsNullOrEmpty(firstName)) customerXml.AppendFormat("<first_name>{0}</first_name>", firstName);
            if (!string.IsNullOrEmpty(lastName)) customerXml.AppendFormat("<last_name>{0}</last_name>", lastName);
            if (!string.IsNullOrEmpty(organization)) customerXml.AppendFormat("<organization>{0}</organization>", HttpUtility.HtmlEncode(organization));
            if (!string.IsNullOrEmpty(systemId)) customerXml.AppendFormat("<reference>{0}</reference>", systemId);
            if (!string.IsNullOrEmpty(shippingAddress)) customerXml.AppendFormat("<address>{0}</address>", shippingAddress);
            if (!string.IsNullOrEmpty(shippingAddress2)) customerXml.AppendFormat("<address_2>{0}</address_2>", shippingAddress2);
            if (!string.IsNullOrEmpty(shippingCity)) customerXml.AppendFormat("<city>{0}</city>", shippingCity);
            if (!string.IsNullOrEmpty(shippingState)) customerXml.AppendFormat("<state>{0}</state>", shippingState);
            if (!string.IsNullOrEmpty(shippingZip)) customerXml.AppendFormat("<zip>{0}</zip>", shippingZip);
            if (!string.IsNullOrEmpty(shippingCountry)) customerXml.AppendFormat("<country>{0}</country>", shippingCountry);
            customerXml.Append("</customer>");
            // now make the request
            string response = DoRequest(string.Format("customers.{0}", GetMethodExtension()), HttpRequestMethod.Post, customerXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Customer>("customer");
        }

        /// <summary>
        /// Create a new chargify customer
        /// </summary>
        /// <param name="firstName">The first name of the customer</param>
        /// <param name="lastName">The last name of the customer</param>
        /// <param name="emailAddress">The email address of the customer</param>
        /// <param name="phone">The phone number of the customer</param>
        /// <param name="organization">The organization of the customer</param>
        /// <param name="systemId">The system ID fro the customer</param>
        /// <returns>The created chargify customer</returns>
        public ICustomer CreateCustomer(string firstName, string lastName, string emailAddress, string phone, string organization, string systemId)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException("firstName");
            if (string.IsNullOrEmpty(lastName)) throw new ArgumentNullException("lastName");
            if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException("emailAddress");
            if (systemId == string.Empty) throw new ArgumentException("Empty SystemID not allowed", "systemId");
            // make sure that the system ID is unique
            if (LoadCustomer(systemId) != null) throw new ArgumentException("Not unique", "systemId");
            // create XML for creation of customer
            var customerXml = new StringBuilder(GetXmlStringIfApplicable());
            customerXml.Append("<customer>");
            customerXml.AppendFormat("<email>{0}</email>", emailAddress);
            customerXml.AppendFormat("<first_name>{0}</first_name>", firstName);
            customerXml.AppendFormat("<last_name>{0}</last_name>", lastName);
            if (!string.IsNullOrEmpty(phone)) customerXml.AppendFormat("<{0}>{1}</{2}>", CustomerAttributes.PhoneKey, phone, CustomerAttributes.PhoneKey);
            if (!string.IsNullOrEmpty(organization)) customerXml.AppendFormat("<organization>{0}</organization>", HttpUtility.HtmlEncode(organization));
            if (!string.IsNullOrEmpty(systemId)) customerXml.AppendFormat("<reference>{0}</reference>", systemId);
            customerXml.Append("</customer>");
            // now make the request
            string response = DoRequest(string.Format("customers.{0}", GetMethodExtension()), HttpRequestMethod.Post, customerXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Customer>("customer");
        }

        /// <summary>
        /// Update the specified chargify customer
        /// </summary>
        /// <param name="customer">The customer to update</param>
        /// <returns>The updated customer</returns>
        public ICustomer UpdateCustomer(ICustomer customer)
        {
            // make sure data is OK
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            if (customer.ChargifyID == int.MinValue) throw new ArgumentException("Invalid chargify ID detected", nameof(customer));
            ICustomer oldCust = LoadCustomer(customer.ChargifyID);

            bool isUpdateRequired = false;

            // create XML for creation of customer
            var customerXml = new StringBuilder(GetXmlStringIfApplicable());
            customerXml.Append("<customer>");
            if (oldCust != null)
            {
                if (oldCust.ChargifyID != customer.ChargifyID) throw new ArgumentException("System ID is not unique", nameof(customer));
                if (oldCust.FirstName != customer.FirstName) { customerXml.AppendFormat("<first_name>{0}</first_name>", HttpUtility.HtmlEncode(customer.FirstName)); isUpdateRequired = true; }
                if (oldCust.LastName != customer.LastName) { customerXml.AppendFormat("<last_name>{0}</last_name>", HttpUtility.HtmlEncode(customer.LastName)); isUpdateRequired = true; }
                if (oldCust.Email != customer.Email) { customerXml.AppendFormat("<email>{0}</email>", customer.Email); isUpdateRequired = true; }
                if (oldCust.Organization != customer.Organization) { customerXml.AppendFormat("<organization>{0}</organization>", HttpUtility.HtmlEncode(customer.Organization)); isUpdateRequired = true; }
                if (oldCust.Phone != customer.Phone) { customerXml.AppendFormat("<phone>{0}</phone>", HttpUtility.HtmlEncode(customer.Phone)); isUpdateRequired = true; }
                if (oldCust.SystemID != customer.SystemID) { customerXml.AppendFormat("<reference>{0}</reference>", customer.SystemID); isUpdateRequired = true; }
                if (oldCust.ShippingAddress != customer.ShippingAddress) { customerXml.AppendFormat("<address>{0}</address>", HttpUtility.HtmlEncode(customer.ShippingAddress)); isUpdateRequired = true; }
                if (oldCust.ShippingAddress2 != customer.ShippingAddress2) { customerXml.AppendFormat("<address_2>{0}</address_2>", HttpUtility.HtmlEncode(customer.ShippingAddress2)); isUpdateRequired = true; }
                if (oldCust.ShippingCity != customer.ShippingCity) { customerXml.AppendFormat("<city>{0}</city>", HttpUtility.HtmlEncode(customer.ShippingCity)); isUpdateRequired = true; }
                if (oldCust.ShippingState != customer.ShippingState) { customerXml.AppendFormat("<state>{0}</state>", HttpUtility.HtmlEncode(customer.ShippingState)); isUpdateRequired = true; }
                if (oldCust.ShippingZip != customer.ShippingZip) { customerXml.AppendFormat("<zip>{0}</zip>", customer.ShippingZip); isUpdateRequired = true; }
                if (oldCust.ShippingCountry != customer.ShippingCountry) { customerXml.AppendFormat("<country>{0}</country>", HttpUtility.HtmlEncode(customer.ShippingCountry)); isUpdateRequired = true; }
            }
            customerXml.Append("</customer>");

            if (isUpdateRequired)
            {
                try
                {
                    // now make the request
                    string response = DoRequest(string.Format("customers/{0}.{1}", customer.ChargifyID, GetMethodExtension()), HttpRequestMethod.Put, customerXml.ToString());
                    // change the response to the object
                    return response.ConvertResponseTo<Customer>("customer");
                }
                catch (ChargifyException cex)
                {
                    if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Customer not found");
                    throw;
                }
            }
            else
            {
                return customer;
            }
        }

        /// <summary>
        /// Get a list of customers (will return 50 for each page)
        /// </summary>
        /// <param name="pageNumber">The page number to load</param>
        /// <returns>A list of customers for the specified page</returns>
        public IDictionary<string, ICustomer> GetCustomerList(int pageNumber)
        {
            return GetCustomerList(pageNumber, false);
        }

        /// <summary>
        /// Get a list of customers (will return 50 for each page)
        /// </summary>
        /// <param name="pageNumber">The page number to load</param>
        /// <param name="keyByChargifyId">If true, the dictionary will be keyed by Chargify ID and not the reference value.</param>
        /// <returns>A list of customers for the specified page</returns>
        public IDictionary<string, ICustomer> GetCustomerList(int pageNumber, bool keyByChargifyId)
        {
            // make sure data is valid
            if (pageNumber < 1) throw new ArgumentException("Page number must be greater than 1", "pageNumber");
            // now make the request
            string response = DoRequest(string.Format("customers.{0}?page={1}", GetMethodExtension(), pageNumber));
            var retValue = new Dictionary<string, ICustomer>();
            if (response.IsXml())
            {
                // now build customer object based on response as XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response); // get the XML into an XML document
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "customers")
                    {
                        foreach (XmlNode customerNode in elementNode.ChildNodes)
                        {
                            if (customerNode.Name == "customer")
                            {
                                ICustomer loadedCustomer = new Customer(customerNode);
                                string key = keyByChargifyId ? loadedCustomer.ChargifyID.ToString() : loadedCustomer.SystemID;
                                if (!retValue.ContainsKey(key))
                                {
                                    retValue.Add(key, loadedCustomer);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("customer"))
                    {
                        JsonObject customerObj = (array.Items[i] as JsonObject)["customer"] as JsonObject;
                        ICustomer loadedCustomer = new Customer(customerObj);
                        string key = keyByChargifyId ? loadedCustomer.ChargifyID.ToString() : loadedCustomer.SystemID;
                        if (!retValue.ContainsKey(key))
                        {
                            retValue.Add(key, loadedCustomer);
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
        /// <param name="keyByChargifyId">If true, the key will be the ChargifyID, otherwise it will be the reference value</param>
        /// <returns>A list of customers</returns>
        public IDictionary<string, ICustomer> GetCustomerList(bool keyByChargifyId)
        {
            var retValue = new Dictionary<string, ICustomer>();
            int pageCount = 1000;
            for (int page = 1; pageCount > 0; page++)
            {
                IDictionary<string, ICustomer> pageList = GetCustomerList(page, keyByChargifyId);
                foreach (ICustomer cust in pageList.Values)
                {
                    string key = keyByChargifyId ? cust.ChargifyID.ToString() : cust.SystemID;
                    if (!retValue.ContainsKey(key))
                    {
                        retValue.Add(key, cust);
                    }
                    else
                    {
                        //throw new InvalidOperationException("Duplicate key values detected");
                    }
                }
                pageCount = pageList.Count;
            }
            return retValue;
        }

        /// <summary>
        /// Delete the specified customer
        /// </summary>
        /// <param name="chargifyId">The integer identifier of the customer</param>
        /// <returns>True if the customer was deleted, false otherwise.</returns>
        /// <remarks>This method does not currently work, but it will once they open up the API. This will always return false, as Chargify will send a Http Forbidden everytime.</remarks>
        public bool DeleteCustomer(int chargifyId)
        {
            try
            {
                // make sure data is valid
                if (chargifyId < 0) throw new ArgumentNullException("chargifyId");

                // now make the request
                DoRequest(string.Format("customers/{0}.{1}", chargifyId, GetMethodExtension()), HttpRequestMethod.Delete, string.Empty);
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
        /// <param name="systemId">The system identifier of the customer.</param>
        /// <returns>True if the customer was deleted, false otherwise.</returns>
        /// <remarks>This method does not currently work, but it will once they open up the API. This will always return false, as Chargify will send a Http Forbidden everytime.</remarks>
        public bool DeleteCustomer(string systemId)
        {
            try
            {
                // make sure data is valid
                if (systemId == string.Empty) throw new ArgumentException("Empty SystemID not allowed", "systemId");

                ICustomer customer = LoadCustomer(systemId);
                if (customer == null) { throw new ArgumentException("Not a known customer", "systemId"); }

                // now make the request
                DoRequest(string.Format("customers/{0}.{1}", customer.ChargifyID, GetMethodExtension()), HttpRequestMethod.Delete, string.Empty);
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
        /// Method that updates a product
        /// </summary>
        /// <param name="productId">The ID of the product to update</param>
        /// <param name="updatedProduct">The details of the updated product</param>
        /// <returns>The updated product</returns>
        public IProduct UpdateProduct(int productId, IProduct updatedProduct)
        {
            var existingProduct = LoadProduct(productId.ToString(), false);
            if (existingProduct == null) throw new ArgumentException(string.Format("No product with ID {0} exists.", productId));
            if (updatedProduct == null) throw new ArgumentNullException(nameof(updatedProduct));
            if (updatedProduct.ProductFamily.ID <= 0) throw new ArgumentOutOfRangeException(nameof(updatedProduct), "Product's ProductFamily-> ID must be > 0");
            if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(updatedProduct), "ProductID is not valid");

            var productXml = new StringBuilder(GetXmlStringIfApplicable());
            productXml.Append("<product>");
            if (!string.IsNullOrWhiteSpace(updatedProduct.Name) && existingProduct.Name != updatedProduct.Name) productXml.AppendFormat("<name>{0}</name>", HttpUtility.HtmlEncode(updatedProduct.Name));
            if (updatedProduct.PriceInCents != int.MinValue && existingProduct.PriceInCents != updatedProduct.PriceInCents) productXml.AppendFormat("<price_in_cents>{0}</price_in_cents>", updatedProduct.PriceInCents);
            if (updatedProduct.Interval != int.MinValue && existingProduct.Interval != updatedProduct.Interval) productXml.AppendFormat("<interval>{0}</interval>", updatedProduct.Interval);
            var intervalUnit = Enum.GetName(typeof(IntervalUnit), updatedProduct.IntervalUnit);
            if (intervalUnit != null) productXml.AppendFormat("<interval_unit>{0}</interval_unit>", intervalUnit.ToLowerInvariant());
            if (!string.IsNullOrEmpty(updatedProduct.Handle) && existingProduct.Handle != updatedProduct.Handle) productXml.AppendFormat("<handle>{0}</handle>", updatedProduct.Handle);
            if (!string.IsNullOrEmpty(updatedProduct.AccountingCode) && existingProduct.AccountingCode != updatedProduct.AccountingCode) productXml.AppendFormat("<accounting_code>{0}</accounting_code>", updatedProduct.AccountingCode);
            if (!string.IsNullOrEmpty(updatedProduct.Description) && existingProduct.Description != updatedProduct.Description) productXml.AppendFormat("<description>{0}</description>", HttpUtility.HtmlEncode(updatedProduct.Description));
            productXml.Append("</product>");

            string response = DoRequest(string.Format("products/{0}.{1}", productId, GetMethodExtension()), HttpRequestMethod.Put, productXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Product>("product");
        }

        /// <summary>
        /// Method to create a new product and add it to the site
        /// </summary>
        /// <param name="productFamilyId">The product family ID, required for adding products</param>
        /// <param name="newProduct">The new product details</param>
        /// <returns>The completed product information</returns>
        /// <remarks>This is largely undocumented currently, especially the fact that you need the product family ID</remarks>
        public IProduct CreateProduct(int productFamilyId, IProduct newProduct)
        {
            if (newProduct == null) throw new ArgumentNullException("newProduct");
            return CreateProduct(productFamilyId, newProduct.Name, newProduct.Handle, newProduct.PriceInCents, newProduct.Interval, newProduct.IntervalUnit, newProduct.AccountingCode, newProduct.Description);
        }

        /// <summary>
        /// Allows the creation of a product
        /// </summary>
        /// <param name="productFamilyId">The family to which this product belongs</param>
        /// <param name="name">The name of the product</param>
        /// <param name="handle">The handle to be used for this product</param>
        /// <param name="priceInCents">The price (in cents)</param>
        /// <param name="interval">The time interval used to determine the recurring nature of this product</param>
        /// <param name="intervalUnit">Either days, or months</param>
        /// <param name="accountingCode">The accounting code used for this product</param>
        /// <param name="description">The product description</param>
        /// <returns>The created product</returns>
        public IProduct CreateProduct(int productFamilyId, string name, string handle, int priceInCents, int interval, IntervalUnit intervalUnit, string accountingCode, string description)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            // create XML for creation of the new product
            var productXml = new StringBuilder(GetXmlStringIfApplicable());
            productXml.Append("<product>");
            productXml.AppendFormat("<name>{0}</name>", HttpUtility.HtmlEncode(name));
            productXml.AppendFormat("<price_in_cents>{0}</price_in_cents>", priceInCents);
            productXml.AppendFormat("<interval>{0}</interval>", interval);
            var intervalUnitName = Enum.GetName(typeof(IntervalUnit), intervalUnit);
            if (intervalUnitName != null)
                productXml.AppendFormat("<interval_unit>{0}</interval_unit>", intervalUnitName.ToLowerInvariant());
            if (!string.IsNullOrEmpty(handle)) productXml.AppendFormat("<handle>{0}</handle>", handle);
            if (!string.IsNullOrEmpty(accountingCode)) productXml.AppendFormat("<accounting_code>{0}</accounting_code>", accountingCode);
            if (!string.IsNullOrEmpty(description)) productXml.AppendFormat("<description>{0}</description>", HttpUtility.HtmlEncode(description));
            productXml.Append("</product>");
            // now make the request
            string response = DoRequest(string.Format("product_families/{0}/products.{1}", productFamilyId, GetMethodExtension()), HttpRequestMethod.Post, productXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Product>("product");
        }

        /// <summary>
        /// Load the requested product from chargify by its handle
        /// </summary>
        /// <param name="handle">The Chargify ID or handle of the product</param>
        /// <returns>The product with the specified chargify ID</returns>
        public IProduct LoadProduct(string handle)
        {
            return LoadProduct(handle, true);
        }

        /// <summary>
        /// Load the requested product from chargify
        /// </summary>
        /// <param name="productId">The Chargify ID or handle of the product</param>
        /// <param name="isHandle">If true, then the ProductID represents the handle, if false the ProductID represents the Chargify ID</param>
        /// <returns>The product with the specified chargify ID</returns>
        public IProduct LoadProduct(string productId, bool isHandle)
        {
            try
            {
                // make sure data is valid
                if (string.IsNullOrEmpty(productId)) throw new ArgumentNullException("productId");
                // now make the request
                string response;
                if (isHandle)
                {
                    response = DoRequest(string.Format("products/handle/{0}.{1}", productId, GetMethodExtension()));
                }
                else
                {
                    response = DoRequest(string.Format("products/{0}.{1}", productId, GetMethodExtension()));
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
            string response = DoRequest(string.Format("products.{0}", GetMethodExtension()));
            // loop through the child nodes of this node
            var retValue = new Dictionary<int, IProduct>();
            if (response.IsXml())
            {
                // now build a product list based on response XML
                // get the XML into an XML document
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "products")
                    {
                        foreach (XmlNode productNode in elementNode.ChildNodes)
                        {
                            if (productNode.Name == "product")
                            {
                                IProduct loadedProduct = new Product(productNode);
                                if (!retValue.ContainsKey(loadedProduct.ID))
                                {
                                    retValue.Add(loadedProduct.ID, loadedProduct);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("product"))
                    {
                        JsonObject productObj = (array.Items[i] as JsonObject)["product"] as JsonObject;
                        IProduct loadedProduct = new Product(productObj);
                        if (!retValue.ContainsKey(loadedProduct.ID))
                        {
                            retValue.Add(loadedProduct.ID, loadedProduct);
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
            if (newFamily == null) throw new ArgumentNullException(nameof(newFamily));
            if (string.IsNullOrEmpty(newFamily.Name)) throw new ArgumentNullException(nameof(newFamily), "The product family name is blank, but required.");
            // create XML for creation of the new product family
            var productFamilyXml = new StringBuilder(GetXmlStringIfApplicable());
            productFamilyXml.Append("<product_family>");
            productFamilyXml.AppendFormat("<name>{0}</name>", HttpUtility.HtmlEncode(newFamily.Name));
            if (!string.IsNullOrEmpty(newFamily.Handle)) productFamilyXml.AppendFormat("<handle>{0}</handle>", newFamily.Handle);
            if (!string.IsNullOrEmpty(newFamily.AccountingCode)) productFamilyXml.AppendFormat("<accounting_code>{0}</accounting_code>", HttpUtility.HtmlEncode(newFamily.AccountingCode));
            if (!string.IsNullOrEmpty(newFamily.Description)) productFamilyXml.AppendFormat("<description>{0}</description>", HttpUtility.HtmlEncode(newFamily.Description));
            productFamilyXml.Append("</product_family>");
            // now make the request
            string response = DoRequest(string.Format("product_families.{0}", GetMethodExtension()), HttpRequestMethod.Post, productFamilyXml.ToString());
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
            string response = DoRequest(string.Format("product_families.{0}", GetMethodExtension()));
            // loop through the child nodes of this node
            var retValue = new Dictionary<int, IProductFamily>();
            if (response.IsXml())
            {
                // now build a product family list based on response XML
                // get the XML into an XML document
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "product_families")
                    {
                        foreach (XmlNode productFamilyNode in elementNode.ChildNodes)
                        {
                            if (productFamilyNode.Name == "product_family")
                            {
                                IProductFamily loadedProductFamily = new ProductFamily(productFamilyNode);
                                if (!retValue.ContainsKey(loadedProductFamily.ID))
                                {
                                    retValue.Add(loadedProductFamily.ID, loadedProductFamily);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("product_family"))
                    {
                        JsonObject productFamilyObj = (array.Items[i] as JsonObject)["product_family"] as JsonObject;
                        IProductFamily loadedProductFamily = new ProductFamily(productFamilyObj);
                        if (!retValue.ContainsKey(loadedProductFamily.ID))
                        {
                            retValue.Add(loadedProductFamily.ID, loadedProductFamily);
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
        /// <param name="handle">The Chargify ID or handle of the product</param>
        /// <returns>The product family with the specified chargify ID</returns>
        public IProductFamily LoadProductFamily(string handle)
        {
            return LoadProductFamily(handle, true);
        }

        /// <summary>
        /// Load the requested product family from chargify by its handle
        /// </summary>
        /// <param name="id">The Chargify ID of the product</param>
        /// <returns>The product family with the specified chargify ID</returns>
        public IProductFamily LoadProductFamily(int id)
        {
            return LoadProductFamily(id.ToString(), false);
        }

        /// <summary>
        /// Load the requested product family from chargify
        /// </summary>
        /// <param name="productFamilyIdentifier">The Chargify identifier (ID or handle) of the product family</param>
        /// <param name="isHandle">If true, then the ProductID represents the handle, if false the ProductFamilyID represents the Chargify ID</param>
        /// <returns>The product family with the specified chargify ID</returns>
        private IProductFamily LoadProductFamily(string productFamilyIdentifier, bool isHandle)
        {
            try
            {
                // make sure data is valid
                if (string.IsNullOrEmpty(productFamilyIdentifier)) throw new ArgumentNullException(nameof(productFamilyIdentifier));
                // now make the request
                string response;
                if (isHandle)
                {
                    response = DoRequest(string.Format("product_families/lookup.{0}?handle={1}", GetMethodExtension(), productFamilyIdentifier));
                }
                else
                {
                    response = DoRequest(string.Format("product_families/{0}.{1}", productFamilyIdentifier, GetMethodExtension()));
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
        /// <param name="firstName">The first name of the customer to add to the pretty url</param>
        /// <param name="lastName">The last name of the customer to add to the pretty url</param>
        /// <param name="subscriptionId">The ID of the subscription to update</param>
        /// <returns>The secure url of the update page</returns>
        public string GetPrettySubscriptionUpdateURL(string firstName, string lastName, int subscriptionId)
        {
            if (string.IsNullOrEmpty(SharedKey)) throw new ArgumentException("SharedKey is required to generate the hosted page url");

            string message = UpdateShortName + "--" + subscriptionId + "--" + SharedKey;
            string token = message.GetChargifyHostedToken();
            string prettyId = string.Format("{0}-{1}-{2}", subscriptionId, firstName.Trim().ToLower(), lastName.Trim().ToLower());
            string methodString = string.Format("{0}/{1}/{2}", UpdateShortName, prettyId, token);
            // just in case?
            methodString = HttpUtility.UrlEncode(methodString);
            string updateUrl = string.Format("{0}{1}{2}", URL, (URL.EndsWith("/") ? "" : "/"), methodString);
            return updateUrl;
        }

        /// <summary>
        /// Method to get the secure URL for updating the payment details for a subscription.
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to update</param>
        /// <returns>The secure url of the update page</returns>
        public string GetSubscriptionUpdateURL(int subscriptionId)
        {
            if (string.IsNullOrEmpty(SharedKey)) throw new ArgumentException("SharedKey is required to generate the hosted page url");

            string message = UpdateShortName + "--" + subscriptionId + "--" + SharedKey;
            string token = message.GetChargifyHostedToken();
            string methodString = string.Format("{0}/{1}/{2}", UpdateShortName, subscriptionId, token);
            methodString = HttpUtility.UrlEncode(methodString);
            string updateUrl = string.Format("{0}{1}{2}", URL, (URL.EndsWith("/") ? "" : "/"), methodString);
            return updateUrl;
        }

        /// <summary>
        /// Chargify offers the ability to reactivate a previously canceled subscription. For details
        /// on how reactivation works, and how to reactivate subscriptions through the Admin interface, see
        /// http://support.chargify.com/faqs/features/reactivation
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to reactivate</param>
        /// <returns>The newly reactivated subscription, or nothing.</returns>
        public ISubscription ReactivateSubscription(int subscriptionId)
        {
            try
            {
                return ReactivateSubscription(subscriptionId, false);
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
        /// <param name="subscriptionId">The ID of the subscription to reactivate</param>
        /// <param name="includeTrial">If true, the reactivated subscription will include a trial if one is available.</param>
        /// <returns>The newly reactivated subscription, or nothing.</returns>
        public ISubscription ReactivateSubscription(int subscriptionId, bool includeTrial)
        {
            return ReactivateSubscription(subscriptionId, includeTrial, null, null);
        }

        /// <summary>
        /// Chargify offers the ability to reactivate a previously canceled subscription. For details
        /// on how reactivation works, and how to reactivate subscriptions through the Admin interface, see
        /// http://support.chargify.com/faqs/features/reactivation
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to reactivate</param>
        /// <param name="includeTrial">If true, the reactivated subscription will include a trial if one is available.</param>
        /// <param name="preserveBalance">If true, the existing subscription balance will NOT be cleared/reset before adding the additional reactivation charges.</param>
        /// <param name="couponCode">The coupon code to be applied during reactivation.</param>
        /// <returns>The newly reactivated subscription, or nothing.</returns>
        public ISubscription ReactivateSubscription(int subscriptionId, bool includeTrial, bool? preserveBalance, string couponCode)
        {
            try
            {
                // make sure data is valid
                if (subscriptionId < 0) throw new ArgumentNullException("subscriptionId");
                string requestString = string.Format("subscriptions/{0}/reactivate.{1}", subscriptionId, GetMethodExtension());

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
                if (queryString.Length > 0) requestString += "?" + queryString;

                // now make the request
                string response = DoRequest(requestString, HttpRequestMethod.Put, string.Empty);
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
        /// <param name="subscriptionId">The ID of the sucscription</param>
        /// <param name="cancellationMessage">The message to associate with the subscription</param>
        /// <returns></returns>
        public bool DeleteSubscription(int subscriptionId, string cancellationMessage)
        {
            try
            {
                // make sure data is valid
                if (subscriptionId < 0) throw new ArgumentNullException("subscriptionId");

                StringBuilder subscriptionXml = new StringBuilder("");
                if (!string.IsNullOrEmpty(cancellationMessage))
                {
                    // create XML for creation of customer
                    subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
                    subscriptionXml.Append("<subscription>");
                    subscriptionXml.AppendFormat("<cancellation_message>{0}</cancellation_message>", cancellationMessage);
                    subscriptionXml.Append("</subscription>");
                }
                // now make the request
                DoRequest(string.Format("subscriptions/{0}.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Delete, subscriptionXml.ToString());
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
        /// <param name="subscriptionId">The ID of the subscription</param>
        /// <returns>The subscription with the specified ID</returns>
        public ISubscription LoadSubscription(int subscriptionId)
        {
            try
            {
                // make sure data is valid
                if (subscriptionId < 0) throw new ArgumentNullException("subscriptionId");
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}.{1}", subscriptionId, GetMethodExtension()));
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
            string response = DoRequest(url);

            var retValue = new Dictionary<int, ISubscription>();
            if (response.IsXml())
            {
                // now build a transaction list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "subscriptions")
                    {
                        foreach (XmlNode subscriptionNode in elementNode.ChildNodes)
                        {
                            if (subscriptionNode.Name == "subscription")
                            {
                                ISubscription loadedSubscription = new Subscription(subscriptionNode);
                                if (!retValue.ContainsKey(loadedSubscription.SubscriptionID))
                                {
                                    retValue.Add(loadedSubscription.SubscriptionID, loadedSubscription);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("subscription"))
                    {
                        JsonObject subscriptionObj = (array.Items[i] as JsonObject)["subscription"] as JsonObject;
                        ISubscription loadedSubscription = new Subscription(subscriptionObj);
                        if (!retValue.ContainsKey(loadedSubscription.SubscriptionID))
                        {
                            retValue.Add(loadedSubscription.SubscriptionID, loadedSubscription);
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
            int pageCount = 1000;
            for (int page = 1; pageCount > 0; page++)
            {
                IDictionary<int, ISubscription> pageList = GetSubscriptionList(page, 50);
                foreach (ISubscription subscription in pageList.Values)
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
                pageCount = pageList.Count;
            }
            return retValue;
            //return GetSubscriptionList(int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Method that returns a list of subscriptions.
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="perPage">The number of results per page (used for pagination)</param>
        /// <returns>Null if there are no results, object otherwise.</returns>
        public IDictionary<int, ISubscription> GetSubscriptionList(int page, int perPage)
        {
            string qs = string.Empty;

            if (page != int.MinValue)
            {
                if (qs.Length > 0) { qs += "&"; }
                qs += string.Format("page={0}", page);
            }

            if (perPage != int.MinValue)
            {
                if (qs.Length > 0) { qs += "&"; }
                qs += string.Format("per_page={0}", perPage);
            }

            string url = string.Format("subscriptions.{0}", GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = DoRequest(url);

            var retValue = new Dictionary<int, ISubscription>();
            if (response.IsXml())
            {
                // now build a transaction list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "subscriptions")
                    {
                        foreach (XmlNode subscriptionNode in elementNode.ChildNodes)
                        {
                            if (subscriptionNode.Name == "subscription")
                            {
                                ISubscription loadedSubscription = new Subscription(subscriptionNode);
                                if (!retValue.ContainsKey(loadedSubscription.SubscriptionID))
                                {
                                    retValue.Add(loadedSubscription.SubscriptionID, loadedSubscription);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("subscription"))
                    {
                        JsonObject subscriptionObj = (array.Items[i] as JsonObject)["subscription"] as JsonObject;
                        ISubscription loadedSubscription = new Subscription(subscriptionObj);
                        if (!retValue.ContainsKey(loadedSubscription.SubscriptionID))
                        {
                            retValue.Add(loadedSubscription.SubscriptionID, loadedSubscription);
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
        /// <param name="chargifyId">The ChargifyID of the customer</param>
        /// <returns>A list of subscriptions</returns>
        public IDictionary<int, ISubscription> GetSubscriptionListForCustomer(int chargifyId)
        {
            try
            {
                // make sure data is valid
                if (chargifyId == int.MinValue) throw new ArgumentNullException("chargifyId");
                // now make the request
                string response = DoRequest(string.Format("customers/{0}/subscriptions.{1}", chargifyId, GetMethodExtension()));
                var retValue = new Dictionary<int, ISubscription>();
                if (response.IsXml())
                {
                    // now build customer object based on response XML
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(response); // get the XML into an XML document
                    if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                    // loop through the child nodes of this node
                    foreach (XmlNode elementNode in doc.ChildNodes)
                    {
                        if (elementNode.Name == "subscriptions")
                        {
                            foreach (XmlNode subscriptionNode in elementNode.ChildNodes)
                            {
                                if (subscriptionNode.Name == "subscription")
                                {
                                    ISubscription loadedSubscription = new Subscription(subscriptionNode);
                                    if (!retValue.ContainsKey(loadedSubscription.SubscriptionID))
                                    {
                                        retValue.Add(loadedSubscription.SubscriptionID, loadedSubscription);
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
                        var jsonObject = array.Items[i] as JsonObject;
                        if (jsonObject != null && jsonObject.ContainsKey("subscription"))
                        {
                            JsonObject subscriptionObj = (array.Items[i] as JsonObject)["subscription"] as JsonObject;
                            ISubscription loadedSubscription = new Subscription(subscriptionObj);
                            if (!retValue.ContainsKey(loadedSubscription.SubscriptionID))
                            {
                                retValue.Add(loadedSubscription.SubscriptionID, loadedSubscription);
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
        /// Create a subscription
        /// </summary>
        /// <param name="options">The input options for creating a subscription</param>
        /// <returns>The subscription</returns>
        public ISubscription CreateSubscription(ISubscriptionCreateOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Customer
            bool customerSpecifiedAlready = options.CustomerID.HasValue;

            if (!string.IsNullOrEmpty(options.CustomerReference))
            {
                if (customerSpecifiedAlready) { throw new ArgumentException("Customer information should only be specified once", nameof(options)); }
                customerSpecifiedAlready = true;
            }
            if (options.CustomerAttributes != null)
            {
                if (customerSpecifiedAlready) throw new ArgumentException("Customer information should only be specified once", nameof(options));
                customerSpecifiedAlready = true;
            }
            if (!customerSpecifiedAlready) { throw new ArgumentException("No customer information was specified. Please specify either the CustomerID, CustomerReference or CustomerAttributes and try again.", "options"); }

            // Product
            bool productSpecifiedAlready = options.ProductID.HasValue;
            if (!string.IsNullOrEmpty(options.ProductHandle))
            {
                if (productSpecifiedAlready) { throw new ArgumentException("Product information should only be specified once", nameof(options)); }
                productSpecifiedAlready = true;
            }
            if (!productSpecifiedAlready) { throw new ArgumentException("No product information was specified. Please specify either the ProductID or ProductHandle and try again.", "options"); }

            var subscriptionXml = new StringBuilder();
            var serializer = new System.Xml.Serialization.XmlSerializer(options.GetType());
            using (StringWriter textWriter = new Utf8StringWriter())
            {
                serializer.Serialize(textWriter, options);
                subscriptionXml.Append(textWriter);
            }

            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription without passing credit card information.
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="chargifyId">The Chargify ID of the customer</param>
        /// <param name="paymentCollectionMethod">Optional, type of payment collection method</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string productHandle, int chargifyId, PaymentCollectionMethod? paymentCollectionMethod = PaymentCollectionMethod.Automatic)
        {
            // make sure data is valid
            if (chargifyId == int.MinValue) throw new ArgumentException("Invalid Customer ID detected", "chargifyId");
            // Create the subscription
            return CreateSubscription(productHandle, chargifyId, string.Empty, paymentCollectionMethod);
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="chargifyId">The Chargify ID of the customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string productHandle, int chargifyId, ICreditCardAttributes creditCardAttributes)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException("creditCardAttributes");
            if (chargifyId == int.MinValue) throw new ArgumentException("Invalid Customer ID detected", "chargifyId");

            return CreateSubscription(productHandle, chargifyId, creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress,
                                      creditCardAttributes.BillingCity, creditCardAttributes.BillingState, creditCardAttributes.BillingZip,
                                      creditCardAttributes.BillingCountry, string.Empty, creditCardAttributes.FirstName, creditCardAttributes.LastName);
        }

        /// <summary>
        /// Create a subscription
        /// </summary>
        /// <param name="productHandle">The handle of the product</param>
        /// <param name="chargifyId">The ID of the customer who should be used in this new subscription</param>
        /// <param name="creditCardAttributes">The credit card attributes to use for the new subscription</param>
        /// <param name="nextBillingAt">The date that should be used for the next_billing_at</param>
        /// <returns>The new subscription, if successful. Null otherwise.</returns>
        public ISubscription CreateSubscription(string productHandle, int chargifyId, ICreditCardAttributes creditCardAttributes, DateTime nextBillingAt)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException("creditCardAttributes");
            if (chargifyId == int.MinValue) throw new ArgumentException("Invalid Customer ID detected", "chargifyId");

            return CreateSubscription(new SubscriptionCreateOptions() { ProductHandle = productHandle, CustomerID = chargifyId, CreditCardAttributes = (CreditCardAttributes)creditCardAttributes, NextBillingAt = nextBillingAt });
        }

        /// <summary>
        /// Create a subscription using a coupon for discounted rate, without using credit card information.
        /// </summary>
        /// <param name="productHandle">The product to subscribe to</param>
        /// <param name="chargifyId">The ID of the Customer to add the subscription for</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <returns>If sucessful, the subscription object. Otherwise null.</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, int chargifyId, string couponCode)
        {
            if (chargifyId == int.MinValue) throw new ArgumentException("Invalid Customer ID detected", "chargifyId");
            if (string.IsNullOrEmpty(couponCode)) throw new ArgumentException("CouponCode can't be empty", "couponCode");
            return CreateSubscription(productHandle, chargifyId, couponCode, default(PaymentCollectionMethod?));
        }

        /// <summary>
        /// Create a subscription using a coupon for discounted rate
        /// </summary>
        /// <param name="productHandle">The product to subscribe to</param>
        /// <param name="chargifyId">The ID of the Customer to add the subscription for</param>
        /// <param name="creditCardAttributes">The credit card attributes to use for this transaction</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <returns></returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, int chargifyId, ICreditCardAttributes creditCardAttributes, string couponCode)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException("creditCardAttributes");
            if (chargifyId == int.MinValue) throw new ArgumentException("Invalid Customer ID detected", "chargifyId");
            if (string.IsNullOrEmpty(couponCode)) throw new ArgumentException("CouponCode can't be empty", "couponCode");

            return CreateSubscription(productHandle, chargifyId, creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress,
                                      creditCardAttributes.BillingCity, creditCardAttributes.BillingState, creditCardAttributes.BillingZip,
                                      creditCardAttributes.BillingCountry, couponCode);
        }

        /// <summary>
        /// Create a new subscription without requiring credit card information
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="systemId">The System ID of the customer</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string productHandle, string systemId)
        {
            if (systemId == string.Empty) throw new ArgumentException("Invalid system ID detected", nameof(systemId));
            return CreateSubscription(productHandle, systemId, string.Empty);
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="systemId">The System ID of the customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string productHandle, string systemId, ICreditCardAttributes creditCardAttributes)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException(nameof(creditCardAttributes));
            if (systemId == string.Empty) throw new ArgumentException("Invalid system ID detected", nameof(systemId));

            return CreateSubscription(productHandle, systemId, creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress,
                                      creditCardAttributes.BillingCity, creditCardAttributes.BillingState, creditCardAttributes.BillingZip,
                                      creditCardAttributes.BillingCountry, string.Empty);
        }

        /// <summary>
        /// Create a new subscription without passing credit card info
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="systemId">The System ID of the customer</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, string systemId, string couponCode)
        {
            // make sure data is valid            
            if (systemId == string.Empty) throw new ArgumentException("Invalid system customer ID detected", nameof(systemId));

            return CreateSubscription(productHandle, systemId, couponCode);
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="systemId">The System ID of the customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, string systemId, ICreditCardAttributes creditCardAttributes, string couponCode)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException(nameof(creditCardAttributes));
            if (systemId == string.Empty) throw new ArgumentException("Invalid system customer ID detected", nameof(systemId));

            return CreateSubscription(productHandle, systemId, creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress,
                                      creditCardAttributes.BillingCity, creditCardAttributes.BillingState, creditCardAttributes.BillingZip,
                                      creditCardAttributes.BillingCountry, couponCode);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time without submitting PaymentProfile attributes
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <returns>The xml describing the new subsscription</returns>
        /// <param name="paymentCollectionMethod">The type of subscription, recurring (automatic) billing, or invoice (if applicable)</param>
        public ISubscription CreateSubscription(string productHandle, ICustomerAttributes customerAttributes, PaymentCollectionMethod? paymentCollectionMethod = PaymentCollectionMethod.Automatic)
        {
            if (customerAttributes == null) throw new ArgumentNullException(nameof(customerAttributes));
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName, customerAttributes.LastName,
                customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, string.Empty, DateTime.MinValue, null, paymentCollectionMethod);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time and import the card data from a specific vault storage
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="nextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="existingProfile">Data concerning the existing profile in vault storage</param>
        /// <returns>The xml describing the new subscription</returns>
        public ISubscription CreateSubscription(string productHandle, ICustomerAttributes customerAttributes, DateTime nextBillingAt, IPaymentProfileAttributes existingProfile)
        {
            if (customerAttributes == null) throw new ArgumentNullException(nameof(customerAttributes));
            if (existingProfile == null) throw new ArgumentNullException(nameof(existingProfile));
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName,
                                      customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, string.Empty, nextBillingAt, existingProfile, null);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time and use the card data from another payment profile (from the same customer).
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="nextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="existingProfileId">The ID of the existing payment profile to use when creating the new subscription.</param>
        /// <returns>The new subscription</returns>
        public ISubscription CreateSubscription(string productHandle, ICustomerAttributes customerAttributes, DateTime nextBillingAt, int existingProfileId)
        {
            if (customerAttributes == null) throw new ArgumentNullException(nameof(customerAttributes));
            if (existingProfileId <= 0) throw new ArgumentNullException(nameof(existingProfileId));
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName,
                                      customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, string.Empty, nextBillingAt, existingProfileId);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <param name="nextBillingAt">DateTime for this customer to be assessed at</param>
        /// <returns>The xml describing the new subscription</returns>
        public ISubscription CreateSubscription(string productHandle, ICustomerAttributes customerAttributes, ICreditCardAttributes creditCardAttributes, DateTime nextBillingAt)
        {
            // version bump
            if (customerAttributes == null) throw new ArgumentNullException(nameof(customerAttributes));
            if (creditCardAttributes == null) throw new ArgumentNullException(nameof(creditCardAttributes));
            if (nextBillingAt == DateTime.MinValue) throw new ArgumentOutOfRangeException(nameof(nextBillingAt));

            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName,
                                      customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, customerAttributes.ShippingAddress, customerAttributes.ShippingCity,
                                      customerAttributes.ShippingState, customerAttributes.ShippingZip, customerAttributes.ShippingCountry,
                                      creditCardAttributes.FirstName, creditCardAttributes.LastName, creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress, creditCardAttributes.BillingCity, creditCardAttributes.BillingState, creditCardAttributes.BillingZip,
                                      creditCardAttributes.BillingCountry, string.Empty, null, nextBillingAt);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string productHandle, ICustomerAttributes customerAttributes,
                                                       ICreditCardAttributes creditCardAttributes)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException(nameof(creditCardAttributes));
            if (customerAttributes == null) throw new ArgumentNullException(nameof(customerAttributes));
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName,
                                      customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, customerAttributes.VatNumber,
                                      customerAttributes.ShippingAddress, customerAttributes.ShippingCity, customerAttributes.ShippingState, customerAttributes.ShippingZip, customerAttributes.ShippingCountry,
                                      creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress, creditCardAttributes.BillingCity,
                                      creditCardAttributes.BillingState, creditCardAttributes.BillingZip, creditCardAttributes.BillingCountry, string.Empty, int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <param name="componentsWithQuantity">The components to set on the subscription initially</param>
        /// <returns></returns>
        public ISubscription CreateSubscription(string productHandle, ICustomerAttributes customerAttributes, ICreditCardAttributes creditCardAttributes, Dictionary<int, string> componentsWithQuantity)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException(nameof(creditCardAttributes));
            if (customerAttributes == null) throw new ArgumentNullException(nameof(customerAttributes));
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName,
                                      customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, customerAttributes.ShippingAddress, customerAttributes.ShippingCity, customerAttributes.ShippingState, customerAttributes.ShippingZip, customerAttributes.ShippingCountry,
                                      creditCardAttributes.FirstName, creditCardAttributes.LastName, creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress, creditCardAttributes.BillingCity,
                                      creditCardAttributes.BillingState, creditCardAttributes.BillingZip, creditCardAttributes.BillingCountry, string.Empty, componentsWithQuantity, null);
        }

        private ISubscription CreateSubscription(string productHandle, string newSystemId, string firstName, string lastName, string emailAddress, string phone,
                                                        string organization, string shippingAddress, string shippingCity, string shippingState, string shippingZip, string shippingCountry,
                                                        string cardFirstName, string cardLastName, string fullNumber, int expirationMonth, int expirationYear,
                                                        string cvv, string billingAddress, string billingCity, string billingState, string billingZip,
                                                        string billingCountry, string couponCode, Dictionary<int, string> componentsWithQuantity, DateTime? nextBillingAt)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException(nameof(productHandle));
            var product = LoadProduct(productHandle);
            if (product == null) throw new ArgumentException("The product doesn't exist", productHandle);
            // if ((ComponentsWithQuantity.Count < 0)) throw new ArgumentNullException("ComponentsWithQuantity", "No components specified");

            if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException(nameof(firstName));
            if (string.IsNullOrEmpty(lastName)) throw new ArgumentNullException(nameof(lastName));
            if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException(nameof(emailAddress));
            if (string.IsNullOrEmpty(fullNumber)) throw new ArgumentNullException(nameof(fullNumber));
            //if (NewSystemID == string.Empty) throw new ArgumentNullException("NewSystemID");
            if ((expirationMonth <= 0) && (expirationMonth > 12)) throw new ArgumentException("Not within range", nameof(expirationMonth));
            if (expirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", nameof(expirationYear));
            if (_cvvRequired && string.IsNullOrEmpty(cvv)) throw new ArgumentNullException(nameof(cvv));
            if (_cvvRequired && ((cvv.Length < 3) || (cvv.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", nameof(cvv));

            //if (!string.IsNullOrEmpty(NewSystemID))
            //{
            //    // make sure that the system ID is unique
            //    if (this.LoadCustomer(NewSystemID) != null) throw new ArgumentException("Not unique", "NewSystemID");
            //}

            // create XML for creation of customer
            var subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            subscriptionXml.Append("<customer_attributes>");
            subscriptionXml.AppendFormat("<first_name>{0}</first_name>", firstName);
            subscriptionXml.AppendFormat("<last_name>{0}</last_name>", lastName);
            subscriptionXml.AppendFormat("<email>{0}</email>", emailAddress);
            if (!string.IsNullOrEmpty(phone)) subscriptionXml.AppendFormat("<phone>{0}</phone>", phone);
            subscriptionXml.AppendFormat("<organization>{0}</organization>", (organization != null) ? HttpUtility.HtmlEncode(organization) : "null");
            subscriptionXml.AppendFormat("<reference>{0}</reference>", newSystemId);
            if (!string.IsNullOrEmpty(shippingAddress)) subscriptionXml.AppendFormat("<address>{0}</address>", shippingAddress);
            if (!string.IsNullOrEmpty(shippingCity)) subscriptionXml.AppendFormat("<city>{0}</city>", shippingCity);
            if (!string.IsNullOrEmpty(shippingState)) subscriptionXml.AppendFormat("<state>{0}</state>", shippingState);
            if (!string.IsNullOrEmpty(shippingZip)) subscriptionXml.AppendFormat("<zip>{0}</zip>", shippingZip);
            if (!string.IsNullOrEmpty(shippingCountry)) subscriptionXml.AppendFormat("<country>{0}</country>", shippingCountry);
            subscriptionXml.Append("</customer_attributes>");
            subscriptionXml.Append("<credit_card_attributes>");
            if (!string.IsNullOrWhiteSpace(cardFirstName)) subscriptionXml.AppendFormat("<first_name>{0}</first_name>", cardFirstName);
            if (!string.IsNullOrWhiteSpace(cardLastName)) subscriptionXml.AppendFormat("<last_name>{0}</last_name>", cardLastName);
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", fullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", expirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", expirationYear);
            if (_cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", cvv); }
            if (!string.IsNullOrEmpty(billingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", billingAddress);
            if (!string.IsNullOrEmpty(billingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", billingCity);
            if (!string.IsNullOrEmpty(billingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", billingState);
            if (!string.IsNullOrEmpty(billingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", billingZip);
            if (!string.IsNullOrEmpty(billingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", billingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); }
            if (nextBillingAt.HasValue) { subscriptionXml.AppendFormat("<next_billing_at>{0}</next_billing_at>", nextBillingAt.Value.ToString("o")); }
            if (componentsWithQuantity != null && componentsWithQuantity.Count > 0)
            {
                subscriptionXml.Append(@"<components type=""array"">");
                foreach (var item in componentsWithQuantity)
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
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription, specifying a coupon
        /// </summary>
        /// <param name="productHandle">The product to subscribe to</param>
        /// <param name="customerAttributes">Details about the customer</param>
        /// <param name="creditCardAttributes">Payment details</param>
        /// <param name="couponCode">The coupon to use</param>
        /// <param name="componentsWithQuantity">Components to set on the subscription initially</param>
        /// <returns>Details about the subscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, ICustomerAttributes customerAttributes, ICreditCardAttributes creditCardAttributes, string couponCode, Dictionary<int, string> componentsWithQuantity)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException("creditCardAttributes");
            if (customerAttributes == null) throw new ArgumentNullException("customerAttributes");
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName, customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization,
                                      customerAttributes.ShippingAddress, customerAttributes.ShippingCity, customerAttributes.ShippingState, customerAttributes.ShippingZip, customerAttributes.ShippingCountry,
                                      creditCardAttributes.FirstName, creditCardAttributes.LastName, creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress, creditCardAttributes.BillingCity,
                                      creditCardAttributes.BillingState, creditCardAttributes.BillingZip, creditCardAttributes.BillingCountry, couponCode, componentsWithQuantity, null);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <param name="componentId">The component to allocate when creating the subscription</param>
        /// <param name="allocatedQuantity">The quantity to allocate of the component when creating the subscription</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string productHandle, ICustomerAttributes customerAttributes, ICreditCardAttributes creditCardAttributes, int componentId, int allocatedQuantity)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException("creditCardAttributes");
            if (customerAttributes == null) throw new ArgumentNullException("customerAttributes");
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName,
                                      customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, customerAttributes.VatNumber,
                                      customerAttributes.ShippingAddress, customerAttributes.ShippingCity, customerAttributes.ShippingState, customerAttributes.ShippingZip, customerAttributes.ShippingCountry,
                                      creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress, creditCardAttributes.BillingCity,
                                      creditCardAttributes.BillingState, creditCardAttributes.BillingZip, creditCardAttributes.BillingCountry, string.Empty, componentId, allocatedQuantity);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time using no credit card information
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, ICustomerAttributes customerAttributes, string couponCode)
        {
            // make sure data is valid
            if (customerAttributes == null) throw new ArgumentNullException("customerAttributes");
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName, customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, couponCode, DateTime.MinValue, null, null);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time using no credit card information
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <param name="nextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="existingProfile">Data concerning the existing profile in vault storage</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, ICustomerAttributes customerAttributes, string couponCode, DateTime nextBillingAt, IPaymentProfileAttributes existingProfile)
        {
            // make sure data is valid
            if (customerAttributes == null) throw new ArgumentNullException("customerAttributes");
            if (existingProfile == null) throw new ArgumentNullException("existingProfile");
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName, customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, couponCode, nextBillingAt, existingProfile, null);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time using no credit card information
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <param name="nextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="existingProfileId">The ID of the data concerning the existing profile in vault storage</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, ICustomerAttributes customerAttributes, string couponCode, DateTime nextBillingAt, int existingProfileId)
        {
            // make sure data is valid
            if (customerAttributes == null) throw new ArgumentNullException("customerAttributes");
            if (existingProfileId <= 0) throw new ArgumentNullException("existingProfileId");
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName, customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, couponCode, nextBillingAt, existingProfileId);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, ICustomerAttributes customerAttributes,
                                                       ICreditCardAttributes creditCardAttributes, string couponCode)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException("creditCardAttributes");
            if (customerAttributes == null) throw new ArgumentNullException("customerAttributes");
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName,
                                      customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, customerAttributes.VatNumber,
                                      customerAttributes.ShippingAddress, customerAttributes.ShippingCity, customerAttributes.ShippingState, customerAttributes.ShippingZip, customerAttributes.ShippingCountry,
                                      creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress, creditCardAttributes.BillingCity,
                                      creditCardAttributes.BillingState, creditCardAttributes.BillingZip, creditCardAttributes.BillingCountry, couponCode, int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <param name="nextBillingAt">Specify the time of first assessment</param>
        /// <returns>The new subscription object</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, ICustomerAttributes customerAttributes, ICreditCardAttributes creditCardAttributes, DateTime nextBillingAt, string couponCode)
        {
            if (creditCardAttributes == null) throw new ArgumentNullException("creditCardAttributes");
            if (customerAttributes == null) throw new ArgumentNullException("customerAttributes");
            if (nextBillingAt == null || nextBillingAt == DateTime.MinValue) throw new ArgumentNullException("nextBillingAt");

            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName,
                                      customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization,
                                      customerAttributes.ShippingAddress, customerAttributes.ShippingCity, customerAttributes.ShippingState, customerAttributes.ShippingZip, customerAttributes.ShippingCountry,
                                      creditCardAttributes.FirstName, creditCardAttributes.LastName, creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress, creditCardAttributes.BillingCity,
                                      creditCardAttributes.BillingState, creditCardAttributes.BillingZip, creditCardAttributes.BillingCountry, couponCode, null, nextBillingAt);
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="customerAttributes">The attributes for the new customer</param>
        /// <param name="creditCardAttributes">The credit card attributes</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <param name="componentId">The component to allocate when creating the subscription</param>
        /// <param name="allocatedQuantity">The quantity to allocate of the component when creating the subscription</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscriptionUsingCoupon(string productHandle, ICustomerAttributes customerAttributes,
                                                       ICreditCardAttributes creditCardAttributes, string couponCode, int componentId, int allocatedQuantity)
        {
            // make sure data is valid
            if (creditCardAttributes == null) throw new ArgumentNullException("creditCardAttributes");
            if (customerAttributes == null) throw new ArgumentNullException("customerAttributes");
            return CreateSubscription(productHandle, customerAttributes.SystemID, customerAttributes.FirstName,
                                      customerAttributes.LastName, customerAttributes.Email, customerAttributes.Phone, customerAttributes.Organization, customerAttributes.VatNumber,
                                      customerAttributes.ShippingAddress, customerAttributes.ShippingCity, customerAttributes.ShippingState, customerAttributes.ShippingZip, customerAttributes.ShippingCountry,
                                      creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth,
                                      creditCardAttributes.ExpirationYear, creditCardAttributes.CVV, creditCardAttributes.BillingAddress, creditCardAttributes.BillingCity,
                                      creditCardAttributes.BillingState, creditCardAttributes.BillingZip, creditCardAttributes.BillingCountry, couponCode, componentId, allocatedQuantity);
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="chargifyId">The Chargify ID of the customer</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <param name="paymentCollectionMethod">Optional, type of payment collection method</param>
        /// <returns>The xml describing the new subsscription</returns>
        public ISubscription CreateSubscription(string productHandle, int chargifyId, string couponCode, PaymentCollectionMethod? paymentCollectionMethod)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException(nameof(productHandle));
            if (chargifyId == int.MinValue) throw new ArgumentNullException(nameof(chargifyId));

            // make sure that the system ID is unique
            if (LoadCustomer(chargifyId) == null) throw new ArgumentException("Customer not found with that ID", nameof(chargifyId));

            IProduct subscribingProduct = LoadProduct(productHandle);
            if (subscribingProduct == null) throw new ArgumentException("Product not found");
            if (subscribingProduct.RequireCreditCard) throw new ChargifyNetException("Product requires credit card information");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            subscriptionXml.AppendFormat("<customer_id>{0}</customer_id>", chargifyId);
            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); }
            if (paymentCollectionMethod.HasValue)
            {
                if (paymentCollectionMethod.Value != PaymentCollectionMethod.Unknown)
                {
                    var paymentCollectionMethodName = Enum.GetName(typeof(PaymentCollectionMethod), paymentCollectionMethod.Value);
                    if (paymentCollectionMethodName != null)
                        subscriptionXml.AppendFormat("<payment_collection_method>{0}</payment_collection_method>", paymentCollectionMethodName.ToLowerInvariant());
                }
            }
            subscriptionXml.Append("</subscription>");

            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="chargifyId">The Chargify ID of the customer</param>
        /// <param name="fullNumber">The full number of the credit card</param>
        /// <param name="expirationMonth">The expritation month of the credit card</param>
        /// <param name="expirationYear">The expiration year of the credit card</param>
        /// <param name="cvv">The CVV for the credit card</param>
        /// <param name="billingAddress">The billing address</param>
        /// <param name="billingCity">The billing city</param>
        /// <param name="billingState">The billing state or province</param>
        /// <param name="billingZip">The billing zip code or postal code</param>
        /// <param name="billingCountry">The billing country</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <param name="firstName">The first name, as it appears on the credit card</param>
        /// <param name="lastName">The last name, as it appears on the credit card</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string productHandle, int chargifyId, string fullNumber, int expirationMonth, int expirationYear,
                                                        string cvv, string billingAddress, string billingCity, string billingState, string billingZip,
                                                        string billingCountry, string couponCode, string firstName, string lastName)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException(nameof(productHandle));
            if (chargifyId == int.MinValue) throw new ArgumentNullException(nameof(chargifyId));
            // make sure that the system ID is unique
            if (LoadCustomer(chargifyId) == null) throw new ArgumentException("Customer not found with that ID", nameof(chargifyId));
            if (string.IsNullOrEmpty(fullNumber)) throw new ArgumentNullException(nameof(fullNumber));
            if ((expirationMonth <= 0) && (expirationMonth > 12)) throw new ArgumentException("Not within range", nameof(expirationMonth));
            if (expirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", nameof(expirationYear));
            if (_cvvRequired && string.IsNullOrEmpty(cvv)) throw new ArgumentNullException(nameof(cvv));

            // Since the hosted pages don't necessarily use these - I'm not sure if we should be including them.
            //if (string.IsNullOrEmpty(BillingZip)) throw new ArgumentNullException("BillingZip");
            //if (string.IsNullOrEmpty(BillingAddress)) throw new ArgumentNullException("BillingAddress");
            //if (string.IsNullOrEmpty(BillingCity)) throw new ArgumentNullException("BillingCity");
            //if (string.IsNullOrEmpty(BillingState)) throw new ArgumentNullException("BillingState");
            //if (string.IsNullOrEmpty(BillingCountry)) throw new ArgumentNullException("BillingCountry");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            subscriptionXml.AppendFormat("<customer_id>{0}</customer_id>", chargifyId);
            subscriptionXml.Append("<credit_card_attributes>");
            if (!string.IsNullOrEmpty(firstName)) { subscriptionXml.AppendFormat("<first_name>{0}</first_name>", firstName); }
            if (!string.IsNullOrEmpty(lastName)) { subscriptionXml.AppendFormat("<last_name>{0}</last_name>", lastName); }
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", fullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", expirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", expirationYear);
            if (_cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", cvv); }
            if (!string.IsNullOrEmpty(billingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", billingAddress);
            if (!string.IsNullOrEmpty(billingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", billingCity);
            if (!string.IsNullOrEmpty(billingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", billingState);
            if (!string.IsNullOrEmpty(billingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", billingZip);
            if (!string.IsNullOrEmpty(billingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", billingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="chargifyId">The Chargify ID of the customer</param>
        /// <param name="fullNumber">The full number of the credit card</param>
        /// <param name="expirationMonth">The expritation month of the credit card</param>
        /// <param name="expirationYear">The expiration year of the credit card</param>
        /// <param name="cvv">The CVV for the credit card</param>
        /// <param name="billingAddress">The billing address</param>
        /// <param name="billingCity">The billing city</param>
        /// <param name="billingState">The billing state or province</param>
        /// <param name="billingZip">The billing zip code or postal code</param>
        /// <param name="billingCountry">The billing country</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string productHandle, int chargifyId, string fullNumber, int expirationMonth, int expirationYear,
                                                        string cvv, string billingAddress, string billingCity, string billingState, string billingZip,
                                                        string billingCountry, string couponCode)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException("productHandle");
            if (chargifyId == int.MinValue) throw new ArgumentNullException("chargifyId");
            // make sure that the system ID is unique
            if (LoadCustomer(chargifyId) == null) throw new ArgumentException("Customer Not Found", nameof(chargifyId));
            if (string.IsNullOrEmpty(fullNumber)) throw new ArgumentNullException("fullNumber");
            if ((expirationMonth <= 0) && (expirationMonth > 12)) throw new ArgumentException("Not within range", "expirationMonth");
            if (expirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", "expirationYear");
            if (_cvvRequired && string.IsNullOrEmpty(cvv)) throw new ArgumentNullException("cvv");
            if (_cvvRequired && ((cvv.Length < 3) || (cvv.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "cvv");

            // Since the hosted pages don't use these - I'm not sure if we should be including them.
            //if (string.IsNullOrEmpty(BillingZip)) throw new ArgumentNullException("BillingZip");
            //if (string.IsNullOrEmpty(BillingAddress)) throw new ArgumentNullException("BillingAddress");
            //if (string.IsNullOrEmpty(BillingCity)) throw new ArgumentNullException("BillingCity");
            //if (string.IsNullOrEmpty(BillingState)) throw new ArgumentNullException("BillingState");
            //if (string.IsNullOrEmpty(BillingCountry)) throw new ArgumentNullException("BillingCountry");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            subscriptionXml.AppendFormat("<customer_id>{0}</customer_id>", chargifyId);
            subscriptionXml.Append("<credit_card_attributes>");
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", fullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", expirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", expirationYear);
            if (_cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", cvv); }
            if (!string.IsNullOrEmpty(billingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", billingAddress);
            if (!string.IsNullOrEmpty(billingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", billingCity);
            if (!string.IsNullOrEmpty(billingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", billingState);
            if (!string.IsNullOrEmpty(billingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", billingZip);
            if (!string.IsNullOrEmpty(billingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", billingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="systemId">The System ID of the customer</param>
        /// <param name="couponCode">The discount coupon code</param> 
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string productHandle, string systemId, string couponCode)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException("productHandle");
            if (systemId == string.Empty) throw new ArgumentNullException("systemId");

            // make sure that the system ID is unique
            if (LoadCustomer(systemId) == null) throw new ArgumentException("Customer Not Found", "systemId");

            IProduct subscribingProduct = LoadProduct(productHandle);
            if (subscribingProduct == null) throw new ArgumentException("Product not found", "productHandle");
            if (subscribingProduct.RequireCreditCard) throw new ChargifyNetException("Product requires credit card information");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            subscriptionXml.AppendFormat("<customer_reference>{0}</customer_reference>", systemId);
            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); }
            subscriptionXml.Append("</subscription>");

            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="systemId">The System ID of the customer</param>
        /// <param name="fullNumber">The full number of the credit card</param>
        /// <param name="expirationMonth">The expritation month of the credit card</param>
        /// <param name="expirationYear">The expiration year of the credit card</param>
        /// <param name="cvv">The CVV for the credit card</param>
        /// <param name="billingAddress">The billing address</param>
        /// <param name="billingCity">The billing city</param>
        /// <param name="billingState">The billing state or province</param>
        /// <param name="billingZip">The billing zip code or postal code</param>
        /// <param name="billingCountry">The billing country</param>
        /// <param name="couponCode">The discount coupon code</param> 
        /// <param name="firstName">The first name, as listed on the credit card</param>
        /// <param name="lastName">The last name, as listed on the credit card</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string productHandle, string systemId, string fullNumber, int expirationMonth, int expirationYear,
                                                        string cvv, string billingAddress, string billingCity, string billingState, string billingZip,
                                                        string billingCountry, string couponCode, string firstName, string lastName)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException(nameof(productHandle));
            var product = LoadProduct(productHandle);
            if (product == null) throw new ArgumentException("That product doesn't exist", nameof(productHandle));
            if (systemId == string.Empty) throw new ArgumentNullException(nameof(systemId));
            // make sure that the system ID is unique
            if (LoadCustomer(systemId) == null) throw new ArgumentException("Customer Not Found", nameof(systemId));
            if (product.RequireCreditCard)
            {
                if (string.IsNullOrEmpty(fullNumber)) throw new ArgumentNullException(nameof(fullNumber));
                if ((expirationMonth <= 0) && (expirationMonth > 12)) throw new ArgumentException("Not within range", nameof(expirationMonth));
                if (expirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", nameof(expirationYear));
                if (_cvvRequired && string.IsNullOrEmpty(cvv)) throw new ArgumentNullException(nameof(cvv));
                if (_cvvRequired && ((cvv.Length < 3) || (cvv.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", nameof(cvv));
            }
            // Don't throw exceptions on these, since we don't know if they are absolutely needed.
            //if (string.IsNullOrEmpty(BillingAddress)) throw new ArgumentNullException("BillingAddress");
            //if (string.IsNullOrEmpty(BillingCity)) throw new ArgumentNullException("BillingCity");
            //if (string.IsNullOrEmpty(BillingState)) throw new ArgumentNullException("BillingState");
            //if (string.IsNullOrEmpty(BillingZip)) throw new ArgumentNullException("BillingZip");
            //if (string.IsNullOrEmpty(BillingCountry)) throw new ArgumentNullException("BillingCountry");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            subscriptionXml.AppendFormat("<customer_reference>{0}</customer_reference>", systemId);
            if (product.RequireCreditCard)
            {
                subscriptionXml.Append("<credit_card_attributes>");
                if (!string.IsNullOrEmpty(firstName)) { subscriptionXml.AppendFormat("<first_name>{0}</first_name>", firstName); }
                if (!string.IsNullOrEmpty(lastName)) { subscriptionXml.AppendFormat("<last_name>{0}</last_name>", lastName); }
                subscriptionXml.AppendFormat("<full_number>{0}</full_number>", fullNumber);
                subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", expirationMonth);
                subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", expirationYear);
                if (_cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", cvv); }
                if (!string.IsNullOrEmpty(billingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", billingAddress);
                if (!string.IsNullOrEmpty(billingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", billingCity);
                if (!string.IsNullOrEmpty(billingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", billingState);
                if (!string.IsNullOrEmpty(billingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", billingZip);
                if (!string.IsNullOrEmpty(billingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", billingCountry);
                subscriptionXml.Append("</credit_card_attributes>");
            }
            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="systemId">The System ID of the customer</param>
        /// <param name="fullNumber">The full number of the credit card</param>
        /// <param name="expirationMonth">The expritation month of the credit card</param>
        /// <param name="expirationYear">The expiration year of the credit card</param>
        /// <param name="cvv">The CVV for the credit card</param>
        /// <param name="billingAddress">The billing address</param>
        /// <param name="billingCity">The billing city</param>
        /// <param name="billingState">The billing state or province</param>
        /// <param name="billingZip">The billing zip code or postal code</param>
        /// <param name="billingCountry">The billing country</param>
        /// <param name="couponCode">The discount coupon code</param> 
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string productHandle, string systemId, string fullNumber, int expirationMonth, int expirationYear,
                                                        string cvv, string billingAddress, string billingCity, string billingState, string billingZip,
                                                        string billingCountry, string couponCode)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException("productHandle");
            if (systemId == string.Empty) throw new ArgumentNullException("systemId");
            // make sure that the system ID is unique
            if (LoadCustomer(systemId) == null) throw new ArgumentException("Customer Not Found", "systemId");
            if (string.IsNullOrEmpty(fullNumber)) throw new ArgumentNullException("fullNumber");
            if ((expirationMonth <= 0) && (expirationMonth > 12)) throw new ArgumentException("Not within range", "expirationMonth");
            if (expirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", "expirationYear");
            if (_cvvRequired && string.IsNullOrEmpty(cvv)) throw new ArgumentNullException("cvv");
            if (_cvvRequired && ((cvv.Length < 3) || (cvv.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "cvv");
            //if (string.IsNullOrEmpty(BillingAddress)) throw new ArgumentNullException("BillingAddress");
            //if (string.IsNullOrEmpty(BillingCity)) throw new ArgumentNullException("BillingCity");
            //if (string.IsNullOrEmpty(BillingState)) throw new ArgumentNullException("BillingState");
            //if (string.IsNullOrEmpty(BillingZip)) throw new ArgumentNullException("BillingZip");
            //if (string.IsNullOrEmpty(BillingCountry)) throw new ArgumentNullException("BillingCountry");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            subscriptionXml.AppendFormat("<customer_reference>{0}</customer_reference>", systemId);
            subscriptionXml.Append("<credit_card_attributes>");
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", fullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", expirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", expirationYear);
            if (_cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", cvv); }
            if (!string.IsNullOrEmpty(billingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", billingAddress);
            if (!string.IsNullOrEmpty(billingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", billingCity);
            if (!string.IsNullOrEmpty(billingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", billingState);
            if (!string.IsNullOrEmpty(billingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", billingZip);
            if (!string.IsNullOrEmpty(billingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", billingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="newSystemId">The reference field value of the customer</param>
        /// <param name="firstName">The first name of the customer</param>
        /// <param name="lastName">The last name of the customer</param>
        /// <param name="emailAddress">The email address of the customer</param>
        /// <param name="phone">The phone number of the customer</param>
        /// <param name="organization">The customer's organization</param>
        /// <param name="couponCode">The discount coupon code</param> 
        /// <param name="nextBillingAt">The next date that the billing should be processed (DateTime.Min if unspecified)</param>
        /// <param name="paymentProfileId">The id of the payment profile to use when creating the subscription (existing data)</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string productHandle, string newSystemId, string firstName, string lastName, string emailAddress, string phone,
                                                 string organization, string couponCode, DateTime nextBillingAt, int paymentProfileId)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException("productHandle");
            if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException("firstName");
            if (string.IsNullOrEmpty(lastName)) throw new ArgumentNullException("lastName");
            if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException("emailAddress");

            ICustomer existingCustomer = LoadCustomer(newSystemId);
            IProduct subscribingProduct = LoadProduct(productHandle);
            if (subscribingProduct == null) throw new ArgumentException("Product not found", "productHandle");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);

            if (existingCustomer == null)
            {
                subscriptionXml.Append("<customer_attributes>");
                subscriptionXml.AppendFormat("<first_name>{0}</first_name>", firstName);
                subscriptionXml.AppendFormat("<last_name>{0}</last_name>", lastName);
                subscriptionXml.AppendFormat("<email>{0}</email>", emailAddress);
                if (!String.IsNullOrEmpty(phone)) subscriptionXml.AppendFormat("<phone>{0}</phone>", phone);
                subscriptionXml.AppendFormat("<organization>{0}</organization>", (organization != null) ? HttpUtility.HtmlEncode(organization) : "null");
                subscriptionXml.AppendFormat("<reference>{0}</reference>", newSystemId);
                subscriptionXml.Append("</customer_attributes>");
            }
            else
            {
                subscriptionXml.AppendFormat("<customer_id>{0}</customer_id>", existingCustomer.ChargifyID);
            }

            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); } // Optional
            subscriptionXml.AppendFormat("<payment_profile_id>{0}</payment_profile_id>", paymentProfileId);
            if (nextBillingAt != DateTime.MinValue) { subscriptionXml.AppendFormat("<next_billing_at>{0}</next_billing_at>", nextBillingAt); }
            subscriptionXml.Append("</subscription>");

            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="newSystemId">The reference field value of the customer</param>
        /// <param name="firstName">The first name of the customer</param>
        /// <param name="lastName">The last name of the customer</param>
        /// <param name="emailAddress">The email address of the customer</param>
        /// <param name="phone">The phone number of the customer</param>
        /// <param name="organization">The customer's organization</param>
        /// <param name="couponCode">The discount coupon code</param> 
        /// <param name="nextBillingAt">The next date that the billing should be processed</param>
        /// <param name="paymentProfile">The paymentProfile object to use when creating the subscription (existing data)</param>
        /// <param name="paymentCollectionMethod">The type of subscription, recurring (automatic) billing, or invoice (if applicable)</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string productHandle, string newSystemId, string firstName, string lastName, string emailAddress, string phone,
                                                 string organization, string couponCode, DateTime nextBillingAt, IPaymentProfileAttributes paymentProfile, PaymentCollectionMethod? paymentCollectionMethod)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException("productHandle");
            if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException("firstName");
            if (string.IsNullOrEmpty(lastName)) throw new ArgumentNullException("lastName");
            if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException("emailAddress");

            //if (!string.IsNullOrEmpty(NewSystemID))
            //{
            //    // make sure that the system ID is unique
            //    if (this.LoadCustomer(NewSystemID) != null) throw new ArgumentException("Not unique", "NewSystemID");
            //}
            IProduct subscribingProduct = LoadProduct(productHandle);
            if (subscribingProduct == null) throw new ArgumentException("Product not found", "productHandle");
            if (subscribingProduct.RequireCreditCard)
            {
                // Product requires credit card and no payment information passed in.
                if (paymentProfile == null) throw new ChargifyNetException("Product requires credit card information");
            }

            // create XML for creation of customer
            var subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            subscriptionXml.Append("<customer_attributes>");
            subscriptionXml.AppendFormat("<first_name>{0}</first_name>", firstName);
            subscriptionXml.AppendFormat("<last_name>{0}</last_name>", lastName);
            subscriptionXml.AppendFormat("<email>{0}</email>", emailAddress);
            if (!String.IsNullOrEmpty(phone)) subscriptionXml.AppendFormat("<phone>{0}</phone>", phone);
            subscriptionXml.AppendFormat("<organization>{0}</organization>", (organization != null) ? HttpUtility.HtmlEncode(organization) : "null");
            subscriptionXml.AppendFormat("<reference>{0}</reference>", newSystemId);
            subscriptionXml.Append("</customer_attributes>");
            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); } // Optional
            if (paymentProfile != null)
            {
                // The round-trip "o" format uses ISO 8601 for date/time representation, neat.
                subscriptionXml.AppendFormat("<next_billing_at>{0}</next_billing_at>", nextBillingAt.ToString("o"));
                subscriptionXml.Append("<payment_profile_attributes>");
                subscriptionXml.AppendFormat("<vault_token>{0}</vault_token>", paymentProfile.VaultToken);
                subscriptionXml.AppendFormat("<customer_vault_token>{0}</customer_vault_token>", paymentProfile.CustomerVaultToken);
                subscriptionXml.AppendFormat("<current_vault>{0}</current_vault>", paymentProfile.CurrentVault.ToString().ToLowerInvariant());
                subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", paymentProfile.ExpirationYear);
                subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", paymentProfile.ExpirationMonth);
                if (paymentProfile.CardType != CardType.Unknown) { subscriptionXml.AppendFormat("<card_type>{0}</card_type>", paymentProfile.CardType.ToString().ToLowerInvariant()); } // Optional
                if (paymentProfile.LastFour != String.Empty) { subscriptionXml.AppendFormat("<last_four>{0}</last_four>", paymentProfile.LastFour); } // Optional
                if (paymentProfile.BankName != String.Empty) { subscriptionXml.AppendFormat("<bank_name>{0}</bank_name>", paymentProfile.BankName); }
                if (paymentProfile.BankRoutingNumber != String.Empty) { subscriptionXml.AppendFormat("<bank_routing_number>{0}</bank_routing_number>", paymentProfile.BankRoutingNumber); }
                if (paymentProfile.BankAccountNumber != String.Empty) { subscriptionXml.AppendFormat("<bank_account_number>{0}</bank_account_number>", paymentProfile.BankAccountNumber); }
                if (paymentProfile.BankAccountType != BankAccountType.Unknown) { subscriptionXml.AppendFormat("<bank_account_type>{0}</bank_account_type>", paymentProfile.BankAccountType.ToString().ToLowerInvariant()); }
                if (paymentProfile.BankAccountHolderType != BankAccountHolderType.Unknown) { subscriptionXml.AppendFormat("<bank_account_holder_type>{0}</bank_account_holder_type>", paymentProfile.BankAccountHolderType.ToString().ToLowerInvariant()); }
                subscriptionXml.Append("</payment_profile_attributes>");
            }
            if (paymentCollectionMethod.HasValue)
            {
                if (paymentCollectionMethod.Value != PaymentCollectionMethod.Unknown)
                {
                    var paymentCollectionMethodName = Enum.GetName(typeof(PaymentCollectionMethod), paymentCollectionMethod.Value);
                    if (paymentCollectionMethodName != null)
                        subscriptionXml.AppendFormat("<payment_collection_method>{0}</payment_collection_method>", paymentCollectionMethodName.ToLowerInvariant());
                }
            }
            subscriptionXml.Append("</subscription>");

            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="productHandle">The handle to the product</param>
        /// <param name="newSystemId">The system ID for the new customer</param>
        /// <param name="firstName">The first name of the new customer</param>
        /// <param name="lastName">The last nameof the new customer</param>
        /// <param name="emailAddress">The email address for the new customer</param>
        /// <param name="phone">The phone number for the customer</param>
        /// <param name="organization">The organization of the new customer</param>
        /// <param name="vatNumber">The value added tax number</param>
        /// <param name="shippingAddress">The shipping address of the customer</param>
        /// <param name="shippingCity">The shipping city of the customer</param>
        /// <param name="shippingState">The shipping state of the customer</param>
        /// <param name="shippingZip">The shipping zip of the customer</param>
        /// <param name="shippingCountry">The shipping country of the customer</param>
        /// <param name="fullNumber">The full number of the credit card</param>
        /// <param name="expirationMonth">The expritation month of the credit card</param>
        /// <param name="expirationYear">The expiration year of the credit card</param>
        /// <param name="cvv">The CVV for the credit card</param>
        /// <param name="billingAddress">The billing address</param>
        /// <param name="billingCity">The billing city</param>
        /// <param name="billingState">The billing state or province</param>
        /// <param name="billingZip">The billing zip code or postal code</param>
        /// <param name="billingCountry">The billing country</param>
        /// <param name="couponCode">The discount coupon code</param>
        /// <param name="componentId">The component to add while creating the subscription</param>
        /// <param name="allocatedQuantity">The quantity of the component to allocate when creating the component usage for the new subscription</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription CreateSubscription(string productHandle, string newSystemId, string firstName, string lastName, string emailAddress, string phone,
                                                        string organization, string vatNumber, string shippingAddress, string shippingCity, string shippingState, string shippingZip, string shippingCountry,
                                                        string fullNumber, int expirationMonth, int expirationYear,
                                                        string cvv, string billingAddress, string billingCity, string billingState, string billingZip,
                                                        string billingCountry, string couponCode, int componentId, int allocatedQuantity)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException("productHandle");
            var product = LoadProduct(productHandle);
            if (product == null) throw new ArgumentException("The product doesn't exist", productHandle);
            if ((componentId != int.MinValue) && (allocatedQuantity == int.MinValue)) throw new ArgumentNullException("allocatedQuantity");

            if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException("firstName");
            if (string.IsNullOrEmpty(lastName)) throw new ArgumentNullException("lastName");
            if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException("emailAddress");
            if (string.IsNullOrEmpty(fullNumber)) throw new ArgumentNullException("fullNumber");
            //if (NewSystemID == string.Empty) throw new ArgumentNullException("NewSystemID");
            if ((expirationMonth <= 0) && (expirationMonth > 12)) throw new ArgumentException("Not within range", "expirationMonth");
            if (expirationYear < DateTime.Today.Year) throw new ArgumentException("Not within range", "expirationYear");
            if (_cvvRequired && string.IsNullOrEmpty(cvv)) throw new ArgumentNullException("cvv");
            if (_cvvRequired && ((cvv.Length < 3) || (cvv.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "cvv");

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
            var subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            subscriptionXml.Append("<customer_attributes>");
            subscriptionXml.AppendFormat("<first_name>{0}</first_name>", firstName.ToHtmlEncoded());
            subscriptionXml.AppendFormat("<last_name>{0}</last_name>", lastName.ToHtmlEncoded());
            subscriptionXml.AppendFormat("<email>{0}</email>", emailAddress);
            if (!string.IsNullOrEmpty(phone)) subscriptionXml.AppendFormat("<phone>{0}</phone>", phone.ToHtmlEncoded());
            subscriptionXml.AppendFormat("<organization>{0}</organization>", (organization != null) ? organization.ToHtmlEncoded() : "null");
            subscriptionXml.AppendFormat("<vat_number>{0}</vat_number>", (vatNumber != null) ? vatNumber.ToHtmlEncoded() : null);
            subscriptionXml.AppendFormat("<reference>{0}</reference>", newSystemId);
            if (!string.IsNullOrEmpty(shippingAddress)) subscriptionXml.AppendFormat("<address>{0}</address>", shippingAddress.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(shippingCity)) subscriptionXml.AppendFormat("<city>{0}</city>", shippingCity.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(shippingState)) subscriptionXml.AppendFormat("<state>{0}</state>", shippingState.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(shippingZip)) subscriptionXml.AppendFormat("<zip>{0}</zip>", shippingZip.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(shippingCountry)) subscriptionXml.AppendFormat("<country>{0}</country>", shippingCountry.ToHtmlEncoded());
            subscriptionXml.Append("</customer_attributes>");
            subscriptionXml.Append("<credit_card_attributes>");
            subscriptionXml.AppendFormat("<full_number>{0}</full_number>", fullNumber);
            subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", expirationMonth);
            subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", expirationYear);
            if (_cvvRequired) { subscriptionXml.AppendFormat("<cvv>{0}</cvv>", cvv); }
            if (!string.IsNullOrEmpty(billingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", billingAddress.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(billingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", billingCity.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(billingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", billingState.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(billingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", billingZip.ToHtmlEncoded());
            if (!string.IsNullOrEmpty(billingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", billingCountry.ToHtmlEncoded());
            subscriptionXml.Append("</credit_card_attributes>");
            if (!string.IsNullOrEmpty(couponCode)) { subscriptionXml.AppendFormat("<coupon_code>{0}</coupon_code>", couponCode); }
            if (componentId != int.MinValue)
            {
                subscriptionXml.Append(@"<components type=""array"">");
                subscriptionXml.Append("<component>");
                subscriptionXml.Append(string.Format("<component_id>{0}</component_id>", componentId));
                subscriptionXml.Append(string.Format("<allocated_quantity>{0}</allocated_quantity>", allocatedQuantity));
                subscriptionXml.Append("</component>");
                subscriptionXml.Append("</components>");
            }
            subscriptionXml.Append("</subscription>");
            // now make the request
            string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="subscription">The subscription to update credit card info for</param>
        /// <param name="creditCardAttributes">The attributes for the updated credit card</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(ISubscription subscription, ICreditCardAttributes creditCardAttributes)
        {
            if (subscription == null) throw new ArgumentNullException("subscription");
            return UpdateSubscriptionCreditCard(subscription.SubscriptionID, creditCardAttributes);
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the suscription to update</param>
        /// <param name="creditCardAttributes">The attributes for the update credit card</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(int subscriptionId, ICreditCardAttributes creditCardAttributes)
        {
            // make sure data is OK
            if (creditCardAttributes == null) throw new ArgumentNullException("creditCardAttributes");
            return UpdateTheSubscriptionCreditCard(subscriptionId, creditCardAttributes.FirstName, creditCardAttributes.LastName, creditCardAttributes.FullNumber, creditCardAttributes.ExpirationMonth, creditCardAttributes.ExpirationYear,
                                                creditCardAttributes.CVV, creditCardAttributes.BillingAddress, creditCardAttributes.BillingCity, creditCardAttributes.BillingState,
                                                creditCardAttributes.BillingZip, creditCardAttributes.BillingCountry);
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="subscription">The subscription to update credit card info for</param>
        /// <param name="fullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="expirationMonth">The expiration month of the credit card (optional - set to null if not required)</param>
        /// <param name="expirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="cvv">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="billingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="billingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="billingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="billingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="billingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(ISubscription subscription, string fullNumber, int? expirationMonth, int? expirationYear, string cvv,
                                                                 string billingAddress, string billingCity, string billingState, string billingZip, string billingCountry)
        {
            // make sure data is OK
            if (subscription == null) throw new ArgumentNullException("subscription");
            return UpdateTheSubscriptionCreditCard(subscription.SubscriptionID, string.Empty, string.Empty, fullNumber, expirationMonth, expirationYear, cvv, billingAddress, billingCity,
                                                billingState, billingZip, billingCountry);
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the suscription to update</param>
        /// <param name="fullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="expirationMonth">The expiration month of the credit card (optional - set to null if not required)</param>
        /// <param name="expirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="cvv">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="billingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="billingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="billingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="billingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="billingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(int subscriptionId, string fullNumber, int? expirationMonth, int? expirationYear, string cvv,
                                                                 string billingAddress, string billingCity, string billingState, string billingZip, string billingCountry)
        {

            // make sure data is OK
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");

            return UpdateTheSubscriptionCreditCard(subscriptionId, string.Empty, string.Empty, fullNumber, expirationMonth, expirationYear, cvv, billingAddress, billingCity,
                                                billingState, billingZip, billingCountry);
        }

        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the suscription to update</param>
        /// <param name="firstName">The billing first name (first name on the card)</param>
        /// <param name="lastName">The billing last name (last name on the card)</param>
        /// <param name="fullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="expirationMonth">The expiration month of the credit card (optional - set to null if not required)</param>
        /// <param name="expirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="cvv">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="billingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="billingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="billingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="billingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="billingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription UpdateSubscriptionCreditCard(int subscriptionId, string firstName, string lastName, string fullNumber, int? expirationMonth, int? expirationYear, string cvv,
                                                                 string billingAddress, string billingCity, string billingState, string billingZip, string billingCountry)
        {

            // make sure data is OK
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");

            return UpdateTheSubscriptionCreditCard(subscriptionId, firstName, lastName, fullNumber, expirationMonth, expirationYear, cvv, billingAddress, billingCity,
                                                billingState, billingZip, billingCountry);
        }

        /// <summary>
        /// Method to update the payment profile
        /// </summary>
        /// <param name="subscriptionId">The subscription to update</param>
        /// <param name="firstName">The billing first name</param>
        /// <param name="lastName">The billing last name</param>
        /// <param name="fullNumber">The credit card number</param>
        /// <param name="expirationMonth">The expiration month</param>
        /// <param name="expirationYear">The expiration year</param>
        /// <param name="cvv">The CVV as written on the back of the card</param>
        /// <param name="billingAddress">The billing address</param>
        /// <param name="billingCity">The billing city</param>
        /// <param name="billingState">The billing state</param>
        /// <param name="billingZip">The billing zip/postal code</param>
        /// <param name="billingCountry">The billing country</param>
        /// <returns>The updated subscription</returns>
        private ISubscription UpdateTheSubscriptionCreditCard(int subscriptionId, string firstName, string lastName, string fullNumber, int? expirationMonth, int? expirationYear, string cvv,
                                                                 string billingAddress, string billingCity, string billingState, string billingZip, string billingCountry)
        {

            // make sure data is OK
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");
            if (string.IsNullOrEmpty("FullNumber")) throw new ArgumentNullException("fullNumber");
            if (!string.IsNullOrWhiteSpace(cvv) && _cvvRequired && ((cvv.Length < 3) || (cvv.Length > 4))) throw new ArgumentException("CVV must be 3 or 4 digits", "cvv");

            // make sure subscription exists
            ISubscription existingSubscription = LoadSubscription(subscriptionId);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", nameof(subscriptionId));

            // create XML for creation of customer
            var subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.Append("<credit_card_attributes>");
            if (!string.IsNullOrEmpty(firstName)) subscriptionXml.AppendFormat("<first_name>{0}</first_name>", firstName);
            if (!string.IsNullOrEmpty(lastName)) subscriptionXml.AppendFormat("<last_name>{0}</last_name>", lastName);
            if (!string.IsNullOrEmpty(fullNumber)) subscriptionXml.AppendFormat("<full_number>{0}</full_number>", fullNumber);
            if (expirationMonth != null && expirationMonth.Value != int.MinValue) subscriptionXml.AppendFormat("<expiration_month>{0}</expiration_month>", expirationMonth);
            if (expirationYear != null && expirationYear.Value != int.MinValue) subscriptionXml.AppendFormat("<expiration_year>{0}</expiration_year>", expirationYear);
            if (_cvvRequired && !string.IsNullOrEmpty(cvv)) subscriptionXml.AppendFormat("<cvv>{0}</cvv>", cvv);
            if (!string.IsNullOrEmpty(billingAddress)) subscriptionXml.AppendFormat("<billing_address>{0}</billing_address>", billingAddress);
            if (!string.IsNullOrEmpty(billingCity)) subscriptionXml.AppendFormat("<billing_city>{0}</billing_city>", billingCity);
            if (!string.IsNullOrEmpty(billingState)) subscriptionXml.AppendFormat("<billing_state>{0}</billing_state>", billingState);
            if (!string.IsNullOrEmpty(billingZip)) subscriptionXml.AppendFormat("<billing_zip>{0}</billing_zip>", billingZip);
            if (!string.IsNullOrEmpty(billingCountry)) subscriptionXml.AppendFormat("<billing_country>{0}</billing_country>", billingCountry);
            subscriptionXml.Append("</credit_card_attributes>");
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Put, subscriptionXml.ToString());
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
        /// <param name="subscription">The subscription to update</param>
        /// <returns>The updated subscriptionn, null otherwise.</returns>
        public ISubscription UpdateSubscription(ISubscription subscription)
        {
            return UpdateSubscription(subscription.SubscriptionID, subscription.Product.Handle, subscription.Customer.SystemID, subscription.Customer.FirstName,
                subscription.Customer.LastName, subscription.Customer.Email, subscription.Customer.Phone, subscription.Customer.Organization, subscription.PaymentProfile.FullNumber, subscription.PaymentProfile.ExpirationMonth,
                subscription.PaymentProfile.ExpirationYear, string.Empty, subscription.PaymentProfile.BillingAddress, subscription.PaymentProfile.BillingCity, subscription.PaymentProfile.BillingState,
                subscription.PaymentProfile.BillingZip, subscription.PaymentProfile.BillingCountry);
        }

        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="subscription">The subscription to migrate</param>
        /// <param name="product">The product to migrate the subscription to</param>
        /// <param name="includeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="includeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns></returns>
        public ISubscription MigrateSubscriptionProduct(ISubscription subscription, IProduct product, bool includeTrial, bool includeInitialCharge)
        {
            // make sure data is OK
            if (subscription == null) throw new ArgumentNullException("subscription");
            if (product == null) throw new ArgumentNullException("product");
            return MigrateSubscriptionProduct(subscription.SubscriptionID, product.Handle, includeTrial, includeInitialCharge);
        }

        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="subscriptionId">The subscription to migrate</param>
        /// <param name="product">The product to migrate to</param>
        /// <param name="includeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="includeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns>The completed subscription if migrated successfully, null otherwise.</returns>
        public ISubscription MigrateSubscriptionProduct(int subscriptionId, IProduct product, bool includeTrial, bool includeInitialCharge)
        {
            // make sure data is OK
            if (product == null) throw new ArgumentNullException("product");
            return MigrateSubscriptionProduct(subscriptionId, product.Handle, includeTrial, includeInitialCharge);
        }

        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="subscription">The subscription to migrate</param>
        /// <param name="productHandle">The product handle of the product to migrate to</param>
        /// <param name="includeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="includeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns>The completed subscription if migrated successfully, null otherwise.</returns>
        public ISubscription MigrateSubscriptionProduct(ISubscription subscription, string productHandle, bool includeTrial, bool includeInitialCharge)
        {
            // make sure data is OK
            if (subscription == null) throw new ArgumentNullException("subscription");
            return MigrateSubscriptionProduct(subscription.SubscriptionID, productHandle, includeTrial, includeInitialCharge);
        }

        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="subscriptionId">The subscription to migrate</param>
        /// <param name="productHandle">The product handle of the product to migrate to</param>
        /// <param name="includeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="includeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns>The completed subscription if migrated successfully, null otherwise.</returns>
        public ISubscription MigrateSubscriptionProduct(int subscriptionId, string productHandle, bool includeTrial, bool includeInitialCharge)
        {
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException("productHandle");

            // make sure subscription exists
            ISubscription existingSubscription = LoadSubscription(subscriptionId);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", nameof(subscriptionId));

            // create XML for creation of customer
            StringBuilder migrationXml = new StringBuilder(GetXmlStringIfApplicable());
            migrationXml.Append("<migration>");
            migrationXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            if (includeTrial) { migrationXml.Append("<include_trial>1</include_trial>"); }
            if (includeInitialCharge) { migrationXml.Append("<include_initial_charge>1</include_initial_charge>"); }
            migrationXml.Append("</migration>");
            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}/migrations.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, migrationXml.ToString());
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
        /// <param name="subscription">The suscription to update</param>
        /// <param name="product">The new product</param>
        /// <param name="productChangeDelayed">Optional, determines if the product change should be done immediately or at the time of assessment.</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription EditSubscriptionProduct(ISubscription subscription, IProduct product, bool? productChangeDelayed = null)
        {
            // make sure data is OK
            if (subscription == null) throw new ArgumentNullException("subscription");
            if (product == null) throw new ArgumentNullException("product");
            return EditSubscriptionProduct(subscription.SubscriptionID, product.Handle, productChangeDelayed);
        }

        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="subscriptionId">The ID of the suscription to update</param>
        /// <param name="product">The new product</param>
        /// <param name="productChangeDelayed">Optional, determines if the product change should be done immediately or at the time of assessment.</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription EditSubscriptionProduct(int subscriptionId, IProduct product, bool? productChangeDelayed = null)
        {
            // make sure data is OK
            if (product == null) throw new ArgumentNullException("product");
            return EditSubscriptionProduct(subscriptionId, product.Handle, productChangeDelayed);
        }

        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="subscription">The suscription to update</param>
        /// <param name="productHandle">The handle to the new product</param>
        /// <param name="productChangeDelayed">Optional, determines if the product change should be done immediately or at the time of assessment.</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription EditSubscriptionProduct(ISubscription subscription, string productHandle, bool? productChangeDelayed = null)
        {
            // make sure data is OK
            if (subscription == null) throw new ArgumentNullException("subscription");
            return EditSubscriptionProduct(subscription.SubscriptionID, productHandle, productChangeDelayed);
        }

        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="subscriptionId">The ID of the suscription to update</param>
        /// <param name="productHandle">The handle to the new product</param>
        /// <param name="productChangeDelayed">Optional, determines if the product change should be done immediately or at the time of assessment.</param>
        /// <returns>The new subscription resulting from the change</returns>
        public ISubscription EditSubscriptionProduct(int subscriptionId, string productHandle, bool? productChangeDelayed = null)
        {
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");
            if (string.IsNullOrEmpty(productHandle)) throw new ArgumentNullException("productHandle");

            // make sure subscription exists
            ISubscription existingSubscription = LoadSubscription(subscriptionId);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", nameof(subscriptionId));

            // create XML for creation of customer
            var subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            if (productChangeDelayed != null)
            {
                //product_change_delayed
                subscriptionXml.AppendFormat("<product_change_delayed>{0}</product_change_delayed>", productChangeDelayed.Value.ToString().ToLowerInvariant());
            }
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Put, subscriptionXml.ToString());
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
        /// Change the delayed product, or cancel by setting it null
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription</param>
        /// <returns>The subscription</returns>
        public ISubscription CancelDelayedProductChange(int subscriptionId)
        {
            if (subscriptionId <= 0) throw new ArgumentNullException("subscriptionId");

            // create XML for creation of customer
            var subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<next_product_id>{0}</next_product_id>", string.Empty);
            subscriptionXml.Append("</subscription>");

            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Put, subscriptionXml.ToString());
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
        /// <param name="subscriptionId">The ID of the subscription to update</param>
        /// <param name="productHandle">The handle to the product (optional - set to null if not required)</param>
        /// <param name="systemId">The system ID for the customer (optional - set to Guid.Empty if not required)</param>
        /// <param name="firstName">The first name of the new customer (optional - set to null if not required)</param>
        /// <param name="lastName">The last name of the new customer (optional - set to null if not required)</param>
        /// <param name="emailAddress">The email address for the new customer (optional - set to null if not required)</param>
        /// <param name="phone">The phone number of the customer (optional - set to null if not required)</param>
        /// <param name="organization">The organization of the new customer (optional - set to null if not required)</param>
        /// <param name="fullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="expirationMonth">The expritation month of the credit card (optional - set to null if not required)</param>
        /// <param name="expirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="cvv">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="billingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="billingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="billingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="billingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="billingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The xml describing the new subsscription</returns>
        private ISubscription UpdateSubscription(int subscriptionId, string productHandle, string systemId, string firstName, string lastName, string emailAddress, string phone,
                                                 string organization, string fullNumber, int? expirationMonth, int? expirationYear,
                                                 string cvv, string billingAddress, string billingCity, string billingState, string billingZip,
                                                 string billingCountry)
        {
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");

            // make sure subscription exists
            ISubscription existingSubscription = LoadSubscription(subscriptionId);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", nameof(subscriptionId));

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            //if (!string.IsNullOrEmpty(ProductHandle) && existingSubscription.Product.Handle != ProductHandle)  
            subscriptionXml.AppendFormat("<product_handle>{0}</product_handle>", productHandle);
            if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName) || !string.IsNullOrEmpty(emailAddress) ||
                !string.IsNullOrEmpty(organization) || systemId != string.Empty)
            {
                subscriptionXml.Append("<customer_attributes>");
                //if (!string.IsNullOrEmpty(FirstName) && existingSubscription.Customer.FirstName != FirstName) 
                subscriptionXml.AppendFormat("<first_name>{0}</first_name>", firstName);
                //if (!string.IsNullOrEmpty(LastName) && existingSubscription.Customer.LastName != LastName) 
                subscriptionXml.AppendFormat("<last_name>{0}</last_name>", lastName);
                if (!string.IsNullOrEmpty(emailAddress) && existingSubscription.Customer.Email != emailAddress) subscriptionXml.AppendFormat("<email>{0}</email>", emailAddress);
                if (!string.IsNullOrEmpty(phone) && existingSubscription.Customer.Phone != phone) subscriptionXml.AppendFormat("<{0}>{1}</{2}>", CustomerAttributes.PhoneKey, phone, CustomerAttributes.PhoneKey);
                if (!string.IsNullOrEmpty(organization) && existingSubscription.Customer.Organization != organization) subscriptionXml.AppendFormat("<organization>{0}</organization>", HttpUtility.HtmlEncode(organization));
                if ((systemId != string.Empty) && (existingSubscription.Customer.SystemID != systemId)) subscriptionXml.AppendFormat("<reference>{0}</reference>", systemId);
                subscriptionXml.Append("</customer_attributes>");
            }

            if (!string.IsNullOrEmpty(fullNumber) || expirationMonth == null || expirationYear == null || !string.IsNullOrEmpty(cvv) ||
                !string.IsNullOrEmpty(billingAddress) || !string.IsNullOrEmpty(billingCity) || !string.IsNullOrEmpty(billingState) ||
                !string.IsNullOrEmpty(billingZip) || !string.IsNullOrEmpty(billingCountry))
            {

                StringBuilder paymentProfileXml = new StringBuilder();
                if ((!string.IsNullOrEmpty(fullNumber)) && (existingSubscription.PaymentProfile.FullNumber != fullNumber)) paymentProfileXml.AppendFormat("<full_number>{0}</full_number>", fullNumber);
                if ((expirationMonth == null) && (existingSubscription.PaymentProfile.ExpirationMonth != expirationMonth)) paymentProfileXml.AppendFormat("<expiration_month>{0}</expiration_month>", expirationMonth);
                if ((expirationYear == null) && (existingSubscription.PaymentProfile.ExpirationYear != expirationYear)) paymentProfileXml.AppendFormat("<expiration_year>{0}</expiration_year>", expirationYear);
                if (_cvvRequired && !string.IsNullOrEmpty(cvv)) paymentProfileXml.AppendFormat("<cvv>{0}</cvv>", cvv);
                if (!string.IsNullOrEmpty(billingAddress) && existingSubscription.PaymentProfile.BillingAddress != billingAddress) paymentProfileXml.AppendFormat("<billing_address>{0}</billing_address>", billingAddress);
                if (!string.IsNullOrEmpty(billingCity) && existingSubscription.PaymentProfile.BillingCity != billingCity) paymentProfileXml.AppendFormat("<billing_city>{0}</billing_city>", billingCity);
                if (!string.IsNullOrEmpty(billingState) && existingSubscription.PaymentProfile.BillingState != billingState) paymentProfileXml.AppendFormat("<billing_state>{0}</billing_state>", billingState);
                if (!string.IsNullOrEmpty(billingZip) && existingSubscription.PaymentProfile.BillingZip != billingZip) paymentProfileXml.AppendFormat("<billing_zip>{0}</billing_zip>", billingZip);
                if (!string.IsNullOrEmpty(billingCountry) && existingSubscription.PaymentProfile.BillingCountry != billingCountry) paymentProfileXml.AppendFormat("<billing_country>{0}</billing_country>", billingCountry);
                if (paymentProfileXml.Length > 0)
                {
                    subscriptionXml.AppendFormat("<credit_card_attributes>{0}</credit_card_attributes>", paymentProfileXml);
                }

            }
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions.{0}", GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.ToString());
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
        /// <param name="subscriptionId">The subscription to modify</param>
        /// <param name="nextBillingAt">The date to next bill the customer</param>
        /// <returns>Subscription if successful, null otherwise.</returns>
        public ISubscription UpdateBillingDateForSubscription(int subscriptionId, DateTime nextBillingAt)
        {
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");

            // make sure subscription exists
            ISubscription existingSubscription = LoadSubscription(subscriptionId);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", nameof(subscriptionId));

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<next_billing_at>{0}</next_billing_at>", nextBillingAt.ToString("o"));
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Put, subscriptionXml.ToString());
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
        /// <param name="subscriptionId">The subscription to modify</param>
        /// <param name="cancelAtEndOfPeriod">True if the subscription should cancel at the end of the current period</param>
        /// <param name="cancellationMessage">The reason for cancelling the subscription</param>
        /// <returns>Subscription if successful, null otherwise.</returns>
        public ISubscription UpdateDelayedCancelForSubscription(int subscriptionId, bool cancelAtEndOfPeriod, string cancellationMessage)
        {
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            subscriptionXml.AppendFormat("<cancel_at_end_of_period>{0}</cancel_at_end_of_period>", cancelAtEndOfPeriod ? "1" : "0");
            if (!String.IsNullOrEmpty(cancellationMessage)) { subscriptionXml.AppendFormat("<cancellation_message>{0}</cancellation_message>", cancellationMessage); }
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Put, subscriptionXml.ToString());
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
        /// <param name="subscriptionId">The ID of the subscription to modify.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool ResetSubscriptionBalance(int subscriptionId)
        {
            try
            {
                // make sure data is valid
                if (subscriptionId < 0) throw new ArgumentNullException("subscriptionId");
                // now make the request
                DoRequest(string.Format("subscriptions/{0}/reset_balance.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Put, string.Empty);
                return true;
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return false;
                throw;
            }
        }

        /// <summary>
        /// Update the collection method of the subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription of who's collection method should be updated</param>
        /// <param name="paymentCollectionMethod">The collection method to set</param>
        /// <returns>The full details of the updated subscription</returns>
        public ISubscription UpdatePaymentCollectionMethod(int subscriptionId, PaymentCollectionMethod paymentCollectionMethod)
        {
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");

            // make sure subscription exists
            ISubscription existingSubscription = LoadSubscription(subscriptionId);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", nameof(subscriptionId));

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());
            subscriptionXml.Append("<subscription>");
            var paymentCollectionMethodName = Enum.GetName(typeof(PaymentCollectionMethod), paymentCollectionMethod);
            if (paymentCollectionMethodName != null)
                subscriptionXml.AppendFormat("<payment_collection_method>{0}</payment_collection_method>", paymentCollectionMethodName.ToLowerInvariant());
            subscriptionXml.Append("</subscription>");
            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Put, subscriptionXml.ToString());
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
        /// This will place the subscription in the on_hold state and it will not renew.
        /// </summary>
        /// <param name="subscriptionId">The (chargify) id of the subscription</param>
        /// <param name="automaticResumeDate">The date the subscription will automatically resume, if applicable</param>
        /// <returns>The subscription data, if successful</returns>
        /// <remarks>https://reference.chargify.com/v1/subscriptions/hold-subscription</remarks>
        public ISubscription PauseSubscription(int subscriptionId, DateTime? automaticResumeDate = null)
        {
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");

            // create XML for creation of customer
            StringBuilder subscriptionXml = new StringBuilder(GetXmlStringIfApplicable());

            if (automaticResumeDate.HasValue)
            {
                subscriptionXml.Append("<hold>");
                subscriptionXml.AppendFormat("<automatically_resume_at>{0}</automatically_resume_at>", automaticResumeDate.Value.ToString("o"));
                subscriptionXml.Append("</hold>");
            }
            else
            {
                subscriptionXml.Clear();
            }

            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}/hold.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, subscriptionXml.Length > 0 ? subscriptionXml.ToString() : null);
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
        /// Resume a paused (on-hold) subscription. If the normal next renewal date has not passed, 
        /// the subscription will return to active and will renew on that date. Otherwise, it will 
        /// behave like a reactivation, setting the billing date to 'now' and charging the subscriber.
        /// </summary>
        /// <param name="subscriptionId">The (Chargify) id of the subscription</param>
        /// <returns>The subscription data, if successful</returns>
        /// <remarks>https://reference.chargify.com/v1/subscriptions/resume-subscription</remarks>
        public ISubscription ResumeSubscription(int subscriptionId)
        {
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");

            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}/resume.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, string.Empty);
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
        /// <param name="subscriptionId"></param>
        /// <param name="overrideDetails"></param>
        /// <returns>The details returned by Chargify</returns>
        public bool SetSubscriptionOverride(int subscriptionId, ISubscriptionOverride overrideDetails)
        {
            if (overrideDetails == null) throw new ArgumentNullException("overrideDetails");
            return SetSubscriptionOverride(subscriptionId, overrideDetails.ActivatedAt, overrideDetails.CanceledAt, overrideDetails.CancellationMessage, overrideDetails.ExpiresAt);
        }

        /// <summary>
        /// This API endpoint allows you to set certain subscription fields that are usually managed for you automatically. Some of the fields can be set via the normal Subscriptions Update API, but others can only be set using this endpoint.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="activatedAt"></param>
        /// <param name="canceledAt"></param>
        /// <param name="cancellationMessage"></param>
        /// <param name="expiresAt"></param>
        /// <returns>The details returned by Chargify</returns>
        public bool SetSubscriptionOverride(int subscriptionId, DateTime? activatedAt = null, DateTime? canceledAt = null, string cancellationMessage = null, DateTime? expiresAt = null)
        {
            try
            {
                // make sure data is valid
                if (activatedAt == null && canceledAt == null && cancellationMessage == null && expiresAt == null)
                {
                    throw new ArgumentException("No valid parameters provided");
                }

                // make sure that the SubscriptionID is unique
                if (LoadSubscription(subscriptionId) == null) throw new ArgumentException("No subscription found with that ID", "subscriptionId");

                // create XML for creation of a charge
                var overrideXml = new StringBuilder(GetXmlStringIfApplicable());
                overrideXml.Append("<subscription>");
                if (activatedAt.HasValue) { overrideXml.AppendFormat("<activated_at>{0}</activated_at>", activatedAt.Value.ToString("o")); }
                if (!string.IsNullOrEmpty(cancellationMessage)) { overrideXml.AppendFormat("<cancellation_message>{0}</cancellation_message>", HttpUtility.HtmlEncode(cancellationMessage)); }
                if (canceledAt.HasValue) { overrideXml.AppendFormat("<canceled_at>{0}</canceled_at>", canceledAt.Value.ToString("o")); }
                if (expiresAt.HasValue) { overrideXml.AppendFormat("<expires_at>{0}</expires_at>", expiresAt.Value.ToString("o")); }
                overrideXml.Append("</subscription>");

                // now make the request
                var result = DoRequest(string.Format("subscriptions/{0}/override.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Put, overrideXml.ToString());
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
        /// <param name="subscriptionId">SubscriptionID</param>
        /// <param name="productId">ProductID</param>
        /// <param name="includeCoupons">Should the migration preview consider subscription coupons?</param>
        /// <param name="includeInitialCharge">Should the migration preview consider a setup fee</param>
        /// <param name="includeTrial">Should the migration preview consider the product trial?</param>
        /// <returns></returns>
        public IMigration PreviewMigrateSubscriptionProduct(int subscriptionId, int productId, bool? includeTrial, bool? includeInitialCharge, bool? includeCoupons)
        {
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");
            if (productId == int.MinValue) throw new ArgumentNullException("productId");

            // make sure subscription exists
            ISubscription existingSubscription = LoadSubscription(subscriptionId);
            if (existingSubscription == null) throw new ArgumentException("Subscription not found", nameof(subscriptionId));

            // create XML for creation of customer
            StringBuilder migrationXml = new StringBuilder(GetXmlStringIfApplicable());
            migrationXml.Append("<migration>");
            migrationXml.AppendFormat("<product_id>{0}</product_id>", productId);
            if (includeTrial.HasValue) { migrationXml.Append(string.Format("<include_trial>{0}</include_trial>", includeTrial.Value ? "1" : "0")); }
            if (includeInitialCharge.HasValue) { migrationXml.Append(string.Format("<include_initial_charge>{0}</include_initial_charge>", includeInitialCharge.Value ? "1" : "0")); }
            if (includeCoupons.HasValue) { migrationXml.Append(string.Format("<include_coupons>{0}</include_coupons>", includeCoupons.Value ? "1" : "0")); }
            else
            {
                // Default is yes, if unspecified.
                migrationXml.Append("<include_coupons>1</include_coupons>");
            }
            migrationXml.Append("</migration>");
            try
            {
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}/migrations/preview.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, migrationXml.ToString());
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
        /// <param name="subscriptionId">Active Subscription</param>
        /// <param name="productId">Active Product</param>
        /// <returns>The migration preview data, if able. Null otherwise.</returns>
        public IMigration PreviewMigrateSubscriptionProduct(int subscriptionId, int productId)
        {
            return PreviewMigrateSubscriptionProduct(subscriptionId, productId, null, null, null);
        }

        /// <summary>
        /// Return a preview of charges for a subscription product migrations
        /// </summary>
        /// <param name="subscription">Active Subscription</param>
        /// <param name="product">Active Product</param>
        /// <returns>The migration preview data, if able. Null otherwise.</returns>
        public IMigration PreviewMigrateSubscriptionProduct(ISubscription subscription, IProduct product)
        {
            if (subscription == null) throw new ArgumentNullException("subscription");
            if (product == null) throw new ArgumentNullException("product");
            return PreviewMigrateSubscriptionProduct(subscription.SubscriptionID, product.ID);
        }
        #endregion

        #region Coupons

        /// <summary>
        /// Method for retrieving information about a coupon using the ID of that coupon.
        /// </summary>
        /// <param name="productFamilyId">The ID of the product family that the coupon belongs to</param>
        /// <param name="couponId">The ID of the coupon</param>
        /// <returns>The object if found, null otherwise.</returns>
        public ICoupon LoadCoupon(int productFamilyId, int couponId)
        {
            try
            {
                // make sure data is valid
                if (productFamilyId < 0) throw new ArgumentException("Invalid ProductFamilyID");
                if (couponId < 0) throw new ArgumentException("Invalid CouponID");
                // now make the request
                string response = DoRequest(string.Format("product_families/{0}/coupons/{1}.{2}", productFamilyId, couponId, GetMethodExtension()));
                // change the response to the object
                return response.ConvertResponseTo<Coupon>("coupon");

            }
            catch (ChargifyException cex)
            {
                // Throw if anything but not found, since not found is telling us that it's working correctly
                // but that there just isn't a coupon with that ID.
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Method for retrieving information about a coupon using the ID of that coupon.
        /// </summary>
        /// <param name="ProductFamilyID">The ID of the product family that the coupon belongs to</param>
        /// <returns>A dictionary of objects if found, empty collection otherwise.</returns>
        public IDictionary<int, ICoupon> GetAllCoupons(int ProductFamilyID)
        {
            var coupons = new Dictionary<int, ICoupon>();

            try
            {
                // make sure data is valid
                if (ProductFamilyID < 0) throw new ArgumentException("Invalid ProductFamilyID");
                // now make the request
                string response = this.DoRequest(string.Format("product_families/{0}/coupons.{1}", ProductFamilyID, GetMethodExtension()));

                if (response.IsXml())
                {
                    // now build a product list based on response XML
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(response); // get the XML into an XML document
                    if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                    // loop through the child nodes of this node

                    foreach (XmlNode elementNode in doc.ChildNodes)
                    {
                        if (elementNode.Name == "coupons")
                        {
                            foreach (XmlNode couponNode in elementNode.ChildNodes)
                            {
                                if (couponNode.Name == "coupon")
                                {
                                    ICoupon LoadedCoupon = new Coupon(couponNode);
                                    if (!coupons.ContainsKey(LoadedCoupon.ID))
                                    {
                                        coupons.Add(LoadedCoupon.ID, LoadedCoupon);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Duplicate Coupon ID values detected");
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
                        if ((array.Items[i] as JsonObject).ContainsKey("coupon"))
                        {
                            JsonObject couponObj = (array.Items[i] as JsonObject)["coupon"] as JsonObject;
                            ICoupon loadedCooupon = new Coupon(couponObj);
                            if (!coupons.ContainsKey(loadedCooupon.ID))
                            {
                                coupons.Add(loadedCooupon.ID, loadedCooupon);
                            }
                            else
                            {
                                throw new InvalidOperationException("Duplicate Coupon ID values detected");
                            }
                        }
                    }
                }
            }
            catch (ChargifyException cex)
            {
                // Throw if anything but not found, since not found is telling us that it's working correctly
                // but that there just isn't a coupon with that ID.
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw cex;
            }

            return coupons;
        }

        /// <summary>
        /// Method for retrieving information about a coupon usage using the ID of that coupon.
        /// </summary>
        /// <param name="CouponID">The ID of the coupon</param>
        /// <returns>The object if found, null otherwise.</returns>
        public IDictionary<int, ICouponUsage> GetCouponUsage(int CouponID)
        {
            var coupons = new Dictionary<int, ICouponUsage>();

            try
            {
                // make sure data is valid
                if (CouponID < 0) throw new ArgumentException("Invalid CouponID");
                // now make the request
                string response = this.DoRequest(string.Format("coupons/{0}/usage.{1}", CouponID, GetMethodExtension()));

                if (response.IsXml())
                {
                    // now build a product list based on response XML
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(response); // get the XML into an XML document
                    if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                    // loop through the child nodes of this node

                    foreach (XmlNode elementNode in doc.ChildNodes)
                    {
                        if (elementNode.Name == "objects")
                        {
                            foreach (XmlNode couponNode in elementNode.ChildNodes)
                            {
                                if (couponNode.Name == "object")
                                {
                                    ICouponUsage LoadedCoupon = new CouponUsage(couponNode);
                                    if (!coupons.ContainsKey(LoadedCoupon.ProductId))
                                    {
                                        coupons.Add(LoadedCoupon.ProductId, LoadedCoupon);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Duplicate Product ID values detected");
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
                        if ((array.Items[i] as JsonObject).ContainsKey("name"))
                        {
                            JsonObject couponUsageObj = (array.Items[i] as JsonObject) as JsonObject;
                            ICouponUsage loadedCoouponUsage = new CouponUsage(couponUsageObj);
                            if (!coupons.ContainsKey(loadedCoouponUsage.ProductId))
                            {
                                coupons.Add(loadedCoouponUsage.ProductId, loadedCoouponUsage);
                            }
                            else
                            {
                                throw new InvalidOperationException("Duplicate Coupon ID values detected");
                            }
                        }
                    }
                }
            }
            catch (ChargifyException cex)
            {
                // Throw if anything but not found, since not found is telling us that it's working correctly
                // but that there just isn't a coupon with that ID.
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw cex;
            }

            return coupons;
        }

        /// <summary>
        /// Retrieve the coupon corresponding to the coupon code, useful for coupon validation.
        /// </summary>
        /// <param name="productFamilyId">The ID of the product family the coupon belongs to</param>
        /// <param name="couponCode">The code used to represent the coupon</param>
        /// <returns>The object if found, otherwise null.</returns>
        public ICoupon FindCoupon(int productFamilyId, string couponCode)
        {
            try
            {
                string response = DoRequest(string.Format("product_families/{0}/coupons/find.{1}?code={2}", productFamilyId, GetMethodExtension(), couponCode));
                // change the response to the object
                return response.ConvertResponseTo<Coupon>("coupon");
            }
            catch (ChargifyException cex)
            {
                // Throw if anything but not found, since not found is telling us that it's working correctly
                // but that there just isn't a coupon with that code.
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Method to add a coupon to a subscription using the API
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to modify</param>
        /// <param name="couponCode">The code of the coupon to apply to the subscription</param>
        /// <returns>The subscription details if successful, null otherwise.</returns>
        public ISubscription AddCoupon(int subscriptionId, string couponCode)
        {
            // make sure that the SubscriptionID is unique
            if (LoadSubscription(subscriptionId) == null) throw new ArgumentException("Not an SubscriptionID", "subscriptionId");
            if (string.IsNullOrEmpty(couponCode)) throw new ArgumentException("Coupon code is empty", "couponCode");
            string response = DoRequest(string.Format("subscriptions/{0}/add_coupon.{1}?code={2}", subscriptionId, GetMethodExtension(), couponCode), HttpRequestMethod.Post, null);
            // change the response to the object            
            return response.ConvertResponseTo<Subscription>("subscription");
        }

        /// <summary>
        /// Method to remove a coupon to a subscription using the API
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify</param>
        /// <param name="CouponCode">The code of the coupon to remove from the subscription</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveCoupon(int SubscriptionID, string CouponCode)
        {
            // make sure that the SubscriptionID is unique
            if (this.LoadSubscription(SubscriptionID) == null) throw new ArgumentException("Not an SubscriptionID", nameof(SubscriptionID));
            if (string.IsNullOrEmpty(CouponCode)) throw new ArgumentException("Coupon code is empty", nameof(CouponCode));
            try
            {
                string response = this.DoRequest(string.Format("subscriptions/{0}/remove_coupon.{1}?code={2}", SubscriptionID, GetMethodExtension(), CouponCode), HttpRequestMethod.Delete, null);

                // change the response to the object            
                return response.Contains("Coupon removed");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Create a new one-time credit
        /// </summary>
        /// <param name="coupon">The coupon parameters</param>
        /// <param name="productFamilyId">The ID of the product family to add this coupon to.</param>
        /// <returns>The object if successful, null otherwise.</returns>
        public ICoupon CreateCoupon(ICoupon coupon, int productFamilyId)
        {
            if (coupon == null) throw new ArgumentNullException("coupon");
            string xml = BuildCouponXml(productFamilyId, coupon.Name, coupon.Code, coupon.Description, coupon.Amount, coupon.Percentage, coupon.AllowNegativeBalance,
                coupon.IsRecurring, coupon.DurationPeriodCount, coupon.EndDate);

            string response = DoRequest(string.Format("coupons.{0}", GetMethodExtension()), HttpRequestMethod.Post, xml);
            // change the response to the object
            return response.ConvertResponseTo<Coupon>("coupon");
        }

        /// <summary>
        /// Update an existing coupon
        /// </summary>
        /// <param name="coupon">Coupon object</param>
        /// <returns>The updated coupon if successful, null otherwise.</returns>
        public ICoupon UpdateCoupon(ICoupon coupon)
        {
            if (coupon == null) throw new ArgumentNullException(nameof(coupon));
            if (coupon.ProductFamilyID <= 0) throw new ArgumentOutOfRangeException(nameof(coupon), "Coupon.ProductFamilyID must be > 0");
            if (coupon.ID <= 0) throw new ArgumentOutOfRangeException(nameof(coupon), "Coupon ID is not valid");

            string xml = BuildCouponXml(coupon.ProductFamilyID, coupon.Name, coupon.Code, coupon.Description, coupon.Amount, coupon.Percentage, coupon.AllowNegativeBalance,
                coupon.IsRecurring, coupon.DurationPeriodCount, coupon.EndDate);

            string response = DoRequest(string.Format("coupons/{0}.{1}", coupon.ID, GetMethodExtension()), HttpRequestMethod.Put, xml);
            // change the response to the object
            return response.ConvertResponseTo<Coupon>("coupon");
        }

        /// <summary>
        /// Builds the coupon XML based on all the coupon values entered.
        /// </summary>
        /// <param name="productFamilyId">The id of the product family the coupon should belong to</param>
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
        private string BuildCouponXml(int productFamilyId, string name, string code, string description, decimal amount, int percentage, bool allowNegativeBalance,
            bool recurring, int durationPeriodCount, DateTime endDate)
        {
            // make sure data is valid
            //if (id <= 0 && !id.Equals(int.MinValue)) throw new ArgumentOutOfRangeException("id", id, "id must be > 0 if specified.");
            // Don't use ID here, since it's only being used to build the URL
            if (productFamilyId < 0 && !productFamilyId.Equals(int.MinValue)) throw new ArgumentOutOfRangeException("productFamilyId", productFamilyId, "Product Family must be >= 0");
            if (amount < 0) throw new ArgumentNullException("amount");
            if (percentage < 0) throw new ArgumentNullException("percentage");
            if (amount > 0 && percentage > 0) throw new ArgumentException("Only one of amount or percentage can have a value > 0");
            if (percentage > 100) throw new ArgumentOutOfRangeException("percentage", percentage, "percentage must be between 1 and 100");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
            if (!recurring && durationPeriodCount > 0)
                throw new ArgumentOutOfRangeException("durationPeriodCount", durationPeriodCount, "duration period count must be 0 if not recurring");

            // create XML for creation of a credit
            StringBuilder couponXml = new StringBuilder(GetXmlStringIfApplicable());
            couponXml.Append("<coupon>");
            couponXml.AppendFormat("<name>{0}</name>", HttpUtility.HtmlEncode(name));
            couponXml.AppendFormat("<code>{0}</code>", code);
            if (!String.IsNullOrEmpty(description)) couponXml.AppendFormat("<description>{0}</description>", HttpUtility.HtmlEncode(description));
            if (amount > 0) couponXml.AppendFormat("<amount>{0}</amount>", amount.ToChargifyCurrencyFormat());
            if (percentage > 0) couponXml.AppendFormat("<percentage>{0}</percentage>", percentage);
            couponXml.AppendFormat("<allow_negative_balance>{0}</allow_negative_balance>", allowNegativeBalance.ToString().ToLower());
            couponXml.AppendFormat("<recurring>{0}</recurring>", recurring.ToString().ToLower());
            if (recurring)
            {
                if (durationPeriodCount > 0)
                {
                    couponXml.AppendFormat("<duration_period_count>{0}</duration_period_count>", durationPeriodCount);
                }
                else
                {
                    couponXml.Append("<duration_period_count />");
                }
            }
            if (!endDate.Equals(DateTime.MinValue)) couponXml.AppendFormat("<end_date>{0}</end_date>", endDate.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            if (productFamilyId > 0) couponXml.AppendFormat("<product_family_id>{0}</product_family_id>", productFamilyId);
            couponXml.Append("</coupon>");
            return couponXml.ToString();

        }
        #endregion

        #region One-Time Charges

        /// <summary>
        /// Create a new one-time charge
        /// </summary>
        /// <param name="subscriptionId">The subscription that will be charged</param>
        /// <param name="charge">The charge parameters</param>
        /// <returns></returns>
        public ICharge CreateCharge(int subscriptionId, ICharge charge)
        {
            // make sure data is valid
            if (charge == null) throw new ArgumentNullException("charge");
            return CreateCharge(subscriptionId, charge.Amount, charge.Memo);
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
        /// <param name="subscriptionId">The subscription that will be charged</param>
        /// <param name="amount">The amount to charge the customer</param>
        /// <param name="memo">A description of the charge</param>
        /// <param name="delayCharge">(Optional) Should the charge be billed during the next assessment? Default = false</param>
        /// <param name="useNegativeBalance">(Optional) Should the subscription balance be taken into consideration? Default = true</param>
        /// <returns>The charge details</returns>
        public ICharge CreateCharge(int subscriptionId, decimal amount, string memo, bool useNegativeBalance = false, bool delayCharge = false)
        {
            // make sure data is valid
            if (amount < 0) throw new ArgumentNullException(nameof(amount)); // Chargify will throw a 422 if a negative number is in this field.
            if (string.IsNullOrEmpty(memo)) throw new ArgumentNullException(nameof(memo));
            // make sure that the SubscriptionID is unique
            if (LoadSubscription(subscriptionId) == null) throw new ArgumentException("Not an SubscriptionID", nameof(subscriptionId));
            // create XML for creation of a charge
            StringBuilder chargeXml = new StringBuilder(GetXmlStringIfApplicable());
            chargeXml.Append("<charge>");
            chargeXml.AppendFormat("<amount>{0}</amount>", amount.ToChargifyCurrencyFormat());
            chargeXml.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo));
            chargeXml.AppendFormat("<delay_capture>{0}</delay_capture>", delayCharge ? "1" : "0");
            chargeXml.AppendFormat("<use_negative_balance>{0}</use_negative_balance>", !useNegativeBalance ? "1" : "0");
            chargeXml.Append("</charge>");
            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/charges.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, chargeXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Charge>("charge");
        }

        #endregion

        #region One-Time Credits

        /// <summary>
        /// Create a new one-time credit
        /// </summary>
        /// <param name="subscriptionId">The subscription that will be credited</param>
        /// <param name="credit">The credit parameters</param>
        /// <returns>The object if successful, null otherwise.</returns>
        public ICredit CreateCredit(int subscriptionId, ICredit credit)
        {
            // make sure data is valid
            if (credit == null) throw new ArgumentNullException("credit");
            return CreateCredit(subscriptionId, credit.Amount, credit.Memo);
        }

        /// <summary>
        /// Create a new one-time credit
        /// </summary>
        /// <param name="subscriptionId">The subscription that will be credited</param>
        /// <param name="amount">The amount to credit the customer</param>
        /// <param name="memo">A note regarding the reason for the credit</param>
        /// <returns>The object if successful, null otherwise.</returns>
        public ICredit CreateCredit(int subscriptionId, decimal amount, string memo)
        {
            // make sure data is valid
            if (amount < 0) throw new ArgumentNullException(nameof(amount));
            if (string.IsNullOrEmpty(memo)) throw new ArgumentNullException(nameof(memo));
            // make sure that the SubscriptionID is unique
            if (LoadSubscription(subscriptionId) == null) throw new ArgumentException("Not an SubscriptionID", nameof(subscriptionId));
            // create XML for creation of a credit
            StringBuilder creditXml = new StringBuilder(GetXmlStringIfApplicable());
            creditXml.Append("<credit>");
            creditXml.AppendFormat("<amount>{0}</amount>", amount.ToChargifyCurrencyFormat());
            creditXml.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo));
            creditXml.Append("</credit>");
            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/credits.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, creditXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Credit>("credit");
        }
        #endregion

        #region Components

        /// <summary>
        /// Method to update the allocated amount of a component for a subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to modify the allocation for</param>
        /// <param name="componentId">The ID of the component</param>
        /// <param name="newAllocatedQuantity">The amount of component to allocate to the subscription</param>
        /// <returns>The ComponentAttributes object with UnitBalance filled in, null otherwise.</returns>
        [Obsolete("Use CreateComponentAllocation() Instead")]
        public IComponentAttributes UpdateComponentAllocationForSubscription(int subscriptionId, int componentId, int newAllocatedQuantity)
        {
            // make sure data is valid
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");
            if (componentId == int.MinValue) throw new ArgumentNullException("componentId");
            if (newAllocatedQuantity < 0) throw new ArgumentOutOfRangeException("newAllocatedQuantity");
            // create XML for change of allocation
            StringBuilder allocationXml = new StringBuilder(GetXmlStringIfApplicable());
            allocationXml.Append("<component>");
            allocationXml.AppendFormat("<allocated_quantity type=\"integer\">{0}</allocated_quantity>", newAllocatedQuantity);
            allocationXml.Append("</component>");
            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/components/{1}.{2}", subscriptionId, componentId, GetMethodExtension()), HttpRequestMethod.Put, allocationXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<ComponentAttributes>("component");
        }

        /// <summary>
        /// Method to retrieve the current information (including allocation) of a component against a subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription in question</param>
        /// <param name="componentId">The ID of the component</param>
        /// <returns>The ComponentAttributes object, null otherwise.</returns>
        public IComponentAttributes GetComponentInfoForSubscription(int subscriptionId, int componentId)
        {
            // make sure data is valid
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");
            if (componentId == int.MinValue) throw new ArgumentNullException("componentId");
            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/components/{1}.{2}", subscriptionId, componentId, GetMethodExtension()));
            // change the response to the object
            return response.ConvertResponseTo<ComponentAttributes>("component");
        }

        /// <summary>
        /// Returns all components "attached" to that subscription.
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to query about</param>
        /// <returns>A dictionary of components, if applicable.</returns>
        public IDictionary<int, IComponentAttributes> GetComponentsForSubscription(int subscriptionId)
        {
            // make sure data is valid
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");

            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/components.{1}", subscriptionId, GetMethodExtension()));
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
                                IComponentAttributes loadedComponent = new ComponentAttributes(componentNode);
                                if (!retValue.ContainsKey(loadedComponent.ComponentID))
                                {
                                    retValue.Add(loadedComponent.ComponentID, loadedComponent);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("component"))
                    {
                        JsonObject componentObj = (array.Items[i] as JsonObject)["component"] as JsonObject;
                        IComponentAttributes loadedComponent = new ComponentAttributes(componentObj);
                        if (!retValue.ContainsKey(loadedComponent.ComponentID))
                        {
                            retValue.Add(loadedComponent.ComponentID, loadedComponent);
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
        /// <param name="chargifyId">The product family ID</param>
        /// <param name="includeArchived">Filter flag for archived components</param>
        /// <returns>A dictionary of components if there are results, null otherwise.</returns>
        public IDictionary<int, IComponentInfo> GetComponentsForProductFamily(int chargifyId, bool includeArchived)
        {
            // make sure data is valid
            if (chargifyId == int.MinValue) throw new ArgumentNullException("chargifyId");

            // now make the request
            string response = DoRequest(string.Format("product_families/{0}/components.{1}?include_archived={2}", chargifyId, GetMethodExtension(), includeArchived ? "1" : "0"));
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
                                IComponentInfo loadedComponent = new ComponentInfo(componentNode);
                                if (!retValue.ContainsKey(loadedComponent.ID))
                                {
                                    retValue.Add(loadedComponent.ID, loadedComponent);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("component"))
                    {
                        JsonObject componentObj = (array.Items[i] as JsonObject)["component"] as JsonObject;
                        IComponentInfo loadedComponent = new ComponentInfo(componentObj);
                        if (!retValue.ContainsKey(loadedComponent.ID))
                        {
                            retValue.Add(loadedComponent.ID, loadedComponent);
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
        /// <param name="chargifyId">The product family ID</param>
        /// <returns>A dictionary of components if there are results, null otherwise.</returns>
        public IDictionary<int, IComponentInfo> GetComponentsForProductFamily(int chargifyId)
        {
            return GetComponentsForProductFamily(chargifyId, false);
        }

        /// <summary>
        /// Method for getting a list of component usages for a specific subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription ID to examine</param>
        /// <param name="componentId">The ID of the component to examine</param>
        /// <returns>A dictionary of usages if there are results, null otherwise.</returns>
        public IDictionary<string, IComponent> GetComponentList(int subscriptionId, int componentId)
        {
            // make sure data is valid
            if (subscriptionId == int.MinValue) throw new ArgumentNullException("subscriptionId");
            if (componentId == int.MinValue) throw new ArgumentNullException("componentId");
            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/components/{1}/usages.{2}", subscriptionId, componentId, GetMethodExtension()));
            var retValue = new Dictionary<string, IComponent>();
            if (response.IsXml())
            {
                // now build a product list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response); // get the XML into an XML document
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "usages")
                    {
                        foreach (XmlNode usageNode in elementNode.ChildNodes)
                        {
                            if (usageNode.Name == "usage")
                            {
                                IComponent loadedComponent = new Component(usageNode);
                                if (!retValue.ContainsKey(loadedComponent.ID))
                                {
                                    retValue.Add(loadedComponent.ID, loadedComponent);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("component"))
                    {
                        JsonObject componentObj = (array.Items[i] as JsonObject)["component"] as JsonObject;
                        IComponent loadedComponent = new Component(componentObj);
                        if (!retValue.ContainsKey(loadedComponent.ID))
                        {
                            retValue.Add(loadedComponent.ID, loadedComponent);
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
        /// <param name="subscriptionId">The subscriptionID to modify</param>
        /// <param name="componentId">The ID of the (metered or quantity) component to add a usage of</param>
        /// <param name="quantity">The number of usages to add</param>
        /// <param name="memo">The memo for the usage</param>
        /// <returns>The usage added if successful, otherwise null.</returns>
        public IUsage AddUsage(int subscriptionId, int componentId, int quantity, string memo)
        {
            // Chargify DOES currently allow a negative value for "quantity", so allow users to call this method that way.
            //if (Quantity < 0) throw new ArgumentNullException("Quantity");
            if (string.IsNullOrEmpty(memo)) throw new ArgumentNullException("memo");
            // make sure that the SubscriptionID is unique
            if (LoadSubscription(subscriptionId) == null) throw new ArgumentException("Not an SubscriptionID", "subscriptionId");
            // create XML for addition of usage
            StringBuilder usageXml = new StringBuilder(GetXmlStringIfApplicable());
            usageXml.Append("<usage>");
            usageXml.AppendFormat("<quantity>{0}</quantity>", quantity);
            usageXml.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo));
            usageXml.Append("</usage>");
            string response = DoRequest(string.Format("subscriptions/{0}/components/{1}/usages.{2}", subscriptionId, componentId, GetMethodExtension()), HttpRequestMethod.Post, usageXml.ToString());
            // change the response to the object            
            return response.ConvertResponseTo<Usage>("usage");
        }

        /// <summary>
        /// Method for turning on or off a component
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to modify</param>
        /// <param name="componentId">The ID of the component (on/off only) to modify</param>
        /// <param name="setEnabled">True if wanting to turn the component "on", false otherwise.</param>
        /// <returns>IComponentAttributes object if successful, null otherwise.</returns>
        [Obsolete("Use CreateComponentAllocation() Instead")]
        public IComponentAttributes SetComponent(int subscriptionId, int componentId, bool setEnabled)
        {
            try
            {
                if (componentId == int.MinValue) throw new ArgumentException("Not an ComponentID", "componentId");
                // make sure that the SubscriptionID is unique
                if (LoadSubscription(subscriptionId) == null) throw new ArgumentException("Not an SubscriptionID", "subscriptionId");
                // create XML for addition of usage
                StringBuilder componentXml = new StringBuilder(GetXmlStringIfApplicable());
                componentXml.Append("<component>");
                componentXml.AppendFormat("<enabled>{0}</enabled>", setEnabled.ToString(CultureInfo.InvariantCulture));
                componentXml.Append("</component>");
                string response = DoRequest(string.Format("subscriptions/{0}/components/{1}.{2}", subscriptionId, componentId, GetMethodExtension()), HttpRequestMethod.Put, componentXml.ToString());
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
        /// <param name="subscriptionId">The subscriptionID to scope this request</param>
        /// <param name="componentId">The componentID to scope this request</param>
        /// <param name="page">Pass an integer in the page parameter via the query string to access subsequent pages of 50 transactions</param>
        /// <returns>A dictionary of allocation objects keyed by ComponentID, or null.</returns>
        public IDictionary<int, List<IComponentAllocation>> GetAllocationListForSubscriptionComponent(int subscriptionId, int componentId, int? page = 0)
        {
            // make sure data is valid
            if (subscriptionId == int.MinValue) throw new ArgumentNullException(nameof(subscriptionId));
            if (page.HasValue && page.Value < 0) throw new ArgumentOutOfRangeException(nameof(page), "Page number must be a positive integer");

            try
            {
                string qs = string.Empty;
                // Add the request options to the query string
                if (page != null && page.Value > 0) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", page); }

                // now make the request
                string url = string.Format("subscriptions/{0}/components/{1}/allocations.{2}", subscriptionId, componentId, GetMethodExtension());
                if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
                string response = DoRequest(url);
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

                            if (!retValue.ContainsKey(componentId) && childComponentAllocations.Count > 0)
                            {
                                retValue.Add(componentId, childComponentAllocations);
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
                        var jsonObject = array.Items[i] as JsonObject;
                        if (jsonObject != null && jsonObject.ContainsKey(ComponentAllocation.AllocationRootKey))
                        {
                            JsonObject componentObj = (array.Items[i] as JsonObject)[ComponentAllocation.AllocationRootKey] as JsonObject;
                            IComponentAllocation loadedComponentAllocation = new ComponentAllocation(componentObj);
                            childComponentAllocations.Add(loadedComponentAllocation);
                        }
                    }

                    if (!retValue.ContainsKey(componentId) && childComponentAllocations.Count > 0)
                    {
                        retValue.Add(componentId, childComponentAllocations);
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
        }

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="componentId"></param>
        /// <param name="allocation"></param>
        /// <returns></returns>
        public IComponentAllocation CreateComponentAllocation(int subscriptionId, int componentId, ComponentAllocation allocation)
        {
            if (allocation == null) throw new ArgumentNullException("allocation");
            return CreateComponentAllocation(subscriptionId, componentId, allocation.Quantity, allocation.Memo, allocation.UpgradeScheme, allocation.DowngradeScheme);
        }

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to apply this quantity allocation to</param>
        /// <param name="componentId">The ID of the component to apply this quantity allocation to</param>
        /// <param name="quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <returns></returns>
        public IComponentAllocation CreateComponentAllocation(int subscriptionId, int componentId, int quantity)
        {
            return CreateComponentAllocation(subscriptionId, componentId, quantity, string.Empty, ComponentUpgradeProrationScheme.Unknown, ComponentDowngradeProrationScheme.Unknown);
        }

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to apply this quantity allocation to</param>
        /// <param name="componentId">The ID of the component to apply this quantity allocation to</param>
        /// <param name="quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <param name="memo">(optional) A memo to record along with the allocation</param>
        /// <returns></returns>
        public IComponentAllocation CreateComponentAllocation(int subscriptionId, int componentId, int quantity, string memo)
        {
            return CreateComponentAllocation(subscriptionId, componentId, quantity, memo, ComponentUpgradeProrationScheme.Unknown, ComponentDowngradeProrationScheme.Unknown);
        }

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to apply this quantity allocation to</param>
        /// <param name="componentId">The ID of the component to apply this quantity allocation to</param>
        /// <param name="quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <param name="memo">(optional) A memo to record along with the allocation</param>
        /// <param name="upgradeScheme">(optional) The scheme used if the proration is an upgrade. Defaults to the site setting if one is not provided.</param>
        /// <param name="downgradeScheme">(optional) The scheme used if the proration is a downgrade. Defaults to the site setting if one is not provided.</param>
        /// <returns>The component allocation object, null otherwise.</returns>
        public IComponentAllocation CreateComponentAllocation(int subscriptionId, int componentId, int quantity, string memo, ComponentUpgradeProrationScheme upgradeScheme, ComponentDowngradeProrationScheme downgradeScheme)
        {
            try
            {
                string xml = BuildComponentAllocationXml(quantity, memo, upgradeScheme, downgradeScheme);

                // perform the request, keep the response
                string response = DoRequest(string.Format("subscriptions/{0}/components/{1}/allocations.{2}", subscriptionId, componentId, GetMethodExtension()), HttpRequestMethod.Post, xml);

                // change the response to the object
                return response.ConvertResponseTo<ComponentAllocation>("allocation");
            }
            catch (ChargifyException cEx)
            {
                if (cEx.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Constructs the XML needed to create a component allocation
        /// </summary>
        /// <param name="quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <param name="memo">(optional) A memo to record along with the allocation</param>
        /// <param name="upgradeScheme">(optional) The scheme used if the proration is an upgrade. Defaults to the site setting if one is not provided.</param>
        /// <param name="downgradeScheme">(optional) The scheme used if the proration is a downgrade. Defaults to the site setting if one is not provided.</param>
        /// <returns>The formatted XML</returns>
        private string BuildComponentAllocationXml(int quantity, string memo, ComponentUpgradeProrationScheme upgradeScheme, ComponentDowngradeProrationScheme downgradeScheme)
        {
            // make sure data is valid
            if (quantity < 0 && !quantity.Equals(int.MinValue)) throw new ArgumentOutOfRangeException("quantity", quantity, "Quantity must be valid");

            // create XML for creation of a ComponentAllocation
            StringBuilder componentAllocationXml = new StringBuilder(GetXmlStringIfApplicable());
            componentAllocationXml.Append("<allocation>");
            componentAllocationXml.Append(string.Format("<quantity>{0}</quantity>", quantity));
            if (!string.IsNullOrEmpty(memo)) { componentAllocationXml.Append(string.Format("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo))); }
            if (upgradeScheme != ComponentUpgradeProrationScheme.Unknown)
            {
                var componentUpgradeProrationSchemeName = Enum.GetName(typeof(ComponentUpgradeProrationScheme), upgradeScheme);
                if (componentUpgradeProrationSchemeName != null)
                    componentAllocationXml.Append(string.Format("<proration_upgrade_scheme>{0}</proration_upgrade_scheme>", componentUpgradeProrationSchemeName.ToLowerInvariant().Replace("_", "-")));
            }
            if (downgradeScheme != ComponentDowngradeProrationScheme.Unknown)
            {
                var componentDowngradeProrationSchemeName = Enum.GetName(typeof(ComponentDowngradeProrationScheme), downgradeScheme);
                if (componentDowngradeProrationSchemeName != null)
                    componentAllocationXml.Append(string.Format("<proration_downgrade_scheme>{0}</proration_downgrade_scheme>", componentDowngradeProrationSchemeName.ToLowerInvariant().Replace("_", "-")));
            }
            componentAllocationXml.Append("</allocation>");
            return componentAllocationXml.ToString();

        }
        #endregion

        #region Transactions
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="descending">Should the results in in descending order?</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(bool descending = true)
        {
            return GetTransactionList(int.MinValue, int.MinValue, null, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue, descending);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="descending">Should the results in in descending order?</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(List<TransactionType> kinds, bool descending = true)
        {
            return GetTransactionList(int.MinValue, int.MinValue, kinds, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue, descending);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="sinceId">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="maxId">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="descending">Should the results in in descending order?</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(List<TransactionType> kinds, int sinceId, int maxId, bool descending = true)
        {
            return GetTransactionList(int.MinValue, int.MinValue, kinds, sinceId, maxId, DateTime.MinValue, DateTime.MinValue, descending);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="sinceId">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="maxId">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="sinceDate">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="untilDate">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <param name="descending">Should the results in in descending order?</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(List<TransactionType> kinds, int sinceId, int maxId, DateTime sinceDate, DateTime untilDate, bool descending = true)
        {
            return GetTransactionList(int.MinValue, int.MinValue, kinds, sinceId, maxId, sinceDate, untilDate, descending);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="perPage">The number of results per page (used for pagination)</param>
        /// <param name="descending">Should the results in in descending order?</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(int page, int perPage, bool descending = true)
        {
            return GetTransactionList(page, perPage, null, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue, descending);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="perPage">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="descending">Should the results in in descending order?</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(int page, int perPage, List<TransactionType> kinds, bool descending = true)
        {
            return GetTransactionList(page, perPage, kinds, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue, descending);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="perPage">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="sinceId">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="maxId">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="descending">Should the results in in descending order?</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(int page, int perPage, List<TransactionType> kinds, int sinceId, int maxId, bool descending = true)
        {
            return GetTransactionList(page, perPage, kinds, sinceId, maxId, DateTime.MinValue, DateTime.MinValue, descending);
        }

        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="perPage">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="sinceId">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="maxId">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="sinceDate">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="untilDate">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <param name="descending">Should the results in in descending order?</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionList(int page, int perPage, List<TransactionType> kinds, int sinceId, int maxId, DateTime sinceDate, DateTime untilDate, bool descending = true)
        {
            string qs = string.Empty;

            // Add the transaction options to the query string ...
            if (page != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", page); }
            if (perPage != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("per_page={0}", perPage); }

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

            if (sinceId != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("since_id={0}", sinceId); }
            if (maxId != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("max_id={0}", maxId); }
            if (sinceDate != DateTime.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("since_date={0}", sinceDate.ToString(DateTimeFormat)); }
            if (untilDate != DateTime.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("until_date={0}", untilDate.ToString(DateTimeFormat)); }
            if (qs.Length > 0) { qs += "&"; }
            qs += "direction=" + (descending ? "desc" : "asc");

            // Construct the url to access Chargify
            string url = string.Format("transactions.{0}", GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = DoRequest(url);

            var retValue = new Dictionary<int, ITransaction>();
            if (response.IsXml())
            {
                // now build a transaction list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "transactions")
                    {
                        foreach (XmlNode transactionNode in elementNode.ChildNodes)
                        {
                            if (transactionNode.Name == "transaction")
                            {
                                ITransaction loadedTransaction = new Transaction(transactionNode);
                                if (!retValue.ContainsKey(loadedTransaction.ID))
                                {
                                    retValue.Add(loadedTransaction.ID, loadedTransaction);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("transaction"))
                    {
                        JsonObject transactionObj = (array.Items[i] as JsonObject)["transaction"] as JsonObject;
                        ITransaction loadedTransaction = new Transaction(transactionObj);
                        if (!retValue.ContainsKey(loadedTransaction.ID))
                        {
                            retValue.Add(loadedTransaction.ID, loadedTransaction);
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
        /// <param name="subscriptionId">The subscriptionID to get a list of transactions for</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int subscriptionId)
        {
            if (subscriptionId < 0) throw new ArgumentNullException("subscriptionId");

            return GetTransactionsForSubscription(subscriptionId, int.MinValue, int.MinValue, null, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscriptionID to get a list of transactions for</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int subscriptionId, List<TransactionType> kinds)
        {
            return GetTransactionsForSubscription(subscriptionId, int.MinValue, int.MinValue, kinds, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscriptionID to get a list of transactions for</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="sinceId">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="maxId">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int subscriptionId, List<TransactionType> kinds, int sinceId, int maxId)
        {
            return GetTransactionsForSubscription(subscriptionId, int.MinValue, int.MinValue, kinds, sinceId, maxId, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscriptionID to get a list of transactions for</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="sinceId">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="maxId">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="sinceDate">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="untilDate">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int subscriptionId, List<TransactionType> kinds, int sinceId, int maxId, DateTime sinceDate, DateTime untilDate)
        {
            return GetTransactionsForSubscription(subscriptionId, int.MinValue, int.MinValue, kinds, sinceId, maxId, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="perPage">The number of results per page (used for pagination)</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int subscriptionId, int page, int perPage)
        {
            return GetTransactionsForSubscription(subscriptionId, page, perPage, null, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="perPage">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int subscriptionId, int page, int perPage, List<TransactionType> kinds)
        {
            return GetTransactionsForSubscription(subscriptionId, page, perPage, kinds, int.MinValue, int.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="perPage">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="sinceId">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="maxId">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int subscriptionId, int page, int perPage, List<TransactionType> kinds, int sinceId, int maxId)
        {
            return GetTransactionsForSubscription(subscriptionId, page, perPage, kinds, sinceId, maxId, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="perPage">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="sinceId">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="maxId">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="sinceDate">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="untilDate">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        public IDictionary<int, ITransaction> GetTransactionsForSubscription(int subscriptionId, int page, int perPage, List<TransactionType> kinds, int sinceId, int maxId, DateTime sinceDate, DateTime untilDate)
        {
            string qs = string.Empty;

            if (page != int.MinValue)
            {
                if (qs.Length > 0) { qs += "&"; }
                qs += string.Format("page={0}", page);
            }

            if (perPage != int.MinValue)
            {
                if (qs.Length > 0) { qs += "&"; }
                qs += string.Format("per_page={0}", perPage);
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

            if (sinceId != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("since_id={0}", sinceId); }
            if (maxId != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("max_id={0}", maxId); }
            if (sinceDate != DateTime.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("since_date={0}", sinceDate.ToString(DateTimeFormat)); }
            if (untilDate != DateTime.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("until_date={0}", untilDate.ToString(DateTimeFormat)); }

            // now make the request
            string url = string.Format("subscriptions/{0}/transactions.{1}", subscriptionId, GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = DoRequest(url);
            var retValue = new Dictionary<int, ITransaction>();
            if (response.IsXml())
            {
                // now build a transaction list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response); // get the XML into an XML document
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "transactions")
                    {
                        foreach (XmlNode transactionNode in elementNode.ChildNodes)
                        {
                            if (transactionNode.Name == "transaction")
                            {
                                ITransaction loadedTransaction = new Transaction(transactionNode);
                                if (!retValue.ContainsKey(loadedTransaction.ID))
                                {
                                    retValue.Add(loadedTransaction.ID, loadedTransaction);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("transaction"))
                    {
                        JsonObject transactionObj = (array.Items[i] as JsonObject)["transaction"] as JsonObject;
                        ITransaction loadedTransaction = new Transaction(transactionObj);
                        if (!retValue.ContainsKey(loadedTransaction.ID))
                        {
                            retValue.Add(loadedTransaction.ID, loadedTransaction);
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
        /// <param name="id">The ID of the transaction</param>
        /// <returns>The transaction with the specified ID</returns>
        public ITransaction LoadTransaction(int id)
        {
            try
            {
                // make sure data is valid
                if (id < 0) throw new ArgumentNullException("id");
                // now make the request
                string response = DoRequest(string.Format("transactions/{0}.{1}", id, GetMethodExtension()));
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
        /// <param name="subscriptionId">The ID of the subscription to modify</param>
        /// <param name="paymentId">The ID of the payment that the credit will be applied to</param>
        /// <param name="amount">The amount (in dollars and cents) like 10.00 is $10.00</param>
        /// <param name="memo">A helpful explanation for the refund.</param>
        /// <returns>The IRefund object indicating successful, or unsuccessful.</returns>
        public IRefund CreateRefund(int subscriptionId, int paymentId, decimal amount, string memo)
        {
            int amountInCents = Convert.ToInt32(amount * 100);
            return CreateRefund(subscriptionId, paymentId, amountInCents, memo);
        }

        /// <summary>
        /// Create a refund
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to modify</param>
        /// <param name="paymentId">The ID of the payment that the credit will be applied to</param>
        /// <param name="amountInCents">The amount (in cents only) like 100 is $1.00</param>
        /// <param name="memo">A helpful explanation for the refund.</param>
        /// <returns>The IRefund object indicating successful, or unsuccessful.</returns>
        public IRefund CreateRefund(int subscriptionId, int paymentId, int amountInCents, string memo)
        {
            if (amountInCents < 0) throw new ArgumentNullException("amountInCents");
            if (string.IsNullOrEmpty(memo)) throw new ArgumentException("Can't have an empty memo", "memo");
            // make sure that the SubscriptionID is unique
            if (LoadSubscription(subscriptionId) == null) throw new ArgumentException("Not an SubscriptionID", "subscriptionId");
            // create XML for addition of refund
            StringBuilder refundXml = new StringBuilder(GetXmlStringIfApplicable());
            refundXml.Append("<refund>");
            refundXml.AppendFormat("<payment_id>{0}</payment_id>", paymentId);
            refundXml.AppendFormat("<amount_in_cents>{0}</amount_in_cents>", amountInCents);
            refundXml.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo));
            refundXml.Append("</refund>");
            string response = DoRequest(string.Format("subscriptions/{0}/refunds.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, refundXml.ToString());
            // change the response to the object            
            return response.ConvertResponseTo<Refund>("refund");
        }
        #endregion

        #region Statements
        /// <summary>
        /// Method for getting a specific statement
        /// </summary>
        /// <param name="statementId">The ID of the statement to retrieve</param>
        /// <returns>The statement if found, null otherwise.</returns>
        public IStatement LoadStatement(int statementId)
        {
            try
            {
                // make sure data is valid
                if (statementId <= 0) throw new ArgumentNullException("statementId");
                // now make the request
                string response = DoRequest(string.Format("statements/{0}.{1}", statementId, GetMethodExtension()));
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
        /// <param name="statementId">The ID of the statement to retrieve the byte[] for</param>
        /// <returns>A byte[] of the PDF data, to be sent to the user in a download</returns>
        public byte[] LoadStatementPDF(int statementId)
        {
            try
            {
                // make sure data is valid
                if (statementId <= 0) throw new ArgumentNullException("statementId");

                // now make the request
                byte[] response = DoFileRequest(string.Format("statements/{0}.pdf", statementId), HttpRequestMethod.Get, string.Empty);

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
        /// <param name="subscriptionId">The ID of the subscription to retrieve the statements for</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        public IList<int> GetStatementIDs(int subscriptionId)
        {
            return GetStatementIDs(subscriptionId, int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Method for getting a list of statment ids for a specific subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to retrieve the statements for</param>
        /// <param name="page">The page number to return</param>
        /// <param name="perPage">The number of results to return per page</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        public IList<int> GetStatementIDs(int subscriptionId, int page, int perPage)
        {
            string qs = string.Empty;

            // Add the transaction options to the query string ...
            if (page != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", page); }
            if (perPage != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("per_page={0}", perPage); }

            // Construct the url to access Chargify
            string url = string.Format("subscriptions/{0}/statements/ids.{1}", subscriptionId, GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = DoRequest(url);

            var retValue = new List<int>();
            if (response.IsXml())
            {
                // now build a statement list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "statement_ids")
                    {
                        foreach (XmlNode statementIdNode in elementNode.ChildNodes)
                        {
                            if (statementIdNode.Name == "id")
                            {
                                int statementId = Convert.ToInt32(statementIdNode.InnerText);
                                if (!retValue.Contains(statementId))
                                {
                                    retValue.Add(statementId);
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
                JsonObject statementIdsObj = JsonObject.Parse(response);
                if (!statementIdsObj.ContainsKey("statement_ids"))
                    throw new InvalidOperationException("Returned JSON not valid");

                JsonArray jsonArray = (statementIdsObj["statement_ids"]) as JsonArray;
                if (jsonArray != null)
                {
                    for (int i = 0; i <= jsonArray.Length - 1; i++)
                    {
                        JsonNumber statementIdValue = jsonArray.Items[i] as JsonNumber;
                        if (statementIdValue == null)
                            throw new InvalidOperationException("Statement ID is not a valid number");
                        if (!retValue.Contains(statementIdValue.IntValue))
                        {
                            retValue.Add(statementIdValue.IntValue);
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

        /// <summary>
        /// Method for getting a list of statments for a specific subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to retrieve the statements for</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        public IDictionary<int, IStatement> GetStatementList(int subscriptionId)
        {
            return GetStatementList(subscriptionId, int.MinValue, int.MinValue);
        }

        /// <summary>
        /// Method for getting a list of statments for a specific subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to retrieve the statements for</param>
        /// <param name="page">The page number to return</param>
        /// <param name="perPage">The number of results to return per page</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        public IDictionary<int, IStatement> GetStatementList(int subscriptionId, int page, int perPage)
        {
            string qs = string.Empty;

            // Add the transaction options to the query string ...
            if (page != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("page={0}", page); }
            if (perPage != int.MinValue) { if (qs.Length > 0) { qs += "&"; } qs += string.Format("per_page={0}", perPage); }

            // Construct the url to access Chargify
            string url = string.Format("subscriptions/{0}/statements.{1}", subscriptionId, GetMethodExtension());
            if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }
            string response = DoRequest(url);

            var retValue = new Dictionary<int, IStatement>();
            if (response.IsXml())
            {
                // now build a statement list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node
                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "statements")
                    {
                        foreach (XmlNode statementNode in elementNode.ChildNodes)
                        {
                            if (statementNode.Name == "statement")
                            {
                                IStatement loadedStatement = new Statement(statementNode);
                                if (!retValue.ContainsKey(loadedStatement.ID))
                                {
                                    retValue.Add(loadedStatement.ID, loadedStatement);
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("statement"))
                    {
                        JsonObject statementObj = (array.Items[i] as JsonObject)["statement"] as JsonObject;
                        IStatement loadedStatement = new Statement(statementObj);
                        if (!retValue.ContainsKey(loadedStatement.ID))
                        {
                            retValue.Add(loadedStatement.ID, loadedStatement);
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
            string response = DoRequest("stats.json");
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
        /// <param name="subscriptionId">The ID of the subscription to adjust</param>
        /// <param name="amount">The amount (in dollars and cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        public IAdjustment CreateAdjustment(int subscriptionId, decimal amount, string memo)
        {
            return CreateAdjustment(subscriptionId, amount, int.MinValue, memo, AdjustmentMethod.Default);
        }

        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to adjust</param>
        /// <param name="amount">The amount (in dollars and cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <param name="method">A string that toggles how the adjustment should be applied</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        public IAdjustment CreateAdjustment(int subscriptionId, decimal amount, string memo, AdjustmentMethod method)
        {
            return CreateAdjustment(subscriptionId, amount, int.MinValue, memo, method);
        }

        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to adjust</param>
        /// <param name="amountInCents">The amount (in cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        public IAdjustment CreateAdjustment(int subscriptionId, int amountInCents, string memo)
        {
            return CreateAdjustment(subscriptionId, decimal.MinValue, amountInCents, memo, AdjustmentMethod.Default);
        }

        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to adjust</param>
        /// <param name="amountInCents">The amount (in cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <param name="method">A string that toggles how the adjustment should be applied</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        public IAdjustment CreateAdjustment(int subscriptionId, int amountInCents, string memo, AdjustmentMethod method)
        {
            return CreateAdjustment(subscriptionId, decimal.MinValue, amountInCents, memo, method);
        }

        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to adjust</param>
        /// <param name="amount">The amount (in dollars and cents)</param>
        /// <param name="amountInCents">The amount (in cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <param name="method">A string that toggles how the adjustment should be applied</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        private IAdjustment CreateAdjustment(int subscriptionId, decimal amount, int amountInCents, string memo, AdjustmentMethod method)
        {
            int valueInCents = 0;
            if (amount == decimal.MinValue) valueInCents = amountInCents;
            if (amountInCents == int.MinValue) valueInCents = Convert.ToInt32(amount * 100);
            if (valueInCents == int.MinValue) valueInCents = 0;
            decimal value = Convert.ToDecimal(valueInCents / 100.0);

            // make sure data is valid
            if (string.IsNullOrEmpty(memo)) throw new ArgumentNullException(nameof(memo));
            // make sure that the SubscriptionID is unique
            if (LoadSubscription(subscriptionId) == null) throw new ArgumentException("Not an SubscriptionID", "subscriptionId");
            // create XML for creation of an adjustment
            StringBuilder adjustmentXml = new StringBuilder(GetXmlStringIfApplicable());
            adjustmentXml.Append("<adjustment>");
            adjustmentXml.AppendFormat("<amount>{0}</amount>", value.ToChargifyCurrencyFormat());
            adjustmentXml.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo));
            if (method != AdjustmentMethod.Default) { adjustmentXml.AppendFormat("<adjustment_method>{0}</adjustment_method>", method.ToString().ToLowerInvariant()); }
            adjustmentXml.Append("</adjustment>");
            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/adjustments.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, adjustmentXml.ToString());
            // change the response to the object
            return response.ConvertResponseTo<Adjustment>("adjustment");
        }
        #endregion

        #region Billing Portal
        /// <summary>
        /// From http://docs.chargify.com/api-billing-portal
        /// </summary>
        public IBillingManagementInfo GetManagementLink(int chargifyId)
        {
            try
            {
                // make sure data is valid
                if (chargifyId < 0) throw new ArgumentNullException("chargifyId");

                // now make the request
                string response = DoRequest(string.Format("portal/customers/{0}/management_link.{1}", chargifyId, GetMethodExtension()));

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
            string response = DoRequest(url);
            var retValue = new Dictionary<int, Invoice>();
            if (response.IsXml())
            {
                // now build an invoice list based on response XML
                retValue = GetListedXmlResponse<Invoice>("invoice", response);
            }
            else if (response.IsJSON())
            {
                // now build an invoice list based on response JSON
                retValue = GetListedJsonResponse<Invoice>("invoice", response);
            }
            return retValue;
        }
        #endregion

        #region Sites
        /// <summary>
        /// Clean up a site in test mode.
        /// </summary>
        /// <param name="cleanupScope">What should be cleaned? DEFAULT IS CUSTOMERS ONLY.</param>
        /// <returns>True if complete, false otherwise</returns>
        /// <remarks>If used against a production site, the result will always be false.</remarks>
        public bool ClearTestSite(SiteCleanupScope? cleanupScope = SiteCleanupScope.Customers)
        {
            bool retVal = false;

            try
            {
                var qs = string.Empty;

                if (cleanupScope != null)
                {
                    var cleanupScopeName = Enum.GetName(typeof(SiteCleanupScope), cleanupScope.Value);
                    if (cleanupScopeName != null)
                        qs += string.Format("cleanup_scope={0}", cleanupScopeName.ToLowerInvariant());
                }
                string url = string.Format("sites/clear_data.{0}", GetMethodExtension());
                if (!string.IsNullOrEmpty(qs)) { url += "?" + qs; }

                DoRequest(url, HttpRequestMethod.Post, null);

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
        /// <param name="subscriptionId">The ID of the subscription to apply this manual payment record to</param>
        /// <param name="amount">The decimal amount of the payment (ie. 10.00 for $10)</param>
        /// <param name="memo">The memo to include with the manual payment</param>
        /// <returns>The payment result, null otherwise.</returns>
        public IPayment AddPayment(int subscriptionId, decimal amount, string memo)
        {
            return AddPayment(subscriptionId, Convert.ToInt32(amount * 100), memo);
        }

        /// <summary>
        /// Chargify allows you to record payments that occur outside of the normal flow of payment processing.
        /// These payments are considered external payments.A common case to apply such a payment is when a 
        /// customer pays by check or some other means for their subscription.
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to apply this manual payment record to</param>
        /// <param name="amountInCents">The amount in cents of the payment (ie. $10 would be 1000 cents)</param>
        /// <param name="memo">The memo to include with the manual payment</param>
        /// <returns>The payment result, null otherwise.</returns>
        public IPayment AddPayment(int subscriptionId, int amountInCents, string memo)
        {
            // make sure data is valid
            if (string.IsNullOrEmpty(memo)) throw new ArgumentNullException("memo");
            // make sure that the SubscriptionID is unique
            if (LoadSubscription(subscriptionId) == null) throw new ArgumentException("Not an SubscriptionID", "subscriptionId");

            // create XML for creation of a payment
            var paymentXml = new StringBuilder(GetXmlStringIfApplicable());
            paymentXml.Append("<payment>");
            paymentXml.AppendFormat("<amount_in_cents>{0}</amount_in_cents>", amountInCents);
            paymentXml.AppendFormat("<memo>{0}</memo>", HttpUtility.HtmlEncode(memo));
            paymentXml.Append("</payment>");

            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/payments.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, paymentXml.ToString());

            // change the response to the object
            return response.ConvertResponseTo<Payment>("payment");
        }

        #endregion

        #region Payment Profiles
        /// <summary>
        /// Retrieve a payment profile
        /// </summary>
        /// <param name="id">The ID of the payment profile</param>
        /// <returns>The payment profile, null if not found.</returns>
        public IPaymentProfileView LoadPaymentProfile(int id)
        {
            try
            {
                // make sure data is valid
                if (id < 0) throw new ArgumentNullException("id");

                // now make the request
                string response = DoRequest(string.Format("payment_profiles/{0}.{1}", id, GetMethodExtension()));

                // Convert the Chargify response into the object we're looking for
                return response.ConvertResponseTo<PaymentProfileView>("payment_profile");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Updates a payment profile
        /// </summary>
        /// <param name="paymentProfile">The payment profile object</param>
        /// <returns>The updated payment profile if successful, null or exception otherwise.</returns>
        public IPaymentProfileView UpdatePaymentProfile(PaymentProfileView paymentProfile)
        {
            try
            {
                if (paymentProfile.Id < 0) throw new ArgumentException("PaymentProfileID");
                var xml = new StringBuilder(GetXmlStringIfApplicable());
                xml.Append("<payment_profile>");
                if (!string.IsNullOrWhiteSpace(paymentProfile.BillingAddress)) xml.AppendFormat("<billing_address>{0}</billing_address>", paymentProfile.BillingAddress);
                if (!string.IsNullOrWhiteSpace(paymentProfile.BillingAddress2)) xml.AppendFormat("<billing_address_2>{0}</billing_address_2>", paymentProfile.BillingAddress2);
                if (!string.IsNullOrWhiteSpace(paymentProfile.BillingCity)) xml.AppendFormat("<billing_city>{0}</billing_city>", paymentProfile.BillingCity);
                if (!string.IsNullOrWhiteSpace(paymentProfile.BillingCountry)) xml.AppendFormat("<billing_country>{0}</billing_country>", paymentProfile.BillingCountry);
                if (!string.IsNullOrWhiteSpace(paymentProfile.BillingState)) xml.AppendFormat("<billing_state>{0}</billing_state>", paymentProfile.BillingState);
                if (!string.IsNullOrWhiteSpace(paymentProfile.BillingZip)) xml.AppendFormat("<billing_zip>{0}</billing_zip>", paymentProfile.BillingZip);
                if (paymentProfile.CustomerID != int.MinValue) xml.AppendFormat("<customer_id>{0}</customer_id>", paymentProfile.CustomerID);
                if (!string.IsNullOrWhiteSpace(paymentProfile.FirstName)) xml.AppendFormat("<first_name>{0}</first_name>", paymentProfile.FirstName);
                if (!string.IsNullOrWhiteSpace(paymentProfile.LastName)) xml.AppendFormat("<last_name>{0}</last_name>", paymentProfile.LastName);
                var paymentTypeName = Enum.GetName(typeof(PaymentProfileType), paymentProfile.PaymentType);
                if (paymentTypeName != null)
                    xml.AppendFormat("<payment_type>{0}</payment_type>", paymentTypeName.ToLowerInvariant());
                if (paymentProfile.PaymentType == PaymentProfileType.Credit_Card)
                {
                    if (!string.IsNullOrWhiteSpace(paymentProfile.CardType)) xml.AppendFormat("<card_type>{0}</card_type>", paymentProfile.CardType);
                    if (!string.IsNullOrWhiteSpace(paymentProfile.FullNumber)) xml.AppendFormat("<full_number>{0}</full_number>", paymentProfile.FullNumber);
                    if (paymentProfile.ExpirationMonth != int.MinValue) xml.AppendFormat("<expiration_month>{0}</expiration_month>", paymentProfile.ExpirationMonth);
                    if (paymentProfile.ExpirationYear != int.MinValue) xml.AppendFormat("<expiration_year>{0}</expiration_year>", paymentProfile.ExpirationYear);
                }
                else if (paymentProfile.PaymentType == PaymentProfileType.Bank_Account)
                {
                    var bankAccountHolderTypeName = Enum.GetName(typeof(BankAccountHolderType), paymentProfile.BankAccountHolderType);
                    if (bankAccountHolderTypeName != null)
                        xml.AppendFormat("<bank_account_holder_type>{0}</bank_account_holder_type>", bankAccountHolderTypeName.ToLowerInvariant());
                    var bankAccountTypeName = Enum.GetName(typeof(BankAccountType), paymentProfile.BankAccountType);
                    if (bankAccountTypeName != null)
                        xml.AppendFormat("<bank_account_type>{0}</bank_account_type>", bankAccountTypeName.ToLowerInvariant());
                    if (!string.IsNullOrWhiteSpace(paymentProfile.BankName)) xml.AppendFormat("<bank_name>{0}</bank_name>", paymentProfile.BankName);
                    if (!string.IsNullOrWhiteSpace(paymentProfile.BankRoutingNumber)) xml.AppendFormat("<bank_routing_number>{0}</bank_routing_number>", paymentProfile.BankRoutingNumber);
                    if (!string.IsNullOrWhiteSpace(paymentProfile.BankAccountNumber)) xml.AppendFormat("<bank_account_number>{0}</bank_account_number>", paymentProfile.BankAccountNumber);
                }
                xml.Append("</payment_profile>");
                string response = DoRequest(string.Format("payment_profiles/{0}.{1}", paymentProfile.Id, GetMethodExtension()), HttpRequestMethod.Put, xml.ToString());
                return response.ConvertResponseTo<PaymentProfileView>("payment_profile");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }
        #endregion

        #region Renewal Preview
        /// <summary>
        /// Renewal Preview is an object representing a subscription’s next assessment. 
        /// You can retrieve it to see a snapshot of how much your customer will be charged on their next renewal.
        /// </summary>
        /// <param name="subscriptionId">Integer, the id for the subscription that is to be previewed</param>
        /// <returns>The snapshot of how much your customer will be charged on their next renewal</returns>
        public IRenewalDetails PreviewRenewal(int subscriptionId)
        {
            // now make the request
            string response = DoRequest($"/subscriptions/{subscriptionId}/renewals/preview.{GetMethodExtension()}", HttpRequestMethod.Post, null);
            // change the response to the object
            return response.ConvertResponseTo<RenewalDetails>("renewal_preview");
        }
        #endregion

        #region Notes
        /// <summary>
        /// Create a note
        /// </summary>
        /// <param name="note">The note to create</param>
        /// <returns></returns>
        public INote CreateNote(INote note)
        {
            return CreateNote(note.SubscriptionID, note.Body, note.Sticky);
        }

        public INote CreateNote(int subscriptionId, string body, bool sticky = false)
        {
            // make sure data is valid
            if (subscriptionId < 0) throw new ArgumentNullException(nameof(subscriptionId));

            // create XML for creation of note
            var noteXml = new StringBuilder(GetXmlStringIfApplicable());
            noteXml.Append("<note>");
            noteXml.AppendFormat("<body>{0}</body>", body);
            noteXml.AppendFormat("<sticky>{0}</sticky>", sticky);
            noteXml.Append("</note>");

            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/notes.{1}", subscriptionId, GetMethodExtension()), HttpRequestMethod.Post, noteXml.ToString());

            // change the response to the object
            return response.ConvertResponseTo<Note>("note");
        }

        /// <summary>
        /// Retrieve the notes 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public IDictionary<int, INote> GetNotesForSubscription(int subscriptionId)
        {
            // make sure data is valid
            if (subscriptionId == int.MinValue) throw new ArgumentNullException(nameof(subscriptionId));

            // now make the request
            string response = DoRequest(string.Format("subscriptions/{0}/notes.{1}", subscriptionId, GetMethodExtension()));
            var retValue = new Dictionary<int, INote>();
            if (response.IsXml())
            {
                // now build a product list based on response XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response); // get the XML into an XML document
                if (doc.ChildNodes.Count == 0) throw new InvalidOperationException("Returned XML not valid");
                // loop through the child nodes of this node

                foreach (XmlNode elementNode in doc.ChildNodes)
                {
                    if (elementNode.Name == "notes")
                    {
                        foreach (XmlNode noteNode in elementNode.ChildNodes)
                        {
                            if (noteNode.Name == "note")
                            {
                                INote loadedNote = new Note(noteNode);
                                if (!retValue.ContainsKey(loadedNote.ID))
                                {
                                    retValue.Add(loadedNote.ID, loadedNote);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Duplicate Note ID values detected");
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
                    var jsonObject = array.Items[i] as JsonObject;
                    if (jsonObject != null && jsonObject.ContainsKey("note"))
                    {
                        JsonObject componentObj = (array.Items[i] as JsonObject)["note"] as JsonObject;
                        INote loadedComponent = new Note(componentObj);
                        if (!retValue.ContainsKey(loadedComponent.ID))
                        {
                            retValue.Add(loadedComponent.ID, loadedComponent);
                        }
                        else
                        {
                            throw new InvalidOperationException("Duplicate Note ID values detected");
                        }
                    }
                }
            }
            // return the dictionary
            return retValue;
        }

        /// <summary>
        /// Load the note
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription</param>
        /// <param name="noteId">The id of the note to retrieve</param>
        /// <returns></returns>
        public INote LoadNote(int subscriptionId, int noteId)
        {
            try
            {
                // make sure data is valid
                if (subscriptionId == int.MinValue) throw new ArgumentNullException(nameof(subscriptionId));
                // now make the request
                string response = DoRequest(string.Format("subscriptions/{0}/notes/{1}.{2}", subscriptionId, noteId, GetMethodExtension()));
                // change the response to the object
                return response.ConvertResponseTo<Note>("note");
            }
            catch (ChargifyException cex)
            {
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        /// <summary>
        /// Deletes a note from a subscription
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription</param>
        /// <param name="noteId">The id of the note to delete</param>
        /// <returns></returns>
        public bool DeleteNote(int subscriptionId, int noteId)
        {
            try
            {
                // make sure data is valid
                if (subscriptionId < 0) throw new ArgumentNullException(nameof(subscriptionId));

                // now make the request
                DoRequest(string.Format("subscriptions/{0}/notes/{1}.{2}", subscriptionId, noteId, GetMethodExtension()), HttpRequestMethod.Delete, string.Empty);
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
        /// Updates a note
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription</param>
        /// <param name="note">The updated note</param>
        /// <returns>Returns the updated note, or the same note if unsuccessful</returns>
        public INote UpdateNote(int subscriptionId, INote note)
        {
            // make sure data is OK
            if (note == null) throw new ArgumentNullException(nameof(note));
            if (note.ID == int.MinValue) throw new ArgumentException("Invalid chargify ID detected", nameof(note));
            INote oldNote = LoadNote(note.SubscriptionID, note.ID);

            bool isUpdateRequired = false;

            // create XML for updating of note
            var customerXml = new StringBuilder(GetXmlStringIfApplicable());
            customerXml.Append("<note>");
            if (oldNote != null)
            {
                if (oldNote.SubscriptionID != note.SubscriptionID) throw new ArgumentException("Subscription IDs do not match", nameof(note));
                if (oldNote.Body != note.Body) { customerXml.AppendFormat("<body>{0}</body>", HttpUtility.HtmlEncode(note.Body)); isUpdateRequired = true; }
                if (oldNote.Sticky != note.Sticky) { customerXml.AppendFormat("<sticky>{0}</sticky>", note.Sticky ? 1 : 0); isUpdateRequired = true; }
            }
            customerXml.Append("</note>");

            if (isUpdateRequired)
            {
                try
                {
                    // now make the request
                    string response = DoRequest(string.Format("subscriptions/{0}/notes/{1}.{2}", subscriptionId, note.ID, GetMethodExtension()), HttpRequestMethod.Put, customerXml.ToString());
                    // change the response to the object
                    return response.ConvertResponseTo<Note>("note");
                }
                catch (ChargifyException cex)
                {
                    if (cex.StatusCode == HttpStatusCode.NotFound) throw new InvalidOperationException("Customer not found");
                    throw;
                }
            }
            return note;
        }
        #endregion

        #region Referral Code

        /// <summary>
        /// Method for retrieving information about a coupon using the ID of that referral code.
        /// </summary>
        /// <param name="ReferralCode">Referral code</param>
        /// <returns>The object if found, null otherwise.</returns>
        public IReferralCode ValidateReferralCode(string ReferralCode)
        {
            try
            {
                string response = this.DoRequest(string.Format("referral_codes/validate.{0}?code={1}", GetMethodExtension(), ReferralCode));
                // change the response to the object
                return response.ConvertResponseTo<ReferralCode>("referral-code");
            }
            catch (ChargifyException cex)
            {
                // Throw if anything but not found, since not found is telling us that it's working correctly
                // but that there just isn't a referral code with that code.
                if (cex.StatusCode == HttpStatusCode.NotFound) return null;
                throw cex;
            }
        }
        #endregion

        #region Utility Methods
        private Dictionary<int, T> GetListedJsonResponse<T>(string key, string response)
            where T : class, IChargifyEntity
        {
            var retValue = Activator.CreateInstance<Dictionary<int, T>>();

            // should be expecting an array
            int position = 0;
            JsonArray array = JsonArray.Parse(response, ref position);
            for (int i = 0; i <= array.Length - 1; i++)
            {
                var jsonObject = array.Items[i] as JsonObject;
                if (jsonObject != null && jsonObject.ContainsKey(key))
                {
                    JsonObject jsonObj = (array.Items[i] as JsonObject)[key] as JsonObject;
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
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);
            if (doc.ChildNodes.Count == 0)
                throw new InvalidOperationException("Returned XML not valid");

            var retValue = Activator.CreateInstance<Dictionary<int, T>>();

            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
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
            return UseJSON ? "json" : "xml";
        }

        private string GetXmlStringIfApplicable()
        {
            string result = string.Empty;
            if (!UseJSON)
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
            if (string.IsNullOrEmpty(URL)) throw new InvalidOperationException("URL not set");
            if (string.IsNullOrEmpty(apiKey)) throw new InvalidOperationException("apiKey not set");
            if (string.IsNullOrEmpty(Password)) throw new InvalidOperationException("Password not set");

            if (_protocolType != null)
            {
                ServicePointManager.SecurityProtocol = _protocolType.Value;
            }

            // create the URI
            string addressString = string.Format("{0}{1}{2}", URL, (URL.EndsWith("/") ? "" : "/"), methodString);
            var uriBuilder = new UriBuilder(addressString)
            {
                Scheme = Uri.UriSchemeHttps,
                Port = -1 // default port for scheme
            };
            Uri address = uriBuilder.Uri;

            // Create the web request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.Timeout = 180000;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey + ":" + Password));
            request.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

            // Set type to POST
            request.Method = requestMethod.ToString().ToUpper();
            request.SendChunked = false;
            if (!UseJSON)
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
                    if (UseJSON)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(postData);
                        dataToPost = XmlToJsonConverter.XmlToJson(doc);
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
                    request.ContentLength = postData?.Length ?? 0;
                }
            }
            // request the data
            try
            {
                LogRequest?.Invoke(requestMethod, addressString, dataToPost);

                byte[] retValue = { };
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response == null) return retValue;

                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            using (var ms = new MemoryStream())
                            {
                                reader.BaseStream.CopyStream(ms);
                                retValue = ms.ToArray();
                            }
                            _lastResponse = response;
                        }
                    }

                    LogResponse?.Invoke(response.StatusCode, addressString, string.Empty);
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
            if (string.IsNullOrEmpty(URL)) throw new InvalidOperationException("URL not set");
            if (string.IsNullOrEmpty(apiKey)) throw new InvalidOperationException("apiKey not set");
            if (string.IsNullOrEmpty(Password)) throw new InvalidOperationException("Password not set");

            if (_protocolType != null)
            {
                ServicePointManager.SecurityProtocol = _protocolType.Value;
            }

            // create the URI
            string addressString = string.Format("{0}{1}{2}", URL, (URL.EndsWith("/") ? string.Empty : "/"), methodString);

            var uriBuilder = new UriBuilder(addressString)
            {
                Scheme = Uri.UriSchemeHttps,
                Port = -1 // default port for scheme
            };
            Uri address = uriBuilder.Uri;

            // Create the web request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.Timeout = _timeout;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey + ":" + Password));
            request.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
            request.UserAgent = UserAgent;
            request.SendChunked = false;

            // Set Content-Type and Accept headers
            request.Method = requestMethod.ToString().ToUpper();
            if (!UseJSON)
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
                    if (UseJSON)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(postData);
                        dataToPost = XmlToJsonConverter.XmlToJson(doc);
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
                    request.ContentLength = postData?.Length ?? 0;
                }
            }
            // request the data
            try
            {
                LogRequest?.Invoke(requestMethod, addressString, dataToPost);

                string retValue = string.Empty;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        var responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                retValue = reader.ReadToEnd();
                                _lastResponse = response;
                            }
                        }
                        LogResponse?.Invoke(response.StatusCode, addressString, retValue);
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