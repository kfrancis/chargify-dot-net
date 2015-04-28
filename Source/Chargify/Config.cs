using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chargify.Configuration;

namespace Chargify
{
    /// <summary>
    /// TODO
    /// </summary>
    public class Config
    {
        /// <summary>
        /// The API consumer's username
        /// </summary>
        public static string ApiKey
        {
            get
            {
                var config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
                ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
                return accountInfo.ApiKey;
            }
        }

        /// <summary>
        /// The API consumer's password
        /// </summary>
        public static string ApiPassword
        {
            get
            {
                var config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
                ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
                return accountInfo.ApiPassword;
            }
        }

        /// <summary>
        /// The site's shared key (used for hosted pages)
        /// </summary>
        public static string SharedKey
        {
            get
            {
                var config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
                ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
                return accountInfo.SharedKey;
            }
        }

        /// <summary>
        /// Should we use json?
        /// </summary>
        public static bool UseJson
        {
            get
            {
                var config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
                ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
                return accountInfo.UseJSON;
            }
        }

        /// <summary>
        /// The base API URL
        /// </summary>
        public static string ApiBaseUrl { get; set; }

        static Config()
        {
            var config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
            ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
            ApiBaseUrl = string.Format("https://{0}.chargify.com", accountInfo.Subdomain);
        }
    }
}
