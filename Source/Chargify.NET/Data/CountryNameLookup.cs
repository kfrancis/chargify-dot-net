using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

namespace ChargifyNET.Data
{
    /// <summary>
    /// Since Chargify suggests using ISO 3166-1 Alpha 2 for the country codes, provide some data to users.
    /// </summary>
    public sealed class CountryNameLookup
    {
        private static XmlDocument countries;
        private const string countriesFilename = @"ISO_3166-1_list_en.xml";

        /// <summary>
        /// Constructor
        /// </summary>
        public CountryNameLookup()
        {
            countries = new XmlDocument();

            var resourceFileWithNamespace = string.Format("{0}.{1}", GetType().Namespace, countriesFilename);
            var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFileWithNamespace);

            if (fileStream != null)
            {
                using (var reader = new StreamReader(fileStream))
                {
                    countries.LoadXml(reader.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// Method for retrieving the country name from the 2 letter country code (as defined in the ISON 3166-1 Alpha 2 list)
        /// </summary>
        /// <param name="countryCode2">The two letter country code</param>
        /// <returns>Return the name of the country if applicable, String.Empty otherwise.</returns>
        public string GetCountryName(string countryCode2)
        {
            string result = String.Empty;
            var countryNode = countries.SelectSingleNode(string.Format(@"/ISO_3166-1_List_en/ISO_3166-1_Entry/ISO_3166-1_Alpha-2_Code_element[.=""{0}""]", countryCode2)).ParentNode;
            if (countryNode != null)
            {
                var countryName = countryNode.SelectSingleNode("ISO_3166-1_Country_name").InnerText.ToLower();
                result = char.ToUpper(countryName[0]) + countryName.Substring(1);
            }
            return result;
        }

        /// <summary>
        /// Method for getting a Dictionary of the country/codes listed in ISO 3166-1 Alpha 2 list
        /// http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2
        /// </summary>
        /// <returns>The dictionary of data in ISO 3166-1 Alpha 2</returns>
        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            XDocument doc = XDocument.Parse(countries.OuterXml);

            result = (from c in doc.Descendants("ISO_3166-1_Entry")
                      select new
                      {
                          Code = (string)c.Element("ISO_3166-1_Alpha-2_Code_element").Value,
                          Name = (string)c.Element("ISO_3166-1_Country_name").Value
                      }).ToDictionary(c => c.Code, c => c.Name);
            
            return result;
        }
    }
}
