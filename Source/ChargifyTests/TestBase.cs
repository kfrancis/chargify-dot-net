using Chargify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargifyTests
{
    public class TestBase
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        public ChargifyClient Chargify => _chargify ?? (_chargify = new ChargifyClient(apiKey: "", apiPassword: "", useJson: false));

        private ChargifyClient _chargify;
    }
}
