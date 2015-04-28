using System;
using System.Linq;
using ChargifyNET;
#if NUNIT
using NUnit.Framework;
#else
using TestFixture = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using TestFixtureSetUp = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using SetUp = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace ChargifyDotNetTests.Base
{
    public class ChargifyTestBase
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        public ChargifyConnect Chargify
        {
            get
            {
                if (this._chargify == null)
                {
                    this._chargify = new ChargifyConnect();
                    this._chargify.apiKey = "";
                    this._chargify.Password = "X";
                    this._chargify.URL = "https://subdomain.chargify.com/";
                    this._chargify.SharedKey = "123456789";
                    this._chargify.UseJSON = false;
                }
                return this._chargify;
            }
        }
        private ChargifyConnect _chargify = null;

        internal void SetJson(bool useJson)
        {
            if (this.Chargify != null) {
                this._chargify.UseJSON = useJson;
            }
        }
    }
}
