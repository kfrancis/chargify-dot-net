using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using ChargifyNET;
using Newtonsoft.Json;

namespace ChargifyDotNet.RequestDTOs
{
    internal class CustomerRequest
    {
        [JsonProperty("first_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; }
        [JsonProperty("last_name", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; }
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }
        [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
        public string Phone { get; set; }
        [JsonProperty("organization", NullValueHandling = NullValueHandling.Ignore)]
        public string Organization { get; set; }
        [JsonProperty("reference", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemID { get; set; }
        [JsonProperty("cc_emails", NullValueHandling = NullValueHandling.Ignore)]
        public string CCEmails { get; set; }
        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        public string ShippingAddress { get; set; }
        [JsonProperty("address_2", NullValueHandling = NullValueHandling.Ignore)]
        public string ShippingAddress2 { get; set; }
        [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
        public string ShippingCity { get; set; }
        [JsonProperty("zip", NullValueHandling = NullValueHandling.Ignore)]
        public string ShippingZip { get; set; }
        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string ShippingState { get; set; }
        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public string ShippingCountry { get; set; }
        [JsonProperty("tax_exempt", NullValueHandling = NullValueHandling.Ignore)]
        public bool TaxExempt { get; set; }

        #region XmlBuilders

        internal static string GetCustomerCreateXml(CustomerRequest customer)
        {
            var customerXml = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            customerXml.Append("<customer>");
            AppendXmlIfValid(customerXml, "<email>{0}</email>", customer.Email);
            AppendXmlIfValid(customerXml, "<{0}>{1}</{2}>", CustomerAttributes.PhoneKey, customer.Phone, CustomerAttributes.PhoneKey);
            AppendXmlIfValid(customerXml, "<first_name>{0}</first_name>", customer.FirstName);
            AppendXmlIfValid(customerXml, "<last_name>{0}</last_name>", customer.LastName);
            AppendXmlIfValid(customerXml, "<organization>{0}</organization>", customer.Organization);
            AppendXmlIfValid(customerXml, "<reference>{0}</reference>", customer.SystemID);
            AppendXmlIfValid(customerXml, "<cc_emails>{0}</cc_emails>", customer.CCEmails);
            AppendXmlIfValid(customerXml, "<address>{0}</address>", customer.ShippingAddress);
            AppendXmlIfValid(customerXml, "<address_2>{0}</address_2>", customer.ShippingAddress2);
            AppendXmlIfValid(customerXml, "<city>{0}</city>", customer.ShippingCity);
            AppendXmlIfValid(customerXml, "<state>{0}</state>", customer.ShippingState);
            AppendXmlIfValid(customerXml, "<zip>{0}</zip>", customer.ShippingZip);
            AppendXmlIfValid(customerXml, "<country>{0}</country>", customer.ShippingCountry);
            AppendXmlIfValid(customerXml, "<tax_exempt>{0}</tax_exempt>", customer.TaxExempt.ToString().ToLowerInvariant());
            customerXml.Append("</customer>");

            return customerXml.ToString();
        }

        private static void AppendXmlIfValid(StringBuilder stringBuilder, string nodeTemplate, params string[] values)
        {
#if NETCORE
            var encodedValues = values.Select(elem => HttpUtility.HtmlEncode(elem)).Cast<object>().ToArray();
#else
            var encodedValues = values.Select(elem => System.Net.WebUtility.HtmlEncode(elem)).Cast<object>().ToArray();
#endif
            if (values.All(elem => !string.IsNullOrWhiteSpace(elem))) stringBuilder.AppendFormat(nodeTemplate, encodedValues);
        }
        #endregion

    }
}
