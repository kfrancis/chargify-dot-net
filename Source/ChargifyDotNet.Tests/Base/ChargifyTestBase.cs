using System;
using ChargifyNET;
using System.Net;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests.Base
{
    public class ChargifyTestBase
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        public ChargifyConnect Chargify => _chargify ?? (_chargify = new ChargifyConnect
        {
            apiKey = "",
            Password = "X",
            URL = "https://subdomain.chargify.com/",
            SharedKey = "123456789",
            UseJSON = false,
            ProtocolType = SecurityProtocolType.Tls12
        });

        private ChargifyConnect _chargify;

        /// <summary>
        /// Method that allows me to use Faker methods in place rather than writing a bunch of specific "GetRandom.." methods.
        /// </summary>
        /// <param name="oldValue">The value that the result cannot be</param>
        /// <param name="generateValue">The method (that returns string) that will be used to generate the random value</param>
        /// <returns>A new random string value that isn't the same as the existing/old value</returns>
        public string GetNewRandomValue(string oldValue, Func<bool, string> generateValue, bool generateArg = false)
        {
            string retVal;
            do
            {
                retVal = generateValue(generateArg);
            }
            while (retVal == oldValue);
            return retVal;
        }

        /// <summary>
        /// Method that allows me to use Faker methods in place rather than writing a bunch of specific "GetRandom.." methods.
        /// </summary>
        /// <param name="oldValue">The value that the result cannot be</param>
        /// <param name="generateValue">The method (that returns string) that will be used to generate the random value</param>
        /// <returns>A new random string value that isn't the same as the existing/old value</returns>
        public string GetNewRandomValue(string oldValue, Func<string> generateValue)
        {
            string retVal;
            do
            {
                retVal = generateValue();
            }
            while (retVal == oldValue);
            return retVal;
        }

        /// <summary>
        /// Method that allows me to use Faker methods in place rather than writing a bunch of specific "GetRandom.." methods.
        /// </summary>
        /// <param name="oldValue">The value that the result cannot be</param>
        /// <param name="generateValue">The method (that returns string) that will be used to generate the random value</param>
        /// <returns>A new random string value that isn't the same as the existing/old value</returns>
        public string GetNewRandomValue(string oldValue, Func<string, string> generateValue, string generateArg = null)
        {
            string retVal;
            do
            {
                retVal = generateValue(generateArg);
            }
            while (retVal == oldValue);
            return retVal;
        }

        public static Faker Faker => new Faker("en");

        internal void SetJson(bool useJson)
        {
            if (Chargify != null) {
                _chargify.UseJSON = useJson;
            }
        }
    }
}
